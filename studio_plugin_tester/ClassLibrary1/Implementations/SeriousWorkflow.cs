using NUnit.Framework;
using System.Collections.Generic;

namespace LibContainingScenarios.Implementations
{
    public class SeriousWorkflow : Workflow
    {
        private List<string> _seriousTopics;

        public SeriousWorkflow()
        {
            _seriousTopics = new List<string>();
        }

        public void Register__as_serious(string topic)
        {
            _seriousTopics.Add(topic);
        }

        public void Verify_that__is_serious(string topic)
        {
            Assert.That(_seriousTopics, Contains.Item(topic));
        }

        public void Verify_that__is_not_serious(string topic)
        {
            Assert.That(_seriousTopics, !Contains.Item(topic));
        }

        public void Remove__from_our_topics(string topic)
        {
            _seriousTopics.Remove(topic);
        }
    }
}
