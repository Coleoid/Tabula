using NUnit.Framework;
using System;
using System.Collections.Generic;
using LibraryHoldingTestWorkflows;
using NUnit.Framework.Constraints;
using NUnit.Framework.Internal;
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

            (string message, NUnitTestResult status) = interpreter.ExecuteStep(step);

            Assert.That(message, Is.EqualTo($"Couldn't find step 'Hey \"{friend}\" {question} am I' on line {lineNumber}"));
            Assert.That(status, Is.EqualTo(NUnitTestResult.Inconclusive));
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

            (string message, NUnitTestResult status) = interpreter.ExecuteStep(step);

            Assert.That(message, Does.StartWith($"Step failed: failed as expected"));
            Assert.That(status, Is.EqualTo(NUnitTestResult.Failed));
        }

        [Test]
        public void Passed_step_sets_status_and_message()
        {
            int quantity = 8;
            var step = new Step(123,
                (TokenType.Word, "There"),
                (TokenType.Word, "should"),
                (TokenType.Word, "be"),
                (TokenType.Word, "eight"),
                (TokenType.Word, "of"),
                (TokenType.Number, quantity.ToString())
            );

            interpreter.Workflow = typeof(GreetingWorkflow);

            (string message, NUnitTestResult status) = interpreter.ExecuteStep(step);

            Assert.That(message, Is.EqualTo("Success"));
            Assert.That(status, Is.EqualTo(NUnitTestResult.Passed));
        }

        [Test]
        public void Given_step_explodes_sets_status_and_message()
        {
            var step = new Step(123,
                (TokenType.Word, "Always"),
                (TokenType.Word, "explode")
            );

            interpreter.Workflow = typeof(GreetingWorkflow);

            (string message, NUnitTestResult status) = interpreter.ExecuteStep(step);

            Assert.That(message, Does.StartWith($"Step threw exception: Attempted to divide by zero."));
            Assert.That(status, Is.EqualTo(NUnitTestResult.Failed));
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
            (string message, NUnitTestResult status) = interpreter.ExecuteStep(stepAfter);


            Assert.That(message, Does.StartWith($"Step skipped: Due to error on line 123."));
            Assert.That(status, Is.EqualTo(NUnitTestResult.Skipped));
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

    }
}
