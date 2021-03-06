﻿namespace BaristaLabs.BaristaCore
{
    using BaristaLabs.BaristaCore.Extensions;
    using BaristaLabs.BaristaCore.Utils;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    public sealed class BaristaTypeConversionStrategy : IBaristaTypeConversionStrategy
    {
        private const string BaristaObjectPropertyName = "__baristaObject";
        private const string BaristaEventListenersPropertyName = "__baristaEventListeners";

        private IDictionary<Type, JsFunction> m_prototypes = new Dictionary<Type, JsFunction>();

        public bool TryCreatePrototypeFunction(BaristaContext context, Type typeToConvert, out JsFunction ctor)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (typeToConvert == null)
                throw new ArgumentNullException(nameof(typeToConvert));

            if (m_prototypes.ContainsKey(typeToConvert))
            {
                ctor = m_prototypes[typeToConvert];
                return true;
            }

            var reflector = new ObjectReflector(typeToConvert);

            JsFunction superCtor = null;
            var baseType = reflector.GetBaseType();
            if (baseType != null && !baseType.IsSameOrSubclass(typeof(JsValue)) && TryCreatePrototypeFunction(context, baseType, out JsFunction fnSuper))
            {
                superCtor = fnSuper;
            }

            var objectName = BaristaObjectAttribute.GetBaristaObjectNameFromType(typeToConvert);
            
            //Get all the property descriptors for the specified type.
            var staticPropertyDescriptors = context.CreateObject();
            var instancePropertyDescriptors = context.CreateObject();

            //Get static and instance properties.
            ProjectProperties(context, staticPropertyDescriptors, reflector.GetProperties(false));
            ProjectProperties(context, instancePropertyDescriptors, reflector.GetProperties(true));

            //Get static and instance indexer properties.
            ProjectIndexerProperties(context, staticPropertyDescriptors, reflector.GetIndexerProperties(false));
            ProjectIndexerProperties(context, instancePropertyDescriptors, reflector.GetIndexerProperties(true));

            //Get static and instance methods.
            ProjectMethods(context, staticPropertyDescriptors, reflector, reflector.GetUniqueMethodsByName(false));
            ProjectMethods(context, instancePropertyDescriptors, reflector, reflector.GetUniqueMethodsByName(true));

            //Get static and instance events.
            ProjectEvents(context, staticPropertyDescriptors, reflector, reflector.GetEventTable(false));
            ProjectEvents(context, instancePropertyDescriptors, reflector, reflector.GetEventTable(true));

            //Get the [[iterator]] property.
            ProjectIEnumerable(context, instancePropertyDescriptors, reflector);

            JsFunction fnCtor;
            var publicConstructors = reflector.GetConstructors();
            if (publicConstructors.Any())
            {
                fnCtor = context.CreateFunction(new BaristaFunctionDelegate((calleeObj, isConstructCall, thisObj, args) =>
                {
                    if (thisObj == null)
                    {
                        var ex = context.CreateTypeError($"Failed to construct '{objectName}': 'this' must be specified.");
                        context.CurrentScope.SetException(ex);
                        return context.Undefined;
                    }

                    var isParentConstructing = thisObj.HasProperty("__isConstructing");

                    if (superCtor != null)
                    {
                        if (!isParentConstructing)
                        {
                            thisObj.SetProperty("__isConstructing", context.True);
                            superCtor.Call(thisObj);
                            thisObj.DeleteProperty("__isConstructing");
                        }
                        else
                        {
                            superCtor.Call(thisObj);
                        }
                    }

                    context.Object.DefineProperties(thisObj, instancePropertyDescriptors);

                    //If this isn't a construct call, don't attempt to set the bean
                    //In 1.10.x, isConstructCall is true when calling the super's constructor
                    //when previously it was false. 
                    if (!isConstructCall || isParentConstructing)
                    {
                        return thisObj;
                    }

                    //Set our native object.
                    JsExternalObject externalObject = null;

                    //!!Special condition -- if there's exactly one argument, and if it matches the enclosing type,
                    //don't invoke the type's constructor, rather, just wrap the object with the JsObject.
                    if (args.Length == 1 && args[0].GetType() == typeToConvert)
                    {
                        externalObject = context.CreateExternalObject(args[0]);
                    }
                    else
                    {
                        try
                        {
                            var bestConstructor = reflector.GetConstructorBestMatch(args);
                            if (bestConstructor == null)
                            {
                                var ex = context.CreateTypeError($"Failed to construct '{objectName}': Could not find a matching constructor for the provided arguments.");
                                context.CurrentScope.SetException(ex);
                                return context.Undefined;
                            }

                            //Convert the args into the native args of the constructor.
                            var constructorParams = bestConstructor.GetParameters();
                            var convertedArgs = ConvertArgsToParamTypes(context, args, constructorParams);

                            var newObj = bestConstructor.Invoke(convertedArgs);
                            externalObject = context.CreateExternalObject(newObj);
                        }
                        catch (Exception ex)
                        {
                            context.CurrentScope.SetException(context.CreateError(ex.Message));
                            return context.Undefined;
                        }
                    }

                    thisObj.SetBean(externalObject);
                    
                    return thisObj;
                }), objectName);
            }
            else
            {
                fnCtor = context.CreateFunction(new BaristaFunctionDelegate((calleeObj, isConstructCall, thisObj, args) =>
                {
                    var ex = context.CreateTypeError($"Failed to construct '{objectName}': This object cannot be constructed.");
                    context.CurrentScope.SetException(ex);
                    return context.Undefined;
                }), objectName);
            }

            //We've got everything we need.
            fnCtor.Prototype = context.Object.Create(superCtor == null ? context.Object.Prototype : superCtor.Prototype);

            context.Object.DefineProperties(fnCtor, staticPropertyDescriptors);

            m_prototypes.Add(typeToConvert, fnCtor);
            ctor = fnCtor;
            return true;
        }

        private void ProjectProperties(BaristaContext context, JsObject targetObject, IEnumerable<PropertyInfo> properties)
        {
            foreach (var prop in properties)
            {
                var propertyAttribute = BaristaPropertyAttribute.GetAttribute(prop);
                var propertyName = propertyAttribute.Name;
                var propertyDescriptor = context.CreateObject();

                if (propertyAttribute.Configurable)
                    propertyDescriptor.SetProperty("configurable", context.True);
                if (propertyAttribute.Enumerable)
                    propertyDescriptor.SetProperty("enumerable", context.True);

                if (prop.GetMethod != null)
                {
                    var jsGet = context.CreateFunction(new BaristaFunctionDelegate((calleeObj, isConstructCall, thisObj, args) =>
                    {
                        return GetPropertyValue(context, prop, propertyName, thisObj);
                    }));

                    propertyDescriptor.SetProperty("get", jsGet);
                }

                if (prop.SetMethod != null)
                {
                    var jsSet = context.CreateFunction(new BaristaFunctionDelegate((calleeObj, isConstructCall, thisObj, args) =>
                    {
                        return SetPropertyValue(context, prop, propertyName, thisObj, args);
                    }));

                    propertyDescriptor.SetProperty("set", jsSet);
                }

                targetObject.SetProperty(context.CreateString(propertyName), propertyDescriptor);
            }
        }

        private void ProjectIndexerProperties(BaristaContext context, JsObject targetObject, IEnumerable<PropertyInfo> indexerProperties)
        {
            foreach (var indexerProp in indexerProperties)
            {
                var propertyAttribute = BaristaPropertyAttribute.GetAttribute(indexerProp);

                if (indexerProp.GetMethod != null)
                {
                    var propertyName = propertyAttribute.Name;
                    var propertyDescriptor = context.CreateObject();

                    if (propertyAttribute.Configurable)
                        propertyDescriptor.SetProperty("configurable", context.True);
                    if (propertyAttribute.Enumerable)
                        propertyDescriptor.SetProperty("enumerable", context.True);
                    if (propertyAttribute.Writable)
                        propertyDescriptor.SetProperty("writable", context.True);

                    var jsGetItemAt = context.CreateFunction(new BaristaFunctionDelegate((calleeObj, isConstructCall, thisObj, args) =>
                    {
                        return GetIndexerPropertyValue(context, indexerProp, propertyName, thisObj, args);
                    }));

                    propertyDescriptor.SetProperty("value", jsGetItemAt);
                    targetObject.SetProperty(context.CreateString("get" + propertyName), propertyDescriptor);
                }

                if (indexerProp.SetMethod != null)
                {
                    var propertyName = propertyAttribute.Name;
                    var propertyDescriptor = context.CreateObject();

                    if (propertyAttribute.Configurable)
                        propertyDescriptor.SetProperty("configurable", context.True);
                    if (propertyAttribute.Enumerable)
                        propertyDescriptor.SetProperty("enumerable", context.True);
                    if (propertyAttribute.Writable)
                        propertyDescriptor.SetProperty("writable", context.True);

                    var jsGetItemAt = context.CreateFunction(new BaristaFunctionDelegate((calleeObj, isConstructCall, thisObj, args) =>
                    {
                        return SetIndexerPropertyValue(context, indexerProp, propertyName, thisObj, args);
                    }));

                    propertyDescriptor.SetProperty("value", jsGetItemAt);
                    targetObject.SetProperty(context.CreateString("set" + propertyName), propertyDescriptor);
                }
            }
        }

        private void ProjectMethods(BaristaContext context, JsObject targetObject, ObjectReflector reflector, IDictionary<string, IList<MethodInfo>> methods)
        {
            foreach(var method in methods)
            {
                var methodName = method.Key;
                var methodInfos = method.Value;

                var fn = context.CreateFunction(new BaristaFunctionDelegate((calleeObj, isConstructCall, thisObj, args) =>
                {
                    object targetObj = null;

                    if (thisObj == null)
                    {
                        context.CurrentScope.SetException(context.CreateTypeError($"Could not call function '{methodName}': Invalid 'this' context."));
                        return context.Undefined;
                    }

                    if (thisObj.TryGetBean(out JsExternalObject xoObj))
                    {
                        targetObj = xoObj.Target;
                    }

                    try
                    {
                        var bestMethod = reflector.GetMethodBestMatch(methodInfos, args);
                        if (bestMethod == null)
                        {
                            var ex = context.CreateTypeError($"Failed to call function '{methodName}': Could not find a matching function for the provided arguments.");
                            context.CurrentScope.SetException(ex);
                            return context.Undefined;
                        }

                        //Convert the args into the native args of the method.
                        var methodParams = bestMethod.GetParameters();
                        var convertedArgs = ConvertArgsToParamTypes(context, args, methodParams);

                        var result = bestMethod.Invoke(targetObj, convertedArgs);
                        if (context.Converter.TryFromObject(context, result, out JsValue resultValue))
                        {
                            return resultValue;
                        }
                        else
                        {
                            context.CurrentScope.SetException(context.CreateTypeError($"The call to '{methodName}' was successful, but the result could not be converted into a JavaScript object."));
                            return context.Undefined;
                        }
                    }
                    catch (Exception ex)
                    {
                        context.CurrentScope.SetException(context.CreateError(ex.Message));
                        return context.Undefined;
                    }

                }));

                var functionDescriptor = context.CreateObject();

                if (methodInfos.All(mi => BaristaPropertyAttribute.GetAttribute(mi).Configurable))
                    functionDescriptor.SetProperty("configurable", context.True);
                if (methodInfos.All(mi => BaristaPropertyAttribute.GetAttribute(mi).Enumerable))
                    functionDescriptor.SetProperty("enumerable", context.True);
                if (methodInfos.All(mi => BaristaPropertyAttribute.GetAttribute(mi).Writable))
                    functionDescriptor.SetProperty("writable", context.True);

                functionDescriptor.SetProperty("value", fn);

                targetObject.SetProperty(context.CreateString(methodName), functionDescriptor);
            }
        }

        private void ProjectEvents(BaristaContext context, JsObject targetObject, ObjectReflector reflector, IDictionary<string, EventInfo> eventsTable)
        {
            if (eventsTable.Count == 0)
                return;

            var fnAddEventListener = context.CreateFunction(new Func<JsObject, string, JsFunction, JsValue>((thisObj, eventName, fnCallback) => {

                if (String.IsNullOrWhiteSpace(eventName))
                {
                    context.CurrentScope.SetException(context.CreateTypeError($"The name of the event listener to register must be specified."));
                    return context.Undefined;
                }

                object targetObj = null;

                if (thisObj == null)
                {
                    context.CurrentScope.SetException(context.CreateTypeError($"Could not register event listener '{eventName}': Invalid 'this' context."));
                    return context.Undefined;
                }

                if (thisObj.TryGetBean(out JsExternalObject xoObj))
                {
                    targetObj = xoObj.Target;
                }

                if (!eventsTable.TryGetValue(eventName, out EventInfo targetEvent))
                    return context.False;

                Action<object[]> invokeListener = (args) =>
                {
                    //TODO: Object conversion.
                    fnCallback.Call(thisObj, null);
                };

                var targetEventMethod = targetEvent.EventHandlerType.GetMethod("Invoke");
                var targetEventParameters = targetEventMethod.GetParameters().Select(p => Expression.Parameter(p.ParameterType, p.Name)).ToArray();

                var exprInvokeListener = Expression.Lambda(targetEvent.EventHandlerType, Expression.Block(
                    Expression.Call(
                        Expression.Constant(invokeListener.Target),
                        invokeListener.Method,
                        Expression.NewArrayInit(typeof(object), targetEventParameters))
                ), targetEventParameters);

                var invokeListenerDelegate = exprInvokeListener.Compile();

                IDictionary<string, IList<Tuple<JsFunction, Delegate>>> eventListeners;
                if (thisObj.HasProperty(BaristaEventListenersPropertyName))
                {
                    var xoListeners = thisObj.GetProperty<JsExternalObject>(BaristaEventListenersPropertyName);
                    eventListeners = xoListeners.Target as IDictionary<string, IList<Tuple<JsFunction, Delegate>>>;
                }
                else
                {
                    eventListeners = new Dictionary<string, IList<Tuple<JsFunction, Delegate>>>();

                    //Set the listeners as a non-configurable, non-enumerable, non-writable property
                    var xoListeners = context.CreateExternalObject(eventListeners);

                    var baristaEventListenersPropertyDescriptor = context.CreateObject();
                    baristaEventListenersPropertyDescriptor.SetProperty("value", xoListeners);
                    context.Object.DefineProperty(thisObj, context.CreateString(BaristaEventListenersPropertyName), baristaEventListenersPropertyDescriptor);
                }

                if (eventListeners != null)
                {
                    if (eventListeners.ContainsKey(eventName))
                        eventListeners[eventName].Add(new Tuple<JsFunction, Delegate>(fnCallback, invokeListenerDelegate));
                    else
                        eventListeners.Add(eventName, new List<Tuple<JsFunction, Delegate>>() { new Tuple<JsFunction, Delegate>(fnCallback, invokeListenerDelegate) });
                }
                
                targetEvent.AddMethod.Invoke(targetObj, new object[] { invokeListenerDelegate });

                return context.True;
            }), "addEventListener");

            var fnRemoveEventListener = context.CreateFunction(new Func<JsObject, string, JsFunction, JsValue>((thisObj, eventName, eventListener) =>
            {
                if (String.IsNullOrWhiteSpace(eventName))
                {
                    context.CurrentScope.SetException(context.CreateTypeError($"The name of the event listener to remove must be specified."));
                    return context.Undefined;
                }

                if (eventListener == null)
                {
                    context.CurrentScope.SetException(context.CreateTypeError($"The event listener to remove must be specified."));
                    return context.Undefined;
                }

                object targetObj = null;

                if (thisObj == null)
                {
                    context.CurrentScope.SetException(context.CreateTypeError($"Could not unregister event listener '{eventName}': Invalid 'this' context."));
                    return context.Undefined;
                }

                if (thisObj.TryGetBean(out JsExternalObject xoObj))
                {
                    targetObj = xoObj.Target;
                }

                if (!eventsTable.TryGetValue(eventName, out EventInfo targetEvent))
                    return context.False;

                //Get the event listeners.
                IDictionary<string, IList<Tuple<JsFunction, Delegate>>> eventListeners = null;
                if (thisObj.HasProperty(BaristaEventListenersPropertyName))
                {
                    var xoListeners = thisObj.GetProperty<JsExternalObject>(BaristaEventListenersPropertyName);
                    eventListeners = xoListeners.Target as IDictionary<string, IList<Tuple<JsFunction, Delegate>>>;
                }

                if (eventListeners == null)
                    return context.False;

                var hasRemoved = false;
                if (eventListeners.ContainsKey(eventName))
                {
                    var listeners = eventListeners[eventName];
                    var toRemove = new List<Tuple<JsFunction, Delegate>>();
                    foreach (var listener in listeners)
                    {
                        if (listener.Item1 == eventListener)
                        {
                            targetEvent.RemoveMethod.Invoke(targetObj, new object[] { listener.Item2 });
                            toRemove.Add(listener);
                            hasRemoved = true;
                        }
                    }

                    eventListeners[eventName] = listeners.Where(l => toRemove.Any(tl => tl == l)).ToList();
                }

                return hasRemoved ? context.True : context.False;
            }), "removeEventListener");

            var fnRemoveAllEventListeners = context.CreateFunction(new Func<JsObject, string, JsValue>((thisObj, eventName) => {

                if (String.IsNullOrWhiteSpace(eventName))
                {
                    context.CurrentScope.SetException(context.CreateTypeError($"The name of the event listener to remove must be specified."));
                    return context.Undefined;
                }

                object targetObj = null;

                if (thisObj == null)
                {
                    context.CurrentScope.SetException(context.CreateTypeError($"Could not unregister event listener '{eventName}': Invalid 'this' context."));
                    return context.Undefined;
                }

                if (thisObj.TryGetBean(out JsExternalObject xoObj))
                {
                    targetObj = xoObj.Target;
                }

                if (!eventsTable.TryGetValue(eventName, out EventInfo targetEvent))
                    return context.False;

                //Get the event listeners.
                IDictionary<string, IList<Tuple<JsFunction, Delegate>>> eventListeners = null;
                if (thisObj.HasProperty(BaristaEventListenersPropertyName))
                {
                    var xoListeners = thisObj.GetProperty<JsExternalObject>(BaristaEventListenersPropertyName);
                    eventListeners = xoListeners.Target as IDictionary<string, IList<Tuple<JsFunction, Delegate>>>;
                }

                if (eventListeners == null)
                    return context.False;

                if (eventListeners.ContainsKey(eventName))
                {
                    foreach(var listener in eventListeners[eventName])
                    {
                        targetEvent.RemoveMethod.Invoke(targetObj, new object[] { listener.Item2 });
                    }

                    eventListeners.Remove(eventName);
                }

                return context.True;
            }), "removeAllEventListeners");

            var addEventListenerFunctionDescriptor = context.CreateObject();
            addEventListenerFunctionDescriptor.SetProperty("enumerable", context.True);
            addEventListenerFunctionDescriptor.SetProperty("value", fnAddEventListener);
            targetObject.SetProperty(context.CreateString("addEventListener"), addEventListenerFunctionDescriptor);

            var removeEventListenerFunctionDescriptor = context.CreateObject();
            removeEventListenerFunctionDescriptor.SetProperty("enumerable", context.True);
            removeEventListenerFunctionDescriptor.SetProperty("value", fnRemoveEventListener);
            targetObject.SetProperty(context.CreateString("removeEventListener"), removeEventListenerFunctionDescriptor);

            var removeAllEventListenersFunctionDescriptor = context.CreateObject();
            removeAllEventListenersFunctionDescriptor.SetProperty("enumerable", context.True);
            removeAllEventListenersFunctionDescriptor.SetProperty("value", fnRemoveAllEventListeners);
            targetObject.SetProperty(context.CreateString("removeAllEventListeners"), removeAllEventListenersFunctionDescriptor);
        }

        /// <summary>
        /// Projects the [[iterator]] protocol on IEnumerable objects.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="targetObject"></param>
        /// <param name="reflector"></param>
        private void ProjectIEnumerable(BaristaContext context, JsObject targetObject, ObjectReflector reflector)
        {
            if (typeof(IEnumerable).IsAssignableFrom(reflector.Type))
            {
                var fnIterator = context.CreateFunction(new Func<JsObject, JsValue>((thisObj) => {
                    IEnumerable targetObj = null;

                    if (thisObj == null)
                    {
                        context.CurrentScope.SetException(context.CreateTypeError($"Could not retrieve iterator on object {targetObject.ToString()}: Invalid 'this' context."));
                        return context.Undefined;
                    }

                    if (thisObj.TryGetBean(out JsExternalObject xoObj))
                    {
                        targetObj = xoObj.Target as IEnumerable;
                    }

                    return context.CreateIterator(targetObj.GetEnumerator());
                }));

                var iteratorDescriptor = context.CreateObject();
                iteratorDescriptor.SetProperty("value", fnIterator);
                targetObject.SetProperty(context.Symbol.Iterator, iteratorDescriptor);
            }
        }

        private JsValue GetPropertyValue(BaristaContext context, PropertyInfo prop, string propertyName, JsObject thisObj)
        {
            object targetObj = null;

            if (thisObj == null)
            {
                context.CurrentScope.SetException(context.CreateTypeError($"Could not retrieve property '{propertyName}': Invalid 'this' context."));
                return context.Undefined;
            }

            if (thisObj.TryGetBean(out JsExternalObject xoObj))
            {
                targetObj = xoObj.Target;
            }

            try
            {
                var result = prop.GetValue(targetObj);
                if (context.Converter.TryFromObject(context, result, out JsValue resultValue))
                {
                    return resultValue;
                }
                else
                {
                    return context.Undefined;
                }
            }
            catch (Exception ex)
            {
                context.CurrentScope.SetException(context.CreateError(ex.Message));
                return context.Undefined;
            }
        }

        private JsValue SetPropertyValue(BaristaContext context, PropertyInfo prop, string propertyName, JsObject thisObj, object[] args)
        {
            object targetObj = null;

            if (thisObj == null)
            {
                context.CurrentScope.SetException(context.CreateTypeError($"Could not set property '{propertyName}': Invalid 'this' context."));
                return context.Undefined;
            }

            if (thisObj.TryGetBean(out JsExternalObject xoObj))
            {
                targetObj = xoObj.Target;
            }

            var setPropertyValueType = prop.SetMethod.GetParameters().First().ParameterType;
            var argumentValue = args.ElementAtOrDefault(0);

            try
            {
                
                //If the exposed property is a JsValue, Attempt to convert and then set the property.
                if (typeof(JsValue).IsSameOrSubclass(setPropertyValueType) &&
                    context.Converter.TryFromObject(context, argumentValue, out JsValue jsValue) &&
                    setPropertyValueType.IsSameOrSubclass(jsValue.GetType()))
                {
                    prop.SetValue(targetObj, jsValue);
                    return context.Undefined;
                }
                
                var value = Convert.ChangeType(args.ElementAtOrDefault(0), setPropertyValueType);
                prop.SetValue(targetObj, value);
                return context.Undefined;
            }
            catch (Exception ex)
            {
                context.CurrentScope.SetException(context.CreateError(ex.Message));
                return context.Undefined;
            }
        }

        private JsValue GetIndexerPropertyValue(BaristaContext context, PropertyInfo indexerProp, string propertyName, JsObject thisObj, object[] args)
        {
            object targetObj = null;

            if (thisObj == null)
            {
                context.CurrentScope.SetException(context.CreateTypeError($"Could not get indexer property '{propertyName}': Invalid 'this' context."));
                return context.Undefined;
            }

            if (args.Length < 1)
            {
                context.CurrentScope.SetException(context.CreateTypeError($"Could not get indexer property '{propertyName}': At least one index must be specified."));
                return context.Undefined;
            }

            if (thisObj.TryGetBean(out JsExternalObject xoObj))
            {
                targetObj = xoObj.Target;
            }

            try
            {
                var indexParameters = indexerProp.GetIndexParameters();
                var indexArgs = new object[indexParameters.Length];
                for(int i = 0; i < indexParameters.Length; i++)
                {
                    indexArgs[i] = Convert.ChangeType(args.ElementAtOrDefault(i), indexParameters[i].ParameterType);
                }

                var result = indexerProp.GetValue(targetObj, indexArgs);
                if (context.Converter.TryFromObject(context, result, out JsValue resultValue))
                {
                    return resultValue;
                }
                else
                {
                    return context.Undefined;
                }
            }
            catch (Exception ex)
            {
                context.CurrentScope.SetException(context.CreateError(ex.Message));
                return context.Undefined;
            }
        }

        private JsValue SetIndexerPropertyValue(BaristaContext context, PropertyInfo indexerProp, string propertyName, JsObject thisObj, object[] args)
        {
            object targetObj = null;

            if (thisObj == null)
            {
                context.CurrentScope.SetException(context.CreateTypeError($"Could not set indexer property '{propertyName}': Invalid 'this' context."));
                return context.Undefined;
            }

            if (args.Length < 2)
            {
                context.CurrentScope.SetException(context.CreateTypeError($"Could not set indexer property '{propertyName}': At least one index and a value be specified."));
                return context.Undefined;
            }

            if (thisObj.TryGetBean(out JsExternalObject xoObj))
            {
                targetObj = xoObj.Target;
            }

            try
            {
                var value = args.LastOrDefault();
                var nativeArgs = args.Take(args.Length - 1).ToArray();
                var indexParameters = indexerProp.GetIndexParameters();
                var indexArgs = new object[indexParameters.Length];
                for (int i = 0; i < indexParameters.Length; i++)
                {
                    indexArgs[i] = Convert.ChangeType(nativeArgs.ElementAtOrDefault(i), indexParameters[i].ParameterType);
                }

                indexerProp.SetValue(targetObj, value, indexArgs);
                return context.Undefined;
            }
            catch (Exception ex)
            {
                context.CurrentScope.SetException(context.CreateError(ex.Message));
                return context.Undefined;
            }
        }

        private object[] ConvertArgsToParamTypes(BaristaContext context, object[] args, ParameterInfo[] parameters)
        {
            //TODO: BaristaValueFactory.CreateNativeFunctionForDelegate has very similar code. Consolidate if possible.

            var convertedArgs = new object[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                var currentParam = parameters[i];
                var currentParamType = currentParam.ParameterType;

                //For nullable values get the underlying type.
                if (currentParamType.IsGenericType && currentParamType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    currentParamType = Nullable.GetUnderlyingType(currentParamType);

                var currentArg = args.ElementAtOrDefault(i);
                if (currentParamType == typeof(BaristaContext))
                {
                    convertedArgs[i] = context;
                }
                else
                {
                    try
                    {
                        convertedArgs[i] = Convert.ChangeType(currentArg, currentParamType);
                    }
                    catch (Exception)
                    {
                        //Something went wrong, use the default value.
                        convertedArgs[i] = currentParamType.GetDefaultValue();
                    }
                }
            }
            return convertedArgs;
        }
    }
}
