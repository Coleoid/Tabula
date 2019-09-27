using NUnit.Framework;
using Tabula.API;

namespace Tabula
{
    [TestFixture]
    public class ScopeTests
    {
        [Test]
        public void Scope_single_level()
        {
            var scope = new Scope(parent: null);
            scope["freddy"] = "Fred Jones";
            scope["my_friend"] = "freddy";

            Assert.That(scope["my_friend"], Is.EqualTo("freddy"));
        }

        [Test]
        public void Scope_two_levels_from_stored_number_sign()
        {
            var scope = new Scope(parent: null);
            scope["freddy"] = "Fred Jones";
            scope["MY_friend"] = "#freddy";

            Assert.That(scope["my_FRIEND"], Is.EqualTo("Fred Jones"));
        }

        [Test]
        public void Scope_two_levels_from_request_with_number_sign()
        {
            var scope = new Scope(parent: null);
            scope["freddy"] = "Fred Jones";
            scope["MY_friend"] = "Freddy";

            Assert.That(scope["#my_FRIEND"], Is.EqualTo("Fred Jones"));
        }

        [Test]
        public void Scope_three_levels()
        {
            var scope = new Scope(parent: null);
            scope["freddy"] = "Fred Jones";
            scope["MY_friend"] = "#freddy";
            scope["guy"] = "my_friend";

            Assert.That(scope["#guy"], Is.EqualTo("Fred Jones"));
        }

        [Test]
        public void Unfound_returns_Empty()
        {
            var scope = new Scope(parent: null);

            Assert.That(scope["#dude"], Is.EqualTo(string.Empty));
            Assert.That(scope["dude"], Is.EqualTo(string.Empty));
        }

        [TestCase(0.0, "two", "mine")]
        [TestCase(1.0, "two", "two")]
        [TestCase(1.0, "TWO", "two")]
        [TestCase(0.5, "Age", "name")]
        public void EditPercentage_returns_ratio_of_unchanged_characters_over_longest_length(double expected, string goal, string candidate)
        {
            double result = Scope.EditPercentage(goal, candidate);
            Assert.That(result, Is.EqualTo(expected));
        }

        [TestCase(1.0, "thunder", "under")]
        [TestCase(1.0, "two", "two")]
        [TestCase(.8, "frien", "frieend")]
        [TestCase(0.5, "ge", "name")]
        public void SubstringPercentage_returns_ratio_of_LCS_over_shortest_length(double expected, string goal, string candidate)
        {
            double result = Scope.SubstringPercentage(goal, candidate);
            Assert.That(result, Is.EqualTo(expected));
        }

        [TestCase(true, "two", "two")]
        [TestCase(true, "thunder", "under")]
        [TestCase(true, "frien", "frieend")]
        [TestCase(true, "friendName", "name")]
        [TestCase(false, "ge", "name")]
        [TestCase(false, "frieendName", "age")]
        [TestCase(false, "name", "age")]
        public void MatchFitness_returns_closeness_to_a_match(bool closeEnough, string goal, string candidate)
        {
            double result = Scope.MatchFitness(goal, candidate);
            Assert.That(result > .5, Is.EqualTo(closeEnough));
        }
    }
}
