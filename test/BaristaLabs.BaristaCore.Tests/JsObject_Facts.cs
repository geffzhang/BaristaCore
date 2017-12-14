﻿namespace BaristaLabs.BaristaCore.Tests
{
    using BaristaCore.Extensions;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Xunit;

    [ExcludeFromCodeCoverage]
    public class JsObject_Facts
    {
        private readonly ServiceCollection ServiceCollection;
        private readonly IServiceProvider m_provider;

        public JsObject_Facts()
        {
            ServiceCollection = new ServiceCollection();
            ServiceCollection.AddBaristaCore();

            m_provider = ServiceCollection.BuildServiceProvider();
        }

        public IBaristaRuntimeService BaristaRuntimeService
        {
            get { return m_provider.GetRequiredService<IBaristaRuntimeService>(); }
        }

        [Fact]
        public void JsObjectCanBeCreated()
        {
            using (var rt = BaristaRuntimeService.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var obj = ctx.ValueService.CreateObject();
                        Assert.True(obj != null);
                        Assert.Equal(JavaScript.JavaScriptValueType.Object, obj.Type);
                    }
                }
            }
        }

        [Fact]
        public void JsObjectReportsHasProperty()
        {
            using (var rt = BaristaRuntimeService.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var script = "export default { foo: 'bar'};";
                        var result = ctx.EvaluateModule<JsObject>(script);
                        Assert.True(result.HasProperty("foo"));
                        Assert.True(result.HasProperty("toString"));
                    }
                }
            }
        }

        [Fact]
        public void JsObjectReportsHasOwnProperty()
        {
            using (var rt = BaristaRuntimeService.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var script = "export default { foo: 'bar'};";
                        var result = ctx.EvaluateModule<JsObject>(script);
                        Assert.True(result.HasOwnProperty("foo"));
                        Assert.False(result.HasOwnProperty("toString"));
                    }
                }
            }
        }

        [Fact]
        public void JsObjectKeysCanBeRetrieved()
        {
            using (var rt = BaristaRuntimeService.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var script = "export default { foo: 'bar'};";
                        var result = ctx.EvaluateModule<JsObject>(script);

                        var keys = result.Keys;
                        Assert.True(keys != null);
                        Assert.Equal(1, keys.Length);
                        Assert.Equal("foo", keys[0].ToString());
                    }
                }
            }
        }

        [Fact]
        public void JsObjectPropertyCanBeRetrievedByStringIndexer()
        {
            using (var rt = BaristaRuntimeService.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var script = "export default { foo: 'bar'};";
                        var result = ctx.EvaluateModule<JsObject>(script);

                        var bar = result["foo"];
                        Assert.True(bar != null);
                        Assert.Equal("bar", bar.ToString());
                    }
                }
            }
        }

        [Fact]
        public void JsObjectPropertyCanBeRetrievedByNumericIndexer()
        {
            using (var rt = BaristaRuntimeService.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var script = "export default { 0: 'bar'};";
                        var result = ctx.EvaluateModule<JsObject>(script);

                        var bar = result[0];
                        Assert.True(bar != null);
                        Assert.Equal("bar", bar.ToString());
                    }
                }
            }
        }

        [Fact]
        public void JsObjectPropertyCanBeRetrievedBySymbolIndexer()
        {
            using (var rt = BaristaRuntimeService.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var script = "export default { 'foo': 'bar'};";
                        var result = ctx.EvaluateModule<JsObject>(script);

                        var symbol = ctx.ValueService.CreateSymbol("baz");
                        result.SetProperty(symbol, ctx.ValueService.CreateString("qix"));

                        var qix = result[symbol];
                        Assert.True(qix != null);
                        Assert.Equal("qix", qix.ToString());
                    }
                }
            }
        }

        [Fact]
        public void JsObjectPropertyCanBeSetByStringIndexer()
        {
            using (var rt = BaristaRuntimeService.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var script = "export default { 'foo': 'bar'};";
                        var result = ctx.EvaluateModule<JsObject>(script);

                        var strValue = ctx.ValueService.CreateString("baz");
                        var qixValue = ctx.ValueService.CreateString("qix");

                        result[strValue] = qixValue;

                        Assert.True(result.HasOwnProperty("baz"));
                    }
                }
            }
        }

        [Fact]
        public void JsObjectPropertyCanBeSetByIntIndexer()
        {
            using (var rt = BaristaRuntimeService.CreateRuntime())
            {
                using (var ctx = rt.CreateContext())
                {
                    using (ctx.Scope())
                    {
                        var script = "export default { 'foo': 'bar'};";
                        var result = ctx.EvaluateModule<JsObject>(script);

                        var indexValue = ctx.ValueService.CreateNumber(0);
                        var qixValue = ctx.ValueService.CreateString("qix");

                        result[indexValue] = qixValue;

                        Assert.Equal("qix", result[0].ToString());
                    }
                }
            }
        }
    }
}
