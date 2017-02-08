use Test;
use Tabula::Grammar-Testing;
use Tabula::Fixture-Class;

my (&parser, $actions) = curry-parser-emitting-Testopia( "Phrases" );

#if False
{   diag "phrases correctly compose from parts";

    my $parse = parser( "Phrases: one phrase", 'three bare words' );
    ok $parse.defined, "recognizes a single phrase as a valid Phrases collection";
    is $parse.made, $parse<Phrase>[0].made, 'Phrase gets no extra decoration from collection';

    $parse = parser( "Phrase: two symbols", '#this, #that' );
    ok $parse.defined, "recognizes two symbols as phrases";
    is $parse<Phrase>.elems, 2, 'finds two phrases';
    is $parse<Phrase>[0].made, 'var["this"]', 'symbol is understood';
    is $parse.made, 'var["this"], var["that"]', 'list roll-up';

    #is $parse.made, '"why not?"', 'Phrase:String should be quoted';
}
