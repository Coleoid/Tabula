use Tabula::Build-Context;

class Target-Testopia {
    has $.Context;

    submethod BUILD {
        $!Context = Build-Context.new();
    }

    method TOP($/) {
        make $<Scenario>.made
    }

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
        make ">alias: $<Step> => $<Action>"
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

    sub line-of-match-start($match) {
        1 + ($match.prematch.comb: /\n/)
    }

    sub lines-in-match($match) {
        1 + ($match.comb: /\n/)
    }

    method Paragraph($/) {
        my $start-line = line-of-match-start($/);
        my $end-line = $start-line + lines-in-match($/) - 2;
        my $range-suffix = sprintf("_from_%03d_to_%03d", $start-line, $end-line);
        make
            "    public void paragraph" ~ $range-suffix ~ "()\n    \{\n "
            ~ [~] $<Statement>.map({ "   " ~ .made})
            ~ "    \}\n"
    }

    method Phrase($/) {
        make $<Symbol>.join(' ');
    }

    method Phrases($/) {
        make $<Phrase>.join(', ');
    }

    method Scenario($/) {
        #TODO:  Class name should be built from file name
        my $class_name = normalized-name-CSharp($<String>) ~ '_generated';
        make "public class $class_name \{\n"
            ~ [~] $<Section>.map({.made})
            ~ "}\n"
    }

    sub normalized-name-CSharp(@words) {
        join '', @words.map({.wordcase})
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
        my $source-location = '"SampleScenario.scn:' ~ line-of-match-start($match) ~ '"'; #HACK: source file name

        my $result = $!Context.GetFixtureCall($match, $source-location.subst('"', '', :g));
        if $result {
            my $quotedCommand = '@"' ~ $result.subst('"', '""', :g) ~ '"';
            $match.make( "Do(() =>     $result,     $source-location, $quotedCommand );" );
        }
        else {
            my $stepText = $match.subst('"', '\"', :g);
            $match.make( "Unfound(     \"$stepText\",     $source-location );");
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
        make $<Date> || $<Number> || $<String> || $<ID>
    }
}
