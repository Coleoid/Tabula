use Test;
use Tabula::Grammar-Testing;
use Tabula::Fixture-Binder;

#if False
{   diag "An Execution-Context will fetch steps from libraries in scope";

    my (&parser, $actions) = curry-parser-emitting-Testopia("Step");
    my $context = $actions.Context;

    ok $context, "get a context from Target-Testopia";

    ok $context.current-scope, "start with one scope";
    nok $context.current-scope.parent, "start with ONLY one scope";
    $context.file-name = "myfile.scn";

    # Phony up a library for testing
    my $lib = Fixture.new(class-name => "AdviceWorkflow");
    $lib.methods{'notthis'} = ("Not_This", ());

    nok $context.current-scope.fixtures.defined, "starting with no libraries declared";

    my $binder = Fixture-Binder.new();

    # A ton of this happens at startup time
    $context.RegisterLibrary($lib);


    is $context.lib-declarations, "        public AdviceWorkflow Advice = new AdviceWorkflow();\n", "registering a library gets it declared and initialized";

    my $match = parser("Basic step found in library", "this is A STEP");

    my $result = $context.GetFixtureCall($match, "myfile.scn:28");
    nok $result, "With no libraries in scope, we find no method";
    is $result.exception.message, "myfile.scn:28:  Did not find step to match 'this is A STEP' in libraries in scope.", "...and get a sensible error";

    # This happens when we encounter a ">use: advice workflow" command
    $context.AddLibraryToScope($lib);

    $result = $context.GetFixtureCall($match, "myfile.scn:28");
    nok $result, "With libraries in scope but no matching method, we find no method";

    # This would not normally happen in mid-run, but it's useful to do now for testing
    $lib.steps{'thisisastep'} = ("This_is_a_step", ());

    $result = $context.GetFixtureCall($match, "myfile.scn:28");
    ok $result, "When method matches in workflow in context, it is found";
    is $result, "Advice.This_is_a_step()", "Class, method, and args are presented";
}

done-testing;
