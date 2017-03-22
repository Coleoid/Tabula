use Test;
use Tabula::Fixture-Binder;
use Tabula::Fixture-Class;

my $binder = Fixture-Binder.new();

my Fixture-Class $workflow = $binder.ready-class(class-name => 'Workflow', namespace => 'Tabula', :add-new);

ok $workflow.defined, "Get Workflow succeeds";
is $workflow.instance-name, 'Workflow', "All-Father is properly named";

if False { # still hashing this out
my Fixture-Class $wf_another .= new(class-name => 'Workflow', namespace => 'Tabula');
try {
    $binder.add-class($wf_another);
    ok False, "Duplicate class name should die.";
    CATCH {
        default {
            is .message, "Binder already has a fixture named [Workflow].",
                "Duplicate class name throws exception when added to Binder";
        }
    }
}
}

$workflow.add-method('Log_out()');

my $method = $workflow.find-step-method-from-text('log out');
ok $method.defined, "Found preloaded method";


#   Really, these belong in fixture-class.t, I'm just lazily piggybacking on the setup already done here.
my $horse = Fixture-Class.new(class-name => "HorseWorkflow", parent => $workflow, namespace => 'foo');
$method = $horse.find-step-method-from-text('log out');
ok $method.defined, "Found inherited method";

$method = $horse.find-step-method-from-text('saw logs');
nok $method.defined, "Correct behavior when no method to find";


done-testing;
