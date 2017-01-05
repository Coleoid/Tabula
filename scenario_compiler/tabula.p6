use Tabula::Fixture-Binder;
use Tabula::Fixture-Class;

my $grammar = Tabula-Grammar.new;
my $actions = Target-Testopia.new;
my $context = $actions.Context;
my $binder = $actions.Binder;


sub MAIN(Str :$folder, Str :$scenario-file) {

    die "$scenario-file does not exist there." unless $scenario-file.IO.f;
    die "$folder does not exist there." unless $folder.IO.d;

    $binder.debug = True;
    $binder.load-fixtures($folder);


    $context.file-name = $scenario-file;
    my $input = slurp $scenario-file;

    my $parse = $grammar.parse( $input, :actions($actions) );

    die "$scenario-file did not parse." unless $parse ~~ Match;

    my $generated-class = $parse.made;
    spurt $scenario-file ~ "_generated.cs", $generated-class;

}
