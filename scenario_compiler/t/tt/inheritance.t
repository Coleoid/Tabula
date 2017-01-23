use Test;
use Tabula::Grammar-Testing;
use Tabula::Fixture-Class;

my (&parser, $actions) = curry-parser-emitting-Testopia( "Step" );
my $context = $actions.Context;
my $binder = $actions.Binder;

$context.file-name = "SampleScenario.scn";

#my $machine-specific-prefix = 'k:\\code\\acadis_trunk\\';
my $machine-specific-prefix = 'd:\\code\\acadis\\';

my $root = $machine-specific-prefix ~ 'ScenarioTests\\ScenarioContext\\';
#my $cdfw-FileName  = $root ~ 'ViewImplementations\\Curriculum\\ClassDescriptiveFieldsWorkflow.cs';
my $ecw-FileName   = $root ~ 'ViewImplementations\\Curriculum\\EditClassWorkflow.cs';
my $mvbw-FileName  = $root ~ 'ViewImplementations\\MVBaseWorkflow.cs';
my $mvbwp-FileName = $root ~ 'ViewImplementations\\MVBaseWorkflowProperties.cs';
#my $mvcbw-FileName = $root ~ 'ViewImplementations\\MvcBaseWorkflow.cs';
my $workf-FileName = $root ~ 'Implementations\\Workflow.cs';

#  Load MGH
#$binder.parse-source($cdfw-FileName);
$binder.parse-source($ecw-FileName);
$binder.parse-source($mvbw-FileName);
$binder.parse-source($mvbwp-FileName);
#$binder.parse-source($mvcbw-FileName);
$binder.parse-source($workf-FileName);

my Fixture-Class $cdfw = $binder.get-class('ClassDescriptiveFieldsWorkflow');
my Fixture-Class $ecw = $binder.get-class('EditClassWorkflow');
my Fixture-Class $mvbw = $binder.get-class('MVBaseWorkflow');
#my Fixture-Class $mvcw = $binder.get-class(?);
my Fixture-Class $work = $binder.get-class('workflow');

#ok $cdfw.parent === $mvbw, "parent of ClassDescriptive is MVBaseWorkflow";
say $ecw.parent-name;
ok $ecw.parent-name === $mvbw, "parent of EditClass is MVBaseWorkflow";
say $mvbw.parent-name;
ok $mvbw.parent-name === $work, "parent of MVBaseWorkflow is Workflow";
say $work.parent-name;
nok $work.parent-name.defined, "parent of Workflow is undefined";

ok $ecw.find-step-method("verifyproctorloginfailed()").defined, "can find method inherited two levels above";
ok $ecw.find-step-method("debuggerbreak()").defined, "inherited from first half of partial class";
ok $ecw.find-step-method("clickbutton()").defined, "inherited from second half of partial class";
