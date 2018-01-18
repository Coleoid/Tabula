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

            var detail = introspector.GetWorkflowDetail(this.GetType());
            var method = detail.Methods["implementationsfrommethods"];

            Assert.That(method.Name == "Implementations_from_methods");
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

            WorkflowDetail detail = introspector.GetWorkflowDetail(myComments);
            var method = detail.Methods["addcomment"];
            Assert.That(method.Name == "Add_comment__");
        }

        [Test]
        public void can_find_arguments_of_method()
        {
            var introspector = new WorkflowIntrospector();
            List<Type> types = introspector.GetLoadedTypes();
            var myComments = types.Single(t => t.Name == "CommentsModalWorkflow");

            var detail = introspector.GetWorkflowDetail(myComments);
            var method = detail.Methods["verifycommenttextis"];

            Assert.That(method.Args.Count(), Is.EqualTo(2));
            Assert.That(method.Args[0], Is.EqualTo("rowNum"));
            Assert.That(method.Args[1], Is.EqualTo("text"));
        }

    }
}
