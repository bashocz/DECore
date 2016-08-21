using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Decore.Tests
{
    public class DummyTest
    {
        [Fact]
        public void Summary_AllPositive()
        {
            var dummy = new Dummy();

            var sum = dummy.Summary(1, 2, 3, 4);

            Assert.Equal(10, sum);
        }
    }
}
