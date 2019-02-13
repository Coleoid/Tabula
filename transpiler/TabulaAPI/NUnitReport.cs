using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Testopia.ReportGenerators
{
    [Serializable]
    [XmlRoot(ElementName = "test-run")]
    public class NUnitReport 
    {
        [XmlElement(ElementName = "test-suite")]
        public List<TestSuite> TestSuites { get; set; }

        [XmlAttribute(AttributeName = "result")]
        public NUnitTestResult Result { get; set; }

        [XmlAttribute(AttributeName = "testcasecount")]
        public int TestCaseCount
        {
            get => TestSuites.Sum(s => s.TestCaseCount);
            set => throw new NotImplementedException();
        }

        [XmlAttribute(AttributeName = "total")]
        public int TotalTests
        {
            get => TestSuites.Sum(s => s.TotalTests);
            set => throw new NotImplementedException();
        }

        [XmlAttribute(AttributeName = "passed")]
        public int PassedTests
        {
            get => TestSuites.Sum(s => s.PassedTests);
            set => throw new NotImplementedException();
        }

        [XmlAttribute(AttributeName = "failed")]
        public int FailedTests
        {
            get => TestSuites.Sum(s => s.FailedTests);
            set => throw new NotImplementedException();
        }

        [XmlAttribute(AttributeName = "inconclusive")]
        public int InconclusiveTests
        {
            get => TestSuites.Sum(s => s.InconclusiveTests);
            set => throw new NotImplementedException();
        }

        [XmlAttribute(AttributeName = "skipped")]
        public int SkippedTests
        {
            get => TestSuites.Sum(s => s.SkippedTests);
            set => throw new NotImplementedException();
        }

        public NUnitReport()
        {
            TestSuites = new List<TestSuite>();
        }

        [Serializable]
        public class TestSuite
        {
            [XmlAttribute(AttributeName = "type")]
            public NUnitTestSuiteType Type { get; set; }

            [XmlAttribute(AttributeName = "id")]
            public string Id { get; set; }

            [XmlAttribute(AttributeName = "name")]
            public string Name { get; set; }

            [XmlAttribute(AttributeName = "fullname")]
            public string FullName { get; set; }

            [XmlAttribute(AttributeName = "classname")]
            public string ClassName { get; set; }

            [XmlAttribute(AttributeName = "testcasecount")]
            public int TestCaseCount { get; set; }

            [XmlAttribute(AttributeName = "result")]
            public NUnitTestResult Result { get; set; }

            [XmlAttribute(AttributeName = "total")]
            public int TotalTests { get; set; }

            [XmlAttribute(AttributeName = "passed")]
            public int PassedTests { get; set; }

            [XmlAttribute(AttributeName = "failed")]
            public int FailedTests { get; set; }

            [XmlAttribute(AttributeName = "inconclusive")]
            public int InconclusiveTests { get; set; }

            [XmlAttribute(AttributeName = "skipped")]
            public int SkippedTests { get; set; }

            [XmlElement(ElementName = "test-suite")]
            public List<TestSuite> TestSuites { get; set; }

            [XmlElement(ElementName = "test-case")]
            public List<TestCase> TestCases { get; set; }

            public TestSuite()
            {
                Result = NUnitTestResult.Passed;
                TestSuites = new List<TestSuite>();
                TestCases = new List<TestCase>();
            }
        }

        [Serializable]
        public class TestCase
        {
            [XmlAttribute(AttributeName = "id")]
            public string Id { get; set; }

            [XmlAttribute(AttributeName = "name")]
            public string Name { get; set; }

            [XmlAttribute(AttributeName = "fullname")]
            public string FullName { get; set; }

            [XmlAttribute(AttributeName = "result")]
            public NUnitTestResult Result { get; set; }

            [XmlElement(ElementName = "failure")]
            public TestCaseFailure FailureInfo { get; set; }
        }

        [Serializable]
        public class TestCaseFailure
        {
            [XmlElement(ElementName = "message")]
            public string Message { get; set; }

            [XmlElement(ElementName = "stack-trace")]
            public string StackTrace { get; set; }
        }
    }

    public enum NUnitTestResult
    {
        Passed,
        Failed,
        Inconclusive,
        Skipped
    }

    public enum NUnitTestSuiteType
    {
        Assembly,
        TestSuite,
        TestFixture
    }
}
