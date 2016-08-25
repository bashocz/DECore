using System;

namespace Decore.MapFunc
{
    internal interface IMapFuncBuilder
    {
        Func<TIn, TOut> Create<TIn, TOut>();
    }
}
