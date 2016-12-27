use v6;
use Tabula::Fixture-Class;

#  Holding fixtures, labels, and aliases for a portion of the scenario.
class Scope {
    has $.name;
    has Scope $.parent;
    submethod BUILD(:$!name, :$!parent) {
        @!fixtures = Empty;
    }

    #  Methods defined in multiple fixtures will be found in the most recently
    # added Fixture-Class.
    has @.fixtures;
    method add-fixture(Fixture-Class $fixture) {
        if (? $fixture) and ($fixture !~~ @!fixtures.any) {
            @!fixtures.unshift($fixture);
        }
    }

    #  An alias value may be defined in terms of other aliases, but only
    # those which have already been defined.  Resolution is most-recent-first.
    has %.aliases;
    method add-alias($alias, $value) { ... }

    #  Arguments, table values, any other symbolic reference
    has %.labels;
    method add-label($label, $value) { ... }

    method resolve-step($step) {
        my $method-call = self.find-step-method($step);

        return $method-call || (self.parent
            ?? self.parent.resolve-step($step)
            !! fail "No method matching ($step) found in scope.");
    }

    method find-step-method($match) {
        my $key = key-from-match($match);
        my $args = args-from-match($match);

        for @!fixtures -> $fixture {
            my Method-Page $page = $fixture.pages{$key};
            if $page.defined {
                my $method = $page.name;
                return $fixture.instance-name ~ '.' ~ $method ~ '(' ~ $args ~ ')';
            }
        }
    }

    sub key-from-match($match) {
        my $arg-count = 0;
        my $flatName = '';
        for $match<Phrase><Symbol> {
            when .<Word> {$flatName ~= .<Word>.lc.subst("'", '', :g)}
            when .<Term> {$arg-count++}
        }

        #TODO: actually comprehend the argument types
        return $flatName ~ '(' ~ 's' x $arg-count ~ ')';
    }

    sub args-from-match($match) {
        my @args = $match<Phrase><Symbol>
            .grep({.<Term>})
            .map({get-Term-string(.<Term>)});

        return join ', ', @args;
    }

    #TODO:  Evaluate terms as number and date types
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
