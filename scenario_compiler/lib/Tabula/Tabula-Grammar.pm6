#use Grammar::Tracer;

grammar Tabula-Grammar {
    rule TOP { <Scenario> }
    # TODO: rule Module { [Scenario || Library] + }

    token ws { \h* }
    token Indentation { \h* }
    rule  Comment { '//' \N* }

    rule  Scenario { <Indentation> [:i scenario] ':' <String> \n <Section>* }
    token Section { <Break-Line> || <Document> || <Table> || <Paragraph> }
    token Break-Line { ^^ <.Comment>? \n }
    rule  Document { 'start_doc' <String>? \n .*? \n 'end_doc' }  #  crappy first draft placeholder

    rule Table { <Table-Label>? <Table-Header> <Table-Row>* }
    rule Table-Label  { <Indentation> '===' <Phrase> '===' \n }
    rule Table-Header { <Indentation> '[' <Table-Cells> ']' \n }
    rule Table-Row    { <Indentation> '|' <Table-Cells> '|' \n }
    rule Table-Cells  { <Table-Cell>+ % '|' }
    rule Table-Cell   { :my $*Phrase-Context = 'Cell'; <Phrases> || <Empty-Cell> }
    rule Empty-Cell   { \h+ }

    rule  Paragraph  { <Paragraph-Label>? <.Para-Open> <Statement>+ <.Para-Close> }
    token Para-Open  { <?> }
    token Para-Close { <?> }
    rule  Paragraph-Label { <Indentation> <String> ':' \n }
    rule  Statement  { <Indentation> <Action> <Comment>? \n }
    rule  Action     { <Block> || <Step> || <Command> }

    rule Block { <String>? <Block-Begin> \n <Section>+ <Block-End> }
    token Block-Begin { '...' }
    token Block-End   { <Indentation> '.' }

    token Step { $<OptQ> = '? '? <Phrase> }

    rule Command {
        '>'
            [<Command-Alias> || <Command-Set> || <Command-Tag> || <Command-Use>
            || <parse-failure("Current commands are: alias, set, tag, and use")>]
    }
    rule Command-Alias {
        alias ':' [
            [<String> means <Action>]
            || <parse-failure('Misformed "alias", should look like: >alias: "verify all" means verify sections 1 through 99')>
        ]
    }
    rule Command-Set {
        set ':' [
            [[<Word> || <Variable>] means <Term>]
            || <parse-failure('Misformed "set", should look like: >set: nickname means "Frankie-Boy"')>
        ]
    }
    rule Command-Tag {
        tags? ':' [
            <Phrases>
            || <parse-failure('Misformed "tag", should look like: >tag: AC-24027, Housing, Maintenance')>
        ]
    }
    rule Command-Use {
        use ':' [
            <Phrases>
            || <parse-failure('Misformed "use", should look like: >use: Person Management, Employment')>
        ]
    }


    rule  Phrases { <Phrase>+ % ',' }
    rule  Phrase  { <Symbol> + }
    token Symbol  { <Word> || <Term> }
    token Word    { [<:Letter> || <[ _ \' \- ]> ] [\w || <[ _ \' \- ]>] * }
    token Term    { [ <Date> || <Number> || <String> || <Variable> ] }

    token Number   { <[+-]>? [[\d+ ['.' \d*]?] || ['.' \d+]] }
    token String   { '"' $<Body> = [ <-["]>* ] '"' }  # TODO: single quotes, quote escaping
    token Variable { '#' <Word> }
    token Date     { \d\d? '/' \d\d? '/' \d\d\d\d }

    method parse-failure($message = 'Parse failure') {
        my $parsed-so-far = self.target.substr(0, self.pos);
        my @lines = $parsed-so-far.lines;
        die $message ~ " at line @lines.elems(), after '@lines[*-1]'";
    }
}
