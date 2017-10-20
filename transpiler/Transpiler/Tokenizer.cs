using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Tabula
{
    public enum TokenType
    {
        Unknown,
        Tag,
        ScenarioLabel,
        UseCommand,
        Date,
        Number,
        String,
        Variable,
        SectionLabel,
        Word,
        TableCellSeparator,
        NewLine
    }

    public class Token
    {
        public TokenType Type;
        public string Text;
        public List<string> Parts;

        public Token(TokenType type, string text)
        {
            this.Type = type;
            this.Text = text;
        }

        public Token(TokenType type, List<string> parts)
        {
            this.Type = type;
            this.Parts = parts;
        }
    }

    public class Tokenizer
    {
        public List<string> Warnings { get; set; }

        //  Token regexes need to be start-anchored
        Regex rxWS = new Regex(@"^([\t ]*)");
        Regex rxScenarioLabel = new Regex(@"^Scenario: *(.*)", RegexOptions.IgnoreCase);
        Regex rxWord = new Regex(@"^([a-zA-Z_]\w*)"); //    token Word    { [<:Letter> || <[ _ \' \- ]> ] [\w || <[ _ \' \- ]>] * }
        Regex rxString = new Regex(@"^'([^']*)'");
        Regex rxUseCommand = new Regex(@"^>use: ?([^\n]*)\n");  //>use: Global Setting Management
        Regex rxTag = new Regex(@"^\[([^]]+)\]");
        Regex rxNumber = new Regex(@"^(-?(?:\d+(?:\.\d*)?)|-?\.\d+)");
        Regex rxDate = new Regex(@"^(\d\d?/\d\d?/\d\d\d?\d?)");
        Regex rxTableCellSeparator = new Regex(@"^\|");
        Regex rxNewLine = new Regex(@"^\n");
        Regex rxSectionHeader = new Regex(@"^'([^']*)':[\t ]*\n");

        public Tokenizer()
        {
            Warnings = new List<string>();
        }

        public List<Token> Tokenize(string inputText)
        {
            var Tokens = new List<Token>();
            int position = 0;

            string remainingText = inputText.Substring(position);

            while (position < inputText.Length)
            {
                remainingText = inputText.Substring(position);

                //  First, nibble off leading whitespace
                var match = rxWS.Match(remainingText);
                position += match.Length;
                remainingText = inputText.Substring(position);


                //  Then, a series of patterns try to match at the cursor position.
                //  On a match, we create the token, advance the cursor, and start again.

                match = rxScenarioLabel.Match(remainingText);
                if (match.Success)
                {
                    Tokens.Add(new Token(TokenType.ScenarioLabel, match.Groups[1].Value));
                    position += match.Length;
                    continue;
                }

                match = rxSectionHeader.Match(remainingText);
                if (match.Success)
                {
                    Tokens.Add(new Token(TokenType.SectionLabel, match.Groups[1].Value));
                    position += match.Length;
                    continue;
                }

                match = rxWord.Match(remainingText);
                if (match.Success)
                {
                    Tokens.Add(new Token(TokenType.Word, match.Groups[1].Value));
                    position += match.Length;
                    continue;
                }

                match = rxString.Match(remainingText);
                if (match.Success)
                {
                    Tokens.Add(new Token(TokenType.String, match.Groups[1].Value));
                    position += match.Length;
                    continue;
                }

                match = rxTag.Match(remainingText);
                if (match.Success)
                {
                    var insideBrackets = match.Groups[1].Value;
                    var result = Regex.Split(insideBrackets, ", *");
                    foreach (var tag in result)
                    {
                        Tokens.Add(new Token(TokenType.Tag, tag));
                    }
                    position += match.Length;
                    continue;
                }

                match = rxDate.Match(remainingText);
                if (match.Success)
                {
                    Tokens.Add(new Token(TokenType.Date, match.Groups[1].Value));
                    position += match.Length;
                    continue;
                }

                match = rxNumber.Match(remainingText);
                if (match.Success)
                {
                    Tokens.Add(new Token(TokenType.Number, match.Groups[1].Value));
                    position += match.Length;
                    continue;
                }

                match = rxUseCommand.Match(remainingText);
                if (match.Success)
                {
                    Tokens.Add(new Token(TokenType.UseCommand, match.Groups[1].Value));
                    position += match.Length;
                    continue;
                }

                match = rxTableCellSeparator.Match(remainingText);
                if (match.Success)
                {
                    Tokens.Add(new Token(TokenType.TableCellSeparator, "|"));
                    position += match.Length;
                    continue;
                }

                match = rxNewLine.Match(remainingText);
                if (match.Success)
                {
                    Tokens.Add(new Token(TokenType.NewLine, "\n"));
                    position += match.Length;
                    continue;
                }

                //  Finally, if none of the token patterns matched at the cursor position, fail.
                //  Right now, that's simply 'advance to end of input', to ignore everything
                //  at and after the unrecognized input.
                Warnings.Add($"Unrecognized token at position {position}.");
                position = inputText.Length;
                //TODO: Warn better.  Line and column.

                //TODO: Fail softer.
                //  Make it consume one 'word' instead, and insert an "Unrecognized" token,
                //  as an attempt to recover from the problem.  Needs some mature problem reporting,
                //  first, or it will cause "hilarious" scenario-writer agony.
            }

            return Tokens;
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
