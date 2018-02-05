using NUnit.Framework;
using System;

namespace Tabula
{
    [TestFixture]
    public class IndentingStringBuilderTests : TranspilerUnitTestBase
    {
        [Test]
        public void Constructed_with_indent()
        {
            var isb = new IndentingStringBuilder(4);
            isb.Append("foo");
            var output = isb.ToString();

            Assert.That(output, Is.EqualTo("    foo"));
        }

        [Test]
        public void Only_indents_on_new_line()
        {
            var isb = new IndentingStringBuilder(4);
            isb.Append("foo");
            isb.Append("-diggity");
            var output = isb.ToString();

            Assert.That(output, Is.EqualTo("    foo-diggity"));
        }

        [Test]
        public void Change_indentation()
        {
            var isb = new IndentingStringBuilder(4);
            isb.Dedent();
            isb.AppendLine("foo");
            isb.Indent();
            isb.AppendLine("bar");
            var output = isb.ToString();

            Assert.That(output, Is.EqualTo("foo\r\n    bar\r\n"));
        }

        [Test]
        public void Change_indentation_width()
        {
            var isb = new IndentingStringBuilder(2, 2);
            isb.AppendLine("stuff");
            isb.Indent();
            isb.AppendLine("detail");
            isb.Dedent();
            isb.Dedent();
            isb.AppendLine("finale");
            var output = isb.ToString();

            Assert.That(output, Is.EqualTo("  stuff\r\n    detail\r\nfinale\r\n"));
        }

        [Test]
        public void Dedenting_past_zero_is_dev_error()
        {
            var isb = new IndentingStringBuilder(4, 8);

            var ex = Assert.Throws<Exception>(() => isb.Dedent());
            Assert.That(ex.Message, Contains.Substring("Negative indentation (-4, in this case)"));
        }
    }
}
