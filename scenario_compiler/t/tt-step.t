use Test;
use Tabula::Grammar-Testing;

my (&parser, $context) = curry-parser-emitting-Testopia( "Step" );
say "\n";

if False
{   diag "Emitting arguments into fixture calls";
    #TODO: Prep a repo of methods via hard-coding Advice Workflow and its method.

    my $parse = parser( "Step with simple args", 'please don\'t "kick" the "alligators"' );
    is $parse.made, 'Do( () =>    Advice.Please_dont__the__("kick", "alligators"),    "SampleTabulaScenario.scn:1", @"Advice.Please_dont__the__(""kick"", ""alligators"")" );',
        "outputs the expected line for one step";

    $parse = parser( "Same step, different args", 'please don\'t "feed" the "trolls"' );
    is $parse.made, 'Do( () =>    Advice.Please_dont__the__("feed", "trolls"),    "SampleTabulaScenario.scn:1", @"Advice.Please_dont__the__(""feed"", ""trolls"")" );',
        "outputs the expected args when changed";

    $parse = parser( "Quoting a numeric arg", 'please don\'t "disrespect" the 42' );
    is $parse.made, 'Do( () =>    Advice.Please_dont__the__("disrespect", "42"),    "SampleTabulaScenario.scn:1", @"Advice.Please_dont__the__(""disrespect"", ""42"")" );',
        "quotes numeric arg when signature takes string";

    $parse = parser( "Quoting a date arg", 'please don\'t "forget" the 9/11/2001' );
    is $parse.made, 'Do( () =>    Advice.Please_dont__the__("forget", "9/11/2001"),    "SampleTabulaScenario.scn:1", @"Advice.Please_dont__the__(""forget"", ""9/11/2001"")" );',
        "quotes date arg when signature takes string";

    $parse = parser( "Interpreting a variable arg", 'please don\'t "hassle" the #Hoff' );
    is $parse.made, 'Do( () =>    Advice.Please_dont__the__("hassle", val["Hoff"]),    "SampleTabulaScenario.scn:1", @"Advice.Please_dont__the__(""hassle"", val[""Hoff""])" );',
        "dereferences variable without further cast when signature takes string";
}

#if False
{   diag "Finding calls matching step text";
    my $lib = StepLibrary.new(name => 'Advice');
    $lib.steps{'thisisastep'} = 'This_is_a_step';
    $context.RegisterLibrary($lib);
    $context.AddLibraryToScope($lib);

    my $parse = parser( "find method with no args", 'this is a step' );
    is $parse.made, 'Do(() =>     Advice.This_is_a_step(),    "SampleScenario.scn:1", @"Advice.This_is_a_step()" );',
        "outputs the expected line for one step";

}


if False
{   diag "Emitting arguments into Tabula blocks"
}

if False
{   diag "Finding calls matching step args"
        #TODO:10 New test cases for argument interpretation
        #   One where we have a string where the signature takes an int, testing that it's cast properly
        #   One where the step has a variable
        #       And then where the v
}
