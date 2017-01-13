use v6;
use Tabula::Fixture-Method;
use JSON::Class;

#  Fixture:  A C# class containing testing methods to be called by the
# IGeneratedScenario.  Fixture-Class organizes what Tabula needs to generate
# the C# scenario classes.  The Fixture-Binder will traverse the file system
# and create these from the C# fixtures it finds there.
class Fixture-Class does JSON::Class {
    has Str $.class-name;
    has Str $.namespace;
    has Fixture-Method %.methods{Str};

    has Fixture-Class $!parent;
    has Str $.parent-name;

    has Str $!instance-name;
    method instance-name() { return $!instance-name; }

    has Str $!key;
    method key() { $!key }

    submethod BUILD(
            :$!class-name is required,
            :$!instance-name = '',
            Fixture-Class :$parent,
            :$!namespace is required) {

        my $short-name = $!class-name.subst('Workflow', '');
        $short-name = 'Workflow' if $short-name eq '';
        $!instance-name = $short-name if $!instance-name eq '';
        $!key = $short-name.lc;

        self.set-parent(:$parent);
    }

    method set-parent(Fixture-Class :$!parent) {
        $!parent-name = $!parent.defined
            ?? $!parent.class-name
            !! Nil;
    }

    method add-method(Str $definition) {
        my $method = Fixture-Method.new(:$definition);
        die "Definition [$definition] did not create a method" unless $method.defined;

        my $key = $method.key;
        %!methods{$key} = $method;

        CATCH { default { .Str.say if False; } }
    }

    method find-step-method($step) {
        my $key = key-from-step($step);

        return %.methods{$key} if %.methods{$key}:exists;
        return $!parent.find-step-method($step) if $!parent.defined;
        return Nil;
    }

    sub key-from-step($step) {
        return $step.lc.subst(' ', '', :g) ~ '()';
    }
}
