﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace glazed_donut.FileParsing.Parsing
{
    class MarkdownParser : Parser
    {
        public MarkdownParser(FileStream stream) : base(stream) { }

        public override string Parse()
        {
            IEnumerable<string> paragraphs = ExtractParagraphs();
            return ProduceHtmlString(paragraphs);
        }

        private IEnumerable<string> ExtractParagraphs()
        {
            List<string> paragraphs = new List<string>();
            string paragraph = "";            

            foreach (string line in lines)
            {
                if (IsHorizontalLine(line))
                {
                    if (!string.IsNullOrWhiteSpace(paragraph))
                    {
                        paragraphs.Add(paragraph);
                    }
                    paragraphs.Add(line);
                    paragraph = "";
                }
                else if (!string.IsNullOrWhiteSpace(line))
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
                    paragraph = "";
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
            string htmlString = "";

            foreach (var p in paragraphs)
            {
                if (IsHeading(p))
                {
                    htmlString += $"<h1>{p.Replace("\n", " ").Replace("#", "")}</h1>\n";
                }
                else if (IsHorizontalLine(p))
                {
                    htmlString += $"<hr>";
                }
                else
                {
                    htmlString += $"<p>{p.Replace("\n", " ")}</p>\n";
                }
            }

            return htmlString;
        }

        private bool IsHeading(string line)
        {
            return line.StartsWith("# ");
        }

        private bool IsHorizontalLine(string line)
        {
            Regex regex = new Regex(@"^\s*---+\s*$");
            return regex.Match(line).Success;
        }
    }
}
