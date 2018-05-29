using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Tabula
{
    public class WorkflowIntrospector
    {
        public Dictionary<Type, WorkflowDetail> CachedWorkflows { get; set; }

        //FUTURE:  Multiple types per search name
        public Dictionary<string, Type> TypeFromSearchName { get; set; }

        //FUTURE:  These elements need configurability, for users with distributed dependencies
        public string Location { get; set; }
        public string Library { get; set; }

        public WorkflowIntrospector()
        {
            CachedWorkflows = new Dictionary<Type, WorkflowDetail>();
            TypeFromSearchName = new Dictionary<string, Type>();
            Location = @"d:\code\acadis\ScenarioTests\ScenarioContext\bin\Debug\";
            Library = "ScenarioContext.dll";
        }

        public void DetailLoadedTypes()
        {
            foreach (var type in GetLoadedTypes())
            {
                GetWorkflowDetail(type);
            }
        }

        //FUTURE:  The decision clause will need to be user-overridable, for projects with
        //  different library/inheritance structures.
        public bool IsWorkflow(Type type)
        {
            if (type == null) return false;
            if (type.BaseType == null) return false;

            //  May need elaboration.  Quick check shows it at least 99% accurate.
            return type.BaseType.Name.Contains("Workflow") 
                || type.Name == "Workflow";
        }

        //  In the dll (or dlls) containing workflows, inherited methods should be found
        //  when they're in a pertinent parent class.
        //  WorkflowDetail.Parent is populated by GetWorkflowDetail recursively.
        //  For this reason, GetWorkflowDetail() returns null when asked for a non-workflow,
        //  and IsWorkflow() is extra bulletproof.
        public WorkflowDetail GetWorkflowDetail(Type type)
        {
            if (!IsWorkflow(type)) return null;

            if (CachedWorkflows.ContainsKey(type))
                return CachedWorkflows[type];

            string searchType = Formatter.SearchName_from_TypeName(type.Name);
            TypeFromSearchName[searchType] = type;

            var workflow = new WorkflowDetail
            {
                Name = type.Name,
                InstanceName = Formatter.InstanceName_from_TypeName(type.Name),
                Parent = GetWorkflowDetail(type.BaseType)
            };

            var myMethods = type.GetMethods().Where(mi => mi.DeclaringType == type);
            foreach (var mi in myMethods)
            {
                var method = new MethodDetail
                {
                    Name = mi.Name,
                    Args = mi.GetParameters().Select(i =>
                        new ArgDetail { Name = i.Name, Type = i.ParameterType }
                    ).ToList()
                };

                var searchMethod = Formatter.SearchName_from_MethodName(mi.Name);
                workflow.Methods[searchMethod] = method;
            }

            CachedWorkflows[type] = workflow;
            return workflow;
        }

        #region Reflective sausage-making

        //TODO:  Get location(s) from config and/or command line args
        private Assembly resolveAssembly(object sender, ResolveEventArgs args)
        {
            string libName = args.Name.Substring(0, args.Name.IndexOf(","));

            string libPath = Location + libName + ".dll";
            if (File.Exists(libPath))
                return Assembly.ReflectionOnlyLoadFrom(libPath);

            string exePath = Location + libName + ".exe";
            if (File.Exists(exePath))
                return Assembly.ReflectionOnlyLoadFrom(exePath);

            return Assembly.ReflectionOnlyLoad(args.Name);
        }


        //TODO:  Get workflow dll(s) from config and/or command line args
        public List<Type> GetLoadedTypes()
        {
            AppDomain curDomain = AppDomain.CurrentDomain;
            curDomain.ReflectionOnlyAssemblyResolve += resolveAssembly;

            Assembly asm = Assembly.ReflectionOnlyLoadFrom(Path.Combine(Location, Library));
            var asms = curDomain.GetAssemblies();

            return asm.ExportedTypes.ToList();
        }
        #endregion
    }
}
