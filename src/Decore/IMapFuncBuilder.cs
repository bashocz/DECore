using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Decore
{
    internal interface IMapFuncBuilder
    {
        Func<TIn, TOut> CreateMapFunc<TIn, TOut>();
    }
}
