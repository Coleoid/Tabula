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
            int rollback = state.Position;
            CST.Section section;

            var tags = ParseTags(state);

            var header = ParseSectionHeader(state);
            if (header == null)
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

            section.Label = header.Text;
            section.Tags = tags;

            return section;
        }

        public CST.Header ParseSectionHeader(ParserState state)
        {
            if (state.NextIs(TokenType.SectionHeader))
                return new CST.Header(state.Take());
            else
                return null;
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

        //public CST.Table ParseTable(ParserState state)
        //{
        //    int rollbackPosition = state.Position;
        //    CST.Table table = new CST.Table();

        //    //  Optional Label
        //    table.Label = state.Take(TokenType.TableLabel)?.Text;

        //    //  If we get no row of column names, we aren't looking at a table.  Rollback.
        //    if (!state.NextIs(TokenType.TableRow))
        //    {
        //        //  ...unless we already found a table label, which can only go on a table.  Abort.
        //        if (table.Label != null)
        //            throw new Exception("A table must have a row of column names");

        //        state.Position = rollbackPosition;
        //        return null;
        //    }
        //    table.ColumnNames = state.Take(TokenType.TableRow).Parts;

        //    //  Zero or more data rows
        //    table.Rows = ParseTableRows(state);

        //    return table;
        //}

        //public List<List<string>> ParseTableRows(ParserState state)
        //{
        //    var rows = new List<List<string>>();

        //    while (state.NextIs(TokenType.TableRow))
        //    {
        //        rows.Add(state.Take().Parts);
        //    }

        //    return rows;
        //}

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

            for (var step = ParseStep(state); step != null; step = ParseStep(state))
                steps.Add(step);

            return steps;
        }
        
        public CST.Step ParseStep(ParserState state)
        {
            var step = new CST.Step();
            
            var symbols = ParseSymbols(state);

            if (symbols.Count > 0)
            {
                step.Symbols = symbols;
                if (state.NextIs(TokenType.NewLine))
                    state.Take();
            }
            else
            {
                step = null;
            }
            return step;
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
            if (state.NextIs(TokenType.Word))
                return new CST.Symbol(state.Take());
            else
                return null;
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
            if (state.AtEnd) return null;

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
