using System;
using System.Collections.Generic;
using System.Linq;

namespace Tabula
{

    public class AssertionException : Exception
    {
        public AssertionException( string message )
            : base(message)
        { }
    }


    public class ScenarioContext
    {
        public void Push(Row row)
        {}

        public void Pop()
        {}
    }

    public class ValueContext
    {
        public Dictionary<string,string> values = new Dictionary<string,string>();
        public string this[string key]
        {
            get { return values[key]; }
            set { values[key] = value; }
        }
    }

    public enum ActionOutcome {
        Unknown = 0,
        Passed,
        Failed,
        Skipped,
        Unfound,
    }

    public class RunResult
    {
        private string _sourceLocation;
        public string SourceLocation
        {
            get { return _sourceLocation; }
            set
            {
                _sourceLocation = value;
                SourceFile = value.Split(':')[0];
                LineNumber = value.Split(':')[1];
            }
        }
        public string SourceFile { get; private set; }
        public string LineNumber { get; private set; }

        public string Message { get; set; }
        public ActionOutcome Outcome { get; set; }
        public TimeSpan TimeElapsed { get; set; }
        public List<string> StackTrace { get; set; }
        public string ActionText { get; set; }

        public void SetResult(TimeSpan elapsed, Exception ex = null)
        {
            if (ex == null)
            {
                Message = string.Empty;
                Outcome = ActionOutcome.Passed;
            }
            else if (ex is NotImplementedException)
            {
                Message = "Step is not implemented";
                Outcome = ActionOutcome.Skipped;
            }
            else
            {
                Message = ex.Message;
                Outcome = ActionOutcome.Failed;
            }

            TimeElapsed = elapsed;
        }

        public void Skip(string reason = "Skipped due to earlier error")
        {
            Message = reason;
            Outcome = ActionOutcome.Skipped;
            TimeElapsed = TimeSpan.Zero;
        }

        public override string ToString()
        {
            return string.Format("{0,4} {1,8}:  {2}    {3}", LineNumber, Outcome, ActionText, Message);
        }
    }

}
