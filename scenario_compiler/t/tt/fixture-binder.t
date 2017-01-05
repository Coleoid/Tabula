use Test;
use Tabula::Fixture-Binder;
use Tabula::Fixture-Class;

my $binder = Fixture-Binder.new();

$binder.preload-base-classes();

my Fixture-Class $workflow = $binder.get-class('Workflow');

ok $workflow.defined, "Get Workflow succeeds";
is $workflow.instance-name, 'Workflow', "All-Father is properly named";

try {
    $binder.add-class($workflow);
    ok False, "Duplicate class name should die.";
    CATCH {
        default {
            my $ex = $_;
            is $ex.message, "Binder already has a fixture named [Workflow].",
                "Duplicate class name throws exception when added to Binder";
        }
    }
}

my $method = $workflow.find-step-method('log out');
ok $method.defined, "Found preloaded method";


#   Really, these belong in fixture-class.t, just lazily piggybacking on setup here.
my $horse = Fixture-Class.new(class-name => "HorseWorkflow", parent => $workflow);
$method = $horse.find-step-method('log out');
ok $method.defined, "Found inherited method";

$method = $horse.find-step-method('saw logs');
nok $method.defined, "Correct behavior when no method to find";



done-testing;
