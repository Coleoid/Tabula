using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Tabula
{
    public class Parser
    {

        public CST.Scenario FileParse(string scenarioText)
        {
            var tokenizer = new Tokenizer();
            var tokens = tokenizer.Tokenize(scenarioText);
            var state = new ParserState(tokens);
            var scenario = ParseScenario(state);

            return scenario;
        }

        public List<string> ParseCommand_Use(ParserState state)
        {
            var workflows = new List<string>();
            while(state.NextIs(TokenType.UseCommand))
                workflows.AddRange(Regex.Split(state.Take().Text, ", *"));

            return workflows;
        }

        public CST.Paragraph ParseParagraph(ParserState state)
        {
            int rollbackPosition = state.Position;
            CST.Paragraph paragraph = new CST.Paragraph();

            paragraph.Workflows = ParseCommand_Use(state);
            paragraph.Steps = ParseSteps(state);

            if (paragraph.Workflows.Count + paragraph.Steps.Count == 0)
            {
                state.Position = rollbackPosition;
                return null;
            }

            return paragraph;
        }

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

        public List<CST.Section> ParseSections(ParserState state)
        {
            var sections = new List<CST.Section>();

            for (var section = ParseSection(state); section != null; section = ParseSection(state))
                sections.Add(section);

            return sections;
        }

        public CST.Section ParseSection(ParserState state)
        {
            int rollback = state.Position;
            CST.Section section;

            var tags = ParseTags(state);

            var label = ParseSectionLabel(state);
            if (label == null)
            {
                state.Position = rollback;
                return null;
            }

            section = ParseParagraph(state);

            // if (section == null)
            //     section = ParseTable(state);

            if (section == null)
            {
                state.Position = rollback;
                return null;
            }

            section.Label = label.Text;
            section.Tags = tags;

            return section;
        }

        public CST.Label ParseSectionLabel(ParserState state)
        {
            return CST.Label.Wrap(state.Take(TokenType.SectionLabel));
        }

        public CST.Step ParseStep(ParserState state)
        {
            var symbols = ParseSymbols(state);
            if (symbols.Count == 0)
                return null;

            state.Take(TokenType.NewLine);

            return new CST.Step(symbols);
        }

        public List<CST.Step> ParseSteps(ParserState state)
        {
            var steps = new List<CST.Step>();
            for (var step = ParseStep(state); step != null; step = ParseStep(state))
                steps.Add(step);

            return steps;
        }

        public CST.Symbol ParseSymbol(ParserState state)
        {
            CST.Symbol symbol = ParseWord(state);
            if (symbol == null) symbol = ParseTerm(state);

            return symbol;
        }

        public List<CST.Symbol> ParseSymbols(ParserState state)
        {
            var symbols = new List<CST.Symbol>();
            for (var symbol = ParseSymbol(state); symbol != null; symbol = ParseSymbol(state))
                symbols.Add(symbol);

            return symbols;
        }

        public CST.Table ParseTable(ParserState state)
        {
            int rollbackPosition = state.Position;
            CST.Table table = new CST.Table();

            // covered by Section at this time, I believe.
            ////  Optional Label
            //table.Label = state.Take(TokenType.SectionLabel)?.Text;

            //  After the 
            if (!state.NextIs(TokenType.TableCellSeparator))
            {
                state.Position = rollbackPosition;
                return null;
            }
            table.ColumnNames = ParseTableRow(state);

            //  Zero or more data rows
            table.Rows = ParseTableRows(state);

            return table;
        }

        public List<string> ParseTableRow(ParserState state)
        {
            if (!state.NextIs(TokenType.TableCellSeparator)) return null;

            var row = new List<string>();

            state.Take(TokenType.TableCellSeparator);

            while (!state.AtEnd && !state.NextIs(TokenType.NewLine))
            {
                var syms = ParseSymbols(state);
                var text = string.Join(" ", syms.Select(s => s.Text).ToArray());
                row.Add(text);

                state.Take(TokenType.TableCellSeparator, "Gotta be | next but isn't.");
            }
            state.Take(TokenType.NewLine);

            return row;
        }

        public List<List<string>> ParseTableRows(ParserState state)
        {
            var rows = new List<List<string>>();

            //while (state.NextIs(TokenType.TableRow))
            //{
            //    rows.Add(state.Take().Parts);
            //}

            return rows;
        }

        public List<string> ParseTags(ParserState state)
        {
            var tags = new List<string>();

            while (state.NextIs(TokenType.Tag))
            {
                tags.Add(state.Take().Text);
            }

            return tags;
        }

        public CST.Symbol ParseTerm(ParserState state)
        {
            var token = state.Take(
                TokenType.Date,
                TokenType.Number,
                TokenType.String,
                TokenType.Variable
            );

            return CST.Symbol.Wrap(token);
        }

        public List<CST.Symbol> ParseTerms(ParserState state)
        {
            var terms = new List<CST.Symbol>();
            for (var term = ParseTerm(state); term != null; term = ParseTerm(state))
                terms.Add(term);

            return terms;
        }

        public CST.Symbol ParseWord(ParserState state)
        {
            return CST.Symbol.Wrap(state.Take(TokenType.Word));
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
        '>' [ <Command-Alias> || <Command-Set> || <Command-Tag> || <Command-Use> ]
    }
    rule Command-Alias {
        alias ':' <String> [ means || is || to ] <Executable;>
    }
    rule Command-Set {
        set ':' 
            [ <Word> || <Variable> ]
            [ means || is || to ]
            <Term>
    }
    rule Command-Tag { tags? ':' <Phrases> }
    rule Command-Use { use ':' <Phrases> }


    rule  Phrases { <Phrase>+ % ',' }
    rule  Phrase  { <Symbol>+ % \h+ }
    token Symbol  { <Word> || <Term> }
    token Word    { [<:Letter> || <[ _ \' \- ]> ] [\w || <[ _ \' \- ]>] * }
    token Term    { [ <Date> || <Number> || <String> || <Variable> ] }

    token Number   { <[+-]>? [[\d+ ['.' \d*]?] || ['.' \d+]] }
    token String   { '"' $<Body> = [ <-["]>* ] '"' }  # TODO: single quotes, quote escaping
    token Variable { '#' <Word> }
    token Date     { \d\d? '/' \d\d? '/' \d\d\d\d }
}
*/
