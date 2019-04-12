using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryHoldingTestWorkflows
{
    public class SomeWorkflow
    {
        public void Fail_if__is_odd(int evenInput)
        {
            Assert.That(evenInput % 2 == 0, $"Input [{evenInput}] should have been even.");
        }
    }
}
