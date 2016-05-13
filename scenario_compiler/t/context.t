use Test;
use Tabula::Grammar-Testing;

#if False
{   diag "A Build-Context will fetch steps from libraries in scope";

    my $context = Build-Context.new;

    ok $context, "get a context via construction";
    is $context.scopes.elems, 1, "start with one scope";

    # Phony up a library for testing
    my $lib = StepLibrary.new(name => "AdviceWorkflow");
    $lib.steps{'notthis'} = "Not_This";

    # A ton of this happens at startup time
    $context.RegisterLibrary($lib);

    my &parser = curry-parser-emitting-Testopia("Step");
    my $match = parser("Basic step found in library", "this is A STEP");

    my ($success, $call) = $context.GetFixtureCall($match);
    nok $success, "With no libraries in scope, we find no method";
    is $call, "this is A STEP", "failure returns original step text";

    # This happens when we encounter a ">use: advice workflow" command
    $context.AddLibraryToScope($lib);

    ($success, $call) = $context.GetFixtureCall($match);
    nok $success, "With libraries in scope but no matching method, we find no method";

    # This would not normally happen in mid-run, but it's useful to do now for testing
    $lib.steps{'thisisastep'} = "This_is_a_step";

    ($success, $call) = $context.GetFixtureCall($match);
    ok $success, "When method matches in workflow in context, it is found";
    is $call, "AdviceWorkflow.This_is_a_step()", "Class, method, and args are presented";
}
