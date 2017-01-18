use Tabula::Fixture-Binder;
use Tabula::Fixture-Class;

my $grammar = Tabula-Grammar.new;
my $actions = Target-Testopia.new;
my $context = $actions.Context;
my $binder = $actions.Binder;

my Bool $fixtures-loaded = False;

my $root_default = 'k:\\code\\acadis_trunk\\';
#my $root_default = 'd:\\code\\acadis\\';

sub MAIN(Str :$folder = $root_default, Str :$scenario-name) {

    die "$folder does not exist there." unless $folder.IO.d;
    die "$scenario-name does not exist there." unless $scenario-name.IO.f;


    say 'l: load fixtures, g: generate .cs from .tab, q: quit';
    say 's <filename>: work on scenario named <filename>';
    while(True) {

        given prompt "Tabula: " {
            when /l/ { load-fixtures(); }
            when /g/ { load-scenario(); }
            when /q/ { exit; }
            when /s/ { / s <.ws>+ (.+) /; $scenario-name = ~$0; }
            default { say "l)oad, g)enerate, s)cenario <filename>, q)uit"; }
        }

        CATCH { default { say .messsage; } }
    }
}

sub load-scenario(:$scenario-name) {
    my $output-name = s/ '.tab' $ / _generated.cs / given $scenario-name;
    say "Generating $output-name...";

    $context.file-name = $scenario-name;
    my $input = slurp $scenario-name;

    my $parse = $grammar.parse( $input, :actions($actions) );

    die "$scenario-name did not parse." unless $parse ~~ Match;

    my $generated-class = $parse.made;
    spurt $output-name, $generated-class;
}

sub load-fixtures() {
    say "Loading fixtures...";

    #$binder.debug = True;
    my $imps = $root_default ~ 'ScenarioTests\\ScenarioContext\\Implementations';
    my $view-imps = $root_default ~ 'ScenarioTests\\ScenarioContext\\ViewImplementations';

    #$binder.load-fixtures($imps);
    $binder.load-fixtures($imps, $view-imps);
    $fixtures-loaded = True;
}
