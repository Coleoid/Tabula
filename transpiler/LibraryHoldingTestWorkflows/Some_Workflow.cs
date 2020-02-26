using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryHoldingTestWorkflows
{
    public class Some_Workflow
    {
        public void Fail_if__is_odd(int evenInput)
        {
            Assert.That(evenInput % 2 == 0, $"Input [{evenInput}] should have been even.");
        }

        public void Hello_World()
        {
            // yep.
        }
    }
}
