use Test;
use Tabula::Grammar-Testing;

my (&parser, $composer) = get-parser-emitting-Testopia( "Command" );
$composer.Context.file-name = "SampleScenario.scn";
say "\n";

#if False
{   diag "A set command adds a variable to the current scope";

    my $parse = parser( '>set: this means "that"' );
    is $parse.made, 'Do(() =>     var["this"] = "that",     "SampleScenario.scn:1", @"var[""this""] = ""that""" );',
        "a simple variable is harnessed the same way as a step";

    $parse = parser( '>set: other means "another"' );
    is $parse.made, 'Do(() =>     var["other"] = "another",     "SampleScenario.scn:1", @"var[""other""] = ""another""" );',
        "resulting variable uses supplied values";

    $parse = parser( '>set: #nickname means #fullName' );
    is $parse.made, 'Do(() =>     var[var["nickname"]] = var["fullname"],     "SampleScenario.scn:1", @"var[var[""nickname""]] = var[""fullname""]" );',
        "variables can be set using other variables";

}

#if False
{   diag "Syntax errors get good messages";
    my (&not-a-command, $actions) = get-parser-expecting-parse-fail( "Command" );

    my $msg = not-a-command( '>sudo: make me a sandwich' );
    my $expected = q:to/EOE/;
        Did not parse as a Command: line 1 at column 2:
        >[here->]sudo: make me a sandwich
        (expected 'alias', 'set', 'tag', or 'use')
        EOE
    is $msg, $expected, "clear complaint about unrecognized command verb";

    $msg = not-a-command( '>set: "this" means "that"' );
    $expected = q:to/EOE/;
        Did not parse as a >set: command: line 1 at column 7:
        >set: [here->]"this" means "that"
        (expected an unquoted word or a variable)
        EOE
    is $msg, $expected, "clear complaint about wrong sort of target";

    $msg = not-a-command( '>set: this = "that"' );
    $expected = q:to/EOE/;
        Did not parse as a >set: command: line 1 at column 12:
        >set: this [here->]= "that"
        (expected 'means', 'is', or 'to')
        EOE
    is $msg, $expected, "clear complaint about wrong conjunctive verb";

    $msg = not-a-command( '>set: this means that' );
    $expected = q:to/EOE/;
        Did not parse as a >set: command: line 1 at column 18:
        >set: this means [here->]that
        (expected a string, number, date, or variable to store)
        EOE
    is $msg, $expected, "clear complaint about unsuitable value";
}

done-testing;
