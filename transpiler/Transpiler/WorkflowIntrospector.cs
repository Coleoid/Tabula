using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Tabula
{
    public class WorkflowIntrospector
    {
        public Dictionary<string, WorkflowDetail> KnownWorkflows { get; set; }

        public WorkflowIntrospector()
        {
            KnownWorkflows = new Dictionary<string, WorkflowDetail>();
        }

        public WorkflowDetail GetWorkflowDetail(Type type)
        {
            if (type == null) return null;
            if (!type.Name.Contains("Workflow")) return null;
            if (KnownWorkflows.ContainsKey(type.Name))
                return KnownWorkflows[type.Name];

            var detail = new WorkflowDetail
            {
                Name = type.Name,
                InstanceName = Formatter.ClassName_to_InstanceName(type.Name),
                Parent = GetWorkflowDetail(type.BaseType)
            };

            var myMethods = type.GetMethods().Where(mi => mi.DeclaringType == type);
            foreach (var mi in myMethods)
            {
                var searchName = Formatter.MethodName_to_SearchName(mi.Name);
                var method = new MethodDetail
                {
                    Name = mi.Name,
                    Args = mi.GetParameters().Select(i => i.Name).ToList()
                };

                detail.Methods[searchName] = method;
            }

            KnownWorkflows[type.Name] = detail;
            return detail;
        }

        //TODO:  path(s) pulled from config
        //TODO:  correctly handle .exes as a separate branch, not special-cased
        private Assembly resolveAssembly(object sender, ResolveEventArgs args)
        {
            string dllName = args.Name.Substring(0, args.Name.IndexOf(","));

            string dllPath = @"k:\code\acadis_trunk\ScenarioTests\ScenarioContext\bin\Debug\" + dllName + ".dll";
            if (dllName == "Crypto")
                dllPath = @"k:\code\acadis_trunk\ScenarioTests\ScenarioContext\bin\Debug\crypto.exe";

            if (File.Exists(dllPath))
                return Assembly.ReflectionOnlyLoadFrom(dllPath);
            else
                return Assembly.ReflectionOnlyLoad(args.Name);
        }


        //TODO:  dll(s) containing workflow classes pulled from config
        public List<Type> GetLoadedTypes()
        {
            AppDomain curDomain = AppDomain.CurrentDomain;
            curDomain.ReflectionOnlyAssemblyResolve += resolveAssembly;

            Assembly asm = Assembly.ReflectionOnlyLoadFrom(@"k:\code\acadis_trunk\ScenarioTests\ScenarioContext\bin\Debug\ScenarioContext.dll");
            var asms = curDomain.GetAssemblies();

            return asm.ExportedTypes.ToList();
        }
    }
}
