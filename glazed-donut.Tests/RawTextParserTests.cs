using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;
using GlazedDonut.FileParsing.Parsing;

namespace GlazedDonut.Tests
{
    public class RawTextParserTests
    {
        [Fact]
        public void Parse_InputIsMemoryStream_ReturnHtmlString()
        {
            var str = "Hello, this is raw text.";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(str));
            var parser = new RawTextParser(stream);

            var htmlString = parser.Parse();

            Assert.Equal($"<p>{str}</p>\n", htmlString);
        }

        [Fact]
        public void Parse_InputIsEmpty_ReturnEmptyString()
        {
            var str = "";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(str));
            var parser = new RawTextParser(stream);

            var htmlString = parser.Parse();

            Assert.Equal("", htmlString);
        }

        [Fact]
        public void Constructor_InputIsNull_ShouldThrowArgumentNullException()
        {
            ArgumentNullException e = Assert.Throws<ArgumentNullException>(() => {
                var parser = new RawTextParser(null);
            });

            Assert.NotNull(e);
        }
    }
}
