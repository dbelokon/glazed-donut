using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GlazedDonut.FileParsing.Parsing;

namespace GlazedDonut.FileParsing
{
    internal class InputFile
    {
        private Parser parser;

        private InputFile(string fileName, string stylesheetUrl, string languageTag, Parser parser)
        {
            this.FileName = fileName;
            this.PageName = Path.GetFileNameWithoutExtension(fileName);
            this.StylesheetUrl = stylesheetUrl;
            this.LanguageTag = languageTag;
            this.parser = parser;
        }

        public string FileName { get; }

        public string PageName { get; }

        public string StylesheetUrl { get; }

        public string LanguageTag { get; }

        public static InputFile AccessInputFile(string inputFileName, string stylesheetUrl, string languageTag)
        {
            if (!IsFileNameValid(inputFileName))
            {
                Console.WriteLine($"The file {inputFileName} does not have the correct extension. It has to be a text file, please make sure that the file name ends with .txt.");

                return null;
            }

            try
            {
                FileStream openedFile = File.Open(inputFileName, FileMode.Open);

                if (openedFile.Length == 0)
                {
                    Console.WriteLine($"The file {inputFileName} does not contain any data. The output file might be empty.");
                }

                Parser parser = Parser.GetParser(inputFileName, openedFile);
                return new InputFile(inputFileName, stylesheetUrl, languageTag, parser);
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"Oops... The file {inputFileName} does not exist. Please create a file or check if the file path was correct.");
                return null;
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine($"The file {inputFileName} cannot be accessed. Please read the following error message for details:");
                Console.WriteLine(e.Message);
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine("An unknown error occurred. Please check the following error message for more details: ");
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public string EmitHtmlPage()
        {
            string htmlBody = parser.Parse();

            string htmlText = $@"<!doctype html>
<html lang=""{$"{LanguageTag}"}"">
 <head>
  <meta charset=""utf-8"">
  <title>{PageName}</title>
  {(string.IsNullOrWhiteSpace(StylesheetUrl) ? string.Empty : $"<link rel=\"stylesheet\" href=\"{StylesheetUrl}\">")}
  <meta name=""viewport"" content=""width=device-width, initial-scale=1"">
 </head>
 <body>
    {htmlBody}
 </body>
 </html>";

            return htmlText;
        }

        private static bool IsFileNameValid(string fileName)
        {
            return fileName.EndsWith(".txt") || fileName.EndsWith(".md");
        }
    }
}