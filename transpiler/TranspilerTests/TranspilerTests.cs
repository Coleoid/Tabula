using NUnit.Framework;
using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Tabula
{
    [TestFixture]
    public class TranspilerTests
    {
        StringBuilder builder;
        Transpiler transpiler;

        [SetUp]
        public void SetUp()
        {
            builder = new StringBuilder();
            transpiler = new Transpiler();
        }

        [Test]
        public void Header_contains_generated_file_warning()
        {
            transpiler.BuildHeader(builder);

            var result = builder.ToString();
            var lines = Regex.Split(result, Environment.NewLine);

            Assert.That(lines.Count(), Is.GreaterThan(2));
            Assert.That(lines[0], Does.Contain("generated"));
            Assert.That(lines[0], Does.Contain("TabulaClassGenerator"));
        }

        //TODO:  Think.  Do I want to make truly empty scenarios work?
        //[Test]
        //public void Empty_scenario_contains_header()
        //{
        //    var headerBuilder = new StringBuilder();

        //    transpiler.BuildHeader(headerBuilder);
        //    transpiler.Transpile("foo.tab", string.Empty, builder);

        //    var headerText = headerBuilder.ToString();
        //    var emptyScenarioText = builder.ToString();

        //    Assert.That(emptyScenarioText, Does.Contain(headerText));
        //}

        //[Test]
        //public void Empty_scenario_creates_class_in_namespace()
        //{
        //    transpiler.Transpile("foo.tab", string.Empty, builder);

        //    var emptyScenarioText = builder.ToString();
        //    Assert.That(emptyScenarioText, Does.Contain("namespace Tabula"));
        //    Assert.That(emptyScenarioText, Does.Contain("public class"));
        //}

        [TestCase("my_scenario.tab", "my_scenario_generated")]
        [TestCase("another_scenario", "another_scenario_generated")]
        public void scenario_class_name_matches_file_name(string fileName, string expectedClassName)
        {
            transpiler.OpenClass(builder, fileName);

            var classText = builder.ToString();
            Assert.That(classText, Does.Contain($"public class {expectedClassName}"));
        }

        [Test]
        public void Class_declaration_includes_base_and_interface()
        {
            transpiler.OpenClass(builder, "TuitionBilling");

            var classText = builder.ToString();
            Assert.That(classText, Does.Contain($": GeneratedScenarioBase, IGeneratedScenario"));
        }

        [Test]
        public void Class_declaration_includes_scenario_label_as_comment()
        {
            var label = "AC-39393: Snock the cubid foratically";
            transpiler.ScenarioLabel = label;
            transpiler.OpenClass(builder, "TuitionBilling");

            var classText = builder.ToString();
            Assert.That(classText, Does.Contain($"  //  {label}"));
        }

        //[Test]
        //public void declarations_empty_to_start()
        //{
        //    transpiler.BuildDeclarations(builder);
        //    var declarations = builder.ToString();

        //    Assert.That(declarations, Is.EqualTo(string.Empty));
        //}

        //[Test]
        //public void one_declaration_per_needed_workflow()
        //{
        //    transpiler.AddWorkflow("ScenarioContext.Implementations.Administration.TaskRunnerWorkflow");
        //    transpiler.AddWorkflow("ScenarioContext.Implementations.Curriculum.AddEnrollmentWorkflow");

        //    transpiler.BuildDeclarations(builder);

        //    var declarations = builder.ToString();
        //    var lines = Regex.Split(declarations, Environment.NewLine);

        //    Assert.That(lines.Count(), Is.EqualTo(3));
        //    Assert.That(lines[0], Is.EqualTo("public ScenarioContext.Implementations.Administration.TaskRunnerWorkflow TaskRunner;"));
        //    Assert.That(lines[1], Is.EqualTo("public ScenarioContext.Implementations.Curriculum.AddEnrollmentWorkflow AddEnrollment;"));
        //    Assert.That(lines[2], Is.EqualTo(string.Empty));
        //}

        //[TestCase("ScenarioContext.Implementations.Administration.TaskRunnerWorkflow", "TaskRunner")]
        //[TestCase("ScenarioContext.Implementations.Curriculum.AddEnrollmentWorkflow", "AddEnrollment")]
        //public void instance_name_known_from_workflow(string workflowName, string expectedInstanceName)
        //{
        //    string instanceName = transpiler.nameOfWorkflowInstance(workflowName);

        //    Assert.That(instanceName, Is.EqualTo(expectedInstanceName));
        //}
    }
}
