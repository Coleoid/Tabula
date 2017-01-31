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

    method resolve-step($step) {
        my $method-call = self.find-step-method($step);

        return $method-call || (self.parent
            ?? self.parent.resolve-step($step)
            !! fail "No method matching ($step) found in scope.");
    }

    method find-step-method($match) {
        my $key = key-from-match($match);

        for @!fixtures -> $fixture {
            my Fixture-Method $method = $fixture.find-step-method($key);# $fixture.methods{$key};
            if $method.defined {
                my $methodName = $method.name;
                my $args = $method.args-from-match($match);
                return $fixture.instance-name ~ '.' ~ $methodName ~ '(' ~ $args ~ ')';
            }
        }
    }

    sub key-from-match($match) {
        my $arg-count = 0;
        my $flatName = '';
        for $match<Symbol> {
            when .<Word> {$flatName ~= .<Word>.lc.subst("'", '', :g)}
            when .<Term> {$arg-count++}
        }

        # say $flatName if $flatName ~~ /verify/;

        #TODO: include and discriminate on the argument types
        #return $flatName ~ '(' ~ 's' x $arg-count ~ ')';
        return $flatName ~ '()';
    }

}
