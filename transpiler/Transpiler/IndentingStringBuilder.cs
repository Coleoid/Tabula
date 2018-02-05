﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Tabula
{
    /// <summary>To factor out indenting each new line.  Supporting readable code at assembly and output.</summary>
    public class IndentingStringBuilder
    {
        private bool StartOfLine = true;

        private int _indentWidth;
        private int _indentSpaceCount;
        public int IndentSpaceCount
        {
            get => _indentSpaceCount;
            set
            {
                if (value < 0)
                    throw new Exception($"Negative indentation ({value}, in this case) is {NegativeBadness[Generator.CurrentVersion]}");
                _indentSpaceCount = value;
                _indentationString = new string(' ', _indentSpaceCount);
            }
        }
        /// <summary> A joke that I probably won't find funny one commit later. </summary>
        private Dictionary<string, string> NegativeBadness = new Dictionary<string, string>
        {
            { "0.1", "a notion of sublime subtlety.  I swoon!" },
            { "0.2", "an idea whose time has come--but not here, and not to you." },
            { "0.3", "not gonna happen on my watch, buddy." },
            { "0.4", "too tricky for a plain-spoken StringBuilder from humble beginnings." },
        };

        private string _indentationString = string.Empty;
        public string IndentationString
        {
            get => _indentationString;
        }

        private StringBuilder _builder;
        /// <summary>
        /// The underlying working StringBuilder.
        /// Normally, avoid.  When the simple API doesn't suffice, use.
        /// </summary>
        public StringBuilder Builder
        {
            get => _builder;
        }

        public IndentingStringBuilder(int startingIndentLevel = 0, int indentWidth = 4)
        {
            IndentSpaceCount = startingIndentLevel;
            _indentWidth = indentWidth;
            _builder = new StringBuilder();
        }

        public void Indent()
        {
            IndentSpaceCount += _indentWidth;
        }
        public void Dedent()
        {
            IndentSpaceCount -= _indentWidth;
        }

        private void IndentAtStartOfLine()
        {
            if (!StartOfLine) return;

            Builder.Append(IndentationString);
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
            Builder.AppendLine();
            StartOfLine = true;
        }

        public override string ToString()
        {
            return Builder.ToString();
        }
    }
}
