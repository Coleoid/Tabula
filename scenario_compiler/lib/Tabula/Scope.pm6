use v6;
use Tabula::Fixture-Book;

#  Holding fixtures, labels, and aliases for a portion of the scenario.
class Scope {
    has $.name;
    has Scope $.parent;
    submethod BUILD(:$!name, :$!parent) {
        @!fixtures = Empty;
    }

    #  Methods defined in multiple fixtures will be found in the most recently
    # added Fixture-Book.
    has @.fixtures;
    method add-fixture(Fixture-Book $fixture) {
        @!fixtures.push($fixture);
    }

    #  An alias value may be defined in terms of other aliases, but only
    # those which have already been defined.  Resolution is most-recent-first.
    has %.aliases;
    method add-alias($alias, $value) { ... }

    #  Arguments, table values, any other symbolic reference
    has %.labels;
    method add-label($label, $value) { ... }

    method find-step-method($flatName, $arg-count) {
        for .fixtures {
            #WHARRGARBL
        }
    }

    method resolve-step($step) {
        my $method-call = .find-step-method($step);

        return $method-call || .parent
            ?? .parent.resolve-step($step)
            !! fail "No method matching ($step) found in scope.";
    }

}
