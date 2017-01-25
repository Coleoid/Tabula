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
}
