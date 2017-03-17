use Test;
use Tabula::Grammar-Testing;
use Tabula::Code-Scribe :test-content;

my (&parser, $actions) = get-parser-emitting-Testopia("Scenario");
my $context = $actions.Context;
$context.file-name = "ScenarioFilename.scn";


sub does-scenario-contain($scenario, $expected-code, $outcome) {
    my $expectation = "ExecuteScenario contains $outcome";

    my $scribe = $actions.Scribe;

    $scribe.clear-work();  # so that each parse has a clean slate.
    #  The slate is left dirty so that teacher (the unit tests) can
    # see the student's work.

    my $parse = parser( $scenario );

    is $scribe.trimmed-body-actions, $expected-code, $expectation;
}


#if False
{
    my $scenario = q:to/EOS/;
    Scenario:  "This and That"

    do a thing
    do another thing
    EOS

    my $expected-code = q:to/EOC/;
    paragraph_from_003_to_004();
    EOC

    does-scenario-contain( $scenario, $expected-code, 'single paragraph' );
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
    paragraph_from_003_to_004();
    paragraph_from_006_to_007();
    EOC

    does-scenario-contain( $scenario, $expected-code, 'each paragraph once' );
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
    Run_para_over_table( paragraph_from_003_to_004, table_from_006_to_008 );
    EOC

    does-scenario-contain( $scenario, $expected-code, 'paragraph run over table' );
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
    Run_para_over_table( paragraph_from_003_to_004, table_from_006_to_008 );
    Run_para_over_table( paragraph_from_003_to_004, table_from_010_to_014 );
    EOC

    does-scenario-contain( $scenario, $expected-code, 'paragraph run over each table' );
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
    paragraph_from_003_to_004();
    Run_para_over_table( paragraph_from_006_to_007, table_from_009_to_011 );
    EOC

    does-scenario-contain( $scenario, $expected-code, 'para1 alone and para2 over table' );
}


done-testing;
