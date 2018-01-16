using NUnit.Framework;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Tabula
{
    [TestFixture]
    public class WorkflowIntrospectorTests
    {
        [Test]
        public void Implementations_from_methods()
        {
            var introspector = new WorkflowIntrospector();

            var iis = introspector.GetImplementationInfoForType(this.GetType());

            Assert.That(iis.Exists(ii => ii.MethodName == "Implementations_from_methods"));
        }

        [Test]
        public void can_find_CommentsModalWorkflow()
        {
            var introspector = new WorkflowIntrospector();

            List<Type> types = introspector.GetLoadedTypes();

            Assert.That(types.Exists(t => t.Name == "CommentsModalWorkflow"));
        }

        [Test]
        public void can_find_methods_of_CommentsModalWorkflow()
        {
            var introspector = new WorkflowIntrospector();
            List<Type> types = introspector.GetLoadedTypes();
            var myComments = types.Single(t => t.Name == "CommentsModalWorkflow");

            var iis = introspector.GetImplementationInfoForType(myComments);

            Assert.That(iis.Exists(i => i.MethodName == "Add_comment__"));
        }

        [Test]
        public void can_find_arguments_of_method()
        {
            var introspector = new WorkflowIntrospector();
            List<Type> types = introspector.GetLoadedTypes();
            var myComments = types.Single(t => t.Name == "CommentsModalWorkflow");

            var iis = introspector.GetImplementationInfoForType(myComments);
            var ii = iis.Single(i => i.MethodName == "Verify_comment__text_is__");

            Assert.That(ii.Arguments.Count(), Is.EqualTo(2));
            Assert.That(ii.Arguments[0], Is.EqualTo("rowNum"));
            Assert.That(ii.Arguments[1], Is.EqualTo("text"));
        }

    }
}
