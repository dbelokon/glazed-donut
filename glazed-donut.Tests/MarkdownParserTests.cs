using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GlazedDonut.FileParsing.Parsing;
using Xunit;

namespace glazed_donut.Tests
{
    public class MarkdownParserTests
    {
        [Fact]
        public void ThrowException_WhenStreamIsNull()
        {
            ArgumentNullException e = Assert.Throws<ArgumentNullException>(() => {
                var parser = new MarkdownParser(null);
            });

            Assert.NotNull(e);
        }

        [Fact]
        public void ReturnEmptyParagraph_WhenStreamIsEmpty()
        {
            var str = "";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(str));
            var parser = new MarkdownParser(stream);

            var htmlString = parser.Parse();

            Assert.Equal("", htmlString); 
        }

        [Theory, MemberData(nameof(ValidInputStringData))]
        public void ReturnHtmlString_WhenParagraphIsValid(string input, string expectedHtml)
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(input));
            var parser = new MarkdownParser(stream);

            var htmlString = parser.Parse();

            Assert.Equal(expectedHtml, htmlString);
        }

        public static IEnumerable<object[]> ValidInputStringData =>
            new List<object[]>
            { 
                new object[] { "Markdown", "<p>Markdown</p>\n" },
                new object[] { "# Markdown", "<h1>Markdown</h1>\n" },
                new object[] { "---", "<hr>" }
            };
    }
}
