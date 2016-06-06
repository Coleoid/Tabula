use Test;
use Tabula::Grammar-Testing;

#if False
{   diag "We emit a C# class corresponding to our scenario";

    my (&parser, $context) = curry-parser-emitting-Testopia("Scenario");
    $context.file-name = "ScenarioFilename.scn";

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
            ScenarioName = "ScenarioFilename.scn:  This and That";

            public ScenarioFilename_generated(TabulaStepRunner runner)
                : base(runner)
            { }

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
