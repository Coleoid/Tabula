use Tabula::Fixture-Binder;
use Tabula::Fixture-Class;

#my $root_default = 'k:\\code\\acadis_trunk\\';
my $root_default = 'd:\\code\\acadis\\';

sub MAIN( Str :$root = $root_default ) {

    my $binder = Fixture-Binder.new();
    $binder.debug = True;

    my $imps = $root_default ~ 'ScenarioTests\\ScenarioContext\\Implementations';
    my $view-imps = $root_default ~ 'ScenarioTests\\ScenarioContext\\ViewImplementations';
    $binder.load-fixtures($imps);
    #$binder.load-fixtures($imps, $view-imps);
    $binder.repl();
}
