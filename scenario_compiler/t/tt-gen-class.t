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
{   diag "The ExecuteScenario method calls a lone paragraph";

    my $scribe = $actions.Scribe;

    my $scenario = q:to/EOS/;
    Scenario:  "This and That"

    do a thing
    do another thing
    EOS

    $scribe.clear-scenario();
    my $parse = parser( "single para", $scenario );

    my $expected = q:to/EOC/;
            public void ExecuteScenario()
            {
                paragraph_from_003_to_004();
            }
    EOC

    my $output-method = $scribe.class-ExecuteScenario;
    is $output-method, $expected, "Scenario with one paragraph calls it in ExecuteScenario";

    $expected = q:to/EOP/;

            public void paragraph_from_003_to_004()
            {
                Unfound(     "do a thing",     "ScenarioFilename.scn:3" );
                Unfound(     "do another thing",     "ScenarioFilename.scn:4" );
            }
    EOP

    my $output-paragraphs = $scribe.get-section-declarations();
    is $output-paragraphs, $expected, "Scenario with one paragraph declares it";
}

#if False
{   diag "The ExecuteScenario method runs a paragraph across all rows of a following table";

    my $scribe = $actions.Scribe;
    my $scenario = q:to/EOS/;
    Scenario:  "This and That"

    do a thing with #lhs
    do another thing with #rhs

    [ lhs | rhs ]
    | abc | xyz |
    | 012 | 789 |
    EOS

    $scribe.clear-scenario();
    my $parse = parser( "table following para", $scenario );

    my $expected = q:to/EOC/;
            public void ExecuteScenario()
            {
                Run_para_over_table( paragraph_from_003_to_004, table_from_006_to_008 );
            }
    EOC

    my $output-method = $scribe.get-class-execute-scenario();
    is $output-method, $expected, "Scenario with one paragraph-table pair runs para for each row";

}


done-testing;
