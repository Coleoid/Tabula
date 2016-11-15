using System;
using System.Collections.Generic;
using System.Linq;

namespace Tabula
{
    public interface IGeneratedScenario
    {
        void ExecuteScenario();
    }

    public class SampleTabulaScenario_generated
        : GeneratedScenarioBase, IGeneratedScenario
    {
        private EditPersonWorkflow EditPerson;

        public SampleTabulaScenario_generated(TabulaStepRunner runner)
            : base(runner)
        { }

        public void ExecuteScenario()
        {
            //  Instances of Workflows which are used by this scenario
            EditPerson = new EditPersonWorkflow();

            paragraph_from_021_to_025();
        }

        public void paragraph_from_021_to_025()
        {
            Do(() =>   EditPerson.AddPerson("Sammy", "Weezel"),     "SampleTabulaScenario.scn:21", "wfEditPerson.AddPerson(\"Sammy\", \"Weezel\")");
            Unfound(   "Add another dude named \"Bart\" \"Wiegmans\"",     "SampleTabulaScenario.scn:22");
            Do(() =>   EditPerson.AddPerson_but_fail("Formica", "Farragut"),     "SampleTabulaScenario.scn:23", "wfEditPerson.AddPerson_but_fail(\"Formica\", \"Farragut\")");
            Do(() =>   EditPerson.Add_some_more_but_die_horribly("Paulie"),     "SampleTabulaScenario.scn:24", "wfEditPerson.Add_some_more_but_die_horribly(\"Paulie\")");
            Do(() =>   EditPerson.AddPerson("Nevva", "Gothere"),     "SampleTabulaScenario.scn:25", "wfEditPerson.AddPerson(\"Nevva\", \"Gothere\")");
        }
    }
}
