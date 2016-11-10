use Tabula::Execution-Context;
use Tabula::Code-Scribe;

class Target-Testopia {
    has Execution-Context $.Context;
    has Code-Scribe $.Scribe;

    submethod BUILD {
        $!Context = Execution-Context.new();
        $!Scribe = Code-Scribe.new();
    }

    sub get-Do-statement( $code, $source-location ) {
        my $quoted-code = '@"' ~ $code.subst('"', '""', :g) ~ '"';

        'Do(() =>     ' ~ $code
            ~ ',     "' ~ $source-location ~ '", ' ~ $quoted-code ~ ' );';
    }

    method normalized-name-CSharp() {
        $!Context.file-name.subst('.scn', '')
    }


    ##################################################################
    ### Grammar methods, alphabetically (putting TOP at the bottom :)

    #ok
    method Action($/) {
        make ($<Step> || $<Block> || $<Command>).made
    }

    #C#
    method Block($/) {
        $!Context.open-scope($<String>);

        make
            '{  //  ' ~ ($<String> // "unnamed block") ~ "\n"
            ~ ($<Section> ?? [~] $<Section>.map({.made}) !! "")
            ~ $<Block-End><Indentation> ~ "\}\n";

        $!Context.close-scope();
    }

    #scribe: no need
    # method Break-Line($/) {
    #     make "\n"
    # }

    #ok
    method Command($/) {
        make ($<Command-Use> || $<Command-Tag> || $<Command-Alias>).made
    }

    #C#
    method Command-Alias($/) {
        my $lhs = $<Word>
            ?? '"' ~ $<Word>.lc ~ '"'
            !! $<ID>.made;
        my $rhs = $<Term>.made;

        make get-Do-statement( 'alias[' ~ $lhs ~ '] = ' ~ $rhs , "SampleScenario.scn:1" );
    }

    #--
    method Command-Step($/) {

    }

    #tb
    method Command-Tag($/) {
        my $cmd = $<PhraseList>.elems == 1 ?? 'tag' !! 'tags';
        make ">$cmd: $<Phrases>"
    }

    #ok
    method Command-Use($/) {
        for $<Phrases><Phrase> {
            $!Context.AddLibraryToScope(~$_);
        }
    }

    #C#
    method ID($/) {
        make 'alias["' ~ $<Word>.lc ~ '"]';
    }

    #scribe
    method Paragraph($/) {
        make $!Scribe.compose-paragraph($/);
    }

    #tb
    method Phrase($/) {
        make '"' ~ $<Symbol>.join(' ') ~ '"';
    }

    #ok
    method Phrases($/) {
        #make $<Phrase>.map({ '"' ~ .made ~ '"' }).join(', ');
        make $<Phrase>[0].made;
        #make "-phrases-"
    }

    #scribe
    method Scenario($/) {
        $!Scribe.scenario-title = $<String>;
        $!Scribe.file-name = $!Context.file-name;

        make $!Scribe.Assemble();
    }

    #ok
    method Section($/) {
        make ($<Paragraph> || $<Table> || $<Document> || $<Break-Line>).made
    }

    #tb
    method Statement($/) {
        make
            ($<Indentation> // "")
            ~ $<Action>.made
            ~ ($<Comment> ?? "  " ~ $<Comment> !! "")
            ~ "\n"
    }

    #C#
    method Step($match) {
        #my $source-location = $!Context.file-name ~ ':' ~ line-of-match-start($match);
        my $source-location = $!Context.source-location($match);

        my $result = $!Context.resolve-action($match);
        if $result {
            $match.make( get-Do-statement( $result, $source-location ) );
        }
        else {
            my $stepText = $match.subst('"', '\"', :g);
            $match.make( 'Unfound(     "' ~ $stepText ~ '",     "' ~ $source-location ~ '" );' );
        }
    }

    #tb
    method Symbol($/) {
        make ($<Word> || $<Term>.made) ~ ' '
    }

    #scribe
    method Table($/) {
        make $!Scribe.compose-table($/);
    }

    #tb
    method Table-Cell($/) {
        make ($<Phrases> ?? $<Phrases>.made !! $<Empty-Cell>)
    }

    #tb
    method Table-Cells($/) {
        make $<Table-Cell>.map({.made}).join(', ')
    }

    #C#
    method Table-Header($/) {
        make $<Indentation> ~ '{ ' ~ $<Table-Cells>.made ~ " \}\n"
    }

    #tb
    method Table-Label($/) {
        make "$<Indentation>=== $<Step> ===\n"
    }

    #C#
    method Table-Row($/) {
        make $<Indentation> ~ '{ ' ~ $<Table-Cells>.made ~ " \}\n"
    }

    #ok
    method Term($/) {
        make $<Date> || $<Number> || $<String> || $<ID>.made
    }

    #ok
    method TOP($/) {
        make $<Scenario>.made
    }
}
