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
            var scope = new Scope();
            scope["freddy"] = "Fred Jones";
            scope["my_friend"] = "freddy";

            Assert.That(scope["my_friend"], Is.EqualTo("freddy"));
        }

        [Test]
        public void Scope_two_levels_from_stored_number_sign()
        {
            var scope = new Scope();
            scope["freddy"] = "Fred Jones";
            scope["MY_friend"] = "#freddy";

            Assert.That(scope["my_FRIEND"], Is.EqualTo("Fred Jones"));
        }

        [Test]
        public void Scope_two_levels_from_request_with_number_sign()
        {
            var scope = new Scope();
            scope["freddy"] = "Fred Jones";
            scope["MY_friend"] = "Freddy";

            Assert.That(scope["#my_FRIEND"], Is.EqualTo("Fred Jones"));
        }

        [Test]
        public void Scope_three_levels()
        {
            var scope = new Scope();
            scope["freddy"] = "Fred Jones";
            scope["MY_friend"] = "#freddy";
            scope["guy"] = "my_friend";

            Assert.That(scope["#guy"], Is.EqualTo("Fred Jones"));
        }

        [Test]
        public void Unfound_returns_Empty()
        {
            var scope = new Scope();

            Assert.That(scope["#dude"], Is.EqualTo(string.Empty));
            Assert.That(scope["dude"], Is.EqualTo(string.Empty));
        }

    }
}
