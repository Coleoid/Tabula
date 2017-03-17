#use Grammar::Tracer;

grammar Tabula-Grammar {
    rule TOP { <Scenario> }
    # TODO: rule Module { [Scenario || Library] + }

    token ws { \h* }
    rule  Comment { '//' \N* }

    rule  Scenario { [:i scenario] ':' <String> \n <Section>* }
    token Section { <Break-Line> || <Document> || <Table> || <Paragraph> }
    token Break-Line { ^^ <.Comment>? \n }
    rule  Document { 'start_doc' <String>? \n .*? \n 'end_doc' }  #  crappy first draft placeholder

    rule Table { <Table-Label>? <Table-Header> <Table-Row>* }
    rule Table-Label  { '===' <Phrase> '===' \n }
    rule Table-Header { '[' <Table-Cells> ']' \n }
    rule Table-Row    { :my $*Cells-Context = 'Row'; '|' <Table-Cells> \n }
    rule Table-Cells  { <Table-Cell>+ % '|' }
    rule Table-Cell   { :my $*Phrase-Context = 'Cell'; <Phrases> || <Empty-Cell> }
    token Empty-Cell  { \h* }

    rule  Paragraph  { <Paragraph-Label>? <.Para-Open> <Statement>+ <.Para-Close> }
    token Para-Open  { <?> }
    token Para-Close { <?> }
    rule  Paragraph-Label { <String> ':' \n }
    rule  Statement  { <Action> <Comment>? \n }
    rule  Action     { <Block> || <Step> || <Command> }

    rule Block { <String>? <Block-Begin> \n <Section>+ <Block-End> }
    token Block-Begin { '...' }
    token Block-End   { '.' }

    token Step { $<OptQ> = '? '? <Phrase> }
    token Step-Phrase { <Step-Symbol>+ % \h+ }
    token Step-Symbol { <Word> || <Terms> }
    token Terms { <Term>+ % [',' \h*] }      # it could be tighter, but payoff is dubious...

    rule Command {
        '>'
            [<Command-Alias> || <Command-Set> || <Command-Tag> || <Command-Use>
            || <rule-failure('Command', "'alias', 'set', 'tag', or 'use'")> ]
    }
    rule Command-Alias {
        alias ':' [
            [<String> means <Action>]
            || <rule-failure('>alias: command', 'you to do it right')>
        ]
    }
    rule Command-Set {
        set ':' [
            [   || <Word>
                || <Variable>
                || <rule-failure('>set: command', 'an unquoted word or a variable')>
            ]
            [   || means
                || is
                || to
                || <rule-failure('>set: command', "'means', 'is', or 'to'")>
            ]
            [   || <Term>
                || <rule-failure('>set: command', 'a string, number, date, or variable to store')>
            ]
        ]
    }
    rule Command-Tag {
        tags? ':' [
            <Phrases>
            || <rule-failure('>tag: command', 'one or more comma-separated phrases')>
        ]
    }
    rule Command-Use {
        use ':' [
            <Phrases>
            || <rule-failure('>use: command', 'one or more comma-separated phrases')>
        ]
    }


    rule  Phrases { <Phrase>+ % ',' }
    rule  Phrase  { <Symbol>+ % \h+ }
    token Symbol  { <Word> || <Term> }
    token Word    { [<:Letter> || <[ _ \' \- ]> ] [\w || <[ _ \' \- ]>] * }
    token Term    { [ <Date> || <Number> || <String> || <Variable> ] }

    token Number   { <[+-]>? [[\d+ ['.' \d*]?] || ['.' \d+]] }
    token String   { '"' $<Body> = [ <-["]>* ] '"' }  # TODO: single quotes, quote escaping
    token Variable { '#' <Word> }
    token Date     { \d\d? '/' \d\d? '/' \d\d\d\d }


    method rule-failure($rule, $expected = '') {
        my $parsed-so-far = self.target.substr(0, self.pos);
        my @lines = $parsed-so-far.lines;
        my $line-of-failure = @lines.elems();
        my $line-before-failure = @lines[*-1];
        my $column-of-failure = $line-before-failure.chars() + 1;
        my $line-after-failure = self.target.substr(self.pos);

        my $expectation-line = ($expected eq '')
            ?? ''
            !! "(expected $expected)\n";

        my $message =
            "Did not parse as a $rule: line $line-of-failure at column $column-of-failure:\n"
            ~ $line-before-failure ~ '[here->]' ~ $line-after-failure ~ "\n"
            ~ $expectation-line;

        die $message;
    }
}
