use Test;
use Tabula::Grammar-Testing;
use Tabula::Fixture-Class;

my (&parser, $actions) = get-parser-emitting-Testopia( "Step" );
my $context = $actions.Context;
my $binder = $actions.Binder;

$context.file-name = "SampleScenario.scn";

my $install-location = 'k:\\code\\acadis_trunk\\';
#my $install-location = 'd:\\code\\acadis\\';

my $root = $install-location ~ 'ScenarioTests\\ScenarioContext\\';
my $cdfw-FileName  = $root ~ 'ViewImplementations\\Curriculum\\ClassDescriptiveFieldsWorkflow.cs';
my $ecw-FileName   = $root ~ 'ViewImplementations\\Curriculum\\EditClassWorkflow.cs';
my $mvbw-FileName  = $root ~ 'ViewImplementations\\MVBaseWorkflow.cs';
my $mvbwp-FileName = $root ~ 'ViewImplementations\\MVBaseWorkflowProperties.cs';
my $workf-FileName = $root ~ 'Implementations\\Workflow.cs';

$binder.parse-source($cdfw-FileName);
$binder.parse-source($ecw-FileName);
$binder.parse-source($mvbw-FileName);
$binder.parse-source($mvbwp-FileName);
$binder.parse-source($workf-FileName);

my Fixture-Class $cdfw = $binder.get-class('ClassDescriptiveFieldsWorkflow');
is $cdfw.class-name, 'ClassDescriptiveFieldsWorkflow', "got 'ClassDescriptiveFieldsWorkflow'";
my Fixture-Class $ecw  = $binder.get-class('EditClassWorkflow');
is $ecw.class-name, 'EditClassWorkflow', "got 'EditClassWorkflow'";
my Fixture-Class $mvbw = $binder.get-class('MVBaseWorkflow');
is $mvbw.class-name, 'MVBaseWorkflow', "got 'MVBaseWorkflow'";

my Fixture-Class $work = $binder.get-class('workflow');
is $work.class-name, 'Workflow', "got 'Workflow'";
ok $work.methods.elems > 20, 'parsed Workflow methods properly';
ok $work.find-step-method-from-text("verify proctor login failed").defined, "can find method directly in Workflow";
ok $mvbw.find-step-method-from-text("verify proctor login failed").defined, "can find method on parent";


ok $cdfw.parent-name eq 'MVBaseWorkflow', "parent of ClassDescriptive is MVBaseWorkflow";
ok $ecw.parent-name eq 'MVBaseWorkflow', "parent of EditClass is MVBaseWorkflow";
ok $mvbw.parent-name eq 'Workflow', "parent of MVBaseWorkflow is Workflow";
nok $work.parent-name.defined, "parent of Workflow is undefined";

ok $ecw.find-step-method-from-text("verify proctor login failed").defined, "can find method inherited two levels above";
ok $ecw.find-step-method-from-text("Wait seconds").defined, "can find method inherited two levels above";
ok $ecw.find-step-method-from-text("debugger break").defined, "inherited from first half of partial class";
ok $ecw.find-step-method-from-text("click button").defined, "inherited from second half of partial class";

###############
#my $mvcbw-FileName = $root ~ 'ViewImplementations\\MvcBaseWorkflow.cs';
#$binder.parse-source($mvcbw-FileName);
#my Fixture-Class $mvcw = $binder.get-class(?);

done-testing;
