using System.Collections.Generic;

namespace Tabula.CST
{
    /// <summary> An entire test file. </summary>
    public class Scenario : CST_Entity
    {
        public string FileName { get; set; }
        public List<Section> Sections { get; set; }
        public List<string> SeenWorkflowRequests { get; set; }

        public Scenario()
            : base()
        {
            Sections = new List<Section>();
            SeenWorkflowRequests = new List<string>();
        }
    }

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
}
