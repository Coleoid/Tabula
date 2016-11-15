use Test;
use Tabula::Grammar-Testing;
use Tabula::Fixture-Binder;

my (&parser, $actions) = curry-parser-emitting-Testopia( "Command" );
my $context = $actions.Context;
say "\n";

#if False
{   diag "A use command adds a library to the current scope";

    my $lib = Fixture-Binder.new(instance-name => "Advice", class-name => "AdviceWorkflow");
    my $fixture = $lib.pull-fixture("Advice");
    ok $fixture, "Can pull fixture by its instance-name.";

    $fixture.add-method("Some_Thing()");
    $fixture.add-method("This_is_a_step()");
    $context.RegisterLibrary($lib);

    my $check-lib = Fixture-Binder.new(instance-name => "Check", class-name => "CheckWorkflow");
    $check-lib.add-fixture("Am_I_Done()");
    $context.RegisterLibrary($check-lib);

    is $context.current-scope.libraries.elems, 0, "begin with no libraries in scope";

    my $parse = parser( "use a workflow", ">use: Advice workflow" );
    is $context.current-scope.libraries.elems, 1, "after a use command, library is in scope";

    $parse = parser( "another workflow", ">use: Check workflow" );
    is $context.current-scope.libraries.elems, 2, "a different use command will add another library to scope";

    is $context.problems.elems, 0, "neither success case is a problem";


    $parse = parser( "duplicate workflow", ">use: Advice workflow" );
    is $context.current-scope.libraries.elems, 2, "a duplicate use command does not add library again";
    is $context.problems.elems, 1, "a use command with a duplicate workflow will mark a problem";
    is $context.problems[0], "Tried to add duplicate library <<advice>>.", "...with a sensible message";

    $parse = parser( "unfound workflow", ">use: No Such workflow" );
    is $context.current-scope.libraries.elems, 2, "a use command not finding a workflow will (le duh) not add one";
    is $context.problems.elems, 2, "a use command not finding a workflow will mark a problem";
    is $context.problems[1], "Did not find library <<nosuch>> to add to scope.", "...with a sensible message";

}

done-testing;
