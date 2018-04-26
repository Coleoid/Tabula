using NUnit.Framework;

namespace LibraryHoldingTestWorkflows
{
    public class MvcBaseWorkflow : Workflow
    {
        public void This_step_is_in_MvcBaseWorkflow()
        {
            int x = 0;
            Assert.True(x == x);
        }
    }
}