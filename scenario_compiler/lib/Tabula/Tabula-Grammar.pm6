#use Grammar::Tracer;

grammar Tabula-Grammar {
    rule TOP { <Scenario> }
    # TODO:70 rule Module { [Scenario || Library] + }

    token Scenario { <Indentation> [:i scenario] <.ws> ':' <.ws> <String> \h* \n <Section>+ }
    token Section { <Break-Line> || <Paragraph> || <Table> || <Document> }
    token Paragraph { <Statement>+ }
    rule Document { start_doc .*? end_doc }  #  crappy first draft placeholder
    token Table { <Table-Label>? <Table-Header> <Table-Row>* }
        token Table-Label  { <Indentation> '===' [\h* <Step> \h*] '===' \h* \n }
        token Table-Header { <Indentation> '[' <Table-Cells> ']' \h* \n }
        token Table-Row    { <Indentation> '|' <Table-Cells> '|' \h* \n }
        token Table-Cells  { <Table-Cell>+ % '|' }
        token Table-Cell   { [\h* <Phrases> \h*] || <Empty-Cell> }
            token Empty-Cell { \h+ }

    token Statement { <Indentation> <Action> \h* <Comment>? \n }
    token Action { <Block> || <Step> || <Command> }
    token Step { $<OptQ> = '? '? \h* <Symbol>+ % \h+ } #TODO: replace body of Step with Phrase
    token Command { <Command-Use> || <Command-Alias> || <Command-Tag> || <Command-Step> }
        token Command-Use   { '>' use ':' \h* <Phrases> \h* }
        token Command-Tag   { '>' tags? ':' \h* <Phrases> \h* }
        token Command-Alias { '>' alias ':' \h* [ <Word> || <ID> ] \h* '=>' \h* <Term> }
        token Command-Step  { '>' step ':' \h* <Phrase> \h* '=>' \h* <Action> }
    token Block { <String>? \h* <Block-Begin> \h* \n <Section>+ <Block-End> }
        token Block-Begin { '...' }
        token Block-End { <Indentation> '.' }

    token Break-Line { ^^ \h* \n }
    token Indentation { \h* }
    token Comment { '//' \N* }

    token Phrases { <Phrase>+ % ',' }
    token Phrase  { \h* <Symbol>+ % \h* }
    token Symbol  { <Word> || <Term> }
    token Word    { [<:Letter> || <[ _ \' \- ]> ] [\w || <[ _ \' \- ]>] * }   # using just \w+, numbers were words.
    token Term    { [ <Date> || <Number> || <String> || <ID> ] }

    token Number { \d+ }
    token String { '"' <-["]>* '"' }
    token ID { '#' <Word> }
    token Date { \d\d? '/' \d\d? '/' \d\d\d\d }
}
