﻿using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Tabula.CST;

namespace Tabula
{
    [TestFixture]
    public class Interpreter_ScenarioTests
    {
        private Interpreter interpreter = null;

        [SetUp]
        public void SetUp()
        {
            interpreter = new Interpreter();
        }

        [Test]
        public void Scenario_runs_paragraph_once()
        {
            Paragraph paragraph = ExampleParagraph(new Symbol(TokenType.Number, "8", 6));
            Scenario scenario = new Scenario();
            scenario.Sections.Add(paragraph);

            interpreter.UseWorkflow("GreetingWorkflow");

            NUnitReport.TestSuite result = interpreter.ExecuteScenario(scenario);

            Assert.That(result.Result, Is.EqualTo(NUnitTestResult.Passed));
            Assert.That(result.TestCaseCount, Is.EqualTo(0));
            Assert.That(result.TotalTests, Is.EqualTo(3));
            Assert.That(result.TestSuites.Count, Is.EqualTo(1));
            Assert.That(result.PassedTests, Is.EqualTo(3));
            Assert.That(result.FailedTests, Is.EqualTo(0));
            Assert.That(result.InconclusiveTests, Is.EqualTo(0));
            Assert.That(result.SkippedTests, Is.EqualTo(0));
        }


        [Test]
        public void SetCommand_sets_variable_in_outer_scope()
        {
            Scenario scenario = new Scenario();
            Paragraph paragraph = new Paragraph();

            paragraph.Actions.Add(
                new CommandSet("friend", new Symbol(TokenType.String, "Annelise"))
                );

            scenario.Sections.Add(paragraph);

            paragraph = new Paragraph();

            paragraph.Actions.Add(
                new CommandUse(new List<string> { "GreetingWorkflow" })
                );

            paragraph.Actions.Add(
                new Step(222,
                    (TokenType.Word, "my"),
                    (TokenType.Word, "friend"),
                    (TokenType.Variable, "friend"),
                    (TokenType.Word, "turned"),
                    (TokenType.Number, "38"),
                    (TokenType.Word, "on"),
                    (TokenType.Date, "11/22/2019")
                ));

            paragraph.Actions.Add(
                new Step(223,
                    (TokenType.Word, "Verify"),
                    (TokenType.Word, "that"),
                    (TokenType.Word, "my"),
                    (TokenType.Word, "friend"),
                    (TokenType.Word, "is"),
                    (TokenType.Word, "named"),
                    (TokenType.String, "Annelise")
                ));

            paragraph.Actions.Add(
                new Step(224,
                    (TokenType.Word, "Verify"),
                    (TokenType.Word, "that"),
                    (TokenType.Word, "my"),
                    (TokenType.Word, "friend"),
                    (TokenType.Word, "is"),
                    (TokenType.Word, "named"),
                    (TokenType.Variable, "friend")
                ));

            scenario.Sections.Add(paragraph);

            NUnitReport.TestSuite result = interpreter.ExecuteScenario(scenario);

            Assert.That(result.Result, Is.EqualTo(NUnitTestResult.Passed));
            Assert.That(result.TotalTests, Is.EqualTo(3));
        }


        [TestCase("GreetingWorkflow")]
        [TestCase("greetingworkflow")]
        [TestCase("Greeting Workflow")]
        [TestCase("some workflow")]
        public void UseCommand_Loads_workflow_class(string workflowName)
        {
            Paragraph paragraph = new Paragraph();
            paragraph.Actions.Add(
                new CommandUse(new List<string> { workflowName })
                );

            paragraph.Actions.Add(
                new Step(222,
                    (TokenType.Word, "hello"),
                    (TokenType.Word, "world")
                ));

            Scenario scenario = new Scenario();
            scenario.Sections.Add(paragraph);

            NUnitReport.TestSuite result = interpreter.ExecuteScenario(scenario);

            Assert.That(result.Result, Is.EqualTo(NUnitTestResult.Passed));
            Assert.That(result.TotalTests, Is.EqualTo(1));
        }

        [Test]
        public void UseCommand_Loads_multiple_workflow_classes()
        {
            Paragraph paragraph = new Paragraph();
            paragraph.Actions.Add(
                new CommandUse(new List<string> { "Greeting Workflow", "Some Workflow" })
                );

            //TODO: determine which workflow was called
            paragraph.Actions.Add(
                new Step(222,
                    (TokenType.Word, "hello"),
                    (TokenType.Word, "world")
                ));

            paragraph.Actions.Add(
                new Step(223,
                    (TokenType.Word, "hello"),
                    (TokenType.Word, "america")
                ));

            paragraph.Actions.Add(
                new Step(224,
                    (TokenType.Word, "fail"),
                    (TokenType.Word, "if"),
                    (TokenType.Number, "2"),
                    (TokenType.Word, "is"),
                    (TokenType.Word, "odd")
                ));

            Scenario scenario = new Scenario();
            scenario.Sections.Add(paragraph);

            NUnitReport.TestSuite result = interpreter.ExecuteScenario(scenario);

            Assert.That(result.Result, Is.EqualTo(NUnitTestResult.Passed));
            Assert.That(result.TotalTests, Is.EqualTo(3));
        }

        [Test]
        public void Multiple_UseCommands_Load_multiple_workflow_classes()
        {
            Paragraph paragraph = new Paragraph();
            paragraph.Actions.Add(
                new CommandUse(new List<string> { "Greeting Workflow" })
                );

            paragraph.Actions.Add(
                new CommandUse(new List<string> { "some Workflow" })
                );

            //TODO: determine which workflow was called
            paragraph.Actions.Add(
                new Step(222,
                    (TokenType.Word, "hello"),
                    (TokenType.Word, "world")
                ));

            paragraph.Actions.Add(
                new Step(223,
                    (TokenType.Word, "hello"),
                    (TokenType.Word, "america")
                ));

            paragraph.Actions.Add(
                new Step(224,
                    (TokenType.Word, "fail"),
                    (TokenType.Word, "if"),
                    (TokenType.Number, "2"),
                    (TokenType.Word, "is"),
                    (TokenType.Word, "odd")
                ));

            Scenario scenario = new Scenario();
            scenario.Sections.Add(paragraph);

            NUnitReport.TestSuite result = interpreter.ExecuteScenario(scenario);

            Assert.That(result.Result, Is.EqualTo(NUnitTestResult.Passed));
            Assert.That(result.TotalTests, Is.EqualTo(3));
        }

        [Test]
        public void Workflows_are_forgotten_between_Paragraphs()
        {
            Paragraph paragraph = new Paragraph();
            paragraph.Actions.Add(
                new CommandUse(new List<string> { "Greeting Workflow" })
                );

            Scenario scenario = new Scenario();
            scenario.Sections.Add(paragraph);

            Paragraph paragraph2 = new Paragraph();
            paragraph2.Actions.Add(
                new CommandUse(new List<string> { "some Workflow" })
                );

            paragraph2.Actions.Add(
                new Step(223,
                    (TokenType.Word, "hello"),
                    (TokenType.Word, "america")
                ));

            scenario.Sections.Add(paragraph2);

            NUnitReport.TestSuite result = interpreter.ExecuteScenario(scenario);

            Assert.That(result.Result, Is.EqualTo(NUnitTestResult.Failed));
            Assert.That(result.TotalTests, Is.EqualTo(1));
        }

        [Test]
        public void UseCommand_complains_clearly_when_not_finding_workflow_class()
        {
            Paragraph paragraph = new Paragraph();

            paragraph.Actions.Add(
                new CommandUse(new List<string> { "GripingWorkflow" }) { StartLine = 221 }
                );

            paragraph.Actions.Add(
                new Step(222,
                    (TokenType.Word, "go"),
                    (TokenType.Word, "away"),
                    (TokenType.Word, "world")
                ));

            Scenario scenario = new Scenario();
            scenario.Sections.Add(paragraph);

            NUnitReport.TestSuite result = interpreter.ExecuteScenario(scenario);

            Assert.That(result.Result, Is.EqualTo(NUnitTestResult.Failed));
            Assert.That(result.TotalTests, Is.EqualTo(2));
            Assert.That(result.TestSuites[0].TestCases[0].FailureInfo.Message,
                Is.EqualTo("Tried to use workflow \"GripingWorkflow\" on line 221, and did not find it."));
            Assert.That(result.TestSuites[0].TestCases[0].Name,
                Is.EqualTo("use: GripingWorkflow"));
        }

        [Test]
        public void UseCommand_loads_multiple_workflows()
        {
            Paragraph paragraph = new Paragraph();

            paragraph.Actions.Add(
                new CommandUse(new List<string> { "GreetingWorkflow", "SomeWorkflow" }) { StartLine = 221 }
                );

            paragraph.Actions.Add(
                new Step(222,
                    (TokenType.Word, "fail"),
                    (TokenType.Word, "if"),
                    (TokenType.Number, "4"),
                    (TokenType.Word, "is"),
                    (TokenType.Word, "odd")
                ));

            Scenario scenario = new Scenario();
            scenario.Sections.Add(paragraph);

            NUnitReport.TestSuite result = interpreter.ExecuteScenario(scenario);

            Assert.That(result.Result, Is.EqualTo(NUnitTestResult.Passed));
            Assert.That(result.TotalTests, Is.EqualTo(1));
        }


        [Test]
        public void Table_before_any_paragraph_is_wrong()
        {
            Scenario scenario = new Scenario();
            scenario.Sections.Add(new Table());

            NUnitReport.TestSuite result = interpreter.ExecuteScenario(scenario);

            string message = result.TestCases[0].FailureInfo.Message;
            Assert.That(message, Is.EqualTo("Cannot have a table as the first section of a scenario."));

            Assert.That(result.Result, Is.EqualTo(NUnitTestResult.Failed));

            Assert.That(result.TestCaseCount, Is.EqualTo(1));
            Assert.That(result.TestSuites.Count, Is.EqualTo(0));
            Assert.That(result.PassedTests, Is.EqualTo(0));
            Assert.That(result.FailedTests, Is.EqualTo(1));
            Assert.That(result.InconclusiveTests, Is.EqualTo(0));
            Assert.That(result.SkippedTests, Is.EqualTo(0));
        }

        [Test]
        public void Scenario_runs_paragraph_multiple_times_when_table_follows_paragraph()
        {
            //  Init data
            Paragraph paragraph = ExampleParagraph(new Symbol(TokenType.Variable, "InputNumber", 6));
            Table table = ExampleTable();
            Scenario scenario = ExampleScenario(null, null, paragraph, table);

            NUnitReport.TestSuite result = interpreter.ExecuteScenario(scenario);

            Assert.That(result.Result, Is.EqualTo(NUnitTestResult.Failed));

            Assert.That(result.TestCaseCount, Is.EqualTo(0));
            Assert.That(result.TotalTests, Is.EqualTo(9));
            Assert.That(result.TestSuites.Count, Is.EqualTo(3));
            Assert.That(result.PassedTests, Is.EqualTo(8));
            Assert.That(result.FailedTests, Is.EqualTo(1));
            Assert.That(result.InconclusiveTests, Is.EqualTo(0));
            Assert.That(result.SkippedTests, Is.EqualTo(0));
        }

        [Test]
        public void Scenario_runs_paragraph_with_multiple_tables()
        {
            Paragraph paragraph = ExampleParagraph(new Symbol(TokenType.Variable, "InputNumber"));
            paragraph.Label = "Hello, everyone, all eight of you";
            Table table = ExampleTable();
            table.Label = "Eight, two, eight";
            Table table2 = ExampleTable2();
            Scenario scenario = ExampleScenario("An example of two tables over the same paragraph",
                                                "two_table.tab",
                                                paragraph,
                                                table,
                                                table2);

            NUnitReport.TestSuite result = interpreter.ExecuteScenario(scenario);

            Assert.That(result.Result, Is.EqualTo(NUnitTestResult.Failed));
            Assert.That(result.Name, Is.EqualTo(scenario.Label));
            Assert.That(result.ClassName, Is.EqualTo(scenario.FileName));

            NUnitReport.TestSuite paraResult = result.TestSuites[0];
            Assert.That(paraResult.Name, Is.EqualTo(paragraph.Label));
            Assert.That(paraResult.ClassName, Is.EqualTo($"Row 0 of table {table.Label}"));

            paraResult = result.TestSuites[1];
            Assert.That(paraResult.Name, Is.EqualTo(paragraph.Label));
            Assert.That(paraResult.ClassName, Is.EqualTo($"Row 1 of table {table.Label}"));

            paraResult = result.TestSuites[3];
            Assert.That(paraResult.Name, Is.EqualTo(paragraph.Label));
            Assert.That(paraResult.ClassName, Is.EqualTo($"Row 0 of table at lines 108-111"));


            //TODO: Assert.That(result.FullName, Is.EqualTo(???));
            Assert.That(result.TestCaseCount, Is.EqualTo(0));
            Assert.That(result.TotalTests, Is.EqualTo(18));
            Assert.That(result.TestSuites.Count, Is.EqualTo(6));
            Assert.That(result.PassedTests, Is.EqualTo(14));
            Assert.That(result.FailedTests, Is.EqualTo(4));
            Assert.That(result.InconclusiveTests, Is.EqualTo(0));
            Assert.That(result.SkippedTests, Is.EqualTo(0));
        }


        private Paragraph ExampleParagraph(Symbol symbolToTest)
        {
            Paragraph paragraph = new Paragraph();

            paragraph.Actions.Add(
                new CommandUse(new List<string> { "Greeting Workflow" })
            );

            paragraph.Actions.Add(
                new Step(4,
                    (TokenType.Word, "Hello"),
                    (TokenType.Word, "America")
                )
            );

            paragraph.Actions.Add(
                new Step(5,
                    (TokenType.Word, "Hello"),
                    (TokenType.Word, "world")
                )
            );

            paragraph.Actions.Add(
                Step6(symbolToTest));
            return paragraph;
        }

        private Step Step6(Symbol symbolToTest)
        {
            return new Step(6,
                (TokenType.Word, "There"),
                (TokenType.Word, "should"),
                (TokenType.Word, "be"),
                (TokenType.Word, "eight"),
                (TokenType.Word, "of"),
                (symbolToTest.Type, symbolToTest.Text)
            );
        }


        private Table ExampleTable()
        {
            Table table = new Table();
            table.ColumnNames = new List<string> { "InputNumber", "RowName" };

            var rowData = new List<List<string>> { new List<string> { "8" }, new List<string> { "Fred" } };
            table.Rows.Add(new TableRow(rowData));
            rowData = new List<List<string>> { new List<string> { "2" }, new List<string> { "Wosmark" } };
            table.Rows.Add(new TableRow(rowData));
            rowData = new List<List<string>> { new List<string> { "8" }, new List<string> { "Miriam" } };
            table.Rows.Add(new TableRow(rowData));
            return table;
        }

        private Table ExampleTable2()
        {
            Table table = new Table();
            table.StartLine = 108;  //TODO: populate this from the parser
            table.EndLine = 111;
            table.ColumnNames = new List<string> { "InputNumber" };

            var rowData2 = new List<List<string>> { new List<string> { "10" } };
            table.Rows.Add(new TableRow(rowData2));
            rowData2 = new List<List<string>> { new List<string> { "0" } };
            table.Rows.Add(new TableRow(rowData2));
            rowData2 = new List<List<string>> { new List<string> { "0" } };
            table.Rows.Add(new TableRow(rowData2));
            return table;
        }

        private Scenario ExampleScenario(string label, string filename, params Section[] sections)
        {
            Scenario scenario = new Scenario();
            scenario.Label = label;
            scenario.FileName = filename;
            scenario.Sections.AddRange(sections);
            return scenario;
        }


    }
}
