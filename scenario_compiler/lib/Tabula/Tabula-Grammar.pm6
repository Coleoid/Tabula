#use Grammar::Tracer;

grammar Tabula-Grammar {
    rule TOP { <Scenario> }
    # TODO:70 rule Module { [Scenario || Library] + }

    token Scenario { <Indentation> [:i scenario] <.ws> ':' <.ws> <String> \h* \n <Section>* }
    token Section { <Break-Line> || <Document> || <Table> || <Paragraph> }

    token Break-Line { ^^ \h* <.Comment>? \n }
    token Document { start_doc [ \h+ <String> ]? \h* \n .*? \n end_doc }  #  crappy first draft placeholder

    token Table { <Table-Label>? <Table-Header> <Table-Row>* }
    token Table-Label  { <Indentation> '===' \h* <Phrase> \h* '===' \h* \n }
    token Table-Header { <Indentation> '[' <Table-Cells> ']' \h* \n }
    token Table-Row    { <Indentation> '|' <Table-Cells> '|' \h* \n }
    token Table-Cells  { <Table-Cell>+ % '|' }
    token Table-Cell   { [\h* <T-Phrase> \h*] || <Empty-Cell> }
    token Empty-Cell   { \h+ }

    token T-Phrase  { <T-Symbol>+ % \h+ }
    token T-Symbol  { <Word> || <T-Term> }
    token T-Term    { [ <Date> || <Number> || <String> || <Variable> ] }

    token Paragraph  { <Paragraph-Label>? <.Para-Open> <Statement>+ <.Para-Close> }
    token Para-Open  { <?> }
    token Para-Close { <?> }
    token Paragraph-Label { <Indentation> <String> \h* ':' \h* \n }
    token Statement  { <Indentation> <Action> \h* <Comment>? \n }
    token Action     { <Block> || <Step> || <Command> }

    token Block { <String>? \h* <Block-Begin> \h* \n <Section>+ <Block-End> }
    token Block-Begin { '...' }
    token Block-End   { <Indentation> '.' }

    token Step { $<OptQ> = '? '? <Phrase> }

    token Command { <Command-Alias> || <Command-Set> || <Command-Tag> || <Command-Use> }
    token Command-Alias { '>' alias ':' \h* <Phrase> \h* means \h* <Action> }  # build time
    token Command-Set   { '>' set   ':' \h* [ <Word> || <Variable> ] \h* means \h* <Term> }  # run time
    token Command-Tag   { '>' tags? ':' \h* <Phrases> \h* }
    token Command-Use   { '>' use   ':' \h* <Phrases> \h* }

    token Indentation { \h* }
    token Comment { '//' \N* }

    token Phrases { <Phrase>+ % [',' \h*] }
    token Phrase  { <Symbol>+ % \h+ }
    token Symbol  { <Word> || <Term> }
    token Word    { [<:Letter> || <[ _ \' \- ]> ] [\w || <[ _ \' \- ]>] * }
    token Term    { [ <Date> || <Number> || <String> || <Variable> ] }

    token Number   { <[+-]>? [[\d+ ['.' \d*]?] || ['.' \d+]] }
    token String   { '"' $<Body> = [ <-["]>* ] '"' }  # TODO: single quotes, quote escaping
    token Variable { '#' <Word> }
    token Date     { \d\d? '/' \d\d? '/' \d\d\d\d }
}
