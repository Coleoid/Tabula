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

    has @!arg-types;

    #  Tabula presumes all methods are public void, and that this information
    # is stripped from the signature before being sent as the definition.
    submethod BUILD(:$!definition is required) {
        ($!name, $!args) = name-and-args-from-signature($!definition);
        $!key = self.key-from-name-and-args($!name, $!args);
    }

    sub name-and-args-from-signature($signature) {
        unless $signature ~~ / $<name> = [ \w+ ] $<sig> = ['(' .* ')'] / {
            die "I didn't understand the method signature:  $signature";
        }

        return (~$<name>, ~$<sig>);
    }

    method key-from-name-and-args($name, $args) {

        my $name-key = $name.lc.subst('workflow', '').subst('_', '', :g);

        #  Future:  Generic, nullable, optional, default values...
        if not $args ~~ / '(' \s* $<args> = ( [ \s* $<type> = [\S+] \s+ $<name> = [\S+] ]* % ',' ) \s* ')' / {
            die "    ? I didn't understand the argument list for:  $name$args";
        }

        for $<args><type> -> $type {
            @!arg-types.push(~$type);
        }

        return $name-key ~ '()';
    }


    method args-from-match($match) {
        my @args = $match<Symbol>
            .grep({.<Term>})
            .map({get-Term-string(.<Term>)});


        return self.typed-args(@args);
        #return join ', ', @args;
    }

    method typed-args(@args) {
        my @result;

        for @!arg-types Z @args -> [$type, $arg] {
            if $type.lc eq 'string' {
                @result.push( $arg );
            }
            else {
                @result.push( $arg ~ '.To<' ~ $type ~ '>()' );
            }
        }

        return @result.join(', ');
    }


    #TODO:  Evaluate Number and Date types as those types when fitting
    sub get-Term-string($term) {
        given $term {
            when .<String>   {return .made};
            when .<Number>   {return '"' ~ .made ~ '"'};
            when .<Date>     {return '"' ~ .made ~ '"'};
            when .<Variable> {return 'var["' ~ .<Variable><Word> ~ '"]'};
            default          {fail "Unknown Term type"};
        }
    }

}
