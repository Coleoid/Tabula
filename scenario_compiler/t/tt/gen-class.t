use Test;
use Tabula::Grammar-Testing;


my (&parser, $actions) = curry-parser-emitting-Testopia("Scenario");
my $context = $actions.Context;
$context.file-name = "ScenarioFilename.scn";

#if False
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


done-testing;
