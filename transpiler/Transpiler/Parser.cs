using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Tabula
{
    public class Parser
    {

        public CST.Scenario ParseScenarioFile(string scenarioText)
        {
            var tokenizer = new Tokenizer();
            var tokens = tokenizer.Tokenize(scenarioText);
            var state = new ParserState(tokens);
            var scenario = ParseScenario(state);

            return scenario;
        }


        // Scenario:  Make stuff from things in place

        public CST.Scenario ParseScenario(ParserState state)
        {
            var scenario = new CST.Scenario();
            scenario.Tags = ParseTags(state);
            var token = state.Take(TokenType.ScenarioLabel,
                "Scenario should start with a label like  'Scenario: Student departure sends mail'");

            scenario.Label = token.Text;
            scenario.Sections = ParseSections(state);

            return scenario;
        }

        public List<string> ParseTags(ParserState state)
        {
            var tags = new List<string>();

            while(state.NextIs(TokenType.Tag))
            {
                tags.Add( state.Take().Text );
            }

            return tags;
        }

        public List<CST.Section> ParseSections(ParserState state)
        {
            var sections = new List<CST.Section>();

            for (var section = ParseSection(state); section != null; section = ParseSection(state))
                sections.Add(section);

            return sections;
        }

        public CST.Section ParseSection(ParserState state)
        {
            CST.Section section = new CST.Section();

            var tags = ParseTags(state);

            section = ParseParagraph(state);

            if (section == null)
                section = ParseTable(state);

            //TODO stick the tags on

            return section;
        }

        public CST.Paragraph ParseParagraph(ParserState state)
        {
            int rollbackPosition = state.Position;
            CST.Paragraph paragraph = new CST.Paragraph();

            paragraph.Workflows = ParseUseCommands(state);
            paragraph.Steps = ParseSteps(state);

            if (paragraph.Workflows.Count + paragraph.Steps.Count == 0)
            {
                state.Position = rollbackPosition;
                return null;
            }

            return paragraph;
        }

        public CST.Table ParseTable(ParserState state)
        {
            int rollbackPosition = state.Position;
            CST.Table table = new CST.Table();

            //  Optional Label
            table.Label = state.Take(TokenType.TableLabel)?.Text;

            //  If we get no row of column names, we aren't looking at a table.  Rollback.
            if (!state.NextIs(TokenType.TableRow))
            {
                //  ...unless we already found a table label, which can only go on a table.  Abort.
                if (table.Label != null)
                    throw new Exception("A table must have a row of column names");

                state.Position = rollbackPosition;
                return null;
            }
            table.ColumnNames = state.Take(TokenType.TableRow).Parts;

            //  Zero or more data rows
            table.Rows = ParseTableRows(state);

            return table;
        }

        public List<List<string>> ParseTableRows(ParserState state)
        {
            var rows = new List<List<string>>();

            while (state.NextIs(TokenType.TableRow))
            {
                rows.Add(state.Take().Parts);
            }

            return rows;
        }

        public List<string> ParseUseCommands(ParserState state)
        {
            var workflows = new List<string>();

            while(state.NextIs(TokenType.UseCommand))
            {
                workflows.AddRange(Regex.Split(state.Take().Text, ", *"));
            }

            return workflows;
        }

        public List<CST.Step> ParseSteps(ParserState state)
        {
            var steps = new List<CST.Step>();



            return steps;
        }

        public List<CST.Symbol> ParseSymbols(ParserState state)
        {
            var symbols = new List<CST.Symbol>();

            for (var symbol = ParseSymbol(state); symbol != null; symbol = ParseSymbol(state))
                symbols.Add(symbol);

            return symbols;
        }

        public CST.Symbol ParseSymbol(ParserState state)
        {
            CST.Symbol symbol = ParseWord(state);
            if (symbol == null) symbol = ParseTerm(state);

            return symbol;
        }

        public CST.Symbol ParseWord(ParserState state)
        {
            CST.Symbol word = null;


            return word;
        }

        public List<CST.Symbol> ParseTerms(ParserState state)
        {
            List<CST.Symbol> terms = new List<CST.Symbol>();

            for (var term = ParseTerm(state); term != null; term = ParseTerm(state))
            {
                terms.Add(term);
            }

            return terms;
        }

        public CST.Symbol ParseTerm(ParserState state)
        {
            switch (state.Peek().Type)
            {
                case TokenType.Date:
                case TokenType.Number:
                case TokenType.String:
                case TokenType.Variable:
                    return new CST.Symbol(state.Take());

                default:
                    return null;
            }
        }

    }
}

/*
grammar Tabula-Grammar {
    rule TOP { <Scenario> }
    # TODO: rule Module { [Scenario || Library] + }

    token ws { \h* }
    rule  Comment { '//' \N* }

    rule  Scenario    { [:i scenario] ':' <String> \n <Section>* }
    token Section     { <Break-Line> || <Document> || <Table> || <Paragraph> }
    token Break-Line  { ^^ <.Comment>? \n }
    rule  Document    { 'start_doc' <String>? \n .*? \n 'end_doc' }  #  crappy first draft placeholder

    rule Table        { <Table-Label>? <Table-Header> <Table-Row>* }
    rule Table-Label  { '===' <Phrase> '===' \n }
    rule Table-Header { '[' <Table-Cells> ']' \n }
    rule Table-Row    { :my $*Cells-Context = 'Row'; '|' <Table-Cells> \n }
    rule Table-Cells  { <Table-Cell>+ % '|' }
    rule Table-Cell   { :my $*Phrase-Context = 'Cell'; <Phrases> || <Empty-Cell> }
    token Empty-Cell  { \h* }

    rule  Paragraph   { <Paragraph-Label>? <.Para-Open> <Statement>+ <.Para-Close> }
    token Para-Open   { <?> }
    token Para-Close  { <?> }
    rule  Paragraph-Label { <String> ':' \n }
    rule  Statement   { <Action> <Comment>? \n }
    rule  Action      { <Block> || <Step> || <Command> }
    rule  Executable  { <Block> || <Step> }

    rule Block { <String>? <Block-Begin> \n <Section>+ <Block-End> }
    token Block-Begin { '...' }
    token Block-End   { '.' }

    token Step        { $<OptQ> = '? '? <Step-Phrase> }
    token Step-Phrase { <Step-Symbol>+ % \h+ }
    token Step-Symbol { <Word> || <Terms> }
    token Terms       { <Term>+ % [',' \h*] }
        # could further constrain so that each set of terms were to be same type of Term, but payoff is dubious...

    rule Command {
        '>'
            [<Command-Alias> || <Command-Set> || <Command-Tag> || <Command-Use>
            || <rule-failure('Command', "'alias', 'set', 'tag', or 'use'")> ]
    }
    rule Command-Alias {
        alias ':' [
            [   || <String>
                || <rule-failure('>alias: command', 'a quoted string for the alias name')>
            ]
            [   || means || is || to
                || <rule-failure('>alias: command', "'means', 'is', or 'to' separating the name from its value")>
            ]
            [   || <Executable;>
                || <rule-failure('>alias: command', 'either a single step or a block for the alias value')>
            ]
        ]
    }
    rule Command-Set {
        set ':' [
            [   || <Word> || <Variable>
                || <rule-failure('>set: command', 'an unquoted word or a variable')>
            ]
            [   || means || is || to
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
*/
