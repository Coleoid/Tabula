use v6;
use Tabula::Fixture-Method;
use JSON::Class;

#  Fixture:  A C# class containing testing methods to be called by the
# IGeneratedScenario.  Fixture-Class organizes what Tabula needs to generate
# the C# scenario classes.  The Fixture-Binder will traverse the file system
# and create these from the C# fixtures it finds there.
class Fixture-Class does JSON::Class {

    submethod BUILD(
            Str :$!namespace?,
            Str :$!class-name is required,
            Fixture-Class :$parent?,
            ) {

        my $short-name = $!class-name.subst('Workflow', '');
        $short-name = 'Workflow' if $short-name eq '';
        $!instance-name = $short-name;
        $!key = $short-name.lc;

        self.set-parent(:$parent) if $parent.defined;
    }


    has Str $.namespace;
    method update-namespace($new-namespace) {
        $!namespace = $new-namespace if $!namespace eq '';
    }

    has Str $.class-name;

    has Fixture-Class $!parent;
    has Str $.parent-name;

    has Str $!instance-name;
    method instance-name() { return $!instance-name; }

    has Str $!key;
    method key() { $!key }

    method set-parent(Fixture-Class :$!parent) {
        $!parent-name = $!parent.defined
            ?? $!parent.class-name
            !! Nil;
    }

    has Fixture-Method %.methods{Str};
    method add-method(Str $definition) {
        my $method = Fixture-Method.new(:$definition);
        die "Definition [$definition] did not create a method" unless $method.defined;

        my $key = $method.key;
        %!methods{$key} = $method;

        CATCH { default { .Str.say if False; } }
    }

    method find-step-method-from-text($text) {
        return self.find-step-method-from-key(key-from-step($text));
    }

    sub key-from-step($step) {
        return $step if $step ~~ / \w+ '()' /;
        return $step.lc.subst(' ', '', :g) ~ '()';
    }

    method find-step-method-from-key($key) {
        return %.methods{$key} if %.methods{$key}.defined;
        return $!parent.find-step-method-from-key($key) if $!parent.defined;
        return Nil;
    }
}
