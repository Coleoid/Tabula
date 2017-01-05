use Tabula::Fixture-Binder;
use Tabula::Fixture-Class;

sub MAIN(Str :$folder) {
    my $binder = Fixture-Binder.new();
    $binder.debug = True;
    $binder.load-fixtures($folder);
}
