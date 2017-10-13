use v6;
use Tabula::Scope;
use Tabula::Match-Helper;

class Compile-Context does Match-Helper {
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
    method open-paragraph() {
        open-scope('Paragraph');
    }

    method close-scope() {
        $!current-scope.parent // fail "trying to end a scope where there is none.";
        $!current-scope = $!current-scope.parent;
    }
    method close-paragraph() {
        close-scope();
    }

    method add-fixture($fixture) {
        $!current-scope.add-fixture($fixture);
    }

    method resolve-step($key) {
        return $!current-scope.resolve-step($key);
    }

    #TODO:  Aliasing
    method add-alias($alias)  { $!current-scope.add-alias($alias); }
    method find-alias($alias) { $!current-scope.find-alias($alias); }
}
