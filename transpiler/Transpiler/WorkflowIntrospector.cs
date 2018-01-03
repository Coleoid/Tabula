using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Tabula
{

    public class WorkflowIntrospector
    {
        public MethodInfo GetMethodInfo()
        {
            var mis = typeof(WorkflowIntrospector).GetMethods();
            return mis.First(mi => mi.Name.StartsWith("Snaff"));
        }

        public void Snaff_the_Blonker()
        {
            throw new NotImplementedException("And won't be.");
        }
    }
}
