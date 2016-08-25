using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Decore
{
    public interface IMapper<in TIn, out TOut>
    {
        TOut Map(TIn source);
        IEnumerable<TOut> MapAll(IEnumerable<TIn> source);
    }
}
