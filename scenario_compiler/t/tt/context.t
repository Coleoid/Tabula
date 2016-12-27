use Test;
use Tabula::Grammar-Testing;
use Tabula::Fixture-Binder;

#  This whole test file has lost relevance as other changes clarified
# the model.  Rethink it later.  What things are specifically about the
# Execution-Context?


#if False
# {   diag "An Execution-Context will fetch steps from libraries in scope";
#
#     my (&parser, $composer) = curry-parser-emitting-Testopia("Step");
#     my $context = $composer.Context;
#     my $binder = $composer.Binder;
#
#     ok $context, "the composer starts with an Execution-Context";
#     ok $binder, "the composer starts with a Fixture-Binder";
#
#     ok $context.current-scope, "start with one scope";
#     nok $context.current-scope.parent, "start with ONLY one scope";
#     $context.file-name = "myfile.scn";
#
#     my $book = Fixture-Class.new(class-name => "AdviceWorkflow");
#     $book.add-method("Not_This()");
#
#     nok $context.current-scope.fixtures.defined, "starting with no libraries declared";
#
#
#     # # A ton of this happens at startup time
#     # $binder.bind-fixture($book);
#     #
#     # my $match = parser("Basic step found in library", "this is A STEP");
#     #
#     # my $result = $context.GetFixtureCall($match, "myfile.scn:28");
#     # nok $result, "With no libraries in scope, we find no method";
#     # is $result.exception.message, "myfile.scn:28:  Did not find step to match 'this is A STEP' in libraries in scope.", "...and get a sensible error";
#     #
#     # # This happens when we encounter a ">use: advice workflow" command
#     # $context.AddLibraryToScope($book);
#     #
#     # $result = $context.GetFixtureCall($match, "myfile.scn:28");
#     # nok $result, "With libraries in scope but no matching method, we find no method";
#     #
#     # # This would not normally happen in mid-run
#     # $book.add-method("This_is_a_step()");
#     #
#     # $result = $context.GetFixtureCall($match, "myfile.scn:28");
#     # ok $result, "When method matches in workflow in context, it is found";
#     # is $result, "Advice.This_is_a_step()", "Class, method, and args are presented";
# }

done-testing;
