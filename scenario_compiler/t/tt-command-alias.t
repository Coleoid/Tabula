use Test;
use Tabula::Grammar-Testing;

my (&parser, $context) = curry-parser-emitting-Testopia( "Command" );
say "\n";

#TODO:  Think!  How should aliases and libraries interact?

#if False
{   diag "An alias command adds an alias to the current scope";

    my $parse = parser( "simple value alias", '>alias: this => "that"' );
    is $parse.made, 'Do(() =>     alias["this"] = "that",     "SampleScenario.scn:1", @"alias[""this""] = ""that""" );',
        "a simple value alias is harnessed the same way as a step";

    $parse = parser( "another simple value alias", '>alias: other => "another"' );
    is $parse.made, 'Do(() =>     alias["other"] = "another",     "SampleScenario.scn:1", @"alias[""other""] = ""another""" );',
        "resulting alias uses supplied values";

    $parse = parser( "aliases on each side", '>alias: #nickname => #fullName' );
    is $parse.made, 'Do(() =>     alias[alias["nickname"]] = alias["fullname"],     "SampleScenario.scn:1", @"alias[alias[""nickname""]] = alias[""fullname""]" );',
        "evaluating references";

}

done-testing;
