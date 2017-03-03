use Test;
use Tabula::Grammar-Testing;

say "\n";



#if False
{   diag "Table-Row failing empty and unspaced cells";

    # Getting the grammar to handle cells with no whitespace,
    # with other content or not.
    #  The odd bit with this is that it finds an extra (empty) cell
    # at row end, instead of having a row-ending pipe.  Acceptable,
    # though I'll need to keep it in mind when processing later.

    my grammar mini-row {
        token ws         { \h* }
        rule Table-Row  { '|' <Table-Cells> \n }
        rule Table-Cells { <Table-Cell>+ % '|' }
        rule Table-Cell  { <.ws> <Phrases> <.ws> || <Empty-Cell> }
        token Empty-Cell { \h* }
        rule Phrases     { 'stub' }
    }

    my $grammar = mini-row.new;


    my $input = "stub";
    my $out = $grammar.parse( $input, :rule('Phrases') );
    ok $out ~~ Match, "parses stub 'Phrases'";

    $input = "stub";
    $out = $grammar.parse( $input, :rule('Table-Cells') );
    ok $out ~~ Match, "parses stub 'Table-Cells'";

    $input = "";
    $out = $grammar.parse( $input, :rule('Empty-Cell') );
    ok $out ~~ Match, "parses zero whitespace 'Empty-Cell'";

    $input = " ";
    $out = $grammar.parse( $input, :rule('Empty-Cell') );
    ok $out ~~ Match, "parses single whitespace 'Empty-Cell'";

    $input = "   ";
    $out = $grammar.parse( $input, :rule('Empty-Cell') );
    ok $out ~~ Match, "parses multiple whitespace 'Empty-Cell'";

    $input = "  ";
    $out = $grammar.parse( $input, :rule('Table-Cells') );
    ok $out ~~ Match, "parses multiple whitespace 'Table-Cells'";

    $input = "| stub |\n";
    $out = $grammar.parse( $input, :rule('Table-Row') );
    ok $out ~~ Match, "parses a single cell with whitespace and value";

    $input = "|stub|\n";
    $out = $grammar.parse( $input, :rule('Table-Row') );
    ok $out ~~ Match, "parses a single cell with no whitespace, just a value";

    $input = "|stub|  \n";
    $out = $grammar.parse( $input, :rule('Table-Row') );
    ok $out ~~ Match, "parses a row with whitespace after last cell";

    $input = "|stub|stub|\n";
    $out = $grammar.parse( $input, :rule('Table-Row') );
    ok $out ~~ Match, "parses multiple cells with no whitespace";

    $input = "||\n";
    $out = $grammar.parse( $input, :rule('Table-Row') );
    ok $out ~~ Match, "parses zero whitespace empty cell";

    $input = "| |\n";
    $out = $grammar.parse( $input, :rule('Table-Row') );
    ok $out ~~ Match, "parses single whitespace empty cell";

    $input = "|   |\n";
    $out = $grammar.parse( $input, :rule('Table-Row') );
    ok $out ~~ Match, "parses multi whitespace empty cell";

}
