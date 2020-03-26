using LibraryHoldingTestWorkflows;
using NUnit.Framework;
using NUnit.Framework.Internal;
using System;
using System.Collections.Generic;
using System.Reflection;
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

            interpreter.UseWorkflow("GreetingWorkflow");

            NUnitReport.TestCase result = interpreter.ExecuteStep(step);

            Assert.That(result.Result, Is.EqualTo(NUnitTestResult.Passed));
            Assert.That(result.Name, Is.EqualTo("There should be eight of 8"));
        }

        [TestCase(TokenType.String, "eight", "\"eight\"", "\"eight\" (String)")]
        [TestCase(TokenType.Date, "11/22/2033", "11/22/2033", "\"11/22/2033\" (Date)")]
        public void Clear_error_when_argument_type_mismatches_int_parameter(TokenType argType, string argValue,
            string nameValue, string argMessageDetail)
        {
            var step = new Step(44,
                (TokenType.Word, "There"),
                (TokenType.Word, "should"),
                (TokenType.Word, "be"),
                (TokenType.Word, "eight"),
                (TokenType.Word, "of"),
                (argType, argValue)
            );

            interpreter.UseWorkflow("GreetingWorkflow");

            NUnitReport.TestCase result = interpreter.ExecuteStep(step);

            Assert.That(result.Result, Is.EqualTo(NUnitTestResult.Failed));
            Assert.That(result.Name, Is.EqualTo($"There should be eight of {nameValue}"));
            Assert.That(result.FailureInfo.Message,
                Does.StartWith(
                    $"Step threw exception: argument {argMessageDetail} does not match parameter 'theseGuys' (Int32)."));
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

            interpreter.UseWorkflow("GreetingWorkflow");

            NUnitReport.TestCase result = interpreter.ExecuteStep(step);

            Assert.That(result.Result, Is.EqualTo(NUnitTestResult.Failed));
            Assert.That(result.Name, Is.EqualTo($"There should be eight of 8 8 8"));
            Assert.That(result.FailureInfo.Message,
                Does.StartWith($"Step threw exception: 3 arguments were provided to a 1 parameter method."));
        }

        [TestCase(TokenType.String, "eight", "\"eight\"", "\"eight\" (String)")]
        [TestCase(TokenType.Number, "12", "12", "\"12\" (Number)")]
        public void Clear_error_when_argument_type_mismatches_DateTime_parameter(TokenType argType, string argValue,
            string nameValue, string argMessageDetail)
        {
            var step = new Step(44,
                (TokenType.Word, "My"),
                (TokenType.Word, "favorite"),
                (TokenType.Word, "day"),
                (TokenType.Word, "is"),
                (argType, argValue)
            );

            interpreter.UseWorkflow("GreetingWorkflow");

            NUnitReport.TestCase result = interpreter.ExecuteStep(step);

            Assert.That(result.Result, Is.EqualTo(NUnitTestResult.Failed));
            Assert.That(result.Name, Is.EqualTo($"My favorite day is {nameValue}"));
            Assert.That(result.FailureInfo.Message,
                Does.StartWith(
                    $"Step threw exception: argument {argMessageDetail} does not match parameter 'favoriteDay' (DateTime)."));
        }

        //[Ignore("Fix references to members now that DLLs are dynamically loaded")]
        [TestCase("Bob", 22, "1/12/2000")]
        [TestCase("Greta", 34, "2/14/1998")]
        public void Step_Call_passes_arguments(string name, int age, string birthday)
        {
            interpreter.UseWorkflow("GreetingWorkflow");
            
            //  example:  my friend "Bob" turned 22 on 1/12/2000
            var step = new Step(222,
                (TokenType.Word, "my"),
                (TokenType.Word, "friend"),
                (TokenType.String, name),
                (TokenType.Word, "turned"),
                (TokenType.Number, age.ToString()),
                (TokenType.Word, "on"),
                (TokenType.Date, birthday)
            );

            //  example:  Verify that my friend is named "Bob"
            var step2 = new Step(223,
                (TokenType.Word, "Verify"),
                (TokenType.Word, "that"),
                (TokenType.Word, "my"),
                (TokenType.Word, "friend"),
                (TokenType.Word, "is"),
                (TokenType.Word, "named"),
                (TokenType.String, name)
            );

            //  example:  Verify that my friend is age 22
            var step3 = new Step(224,
                (TokenType.Word, "Verify"),
                (TokenType.Word, "that"),
                (TokenType.Word, "my"),
                (TokenType.Word, "friend"),
                (TokenType.Word, "is"),
                (TokenType.Word, "age"),
                (TokenType.Number, age.ToString())
            );

            //  example:  Verify that my friend has birthday 1/12/2000
            var step4 = new Step(225,
                (TokenType.Word, "Verify"),
                (TokenType.Word, "that"),
                (TokenType.Word, "my"),
                (TokenType.Word, "friend"),
                (TokenType.Word, "has"),
                (TokenType.Word, "birthday"),
                (TokenType.Date, birthday)
            );

            var result = interpreter.ExecuteStep(step);
            Assert.That(result.Result, Is.EqualTo(NUnitTestResult.Passed));
            Assert.That(result.Name, Is.EqualTo($"my friend \"{name}\" turned {age} on {birthday}"));

            var result2 = interpreter.ExecuteStep(step2);
            Assert.That(result2.Result, Is.EqualTo(NUnitTestResult.Passed));
            Assert.That(result2.Name, Is.EqualTo($"Verify that my friend is named \"{name}\""));

            var result3 = interpreter.ExecuteStep(step3);
            Assert.That(result3.Result, Is.EqualTo(NUnitTestResult.Passed));
            Assert.That(result3.Name, Is.EqualTo($"Verify that my friend is age {age}"));

            var result4 = interpreter.ExecuteStep(step4);
            Assert.That(result4.Result, Is.EqualTo(NUnitTestResult.Passed));
            Assert.That(result4.Name, Is.EqualTo($"Verify that my friend has birthday {birthday}"));
        }


        [TestCase("Bob", "twenty-two", "1/12/2000",
            "Step threw exception: argument \"twenty-two\" (String) does not match parameter 'age' (Int32).")]
        [TestCase("Greta", "34", "Mayday, 1998",
            "Step threw exception: argument \"Mayday, 1998\" (String) does not match parameter 'birthday' (DateTime).")]
        public void Step_Call_complains_clearly_when_value_types_do_not_match_param_types(string name, string age,
            string birthday, string expectedMessage)
        {
            var step = new Step(222,
                (TokenType.Word, "my"),
                (TokenType.Word, "friend"),
                (TokenType.String, name),
                (TokenType.Word, "turned"),
                (TokenType.String, age),
                (TokenType.Word, "on"),
                (TokenType.String, birthday)
            );

            interpreter.UseWorkflow("GreetingWorkflow");

            var result = interpreter.ExecuteStep(step);
            Assert.That(result.FailureInfo.Message, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void Step_Call_complains_clearly_when_date_does_not_match_int()
        {
            var step = new Step(222,
                (TokenType.Word, "my"),
                (TokenType.Word, "friend"),
                (TokenType.String, "Ann"),
                (TokenType.Word, "turned"),
                (TokenType.Date, "2/2/2002"),
                (TokenType.Word, "on"),
                (TokenType.Date, "1/1/2001")
            );

            interpreter.UseWorkflow("GreetingWorkflow");

            var result = interpreter.ExecuteStep(step);
            Assert.That(result.FailureInfo.Message,
                Is.EqualTo(
                    "Step threw exception: argument \"2/2/2002\" (Date) does not match parameter 'age' (Int32)."));
        }

        [Test]
        public void Step_Call_complains_clearly_when_number_does_not_match_datetime()
        {
            var step = new Step(222,
                (TokenType.Word, "my"),
                (TokenType.Word, "friend"),
                (TokenType.String, "Phred"),
                (TokenType.Word, "turned"),
                (TokenType.String, "111"),
                (TokenType.Word, "on"),
                (TokenType.Number, "88")
            );

            interpreter.UseWorkflow("GreetingWorkflow");

            var result = interpreter.ExecuteStep(step);
            Assert.That(result.FailureInfo.Message,
                Is.EqualTo(
                    "Step threw exception: argument \"88\" (Number) does not match parameter 'birthday' (DateTime)."));
        }

        [TestCase("Bob", "22", "1/12/2000")]
        [TestCase("Greta", "34", "2/14/1998")]
        public void Step_Call_passes_values_from_variables(string name, string age, string birthday)
        {
            interpreter.UseWorkflow("GreetingWorkflow");

            interpreter.SetVariable("friendName", name);
            interpreter.SetVariable("age", age);
            interpreter.SetVariable("birthday", birthday);

            //  example:  my friend #friendName turned #age on #birthday
            var step = new Step(222,
                (TokenType.Word, "my"),
                (TokenType.Word, "friend"),
                (TokenType.Variable, "friendName"),
                (TokenType.Word, "turned"),
                (TokenType.Variable, "age"),
                (TokenType.Word, "on"),
                (TokenType.Variable, "birthday")
            );

            //  example:  Verify that my friend is named "Bob"
            var step2 = new Step(223,
                (TokenType.Word, "Verify"),
                (TokenType.Word, "that"),
                (TokenType.Word, "my"),
                (TokenType.Word, "friend"),
                (TokenType.Word, "is"),
                (TokenType.Word, "named"),
                (TokenType.Variable, "friendName")
            );

            //  example:  Verify that my friend is age 22
            var step3 = new Step(224,
                (TokenType.Word, "Verify"),
                (TokenType.Word, "that"),
                (TokenType.Word, "my"),
                (TokenType.Word, "friend"),
                (TokenType.Word, "is"),
                (TokenType.Word, "age"),
                (TokenType.Variable, "age")
            );

            //  example:  Verify that my friend has birthday 1/12/2000
            var step4 = new Step(225,
                (TokenType.Word, "Verify"),
                (TokenType.Word, "that"),
                (TokenType.Word, "my"),
                (TokenType.Word, "friend"),
                (TokenType.Word, "has"),
                (TokenType.Word, "birthday"),
                (TokenType.Variable, "birthday")
            );

            var result = interpreter.ExecuteStep(step);
            Assert.That(result.Result, Is.EqualTo(NUnitTestResult.Passed));
            Assert.That(result.Name, Is.EqualTo("my friend #friendName turned #age on #birthday"));

            var result2 = interpreter.ExecuteStep(step2);
            Assert.That(result2.Result, Is.EqualTo(NUnitTestResult.Passed));
            Assert.That(result2.Name, Is.EqualTo($"Verify that my friend is named #friendName"));

            var result3 = interpreter.ExecuteStep(step3);
            Assert.That(result3.Result, Is.EqualTo(NUnitTestResult.Passed));
            Assert.That(result3.Name, Is.EqualTo($"Verify that my friend is age #age"));

            var result4 = interpreter.ExecuteStep(step4);
            Assert.That(result4.Result, Is.EqualTo(NUnitTestResult.Passed));
            Assert.That(result4.Name, Is.EqualTo($"Verify that my friend has birthday #birthday"));
        }

        [TestCase("frieendName", "Maybe you meant 'friendname'?")]
        [TestCase("frieendNamr", "Maybe you meant 'friendname'?")]
        [TestCase("friend", "Maybe you meant 'friendname'?")]
        [TestCase("Name", "Maybe you meant 'friendname'?")]
        [TestCase("WhoAmI", "This doesn't sound like any variable I know about.")]
        public void Step_Call_complains_clearly_when_a_variable_is_missing(string unfoundVarName, string message)
        {
            interpreter.SetVariable("friendName", "Bob");
            interpreter.SetVariable("age", "23");
            interpreter.SetVariable("birthday", "3/3/2003");

            var step = new Step(222,
                (TokenType.Word, "my"),
                (TokenType.Word, "friend"),
                (TokenType.Variable, unfoundVarName),
                (TokenType.Word, "turned"),
                (TokenType.Variable, "age"),
                (TokenType.Word, "on"),
                (TokenType.Variable, "birthday")
            );

            interpreter.UseWorkflow("GreetingWorkflow");

            var result = interpreter.ExecuteStep(step);

            Assert.That(result.FailureInfo.Message,
                Is.EqualTo(
                    $"Step threw exception: Expected variable \"{unfoundVarName}\" but it was not passed in. {message}"));
        }


        [TestCase("world", "all of us")]
        [TestCase("america", "how are you?")]
        public void Step_Call_finds_correct_method(string location, string range)
        {
            var step = new Step(222,
                (TokenType.Word, "hello"),
                (TokenType.Word, location)
            );

            interpreter.UseWorkflow("GreetingWorkflow");

            interpreter.ExecuteStep(step);

            //var greetings = (GreetingWorkflow)interpreter.Instance;
            //Assert.That(greetings.range, Is.EqualTo(range));
            MethodInfo mInfo =  interpreter.FindMethod("getrange");
            var result = mInfo.Invoke(interpreter.Instance, new object[] { });

            Assert.That((string)result, Is.EqualTo(range));
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

            interpreter.UseWorkflow("GreetingWorkflow");

            NUnitReport.TestCase result = interpreter.ExecuteStep(step);

            Assert.That(result.Name, Is.EqualTo($"Hey \"{friend}\" {question} am I"));
            Assert.That(result.FailureInfo.Message,
                Is.EqualTo($"Couldn't find step 'Hey \"{friend}\" {question} am I' on line {lineNumber}"));
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

            interpreter.UseWorkflow("GreetingWorkflow");

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

            interpreter.UseWorkflow("GreetingWorkflow");

            NUnitReport.TestCase result = interpreter.ExecuteStep(step);

            Assert.That(result.FailureInfo.Message,
                Does.StartWith($"Step threw exception: Attempted to divide by zero."));
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


            //interpreter.Workflow = typeof(GreetingWorkflow);
            interpreter.UseWorkflow("GreetingWorkflow");

            interpreter.ExecuteStep(step);
            NUnitReport.TestCase result = interpreter.ExecuteStep(stepAfter);

            Assert.That(result.FailureInfo.Message, Does.StartWith($"Step skipped: Due to error on line 123."));
            Assert.That(result.Result, Is.EqualTo(NUnitTestResult.Skipped));
        }


        [Test]
        public void Variables_have_lifetime_of_scope()
        {
            interpreter.OpenScope();

            interpreter.Scope["foo"] = "more foo";

            Assert.That(interpreter.Scope.HasVariable("foo"));

            interpreter.CloseScope();

            Assert.False(interpreter.Scope.HasVariable("foo"));
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


            interpreter.UseWorkflow("GreetingWorkflow");

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
            Assert.That(caseResult.FailureInfo.Message,
                Does.StartWith($"Step threw exception: Attempted to divide by zero."));
            Assert.That(caseResult.Result, Is.EqualTo(NUnitTestResult.Failed));

            caseResult = result.TestCases[1];
            Assert.That(caseResult.FailureInfo.Message, Does.StartWith($"Step skipped: Due to error on line 123."));
            Assert.That(caseResult.Result, Is.EqualTo(NUnitTestResult.Skipped));
        }


        [Test]
        public void Paragraph_loads_workflow()
        {
            Paragraph paragraph = new Paragraph();

            paragraph.Actions.Add(new CommandUse(new List<string> { "GreetingWorkflow" }));

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
            Assert.That(caseResult.FailureInfo.Message,
                Does.StartWith($"Step threw exception: Attempted to divide by zero."));
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
                new Step(124,
                    (TokenType.Word, "hello"),
                    (TokenType.Word, "world")
                ));


            interpreter.UseWorkflow("GreetingWorkflow");

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
        public void Empty_Scenario_succeeds_and_reports_zeroes()
        {
            Scenario scenario = new Scenario();
            NUnitReport.TestSuite result = interpreter.ExecuteScenario(scenario);

            Assert.That(result.TestCaseCount, Is.EqualTo(0));
            Assert.That(result.TestSuites.Count, Is.EqualTo(0));
            Assert.That(result.PassedTests, Is.EqualTo(0));
            Assert.That(result.FailedTests, Is.EqualTo(0));
            Assert.That(result.InconclusiveTests, Is.EqualTo(0));
            Assert.That(result.SkippedTests, Is.EqualTo(0));
        }

        [Test]
        public void Missing_variable_reported_in_results()
        {
            var paragraph = new Paragraph();
            paragraph.Actions.Add(new Step(6,
                (TokenType.Word, "There"),
                (TokenType.Word, "should"),
                (TokenType.Word, "be"),
                (TokenType.Word, "eight"),
                (TokenType.Word, "of"),
                (TokenType.Variable, "InputNumber")
            ));

            Scenario scenario = new Scenario();
            scenario.Sections.Add(paragraph);
            interpreter.UseWorkflow("GreetingWorkflow");

            NUnitReport.TestSuite result = interpreter.ExecuteScenario(scenario);

            Assert.That(result.TestCaseCount, Is.EqualTo(0));
            Assert.That(result.TotalTests, Is.EqualTo(1));
            Assert.That(result.TestSuites.Count, Is.EqualTo(1));
            Assert.That(result.PassedTests, Is.EqualTo(0));
            Assert.That(result.FailedTests, Is.EqualTo(1));
            Assert.That(result.InconclusiveTests, Is.EqualTo(0));
            Assert.That(result.SkippedTests, Is.EqualTo(0));

            //TODO: propagate execution-level error messages
            string message = result.TestSuites[0].TestCases[0].FailureInfo.Message;
            Assert.That(message.IndexOf("Expected variable \"InputNumber\"") > -1);
        }

        [Test]
        public void Table_puts_column_values_into_variables()
        {
            var paragraph = new Paragraph();
            paragraph.Actions.Add(new Step(6,
                (TokenType.Word, "There"),
                (TokenType.Word, "should"),
                (TokenType.Word, "be"),
                (TokenType.Word, "eight"),
                (TokenType.Word, "of"),
                (TokenType.Variable, "InputNumber")
            ));

            Table table = new Table
            {
                ColumnNames = new List<string> {"InputNumber"}
            };
            var rowData = new List<List<string>> {new List<string> {"8"}};
            table.Rows.Add(new TableRow(rowData));

            Scenario scenario = new Scenario();
            scenario.Sections.Add(paragraph);
            scenario.Sections.Add(table);
            interpreter.UseWorkflow("GreetingWorkflow");

            NUnitReport.TestSuite result = interpreter.ExecuteScenario(scenario);

            Assert.That(result.TestCaseCount, Is.EqualTo(0));
            Assert.That(result.TotalTests, Is.EqualTo(1));
            Assert.That(result.TestSuites.Count, Is.EqualTo(1));
            Assert.That(result.PassedTests, Is.EqualTo(1));
            Assert.That(result.FailedTests, Is.EqualTo(0));
            Assert.That(result.InconclusiveTests, Is.EqualTo(0));
            Assert.That(result.SkippedTests, Is.EqualTo(0));
        }


    }
}
