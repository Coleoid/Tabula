using System;
using System.Text;

namespace Tabula
{
    public class IndentingStringBuilder
    {
        private bool StartOfLine = true;
        private StringBuilder _builder;
        public StringBuilder Builder
        {
            get
            {
                if (_builder == null)
                    _builder = new StringBuilder();
                return _builder;
            }
        }

        internal int indentLevel = 0;

        public void Indent()
        {
            indentLevel += 4;
            _indentation = new string(' ', indentLevel);
        }
        public void Dedent()
        {
            if (indentLevel <= 0) throw new Exception("Tried to dedent past zero.");
            indentLevel -= 4;
            _indentation = new string(' ', indentLevel);
        }

        private string _indentation = string.Empty;
        public string Indentation
        {
            get { return _indentation; }
        }

        private void IndentAtStartOfLine()
        {
            if (!StartOfLine) return;

            Builder.Append(Indentation);
            StartOfLine = false;
        }

        public void Append(string input)
        {
            IndentAtStartOfLine();
            Builder.Append(input);
        }

        public void AppendLine(string input)
        {
            IndentAtStartOfLine();
            Builder.AppendLine(input);
            StartOfLine = true;
        }

        public void AppendLine()
        {
            //probably don't need:  IndentAtStartOfLine();
            Builder.AppendLine();
            StartOfLine = true;
        }

        public IndentingStringBuilder(int startingIndentLevel)
        {
            indentLevel = startingIndentLevel;
            _indentation = new string(' ', indentLevel);
        }
    }
}
