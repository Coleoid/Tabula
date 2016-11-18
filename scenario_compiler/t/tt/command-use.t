use Test;
use Tabula::Grammar-Testing;
use Tabula::Fixture-Binder;

my (&parser, $composer) = curry-parser-emitting-Testopia( "Command" );
my $context = $composer.Context;
my $binder = $composer.Binder;
say "\n";

#if False
{   diag "A use command adds a fixture to the current scope";

    my $book = Fixture-Book.new(instance-name => "Advice", class-name => "AdviceWorkflow");
    $book.add-method("Some_Thing()");
    $book.add-method("This_is_a_step()");

    my $check-book = Fixture-Book.new(instance-name => "Check", class-name => "CheckWorkflow");
    $check-book.add-method("Am_I_Done()");

    $binder.bind-fixture($book);
    $binder.bind-fixture($check-book);


    is $context.current-scope.fixtures.elems, 0, "begin with no fixtures in scope";

    my $parse = parser( "use a workflow", ">use: Advice workflow" );
    is $context.current-scope.fixtures.elems, 1, "after a use command, fixture is in scope";

    $parse = parser( "another workflow", ">use: Check workflow" );
    is $context.current-scope.fixtures.elems, 2, "a different use command will add another fixture to scope";

    $parse = parser( "duplicate workflow", ">use: Advice workflow" );
    is $context.current-scope.fixtures.elems, 2, "a duplicate use command does not add fixture again";

    $parse = parser( "unfound workflow", ">use: No Such workflow" );
    is $context.current-scope.fixtures.elems, 2, "a use command not finding a workflow will (le duh) not add one";
}

done-testing;
