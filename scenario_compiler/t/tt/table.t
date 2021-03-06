use Test;
use Tabula::Grammar-Testing;
use Tabula::Fixture-Class;

my (&parser, $actions) = get-parser-emitting-Testopia( "Table" );

#if False
{   diag "Table so small";

    my $parse = parser( '[ sma ]' ~ "\n" );
    ok $parse.defined, "a single header cell is plenty";
}

#if False
{   diag "Table not too big";

    my $table = q:to/EOS/;
    [ this | that ]
    | near | far |
    | hither | yon, and away |
    EOS

    my $parse = parser( $table );
    ok $parse.defined, "recognizes a table";
    ok $parse<Table-Header>.defined, 'I see the header';
    is $parse<Table-Row>.elems, 2, "two rows";
}

done-testing;
