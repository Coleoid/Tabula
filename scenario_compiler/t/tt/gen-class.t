use Test;
use Tabula::Grammar-Testing;


my (&parser, $actions) = curry-parser-emitting-Testopia("Scenario");
my $context = $actions.Context;
$context.file-name = "ScenarioFilename.scn";

if False
{   diag "We emit a C# class corresponding to our scenario";

    my $scenario = q:to/EOS/;
    Scenario:  "This and That"

    EOS

    my $expected-class-output = q:to/EOC/;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    namespace Tabula
    {
        public class ScenarioFilename_generated
            : GeneratedScenarioBase, IGeneratedScenario
        {
            public string ScenarioLabel = "ScenarioFilename.scn:  "This and That"";

            public ScenarioFilename_generated(TabulaStepRunner runner)
                : base(runner)
            {
            }

            public void ExecuteScenario()
            {
            }
        }
    }
    EOC

    my $parse = parser( "empty scenario", $scenario );
    my $output-class = $parse.made;

    is $output-class, $expected-class-output, "empty scenario creates compilable empty class";
}

{   diag "The class has tables, paragraphs, and and an execution plan matching the scenario";

    my $scenario = q:to/EOS/;
    Scenario:  "This and That"

    Action one with #flavor argument
    Action two with #color and #attitude
    [ flavor | color | attitude ]
    | spicy  | red   | sassy    |
    | zesty  | green | lively   |

    EOS

    my $expected-class-output = q:to/EOC/;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    namespace Tabula
    {
        public class ScenarioFilename_generated
            : GeneratedScenarioBase, IGeneratedScenario
        {
            public string ScenarioLabel = "ScenarioFilename.scn:  "This and That"";

            public ScenarioFilename_generated(TabulaStepRunner runner)
                : base(runner)
            {
            }

            public void ExecuteScenario()
            {
            }
        }
    }
    EOC

    my $parse = parser( "empty scenario", $scenario );
    my $output-class = $parse.made;

    is $output-class, $expected-class-output, "empty scenario creates compilable empty class";
}


done-testing;
