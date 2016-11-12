use v6;

class Method-Location {
    has Str $.signature is rw;
    has Str $.name;
    has $.args;
    has $.name-key;
    has $.args-key;

    submethod BUILD(:$!signature is required) {
        unless $!signature ~~ / $<name> = [ \w+ ] $<args> = ['(' .* ')'] / {
            die "I didn't understand the method signature:  $!signature";
        }

        $!name = ~$<name>;
        $!args = ~$<args>;

        $!name-key = $!name.lc.subst('_', '', :g);

        unless $!args ~~ / '(' $<arg> = ( \s* $<t1> = \S \S* \s+ $<argname> = [\S+] )* % ',' ')' / {
            die "I didn't understand the argument list:  $!args";
        }

        #  The args key encodes the method arg types.  For example,
        # "(sdi)" for (String, DateTime, int).  To differentiate
        # between overloads while being good for approximate matching.
        my $type-brief = '';
        for $<arg> -> $arg {
            $type-brief ~= ~$arg<t1>.lc;
        }
        $!args-key = "($type-brief)";
    }

    method key() { $!name-key ~ $!args-key }
}


#  Fixture:  A class containing testing methods to be called by the
# class which implements a Tabula scenario.
class Fixture {
    has Str $.class-name;
    has Str $.instance-name;
    has Str $.key;

    submethod BUILD(:$!class-name is required, :$!instance-name = "") {
        my str $short-name = $!class-name.subst('Workflow', '');
        if $!instance-name eq "" {
            $!instance-name = $short-name;
        }
        $!key = $short-name.lc;
    }

    has %.methods;
    method add-method($signature) {
        my $method = Method-Location.new(:signature($signature));
        %!methods{$method.key} = $method;
        say $method.key;
    }
}
