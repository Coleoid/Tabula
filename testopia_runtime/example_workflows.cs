using System;
using System.Collections.Generic;
using System.Linq;

namespace Tabula
{
    //  Code below not generated, just here to support fantasy syntax coding
    public class EditPersonWorkflow
    {
        public void AddPerson(string firstName, string lastName)
        {
            //la de dah
        }

        public void AddPerson_but_fail(string firstName, string lastName)
        {
            throw new AssertionException("Made you look");
        }

        public void Add_some_more_but_die_horribly(string firstName)
        {
            throw new Exception("Not for " + firstName);
        }

    }

    public class FunkyWorkflow : TabulaWorkflow
    {
        public FunkyWorkflow(ScenarioContext context)
            : base(context)
        {
        }

        public void I_do__(string action)
        { }

        public void Then_I_create__(string result)
        { }
    }

    public class EvaluateResultsWorkflow : TabulaWorkflow
    {
        public EvaluateResultsWorkflow(ScenarioContext context)
            : base(context)
        {
        }

        public void Both__and__should_show__results(string first, string second, string resultQuality)
        { }
    }

}
