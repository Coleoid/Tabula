use v6;
use Tabula::Fixture;

#  Loads fixtures from disk, parses methods and arguments, holds results
class Fixture-Binder {
    has %.fixtures;

    #  Main responsibilities
    method load-fixtures($folder) { ... }
    method parse-source($source) { ... }
    method get-fixture($name) { ... }

    method canonicalize-fixture-name($name) { ... }
    method canonicalize-method-name($name) { ... }
}
