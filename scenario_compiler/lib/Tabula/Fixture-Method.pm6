use v6;
use MONKEY-SEE-NO-EVAL;
use JSON::Class;

class Fixture-Method does JSON::Class {
    has Str $.definition is rw;

    has Str $!name;
    method name() { $!name }
    has Str $!args;

    has Str $!key;
    method key() { $!key }

    #  Tabula presumes all methods are public void, and that this information
    # is stripped from the signature before being sent as the definition.
    submethod BUILD(:$!definition is required) {
        ($!name, $!args) = name-and-args-from-signature($!definition);
        $!key = key-from-name-and-args($!name, $!args);
    }

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
}
