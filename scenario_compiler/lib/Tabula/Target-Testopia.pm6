use v6;
use Tabula::Execution-Context;
use Tabula::Code-Scribe;
use Tabula::Fixture-Binder;
use Tabula::Match-Helper;

class Target-Testopia does Match-Helper {
    has Execution-Context $.Context;
    has Code-Scribe $.Scribe;
    has Fixture-Binder $.Binder;

    submethod BUILD {
        $!Context = Execution-Context.new();
        $!Scribe = Code-Scribe.new();
        $!Binder = Fixture-Binder.new();
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

    #nn?
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

    #nn?
    method Command($/) {
        # I think I can simply make "" here and not bother with it lower down
        make ($<Command-Use> || $<Command-Tag> || $<Command-Set>).made
    }

    #C#
    method Command-Set($/) {
        my $lhs = $<Word>
            ?? '"' ~ $<Word>.lc ~ '"'
            !! $<Variable>.made;
        my $rhs = $<Term>.made;

        make get-Do-statement( 'var[' ~ $lhs ~ '] = ' ~ $rhs , "SampleScenario.scn:1" );
    }

    #--
    method Command-Alias($/) {
    }

    #echo
    method Command-Tag($/) {
        my $cmd = $<PhraseList>.elems == 1 ?? 'tag' !! 'tags';
        make ">$cmd: $<Phrases>"
    }

    #scribe
    method Command-Use($/) {
        for $<Phrases><Phrase> -> $fixture-label {
            my $fixture = $!Binder.get-class($fixture-label);
            if so $fixture {
                $!Context.add-fixture($fixture);
                $!Scribe.initialize-fixture($fixture);
            }
            else {
                #TODO: notify on failure to find fixture
            }
        }

        make ""
    }

    #C#
    method Variable($/) {
        make 'var["' ~ $<Word>.lc ~ '"]';
    }

    #scribe
    method Paragraph($/) {
        my $name = self.name-section('paragraph', $/);
        my $statements = [~] $<Statement>.map({ .made ?? ("            " ~ .made) !! "" });

        make $!Scribe.compose-paragraph( $name, $statements );
    }

    #echo
    method Phrase($/) {
        make '"' ~ $<Symbol>.join(' ') ~ '"';
    }

    #ok
    method Phrases($/) {
        make $<Phrase>[0].made;
    }

    #scribe
    method Scenario($/) {
        $!Scribe.scenario-title = $<String>;
        $!Scribe.file-name = $!Context.file-name;

        make $!Scribe.compose-file();
    }

    #nn?
    method Section($/) {
        make ($<Paragraph> || $<Table> || $<Document> || $<Break-Line>).made
    }

    #echo
    method Statement($/) {
        my $statement = $<Action>.made;
        my $comment-suffix = ($<Comment> ?? "  " ~ $<Comment> !! "");
        if $statement or $comment-suffix {
            make $statement ~ $comment-suffix ~ "\n";
        }
        else {
            make "";
        }
    }

    #C#
    method Step($match) {
        my $source-location = $!Context.source-location($match);

        my $result = $!Context.resolve-step($match);
        if $result {
            $match.make( get-Do-statement( $result, $source-location ) );
        }
        else {
            my $stepText = $match.subst('"', '\"', :g);
            $match.make( 'Unfound(     "' ~ $stepText ~ '",     "' ~ $source-location ~ '" );' );
        }
    }

    #echo
    method Symbol($/) {
        make ($<Word> || $<Term>.made) ~ ' '
    }

    #scribe
    method Table($/) {
        my $name = self.name-section('table', $/);
        my $header = $<Table-Header>.made.chomp;
        my @rows = $<Table-Row>.map({.made.chomp});

        make $!Scribe.compose-table($name, $header, @rows);
    }

    #nn?
    method Table-Cell($/) {
        make ($<Phrases> ?? $<Phrases>.made !! $<Empty-Cell>)
    }

    #echo
    method Table-Cells($/) {
        make $<Table-Cell>.map({.made}).join(', ')
    }

    #C#
    method Table-Header($/) {
        make $<Indentation> ~ '{ ' ~ $<Table-Cells>.made ~ " \}\n"
    }

    #echo
    method Table-Label($/) {
        make "$<Indentation>=== $<Step> ===\n"
    }

    #C#
    method Table-Row($/) {
        make $<Indentation> ~ '{ ' ~ $<Table-Cells>.made ~ " \}\n"
    }

    #nn?
    method Term($/) {
        make $<Date> || $<Number> || $<String> || $<Variable>.made
    }

}
