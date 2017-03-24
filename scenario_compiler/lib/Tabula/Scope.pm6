use v6;
use Tabula::Fixture-Class;
use Tabula::Fixture-Method;

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

    #TODO: you know it--aliases.
    has @.aliases;
    method add-alias($alias)  { ... }
    method find-alias($alias) { ... }


    method resolve-step(Str $key) {
        my ($fixture, $method) = self.find-step-method($key);
        return ($fixture, $method) if $fixture.defined;

        return self.parent.defined
            ?? self.parent.resolve-step($key)
            !! (Nil, Nil);
    }

    method find-step-method($key) {
        for @!fixtures -> $fixture {
            my Fixture-Method $method = $fixture.find-step-method($key);
            return ($fixture, $method) if $method.defined;
        }
    }

}
