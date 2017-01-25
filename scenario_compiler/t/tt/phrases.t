use Test;
use Tabula::Grammar-Testing;
use Tabula::Fixture-Class;

my (&parser, $actions) = curry-parser-emitting-Testopia( "Phrases" );
# my $context = $actions.Context;
#
# $context.file-name = "SampleScenario.scn";

#if False
{   diag "phrases correctly compose from parts";

    my $parse = parser( "Phrases: Words", 'three bare words' );
    ok $parse.defined, "recognizes bare words";
    is $parse.made, 'three bare words', 'Phrases are not innately quoted';

    $parse = parser( "Phrases: String", '"why not?"' );
    ok $parse.defined, "recognizes quoted string as Phrase";
    is $parse.made, '"why not?"', 'Phrase:String should be quoted';

    $parse = parser( "Phrases: String", '"why, or why not?"' );
    ok $parse.defined, "Recognizes quoted string with comma as single String Phrase";
    is $parse.made, '"why, or why not?"', 'Phrases with String containing comma is still only one string';
}
