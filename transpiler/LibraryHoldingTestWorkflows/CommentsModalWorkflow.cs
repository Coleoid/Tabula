using System.Collections.Generic;
using NUnit.Framework;

namespace LibraryHoldingTestWorkflows
{
    public class CommentsModalWorkflow : Workflow
    {
        private List<string> _comments = new List<string>();
        public void Add_comment__(string comment)
        {
            Assert.That(comment.Length, Is.GreaterThan(5), $"Comment [{comment}] is too short to add.");
            _comments.Add(comment);
        }

        public void Verify_comment__text_is__(int rowNum, string text)
        {
            Assert.That(rowNum, Is.GreaterThan(0), "Row numbers start at 1");
            Assert.That(rowNum, Is.LessThanOrEqualTo(_comments.Count), $"There are currently only {_comments.Count} comments.");
            int index = rowNum - 1;
            Assert.That(_comments[index], Is.EqualTo(text));
        }

        public void With_duty_locations__(List<string> orgNames)
        {
        }

    }
}
