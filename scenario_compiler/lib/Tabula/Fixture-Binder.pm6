use v6;
use Tabula::Fixture-Class;
use MONKEY-SEE-NO-EVAL;
use JSON::Class;

#  Fixture-Class factory and repository
#  Deserializes from C# source in subtrees passed to load-fixtures()
class Fixture-Binder does JSON::Class {
    has Fixture-Class %.classes{Str};
    has Bool $.debug is rw = False;
    has SetHash $pucks;  #  Possibly Useless Classes Known

    #  Main responsibilities
    method load-fixtures(*@subtree-trunks) {

        my @folders;
        for @subtree-trunks -> $trunk {
            die "[$trunk] is not a folder within [$*CWD]." unless $trunk.IO.d;
            #say "Adding folder [$trunk]" if $!debug;
            @folders.push($trunk);
        }

        while (my $target = @folders.pop) {
            #say ">>> into $target";
            my @folder-contents = dir $target;

            for @folder-contents -> $entry {
                when $entry.IO.d {
                    #say "folder $_ added." if $!debug;
                    @folders.push($entry) unless $entry ~~ / '\.' \w /;  # skip dot-folders
                }
                when $entry ~~ / .+ '.' cs $ / {
                    #say "==> parsing $entry" if $!debug;
                    self.parse-source($entry);
                }
            }
        }

        #  A patch that lets me avoid parsing generics.
        my Fixture-Class $mvcbw = self.get-class('MvcBaseWorkflow');
        my Fixture-Class $mvbw = self.get-class('MVBaseWorkflow');
        $mvcbw.set-parent(parent => $mvbw);
    }

    method repl() {
        # say "\n=== Base classes:";
        # for %!classes.values -> $class {
        #     say $class.class-name ~ " has $($class.methods.elems) methods inheritable."
        #         if $class.is-parent;
        # }

        # say "\n=== Pucks providing methods or parents to scenarios:";
        # for $!pucks.keys {
        #     say .class-name if .methods.elems > 0 or .parent-name.defined;
        # }
        #
        # say "\n=== Pucks NOT providing methods or parents to scenarios:";
        # for $!pucks.keys {
        #     say .class-name if .methods.elems == 0 and not .parent-name.defined;
        # }

        say "";
        while True {
            my $in = prompt "> ";
            return if $in eq 'exit';
            my $result = EVAL $in;
            say $result if $result.defined;

            CATCH { default { say .message; } }
        }
    }

    my regex namespace { ^ \s* namespace \s+ ( <[\w.]>+ ) }

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
        my $namespace = '';

        for $path.IO.lines -> $line {

            if $line ~~ / <namespace>[0] / {
                $namespace = ~$<namespace>;
            }

            if $line ~~ / <method-decl> / && $class.defined {
                $class.add-method(~$<method-decl><name>);
            }

            if $line ~~ / <class-decl> / {
                self.add-class($class);
                $class = Nil;

                my $class-name = ~$<class-decl>[1];
                my $parent-name = ~($<class-decl>[2] // '');

                my Fixture-Class $parent = self.ready-parent($parent-name);
                $class = self.ready-class($class-name, :$parent, :$namespace);
            }
        }

        self.add-class($class);
        $class = Nil;
    }

    #  Here's one difference between a tool and a product.
    my $any-non-parent = any('', 'IAcadisServiceClient', 'System',
        'IAcadisSmtpClientFactory', 'IMutableUserContext', 'ITestopiaMonitor',
        'IMessenger', 'IObjectBuilder', 'IDateTimeProvider', 'TraceListener',
        'IUrlResolver', 'IAcadisSmtpClient', 'ISetupAndTeardown',
        'IAcadisReadOnlyDBConnectionFactory', 'EmailSender', 'IDisposable',
        'DocumentStorageProviderFactory', 'Exception',
        'AcadisConfigurationManager', 'CollectionEquivalentConstraint',
        'IFiltersView', 'IStudentListViewRowItem', 'FieldAttribute',
        'Attribute', 'Is', 'MvcBaseWorkflow', 'HttpPostedFileBase',
        'IAcadisUserDetailsEditorView'
        );

    method ready-parent($parent-name) {
        return Nil if $parent-name eq $any-non-parent;

        my $parent = self.ready-class($parent-name, :add-new);
        #$parent.is-parent = True;
        return $parent;
    }

    #   To either pull matching class or create one new, with parent.
    method ready-class(:$class-name, :$namespace is required,
            Fixture-Class :$parent?, Bool :$add-new = False
            --> Fixture-Class) {

        my Fixture-Class $class = self.get-class($class-name);
        if not $class.defined {
            $class .= new(:$namespace, :$class-name, :$parent);
            self.add-class($class) if $add-new;
        }

        $class.set-parent(:$parent);

        return $class;
    }

    method add-class(Fixture-Class $class) {
        return unless $class.defined;

        if %!classes{$class.key}:exists {
            # die "Binder contains a different fixture named [$($class.class-name)]."
            #     unless $class === %!classes{$class.key};
            # #  When this happens, we may need more code to deal
            # # with fixture name collisions.
        }

        %!classes{$class.key} = $class;

        $pucks{$class} = ($class.methods.elems == 0);
    }

    method get-class(Str $name --> Fixture-Class) {
        %!classes{$name}
            || %!classes{key-from-command-text($name)}
            || Nil;
    }

    sub key-from-command-text($text) {
        $text.lc
            .subst('workflow', '')
            .subst('_', '', :g)
            .subst(' ', '', :g);
    }
}
