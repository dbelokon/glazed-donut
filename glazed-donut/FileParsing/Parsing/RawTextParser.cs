using System;
using System.Collections.Generic;
using System.IO;

namespace GlazedDonut.FileParsing.Parsing
{
    internal class RawTextParser : Parser
    {
        public RawTextParser(FileStream stream)
            : base(stream)
        {
        }

        /// <inheritdoc/>
        public override string Parse()
        {
            IEnumerable<string> paragraphs = ExtractParagraphs();
            return ProduceHtmlString(paragraphs);
        }

        private IEnumerable<string> ExtractParagraphs()
        {
            List<string> paragraphs = new List<string>();
            string paragraph = string.Empty;

            foreach (string line in lines)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    if (string.IsNullOrWhiteSpace(paragraph))
                    {
                        paragraph += line;
                    }
                    else
                    {
                        paragraph += "\n" + line;
                    }
                }
                else if (!string.IsNullOrWhiteSpace(paragraph))
                {
                    paragraphs.Add(paragraph);
                    paragraph = string.Empty;
                }
            }

            if (!string.IsNullOrWhiteSpace(paragraph))
            {
                paragraphs.Add(paragraph);
            }

            return paragraphs;
        }

        private string ProduceHtmlString(IEnumerable<string> paragraphs)
        {
            string htmlString = string.Empty;

            foreach (var p in paragraphs)
            {
                htmlString += $"<p>{p.Replace("\n", " ")}</p>\n";
            }

            return htmlString;
        }
    }
}