using NUnit.Framework;
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
    }
}
