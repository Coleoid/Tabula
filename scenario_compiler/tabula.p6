use Tabula::Fixture-Binder;
use Tabula::Fixture-Class;

my $grammar = Tabula-Grammar.new;
my $actions = Target-Testopia.new;
my $context = $actions.Context;
my $binder = $actions.Binder;


#my $root_default = 'k:\\code\\acadis_trunk\\';
my $root_default = 'd:\\code\\acadis\\';

sub MAIN(Str :$folder = $root_default, Str :$scenario-file) {

    die "$folder does not exist there." unless $folder.IO.d;
    die "$scenario-file does not exist there." unless $scenario-file.IO.f;

    say "Loading fixtures...";
    $binder.debug = True;
    my $imps = $root_default ~ 'ScenarioTests\\ScenarioContext\\Implementations';
    my $view-imps = $root_default ~ 'ScenarioTests\\ScenarioContext\\ViewImplementations';
    #$binder.load-fixtures($imps);
    $binder.load-fixtures($imps, $view-imps);


    while(True) {

        say "Parsing $scenario-file..."
        $context.file-name = $scenario-file;
        my $input = slurp $scenario-file;

        my $parse = $grammar.parse( $input, :actions($actions) );

        die "$scenario-file did not parse." unless $parse ~~ Match;

        my $generated-class = $parse.made;
        spurt $scenario-file ~ "_generated.cs", $generated-class;
        say "Done.";

        my $response = prompt "'q' to quit or anything else to reparse: ";
        exit if $response ~~ /q/;
    }
    
}
