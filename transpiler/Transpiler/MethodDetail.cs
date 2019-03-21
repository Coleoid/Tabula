using System;
using System.Collections.Generic;

namespace Tabula
{
    public class WorkflowDetail
    {
        // key = SearchName
        private Dictionary<string, MethodDetail> Methods { get; set; }

        public MethodDetail GetMethodDetail(string searchName)
        {
            MethodDetail method = null;
            
            if (Methods.ContainsKey(searchName))
                method = Methods[searchName];

            if (method == null)
            {
                if (Parent != null)
                    method = Parent.GetMethodDetail(searchName);
            }

            return method;
        }

        public WorkflowDetail Parent { get; set; }
        public string Name { get; set; }
        public string InstanceName { get; set; }
        public string Namespace { get; set; }
        public WorkflowDetail()
        {
            Methods = new Dictionary<string, MethodDetail>();
        }

        public void AddMethod(MethodDetail methodDetail)
        {
            Methods[methodDetail.SearchName] = methodDetail;
        }
    }

    public class MethodDetail
    {
        private string _name;
        public string Name {
            get => _name;
            set
            {
                _name = value;
                _searchName = Formatter.SearchName_from_MethodName(value);
            }
        }

        private string _searchName;
        //FUTURE:  Suffix with arity, probably like "/2".  No current need.
        public string SearchName
        {
            get => _searchName;
        }  //  lower case, no underscores

        //public List<string> Args { get; set; }
        //TODO: rename to Params
        public List<ArgDetail> Args { get; set; }

        public MethodDetail()
        {
            Args = new List<ArgDetail>();
        }
    }

    public class ArgDetail
    {
        public string Name { get; set; }
        public Type Type { get; set; }
    }
}
