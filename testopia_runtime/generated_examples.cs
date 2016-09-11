using System;
using System.Collections.Generic;
using System.Linq;

namespace Tabula
{
    public class paragraphAndTable_generated
        : GeneratedScenarioBase, IGeneratedScenario
    {
        public FunkyWorkflow Funky;
        public EvaluateResultsWorkflow EvaluateResults;
        public Table table_from_007_to_009;
        public ScenarioContext Context;

        public paragraphAndTable_generated(TabulaStepRunner runner)
            : base(runner)
        { }

        public void ExecuteScenario()
        {
            Funky = new FunkyWorkflow(Context);
            EvaluateResults = new EvaluateResultsWorkflow(Context);

            TableOverParagraph(table_from_007_to_009, paragraph_from_004_to_006);
        }

        public void paragraph_from_004_to_006()
        {
            //? The filename:line is a combined literal for dev-searchability
            Do(() =>    Funky.I_do__( val["things"] ),    "paragraphAndTable.scn:4", "Funky.I_do__( val[\"things\"] )");
            Do(() =>    Funky.Then_I_create__( val["stuff"] ),    "paragraphAndTable.scn:5", "Funky.Then_I_create__( val[\"stuff\"] )");
            Do(() =>    EvaluateResults.Both__and__should_show__results( val["things"], val["stuff"], "good" ),    "paragraphAndTable.scn:6", "EvaluateResults.Both__and__should_show__results( val[\"things\"], val[\"stuff\"], \"good\" )");
        }

        table_from_007_to_009 = new Table {
            Header = new List<string>     { "Things"    , "Stuff"     },
            Data = new List<List<string>> {
                new List<string>          { "email"     , "paperwork" },
                new List<string>          { "groceries" , "dinner"    }
            }
        };
    }


            //  For sake of this example, this text is found in the file paragraphAndTable.scn
    public const string scenarioText =
@"Scenario: ""This and That""
>use: Funky Workflow, EvaluateResultsWorkflow

I do #things
Then I create #stuff
? Both #things and #stuff should show ""good"" results
[ Things    | Stuff     ]
| email     | paperwork |
| groceries | dinner    |
";
}
