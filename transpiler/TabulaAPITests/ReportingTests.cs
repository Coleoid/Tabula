using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Tabula.API.Tests
{
    [TestFixture]
    public class ReportingTests
    {
        [Test]
        public void Unfound_addsUnitReport_result()
        {
            var runtime = new GeneratedScenarioBase();

            string stepText = "Assign 'Bob Ross' to class 'EVOC 101'";
            string location = "my_scenario.tabula:88";
            runtime.Unfound(stepText, location);

            //one test case in the suite
            List<NUnitReport.TestCase> cases = runtime.GetResults();
            Assert.AreEqual(1, cases.Count);

            //test failed
            var testCase = cases[0];
            Assert.AreEqual(NUnitTestResult.Failed, testCase.Result);

            //name holds step text
            Assert.AreEqual(stepText, testCase.Name);

            //failure info holds location and error cause
            var failureInfo = testCase.FailureInfo;
            Assert.AreEqual(location, failureInfo.StackTrace);
            Assert.AreEqual("Did not find method to match step.", failureInfo.Message);
        }

        [Test]
        public void Do_records_Success()
        {
            var runtime = new GeneratedScenarioBase();

            string stepText = "some_fixture.some_method(1, 2, 3)";
            string location = "my_scenario.tabula:88";
            runtime.Do(() => { }, location, stepText);

            //one test case in the suite
            List<NUnitReport.TestCase> cases = runtime.GetResults();
            Assert.AreEqual(1, cases.Count);

            //test passed
            var testCase = cases[0];
            Assert.AreEqual(NUnitTestResult.Passed, testCase.Result);

            //name holds step text
            Assert.AreEqual(stepText, testCase.Name);

            //no failure info
            Assert.IsNull(testCase.FailureInfo);
        }

        [Test]
        public void Multiple_calls_each_record_a_result()
        {
            var runtime = new GeneratedScenarioBase();

            string stepText = "some_fixture.some_method(1, 2, 3)";
            string location = "my_scenario.tabula:88";

            runtime.Do(() => { }, location, stepText);
            runtime.Do(() => { throw new NotImplementedException(); }, location, stepText);
            runtime.Unfound(stepText, location);
            runtime.Do(() => { throw new Exception("This is bad"); }, location, stepText);

            List<NUnitReport.TestCase> cases = runtime.GetResults();
            Assert.AreEqual(4, cases.Count);
        }

        [Test]
        public void Do_recordsNotImplemented()
        {
            var runtime = new GeneratedScenarioBase();

            string stepText = "some_fixture.some_method(1, 2, 3)";
            string location = "my_scenario.tabula:88";
            runtime.Do(() => { throw new NotImplementedException(); }, location, stepText);

            //suite should hold 1 case
            List<NUnitReport.TestCase> cases = runtime.GetResults();
            Assert.AreEqual(1, cases.Count);

            //test skipped
            var testCase = cases[0];
            Assert.AreEqual(NUnitTestResult.Skipped, testCase.Result);

            //name holds step text
            Assert.AreEqual(stepText, testCase.Name);

            //failure info holds location and message
            Assert.AreEqual(location, testCase.FailureInfo.StackTrace);
            Assert.AreEqual("Step is not implemented", testCase.FailureInfo.Message);
        }

        [Test]
        public void Do_records_Failure()
        {
            var runtime = new GeneratedScenarioBase();

            string stepText = "some_fixture.some_method(1, 2, 3)";
            string location = "my_scenario.tabula:88";
            string message = "Our application blew up";
            runtime.Do(() => { throw new Exception(message); }, location, stepText);

            //suite should hold 1 case
            List<NUnitReport.TestCase> cases = runtime.GetResults();
            Assert.AreEqual(1, cases.Count);

            //case should be a fail
            var testCase = cases[0];
            Assert.AreEqual(NUnitTestResult.Failed, testCase.Result);

            //name should hold step text
            Assert.AreEqual(stepText, testCase.Name);

            //failure info should hold location and message
            Assert.AreEqual(location, testCase.FailureInfo.StackTrace);
            Assert.AreEqual(message, testCase.FailureInfo.Message);
        }

        [Test]
        public void Exception_skips_later_steps()
        {
            var runtime = new GeneratedScenarioBase();

            string stepText = "some_fixture.some_method(1, 2, 3)";
            string location = "my_scenario.tabula:88";

            runtime.Do(() => { throw new Exception("This is bad"); }, location, stepText);
            runtime.Do(() => { }, location, stepText);

            List<NUnitReport.TestCase> cases = runtime.GetResults();

            //two test cases in the suite
            Assert.AreEqual(2, cases.Count);

            //second test was skipped because first threw general exception
            var testCase = cases[1];
            Assert.AreEqual(NUnitTestResult.Skipped, testCase.Result);

            //name holds step text
            Assert.AreEqual(stepText, testCase.Name);

            //failure info holds message about original error causing skip
            Assert.AreEqual(location, testCase.FailureInfo.StackTrace);
            Assert.AreEqual("Skipped due to error on line 88", testCase.FailureInfo.Message);
        }
    }
}
