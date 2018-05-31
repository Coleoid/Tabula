using System.Collections.Generic;

namespace Tabula.CST
{
    /// <summary> A call across to a glue 'implementation' class (or an invocation of an alias?) </summary>
    public class Step : Action, ITaggable
    {
        public List<Symbol> Symbols { get; set; }

        public Step(List<Symbol> symbols)
        {
            Symbols = symbols;
        }

        public Step(int line, params (TokenType Word, string)[] syms)
        {
            Symbols = new List<Symbol>();
            foreach(var (type, text) in syms)
            {
                Symbols.Add(new Symbol(type, text, line));
            }
            StartLine = line;
        }

        public string GetMethodSearchName()
        {
            string name = string.Empty;

            foreach(var symbol in Symbols)
            {
                if (symbol.Type == TokenType.Word)
                {
                    name += symbol.Text.ToLower();
                }
            }

            return name;
        }

        //TODO:  Keep watch on string representations, eventual rework
        public string GetReadableText()
        {
            var stepText = "";
            string delim = "";
            foreach (var sym in Symbols)
            {
                if (sym.Type == TokenType.String)
                {
                    stepText += delim + "\"" + sym.Text + "\"";
                }
                else if (sym.Type == TokenType.Variable)
                {
                    stepText += delim + "#" + sym.Text;
                }
                else
                {
                    stepText += delim + sym.Text;
                }

                delim = " ";
            }
            return stepText;

        }

        //TODO:  Keep watch on string representations, eventual rework
        public object GetReadableString()
        {
            var readableText = GetReadableText();

            string readableString = readableText.Replace("\"", "\"\"");
            readableString = "@\"" + readableString + "\"";

            return readableString;
        }
    }
}
