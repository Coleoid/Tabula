use Test;
use Tabula::Grammar-Testing;
use Tabula::Fixture-Binder;

my (&parser, $composer) = curry-parser-emitting-Testopia( "Command" );
my $context = $composer.Context;
my $binder = $composer.Binder;
say "\n";


my $class = Fixture-Class.new(instance-name => "Advice", class-name => "AdviceWorkflow", namespace => 'foo');
$class.add-method("Some_Thing()");
$class.add-method("This_is_a_step()");

my $check-class = Fixture-Class.new(instance-name => "Check", class-name => "CheckWorkflow", namespace => 'foo');
$check-class.add-method("Am_I_Done()");

$binder.add-class($class);
$binder.add-class($check-class);


#if False
{   diag "A use command adds a fixture to the current scope";

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

{   diag "A use command will add multiple fixtures to the current scope";
    $context.open-scope("multi");
    is $context.current-scope.fixtures.elems, 0, "begin with no fixtures in scope";

    my $parse = parser( "use comma-separated workflows", ">use: Advice workflow, check" );
    is $context.current-scope.fixtures.elems, 2, "after use command with multiple elements, all fixtures are in scope";
}



done-testing;
