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
            Type myComments = types.Single(t => t.Name == "CommentsModalWorkflow");

            var detail = introspector.GetWorkflowDetail(myComments);
            var method = detail.Methods["verifycommenttextis"];

            Assert.That(method.Args.Count(), Is.EqualTo(2));
            Assert.That(method.Args[0], Is.EqualTo("rowNum"));
            Assert.That(method.Args[1], Is.EqualTo("text"));
        }

        [Test]
        public void GetWorkflowDetail_even_when_class_name_does_not_include_workflow()
        {
            var introspector = new WorkflowIntrospector();
            List<Type> types = introspector.GetLoadedTypes();

            Type listMgmt = types.Single(t => t.Name == "ListManagement");

            var detail = introspector.GetWorkflowDetail(listMgmt);

            Assert.That(detail, Is.Not.Null);
        }

        [TestCase("RuntimeContext")]
        [TestCase("WorkflowProperty")]
        [TestCase("MvcAction")]
        [TestCase("TuitionBilling_generated")]
        [TestCase("TextAttribute")]
        public void GetWorkflowDetail_does_not_grab_irrelevant_classes(string typeName)
        {
            var introspector = new WorkflowIntrospector();
            List<Type> types = introspector.GetLoadedTypes();

            var type = types.Single(t => t.Name == typeName);
            var detail = introspector.GetWorkflowDetail(type);

            Assert.That(detail, Is.Null);
        }

        [TestCase("DivisionManagement")]
        [TestCase("Workflow")]
        [TestCase("MvcBaseWorkflow")]
        public void GetWorkflowDetail_does_grab_relevant_classes(string typeName)
        {
            var introspector = new WorkflowIntrospector();
            List<Type> types = introspector.GetLoadedTypes();

            var type = types.Single(t => t.Name == typeName);
            var detail = introspector.GetWorkflowDetail(type);

            Assert.That(detail, Is.Not.Null);
        }

    }
}
