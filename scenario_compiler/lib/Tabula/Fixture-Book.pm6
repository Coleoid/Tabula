use v6;

class Method-Page {
    has Str $.definition is rw;
    has Str $.name;
    has Str $.key;


    method name-and-args-from-signature($signature) {
        unless $signature ~~ / $<name> = [ \w+ ] $<sig> = ['(' .* ')'] / {
            die "I didn't understand the method signature:  $signature";
        }

        return (~$<name>, ~$<sig>);
    }

    method key-from-name-and-args($name, $args) {

        my $name-key = $name.lc.subst('_', '', :g);

        #  Future:  Optional args, args with defaults?
        unless $args ~~ / '(' $<arg> = ( \s* $<t1> = \S \S* \s+ $<argname> = [\S+] )* % ',' ')' / {
            die "I didn't understand the argument list:  $args";
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
        my ($name, $args) = self.name-and-args-from-signature($!definition);
        $!key = self.key-from-name-and-args($name, $args);
        $!name = $name;
    }
}

#  Fixture:  A C# class containing testing methods to be called by the
# IGeneratedScenario.  Fixture-Book organizes what Tabula needs to generate
# the C# scenario classes.  The Fixture-Binder will traverse the file system
# and create these books from the C# fixtures it finds there.
class Fixture-Book {
    has Str $.class-name;
    has Str $.instance-name;
    has Str $.key;

    #  Future:  Namespace detection?
    submethod BUILD(:$!class-name is required, :$!instance-name = "") {
        my str $short-name = $!class-name.subst('Workflow', '');
        if $!instance-name eq "" {
            $!instance-name = $short-name;
        }
        $!key = $short-name.lc;
    }

    has Method-Page %.pages;
    method add-method($definition) {
        my $method = Method-Page.new(:definition($definition));
        %.pages{$method.key} = $method;
    }

    method key-from-step($step) {
        return $step.lc.subst(' ', '', :g) ~ '()';
    }

    method find-step-method($step) {
        my $key = self.key-from-step($step);
        return %.pages{$key};
    }
}
