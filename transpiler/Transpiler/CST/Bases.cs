using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tabula.CST
{
    public interface ITaggable
    {
        List<string> Tags { get; }
    }

    public class CST_Entity : ITaggable
    {
        public List<string> Tags { get; set; }
        public string Label { get; set; }

        public CST_Entity()
        {
            Tags = new List<string>();
        }
    }

    public class Wrapper
    {
        private Token token;

        public Wrapper(Token token)
        {
            this.token = token;
        }

        public TokenType Type { get => token.Type; }
        public string Text { get => token.Text; }
        public int LineNumber { get => token.Line; }

    }

    public class Action : ITaggable
    {
        public List<string> Tags { get; set; }
        public int StartLine { get; set; }
        public int EndLine { get; set; }
    }

    public class Section : CST_Entity
    {
    }
}
