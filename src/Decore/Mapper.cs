using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Decore
{
    public class Mapper<TIn, TOut> : IMapper<TIn, TOut>
        where TIn : class
        where TOut : class, new()
    {
        private Func<TIn, TOut> _mapFunc;
        private readonly IMapFuncBuilder _builder;

        public Mapper(IMapFuncBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            _builder = builder;
        }

        internal Mapper(Func<TIn, TOut> mapFunc)
        {
            if (mapFunc == null)
                throw new ArgumentNullException(nameof(mapFunc));
            _mapFunc = mapFunc;
        }

        internal Func<TIn, TOut> MapFunc => _mapFunc ?? (_mapFunc = _builder.CreateMapFunc<TIn, TOut>());

        public TOut Map(TIn source)
        {
            return MapFunc(source);
        }
    }
}
