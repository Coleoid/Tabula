use Test;
use Tabula::Grammar-Testing;

my (&parser, $actions) = curry-parser-emitting-Testopia( "Step" );
my $context = $actions.Context;
say "\n";

my $lib = StepLibrary.new(class-name => 'AdviceWorkflow');
$lib.steps{'thisisastep'} = ('This_is_a_step', ());
$lib.steps{'pleasedontthe'} = ('Please_dont__the__', (Str, Str));

$context.file-name = "SampleScenario.scn";
$context.RegisterLibrary($lib);
$context.AddLibraryToScope($lib);

#if False
{   diag "Finding calls matching step text";
    my $parse = parser( "find method with no args", 'this is a step' );
    is $parse.made, 'Do(() =>     Advice.This_is_a_step(),     "SampleScenario.scn:1", @"Advice.This_is_a_step()" );',
        "outputs the expected line for one step";
}

#if False
{   diag "Emitting arguments into fixture calls";

    my $parse = parser( "Step with simple args", 'please don\'t "kick" the "alligators"' );
    is $parse.made, 'Do(() =>     Advice.Please_dont__the__("kick", "alligators"),     "SampleScenario.scn:1", @"Advice.Please_dont__the__(""kick"", ""alligators"")" );',
        "outputs the expected line for one step";
}

#if False    #   this will be unified with the above block when it passes
{
    my $parse = parser( "Same step, different args", 'please don\'t "feed" the "trolls"' );
    is $parse.made, 'Do(() =>     Advice.Please_dont__the__("feed", "trolls"),     "SampleScenario.scn:1", @"Advice.Please_dont__the__(""feed"", ""trolls"")" );',
        "outputs the expected args when changed";

    $parse = parser( "Quoting a numeric arg", 'please don\'t "disrespect" the 42' );
    is $parse.made, 'Do(() =>     Advice.Please_dont__the__("disrespect", "42"),     "SampleScenario.scn:1", @"Advice.Please_dont__the__(""disrespect"", ""42"")" );',
        "quotes numeric arg when signature takes string";

    $parse = parser( "Quoting a date arg", 'please don\'t "forget" the 9/11/2001' );
    is $parse.made, 'Do(() =>     Advice.Please_dont__the__("forget", "9/11/2001"),     "SampleScenario.scn:1", @"Advice.Please_dont__the__(""forget"", ""9/11/2001"")" );',
        "quotes date arg when signature takes string";

    $parse = parser( "Interpreting a variable arg", 'please don\'t "hassle" the #Hoff' );
    is $parse.made, 'Do(() =>     Advice.Please_dont__the__("hassle", alias["Hoff"]),     "SampleScenario.scn:1", @"Advice.Please_dont__the__(""hassle"", alias[""Hoff""])" );',
        "dereferences variable without further cast when signature takes string";
}

#if False
{   diag "discriminate on method signature";
    my $parse = parser( "fail to find method", 'this is a step "which should not be found"' );
    is $parse.made, 'Unfound(     "this is a step \"which should not be found\"",     "SampleScenario.scn:1" );',
        "should not find a method when the step usage does not match the method signature";
}

if False
{   diag "Emitting arguments into Tabula blocks"
}

if False
{   diag "Finding calls matching step args"


        #TODO: New test cases for argument interpretation
        #   One where we have a string where the signature takes an int, testing that it's cast properly
        #   One where the step has a variable
        #       And then where the v
}

done-testing;
diag $context.problems;
