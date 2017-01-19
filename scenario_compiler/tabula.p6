use Tabula::Fixture-Binder;
use Tabula::Fixture-Class;
use Tabula::Tabula-Grammar;
use Tabula::Target-Testopia;

my $grammar = Tabula-Grammar.new;
my $actions = Target-Testopia.new;
my $context = $actions.Context;
my $binder = $actions.Binder;
my $scribe = $actions.Scribe;

my Bool $debug = False;

my $root-default = 'k:\\code\\acadis_trunk\\';
#my $root-default = 'd:\\code\\acadis\\';

sub MAIN(Str :$scenario-name is required) {

    die "$root-default does not exist there." unless $root-default.IO.d;
    die "$scenario-name does not exist there." unless $scenario-name.IO.f;


    my $commands = "l)oad fixtures, g)enerate <scenario filename>, r)egenerate same scenario, q)uit";
    say $commands;
    while True {

        given prompt "Tabula: " {
            when /l/      { load-fixtures(); }
            when /g (.+)/ { $scenario-name = ~$0; load-scenario($scenario-name); }
            when /r/      { load-scenario($scenario-name); }
            when /q/      { exit; }
            when /d/      { $debug = not $debug; say "Debug now $debug."; }
            default       { say $commands; }
        }

        #CATCH { default { say .messsage; } }
    }
}

sub load-scenario($scenario-name) {
    my $output-name = S/ '.tab' $ /_generated.cs/ given $scenario-name;
    say "Generating $output-name...";

    my $file-name = $scenario-name.split( '\\' ).tail;
    $context.file-name = $file-name;
    my $input = slurp $scenario-name;

    my $parse = $grammar.parse( $input, :actions($actions) );
    say $parse if $debug;

    die "$scenario-name did not parse." unless $parse ~~ Match;

    my $generated-class = $scribe.compose-file();
    spurt $output-name, $generated-class;
}

sub load-fixtures() {
    say "Loading fixtures...";

    #$binder.debug = True;
    my $imps = $root-default ~ 'ScenarioTests\\ScenarioContext\\Implementations';
    my $view-imps = $root-default ~ 'ScenarioTests\\ScenarioContext\\ViewImplementations';

    #$binder.load-fixtures($imps);
    $binder.load-fixtures($imps, $view-imps);
}
