class Target-Echo {

    method TOP($/) {
        make $<Scenario>.made
        # future:  make all modules (with usual case being one scenario :)
    }

    method Action($/) {
        make ($<Step> || $<Block> || $<Command>).made
    }

    method Block($/) {
        make
            ($<String> // "") ~ "...\n"
            ~ ($<Section> ?? [~] $<Section>.map({.made}) !! "")
            ~ $<Block-End><Indentation> ~ '.'
    }

    method Break-Line($/) {
        make "\n"
    }

    method Command($/) {
        make ($<Command-Use> || $<Command-Tag> || $<Command-Alias> || $<Command-Step>).made
    }

    method Command-Alias($/) {
        my $lhs = $<Word> || $<ID>;
        make ">alias: $lhs => $<Term>"
    }

    method Command-Step($/) {
        make ">step: $<Phrase> => $<Action>"
    }

    method Command-Tag($/) {
        my $cmd = $<Phrases>.elems == 1 ?? 'tag' !! 'tags';
        make ">$cmd: $<Phrases>"
    }

    method Command-Use($/) {
        make ">use: $<Phrases>"
    }

    method Document($/) {
        make "crappy first draft"
    }

    method Empty-Cell($/) {
        make $/
    }

    method Paragraph($/) {
        make [~] $<Statement>.map({.made})
        # future (grammar):  blank lines and shifts from statements to tables
    }

    method Phrases($/) {
        make $<Phrase>.join(', ')
    }

    method Phrase($/) {
        make $<Symbol>.join(' ')
    }

    method Scenario($/) {
        make "Scenario:  $<String>\n" ~ [~] $<Section>.map({.made})
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

    method Step($/) {
        make "$<OptQ>$<Phrase>".trim
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
        make "$<Indentation>=== $<Phrase> ===\n"
    }

    method Table-Row($/) {
        make "$<Indentation>|$<Table-Cells>|\n"
    }

    method Term($/) {
        make $<Date> || $<Number> || $<String> || $<ID>
    }

}
