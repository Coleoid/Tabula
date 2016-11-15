use v6;

class Method-Page {
    has Str $.definition is rw;
    has Str $.name;
    has $.args;
    has $.name-key;
    has $.sig-key;

    #  Tabula presumes all methods are public void, and that this information
    # is stripped out before we get here.
    submethod BUILD(:$!definition is required) {
        unless $!definition ~~ / $<name> = [ \w+ ] $<sig> = ['(' .* ')'] / {
            die "I didn't understand the method definition:  $!definition";
        }

        $!name = ~$<name>;
        $!args = ~$<sig>;

        $!name-key = $!name.lc.subst('_', '', :g);

        #  Future:  Optional args, args with defaults?
        unless $!args ~~ / '(' $<arg> = ( \s* $<t1> = \S \S* \s+ $<argname> = [\S+] )* % ',' ')' / {
            die "I didn't understand the argument list:  $!args";
        }

        #  The sig key encodes the method arg types.  For example,
        # "(sdi)" for (String, DateTime, int).  To differentiate
        # between overloads while being good for approximate matching.
        my $sig-brief = '';
        for $<arg> -> $arg {
            $sig-brief ~= ~$arg<t1>.lc;
        }
        $!sig-key = "($sig-brief)";
    }

    method key() { $!name-key ~ $!sig-key }
}


#  Fixture:  A C# class containing testing methods to be called by the
# IGeneratedScenario.  Fixture-Book organizes what Tabula needs to generate
# the class which implements IGeneratedScenario.
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

    has Method-Page %.methods;
    method add-method($definition) {
        my $method = Method-Page.new(:definition($definition));
        %!methods{$method.key} = $method;
    }
}
