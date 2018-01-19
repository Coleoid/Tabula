﻿using System;
using System.Collections.Generic;

namespace Tabula
{
    public class WorkflowDetail
    {
        // key = SearchName
        public Dictionary<string, MethodDetail> Methods { get; set; }

        public WorkflowDetail Parent { get; set; }
        public string Name { get; set; }
        public string InstanceName { get; set; }

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
                _searchName = Formatter.MethodName_to_SearchName(value);
            }
        }

        private string _searchName;
        //FUTURE:  Suffix with arity, probably "/2"
        public string SearchName
        {
            get => _searchName;
        }  //  lower case, no underscores

        public List<string> Args { get; set; }

        public MethodDetail()
        {
            Args = new List<string>();
        }
    }
}