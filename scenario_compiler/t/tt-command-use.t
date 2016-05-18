use Test;
use Tabula::Grammar-Testing;

my (&parser, $context) = curry-parser-emitting-Testopia( "Command" );
say "\n";

#if False
{   diag "A use command adds a library to the current scope";

    my $lib = StepLibrary.new(instance-name => "Advice", class-name => "AdviceWorkflow");
    $lib.steps{'something'} = "Some_Thing";
    $lib.steps{'thisisastep'} = "This_is_a_step";
    $context.RegisterLibrary($lib);

    my $check-lib = StepLibrary.new(instance-name => "Check", class-name => "CheckWorkflow");
    $check-lib.steps{'amidone'} = "Am_I_Done";
    $context.RegisterLibrary($check-lib);

    is $context.current-scope.libraries.elems, 0, "begin with no libraries in scope";

    my $parse = parser( "use a workflow", ">use: Advice workflow" );
    is $context.current-scope.libraries.elems, 1, "after a use command, library is in scope";

    $parse = parser( "another workflow", ">use: Check workflow" );
    is $context.current-scope.libraries.elems, 2, "a different use command will add another library to scope";

    is $context.Problems.elems, 0, "neither success case is a problem";


    $parse = parser( "duplicate workflow", ">use: Advice workflow" );
    is $context.current-scope.libraries.elems, 2, "a duplicate use command does not add library again";
    is $context.Problems.elems, 1, "a use command with a duplicate workflow will mark a problem";

    $parse = parser( "unfound workflow", ">use: No Such workflow" );
    is $context.current-scope.libraries.elems, 2, "a use command not finding a workflow will (le duh) not add one";
    is $context.Problems.elems, 1, "a use command not finding a workflow will mark a problem";

}
