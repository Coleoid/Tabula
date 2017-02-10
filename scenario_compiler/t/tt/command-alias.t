use Test;
use Tabula::Grammar-Testing;

my (&parser, $composer) = curry-parser-emitting-Testopia( "Command" );
$composer.Context.file-name = "SampleScenario.scn";
my $binder = $composer.Binder;
say "\n";

#if False
{   diag "A set command adds a variable to the current scope";

    my $parse = parser( "simple alias", '>alias: "go" means "go like gangbusters"' ~ "\n" );

    my $alias = q:to/EOA/;
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

}

done-testing;
