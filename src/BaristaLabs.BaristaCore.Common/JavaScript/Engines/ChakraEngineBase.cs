namespace BaristaLabs.BaristaCore.JavaScript
{
	using Internal;

	using System;
	using System.Runtime.InteropServices;

    public abstract class ChakraEngineBase : IJavaScriptEngine
    {
        public IntPtr JsInitializeModuleRecord(IntPtr referencingModule, JavaScriptValueSafeHandle normalizedSpecifier)
        {
            IntPtr moduleRecord;
            Errors.ThrowIfError(LibChakraCore.JsInitializeModuleRecord(referencingModule, normalizedSpecifier, out moduleRecord));
            return moduleRecord;
        }

        public JavaScriptValueSafeHandle JsParseModuleSource(IntPtr requestModule, JavaScriptSourceContext sourceContext, byte[] script, uint scriptLength, JavaScriptParseModuleSourceFlags sourceFlag)
        {
            JavaScriptValueSafeHandle exceptionValueRef;
            Errors.ThrowIfError(LibChakraCore.JsParseModuleSource(requestModule, sourceContext, script, scriptLength, sourceFlag, out exceptionValueRef));
            exceptionValueRef.NativeFunctionSource = nameof(LibChakraCore.JsParseModuleSource);
            if (exceptionValueRef != JavaScriptValueSafeHandle.Invalid)
            {
				uint valueRefCount;
				Errors.ThrowIfError(LibChakraCore.JsAddRef(exceptionValueRef, out valueRefCount));
			}
            return exceptionValueRef;
        }

        public JavaScriptValueSafeHandle JsModuleEvaluation(IntPtr requestModule)
        {
            JavaScriptValueSafeHandle result;
            Errors.ThrowIfError(LibChakraCore.JsModuleEvaluation(requestModule, out result));
            result.NativeFunctionSource = nameof(LibChakraCore.JsModuleEvaluation);
            if (result != JavaScriptValueSafeHandle.Invalid)
            {
				uint valueRefCount;
				Errors.ThrowIfError(LibChakraCore.JsAddRef(result, out valueRefCount));
			}
            return result;
        }

        public void JsSetModuleHostInfo(IntPtr requestModule, JavaScriptModuleHostInfoKind moduleHostInfo, IntPtr hostInfo)
        {
            Errors.ThrowIfError(LibChakraCore.JsSetModuleHostInfo(requestModule, moduleHostInfo, hostInfo));
        }

        public IntPtr JsGetModuleHostInfo(IntPtr requestModule, JavaScriptModuleHostInfoKind moduleHostInfo)
        {
            IntPtr hostInfo;
            Errors.ThrowIfError(LibChakraCore.JsGetModuleHostInfo(requestModule, moduleHostInfo, out hostInfo));
            return hostInfo;
        }

        public JavaScriptValueSafeHandle JsCreateString(string content, ulong length)
        {
            JavaScriptValueSafeHandle value;
            Errors.ThrowIfError(LibChakraCore.JsCreateString(content, length, out value));
            value.NativeFunctionSource = nameof(LibChakraCore.JsCreateString);
            if (value != JavaScriptValueSafeHandle.Invalid)
            {
				uint valueRefCount;
				Errors.ThrowIfError(LibChakraCore.JsAddRef(value, out valueRefCount));
			}
            return value;
        }

        public JavaScriptValueSafeHandle JsCreateStringUtf16(string content, ulong length)
        {
            JavaScriptValueSafeHandle value;
            Errors.ThrowIfError(LibChakraCore.JsCreateStringUtf16(content, length, out value));
            value.NativeFunctionSource = nameof(LibChakraCore.JsCreateStringUtf16);
            if (value != JavaScriptValueSafeHandle.Invalid)
            {
				uint valueRefCount;
				Errors.ThrowIfError(LibChakraCore.JsAddRef(value, out valueRefCount));
			}
            return value;
        }

        public ulong JsCopyString(JavaScriptValueSafeHandle value, byte[] buffer, ulong bufferSize)
        {
            ulong written;
            Errors.ThrowIfError(LibChakraCore.JsCopyString(value, buffer, bufferSize, out written));
            return written;
        }

        public ulong JsCopyStringUtf16(JavaScriptValueSafeHandle value, int start, int length, byte[] buffer)
        {
            ulong written;
            Errors.ThrowIfError(LibChakraCore.JsCopyStringUtf16(value, start, length, buffer, out written));
            return written;
        }

        public JavaScriptValueSafeHandle JsParse(JavaScriptValueSafeHandle script, JavaScriptSourceContext sourceContext, JavaScriptValueSafeHandle sourceUrl, JavaScriptParseScriptAttributes parseAttributes)
        {
            JavaScriptValueSafeHandle result;
            Errors.ThrowIfError(LibChakraCore.JsParse(script, sourceContext, sourceUrl, parseAttributes, out result));
            result.NativeFunctionSource = nameof(LibChakraCore.JsParse);
            if (result != JavaScriptValueSafeHandle.Invalid)
            {
				uint valueRefCount;
				Errors.ThrowIfError(LibChakraCore.JsAddRef(result, out valueRefCount));
			}
            return result;
        }

        public JavaScriptValueSafeHandle JsRun(JavaScriptValueSafeHandle script, JavaScriptSourceContext sourceContext, JavaScriptValueSafeHandle sourceUrl, JavaScriptParseScriptAttributes parseAttributes)
        {
            JavaScriptValueSafeHandle result;
            Errors.ThrowIfError(LibChakraCore.JsRun(script, sourceContext, sourceUrl, parseAttributes, out result));
            result.NativeFunctionSource = nameof(LibChakraCore.JsRun);
            if (result != JavaScriptValueSafeHandle.Invalid)
            {
				uint valueRefCount;
				Errors.ThrowIfError(LibChakraCore.JsAddRef(result, out valueRefCount));
			}
            return result;
        }

        public JavaScriptPropertyIdSafeHandle JsCreatePropertyId(string name, ulong length)
        {
            JavaScriptPropertyIdSafeHandle propertyId;
            Errors.ThrowIfError(LibChakraCore.JsCreatePropertyId(name, length, out propertyId));
            propertyId.NativeFunctionSource = nameof(LibChakraCore.JsCreatePropertyId);
            if (propertyId != JavaScriptPropertyIdSafeHandle.Invalid)
            {
				uint valueRefCount;
				Errors.ThrowIfError(LibChakraCore.JsAddRef(propertyId, out valueRefCount));
			}
            return propertyId;
        }

        public ulong JsCopyPropertyId(JavaScriptPropertyIdSafeHandle propertyId, byte[] buffer, ulong bufferSize)
        {
            ulong length;
            Errors.ThrowIfError(LibChakraCore.JsCopyPropertyId(propertyId, buffer, bufferSize, out length));
            return length;
        }

        public JavaScriptValueSafeHandle JsSerialize(JavaScriptValueSafeHandle script, JavaScriptParseScriptAttributes parseAttributes)
        {
            JavaScriptValueSafeHandle buffer;
            Errors.ThrowIfError(LibChakraCore.JsSerialize(script, out buffer, parseAttributes));
            buffer.NativeFunctionSource = nameof(LibChakraCore.JsSerialize);
            if (buffer != JavaScriptValueSafeHandle.Invalid)
            {
				uint valueRefCount;
				Errors.ThrowIfError(LibChakraCore.JsAddRef(buffer, out valueRefCount));
			}
            return buffer;
        }

        public JavaScriptValueSafeHandle JsParseSerialized(JavaScriptValueSafeHandle buffer, JavaScriptSerializedLoadScriptCallback scriptLoadCallback, JavaScriptSourceContext sourceContext, JavaScriptValueSafeHandle sourceUrl)
        {
            JavaScriptValueSafeHandle result;
            Errors.ThrowIfError(LibChakraCore.JsParseSerialized(buffer, scriptLoadCallback, sourceContext, sourceUrl, out result));
            result.NativeFunctionSource = nameof(LibChakraCore.JsParseSerialized);
            if (result != JavaScriptValueSafeHandle.Invalid)
            {
				uint valueRefCount;
				Errors.ThrowIfError(LibChakraCore.JsAddRef(result, out valueRefCount));
			}
            return result;
        }

        public JavaScriptValueSafeHandle JsRunSerialized(JavaScriptValueSafeHandle buffer, JavaScriptSerializedLoadScriptCallback scriptLoadCallback, JavaScriptSourceContext sourceContext, JavaScriptValueSafeHandle sourceUrl)
        {
            JavaScriptValueSafeHandle result;
            Errors.ThrowIfError(LibChakraCore.JsRunSerialized(buffer, scriptLoadCallback, sourceContext, sourceUrl, out result));
            result.NativeFunctionSource = nameof(LibChakraCore.JsRunSerialized);
            if (result != JavaScriptValueSafeHandle.Invalid)
            {
				uint valueRefCount;
				Errors.ThrowIfError(LibChakraCore.JsAddRef(result, out valueRefCount));
			}
            return result;
        }

        public JavaScriptValueSafeHandle JsCreatePromise(out JavaScriptValueSafeHandle resolveFunction, out JavaScriptValueSafeHandle rejectFunction)
        {
            JavaScriptValueSafeHandle promise;
            Errors.ThrowIfError(LibChakraCore.JsCreatePromise(out promise, out resolveFunction, out rejectFunction));
            promise.NativeFunctionSource = nameof(LibChakraCore.JsCreatePromise);
            if (promise != JavaScriptValueSafeHandle.Invalid)
            {
				uint valueRefCount;
				Errors.ThrowIfError(LibChakraCore.JsAddRef(promise, out valueRefCount));
			}
            resolveFunction.NativeFunctionSource = nameof(LibChakraCore.JsCreatePromise);
            if (resolveFunction != JavaScriptValueSafeHandle.Invalid)
            {
				uint valueRefCount;
				Errors.ThrowIfError(LibChakraCore.JsAddRef(resolveFunction, out valueRefCount));
			}
            rejectFunction.NativeFunctionSource = nameof(LibChakraCore.JsCreatePromise);
            if (rejectFunction != JavaScriptValueSafeHandle.Invalid)
            {
				uint valueRefCount;
				Errors.ThrowIfError(LibChakraCore.JsAddRef(rejectFunction, out valueRefCount));
			}
            return promise;
        }

        public JavaScriptRuntimeSafeHandle JsCreateRuntime(JavaScriptRuntimeAttributes attributes, JavaScriptThreadServiceCallback threadService)
        {
            JavaScriptRuntimeSafeHandle runtime;
            Errors.ThrowIfError(LibChakraCore.JsCreateRuntime(attributes, threadService, out runtime));
            runtime.NativeFunctionSource = nameof(LibChakraCore.JsCreateRuntime);
            return runtime;
        }

        public void JsCollectGarbage(JavaScriptRuntimeSafeHandle runtime)
        {
            Errors.ThrowIfError(LibChakraCore.JsCollectGarbage(runtime));
        }

        public void JsDisposeRuntime(IntPtr runtime)
        {
            Errors.ThrowIfError(LibChakraCore.JsDisposeRuntime(runtime));
        }

        public ulong JsGetRuntimeMemoryUsage(JavaScriptRuntimeSafeHandle runtime)
        {
            ulong memoryUsage;
            Errors.ThrowIfError(LibChakraCore.JsGetRuntimeMemoryUsage(runtime, out memoryUsage));
            return memoryUsage;
        }

        public ulong JsGetRuntimeMemoryLimit(JavaScriptRuntimeSafeHandle runtime)
        {
            ulong memoryLimit;
            Errors.ThrowIfError(LibChakraCore.JsGetRuntimeMemoryLimit(runtime, out memoryLimit));
            return memoryLimit;
        }

        public void JsSetRuntimeMemoryLimit(JavaScriptRuntimeSafeHandle runtime, ulong memoryLimit)
        {
            Errors.ThrowIfError(LibChakraCore.JsSetRuntimeMemoryLimit(runtime, memoryLimit));
        }

        public void JsSetRuntimeMemoryAllocationCallback(JavaScriptRuntimeSafeHandle runtime, IntPtr callbackState, JavaScriptMemoryAllocationCallback allocationCallback)
        {
            Errors.ThrowIfError(LibChakraCore.JsSetRuntimeMemoryAllocationCallback(runtime, callbackState, allocationCallback));
        }

        public void JsSetRuntimeBeforeCollectCallback(JavaScriptRuntimeSafeHandle runtime, IntPtr callbackState, JavaScriptBeforeCollectCallback beforeCollectCallback)
        {
            Errors.ThrowIfError(LibChakraCore.JsSetRuntimeBeforeCollectCallback(runtime, callbackState, beforeCollectCallback));
        }

        public uint JsAddRef(SafeHandle @ref)
        {
            uint count;
            Errors.ThrowIfError(LibChakraCore.JsAddRef(@ref, out count));
            return count;
        }

        public uint JsRelease(SafeHandle @ref)
        {
            uint count;
            Errors.ThrowIfError(LibChakraCore.JsRelease(@ref, out count));
            return count;
        }

        public void JsSetObjectBeforeCollectCallback(SafeHandle @ref, IntPtr callbackState, JavaScriptObjectBeforeCollectCallback objectBeforeCollectCallback)
        {
            Errors.ThrowIfError(LibChakraCore.JsSetObjectBeforeCollectCallback(@ref, callbackState, objectBeforeCollectCallback));
        }

        public JavaScriptContextSafeHandle JsCreateContext(JavaScriptRuntimeSafeHandle runtime)
        {
            JavaScriptContextSafeHandle newContext;
            Errors.ThrowIfError(LibChakraCore.JsCreateContext(runtime, out newContext));
            newContext.NativeFunctionSource = nameof(LibChakraCore.JsCreateContext);
            if (newContext != JavaScriptContextSafeHandle.Invalid)
            {
				uint valueRefCount;
				Errors.ThrowIfError(LibChakraCore.JsAddRef(newContext, out valueRefCount));
			}
            return newContext;
        }

        public JavaScriptContextSafeHandle JsGetCurrentContext()
        {
            JavaScriptContextSafeHandle currentContext;
            Errors.ThrowIfError(LibChakraCore.JsGetCurrentContext(out currentContext));
            currentContext.NativeFunctionSource = nameof(LibChakraCore.JsGetCurrentContext);
            if (currentContext != JavaScriptContextSafeHandle.Invalid)
            {
				uint valueRefCount;
				Errors.ThrowIfError(LibChakraCore.JsAddRef(currentContext, out valueRefCount));
			}
            return currentContext;
        }

        public void JsSetCurrentContext(JavaScriptContextSafeHandle context)
        {
            Errors.ThrowIfError(LibChakraCore.JsSetCurrentContext(context));
        }

        public JavaScriptContextSafeHandle JsGetContextOfObject(JavaScriptValueSafeHandle @object)
        {
            JavaScriptContextSafeHandle context;
            Errors.ThrowIfError(LibChakraCore.JsGetContextOfObject(@object, out context));
            context.NativeFunctionSource = nameof(LibChakraCore.JsGetContextOfObject);
            if (context != JavaScriptContextSafeHandle.Invalid)
            {
				uint valueRefCount;
				Errors.ThrowIfError(LibChakraCore.JsAddRef(context, out valueRefCount));
			}
            return context;
        }

        public IntPtr JsGetContextData(JavaScriptContextSafeHandle context)
        {
            IntPtr data;
            Errors.ThrowIfError(LibChakraCore.JsGetContextData(context, out data));
            return data;
        }

        public void JsSetContextData(JavaScriptContextSafeHandle context, IntPtr data)
        {
            Errors.ThrowIfError(LibChakraCore.JsSetContextData(context, data));
        }

        public JavaScriptRuntimeSafeHandle JsGetRuntime(JavaScriptContextSafeHandle context)
        {
            JavaScriptRuntimeSafeHandle runtime;
            Errors.ThrowIfError(LibChakraCore.JsGetRuntime(context, out runtime));
            runtime.NativeFunctionSource = nameof(LibChakraCore.JsGetRuntime);
            return runtime;
        }

        public uint JsIdle()
        {
            uint nextIdleTick;
            Errors.ThrowIfError(LibChakraCore.JsIdle(out nextIdleTick));
            return nextIdleTick;
        }

        public JavaScriptValueSafeHandle JsGetSymbolFromPropertyId(JavaScriptPropertyIdSafeHandle propertyId)
        {
            JavaScriptValueSafeHandle symbol;
            Errors.ThrowIfError(LibChakraCore.JsGetSymbolFromPropertyId(propertyId, out symbol));
            symbol.NativeFunctionSource = nameof(LibChakraCore.JsGetSymbolFromPropertyId);
            if (symbol != JavaScriptValueSafeHandle.Invalid)
            {
				uint valueRefCount;
				Errors.ThrowIfError(LibChakraCore.JsAddRef(symbol, out valueRefCount));
			}
            return symbol;
        }

        public JavaScriptPropertyIdType JsGetPropertyIdType(JavaScriptPropertyIdSafeHandle propertyId)
        {
            JavaScriptPropertyIdType propertyIdType;
            Errors.ThrowIfError(LibChakraCore.JsGetPropertyIdType(propertyId, out propertyIdType));
            return propertyIdType;
        }

        public JavaScriptPropertyIdSafeHandle JsGetPropertyIdFromSymbol(JavaScriptValueSafeHandle symbol)
        {
            JavaScriptPropertyIdSafeHandle propertyId;
            Errors.ThrowIfError(LibChakraCore.JsGetPropertyIdFromSymbol(symbol, out propertyId));
            propertyId.NativeFunctionSource = nameof(LibChakraCore.JsGetPropertyIdFromSymbol);
            if (propertyId != JavaScriptPropertyIdSafeHandle.Invalid)
            {
				uint valueRefCount;
				Errors.ThrowIfError(LibChakraCore.JsAddRef(propertyId, out valueRefCount));
			}
            return propertyId;
        }

        public JavaScriptValueSafeHandle JsCreateSymbol(JavaScriptValueSafeHandle description)
        {
            JavaScriptValueSafeHandle result;
            Errors.ThrowIfError(LibChakraCore.JsCreateSymbol(description, out result));
            result.NativeFunctionSource = nameof(LibChakraCore.JsCreateSymbol);
            if (result != JavaScriptValueSafeHandle.Invalid)
            {
				uint valueRefCount;
				Errors.ThrowIfError(LibChakraCore.JsAddRef(result, out valueRefCount));
			}
            return result;
        }

        public JavaScriptValueSafeHandle JsGetOwnPropertySymbols(JavaScriptValueSafeHandle @object)
        {
            JavaScriptValueSafeHandle propertySymbols;
            Errors.ThrowIfError(LibChakraCore.JsGetOwnPropertySymbols(@object, out propertySymbols));
            propertySymbols.NativeFunctionSource = nameof(LibChakraCore.JsGetOwnPropertySymbols);
            if (propertySymbols != JavaScriptValueSafeHandle.Invalid)
            {
				uint valueRefCount;
				Errors.ThrowIfError(LibChakraCore.JsAddRef(propertySymbols, out valueRefCount));
			}
            return propertySymbols;
        }

        public JavaScriptValueSafeHandle JsGetUndefinedValue()
        {
            JavaScriptValueSafeHandle undefinedValue;
            Errors.ThrowIfError(LibChakraCore.JsGetUndefinedValue(out undefinedValue));
            undefinedValue.NativeFunctionSource = nameof(LibChakraCore.JsGetUndefinedValue);
            if (undefinedValue != JavaScriptValueSafeHandle.Invalid)
            {
				uint valueRefCount;
				Errors.ThrowIfError(LibChakraCore.JsAddRef(undefinedValue, out valueRefCount));
			}
            return undefinedValue;
        }

        public JavaScriptValueSafeHandle JsGetNullValue()
        {
            JavaScriptValueSafeHandle nullValue;
            Errors.ThrowIfError(LibChakraCore.JsGetNullValue(out nullValue));
            nullValue.NativeFunctionSource = nameof(LibChakraCore.JsGetNullValue);
            if (nullValue != JavaScriptValueSafeHandle.Invalid)
            {
				uint valueRefCount;
				Errors.ThrowIfError(LibChakraCore.JsAddRef(nullValue, out valueRefCount));
			}
            return nullValue;
        }

        public JavaScriptValueSafeHandle JsGetTrueValue()
        {
            JavaScriptValueSafeHandle trueValue;
            Errors.ThrowIfError(LibChakraCore.JsGetTrueValue(out trueValue));
            trueValue.NativeFunctionSource = nameof(LibChakraCore.JsGetTrueValue);
            if (trueValue != JavaScriptValueSafeHandle.Invalid)
            {
				uint valueRefCount;
				Errors.ThrowIfError(LibChakraCore.JsAddRef(trueValue, out valueRefCount));
			}
            return trueValue;
        }

        public JavaScriptValueSafeHandle JsGetFalseValue()
        {
            JavaScriptValueSafeHandle falseValue;
            Errors.ThrowIfError(LibChakraCore.JsGetFalseValue(out falseValue));
            falseValue.NativeFunctionSource = nameof(LibChakraCore.JsGetFalseValue);
            if (falseValue != JavaScriptValueSafeHandle.Invalid)
            {
				uint valueRefCount;
				Errors.ThrowIfError(LibChakraCore.JsAddRef(falseValue, out valueRefCount));
			}
            return falseValue;
        }

        public JavaScriptValueSafeHandle JsBoolToBoolean(bool value)
        {
            JavaScriptValueSafeHandle booleanValue;
            Errors.ThrowIfError(LibChakraCore.JsBoolToBoolean(value, out booleanValue));
            booleanValue.NativeFunctionSource = nameof(LibChakraCore.JsBoolToBoolean);
            if (booleanValue != JavaScriptValueSafeHandle.Invalid)
            {
				uint valueRefCount;
				Errors.ThrowIfError(LibChakraCore.JsAddRef(booleanValue, out valueRefCount));
			}
            return booleanValue;
        }

        public bool JsBooleanToBool(JavaScriptValueSafeHandle value)
        {
            bool boolValue;
            Errors.ThrowIfError(LibChakraCore.JsBooleanToBool(value, out boolValue));
            return boolValue;
        }

        public JavaScriptValueSafeHandle JsConvertValueToBoolean(JavaScriptValueSafeHandle value)
        {
            JavaScriptValueSafeHandle booleanValue;
            Errors.ThrowIfError(LibChakraCore.JsConvertValueToBoolean(value, out booleanValue));
            booleanValue.NativeFunctionSource = nameof(LibChakraCore.JsConvertValueToBoolean);
            if (booleanValue != JavaScriptValueSafeHandle.Invalid)
            {
				uint valueRefCount;
				Errors.ThrowIfError(LibChakraCore.JsAddRef(booleanValue, out valueRefCount));
			}
            return booleanValue;
        }

        public JavaScriptValueType JsGetValueType(JavaScriptValueSafeHandle value)
        {
            JavaScriptValueType type;
            Errors.ThrowIfError(LibChakraCore.JsGetValueType(value, out type));
            return type;
        }

        public JavaScriptValueSafeHandle JsDoubleToNumber(double doubleValue)
        {
            JavaScriptValueSafeHandle value;
            Errors.ThrowIfError(LibChakraCore.JsDoubleToNumber(doubleValue, out value));
            value.NativeFunctionSource = nameof(LibChakraCore.JsDoubleToNumber);
            if (value != JavaScriptValueSafeHandle.Invalid)
            {
				uint valueRefCount;
				Errors.ThrowIfError(LibChakraCore.JsAddRef(value, out valueRefCount));
			}
            return value;
        }

        public JavaScriptValueSafeHandle JsIntToNumber(int intValue)
        {
            JavaScriptValueSafeHandle value;
            Errors.ThrowIfError(LibChakraCore.JsIntToNumber(intValue, out value));
            value.NativeFunctionSource = nameof(LibChakraCore.JsIntToNumber);
            if (value != JavaScriptValueSafeHandle.Invalid)
            {
				uint valueRefCount;
				Errors.ThrowIfError(LibChakraCore.JsAddRef(value, out valueRefCount));
			}
            return value;
        }

        public double JsNumberToDouble(JavaScriptValueSafeHandle value)
        {
            double doubleValue;
            Errors.ThrowIfError(LibChakraCore.JsNumberToDouble(value, out doubleValue));
            return doubleValue;
        }

        public int JsNumberToInt(JavaScriptValueSafeHandle value)
        {
            int intValue;
            Errors.ThrowIfError(LibChakraCore.JsNumberToInt(value, out intValue));
            return intValue;
        }

        public JavaScriptValueSafeHandle JsConvertValueToNumber(JavaScriptValueSafeHandle value)
        {
            JavaScriptValueSafeHandle numberValue;
            Errors.ThrowIfError(LibChakraCore.JsConvertValueToNumber(value, out numberValue));
            numberValue.NativeFunctionSource = nameof(LibChakraCore.JsConvertValueToNumber);
            if (numberValue != JavaScriptValueSafeHandle.Invalid)
            {
				uint valueRefCount;
				Errors.ThrowIfError(LibChakraCore.JsAddRef(numberValue, out valueRefCount));
			}
            return numberValue;
        }

        public int JsGetStringLength(JavaScriptValueSafeHandle stringValue)
        {
            int length;
            Errors.ThrowIfError(LibChakraCore.JsGetStringLength(stringValue, out length));
            return length;
        }

        public JavaScriptValueSafeHandle JsConvertValueToString(JavaScriptValueSafeHandle value)
        {
            JavaScriptValueSafeHandle stringValue;
            Errors.ThrowIfError(LibChakraCore.JsConvertValueToString(value, out stringValue));
            stringValue.NativeFunctionSource = nameof(LibChakraCore.JsConvertValueToString);
            if (stringValue != JavaScriptValueSafeHandle.Invalid)
            {
				uint valueRefCount;
				Errors.ThrowIfError(LibChakraCore.JsAddRef(stringValue, out valueRefCount));
			}
            return stringValue;
        }

        public JavaScriptValueSafeHandle JsGetGlobalObject()
        {
            JavaScriptValueSafeHandle globalObject;
            Errors.ThrowIfError(LibChakraCore.JsGetGlobalObject(out globalObject));
            globalObject.NativeFunctionSource = nameof(LibChakraCore.JsGetGlobalObject);
            if (globalObject != JavaScriptValueSafeHandle.Invalid)
            {
				uint valueRefCount;
				Errors.ThrowIfError(LibChakraCore.JsAddRef(globalObject, out valueRefCount));
			}
            return globalObject;
        }

        public JavaScriptValueSafeHandle JsCreateObject()
        {
            JavaScriptValueSafeHandle @object;
            Errors.ThrowIfError(LibChakraCore.JsCreateObject(out @object));
            @object.NativeFunctionSource = nameof(LibChakraCore.JsCreateObject);
            if (@object != JavaScriptValueSafeHandle.Invalid)
            {
				uint valueRefCount;
				Errors.ThrowIfError(LibChakraCore.JsAddRef(@object, out valueRefCount));
			}
            return @object;
        }

        public JavaScriptValueSafeHandle JsCreateExternalObject(IntPtr data, JavaScriptObjectFinalizeCallback finalizeCallback)
        {
            JavaScriptValueSafeHandle @object;
            Errors.ThrowIfError(LibChakraCore.JsCreateExternalObject(data, finalizeCallback, out @object));
            @object.NativeFunctionSource = nameof(LibChakraCore.JsCreateExternalObject);
            if (@object != JavaScriptValueSafeHandle.Invalid)
            {
				uint valueRefCount;
				Errors.ThrowIfError(LibChakraCore.JsAddRef(@object, out valueRefCount));
			}
            return @object;
        }

        public JavaScriptValueSafeHandle JsConvertValueToObject(JavaScriptValueSafeHandle value)
        {
            JavaScriptValueSafeHandle @object;
            Errors.ThrowIfError(LibChakraCore.JsConvertValueToObject(value, out @object));
            @object.NativeFunctionSource = nameof(LibChakraCore.JsConvertValueToObject);
            if (@object != JavaScriptValueSafeHandle.Invalid)
            {
				uint valueRefCount;
				Errors.ThrowIfError(LibChakraCore.JsAddRef(@object, out valueRefCount));
			}
            return @object;
        }

        public JavaScriptValueSafeHandle JsGetPrototype(JavaScriptValueSafeHandle @object)
        {
            JavaScriptValueSafeHandle prototypeObject;
            Errors.ThrowIfError(LibChakraCore.JsGetPrototype(@object, out prototypeObject));
            prototypeObject.NativeFunctionSource = nameof(LibChakraCore.JsGetPrototype);
            if (prototypeObject != JavaScriptValueSafeHandle.Invalid)
            {
				uint valueRefCount;
				Errors.ThrowIfError(LibChakraCore.JsAddRef(prototypeObject, out valueRefCount));
			}
            return prototypeObject;
        }

        public void JsSetPrototype(JavaScriptValueSafeHandle @object, JavaScriptValueSafeHandle prototypeObject)
        {
            Errors.ThrowIfError(LibChakraCore.JsSetPrototype(@object, prototypeObject));
        }

        public bool JsInstanceOf(JavaScriptValueSafeHandle @object, JavaScriptValueSafeHandle constructor)
        {
            bool result;
            Errors.ThrowIfError(LibChakraCore.JsInstanceOf(@object, constructor, out result));
            return result;
        }

        public bool JsGetExtensionAllowed(JavaScriptValueSafeHandle @object)
        {
            bool value;
            Errors.ThrowIfError(LibChakraCore.JsGetExtensionAllowed(@object, out value));
            return value;
        }

        public void JsPreventExtension(JavaScriptValueSafeHandle @object)
        {
            Errors.ThrowIfError(LibChakraCore.JsPreventExtension(@object));
        }

        public JavaScriptValueSafeHandle JsGetProperty(JavaScriptValueSafeHandle @object, JavaScriptPropertyIdSafeHandle propertyId)
        {
            JavaScriptValueSafeHandle value;
            Errors.ThrowIfError(LibChakraCore.JsGetProperty(@object, propertyId, out value));
            value.NativeFunctionSource = nameof(LibChakraCore.JsGetProperty);
            if (value != JavaScriptValueSafeHandle.Invalid)
            {
				uint valueRefCount;
				Errors.ThrowIfError(LibChakraCore.JsAddRef(value, out valueRefCount));
			}
            return value;
        }

        public JavaScriptValueSafeHandle JsGetOwnPropertyDescriptor(JavaScriptValueSafeHandle @object, JavaScriptPropertyIdSafeHandle propertyId)
        {
            JavaScriptValueSafeHandle propertyDescriptor;
            Errors.ThrowIfError(LibChakraCore.JsGetOwnPropertyDescriptor(@object, propertyId, out propertyDescriptor));
            propertyDescriptor.NativeFunctionSource = nameof(LibChakraCore.JsGetOwnPropertyDescriptor);
            if (propertyDescriptor != JavaScriptValueSafeHandle.Invalid)
            {
				uint valueRefCount;
				Errors.ThrowIfError(LibChakraCore.JsAddRef(propertyDescriptor, out valueRefCount));
			}
            return propertyDescriptor;
        }

        public JavaScriptValueSafeHandle JsGetOwnPropertyNames(JavaScriptValueSafeHandle @object)
        {
            JavaScriptValueSafeHandle propertyNames;
            Errors.ThrowIfError(LibChakraCore.JsGetOwnPropertyNames(@object, out propertyNames));
            propertyNames.NativeFunctionSource = nameof(LibChakraCore.JsGetOwnPropertyNames);
            if (propertyNames != JavaScriptValueSafeHandle.Invalid)
            {
				uint valueRefCount;
				Errors.ThrowIfError(LibChakraCore.JsAddRef(propertyNames, out valueRefCount));
			}
            return propertyNames;
        }

        public void JsSetProperty(JavaScriptValueSafeHandle @object, JavaScriptPropertyIdSafeHandle propertyId, JavaScriptValueSafeHandle value, bool useStrictRules)
        {
            Errors.ThrowIfError(LibChakraCore.JsSetProperty(@object, propertyId, value, useStrictRules));
        }

        public bool JsHasProperty(JavaScriptValueSafeHandle @object, JavaScriptPropertyIdSafeHandle propertyId)
        {
            bool hasProperty;
            Errors.ThrowIfError(LibChakraCore.JsHasProperty(@object, propertyId, out hasProperty));
            return hasProperty;
        }

        public JavaScriptValueSafeHandle JsDeleteProperty(JavaScriptValueSafeHandle @object, JavaScriptPropertyIdSafeHandle propertyId, bool useStrictRules)
        {
            JavaScriptValueSafeHandle result;
            Errors.ThrowIfError(LibChakraCore.JsDeleteProperty(@object, propertyId, useStrictRules, out result));
            result.NativeFunctionSource = nameof(LibChakraCore.JsDeleteProperty);
            if (result != JavaScriptValueSafeHandle.Invalid)
            {
				uint valueRefCount;
				Errors.ThrowIfError(LibChakraCore.JsAddRef(result, out valueRefCount));
			}
            return result;
        }

        public bool JsDefineProperty(JavaScriptValueSafeHandle @object, JavaScriptPropertyIdSafeHandle propertyId, JavaScriptValueSafeHandle propertyDescriptor)
        {
            bool result;
            Errors.ThrowIfError(LibChakraCore.JsDefineProperty(@object, propertyId, propertyDescriptor, out result));
            return result;
        }

        public bool JsHasIndexedProperty(JavaScriptValueSafeHandle @object, JavaScriptValueSafeHandle index)
        {
            bool result;
            Errors.ThrowIfError(LibChakraCore.JsHasIndexedProperty(@object, index, out result));
            return result;
        }

        public JavaScriptValueSafeHandle JsGetIndexedProperty(JavaScriptValueSafeHandle @object, JavaScriptValueSafeHandle index)
        {
            JavaScriptValueSafeHandle result;
            Errors.ThrowIfError(LibChakraCore.JsGetIndexedProperty(@object, index, out result));
            result.NativeFunctionSource = nameof(LibChakraCore.JsGetIndexedProperty);
            if (result != JavaScriptValueSafeHandle.Invalid)
            {
				uint valueRefCount;
				Errors.ThrowIfError(LibChakraCore.JsAddRef(result, out valueRefCount));
			}
            return result;
        }

        public void JsSetIndexedProperty(JavaScriptValueSafeHandle @object, JavaScriptValueSafeHandle index, JavaScriptValueSafeHandle value)
        {
            Errors.ThrowIfError(LibChakraCore.JsSetIndexedProperty(@object, index, value));
        }

        public void JsDeleteIndexedProperty(JavaScriptValueSafeHandle @object, JavaScriptValueSafeHandle index)
        {
            Errors.ThrowIfError(LibChakraCore.JsDeleteIndexedProperty(@object, index));
        }

        public bool JsHasIndexedPropertiesExternalData(JavaScriptValueSafeHandle @object)
        {
            bool value;
            Errors.ThrowIfError(LibChakraCore.JsHasIndexedPropertiesExternalData(@object, out value));
            return value;
        }

        public IntPtr JsGetIndexedPropertiesExternalData(JavaScriptValueSafeHandle @object, out JavaScriptTypedArrayType arrayType, out uint elementLength)
        {
            IntPtr data;
            Errors.ThrowIfError(LibChakraCore.JsGetIndexedPropertiesExternalData(@object, out data, out arrayType, out elementLength));
            return data;
        }

        public void JsSetIndexedPropertiesToExternalData(JavaScriptValueSafeHandle @object, IntPtr data, JavaScriptTypedArrayType arrayType, uint elementLength)
        {
            Errors.ThrowIfError(LibChakraCore.JsSetIndexedPropertiesToExternalData(@object, data, arrayType, elementLength));
        }

        public bool JsEquals(JavaScriptValueSafeHandle object1, JavaScriptValueSafeHandle object2)
        {
            bool result;
            Errors.ThrowIfError(LibChakraCore.JsEquals(object1, object2, out result));
            return result;
        }

        public bool JsStrictEquals(JavaScriptValueSafeHandle object1, JavaScriptValueSafeHandle object2)
        {
            bool result;
            Errors.ThrowIfError(LibChakraCore.JsStrictEquals(object1, object2, out result));
            return result;
        }

        public bool JsHasExternalData(JavaScriptValueSafeHandle @object)
        {
            bool value;
            Errors.ThrowIfError(LibChakraCore.JsHasExternalData(@object, out value));
            return value;
        }

        public IntPtr JsGetExternalData(JavaScriptValueSafeHandle @object)
        {
            IntPtr externalData;
            Errors.ThrowIfError(LibChakraCore.JsGetExternalData(@object, out externalData));
            return externalData;
        }

        public void JsSetExternalData(JavaScriptValueSafeHandle @object, IntPtr externalData)
        {
            Errors.ThrowIfError(LibChakraCore.JsSetExternalData(@object, externalData));
        }

        public JavaScriptValueSafeHandle JsCreateArray(uint length)
        {
            JavaScriptValueSafeHandle result;
            Errors.ThrowIfError(LibChakraCore.JsCreateArray(length, out result));
            result.NativeFunctionSource = nameof(LibChakraCore.JsCreateArray);
            if (result != JavaScriptValueSafeHandle.Invalid)
            {
				uint valueRefCount;
				Errors.ThrowIfError(LibChakraCore.JsAddRef(result, out valueRefCount));
			}
            return result;
        }

        public JavaScriptValueSafeHandle JsCreateArrayBuffer(uint byteLength)
        {
            JavaScriptValueSafeHandle result;
            Errors.ThrowIfError(LibChakraCore.JsCreateArrayBuffer(byteLength, out result));
            result.NativeFunctionSource = nameof(LibChakraCore.JsCreateArrayBuffer);
            if (result != JavaScriptValueSafeHandle.Invalid)
            {
				uint valueRefCount;
				Errors.ThrowIfError(LibChakraCore.JsAddRef(result, out valueRefCount));
			}
            return result;
        }

        public JavaScriptValueSafeHandle JsCreateExternalArrayBuffer(IntPtr data, uint byteLength, JavaScriptObjectFinalizeCallback finalizeCallback, IntPtr callbackState)
        {
            JavaScriptValueSafeHandle result;
            Errors.ThrowIfError(LibChakraCore.JsCreateExternalArrayBuffer(data, byteLength, finalizeCallback, callbackState, out result));
            result.NativeFunctionSource = nameof(LibChakraCore.JsCreateExternalArrayBuffer);
            if (result != JavaScriptValueSafeHandle.Invalid)
            {
				uint valueRefCount;
				Errors.ThrowIfError(LibChakraCore.JsAddRef(result, out valueRefCount));
			}
            return result;
        }

        public JavaScriptValueSafeHandle JsCreateTypedArray(JavaScriptTypedArrayType arrayType, JavaScriptValueSafeHandle baseArray, uint byteOffset, uint elementLength)
        {
            JavaScriptValueSafeHandle result;
            Errors.ThrowIfError(LibChakraCore.JsCreateTypedArray(arrayType, baseArray, byteOffset, elementLength, out result));
            result.NativeFunctionSource = nameof(LibChakraCore.JsCreateTypedArray);
            if (result != JavaScriptValueSafeHandle.Invalid)
            {
				uint valueRefCount;
				Errors.ThrowIfError(LibChakraCore.JsAddRef(result, out valueRefCount));
			}
            return result;
        }

        public JavaScriptValueSafeHandle JsCreateDataView(JavaScriptValueSafeHandle arrayBuffer, uint byteOffset, uint byteLength)
        {
            JavaScriptValueSafeHandle result;
            Errors.ThrowIfError(LibChakraCore.JsCreateDataView(arrayBuffer, byteOffset, byteLength, out result));
            result.NativeFunctionSource = nameof(LibChakraCore.JsCreateDataView);
            if (result != JavaScriptValueSafeHandle.Invalid)
            {
				uint valueRefCount;
				Errors.ThrowIfError(LibChakraCore.JsAddRef(result, out valueRefCount));
			}
            return result;
        }

        public JavaScriptTypedArrayType JsGetTypedArrayInfo(JavaScriptValueSafeHandle typedArray, out JavaScriptValueSafeHandle arrayBuffer, out uint byteOffset, out uint byteLength)
        {
            JavaScriptTypedArrayType arrayType;
            Errors.ThrowIfError(LibChakraCore.JsGetTypedArrayInfo(typedArray, out arrayType, out arrayBuffer, out byteOffset, out byteLength));
            arrayBuffer.NativeFunctionSource = nameof(LibChakraCore.JsGetTypedArrayInfo);
            if (arrayBuffer != JavaScriptValueSafeHandle.Invalid)
            {
				uint valueRefCount;
				Errors.ThrowIfError(LibChakraCore.JsAddRef(arrayBuffer, out valueRefCount));
			}
            return arrayType;
        }

        public IntPtr JsGetArrayBufferStorage(JavaScriptValueSafeHandle arrayBuffer, out uint bufferLength)
        {
            IntPtr buffer;
            Errors.ThrowIfError(LibChakraCore.JsGetArrayBufferStorage(arrayBuffer, out buffer, out bufferLength));
            return buffer;
        }

        public IntPtr JsGetTypedArrayStorage(JavaScriptValueSafeHandle typedArray, out uint bufferLength, out JavaScriptTypedArrayType arrayType, out int elementSize)
        {
            IntPtr buffer;
            Errors.ThrowIfError(LibChakraCore.JsGetTypedArrayStorage(typedArray, out buffer, out bufferLength, out arrayType, out elementSize));
            return buffer;
        }

        public IntPtr JsGetDataViewStorage(JavaScriptValueSafeHandle dataView, out uint bufferLength)
        {
            IntPtr buffer;
            Errors.ThrowIfError(LibChakraCore.JsGetDataViewStorage(dataView, out buffer, out bufferLength));
            return buffer;
        }

        public JavaScriptValueSafeHandle JsCallFunction(JavaScriptValueSafeHandle function, IntPtr[] arguments, ushort argumentCount)
        {
            JavaScriptValueSafeHandle result;
            Errors.ThrowIfError(LibChakraCore.JsCallFunction(function, arguments, argumentCount, out result));
            result.NativeFunctionSource = nameof(LibChakraCore.JsCallFunction);
            if (result != JavaScriptValueSafeHandle.Invalid)
            {
				uint valueRefCount;
				Errors.ThrowIfError(LibChakraCore.JsAddRef(result, out valueRefCount));
			}
            return result;
        }

        public JavaScriptValueSafeHandle JsConstructObject(JavaScriptValueSafeHandle function, IntPtr[] arguments, ushort argumentCount)
        {
            JavaScriptValueSafeHandle result;
            Errors.ThrowIfError(LibChakraCore.JsConstructObject(function, arguments, argumentCount, out result));
            result.NativeFunctionSource = nameof(LibChakraCore.JsConstructObject);
            if (result != JavaScriptValueSafeHandle.Invalid)
            {
				uint valueRefCount;
				Errors.ThrowIfError(LibChakraCore.JsAddRef(result, out valueRefCount));
			}
            return result;
        }

        public JavaScriptValueSafeHandle JsCreateFunction(JavaScriptNativeFunction nativeFunction, IntPtr callbackState)
        {
            JavaScriptValueSafeHandle function;
            Errors.ThrowIfError(LibChakraCore.JsCreateFunction(nativeFunction, callbackState, out function));
            function.NativeFunctionSource = nameof(LibChakraCore.JsCreateFunction);
            if (function != JavaScriptValueSafeHandle.Invalid)
            {
				uint valueRefCount;
				Errors.ThrowIfError(LibChakraCore.JsAddRef(function, out valueRefCount));
			}
            return function;
        }

        public JavaScriptValueSafeHandle JsCreateNamedFunction(JavaScriptValueSafeHandle name, JavaScriptNativeFunction nativeFunction, IntPtr callbackState)
        {
            JavaScriptValueSafeHandle function;
            Errors.ThrowIfError(LibChakraCore.JsCreateNamedFunction(name, nativeFunction, callbackState, out function));
            function.NativeFunctionSource = nameof(LibChakraCore.JsCreateNamedFunction);
            if (function != JavaScriptValueSafeHandle.Invalid)
            {
				uint valueRefCount;
				Errors.ThrowIfError(LibChakraCore.JsAddRef(function, out valueRefCount));
			}
            return function;
        }

        public JavaScriptValueSafeHandle JsCreateError(JavaScriptValueSafeHandle message)
        {
            JavaScriptValueSafeHandle error;
            Errors.ThrowIfError(LibChakraCore.JsCreateError(message, out error));
            error.NativeFunctionSource = nameof(LibChakraCore.JsCreateError);
            if (error != JavaScriptValueSafeHandle.Invalid)
            {
				uint valueRefCount;
				Errors.ThrowIfError(LibChakraCore.JsAddRef(error, out valueRefCount));
			}
            return error;
        }

        public JavaScriptValueSafeHandle JsCreateRangeError(JavaScriptValueSafeHandle message)
        {
            JavaScriptValueSafeHandle error;
            Errors.ThrowIfError(LibChakraCore.JsCreateRangeError(message, out error));
            error.NativeFunctionSource = nameof(LibChakraCore.JsCreateRangeError);
            if (error != JavaScriptValueSafeHandle.Invalid)
            {
				uint valueRefCount;
				Errors.ThrowIfError(LibChakraCore.JsAddRef(error, out valueRefCount));
			}
            return error;
        }

        public JavaScriptValueSafeHandle JsCreateReferenceError(JavaScriptValueSafeHandle message)
        {
            JavaScriptValueSafeHandle error;
            Errors.ThrowIfError(LibChakraCore.JsCreateReferenceError(message, out error));
            error.NativeFunctionSource = nameof(LibChakraCore.JsCreateReferenceError);
            if (error != JavaScriptValueSafeHandle.Invalid)
            {
				uint valueRefCount;
				Errors.ThrowIfError(LibChakraCore.JsAddRef(error, out valueRefCount));
			}
            return error;
        }

        public JavaScriptValueSafeHandle JsCreateSyntaxError(JavaScriptValueSafeHandle message)
        {
            JavaScriptValueSafeHandle error;
            Errors.ThrowIfError(LibChakraCore.JsCreateSyntaxError(message, out error));
            error.NativeFunctionSource = nameof(LibChakraCore.JsCreateSyntaxError);
            if (error != JavaScriptValueSafeHandle.Invalid)
            {
				uint valueRefCount;
				Errors.ThrowIfError(LibChakraCore.JsAddRef(error, out valueRefCount));
			}
            return error;
        }

        public JavaScriptValueSafeHandle JsCreateTypeError(JavaScriptValueSafeHandle message)
        {
            JavaScriptValueSafeHandle error;
            Errors.ThrowIfError(LibChakraCore.JsCreateTypeError(message, out error));
            error.NativeFunctionSource = nameof(LibChakraCore.JsCreateTypeError);
            if (error != JavaScriptValueSafeHandle.Invalid)
            {
				uint valueRefCount;
				Errors.ThrowIfError(LibChakraCore.JsAddRef(error, out valueRefCount));
			}
            return error;
        }

        public JavaScriptValueSafeHandle JsCreateURIError(JavaScriptValueSafeHandle message)
        {
            JavaScriptValueSafeHandle error;
            Errors.ThrowIfError(LibChakraCore.JsCreateURIError(message, out error));
            error.NativeFunctionSource = nameof(LibChakraCore.JsCreateURIError);
            if (error != JavaScriptValueSafeHandle.Invalid)
            {
				uint valueRefCount;
				Errors.ThrowIfError(LibChakraCore.JsAddRef(error, out valueRefCount));
			}
            return error;
        }

        public bool JsHasException()
        {
            bool hasException;
            Errors.ThrowIfError(LibChakraCore.JsHasException(out hasException));
            return hasException;
        }

        public JavaScriptValueSafeHandle JsGetAndClearException()
        {
            JavaScriptValueSafeHandle exception;
            Errors.ThrowIfError(LibChakraCore.JsGetAndClearException(out exception));
            exception.NativeFunctionSource = nameof(LibChakraCore.JsGetAndClearException);
            if (exception != JavaScriptValueSafeHandle.Invalid)
            {
				uint valueRefCount;
				Errors.ThrowIfError(LibChakraCore.JsAddRef(exception, out valueRefCount));
			}
            return exception;
        }

        public void JsSetException(JavaScriptValueSafeHandle exception)
        {
            Errors.ThrowIfError(LibChakraCore.JsSetException(exception));
        }

        public void JsDisableRuntimeExecution(JavaScriptRuntimeSafeHandle runtime)
        {
            Errors.ThrowIfError(LibChakraCore.JsDisableRuntimeExecution(runtime));
        }

        public void JsEnableRuntimeExecution(JavaScriptRuntimeSafeHandle runtime)
        {
            Errors.ThrowIfError(LibChakraCore.JsEnableRuntimeExecution(runtime));
        }

        public bool JsIsRuntimeExecutionDisabled(JavaScriptRuntimeSafeHandle runtime)
        {
            bool isDisabled;
            Errors.ThrowIfError(LibChakraCore.JsIsRuntimeExecutionDisabled(runtime, out isDisabled));
            return isDisabled;
        }

        public void JsSetPromiseContinuationCallback(JavaScriptPromiseContinuationCallback promiseContinuationCallback, IntPtr callbackState)
        {
            Errors.ThrowIfError(LibChakraCore.JsSetPromiseContinuationCallback(promiseContinuationCallback, callbackState));
        }

        public void JsDiagStartDebugging(JavaScriptRuntimeSafeHandle runtimeHandle, JavaScriptDiagDebugEventCallback debugEventCallback, IntPtr callbackState)
        {
            Errors.ThrowIfError(LibChakraCore.JsDiagStartDebugging(runtimeHandle, debugEventCallback, callbackState));
        }

        public IntPtr JsDiagStopDebugging(JavaScriptRuntimeSafeHandle runtimeHandle)
        {
            IntPtr callbackState;
            Errors.ThrowIfError(LibChakraCore.JsDiagStopDebugging(runtimeHandle, out callbackState));
            return callbackState;
        }

        public void JsDiagRequestAsyncBreak(JavaScriptRuntimeSafeHandle runtimeHandle)
        {
            Errors.ThrowIfError(LibChakraCore.JsDiagRequestAsyncBreak(runtimeHandle));
        }

        public JavaScriptValueSafeHandle JsDiagGetBreakpoints()
        {
            JavaScriptValueSafeHandle breakpoints;
            Errors.ThrowIfError(LibChakraCore.JsDiagGetBreakpoints(out breakpoints));
            breakpoints.NativeFunctionSource = nameof(LibChakraCore.JsDiagGetBreakpoints);
            if (breakpoints != JavaScriptValueSafeHandle.Invalid)
            {
				uint valueRefCount;
				Errors.ThrowIfError(LibChakraCore.JsAddRef(breakpoints, out valueRefCount));
			}
            return breakpoints;
        }

        public JavaScriptValueSafeHandle JsDiagSetBreakpoint(uint scriptId, uint lineNumber, uint columnNumber)
        {
            JavaScriptValueSafeHandle breakpoint;
            Errors.ThrowIfError(LibChakraCore.JsDiagSetBreakpoint(scriptId, lineNumber, columnNumber, out breakpoint));
            breakpoint.NativeFunctionSource = nameof(LibChakraCore.JsDiagSetBreakpoint);
            if (breakpoint != JavaScriptValueSafeHandle.Invalid)
            {
				uint valueRefCount;
				Errors.ThrowIfError(LibChakraCore.JsAddRef(breakpoint, out valueRefCount));
			}
            return breakpoint;
        }

        public void JsDiagRemoveBreakpoint(uint breakpointId)
        {
            Errors.ThrowIfError(LibChakraCore.JsDiagRemoveBreakpoint(breakpointId));
        }

        public void JsDiagSetBreakOnException(JavaScriptRuntimeSafeHandle runtimeHandle, JavaScriptDiagBreakOnExceptionAttributes exceptionAttributes)
        {
            Errors.ThrowIfError(LibChakraCore.JsDiagSetBreakOnException(runtimeHandle, exceptionAttributes));
        }

        public JavaScriptDiagBreakOnExceptionAttributes JsDiagGetBreakOnException(JavaScriptRuntimeSafeHandle runtimeHandle)
        {
            JavaScriptDiagBreakOnExceptionAttributes exceptionAttributes;
            Errors.ThrowIfError(LibChakraCore.JsDiagGetBreakOnException(runtimeHandle, out exceptionAttributes));
            return exceptionAttributes;
        }

        public void JsDiagSetStepType(JavaScriptDiagStepType stepType)
        {
            Errors.ThrowIfError(LibChakraCore.JsDiagSetStepType(stepType));
        }

        public JavaScriptValueSafeHandle JsDiagGetScripts()
        {
            JavaScriptValueSafeHandle scriptsArray;
            Errors.ThrowIfError(LibChakraCore.JsDiagGetScripts(out scriptsArray));
            scriptsArray.NativeFunctionSource = nameof(LibChakraCore.JsDiagGetScripts);
            if (scriptsArray != JavaScriptValueSafeHandle.Invalid)
            {
				uint valueRefCount;
				Errors.ThrowIfError(LibChakraCore.JsAddRef(scriptsArray, out valueRefCount));
			}
            return scriptsArray;
        }

        public JavaScriptValueSafeHandle JsDiagGetSource(uint scriptId)
        {
            JavaScriptValueSafeHandle source;
            Errors.ThrowIfError(LibChakraCore.JsDiagGetSource(scriptId, out source));
            source.NativeFunctionSource = nameof(LibChakraCore.JsDiagGetSource);
            if (source != JavaScriptValueSafeHandle.Invalid)
            {
				uint valueRefCount;
				Errors.ThrowIfError(LibChakraCore.JsAddRef(source, out valueRefCount));
			}
            return source;
        }

        public JavaScriptValueSafeHandle JsDiagGetFunctionPosition(JavaScriptValueSafeHandle function)
        {
            JavaScriptValueSafeHandle functionPosition;
            Errors.ThrowIfError(LibChakraCore.JsDiagGetFunctionPosition(function, out functionPosition));
            functionPosition.NativeFunctionSource = nameof(LibChakraCore.JsDiagGetFunctionPosition);
            if (functionPosition != JavaScriptValueSafeHandle.Invalid)
            {
				uint valueRefCount;
				Errors.ThrowIfError(LibChakraCore.JsAddRef(functionPosition, out valueRefCount));
			}
            return functionPosition;
        }

        public JavaScriptValueSafeHandle JsDiagGetStackTrace()
        {
            JavaScriptValueSafeHandle stackTrace;
            Errors.ThrowIfError(LibChakraCore.JsDiagGetStackTrace(out stackTrace));
            stackTrace.NativeFunctionSource = nameof(LibChakraCore.JsDiagGetStackTrace);
            if (stackTrace != JavaScriptValueSafeHandle.Invalid)
            {
				uint valueRefCount;
				Errors.ThrowIfError(LibChakraCore.JsAddRef(stackTrace, out valueRefCount));
			}
            return stackTrace;
        }

        public JavaScriptValueSafeHandle JsDiagGetStackProperties(uint stackFrameIndex)
        {
            JavaScriptValueSafeHandle properties;
            Errors.ThrowIfError(LibChakraCore.JsDiagGetStackProperties(stackFrameIndex, out properties));
            properties.NativeFunctionSource = nameof(LibChakraCore.JsDiagGetStackProperties);
            if (properties != JavaScriptValueSafeHandle.Invalid)
            {
				uint valueRefCount;
				Errors.ThrowIfError(LibChakraCore.JsAddRef(properties, out valueRefCount));
			}
            return properties;
        }

        public JavaScriptValueSafeHandle JsDiagGetProperties(uint objectHandle, uint fromCount, uint totalCount)
        {
            JavaScriptValueSafeHandle propertiesObject;
            Errors.ThrowIfError(LibChakraCore.JsDiagGetProperties(objectHandle, fromCount, totalCount, out propertiesObject));
            propertiesObject.NativeFunctionSource = nameof(LibChakraCore.JsDiagGetProperties);
            if (propertiesObject != JavaScriptValueSafeHandle.Invalid)
            {
				uint valueRefCount;
				Errors.ThrowIfError(LibChakraCore.JsAddRef(propertiesObject, out valueRefCount));
			}
            return propertiesObject;
        }

        public JavaScriptValueSafeHandle JsDiagGetObjectFromHandle(uint objectHandle)
        {
            JavaScriptValueSafeHandle handleObject;
            Errors.ThrowIfError(LibChakraCore.JsDiagGetObjectFromHandle(objectHandle, out handleObject));
            handleObject.NativeFunctionSource = nameof(LibChakraCore.JsDiagGetObjectFromHandle);
            if (handleObject != JavaScriptValueSafeHandle.Invalid)
            {
				uint valueRefCount;
				Errors.ThrowIfError(LibChakraCore.JsAddRef(handleObject, out valueRefCount));
			}
            return handleObject;
        }

        public JavaScriptValueSafeHandle JsDiagEvaluate(JavaScriptValueSafeHandle expression, uint stackFrameIndex, JavaScriptParseScriptAttributes parseAttributes)
        {
            JavaScriptValueSafeHandle evalResult;
            Errors.ThrowIfError(LibChakraCore.JsDiagEvaluate(expression, stackFrameIndex, parseAttributes, out evalResult));
            evalResult.NativeFunctionSource = nameof(LibChakraCore.JsDiagEvaluate);
            if (evalResult != JavaScriptValueSafeHandle.Invalid)
            {
				uint valueRefCount;
				Errors.ThrowIfError(LibChakraCore.JsAddRef(evalResult, out valueRefCount));
			}
            return evalResult;
        }

    }
}