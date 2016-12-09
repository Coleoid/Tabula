use Test;
use Tabula::Grammar-Testing;

my (&parser, $actions) = curry-parser-emitting-Testopia( "Command" );
say "\n";

#TODO:  Think!  How should aliases and libraries interact?

#if False
{   diag "A set command adds a variable to the current scope";

    my $parse = parser( "simple value set", '>set: this => "that"' );
    is $parse.made, 'Do(() =>     var["this"] = "that",     "SampleScenario.scn:1", @"var[""this""] = ""that""" );',
        "a simple variable is harnessed the same way as a step";

    $parse = parser( "another simple value set", '>set: other => "another"' );
    is $parse.made, 'Do(() =>     var["other"] = "another",     "SampleScenario.scn:1", @"var[""other""] = ""another""" );',
        "resulting variable uses supplied values";

    $parse = parser( "aliases on each side", '>set: #nickname => #fullName' );
    is $parse.made, 'Do(() =>     var[var["nickname"]] = var["fullname"],     "SampleScenario.scn:1", @"var[var[""nickname""]] = var[""fullname""]" );',
        "variables can be set using other variables";

}

done-testing;
