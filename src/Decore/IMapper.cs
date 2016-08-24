using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Decore
{
    public interface IMapper<in TIn, out TOut>
        where TIn: class
        where TOut : class, new()
    {
        TOut Map(TIn source);
    }
}
