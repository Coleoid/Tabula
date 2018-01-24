using System;
using System.Collections.Generic;

namespace Tabula.CST
{
    public class Scenario: CST_Entity
    {
        public List<Section> Sections { get; set; }
        public List<string> SeenWorkflowRequests { get; set; }

        public Scenario()
            : base()
        {
            Sections = new List<Section>();
            SeenWorkflowRequests = new List<string>();
        }
    }
}
