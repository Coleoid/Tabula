use Test;
use Tabula::Grammar-Testing;
use Tabula::Fixture-Book;

my (&parser, $actions) = curry-parser-emitting-Testopia( "Action" );
my $context = $actions.Context;
say "\n";

if False  # Fantasy/Hypothetical code
{  diag "A step command creates a callable equivalent to other code";

    my $fixture = Fixture-Book.new(instance-name => "Advice", class-name => "AdviceWorkflow");
    $fixture.steps{'something()'} = "Some_Thing()";
    $fixture.steps{'thisisastep()'} = "This_is_a_step()";
    $context.RegisterLibrary($fixture);
    $context.AddLibraryToScope($fixture);

    my $parse = parser( "simple step definition", '>step: foo => this is a step' );

    #TODO:  test that it exists where and as expected (not yet determined)

    $parse = parser( "use the defined step", 'foo' );
    is $parse.made, 'Do(() =>     Advice.This_is_a_step(),     "SampleScenario.scn:1", @"Advice.This_is_a_step()" );',
        "a single-step definition is directly replaced with its target call";

    #TODO:  Test that arguments are passed
}

done-testing;
