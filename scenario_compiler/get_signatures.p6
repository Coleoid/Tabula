use Tabula::Fixture-Binder;
use Tabula::Fixture-Class;

sub MAIN(Str :$folder) {
    my $binder = Fixture-Binder.new();
    $binder.load-fixtures($folder);
}
