use v6;
use Tabula::Fixture-Book;

#  Loads fixtures from disk, binds them into books, shelves results
class Fixture-Binder {
    has %.shelf;

    #  Main responsibilities
    method load-fixtures($folder) { ... }
    method parse-source($source) { ... }
    method pull-fixture($name) { ... }

    method canonicalize-fixture-name($name) { ... }
    method canonicalize-method-name($name) { ... }
}
