use v6;
use Tabula::Fixture-Class;
use MONKEY-SEE-NO-EVAL;

#  Fixture-Class factory and repository
#  Deserializes from C# source in subtrees passed to load-fixtures()
class Fixture-Binder {
    has Fixture-Class %.classes{Str};
    has Bool $.debug is rw = False;
    has SetHash $pucks;  #  Possibly Useless Classes Known

    #  Main responsibilities
    method load-fixtures(*@subtree-trunks) {

        my @folders;
        for @subtree-trunks -> $trunk {
            die "[$trunk] is not a folder within [$*CWD]." unless $trunk.IO.d;
            say "Adding folder [$trunk]" if $!debug;
            @folders.push($trunk);
        }

        while (my $target = @folders.pop) {
            my @folder-contents = dir $target;

            for @folder-contents {
                when .IO.d {
                    #say "folder $_ added." if $!debug;
                    @folders.push($_) unless / '\.' \w /;  # skip dot-folders
                }
                when / .+ '.' cs $ / {
                    #say "parsing $_." if $!debug;
                    self.parse-source($_);
                }
            }
        }
    }

    method repl() {
        while True {
            my $in = prompt "> ";
            return if $in eq 'exit';
            my $result = EVAL $in;
            say $result if $result.defined;

            CATCH { default { say .message; } }
        }
    }

    my regex method-decl {
        ^ \s* public \s+ [override \s+]? [virtual \s+]? void \s+ $<name> = (\w+ '(' <-[)]>* ')')
    }

    my regex class-decl  {
        ^ \s* public \s+ ([abstract \s+]? [partial \s+]?)
        class \s+ (\w+)
        [ \s* ':' \s* (\w+) ]?
    }

    method parse-source($path) {
        my Fixture-Class $class = Nil;
        my Bool $filling-class = False;

        self.preload-base-classes();

        for $path.IO.lines -> $line {

            if $filling-class && $line ~~ / <method-decl> / {
                $class.add-method(~$<method-decl><name>);
            }

            if $line ~~ / <class-decl> / {
                my $modifiers = ~$<class-decl>[0].trim;
                my $class-name = ~$<class-decl>[1];
                my $parent-name = ~($<class-decl>[2] // '');

                self.add-class($class);
                $class = Nil;

                $filling-class = not ($class-name ~~ /^[MVAWorkflow|MVBaseWorkflow|Workflow]$/);
                if $filling-class {
                    #say $line;
                    my Fixture-Class $parent = self.ready-parent($parent-name);
                    $class = self.ready-class($class-name, :$parent);
                }
                else {
                    say ">>> Skipped preloaded base class: $modifiers $class-name";
                    $class = Nil;
                }
            }
        }

        self.add-class($class);
        $class = Nil;
    }

    method ready-parent($parent-name) {
        return Nil if $parent-name eq '';

        return self.ready-class($parent-name, :add-new);
    }

    #   To either pull matching class or create one new.
    method ready-class($class-name, Fixture-Class :$parent?, Bool :$add-new = False) {
        my Fixture-Class $class = self.get-class($class-name);
        if not $class.defined {
            $class .= new(:$class-name, :$parent);
            self.add-class($class) if $add-new;
        }

        if $parent.defined {
            $parent.is-parent = True;
            if $class.parent.defined {
                return if $class.parent === $parent;
                die "Conflicting parents [$($parent.class-name)] and [$($class.parent.class-name)] for [$($class.class-name)].";
            }
            $class.parent = $parent;
        }

        return $class;
    }

    method add-class(Fixture-Class $class) {
        return unless $class.defined;

        if %!classes{$class.key}:exists {
            return if $class === %!classes{$class.key}; # my work here is done!
            die "Binder contains a different fixture named [$($class.class-name)].";
            #  Since this doesn't happen, I don't worry about namespaces.  :D
        }

        #say "+++ Added $($class.class-name)";
        %!classes{$class.key} = $class;

        if $class.methods.elems == 0 {
            #say "--- Class $($class.class-name) currently supplies no methods.";
            $pucks{$class} = True;
        }
    }

    method get-class(Str $name --> Fixture-Class) {
        %!classes{$name} || %!classes{key-from-command-text($name)} || Nil;
    }

    sub key-from-command-text($text) {
        $text.lc
            .subst('workflow', '')
            .subst('_', '', :g)
            .subst(' ', '', :g);
    }

    # FUTURE:  Load these dynamically
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
        %!classes{$workflow.key} = $workflow;
        #self.add-class($workflow);

        #  Acadis\ScenarioTests\ScenarioContext\ViewImplementations\MVAWorkflow.cs
        #   public abstract class MVAWorkflow<TestopiaView, TView> : Workflow
        #        where TestopiaView : TestopiaView<TView>, new()
        #        where TView : IView
        my $mva-workflow = Fixture-Class.new(class-name => 'MVAWorkflow', instance-name => 'MVAWorkflow', parent => $workflow);
        $mva-workflow.add-method("Browse_to_Page()");
        $mva-workflow.add-method("Verify_access_denied()");
        $mva-workflow.add-method("Verify_access_granted()");
        %!classes{$mva-workflow.key} = $mva-workflow;
        #self.add-class($mva-workflow);

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
        %!classes{$mvBase-workflow.key} = $mvBase-workflow;
        #self.add-class($mvBase-workflow);
    }
}
