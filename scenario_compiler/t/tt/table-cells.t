use Test;
use Tabula::Grammar-Testing;
use Tabula::Fixture-Class;

my (&parser, $actions) = get-parser-emitting-Testopia( "Table-Cells" );

#if False
{   diag "Table-Cells correctly compose from parts";

    my $parse = parser( 'three bare words' );
    ok $parse.defined, "recognizes bare words";
    is $parse.made, '"three bare words"', 'Words in Table-Cells should be quoted';

    $parse = parser( '' );
    ok $parse.defined, "Ympte";
    is $parse.made, ' ', 'Empte cell is space!';
}

#if False
{   diag "Table-Cells can be a list";

    my $parse = parser( '#this, #that' );
    ok $parse.defined, "recognizes lists";
    is $parse.made, 'var["this"], var["that"]', 'expresses a list tolerably';
}

done-testing;
