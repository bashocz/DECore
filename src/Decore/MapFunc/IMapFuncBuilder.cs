using System;

namespace Decore.MapFunc
{
    internal interface IMapFuncBuilder
    {
        Func<TIn, TOut> Build<TIn, TOut>();
    }
}
