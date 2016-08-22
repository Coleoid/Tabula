use Test;
use Tabula::Grammar-Testing;

my (&parser, $actions) = curry-parser-emitting-Testopia("Scenario");
my $context = $actions.Context;
$context.file-name = "ScenarioFilename.scn";


sub are-SectionDeclarations($sequence, $outcome, $scenario, $expected-SDs) {
    my $expectation = "SectionDeclarations for $sequence have $outcome";

    my $scribe = $actions.Scribe;
    $scribe.start-class();
    my $parse = parser( $sequence, $scenario );

    is $scribe.generated-SectionDeclarations, $expected-SDs, $expectation;
}


#if False
{
    my $scenario = q:to/EOS/;
    Scenario:  "This and That"

    do a thing
    do another thing
    EOS

    my $expected-SDs = q:to/EOP/;

            public void paragraph_from_003_to_004()
            {
                Unfound(     "do a thing",     "ScenarioFilename.scn:3" );
                Unfound(     "do another thing",     "ScenarioFilename.scn:4" );
            }
    EOP

    are-SectionDeclarations( '(para)', 'single paragraph', $scenario, $expected-SDs );
}

#if False
{  #  Note that the spacing in the tables will be handled via a different test
    my $scenario = q:to/EOS/;
    Scenario:  "This and That"

    [ this | that ]
    | near | far |
    | hither | yon |
    EOS

    my $expected-SDs = q:to/EOP/;

            table_from_003_to_005 = new Table {
                Header = new List<string>     { "this", "that" },
                Data = new List<List<string>> {
                    new List<string>          { "near", "far" },
                    new List<string>          { "hither", "yon" },
                }
            };
    EOP

    are-SectionDeclarations( '(para)', 'single table', $scenario, $expected-SDs );
}

#if False
{
    my $scenario = q:to/EOS/;
    Scenario:  "Only This"

    [ this ]
    | only |
    EOS

    my $expected-SDs = q:to/EOP/;

            table_from_003_to_004 = new Table {
                Header = new List<string>     { "this" },
                Data = new List<List<string>> {
                    new List<string>          { "only" },
                }
            };
    EOP

    are-SectionDeclarations( '(para)', 'tiny table', $scenario, $expected-SDs );
}


done-testing;
