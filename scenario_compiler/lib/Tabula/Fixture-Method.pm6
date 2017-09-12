use v6;
use MONKEY-SEE-NO-EVAL;
#use JSON::Class;

#class Fixture-Method does JSON::Class {
class Fixture-Method {
    has Str $.definition is rw;

    has Str $!name;
    method name() { $!name }

    has Str $!key;
    method key() { $!key }

    has @!param-types;

    #  Tabula presumes all methods are public void, and that this information
    # is stripped from the signature before being sent as the definition.
    submethod BUILD(:$!definition is required) {
        @!param-types = param-types-from-definition($!definition);
        $!name = name-from-definition($!definition);
        $!key = self.key-from-name($!name);
    }

    sub param-types-from-definition($definition) {

        #TODO:  parse generics (I know of no nested generics in Testopia...)
        #TODO:  recognize collection types (List<T>, string [], et c.)
        #TODO:  parse nullable as base type
        #TODO:  parse default values
        #TODO:  recognize defaulted args as optional
        if not $definition ~~ / '(' \s*
            $<args> = ( [ \s*
                $<type> = [\S+] \s+
                $<name> = [\S+]
            ]* % ',' ) \s* ')' / {
            die "    ? I didn't understand the argument list for:  $definition";
        }

        my @param-types = [];
        for $<args><type> -> $type {
            @param-types.push(~$type);
        }

        return @param-types;
    }

    sub name-from-definition($signature) {
        unless $signature ~~ / $<name> = [ \w+ ] $<args> = ['(' .* ')'] / {
            die "I didn't understand the method signature:  $signature";
        }

        return (~$<name>, ~$<args>);
    }

    method key-from-name($name) {
        $name.lc.subst('workflow', '').subst('_', '', :g) ~ '()';
    }

    method generate-call($fixture-name, @terms) {
        my $args = @terms.join(', ');
        return $fixture-name ~ '.' ~ $!name ~ '(' ~ $args ~ ')';
    }

    method args-from-match($match) {
        my @args = $match<Symbol>
            .grep({.<Term>})
            .map({get-Term-string(.<Term>)});

        return self.typed-arg-text(@args);
    }

    method typed-arg-text(@args) {
        my @result;

        for @!param-types Z @args -> [$type, $val] {
            my Str $arg;
            given $type.lc {
                when 'int'    { $arg = ($val ~~ /^var/)
                    ?? $val ~ '.To<int>()'
                    !! $val.subst('"', '', :g);
                }
                when 'string' { $arg = $val }
                default       { $arg = $val ~ '.To<' ~ $type ~ '>()' }
            }
            @result.push( $arg );
        }

        return @result.join(', ');
    }

    #TODO:  Evaluate Number and Date types as those types when fitting
    sub get-Term-string($term) {
        given $term {
            when .<String>   {return .made};
            when .<Number>   {return .made};
            when .<Date>     {return .made};
            when .<Variable> {return 'var["' ~ .<Variable><Word> ~ '"]'};
            default          {fail "Unknown Term type"};
        }
    }
}
