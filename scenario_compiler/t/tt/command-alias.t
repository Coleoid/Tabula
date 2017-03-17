use Test;
use Tabula::Grammar-Testing;

my (&parser, $composer) = get-parser-emitting-Testopia( "Command" );
$composer.Context.file-name = "SampleScenario.scn";
say "\n";

#if False
{   diag "An alias command adds a findable step to the current scope";

    #TODO: this becomes a file of checks
    #my $arse = parser( "unrecognized command", '>format c:' );

    my $parse = parser( '>alias: "go #speed" means go at #speed for 4 hours' );
    ok $parse.defined, 'simple alias worked';


    my $alias-text = q:to/EOA/.chomp;
    >alias: "Add employment actions for #employeeName at #orgName" means ...
        >use: Employment Action Edit
        Browse to Add Employment Action for #employeeName at #orgName
        Set action to #actionName
        Set employment type to "Testopia #empType"
        Set appointment type to "Testopia #apptType"
        Set title to "Testopia #empTitle"
        Set status to #newStatus
        Set effective date to #effectiveDate
        Set comments to #comment
        Click Save
    .
    EOA

    $parse = parser( $alias-text );
    ok $parse.defined, 'block alias worked';

    my $alias = $composer.Context.find-alias('add employment actions for at');
    ok $alias.defined, 'found an alias';
}

done-testing;
