use Test;
use Grammar;
use Tabula_grammar_testing_utility;

say "\n";

#if False
{   diag "Troublesome Table tests";
    my &parser = curry-parser-for( 'Table' );

    my $table = q:to/EOT/;
        [ Supervisors ]
        | #Joan, #Bob |
        EOT

    my $parse = parser( "list in Table Cell", $table );
}
