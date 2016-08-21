using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Decore
{
    public class Dummy
    {
        public int Summary(params int[] numbers)
        {
            return numbers.Sum();
        }
    }
}
