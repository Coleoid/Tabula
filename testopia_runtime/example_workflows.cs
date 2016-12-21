using System;
using System.Collections.Generic;
using System.Linq;

//  Workflows living here to support the spike, normally would live in Acadis / 1F.
namespace Tabula
{

    public class TabulaWorkflow
    {
        public ScenarioContext Context;
        public TabulaWorkflow( ScenarioContext context )
        {
            Context = context;
        }
    }

    public class EditPersonWorkflow
    {
        int people = 0;
        string addedFirstName = string.Empty;
        string addedLastName = string.Empty;
        public void AddPerson(string firstName, string lastName)
        {
            people++;
            addedFirstName = firstName;
            addedLastName = lastName;
        }

        public void Verify_we_added____(string firstName, string lastName)
        {
            if (addedFirstName != firstName || addedLastName != lastName)
            {
                string message = String.Format("You have not added [{0} {1}]", firstName, lastName);
                throw new AssertionException(message);
            }
        }

        public void Try_to_add__but_die_horribly(string firstName)
        {
            throw new Exception("Some internal problem with adding " + firstName);
        }

        public void Fancify(string firstName, string lastName)
        {
            throw new NotImplementedException();
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
