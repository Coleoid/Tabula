use Test;
use Tabula::Grammar-Testing;
use Tabula::Fixture-Class;

my (&parser, $actions) = get-parser-emitting-Testopia( "Phrase" );
# my $context = $actions.Context;
#
# $context.file-name = "SampleScenario.scn";

#if False
{   diag "phrases correctly compose from parts";

    my $parse = parser( 'three bare words' );
    ok $parse.defined, "recognizes bare words as Word x 3";
    is $parse.made, 'three bare words', 'Phrase should not itself be quoted';

    $parse = parser( '"why not?"' );
    ok $parse.defined, "recognizes quoted string as Phrase";
    is $parse.made, '"why not?"', 'Phrase:String should be quoted';

    $parse = parser( '"why, or why not?"' );
    ok $parse.defined, "Recognizes quoted string with comma as single String Phrase";
    is $parse.made, '"why, or why not?"', 'Phrase with String containing comma is still only one string';
}

done-testing;
