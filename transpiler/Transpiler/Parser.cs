using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Tabula
{
    public class Parser
    {
        public CST.Scenario Scenario { get; set; }

        public Parser()
        {
            Scenario = new CST.Scenario();
        }

        public CST.Action ParseAction(ParserState state)
        {
            state.AdvanceLines();
            if (state.AtEnd) return null;

            switch (state.Peek().Type)
            {
                case TokenType.cmd_Alias:
                    return ParseCommand_Alias(state);

                case TokenType.cmd_Set:
                    return ParseCommand_Set(state);

                case TokenType.cmd_Use:
                    return ParseCommand_Use(state);

                case TokenType.BlockStart:
                    return ParseBlock(state);

                default:
                    break;
            }

            return ParseStep(state);
        }

        public List<CST.Action> ParseActions(ParserState state)
        {
            var actions = new List<CST.Action>();
            while (true)
            {
                var action = ParseAction(state);
                if (action == null) break;
                actions.Add(action);
            }

            return actions;
        }

        public CST.Block ParseBlock(ParserState state)
        {
            if (!state.NextIs(TokenType.BlockStart))
                return null;
            var start = state.Take(TokenType.BlockStart);

            var actions = ParseActions(state);

            if (!state.NextIs(TokenType.BlockEnd))
                throw new Exception("After the actions in a block, we need a block end, a period.");
            var end = state.Take(TokenType.BlockEnd);

            return new CST.Block(actions) { StartLine = start.Line, EndLine = end.Line};
        }

        public CST.CommandAlias ParseCommand_Alias(ParserState state)
        {
            state.AdvanceLines();
            if (!state.NextIs(TokenType.cmd_Alias)) return null;

            var aliasToken = state.Take();
            var action = ParseAction(state);
            if (action == null)
                throw new Exception("The target of an Alias command must be a step or a block of steps.");

            var alias = new CST.CommandAlias(aliasToken.Text, action);
            alias.StartLine = action.StartLine;
            alias.EndLine = action.EndLine;
            return alias;
        }

        public CST.CommandSet ParseCommand_Set(ParserState state)
        {
            state.AdvanceLines();
            if (!state.NextIs(TokenType.cmd_Set)) return null;

            var setToken = state.Take();
            var term = ParseTerm(state);
            if (term == null)
                throw new Exception("The target of a Set command must be a term--a variable, string, date, or number.");

            var set = new CST.CommandSet(setToken.Text, term);
            return set;
        }

        public CST.CommandUse ParseCommand_Use(ParserState state)
        {
            state.AdvanceLines();
            var workflows = new List<string>();
            var token = state.Peek();

            while (state.NextIs(TokenType.cmd_Use))
            {
                workflows.AddRange(Regex.Split(state.Take().Text, ", *"));
                state.AdvanceLines();
            }
            Scenario.SeenWorkflowRequests.AddRange(workflows);
            return new CST.CommandUse(workflows) { StartLine = token.Line , EndLine = token.Line};
        }

        public CST.Paragraph ParseParagraph(ParserState state)
        {
            int rollbackPosition = state.Position;
            CST.Paragraph paragraph = new CST.Paragraph();

            paragraph.Actions = ParseActions(state);

            if (paragraph.Actions.Count == 0)
            {
                state.Position = rollbackPosition;
                return null;
            }

            string start = paragraph.Actions[0].StartLine.ToString("D3");
            string end = paragraph.Actions.Last().EndLine.ToString("D3");
            paragraph.MethodName = $"paragraph_from_{start}_to_{end}";

            return paragraph;
        }

        public CST.Scenario ParseScenario(ParserState state)
        {
            state.AdvanceLines();
            Scenario.Tags.AddRange(ParseTags(state));
            var token = state.Take(TokenType.ScenarioLabel,
                "Scenario should start with a label like  'Scenario: Student departure sends mail'");

            Scenario.Label = token.Text;
            Scenario.Sections.AddRange(ParseSections(state));

            return Scenario;
        }

        public List<CST.Section> ParseSections(ParserState state)
        {
            var sections = new List<CST.Section>();

            while (true)
            {
                var section = ParseSection(state);
                if (section == null) break;
                sections.Add(section);
            }

            return sections;
        }

        public CST.Section ParseSection(ParserState state)
        {
            int rollback = state.Position;
            CST.Section section;

            state.AdvanceLines();
            var tags = ParseTags(state);

            var label = ParseSectionLabel(state);

            section = ParseParagraph(state);

            if (section == null)
                section = ParseTable(state);

            if (section == null)
            {
                state.Position = rollback;
                return null;
            }

            section.Label = label?.Text;
            section.Tags = tags;

            return section;
        }

        public CST.Label ParseSectionLabel(ParserState state)
        {
            CST.Label label = CST.Label.Wrap(state.Take(TokenType.SectionLabel));
            state.AdvanceLines();
            return label;
        }

        public CST.Step ParseStep(ParserState state)
        {
            var symbols = ParseSymbols(state);
            if (symbols.Count == 0)
                return null;

            state.AdvanceLines();

            return new CST.Step(symbols) { StartLine = symbols[0].LineNumber, EndLine = symbols[0].LineNumber };
        }

        public List<CST.Step> ParseSteps(ParserState state)
        {
            var steps = new List<CST.Step>();

            while (true)
            {
                var step = ParseStep(state);
                if (step == null) break;
                steps.Add(step);
            }

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
            while (true)
            {
                var symbol = ParseSymbol(state);
                if (symbol == null) break;
                symbols.Add(symbol);
            }

            return symbols;
        }

        public CST.Table ParseTable(ParserState state)
        {
            var topRow = ParseTableRow(state);
            if (topRow == null) return null;
            var columnNames = topRow.Cells.Select(c => c[0]).ToList();
            if (columnNames == null) return null;

            var table = new CST.Table {
                ColumnNames = columnNames,
                Rows = ParseTableRows(state)
            };

            string start = table.Rows[0].StartLine.ToString("D3");
            string end = table.Rows.Last().EndLine.ToString("D3");
            table.MethodName = $"paragraph_from_{start}_to_{end}";

            return table;
        }

        public List<string> ParseTableCell(ParserState state)
        {
            var values = new List<string>();
            while (!state.NextIs(TokenType.TableCellSeparator))
            {
                var syms = ParseSymbols(state);
                var text = string.Join(" ", syms.Select(s => s.Text).ToArray());

                if (!state.NextIsIn(TokenType.TableCellSeparator, TokenType.Comma))
                    throw new Exception("Can only handle symbols and commas inside a table cell.");

                state.Take(TokenType.Comma);
                values.Add(text);
            }

            return values;
        }

        public CST.TableRow ParseTableRow(ParserState state)
        {
            if (!state.NextIs(TokenType.TableCellSeparator)) return null;

            state.Take(TokenType.TableCellSeparator);

            var cells = new List<List<string>>();
            while (!state.LineComplete)
            {
                var cell = ParseTableCell(state);
                cells.Add(cell);
                state.Take(TokenType.TableCellSeparator, "Gotta be | next but isn't.");
            }
            state.AdvanceLines();

            var row = new CST.TableRow(cells);
            return row;
        }

        public List<CST.TableRow> ParseTableRows(ParserState state)
        {
            var rows = new List<CST.TableRow>();
            while (state.NextIs(TokenType.TableCellSeparator))
            {
                var row = ParseTableRow(state);
                rows.Add(row);
            }

            return rows;
        }

        public List<string> ParseTags(ParserState state)
        {
            var tags = new List<string>();

            while (state.NextIs(TokenType.Tag))
            {
                var tag = state.Take();

                if (tag.Parts != null)
                    tags.AddRange(tag.Parts);
                else
                    tags.Add(tag.Text);

                state.AdvanceLines();
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
