use Tabula::Execution-Context;
use Tabula::Code-Scribe;

class Target-Testopia {
    has $.Context;
    has $.Scribe;

    submethod BUILD {
        $!Context = Execution-Context.new();
        $!Scribe = Code-Scribe.new();
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
    ### Grammar methods, alphabetically (putting TOP at the bottom :)

    method Action($/) {
        make ($<Step> || $<Block> || $<Command>).made
    }

    method Block($/) {
        $!Context.open-scope($<String>);

        make
            '{  //  ' ~ ($<String> // "unnamed block") ~ "\n"
            ~ ($<Section> ?? [~] $<Section>.map({.made}) !! "")
            ~ $<Block-End><Indentation> ~ "\}\n";

        $!Context.close-scope();
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

    method name-paragraph($/) {
        my $start-line = line-of-match-start($/);
        my $end-line = $start-line + lines-in-match($/) - 2;

        sprintf("paragraph_from_%03d_to_%03d", $start-line, $end-line);
    }

    method compose-paragraph($/, $name) {
        my $para = "        public void " ~ $name ~ "()\n        \{\n "
            ~ [~] $<Statement>.map({ "           " ~ .made})
            ~ "        \}\n";

        return $para;
    }

    method Paragraph($/) {
        my $name = self.name-paragraph($/);
        my $para = self.compose-paragraph($/, $name);

        $!Scribe.declare-section($para);
        $!Scribe.use-section-in-scenario($name);

        make $para;  # just to support unit tests, right now
    }

    method Phrase($/) {
        make '"' ~ $<Symbol>.join(' ') ~ '"';
    }

    method Phrases($/) {
        #make $<Phrase>.map({ '"' ~ .made ~ '"' }).join(', ');
        make $<Phrase>[0].made;
        #make "-phrases-"
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

        my $result = $!Context.resolve-action($match);
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
        my $start-line = line-of-match-start($/);
        my $end-line = $start-line + lines-in-match($/) - 2;

        my $name = sprintf("table_from_%03d_to_%03d", $start-line, $end-line);

        my $table = '        ' ~ $name ~ " = new Table \{
            Header = new List<string>     " ~ $<Table-Header>.ast.chomp ~ ',';

        $table ~= '
            Data = new List<List<string>> {';

        for $<Table-Row> {
            $table ~= '
                new List<string>          ' ~ $_.ast.chomp ~ ',';
        }

        $table ~= '
            }
        };
';

        $!Scribe.add-section-to-scenario( $name );
        $!Scribe.declare-section( $table );
    }

    method Table-Cell($/) {
        make ($<Phrases> ?? $<Phrases>.made !! $<Empty-Cell>)
    }

    method Table-Cells($/) {
        make $<Table-Cell>.map({.made}).join(', ')
    }

    method Table-Header($/) {
        make $<Indentation> ~ '{ ' ~ $<Table-Cells>.made ~ " \}\n"
    }

    method Table-Label($/) {
        make "$<Indentation>=== $<Step> ===\n"
    }

    method Table-Row($/) {
        make $<Indentation> ~ '{ ' ~ $<Table-Cells>.made ~ " \}\n"
    }

    method Term($/) {
        make $<Date> || $<Number> || $<String> || $<ID>.made
    }

    method TOP($/) {
        make $<Scenario>.made
    }
}
