using System;
using System.Collections.Generic;
using System.IO;
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

        public List<ImplementationInfo> GetImplementationInfoForType(Type type)
        {
            List<ImplementationInfo> iis = new List<ImplementationInfo>();
            var mis = type.GetMethods();

            foreach (var mi in mis)
            {
                var ii = new ImplementationInfo { MethodName = mi.Name, ObjectName = "foo", Arguments = new List<string>() };
                foreach (ParameterInfo p in mi.GetParameters())
                {
                    ii.Arguments.Add(p.Name);
                }
                

                iis.Add(ii);
            }

            return iis;
        }
    
        public Assembly resolveAssembly(object sender, ResolveEventArgs args)
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
