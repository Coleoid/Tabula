use Test;
use Tabula::Grammar-Testing;
use Tabula::Fixture-Class;

my (&parser, $actions) = curry-parser-emitting-Testopia( "Table-Cells" );
# my $context = $actions.Context;
#
# $context.file-name = "SampleScenario.scn";

#if False
{   diag "Table-Cells correctly compose from parts";

    my $parse = parser( "Table-Cells: just words", 'three bare words' );
    ok $parse.defined, "recognizes bare words";
    is $parse.made, '"three bare words"', 'Words in Table-Cells should be quoted';

    $parse = parser( "Table-Cells: Ympty", '' );
    ok $parse.defined, "Ympte";
    is $parse.made, ' ', 'Empte cell is space!';
}

#if False
{   diag "Table-Cells can be a list";

    my $parse = parser( "Table-Cells: list", '#this, #that' );
    ok $parse.defined, "recognizes lists";
    is $parse.made, 'var["this"], var["that"]', 'expresses a list tolerably';
}

done-testing;
