use Test;
use Tabula::Grammar-Testing;

my (&parser, $actions) = curry-parser-emitting-Testopia("Scenario");
my $context = $actions.Context;
$context.file-name = "ScenarioFilename.scn";


sub is-ExecuteScenario($sequence, $outcome, $scenario, $expected-ES) {
    my $expectation = "ExecuteScenario for $sequence has $outcome";

    my $scribe = $actions.Scribe;
    $scribe.start-class();
    my $parse = parser( $sequence, $scenario );

    is $scribe.generated-ExecuteScenario, $expected-ES, $expectation;
}


#if False
{
    my $scenario = q:to/EOS/;
    Scenario:  "This and That"

    do a thing
    do another thing
    EOS

    my $expected-ES = q:to/EOC/;
            public void ExecuteScenario()
            {
                paragraph_from_003_to_004();
            }
    EOC

    is-ExecuteScenario( '(para)', 'single paragraph', $scenario, $expected-ES );
}

#if False
{
    my $scenario = q:to/EOS/;
    Scenario:  "This and That"

    I am a talker
    This paragraph only happens once

    do a thing with #lhs
    do another thing with #rhs
    EOS

    my $expected-code = q:to/EOC/;
            public void ExecuteScenario()
            {
                paragraph_from_003_to_004();
                paragraph_from_006_to_007();
            }
    EOC

    is-ExecuteScenario( '(para, para)', 'each paragraph once', $scenario, $expected-code );
}


#if False
{
    my $scenario = q:to/EOS/;
    Scenario:  "This and That"

    do a thing with #lhs
    do another thing with #rhs

    [ lhs | rhs ]
    | abc | xyz |
    | 012 | 789 |
    EOS

    my $expected-code = q:to/EOC/;
            public void ExecuteScenario()
            {
                Run_para_over_table( paragraph_from_003_to_004, table_from_006_to_008 );
            }
    EOC

    is-ExecuteScenario( '(para, table)', 'paragraph run over table', $scenario, $expected-code );
}

#if False
{
    my $scenario = q:to/EOS/;
    Scenario:  "This and That"

    do a thing with #lhs
    do another thing with #rhs

    [ lhs | rhs ]
    | abc | xyz |
    | 012 | 789 |

    [ lhs | rhs ]
    | def | uvw |
    | 345 | 567 |
    | eee | ggg |
    | 012 | 789 |
    EOS

    my $expected-code = q:to/EOC/;
            public void ExecuteScenario()
            {
                Run_para_over_table( paragraph_from_003_to_004, table_from_006_to_008 );
                Run_para_over_table( paragraph_from_003_to_004, table_from_010_to_014 );
            }
    EOC

    is-ExecuteScenario( '(para, table, table)', 'paragraph run over each table', $scenario, $expected-code );
}

#if False
{
    my $scenario = q:to/EOS/;
    Scenario:  "This and That"

    I am a talker
    This paragraph only happens once

    do a thing with #lhs
    do another thing with #rhs

    [ lhs | rhs ]
    | abc | xyz |
    | 012 | 789 |
    EOS

    my $expected-code = q:to/EOC/;
            public void ExecuteScenario()
            {
                paragraph_from_003_to_004();
                Run_para_over_table( paragraph_from_006_to_007, table_from_009_to_011 );
            }
    EOC

    is-ExecuteScenario( '(para1, para2, table)', 'para1 alone and para2 over table', $scenario, $expected-code );
}


done-testing;
