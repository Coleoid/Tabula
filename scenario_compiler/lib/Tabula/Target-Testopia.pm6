use Tabula::Build-Context;
use Tabula::CSharp-Scribe;

class Target-Testopia {
    has $.Context;
    has $.Scribe;

    submethod BUILD {
        $!Context = Build-Context.new();
        $!Scribe = CSharp-Scribe.new();
    }



    sub line-of-match-start($match) {
        1 + ($match.prematch.comb: /\n/)
    }

    sub lines-in-match($match) {
        1 + ($match.comb: /\n/)
    }

    sub get-Do-statement( $code, $source-location ) {
        my $quoted-code = '@"' ~ $code.subst('"', '""', :g) ~ '"';

        'Do(() =>     ' ~ $code
            ~ ',     "' ~ $source-location ~ '", ' ~ $quoted-code ~ ' );';
    }


    ##################################################################
    ### Grammar methods, alphabetically (which puts TOP at the bottom)

    method Action($/) {
        make ($<Step> || $<Block> || $<Command>).made
    }

    method Block($/) {
        $!Context.BeginScope($<String>);

        make
            '{  //  ' ~ ($<String> // "unnamed block") ~ "\n"
            ~ ($<Section> ?? [~] $<Section>.map({.made}) !! "")
            ~ $<Block-End><Indentation> ~ "\}\n";

        $!Context.EndScope();
    }

    method Break-Line($/) {
        make "\n"
    }

    method Command($/) {
        make ($<Command-Use> || $<Command-Tag> || $<Command-Alias>).made
    }

    method Command-Alias($/) {
        my $lhs = $<Word>
            ?? '"' ~ $<Word>.lc ~ '"'
            !! $<ID>.made;
        my $rhs = $<Term>.made;

        make get-Do-statement( 'alias[' ~ $lhs ~ '] = ' ~ $rhs , "SampleScenario.scn:1" );
    }

    method Command-Step($/) {

    }

    method Command-Tag($/) {
        my $cmd = $<PhraseList>.elems == 1 ?? 'tag' !! 'tags';
        make ">$cmd: $<Phrases>"
    }

    method Command-Use($/) {
        for $<Phrases><Phrase> {
            $!Context.AddLibraryToScope(~$_);
        }
    }

    method ID($/) {
        make 'alias["' ~ $<Word>.lc ~ '"]';
    }

    method Paragraph($/) {
        my $start-line = line-of-match-start($/);
        my $end-line = $start-line + lines-in-match($/) - 2;

        my $name = sprintf("paragraph_from_%03d_to_%03d", $start-line, $end-line);
        my $para = "        public void " ~ $name ~ "()\n        \{\n "
            ~ [~] $<Statement>.map({ "           " ~ .made})
            ~ "        \}\n";

        $!Scribe.add-para-to-scenario($name);
        $!Scribe.declare-paragraph($para);

        make $para;  # this only supports unit tests, yet pays off until UT rewrite, at least.
    }

    method Phrase($/) {
        make $<Symbol>.join(' ');
    }

    method Phrases($/) {
        make $<Phrase>.join(', ');
    }

    method normalized-name-CSharp() {
        $!Context.file-name.subst('.scn', '')
    }

    method Scenario($/) {
        $!Scribe.scenario-title = $<String>;
        $!Scribe.file-name = $!Context.file-name;

        make $!Scribe.Assemble();
    }

    method Section($/) {
        make ($<Paragraph> || $<Table> || $<Document> || $<Break-Line>).made
    }

    method Statement($/) {
        make
            ($<Indentation> // "")
            ~ $<Action>.made
            ~ ($<Comment> ?? "  " ~ $<Comment> !! "")
            ~ "\n"
    }

    method Step($match) {
        my $source-location = $!Context.file-name ~ ':' ~ line-of-match-start($match);

        my $result = $!Context.GetFixtureCall($match, $source-location);
        if $result {
            $match.make( get-Do-statement( $result, $source-location ) );
        }
        else {
            my $stepText = $match.subst('"', '\"', :g);
            $match.make( 'Unfound(     "' ~ $stepText ~ '",     "' ~ $source-location ~ '" );' );
        }
    }

    method Symbol($/) {
        make ($<Word> || $<Term>.made) ~ ' '
    }

    method Table($/) {
        make
            ($<Table-Label> || "")
            ~ ($<Table-Header> || "")
            ~ [~] $<Table-Row>.map({.made})
    }

    method Table-Cell($/) {
        make ($<Phrases> || $<Empty-Cell>).made
    }

    method Table-Cells($/) {
        make $<Table-Cell>.join('|')
    }

    method Table-Header($/) {
        make $<Indentation> ~ "[ $<Table-Cells>]\n"
    }

    method Table-Label($/) {
        make "$<Indentation>=== $<Step> ===\n"
    }

    method Table-Row($/) {
        make "$<Indentation>|$<Table-Cells>|\n"
    }

    method Term($/) {
        make $<Date> || $<Number> || $<String> || $<ID>.made
    }

    method TOP($/) {
        make $<Scenario>.made
    }
}
