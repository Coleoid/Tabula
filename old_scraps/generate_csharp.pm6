
unit module Tabula;


class Generate-CSharp {

    method TOP($/) {
        make $<Scenario>.made
    }

    method Scenario($/) {
        #TODO: scenario class naming
        make "public class $<String>_generated \{\n"
        ~ [~] $<Section>.map({.made})
    }

    method Section($/) {
        make ($<Paragraph> || $<Table> || $<Document> || $<Break-Line>).made
    }

    method Paragraph($/) {
        #TODO: paragraph naming
        make
            "\tpublic method paragraph_01()\n\{"
            ~ [~] $<Statement>.map({ "\t\t" ~ .made})
            ~ "\}\n\n"
    }

    method Statement($/) {
        make
            ($<Indentation> // "")
            ~ $<Action>.made
            ~ ($<Comment> ?? "  " ~ $<Comment> !! "")
            ~ "\n"
    }

    method Action($/) {
        make ($<Step> || $<Block> || $<Command>).made
    }

    method Break-Line($/) {
        make "\n"
    }

    method Block($/) {
        make
            $<String> ~ "...\n"
            ~ ($<Section> ?? [~] $<Section>.map({.made}) !! "")
            ~ $<Block-End><Indentation> ~ '.'
    }

    method Step($/) {
        make "$<OptQ>$<Symbol>".trim
    }

    method Symbol($/) {
        make ($<Word> || $<Term>.made) ~ ' '
    }

    method Term($/) {
        make $<Date> || $<Number> || $<String> || $<ID>
    }

    method Table($/) {
        make
            ($<Table-Label> || "")
            ~ ($<Table-Header> || "")
            ~ [~] $<Table-Row>.map({.made})
    }

    method Table-Label($/) {
        make "$<Indentation>=== $<Step> ===\n"
    }

    method Table-Header($/) {
        make $<Indentation> ~ "[ $<Table-Cells>]\n"
    }

    method Table-Row($/) {
        make "$<Indentation>|$<Table-Cells>|\n"
    }

    method Table-Cells($/) {
        make join '|', $<Phrases>
    }

    method Command($/) {
        make ($<Use-Command> || $<Tag-Command> || $<Alias-Command>).made
    }

    method Use-Command($/) {
        make ">use: $<Phrases>"
    }

    method Tag-Command($/) {
        my $cmd = $<PhraseList>.elems == 1 ?? 'tag' !! 'tags';
        make ">$cmd: $<Phrases>"
    }

    method Phrases($/) {
        make $<Phrase>.join(', ');
    }

    method Phrase($/) {
        make $<Symbol>.join(' ');
    }

    method Alias-Command($/) {
        make ">alias: $<Step> => $<Action>"
    }
}
