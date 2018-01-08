using NUnit.Framework;
using System;
using System.Linq;
using System.Reflection;

namespace Tabula
{
    [TestFixture]
    public class WorkflowIntrospectorTests
    {
        [Test]
        public void Introspector_will_find_public_void_methods()
        {

            var introspector = new WorkflowIntrospector();
            MethodInfo method = introspector.GetMethodInfo();

            Assert.That(method.Name, Is.EqualTo("Snaff_the_Blonker"));
        }

        public Assembly resolveAssembly(object sender, ResolveEventArgs args)
        {
            return Assembly.ReflectionOnlyLoadFrom(@"k:\code\acadis_trunk\ScenarioTests\ScenarioContext\bin\Debug\TestopiaAPI.dll");
        }

        [Test]
        public void ReflectionOnly_can_reflect_on_scenarios_loaded_dynamically()
        {
            AppDomain curDomain = AppDomain.CurrentDomain;
            curDomain.ReflectionOnlyAssemblyResolve += resolveAssembly;

            //Assembly asm = Assembly.ReflectionOnlyLoadFrom(@"k:\code\acadis_trunk\ScenarioTests\ScenarioContext\bin\Debug\ScenarioContext.dll");
            var asms = curDomain.GetAssemblies();
            Assembly asm = asms.SingleOrDefault(a => a.FullName.StartsWith("System.Configuration"));


            foreach (var type in asm.ExportedTypes)
            {
                Console.WriteLine(type.Name);

                //foreach (var mi in type.GetMethods())
                //{
                //    Console.WriteLine(mi.Name);
                //}
            }

            //Type t = asm.GetType("Test");
            //MethodInfo m = t.GetMethod("TestMethod");
            //ParameterInfo[] p = m.GetParameters();

            //Console.WriteLine("\r\nAttributes for assembly: '{0}'", asm);
            //ShowAttributeData(CustomAttributeData.GetCustomAttributes(asm));
            //Console.WriteLine("\r\nAttributes for type: '{0}'", t);
            //ShowAttributeData(CustomAttributeData.GetCustomAttributes(t));
            //Console.WriteLine("\r\nAttributes for member: '{0}'", m);
            //ShowAttributeData(CustomAttributeData.GetCustomAttributes(m));
            //Console.WriteLine("\r\nAttributes for parameter: '{0}'", p);
            //ShowAttributeData(CustomAttributeData.GetCustomAttributes(p[0]));
        }

    }
}
