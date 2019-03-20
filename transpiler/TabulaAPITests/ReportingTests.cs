using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Tabula.API.Tests
{
    [TestFixture]
    public class ReportingTests
    {
        [Test]
        public void Do_records_Success()
        {
            var runtime = new GeneratedScenarioBase();

            string stepText = "some_fixture.some_method(1, 2, 3)";
            string location = "my_scenario.tabula:88";
            runtime.Do(() => { }, location, stepText);

            //one test case in the suite
            NUnitReport.TestSuite suite = runtime.GetResults();
            Assert.AreEqual(1, suite.TestCaseCount);
            Assert.AreEqual(1, suite.TotalTests);

            //test passed
            var testCase = suite.TestCases[0];
            Assert.AreEqual(NUnitTestResult.Passed, testCase.Result);

            //name holds step text
            Assert.AreEqual(stepText, testCase.Name);

            //no failure info
            Assert.IsNull(testCase.FailureInfo);
        }

        [Test]
        public void Unfound_addsUnitReport_result()
        {
            var runtime = new GeneratedScenarioBase();

            string stepText = "Assign 'Bob Ross' to class 'EVOC 101'";
            string location = "my_scenario.tabula:88";
            runtime.Unfound(stepText, location);

            //one test case in the suite
            NUnitReport.TestSuite suite = runtime.GetResults();
            Assert.AreEqual(1, suite.TestCaseCount);

            //test failed
            var testCase = suite.TestCases[0];
            Assert.AreEqual(NUnitTestResult.Failed, testCase.Result);

            //name holds step text
            Assert.AreEqual(stepText, testCase.Name);

            //failure info holds location and error cause
            var failureInfo = testCase.FailureInfo;
            Assert.AreEqual(location, failureInfo.StackTrace);
            Assert.AreEqual("Did not find method to match step.", failureInfo.Message);
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

            NUnitReport.TestSuite suite = runtime.GetResults();
            Assert.AreEqual(4, suite.TestCaseCount);
        }

        [Test]
        public void Do_recordsNotImplemented()
        {
            var runtime = new GeneratedScenarioBase();

            string stepText = "some_fixture.some_method(1, 2, 3)";
            string location = "my_scenario.tabula:88";
            runtime.Do(() => { throw new NotImplementedException(); }, location, stepText);

            //suite should hold 1 case
            NUnitReport.TestSuite suite = runtime.GetResults();
            Assert.AreEqual(1, suite.TestCaseCount);

            //test inconclusive
            var testCase = suite.TestCases[0];
            Assert.AreEqual(NUnitTestResult.Inconclusive, testCase.Result);

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
            NUnitReport.TestSuite suite = runtime.GetResults();
            Assert.AreEqual(1, suite.TestCaseCount);

            //case should be a fail
            var testCase = suite.TestCases[0];
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

            NUnitReport.TestSuite suite = runtime.GetResults();

            //two test cases in the suite
            Assert.AreEqual(2, suite.TestCaseCount);

            //second test was skipped because first threw general exception
            var testCase = suite.TestCases[1];
            Assert.AreEqual(NUnitTestResult.Skipped, testCase.Result);

            //name holds step text
            Assert.AreEqual(stepText, testCase.Name);

            //failure info holds message about original error causing skip
            Assert.AreEqual(location, testCase.FailureInfo.StackTrace);
            Assert.AreEqual("Skipped due to error on line 88", testCase.FailureInfo.Message);
        }

        [Test]
        public void Failure_and_NotImplemented_do_not_skip_later_steps()
        {
            var runtime = new GeneratedScenarioBase();

            string stepText = "some_fixture.some_method(1, 2, 3)";
            string location = "my_scenario.tabula:88";

            // This test should fail but continue testing
            runtime.Do(() => { Assert.Fail("Expecting this first test to fail"); }, location, stepText);

            // This test should not be implemented but not halt testing
            runtime.Do(() => { throw new NotImplementedException(); }, location, stepText);

            // Successful run, do this last
            runtime.Do(() => { }, location, stepText);

            NUnitReport.TestSuite suite = runtime.GetResults();

            // Three test cases in the suite
            Assert.AreEqual(3, suite.TestCaseCount);

            // Check first result
            var testCase = suite.TestCases[0];

            // First test was a planned failure
            Assert.AreEqual(NUnitTestResult.Failed, testCase.Result);

            // Check second result
            testCase = suite.TestCases[1];

            // Second test was a planned Not-Implemented exception
            Assert.AreEqual(NUnitTestResult.Inconclusive, testCase.Result);

            // Check third result
            testCase = suite.TestCases[2];

            // The third test should not be skipped as a result of prior two tests
            Assert.AreNotEqual(NUnitTestResult.Skipped, testCase.Result);
        }



        [Test]
        public void Suite_statistics_properly_reflect_case_results()
        {
            var runtime = new GeneratedScenarioBase
            {
                FileName = "my_scenario.tabula",
                ScenarioLabel = "Important behavior that must work"
            };

            string stepText = "irrelevant_fixture.dont_care(1, 2, 3)";

            int lineNumber = 88;
            string getLocation()
            {
                return $"my_scenario.tabula:{lineNumber++}";
            }

            // Enough results to show numbers for all our categories/results
            runtime.Do(() => { Assert.Fail("Expecting this first test to fail"); }, getLocation(), stepText);
            runtime.Do(() => { }, getLocation(), stepText);
            runtime.Do(() => { }, getLocation(), stepText);
            runtime.Do(() => { Assert.Fail("Another thing not quite right"); }, getLocation(), stepText);
            runtime.Unfound(stepText, getLocation());
            runtime.Do(() => { throw new NotImplementedException(); }, getLocation(), stepText);
            runtime.Do(() => { throw new Exception("Below here steps will be skipped"); }, getLocation(), stepText);
            runtime.Unfound(stepText, getLocation());
            runtime.Do(() => { Assert.Fail("Another thing not quite right"); }, getLocation(), stepText);
            runtime.Do(() => { }, getLocation(), stepText);
            runtime.Do(() => { }, getLocation(), stepText);
            runtime.Do(() => { }, getLocation(), stepText);

            NUnitReport.TestSuite suite = runtime.GetResults();

            Assert.AreEqual(12, suite.TestCaseCount);
            Assert.AreEqual(12, suite.TestCases.Count);
            Assert.AreEqual(0, suite.TestSuites.Count);
            Assert.AreEqual(runtime.FileName, suite.ClassName);
            Assert.AreEqual(5, suite.FailedTests);
            Assert.AreEqual(1, suite.InconclusiveTests);
            Assert.AreEqual(2, suite.PassedTests);
            Assert.AreEqual(4, suite.SkippedTests);  // the unfound after the exception isn't skipped.

            Assert.AreEqual(runtime.ScenarioLabel, suite.FullName);
            Assert.AreEqual(NUnitTestSuiteType.TestFixture, suite.Type);
        }

        [Test]
        public void Suite_result_is_derived_from_individual_results()
        {
            var runtime = new GeneratedScenarioBase
            {
                FileName = "my_scenario.tabula",
                ScenarioLabel = "Important behavior that must work"
            };

            string stepText = "irrelevant_fixture.dont_care(1, 2, 3)";

            int lineNumber = 88;
            string getLocation()
            {
                return $"my_scenario.tabula:{lineNumber++}";
            }

            NUnitReport.TestSuite suite = runtime.GetResults();
            Assert.AreEqual(NUnitTestResult.Passed, suite.Result);


            // Enough results to show numbers for all our categories/results
            runtime.Do(() => { }, getLocation(), stepText);
            runtime.Do(() => { }, getLocation(), stepText);

            suite = runtime.GetResults();
            Assert.AreEqual(NUnitTestResult.Passed, suite.Result);

            runtime.Do(() => { throw new NotImplementedException(); }, getLocation(), stepText);

            suite = runtime.GetResults();
            Assert.AreEqual(NUnitTestResult.Passed, suite.Result);

            runtime.Unfound(stepText, getLocation());

            suite = runtime.GetResults();
            Assert.AreEqual(NUnitTestResult.Failed, suite.Result);

            runtime.Do(() => { }, getLocation(), stepText);

            suite = runtime.GetResults();
            Assert.AreEqual(NUnitTestResult.Failed, suite.Result);
        }
    }
}
