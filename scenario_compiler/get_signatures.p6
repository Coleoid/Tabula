use Tabula::Fixture-Binder;
use Tabula::Fixture-Class;

sub MAIN(
        Str :$folder = 'k:\\code\\acadis_trunk\\ScenarioTests\\ScenarioContext\\Implementations'
        #Str :$folder = 'd:\\code\\acadis\\ScenarioTests\\ScenarioContext\\Implementations'
        ) {
    my $binder = Fixture-Binder.new();
    $binder.debug = True;
    $binder.load-fixtures($folder);
}
