use v6;
use Tabula::Scope;
use Tabula::Match-Helper;

class Execution-Context does Match-Helper {
    has str $.file-name is rw;

    submethod BUILD {
        self.open-scope("Outer scenario scope");
    }

    has Scope $.current-scope;
    method open-scope($name = 'unnamed block') {
        $!current-scope = Scope.new(
            name => $name,
            parent => $!current-scope
        );
    }

    method close-scope() {
        $!current-scope // fail "trying to end a scope where there is none.";
        $!current-scope = $!current-scope.parent;
    }

    method add-fixture($fixture) {
        $!current-scope.add-fixture($fixture);
    }

    #  Given an action match, finds step method it fits
    method resolve-step($match) {
        return $!current-scope.resolve-step($match);
    }
}
