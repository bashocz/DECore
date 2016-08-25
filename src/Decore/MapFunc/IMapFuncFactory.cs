using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Decore
{
    internal interface IMapFuncFactory
    {
        Func<TIn, TOut> Get<TIn, TOut>();
    }
}
