using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GlazedDonut.FileParsing.Parsing
{
    public abstract class Parser
    {
        protected IEnumerable<string> lines;

        public Parser(FileStream stream)
        {
            var lines = new List<string>();

            StreamReader reader = new StreamReader(stream);

            string currentLine;

            while ((currentLine = reader.ReadLine()) != null)
            {
                lines.Add(currentLine);
            }

            this.lines = lines;
        }

        public static Parser GetParser(string fileName, FileStream stream)
        {
            if (IsFileRawTextFile(fileName))
            {
                return new RawTextParser(stream);
            }
            else if (IsFileMarkdownFile(fileName))
            {
                return new MarkdownParser(stream);
            }

            return null;
        }

        public abstract string Parse();

        private static bool IsFileRawTextFile(string fileName)
        {
            return fileName.EndsWith(".txt");
        }

        private static bool IsFileMarkdownFile(string fileName)
        {
            return fileName.EndsWith(".md");
        }
    }
}