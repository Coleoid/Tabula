use Test;
use Tabula::Grammar-Testing;

say "\n";

#if False
{   diag "Section tests";
    my &parser = curry-parser-emitting-Tabula( 'Scenario' );

    my $parse = parser( 'two Paragraph Scenario', q:to/EOS/ );
    Scenario:  "A Tale of two Paras"
    bla bla bla
    blah blah de blah

    foo bar baz
    qux quux quuux
    EOS

    is $parse<Section>.elems, 3, "Three Sections in this Scenario";
    ok $parse<Section>[0]<Paragraph>, "First a Paragraph";
    ok $parse<Section>[1]<Break-Line>, "Then a Break-Line";
    ok $parse<Section>[2]<Paragraph>, "Then our second Paragraph";
}

#if False
{   diag "Command tests";
    my &parser = curry-parser-emitting-Tabula( 'Command' );
    my $parse;

    $parse = parser( 'use', ">use: Mail Server" );
    $parse = parser( 'tags', ">tags: Housing, Person Search, AC-20341" );
    $parse = parser( 'set', '>set: JR => "Jackie Robinson"' );
}


#if False
{   diag "Parameterized Step tests";
    my &parser = curry-parser-emitting-Tabula( 'Step' );

    my $step = 'Give me 4 things like "Bill" got on #day';
    my $parse = parser( 'parameterized', $step );

    my $symbols = $parse<Phrase><Symbol>;
    ok $symbols[2]<Term><Number>, "Number in Step parses as a Term";
    is $symbols[2], 4, "Expected value comes through for Number";

    ok $symbols[5]<Term><String>, "String in Step parses as a Term";
    is $symbols[5], '"Bill"', "Find value of String";

    ok $symbols[8]<Term><Variable>, "Tagged word in Step parses as a Term";
    is $symbols[8], "#day", "Find value of tag";

    $step = '"Eek!" Blocks and string-first Steps both work with LTM';
    $parse = parser( "string-first Step", $step );
}


#if False
{   diag "Statement tests -- Note that Statements are newline terminated.";
    my &parser = curry-parser-emitting-Tabula( "Statement" );

    my $parse = parser( "simple step", "do a thing\n" );
    #TODO: invalidate this test when I expand strings to be single-quote delimitable
    $parse = parser( "standard punctuation in step", "Better than half-baked\n" );
    $parse = parser( "Use Command", ">use: Floor Wax\n" );
    $parse = parser( "commented", "Thing  // the suchness of thing-nature\n" );
    is $parse<Comment>, '// the suchness of thing-nature', "Catch the optional comment";
}


#if False
{   diag "Block tests";
    my &parser = curry-parser-emitting-Tabula( "Block" );

    my $block-as-statement = q:to/EOB/;
        "Blocky string"...
        pick apples  // should only pick ripe
        ? verify 7 in basket
        eat #two
        .
        EOB
    my $block = $block-as-statement.chomp;

    my $parse = parser( "one paragraph block", $block );

    is ~$parse<String>, '"Blocky string"', "Catch the String header";
    is $parse<Section>.elems, 1, "Catch the single Section";

    my &statement-parser = curry-parser-emitting-Tabula( "Statement" );
    $parse = statement-parser( "block as statement", $block-as-statement );
}


#if False
{   diag "Table-Cells tests";
    my &parser = curry-parser-emitting-Tabula( 'Table-Cells' );

    my $table-cells = "Sinister | Dexter | Dorsal | Ventral";

    my $parse = parser( 'simple', $table-cells );
    is $parse<Table-Cell>.elems, 4, "Find four cells";
}

#if False
{   diag "Table tests";
    my &parser = curry-parser-emitting-Tabula( 'Table' );

    my $table = q:to/EOT/;
        ===   Chirality Today   ===
        [ Left        | Right     ]
        | Sinister    | Dexter    |
        | Port        | Starboard |
        | Exceptional | Standard  |
        EOT

    my $parse = parser( 'small full', $table );
    is $parse<Table-Label><Phrase>.made, "Chirality Today", "Get the label";
    is $parse<Table-Header><Table-Cells><Table-Cell>.elems, 2, "Find two columns in the header";
    is $parse<Table-Row>.elems, 3, "Found two rows";


    $table = q:to/EOT/;
        [ Left        | Right     ]
        EOT

    $parse = parser( 'header only', $table );
    nok $parse<Table-Label>, "Find no label";
    is $parse<Table-Header><Table-Cells><Table-Cell>.elems, 2, "Find two columns in the header";
    nok $parse<Table-Row>, "Find no rows";

    $table = q:to/EOT/;
        [ comment ]
        |         |
        EOT

    $parse = parser( "Empty Cell in Table", $table );

}

#if False
{   diag "Troublesome Table tests";
    my &parser = curry-parser-emitting-Tabula( 'Table' );

    my $table = q:to/EOT/;
        [ Supervisors ]
        | #Joan, #Bob |
        EOT

    my $parse = parser( "list in Table Cell", $table );
}


#if False
{   diag "Paragraph tests";
    my &parser = curry-parser-emitting-Tabula( 'Paragraph' );

    # TODO:30 Statement parsing which catches indentation so that the actions class can re-emit it.
    my $paragraph = q:to/EOP/;
        This is a step
        "A short block"...
            remains part of the paragraph
        .
        another step
        EOP

    my $parse = parser( 'contains block', $paragraph );
    is $parse<Statement>.elems, 3, "Three Statements in this paragraph";
}


#if False
{   diag "Scenario tests";
    my &parser = curry-parser-emitting-Tabula( 'Scenario' );

    my $scenario = q:to/EOS/;
        Scenario:  "Eat the music"

        Like a pomegranate
        Insides out
        EOS

    my $parse = parser( 'tiny scenario', $scenario );
    is $parse<String>, '"Eat the music"', "Scenario title";
    is $parse<Section>.elems, 2, "Two sections";
    ok $parse<Section>[0]<Break-Line>, "First a Break-Line";
    ok $parse<Section>[1]<Paragraph>, "Then a Paragraph";
}

done-testing;
