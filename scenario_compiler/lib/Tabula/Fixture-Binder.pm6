use v6;
use Tabula::Fixture-Class;

#  Reads fixtures from disk, creates Fixture-Class objects
#  Stores/fetches results
class Fixture-Binder {
    has %.shelf;
    has Bool $.debug = False;

    #  Main responsibilities
    method load-fixtures($folder) {

        die "[$folder] is not a folder within [$*CWD]." unless $folder.IO.d;

        my @folders;
        @folders.push($folder);

        while (my $target = @folders.pop) {
            my @folder-contents = dir $target;

            for @folder-contents {
                when .IO.d {
                    @folders.push($_) unless / '\.' \w /;  # skip dot-folders
                }
                when / .+ '.' cs $ / {
                    self.parse-source($_);
                }
            }
        }
    }

    method parse-source($file) {
        my Fixture-Class $class;
        my Bool $found-class = False;

        for $file.IO.lines {
            if $found-class {
                if / public \s+ [virtual \s+]? void \s+ (\w+ '(' <-[)]>* ')') / {
                    say "  $0" if $!debug;
                    $class.add-method(~$0);
                }
            }

            if / public \s+ class \s+ (\w+) / {
                my $class-name = ~$0;
                $found-class = so ($class-name ~~ / Workflow $ /);
                if $found-class {
                    $class = Fixture-Class.new(class-name => $class-name);
                    self.add-class($class);
                    say "$class-name:" if $!debug;
                }
                else {
                    $class = Nil;
                }
            }
        }
    }

    method add-class($class) {
        %!shelf{$class.key} = $class;
    }

    method get-class($name) {
        %!shelf{$name} || %!shelf{key-from-command-text($name)};
    }

    sub key-from-command-text($text) {
        $text.lc
            .subst('workflow', '')
            .subst('_', '', :g)
            .subst(' ', '', :g);
    }

    method preload-base-classes() {

        #  Acadis\ScenarioTests\ScenarioContext\Implementations\Workflow.cs
        #   public abstract class Workflow : ISetupAndTeardown, IScenarioMetadata
        my $workflow = Fixture-Class.new(class-name => 'Workflow', instance-name => 'Workflow');
        $workflow.add-method("Set_today_to_( DateTime today )");
        $workflow.add-method("Set_today_to_current_date_plus__days(int days)");
        $workflow.add-method("set_today_to_start_of_fiscal_year()");
        $workflow.add-method("set_today_to_end_of_fiscal_year()");
        $workflow.add-method("set_today_to_end_of_fiscal_year_plus( int interval, string timeUnit )");
        $workflow.add-method("Add__to_today( int interval, string timeUnit )");
        $workflow.add-method("Log_in_as(string userName)");
        $workflow.add-method("Log_into_portal_as(string emailAddress)");
        $workflow.add-method("Log_out()");
        $workflow.add-method("Enable_login_in_scenario_scope()");

        #  Acadis\ScenarioTests\ScenarioContext\ViewImplementations\MVAWorkflow.cs
        #   public abstract class MVAWorkflow<TestopiaView, TView> : Workflow
        #        where TestopiaView : TestopiaView<TView>, new()
        #        where TView : IView
        my $mva-workflow = Fixture-Class.new(class-name => 'MVAWorkflow', instance-name => 'MVAWorkflow', parent => $workflow);
        $mva-workflow.add-method("Browse_to_Page()");
        $mva-workflow.add-method("Verify_access_denied()");
        $mva-workflow.add-method("Verify_access_granted()");

        #  Acadis\ScenarioTests\ScenarioContext\ViewImplementations\MVBaseWorkflow.cs
        #   public abstract partial class MVBaseWorkflow<TView> : MVBaseWorkflow where TView : IView
        #   public abstract class MVBaseWorkflow : Workflow, IView
        #  Acadis\ScenarioTests\ScenarioContext\ViewImplementations\MVBaseWorkflowProperties.cs
        #   public abstract partial class MVBaseWorkflow<TView> where TView : IView
        my $mvBase-workflow = Fixture-Class.new(class-name => 'MVBaseWorkflow', instance-name => 'MVBaseWorkflow', parent => $workflow);
        $mvBase-workflow.add-method("Enter_text__for__(string value, string label)");
        $mvBase-workflow.add-method("Choose_option__for_( string value, string label )");
        $mvBase-workflow.add-method("Check__( string label )");
        $mvBase-workflow.add-method("Uncheck__(string label)");
        $mvBase-workflow.add-method("Select__for__(string selectedText, string fieldLabel)");
        $mvBase-workflow.add-method("Verify_option_for__is__(string label, string expectedValue)");
        $mvBase-workflow.add-method("Verify_label_for__is__( string fieldLabel, string expectedValue )");
        $mvBase-workflow.add-method("Verify_text_for__is__(string fieldLabel, string expectedValue)");
        $mvBase-workflow.add-method("Verify_text_for__is_empty(string fieldLabel)");
        $mvBase-workflow.add-method("Verify__is_checked(string fieldLabel)");
        $mvBase-workflow.add-method("Verify__is_unchecked(string fieldLabel)");
        $mvBase-workflow.add-method("Verify_options_for__are__(string fieldLabel, string options)");
        $mvBase-workflow.add-method("Verify__is_selected_for__(string selectedOption, string fieldLabel)");
        $mvBase-workflow.add-method("Click_button__(string buttonName)");
        $mvBase-workflow.add-method("Verify_feature____available(string featureName, string isOrNot)");
        $mvBase-workflow.add-method("public virtual void Browse_to_Page()");
        $mvBase-workflow.add-method("public virtual void Verify_access_denied()");
        $mvBase-workflow.add-method("ublic void Verify_access_granted()");

    }
}
