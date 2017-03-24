use Test;
use Tabula::Grammar-Testing;
use Tabula::Execution-Context;
use Tabula::Fixture-Binder;
use Tabula::Fixture-Class;


#if False
{   diag "The Execution-Context manages scopes";

    my $context = Execution-Context.new;

    my $scope = $context.current-scope;
    ok $scope, "start with one scope";
    nok $scope.parent, "start with ONLY one scope";

    #$context.file-name = "myfile.scn";
}

#if False
{   diag "The Execution-Context finds methods via scopes";

    my $context = Execution-Context.new;

    my $fixture = Fixture-Class.new(class-name => "AdviceWorkflow");
    $fixture.add-method("Not_This()");

    ok $context.current-scope.fixtures.elems == 0, "starting with no libraries declared";

    my ($found-fixture, $found-method) = $context.resolve-step('thisisastep()');
    nok $found-method, "With no libraries in scope, we find no method";

    # This happens when we encounter a ">use: advice workflow" command
    $context.add-fixture($fixture);

    ($found-fixture, $found-method) = $context.resolve-step('thisisastep()');
    nok $found-method, "With libraries in scope but no matching method, we find no method";

    # This would not normally happen in mid-run
    $fixture.add-method("This_is_a_step()");

    ($found-fixture, $found-method) = $context.resolve-step('thisisastep()');
    ok $found-method, "When method matches in workflow in context, it is found";
    is $found-method, "Advice.This_is_a_step()", "Class, method, and args are presented";
}


done-testing;
