using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Decore.MapFunc
{
    internal sealed class MapFuncFactory : IMapFuncFactory
    {
        private static readonly Lazy<IMapFuncFactory> DefaultLazy = new Lazy<IMapFuncFactory>(() => new MapFuncFactory(), LazyThreadSafetyMode.ExecutionAndPublication);
        public static IMapFuncFactory Default => DefaultLazy.Value;

        private readonly IDictionary<Tuple<Type, Type>, object> _funcStore;
        private readonly Func<IMapFuncBuilder> _builderFactoryMethod;

        private MapFuncFactory()
            :this(null) { }

        internal MapFuncFactory(IDictionary<Tuple<Type, Type>, object> funcStore, Func<IMapFuncBuilder> builderFactoryMethod = null)
        {
            _funcStore = funcStore ?? new Dictionary<Tuple<Type, Type>, object>();
            _builderFactoryMethod = builderFactoryMethod;
        }

        public Func<TIn, TOut> Get<TIn, TOut>()
        {
            var key = new Tuple<Type, Type>(typeof(TIn), typeof(TOut));
            if (_funcStore.ContainsKey(key))
                return _funcStore[key] as Func<TIn, TOut>;

            var func = BuildFunc<TIn, TOut>();
            _funcStore.Add(key, func);
            return func;
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
