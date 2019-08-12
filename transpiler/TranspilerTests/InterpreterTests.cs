using System;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Internal;
using LibraryHoldingTestWorkflows;
using Tabula.CST;

namespace Tabula
{
    [TestFixture]
    public class InterpreterTests
    {
        private Interpreter interpreter = null;

        [SetUp]
        public void SetUp()
        {
            interpreter = new Interpreter();
        }

        [TestCase("helloworld", true)]
        [TestCase("Hello_World", false)]
        [TestCase("nosuchthing", false)]
        [TestCase("myfriendturnedon", true)]
        public void LearnMethods_builds_dictionary(string searchName, bool hasKey)
        {
            interpreter.LearnMethods(typeof(GreetingWorkflow));

            Assert.That(interpreter.searchableMethods.ContainsKey(searchName), Is.EqualTo(hasKey));
        }


        [Test]
        public void Passed_step_sets_status_and_message()
        {
            var step = new Step(123,
                (TokenType.Word, "There"),
                (TokenType.Word, "should"),
                (TokenType.Word, "be"),
                (TokenType.Word, "eight"),
                (TokenType.Word, "of"),
                (TokenType.Number, "8")
            );

            interpreter.Workflow = typeof(GreetingWorkflow);

            NUnitReport.TestCase result = interpreter.ExecuteStep(step);

            Assert.That(result.Result, Is.EqualTo(NUnitTestResult.Passed));
            Assert.That(result.Name, Is.EqualTo("There should be eight of 8"));
        }

        [TestCase(TokenType.String, "eight", "\"eight\"", "\"eight\" (String)")]
        [TestCase(TokenType.Date, "11/22/2033", "11/22/2033", "\"11/22/2033\" (Date)")]
        public void Clear_error_when_argument_type_mismatches_int_parameter(TokenType argType, string argValue, string nameValue, string argMessageDetail)
        {
            var step = new Step(44,
                (TokenType.Word, "There"),
                (TokenType.Word, "should"),
                (TokenType.Word, "be"),
                (TokenType.Word, "eight"),
                (TokenType.Word, "of"),
                (argType, argValue)
            );

            interpreter.Workflow = typeof(GreetingWorkflow);

            NUnitReport.TestCase result = interpreter.ExecuteStep(step);

            Assert.That(result.Result, Is.EqualTo(NUnitTestResult.Failed));
            Assert.That(result.Name, Is.EqualTo($"There should be eight of {nameValue}"));
            Assert.That(result.FailureInfo.Message, Does.StartWith($"Step threw exception: argument {argMessageDetail} does not match parameter 'theseGuys' (Int32)."));
        }

        [Test]
        public void Clear_error_when_more_arguments_than_parameters()
        {
            var step = new Step(44,
                (TokenType.Word, "There"),
                (TokenType.Word, "should"),
                (TokenType.Word, "be"),
                (TokenType.Word, "eight"),
                (TokenType.Word, "of"),
                (TokenType.Number, "8"),
                (TokenType.Number, "8"),
                (TokenType.Number, "8")
            );

            interpreter.Workflow = typeof(GreetingWorkflow);

            NUnitReport.TestCase result = interpreter.ExecuteStep(step);

            Assert.That(result.Result, Is.EqualTo(NUnitTestResult.Failed));
            Assert.That(result.Name, Is.EqualTo($"There should be eight of 8 8 8"));
            Assert.That(result.FailureInfo.Message, Does.StartWith($"Step threw exception: 3 arguments were provided to a 1 parameter method."));
        }

        [TestCase(TokenType.String, "eight", "\"eight\"", "\"eight\" (String)")]
        [TestCase(TokenType.Number, "12", "12", "\"12\" (Number)")]
        public void Clear_error_when_argument_type_mismatches_DateTime_parameter(TokenType argType, string argValue, string nameValue, string argMessageDetail)
        {
            var step = new Step(44,
                (TokenType.Word, "My"),
                (TokenType.Word, "favorite"),
                (TokenType.Word, "day"),
                (TokenType.Word, "is"),
                (argType, argValue)
            );

            interpreter.Workflow = typeof(GreetingWorkflow);

            NUnitReport.TestCase result = interpreter.ExecuteStep(step);

            Assert.That(result.Result, Is.EqualTo(NUnitTestResult.Failed));
            Assert.That(result.Name, Is.EqualTo($"My favorite day is {nameValue}"));
            Assert.That(result.FailureInfo.Message, Does.StartWith($"Step threw exception: argument {argMessageDetail} does not match parameter 'favoriteDay' (DateTime)."));
        }

        [TestCase("Bob", 22, "1/12/2000")]
        [TestCase("Greta", 34, "2/14/1998")]
        public void Step_Call_passes_arguments(string name, int age, string birthday)
        {
            var args = new List<ArgDetail>()
            {
                new ArgDetail {Name = "name", Type = typeof(string)},
                new ArgDetail {Name = "age", Type = typeof(int)},
                new ArgDetail {Name = "birthday", Type = typeof(DateTime)}
            };

            //  my friend "Bob" turned 22 on 1/12/2000

            var step = new Step(222,
                (TokenType.Word, "my"),
                (TokenType.Word, "friend"),
                (TokenType.String, name),
                (TokenType.Word, "turned"),
                (TokenType.Number, age.ToString()),
                (TokenType.Word, "on"),
                (TokenType.Date, birthday)
            );

            interpreter.Workflow = typeof(GreetingWorkflow);

            var result = interpreter.ExecuteStep(step);

            var greetings = (GreetingWorkflow)interpreter.Instance;
            Assert.That(greetings.friendName, Is.EqualTo(name));
            Assert.That(greetings.friendAge, Is.EqualTo(age));
            Assert.That(greetings.friendBirthday, Is.EqualTo(DateTime.Parse(birthday)));
        }

        [TestCase("Bob", 22, "1/12/2000")]
        [TestCase("Greta", 34, "2/14/1998")]
        public void Step_Call_passes_values_from_variables(string name, int age, string birthday)
        {
            var args = new List<ArgDetail>()
            {
                new ArgDetail {Name = "name", Type = typeof(string)},
                new ArgDetail {Name = "age", Type = typeof(int)},
                new ArgDetail {Name = "birthday", Type = typeof(DateTime)}
            };

            //  my friend #friendName turned #age on #birthday

            interpreter.SetVariable("friendName", name);
            interpreter.SetVariable("age", age.ToString());
            interpreter.SetVariable("birthday", birthday);

            var step = new Step(222,
                (TokenType.Word, "my"),
                (TokenType.Word, "friend"),
                (TokenType.Variable, "friendName"),
                (TokenType.Word, "turned"),
                (TokenType.Variable, "age"),
                (TokenType.Word, "on"),
                (TokenType.Variable, "birthday")
            );

            interpreter.Workflow = typeof(GreetingWorkflow);

            var result = interpreter.ExecuteStep(step);

            Assert.That(result.FailureInfo.Message, Is.Null);
            var greetings = (GreetingWorkflow)interpreter.Instance;
            Assert.That(greetings.friendName, Is.EqualTo(name));
            Assert.That(greetings.friendAge, Is.EqualTo(age));
            Assert.That(greetings.friendBirthday, Is.EqualTo(DateTime.Parse(birthday)));
        }

        [TestCase("world", "all of us")]
        [TestCase("america", "how are you?")]
        public void Step_Call_finds_correct_method(string location, string range)
        {
            var step = new Step(222,
                (TokenType.Word, "hello"),
                (TokenType.Word, location)
            );

            interpreter.Workflow = typeof(GreetingWorkflow);

            interpreter.ExecuteStep(step);

            var greetings = (GreetingWorkflow)interpreter.Instance;

            Assert.That(greetings.range, Is.EqualTo(range));
        }

        [TestCase(222, "who", "George")]
        [TestCase(144, "where", "Annette")]
        public void Step_Call_complains_clearly_when_step_unfound(int lineNumber, string question, string friend)
        {
            var step = new Step(lineNumber,
                (TokenType.Word, "Hey"),
                (TokenType.String, friend),
                (TokenType.Word, question),
                (TokenType.Word, "am"),
                (TokenType.Word, "I")
            );

            interpreter.Workflow = typeof(GreetingWorkflow);

            NUnitReport.TestCase result = interpreter.ExecuteStep(step);

            Assert.That(result.Name, Is.EqualTo($"Hey \"{friend}\" {question} am I"));
            Assert.That(result.FailureInfo.Message, Is.EqualTo($"Couldn't find step 'Hey \"{friend}\" {question} am I' on line {lineNumber}"));
            //TODO:  stack trace
            Assert.That(result.Result, Is.EqualTo(NUnitTestResult.Inconclusive));
        }

        [Test]
        public void Failed_step_sets_status_and_message()
        {
            int quantity = 9;
            var step = new Step(123,
                (TokenType.Word, "There"),
                (TokenType.Word, "should"),
                (TokenType.Word, "be"),
                (TokenType.Word, "eight"),
                (TokenType.Word, "of"),
                (TokenType.Number, quantity.ToString())
            );

            interpreter.Workflow = typeof(GreetingWorkflow);

            NUnitReport.TestCase result = interpreter.ExecuteStep(step);

            Assert.That(result.FailureInfo.Message, Does.StartWith($"Step failed: failed as expected"));
            Assert.That(result.Name, Is.EqualTo("There should be eight of 9"));
            Assert.That(result.Result, Is.EqualTo(NUnitTestResult.Failed));
        }

        [Test]
        public void Given_step_explodes_sets_status_and_message()
        {
            var step = new Step(123,
                (TokenType.Word, "Always"),
                (TokenType.Word, "explode")
            );

            interpreter.Workflow = typeof(GreetingWorkflow);

            NUnitReport.TestCase result = interpreter.ExecuteStep(step);

            Assert.That(result.FailureInfo.Message, Does.StartWith($"Step threw exception: Attempted to divide by zero."));
            Assert.That(result.Name, Is.EqualTo("Always explode"));
            Assert.That(result.Result, Is.EqualTo(NUnitTestResult.Failed));
        }


        [Test]
        public void After_step_explodes_later_steps_are_skipped()
        {
            var step = new Step(123,
                (TokenType.Word, "Always"),
                (TokenType.Word, "explode")
            );

            var stepAfter = new Step(222,
                (TokenType.Word, "hello"),
                (TokenType.Word, "world")
            );


            interpreter.Workflow = typeof(GreetingWorkflow);

            interpreter.ExecuteStep(step);
            NUnitReport.TestCase result = interpreter.ExecuteStep(stepAfter);

            Assert.That(result.FailureInfo.Message, Does.StartWith($"Step skipped: Due to error on line 123."));
            Assert.That(result.Result, Is.EqualTo(NUnitTestResult.Skipped));
        }


        [Test]
        public void Failing_paragraph_accumulates_results()
        {
            Paragraph paragraph = new Paragraph();

            paragraph.Actions.Add( 
                new Step(123,
                    (TokenType.Word, "Always"),
                    (TokenType.Word, "explode")
            ));

            paragraph.Actions.Add(
                new Step(222,
                    (TokenType.Word, "hello"),
                    (TokenType.Word, "world")
            ));


            interpreter.Workflow = typeof(GreetingWorkflow);

            NUnitReport.TestSuite result = interpreter.ExecuteParagraph(paragraph);
            
            Assert.That(result, Is.Not.Null);
            Assert.That(result.TestCases.Count, Is.EqualTo(2));
            Assert.That(result.TestCaseCount, Is.EqualTo(2));

            Assert.That(result.PassedTests, Is.EqualTo(0));
            Assert.That(result.FailedTests, Is.EqualTo(1));
            Assert.That(result.InconclusiveTests, Is.EqualTo(0));
            Assert.That(result.SkippedTests, Is.EqualTo(1));
            
            Assert.That(result.Result, Is.EqualTo(NUnitTestResult.Failed));


            var caseResult = result.TestCases[0];
            Assert.That(caseResult.FailureInfo.Message, Does.StartWith($"Step threw exception: Attempted to divide by zero."));
            Assert.That(caseResult.Result, Is.EqualTo(NUnitTestResult.Failed));

            caseResult = result.TestCases[1];
            Assert.That(caseResult.FailureInfo.Message, Does.StartWith($"Step skipped: Due to error on line 123."));
            Assert.That(caseResult.Result, Is.EqualTo(NUnitTestResult.Skipped));
        }


        [Test]
        public void Passing_paragraph_accumulates_results()
        {
            Paragraph paragraph = new Paragraph();

            paragraph.Actions.Add(
                new Step(123,
                    (TokenType.Word, "Hello"),
                    (TokenType.Word, "America")
                ));

            paragraph.Actions.Add(
                new Step(222,
                    (TokenType.Word, "hello"),
                    (TokenType.Word, "world")
                ));


            interpreter.Workflow = typeof(GreetingWorkflow);

            NUnitReport.TestSuite result = interpreter.ExecuteParagraph(paragraph);

            Assert.That(result.TestCases.Count, Is.EqualTo(2));
            Assert.That(result.TestCaseCount, Is.EqualTo(2));

            Assert.That(result.PassedTests, Is.EqualTo(2));
            Assert.That(result.FailedTests, Is.EqualTo(0));
            Assert.That(result.InconclusiveTests, Is.EqualTo(0));
            Assert.That(result.SkippedTests, Is.EqualTo(0));

            Assert.That(result.Result, Is.EqualTo(NUnitTestResult.Passed));


            var caseResult = result.TestCases[0];
            Assert.That(caseResult.Result, Is.EqualTo(NUnitTestResult.Passed));

            caseResult = result.TestCases[1];
            Assert.That(caseResult.Result, Is.EqualTo(NUnitTestResult.Passed));
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

            var rowData = new List<List<string>>{ new List<string> { "8" }, new List<string> { "Fred" } };
            table.Rows.Add(new TableRow(rowData));
            rowData = new List<List<string>> { new List<string> { "2" }, new List<string> { "Wosmark" } };
            table.Rows.Add(new TableRow(rowData));
            rowData = new List<List<string>> { new List<string> { "8" }, new List<string> { "Miriam" } };
            table.Rows.Add(new TableRow(rowData));

            Scenario scenario = new Scenario();
            scenario.Sections.Add(paragraph);
            scenario.Sections.Add(table);
            interpreter.Workflow = typeof(GreetingWorkflow);

            NUnitReport.TestSuite result = interpreter.ExecuteScenario(scenario);

            Assert.That(result.TestCaseCount, Is.EqualTo(9));
            Assert.That(result.TestSuites.Count, Is.EqualTo(3));
            Assert.That(result.PassedTests, Is.EqualTo(8));
            Assert.That(result.FailedTests, Is.EqualTo(1));
            Assert.That(result.InconclusiveTests, Is.EqualTo(0));
            Assert.That(result.SkippedTests, Is.EqualTo(0));
        }
    }
}
