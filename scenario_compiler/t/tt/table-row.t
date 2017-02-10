use Test;
use Tabula::Grammar-Testing;
use Tabula::Fixture-Class;


my $sma-out = '{ "sma" }';
my $wee-sma-out = '{ "wee", "sma" }';

#if False
{   diag "Table-Row minimal";
    my (&parser, $actions) = curry-parser-emitting-Testopia( "Table-Row" );

    my $parse = parser( "Table-Row: just words", '|sma|' ~ "\n" );
    ok $parse.defined, "recognizes single word";
    is $parse.made, $sma-out, 'brings it back';

    $parse = parser( "Table-Row: space word", '| sma |' ~ "\n" );
    ok $parse.defined, "recognizes word with whitespace";
    is $parse.made, $sma-out, 'brings it back';

    $parse = parser( "Table-Row: space words", '| wee | sma |' ~ "\n" );
    ok $parse.defined, "recognizes word with whitespace";
    is $parse.made, $wee-sma-out, 'brings words!';

}

{   diag "Table-Header minimal";
    my (&parser, $actions) = curry-parser-emitting-Testopia( "Table-Header" );

    my $parse = parser( "Table-Header: just words", '[sma]' ~ "\n" );
    ok $parse.defined, "recognizes single word";
    is $parse.made, $sma-out, 'brings it back';

    $parse = parser( "Table-Header: space words", '[ sma ]' ~ "\n" );
    ok $parse.defined, "recognizes word with whitespace";
    is $parse.made, $sma-out, 'brings it back';

}
