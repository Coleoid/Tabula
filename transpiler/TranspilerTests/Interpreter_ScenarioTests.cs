using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Internal;
using LibraryHoldingTestWorkflows;
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
            Paragraph paragraph = new Paragraph();
            paragraph.Actions.Add(
                new Step(4,
                    (TokenType.Word, "Hello"),
                    (TokenType.Word, "America")
                ));

            paragraph.Actions.Add(
                new Step(5,
                    (TokenType.Word, "hello"),
                    (TokenType.Word, "world")
                ));

            paragraph.Actions.Add(
                new Step(6,
                    (TokenType.Word, "There"),
                    (TokenType.Word, "should"),
                    (TokenType.Word, "be"),
                    (TokenType.Word, "eight"),
                    (TokenType.Word, "of"),
                    (TokenType.Number, "8")
                ));

            Scenario scenario = new Scenario();
            scenario.Sections.Add(paragraph);
            interpreter.Workflow = typeof(GreetingWorkflow);

            NUnitReport.TestSuite result = interpreter.ExecuteScenario(scenario);

            Assert.That(result.Result, Is.EqualTo(NUnitTestResult.Passed));
            Assert.That(result.TestCaseCount, Is.EqualTo(3));
            Assert.That(result.TestSuites.Count, Is.EqualTo(1));
            Assert.That(result.PassedTests, Is.EqualTo(3));
            Assert.That(result.FailedTests, Is.EqualTo(0));
            Assert.That(result.InconclusiveTests, Is.EqualTo(0));
            Assert.That(result.SkippedTests, Is.EqualTo(0));
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
            Paragraph paragraph = new Paragraph();
            paragraph.Actions.Add(
                new Step(4,
                    (TokenType.Word, "Hello"),
                    (TokenType.Word, "America")
                ));

            paragraph.Actions.Add(
                new Step(5,
                    (TokenType.Word, "hello"),
                    (TokenType.Word, "world")
                ));

            paragraph.Actions.Add(
                new Step(6,
                    (TokenType.Word, "There"),
                    (TokenType.Word, "should"),
                    (TokenType.Word, "be"),
                    (TokenType.Word, "eight"),
                    (TokenType.Word, "of"),
                    (TokenType.Variable, "InputNumber")
                ));

            Table table = new Table();
            table.ColumnNames = new List<string> {"InputNumber", "RowName"};

            var rowData = new List<List<string>> {new List<string> {"8"}, new List<string> {"Fred"}};
            table.Rows.Add(new TableRow(rowData));
            rowData = new List<List<string>> {new List<string> {"2"}, new List<string> {"Wosmark"}};
            table.Rows.Add(new TableRow(rowData));
            rowData = new List<List<string>> {new List<string> {"8"}, new List<string> {"Miriam"}};
            table.Rows.Add(new TableRow(rowData));

            Scenario scenario = new Scenario();
            scenario.Sections.Add(paragraph);
            scenario.Sections.Add(table);
            interpreter.Workflow = typeof(GreetingWorkflow);

            NUnitReport.TestSuite result = interpreter.ExecuteScenario(scenario);

            Assert.That(result.Result, Is.EqualTo(NUnitTestResult.Failed));

            Assert.That(result.TestCaseCount, Is.EqualTo(9));
            Assert.That(result.TestSuites.Count, Is.EqualTo(3));
            Assert.That(result.PassedTests, Is.EqualTo(8));
            Assert.That(result.FailedTests, Is.EqualTo(1));
            Assert.That(result.InconclusiveTests, Is.EqualTo(0));
            Assert.That(result.SkippedTests, Is.EqualTo(0));
        }

        [Test]
        public void Scenario_runs_paragraph_with_multiple_tables()
        {
            Paragraph paragraph = new Paragraph();
            paragraph.Label = "Hello, everyone, all eight of you";
            paragraph.Actions.Add(
                new Step(4,
                    (TokenType.Word, "Hello"),
                    (TokenType.Word, "America")
                ));

            paragraph.Actions.Add(
                new Step(5,
                    (TokenType.Word, "hello"),
                    (TokenType.Word, "world")
                ));

            paragraph.Actions.Add(
                new Step(6,
                    (TokenType.Word, "There"),
                    (TokenType.Word, "should"),
                    (TokenType.Word, "be"),
                    (TokenType.Word, "eight"),
                    (TokenType.Word, "of"),
                    (TokenType.Variable, "InputNumber")
                ));

            Table table = new Table();
            table.Label = "Eight, two, eight";
            table.ColumnNames = new List<string> { "InputNumber", "RowName" };

            var rowData = new List<List<string>> { new List<string> { "8" }, new List<string> { "Fred" } };
            table.Rows.Add(new TableRow(rowData));
            rowData = new List<List<string>> { new List<string> { "2" }, new List<string> { "Wosmark" } };
            table.Rows.Add(new TableRow(rowData));
            rowData = new List<List<string>> { new List<string> { "8" }, new List<string> { "Miriam" } };
            table.Rows.Add(new TableRow(rowData));

            Table table2 = new Table();
            table2.StartLine = 108;  //TODO: populate this from the parser
            table2.EndLine = 111;
            table2.ColumnNames = new List<string> {"InputNumber"};

            var rowData2 = new List<List<string>> { new List<string> { "10" } };
            table2.Rows.Add(new TableRow(rowData2));
            rowData2 = new List<List<string>> { new List<string> { "0" } };
            table2.Rows.Add(new TableRow(rowData2));
            rowData2 = new List<List<string>> { new List<string> { "0" } };
            table2.Rows.Add(new TableRow(rowData2));

            Scenario scenario = new Scenario();
            scenario.Label = "An example of two tables over the same paragraph";
            scenario.FileName = "two_table.tab";
            scenario.Sections.Add(paragraph);
            scenario.Sections.Add(table);
            scenario.Sections.Add(table2);
            interpreter.Workflow = typeof(GreetingWorkflow);

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
            Assert.That(result.TestCaseCount, Is.EqualTo(18));
            Assert.That(result.TestSuites.Count, Is.EqualTo(6));
            Assert.That(result.PassedTests, Is.EqualTo(14));
            Assert.That(result.FailedTests, Is.EqualTo(4));
            Assert.That(result.InconclusiveTests, Is.EqualTo(0));
            Assert.That(result.SkippedTests, Is.EqualTo(0));
        }

    }
}
