use Test;
use Tabula::Grammar-Testing;
use Tabula::Code-Scribe :test-content;

my (&parser, $actions) = curry-parser-emitting-Testopia("Scenario");
my $context = $actions.Context;
$context.file-name = "ScenarioFilename.scn";


sub are-SectionDeclarations($sequence, $outcome, $scenario, $expected-SDs) {
    my $expectation = "SectionDeclarations for $sequence have $outcome";

    my $scribe = $actions.Scribe;
    $scribe.clear-work();
    my $parse = parser( $sequence, $scenario );

    is $scribe.show-sections, $expected-SDs, $expectation;
}


#if False
{
    my $scenario = q:to/EOS/;
    Scenario:  "Doing things"

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
{
    my $scenario = q:to/EOS/;
    Scenario:  "This and That"

    [ this | that ]
    | near | far |
    | hither | yon |
    EOS

    my $expected-SDs = q:to/EOP/;
            public Table table_from_003_to_005()
            {
                return new Table {
                    Header = new List<string>     { "this", "that" },
                    Data = new List<List<string>> {
                        new List<string>          { "near", "far" },
                        new List<string>          { "hither", "yon" },
                    }
                };
            }
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
            public Table table_from_003_to_004()
            {
                return new Table {
                    Header = new List<string>     { "this" },
                    Data = new List<List<string>> {
                        new List<string>          { "only" },
                    }
                };
            }
    EOP

    are-SectionDeclarations( '(para)', 'tiny table', $scenario, $expected-SDs );
}

#if False
{
    my $scenario = q:to/EOS/;
    Scenario:  "This"

    [ this ]
    EOS

    my $expected-SDs = q:to/EOP/;
            public Table table_from_003_to_003()
            {
                return new Table {
                    Header = new List<string>     { "this" },
                    Data = new List<List<string>> {
                    }
                };
            }
    EOP

    are-SectionDeclarations( '(para)', 'empty table', $scenario, $expected-SDs );
}


done-testing;
