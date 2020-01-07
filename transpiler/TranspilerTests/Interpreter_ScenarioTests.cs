using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
            Paragraph paragraph = ExampleParagraph(new Symbol(TokenType.Number, "8", 6));
            Scenario scenario = new Scenario();
            scenario.Sections.Add(paragraph);

            interpreter.Workflow = typeof(GreetingWorkflow);

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

            interpreter.Workflow = typeof(GreetingWorkflow);

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
                Step4());

            paragraph.Actions.Add(
                Step5());

            paragraph.Actions.Add(
                Step6(symbolToTest));
            return paragraph;
        }

        private Step Step4()
        {
            return new Step(4,
                (TokenType.Word, "Hello"),
                (TokenType.Word, "America")
            );
        }

        private Step Step5()
        {
            return new Step(5,
                (TokenType.Word, "hello"),
                (TokenType.Word, "world")
            );
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
