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
            ~ '.'
    }

    method Break-Line($/) {
        make "\n"
    }

    method Command($/) {
        make ($<Command-Use> || $<Command-Tag> || $<Command-Alias> || $<Command-Set>).made
    }

    method Command-Set($/) {
        my $lhs = $<Word> || $<Variable>;
        make ">set: $lhs means $<Term>"
    }

    method Command-Alias($/) {
        make ">alias: $<Phrase> means $<Action>"
    }

    method Command-Tag($/) {
        my $cmd = $<Phrases>.elems == 1 ?? 'tag' !! 'tags';
        make ">$cmd: $<Phrases>"
    }

    method Command-Use($/) {
        make ">use: $<Phrases>"
    }

    method Document($/) {
        make "crappy first draft placeholder"
    }

    method Empty-Cell($/) {
        make ' '
    }

    method Paragraph($/) {
        make ($<Paragraph-Label>.defined ?? $<Paragraph-Label>.made ~ "\n" !! '') ~ [~] $<Statement>.map({.made})
        # future (grammar):  blank lines and shifts from statements to tables
    }

    method Paragraph-Label($/) {
        make $<String> ~ ':'
    }

    method Phrases($/) {
        make $<Phrase>.map({.made}).join(', ')
    }

    method Phrase($/) {
        make $<Symbol>.map({.made}).join(' ')
    }

    method Scenario($/) {
        make "Scenario:  $<String>\n" ~ [~] $<Section>.map({.made})
    }

    method Section($/) {
        make ($<Paragraph> || $<Table> || $<Document> || $<Break-Line>).made
    }

    method Statement($/) {
        make
            $<Action>.made
            ~ ($<Comment> ?? "  " ~ $<Comment> !! "")
            ~ "\n"
    }

    method Step($/) {
        (make $<OptQ> ~ $<Phrase>.made).trim
    }

    method Symbol($/) {
        make ($<Word> ?? $<Word>.trim !! $<Term>.made)
    }

    method Table($/) {
        make
            ($<Table-Label> ?? $<Table-Label>.made !! "")
            ~ ($<Table-Header> ?? $<Table-Header>.made !! "")
            ~ [~] $<Table-Row>.map({.made})
    }

    method Table-Cell($/) {
        if $<Phrases> { make $<Phrases>.made }
        else { make ' ' }
    }

    method Table-Cells($/) {
        my @cells = $<Table-Cell>;
        @cells.pop if ($*Cells-Context.defined and $*Cells-Context eq 'Row');
        make @cells.map({.made}).join(' | ')
    }

    method Table-Header($/) {
        make "[ $($<Table-Cells>.made) ]\n"
    }

    method Table-Label($/) {
        make "=== $<Phrase>.made ===\n"
    }

    method Table-Row($/) {
        make "| $($<Table-Cells>.made) |\n"
    }

    method Term($/) {
        make $<Date> || $<Number> || $<String> || $<Variable>
    }
}
