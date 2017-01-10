use v6;
use MONKEY-SEE-NO-EVAL;

class Fixture-Method {
    has Str $.definition is rw;
    has Str $.name;
    has Str $.key;


    sub name-and-args-from-signature($signature) {
        unless $signature ~~ / $<name> = [ \w+ ] $<sig> = ['(' .* ')'] / {
            die "I didn't understand the method signature:  $signature";
        }

        return (~$<name>, ~$<sig>);
    }

    sub key-from-name-and-args($name, $args) {

        my $name-key = $name.lc.subst('workflow', '').subst('_', '', :g);

        #  Future:  Generic, nullable, optional, default values...
        if not $args ~~ / '(' \s* $<arg> = ( [ \s* $<t1> = \S \S* \s+ $<argname> = [\S+] ]* % ',') \s* ')' / {
            die "    ? I didn't understand the argument list for:  $name$args";
        }

        #  The sig key encodes the method arg types.  For example,
        # "(sdi)" for (String, DateTime, int).  To differentiate
        # between overloads while being good for approximate matching.
        my $sig-brief = '';
        for $<arg> -> $arg {
            $sig-brief ~= ~$arg<t1>.lc;
        }
        my $sig-key = "($sig-brief)";

        return $name-key ~ $sig-key;
    }

    #  Tabula presumes all methods are public void, and that this information
    # is stripped from the signature before being sent as the definition.
    submethod BUILD(:$!definition is required) {
        my ($name, $args) = name-and-args-from-signature($!definition);
        $!key = key-from-name-and-args($name, $args);
        $!name = $name;
    }
}

#  Fixture:  A C# class containing testing methods to be called by the
# IGeneratedScenario.  Fixture-Class organizes what Tabula needs to generate
# the C# scenario classes.  The Fixture-Binder will traverse the file system
# and create these from the C# fixtures it finds there.
class Fixture-Class {
    has Str $.class-name;
    has Str $.instance-name;
    has Str $.key;
    has Fixture-Class $.parent is rw;
    has Bool $.is-parent is rw = False;
    has Fixture-Method %.methods{Str};
    has Bool $.debug = False;

    #  Future:  Namespace detection in case of Fixture name collision
    submethod BUILD(
            :$!class-name is required,
            :$!instance-name = "",
            :$!parent = Nil) {
        my str $short-name = $!class-name.subst('Workflow', '');
        if $!instance-name eq "" {
            $!instance-name = $short-name;
        }
        $!key = $short-name.lc;
    }

    method add-method(Str $definition) {
        my $method = Fixture-Method.new(:$definition);
        die "Definition [$definition] did not create a method" unless $method.defined;

        #say "[$definition]";
        my $key = $method.key;
        %!methods{$key} = $method;

        CATCH { default { .Str.say if $!debug; } }
    }

    method key-from-step($step) {
        return $step.lc.subst(' ', '', :g) ~ '()';
    }

    method find-step-method($step) {
        my $key = self.key-from-step($step);
        if %.methods{$key}:exists {
            return %.methods{$key};
        }
        elsif $!parent.defined {
            return $!parent.find-step-method($step);
        }
        else {
            return Nil;
        }
    }
}
