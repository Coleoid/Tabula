use Test;
use Tabula::Grammar-Testing;
use Tabula::Fixture-Class;

my (&parser, $actions) = curry-parser-emitting-Testopia( "Table-Cell" );
# my $context = $actions.Context;
#
# $context.file-name = "SampleScenario.scn";

#if False
{   diag "Table-Cell correctly composes from parts";

    my $parse = parser( "Table-Cell: just words", 'three bare words' );
    ok $parse.defined, "recognizes bare words";
    is $parse.made, '"three bare words"', 'Words in a Table-Cell should be quoted';

    $parse = parser( "Table-Cell: String", '"why not?"' );
    ok $parse.defined, "String counts as a Table-Cell";
    is $parse.made, '"why not?"', 'Table-Cell string has one set of quotes';

    $parse = parser( "Table-Cell: String", '"why, or why not?"' );
    ok $parse.defined, "Recognizes quoted string with comma as single String Table-Cell";
    is $parse.made, '"why, or why not?"', 'Table-Cell with String containing comma is still only one string';

    $parse = parser( "Table-Cell: String", 'tick, tock' );
    ok $parse.defined, "Unquoted text with comma as single (Phrases) Table-Cell";
    is $parse.made, 'tick, tock', 'Table-Cell with Phrases, got it!';
}

#if False
{   diag "Table-Cell can be a list";

    my $parse = parser( "Table-Cell: list", '#this, #that' );
    ok $parse.defined, "recognizes lists";
    is $parse.made, 'var["this"], var["that"]', 'expresses a list tolerably';
}


(&parser, $actions) = curry-parser-emitting-Testopia( "Table-Cells" );

{   diag "Table-Cells might be singular";

    my $parse = parser( "Table-Cells: single-word phrase", 'sma' );
    ok $parse.defined, "recognizes single word phrase";
    is $parse.made, 'sma', 'Phrases in Table-Cells should be quoted';
}
