use Tabula::Build-Context;

class Generate-CSharp {
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
        for $<Phrases> {
            $!Context.AddLibraryToScope($_);
        }
    }

    method Paragraph($/) {
        #TODO:  Line number counting
        make
            "\tpublic void paragraph_01()\n\{"
            ~ [~] $<Statement>.map({ "\t\t" ~ .made})
            ~ "\}\n\n"
    }

    method Phrase($/) {
        make $<Symbol>.join(' ');
    }

    method Phrases($/) {
        make $<Phrase>.join(', ');
    }

    method Scenario($/) {
        #TODO:  Class name should come off of file name
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
        my $sourceLocation = '"SampleScenario.scn:1"'; #HACK: sourceLocation

        my ($success, $fixtureCall) = $!Context.GetFixtureCall($match);
        if $success {
            my $quotedCommand = '@"' ~ (S:g / '"' /""/) ~ '"';
            $match.make( "Do( () =>    $_,    $sourceLocation, $quotedCommand );" );
        }
        else {
            my $stepText = ~$match;
            $match.make( "Unfound(     \"$stepText\",    $sourceLocation )");
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
