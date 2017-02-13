#use Grammar::Tracer;

grammar Tabula-Grammar {
    rule TOP { <Scenario> }
    # TODO: rule Module { [Scenario || Library] + }
    token ws { \h* }

    rule  Scenario { <Indentation> [:i scenario] ':' <String> \n <Section>* }
    token Section { <Break-Line> || <Document> || <Table> || <Paragraph> }

    token Break-Line { ^^ <.Comment>? \n }
    rule  Document { 'start_doc' <String>? \n .*? \n 'end_doc' }  #  crappy first draft placeholder

    rule Table { <Table-Label>? <Table-Header> <Table-Row>* }
    rule Table-Label  { <Indentation> '===' <Phrase> '===' \n }
    rule Table-Header { <Indentation> '[' <Table-Cells> ']' \n }
    rule Table-Row    { <Indentation> '|' <Table-Cells> '|' \n }
    rule Table-Cells  { <Table-Cell>+ % '|' }
    token Table-Cell   { :my $*Phrase-Context = 'Cell'; <Phrases> || <Empty-Cell> }
    token Empty-Cell   { \h+ }

    token Paragraph  { <Paragraph-Label>? <.Para-Open> <Statement>+ <.Para-Close> }
    token Para-Open  { <?> }
    token Para-Close { <?> }
    rule Paragraph-Label { <Indentation> <String> ':' \n }
    rule Statement  { <Indentation> <Action> <Comment>? \n }
    token Action     { <Block> || <Step> || <Command> }

    rule Block { <String>? <Block-Begin> \n <Section>+ <Block-End> }
    token Block-Begin { '...' }
    token Block-End   { <Indentation> '.' }

    token Step { $<OptQ> = '? '? <Phrase> }

    rule Command { '>' <Command-Alias> || <Command-Set> || <Command-Tag> || <Command-Use> }
    rule Command-Alias { alias ':' <String> means <Action> }  # build time
    rule Command-Set   { set   ':' [ <Word> || <Variable> ] means <Term> }  # run time
    rule Command-Tag   { tags? ':' <Phrases> }
    rule Command-Use   { use   ':' <Phrases> }

    token Indentation { \h* }
    rule  Comment { '//' \N* }

    rule Phrases { <Phrase>+ % ',' }
    rule Phrase  { <Symbol> + }
    token Symbol  { <Word> || <Term> }
    token Word    { [<:Letter> || <[ _ \' \- ]> ] [\w || <[ _ \' \- ]>] * }
    token Term    { [ <Date> || <Number> || <String> || <Variable> ] }

    token Number   { <[+-]>? [[\d+ ['.' \d*]?] || ['.' \d+]] }
    token String   { '"' $<Body> = [ <-["]>* ] '"' }  # TODO: single quotes, quote escaping
    token Variable { '#' <Word> }
    token Date     { \d\d? '/' \d\d? '/' \d\d\d\d }
}
