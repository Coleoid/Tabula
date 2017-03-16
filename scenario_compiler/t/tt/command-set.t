use Test;
use Tabula::Grammar-Testing;

my (&parser, $composer) = curry-parser-emitting-Testopia( "Command" );
$composer.Context.file-name = "SampleScenario.scn";
say "\n";

if False
{   diag "A set command adds a variable to the current scope";

    my $parse = parser( "simple value set", '>set: this means "that"' );
    is $parse.made, 'Do(() =>     var["this"] = "that",     "SampleScenario.scn:1", @"var[""this""] = ""that""" );',
        "a simple variable is harnessed the same way as a step";

    $parse = parser( "another simple value set", '>set: other means "another"' );
    is $parse.made, 'Do(() =>     var["other"] = "another",     "SampleScenario.scn:1", @"var[""other""] = ""another""" );',
        "resulting variable uses supplied values";

    $parse = parser( "aliases on each side", '>set: #nickname means #fullName' );
    is $parse.made, 'Do(() =>     var[var["nickname"]] = var["fullname"],     "SampleScenario.scn:1", @"var[var[""nickname""]] = var[""fullname""]" );',
        "variables can be set using other variables";

}

#if False
{   diag "Syntax errors get good messages";
    my (&parse-failer, $actions) = curry-parser-expecting-parse-fail( "Command" );

    my &assert-parse-fail = curry-expect-parse-error(&parser);

    my $msg = assert-parse-fail( '>seet: this means "that"' );
    ,
        '"seet" is not a command.  Commands are: alias, set, tag, and use at line 1', 'bad command, good message' );

    assert-parse-fail( '>set: "this" means "that"',
        'Misformed "set", should look like: >set: nickname means "Frankie-Boy" at line 1, after \'>set: \'', 'recognized \'>set:\' when key was quoted' );
    assert-parse-fail( '>set: this means that',
        'Misformed "set", should look like: >set: nickname means "Frankie-Boy" at line 1, after \'>set: \'', 'recognized \'>set:\' when value was not a term' );
}

done-testing;
