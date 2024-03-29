﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Tabula.CST;

namespace Tabula.Parse
{
    public class TokenizerOutput
    {
        public List<Token> Tokens { get; set; }
        public List<Token> UnrecognizedTokens { get; set; }

        public TokenizerOutput(List<Token> startingTokens)
        {
            Tokens = startingTokens;
            UnrecognizedTokens = new List<Token>();
        }

        public TokenizerOutput()
        {
            Tokens = new List<Token>();
            UnrecognizedTokens = new List<Token>();
        }
    }

    public class Tokenizer
    {
        public List<string> Warnings { get; set; }
        public TokenizerOutput Output;
        public int position;
        public int line;
        public int column;

        //  Token regexes need to be start-anchored, or they will skip input.
        Regex rxHWS             = new Regex(@"^([\t ]*)");
        Regex rxNewLine         = new Regex(@"^\r?\n");
        Regex rxScenarioLabel   = new Regex(@"^Scenario: *([""']?)(.*)\1", RegexOptions.IgnoreCase);
        Regex rxWord            = new Regex(@"^([a-zA-Z_]\w*)");  //  first character = letter or underscore, then any word characters
        Regex rxStringDQ        = new Regex(@"^""((?:\\.|[^\\""\n])*)""");  //  double quotes with backslash escaping
        Regex rxStringSQ        = new Regex(@"^'((?:(?:'')|[^'\n])*)'(?!')");  //  single quotes--escape single quotes via two in a row
        Regex rxStringML        = new Regex(@"^`((?:(?:``)|[^`])*)`(?!`)");  //  backticks--multi-line string, escape backticks via two in a row
        Regex rxCommandUse      = new Regex(@"^use: *([^\n]*)");  //  use: Global Setting Management
        Regex rxCommandSet      = new Regex(@"^set: (.+) =>");    //  set: bob => "Nordberg, Robert" (run time)
        Regex rxCommandAlias    = new Regex(@"^alias: (.+) =>");  //  alias: "prep #name" => prepare user #name for class (build time)
        Regex rxTag             = new Regex(@"^\[([^]]+)\]");
        Regex rxNumber          = new Regex(@"^(-?(?:\d+(?:\.\d*)?)|-?\.\d+)");
        Regex rxDate            = new Regex(@"^(\d\d?/\d\d?/\d\d\d?\d?)");
        Regex rxTableMarker     = new Regex(@"^\|");
        Regex rxSectionLabel    = new Regex(@"^([""'])(.*)\1:");
        Regex rxVariable        = new Regex(@"^#([a-zA-Z_]\w*)");
        Regex rxBlockStart      = new Regex(@"^\.\.\.");
        Regex rxBlockEnd        = new Regex(@"^\.");
        Regex rxComma           = new Regex(@"^,");
        Regex rxUnrecognized    = new Regex(@"^\S+");

        public Tokenizer()
        {
            Warnings = new List<string>();
            Output = new TokenizerOutput();
            position = 0;
            line = 1;
            column = 1;
        }

        public void Advance(int length)
        {
            position += length;
            column += length;
        }

        public void AdvanceLine()
        {
            line++;
            column = 1;
        }

        public Token AddToken(TokenType type, string text, int length)
        {
            return NewToken(type, text, length, Output.Tokens);
        }

        private Token NewToken(TokenType type, string text, int length, List<Token> collection)
        {
            Token token = new Token(type, text, line) { Column = column, StartPosition = position, FullLength = length };
            Advance(length);
            collection.Add(token);
            return token;
        }

        public Token AddUnrecognizedToken(string text, int length)
        {
            return NewToken(TokenType.Unrecognized, text, length, Output.UnrecognizedTokens);
        }

        public TokenizerOutput Tokenize(string inputText)
        {
            string remainingText = inputText.Substring(position);

            while (position < inputText.Length)
            {
                remainingText = inputText.Substring(position);

                //  Leading (horizontal) whitespace is skipped past
                var match = rxHWS.Match(remainingText);
                Advance(match.Length);
                remainingText = inputText.Substring(position);
                //FUTURE:  comments will also advance without adding a token.

                //  Newlines complete commands, so they are recorded as tokens.
                match = rxNewLine.Match(remainingText);
                if (match.Success)
                {
                    AddToken(TokenType.NewLine, "\n", match.Length);
                    AdvanceLine();
                    continue;
                }


                //  Then, a series of patterns try to match at the cursor position.
                //  On a match, we create the token, advance the cursor, and start again.
                //  Some parse ambiguities are resolved by the order of the matches,
                //  providing a kind of poor man's Longest Token Matching.
                //  If you want theoretical purity, there are a lot of miserable
                //  experiences out there waiting for you.  Look up "parser generator".

                //  ScenarioLabel before Word to allow 'Scenario: mumph blumb foo'
                match = rxScenarioLabel.Match(remainingText);
                if (match.Success)
                {
                    AddToken(TokenType.ScenarioLabel, match.Groups[2].Value, match.Length);
                    continue;
                }

                //  SectionLabel before String, to allow '"set up people":'
                match = rxSectionLabel.Match(remainingText);
                if (match.Success)
                {
                    AddToken(TokenType.SectionLabel, match.Groups[2].Value, match.Length);
                    continue;
                }

                //  Commands before Word, to allow keywords: 'set: bob => "Nordberg, Robert"'
                match = rxCommandSet.Match(remainingText);
                if (match.Success)
                {
                    AddToken(TokenType.cmd_Set, match.Groups[1].Value, match.Length);
                    continue;
                }

                match = rxCommandAlias.Match(remainingText);
                if (match.Success)
                {
                    AddToken(TokenType.cmd_Alias, match.Groups[1].Value, match.Length);
                    continue;
                }

                match = rxCommandUse.Match(remainingText);
                if (match.Success)
                {
                    AddToken(TokenType.cmd_Use, match.Groups[1].Value, match.Length);
                    continue;
                }

                match = rxVariable.Match(remainingText);
                if (match.Success)
                {
                    AddToken(TokenType.Variable, match.Groups[1].Value.ToLower(), match.Length);
                    continue;
                }

                match = rxWord.Match(remainingText);
                if (match.Success)
                {
                    AddToken(TokenType.Word, match.Groups[1].Value, match.Length);
                    continue;
                }

                match = rxStringDQ.Match(remainingText);
                if (match.Success)
                {
                    var rawText = match.Groups[1].Value;

                    var cleanText = rawText
                            .Replace("\\\"", "\"")
                            .Replace("\\'", "'")
                            .Replace("\\\\", "\\")
                            .Replace("\\n", "\n")
                            .Replace("\\r","\r")
                            .Replace("\\t","\t")
                            ;

                    AddToken(TokenType.String, cleanText, match.Length);
                    continue;
                }

                match = rxStringSQ.Match(remainingText);
                if (match.Success)
                {
                    var rawText = match.Groups[1].Value;

                    var cleanText = rawText
                            .Replace("''", "'")
                        ;

                    AddToken(TokenType.String, cleanText, match.Length);
                    continue;
                }

                match = rxStringML.Match(remainingText);
                if (match.Success)
                {
                    var rawText = match.Groups[1].Value;

                    var cleanText = rawText
                            .Replace("``", "`")
                        ;

                    AddToken(TokenType.String, cleanText, match.Length);
                    continue;
                }


                //FIXME:  Not updated to catch token position/length yet
                //  multiple matches with variable whitespace make the numbers tricky.
                match = rxTag.Match(remainingText);
                if (match.Success)
                {
                    var insideBrackets = match.Groups[1].Value;
                    var tags = Regex.Split(insideBrackets, ", *");

                    var token = AddToken(TokenType.Tag, match.Groups[1].Value, match.Length);
                    token.Parts = tags.ToList();
                    continue;
                }

                //  Date before Number, to allow '11/22/1984'
                match = rxDate.Match(remainingText);
                if (match.Success)
                {
                    AddToken(TokenType.Date, match.Groups[1].Value, match.Length);
                    continue;
                }

                //  Number before BlockEnd, to allow '.25'
                match = rxNumber.Match(remainingText);
                if (match.Success)
                {
                    AddToken(TokenType.Number, match.Groups[1].Value, match.Length);
                    continue;
                }

                //  BlockStart before BlockEnd, to allow '...'
                match = rxBlockStart.Match(remainingText);
                if (match.Success)
                {
                    AddToken(TokenType.BlockStart, "...", 3);
                    continue;
                }

                match = rxBlockEnd.Match(remainingText);
                if (match.Success)
                {
                    AddToken(TokenType.BlockEnd, ".", 1);
                    continue;
                }

                match = rxTableMarker.Match(remainingText);
                if (match.Success)
                {
                    AddToken(TokenType.TableCellSeparator, "|", 1);
                    continue;
                }

                match = rxComma.Match(remainingText);
                if (match.Success)
                {
                    AddToken(TokenType.Comma, ",", 1);
                    continue;
                }


                // Catch an unrocognized chunk, add it to the unrecognized list, move forward.
                match = rxUnrecognized.Match(remainingText);
                if (match.Success)
                {
                    int snippetLength = Math.Min(20, remainingText.Length);
                    Warnings.Add($"Text not tokenizable on line {line} column {column}, at:\n{remainingText.Substring(0, snippetLength)}\n");

                    AddUnrecognizedToken(match.Groups[0].Value, match.Length);
                    continue;
                }
            }

            return Output;
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
