using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryHoldingTestWorkflows
{
    public class DivisionManagement : MvcBaseWorkflow
    {
        public void This_step_fails()
        {
            Assert.Fail("You see?  I told you.");
        }
    }
}
