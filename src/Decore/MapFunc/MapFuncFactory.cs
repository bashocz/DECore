using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
// ReSharper disable InconsistentlySynchronizedField

namespace Decore.MapFunc
{
    internal sealed class MapFuncFactory : IMapFuncFactory
    {
        private static readonly Lazy<IMapFuncFactory> DefaultLazy = new Lazy<IMapFuncFactory>(() => new MapFuncFactory(), LazyThreadSafetyMode.ExecutionAndPublication);
        public static IMapFuncFactory Default => DefaultLazy.Value;

        private readonly IDictionary<Tuple<Type, Type>, object> _funcStore = new ConcurrentDictionary<Tuple<Type, Type>, object>();
        private readonly Func<IMapFuncBuilder> _builderFactoryMethod;

        private readonly object _lockGet = new object();

        private MapFuncFactory()
            :this(null) { }

        internal MapFuncFactory(Func<IMapFuncBuilder> builderFactoryMethod)
        {
            _builderFactoryMethod = builderFactoryMethod;
        }

        public Func<TIn, TOut> Get<TIn, TOut>()
        {
            var key = new Tuple<Type, Type>(typeof(TIn), typeof(TOut));
            if (_funcStore.ContainsKey(key))
                return _funcStore[key] as Func<TIn, TOut>;

            lock (_lockGet)
            {
                if (_funcStore.ContainsKey(key))
                    return _funcStore[key] as Func<TIn, TOut>;

                var func = BuildFunc<TIn, TOut>();
                _funcStore.Add(key, func);
                return func;
            }
        }

        internal Func<TIn, TOut> BuildFunc<TIn, TOut>()
        {
            IMapFuncBuilder builder = GetBuilder();

            return builder?.Build<TIn, TOut>() ?? DefaultFunc<TIn, TOut>;
        }

        internal IMapFuncBuilder GetBuilder()
        {
            return _builderFactoryMethod?.Invoke();
        }

        internal TOut DefaultFunc<TIn, TOut>(TIn source)
        {
            return default(TOut);
        }
    }
}
