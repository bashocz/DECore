using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Decore.MapFunc;

namespace Decore
{
    public class Mapper<TIn, TOut> : IMapper<TIn, TOut>
    {
        private Func<TIn, TOut> _mapFunc;
        private readonly IMapFuncFactory _factory;

        public Mapper(Func<TIn, TOut> mapFunc)
        {
            if (mapFunc == null)
                throw new ArgumentNullException(nameof(mapFunc));
            _mapFunc = mapFunc;
        }

        internal Mapper(IMapFuncFactory factory)
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));
            _factory = factory;
        }

        internal Func<TIn, TOut> MapFunc => _mapFunc ?? (_mapFunc = _factory.Get<TIn, TOut>());

        public TOut Map(TIn source)
        {
            return MapFunc(source);
        }

        public IEnumerable<TOut> MapAll(IEnumerable<TIn> source)
        {
            return source.Select(Map);
        }
    }
}
