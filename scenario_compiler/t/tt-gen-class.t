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

#if False
{   diag "The ExecuteScenario method contains calls to paragraphs";

    my $scribe = $actions.Scribe;
    my $scenario = q:to/EOS/;
    Scenario:  "This and That"

    do a thing
    do another thing
    EOS

    my $expected-output = q:to/EOC/;
            public void ExecuteScenario()
            {
                paragraph_from_003_to_004();
            }
    EOC

    my $parse = parser( "empty scenario", $scenario );

    my $output-method = $scribe.get-class-execute-scenario();

    is $output-method, $expected-output, "Scenario with one paragraph calls it in ExecuteScenario";
}


done-testing;
