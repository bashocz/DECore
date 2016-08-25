using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Decore
{
    public class Mapper<TIn, TOut> : IMapper<TIn, TOut>
    {
        private Func<TIn, TOut> _mapFunc;
        private readonly IMapFuncBuilder _builder;

        public Mapper(Func<TIn, TOut> mapFunc)
        {
            if (mapFunc == null)
                throw new ArgumentNullException(nameof(mapFunc));
            _mapFunc = mapFunc;
        }

        internal Mapper(IMapFuncBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            _builder = builder;
        }

        internal Func<TIn, TOut> MapFunc => _mapFunc ?? (_mapFunc = _builder.CreateMapFunc<TIn, TOut>());

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
