use Test;
use Tabula::Grammar-Testing;

my (&parser, $composer) = curry-parser-emitting-Testopia( "Command-Alias" );
$composer.Context.file-name = "SampleScenario.scn";
say "\n";

#if False
{   diag "An alias command adds a findable step to the current scope";

    my $parse = parser( "simple alias", '>alias: "go #speed" means go at #speed for 4 hours' );
    ok $parse.defined, 'simple alias worked';


    my $alias = q:to/EOA/.chomp;
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

    $parse = parser( "alias to a block", $alias );
    ok $parse.defined, 'block alias worked';
    is $parse.made, 'alias', 'block alias worked';

}

done-testing;
