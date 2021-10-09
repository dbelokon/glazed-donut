using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using CommandLine;
using CommandLine.Text;
using Newtonsoft.Json.Linq;

namespace glazed_donut
{
    
    class Program
    {
        const string VERSION = "0.1";
        static readonly string VERSION_STRING = $"Glazed Donut {VERSION}";

        static Parser parser = new Parser(with =>
        {
            with.HelpWriter = null;
            with.AutoHelp = false;
            with.AutoVersion = false;
        });

        class Options
        {
            [Option('c', "config", HelpText = "Supports --config for config.json.")]
            public string OutputConfigDirectory { get; set; }

            [Option('v', "version", HelpText = "Displays the version of the software.")]
            public bool Version { get; set; }
            
            [Option('h', "help", HelpText = "Displays this helpful message.")]
            public bool Help { get; set; } 

            [Option('i', "input", HelpText = "Specifies the file name or folder name that it should use to convert from.")]
            public string Input { get; set; }

            [Option('o', "output", HelpText = "Specifies the folder name that contains the generated HTML files.", Default = "./dist")]
            public string OutputDirectory { get; set; }

            [Option('s', "stylesheet", HelpText = "Accepts a URL to a CSS stylesheet to style the generated HTML files.")]
            public string StylesheetURL { get; set; }

            [Option('l', "lang", HelpText = "Acepts a language tag to mark the HTML document.", Default = "en-CA")]
            public string Lang { get; set; }
        }

        static void DisplayHelp<T>(ParserResult<T> result)
        {
            var helpText = HelpText.AutoBuild(result, h =>
            {
                h.AdditionalNewLineAfterOption = true;
                h.Heading = VERSION_STRING;
                h.Copyright = "Copyright (c) 2021 Diana B.";
                return HelpText.DefaultParsingErrorsHandler(result, h);
            }, e => e);
            
            helpText.AutoVersion = false;
            helpText.AutoHelp = false;
            helpText.AddOptions(result);

            Console.WriteLine(helpText);
        }

        static void Main(string[] args)
        {
            var result = parser.ParseArguments<Options>(args); 
            
            string inputArgument = null;
            string outputDirectory = null;
            string stylesheetUrl = null;
            string language = null;
            string outputConfigDirectory = null;

            result.WithParsed(o => 
            {
                if (o.Version)
                {
                    Console.WriteLine(VERSION_STRING);
                    Environment.Exit(0);
                }
                else if (o.Help)
                {
                    DisplayHelp(result);
                    Environment.Exit(0);
                }
                else if (!string.IsNullOrEmpty(o.OutputConfigDirectory)) {
                    // parse the JSON
                    string myJsonString = File.ReadAllText(o.OutputConfigDirectory); 
                    JObject jObject = JObject.Parse(myJsonString);
                    foreach (KeyValuePair<string, JToken> keyValuePair in jObject)
                    {
                        switch (keyValuePair.Key) {
                            case "input":
                                o.Input = (string)keyValuePair.Value;
                                break;
                            case "output":
                                o.OutputDirectory = (string)keyValuePair.Value;
                                break;
                            case "stylesheet":
                                o.StylesheetURL = (string)keyValuePair.Value;
                                break;
                            case "lang":
                                o.Lang = (string)keyValuePair.Value;
                                break;
                        }
                    }

                }
                if (!string.IsNullOrWhiteSpace(o.Input))
                {
                    inputArgument = o.Input;
                    outputDirectory = o.OutputDirectory;
                    language = o.Lang;
                    if (!string.IsNullOrWhiteSpace(o.StylesheetURL))
                    {
                        stylesheetUrl = o.StylesheetURL;
                    }
                }
            }).WithNotParsed(err => 
            { 
                DisplayHelp(result);
                Environment.Exit(1);
            });


            if (inputArgument == null)
            {
                Console.WriteLine("A file name needs to be provided.");
                Environment.Exit(1);
            }

            FileAttributes? attr = null;
            try
            {            
                attr = File.GetAttributes(inputArgument);
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"The file {inputArgument} does not exist or wasn't found.");
                Environment.Exit(1);
            }
            catch (DirectoryNotFoundException)
            {
                Console.WriteLine($"The directory {inputArgument} does not exist or wasn't found. ");
                Environment.Exit(1);
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine($"The file or directory cannot be authorized. There is no read permission for {inputArgument} file or directory. ");
                Environment.Exit(1);
            }
            catch (Exception e)
            {
                Console.WriteLine($"There was an error. Please check the message below for more details: ");
                Console.WriteLine(e.Message);
                Environment.Exit(1);
            }
            
            if (attr.Value.HasFlag(FileAttributes.Directory))
            {
                mainDirectoryCase(inputArgument, outputDirectory, stylesheetUrl, language);
            }
            else
            {
                mainSingleFileCase(inputArgument, outputDirectory, stylesheetUrl, language);
            }
        }

        
        static bool IsFileNameValid
            (string fileName)
        {
            //only process file if the file type is text or markdown
            return fileName.EndsWith(".txt") || fileName.EndsWith(".md") || fileName.EndsWith(".json");
        }

        static bool isMarkDown
            (string fileName)
        {
            return fileName.EndsWith(".md");
        }

        static void mainDirectoryCase(string directoryName, string outputDirectory, string stylesheetURL, string language)
        {

            if (!Directory.Exists(directoryName))
            {
                Console.WriteLine($"The directory {directoryName} does not exist or could not be found. Please, make sure that the directory that you specified exists.");
                Environment.Exit(1);
            }

            IEnumerable<string> fileNames = null;

            try
            {
                fileNames = Directory.EnumerateFiles(directoryName, "*.txt");
            } 
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine($"The directory {directoryName} cannot be accessed.");
                Environment.Exit(1);
            }
            catch (Exception e)
            {
                Console.WriteLine($"There was an error when accessing the folder {directoryName}. See details below.");
                Console.WriteLine(e.Message);
                Environment.Exit(1);
            }

            DirectoryInfo dirInfo = null;

            try
            {
                dirInfo = CreateDirectory(outputDirectory);
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine($"The directory '{outputDirectory}' cannot be accessed, due to lack of reading permission. Try using another directory.");
                Environment.Exit(1);
            }
            catch (Exception e)
            {
                Console.WriteLine($"The directory '{outputDirectory}' cannot be accessed, due to an unknown error. See details below.");
                Console.WriteLine(e.Message);
                Environment.Exit(1);
            }
            

            foreach (string fileName in fileNames)
            {
                FileStream openedFile;

                try
                {
                    openedFile = File.Open(fileName, FileMode.Open);
                }
                catch (FileNotFoundException)
                {
                    Console.WriteLine($"Oops... The file {fileName} does not exist. Please create a file or check if the file path was correct.");
                    continue;
                }
                catch (UnauthorizedAccessException e)
                {
                    Console.WriteLine($"The file {fileName} cannot be accessed. Please read the following error message for details:");
                    Console.WriteLine(e.Message);
                    continue;
                }
                catch (Exception e)
                {
                    Console.WriteLine("An unknown error occurred. Please check the following error message for more details: ");
                    Console.WriteLine(e.Message);
                    continue;
                }

                List<string> paragraphs = ExtractParagraphs(openedFile);
                string htmlText = GenerateHtmlPage(paragraphs, stylesheetURL, language);
                InsertFileInDirectory(dirInfo, fileName, htmlText);
            }
        }

        static void mainSingleFileCase(string fileName, string outputDirectory, string stylesheetURL, string language)
        {
            FileStream openedFile = null;

            if (!IsFileNameValid(fileName))
            {
                Console.WriteLine($"The file {fileName} does not have the correct extension. It has to be a text file, please make sure that the file name ends with .txt.");

                Environment.Exit(1);
            }


            try
            {
                openedFile = File.Open(fileName, FileMode.Open);
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"Oops... The file {fileName} does not exist. Please create a file or check if the file path was correct.");
                Environment.Exit(1);
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine($"The file {fileName} cannot be accessed. Please read the following error message for details:");
                Console.WriteLine(e.Message);
                Environment.Exit(1);
            }
            catch (Exception e)
            {
                Console.WriteLine("An unknown error occurred. Please check the following error message for more details: ");
                Console.WriteLine(e.Message);
                Environment.Exit(1);
            }

            if (openedFile.Length == 0)
            {
                Console.WriteLine($"The file {fileName} does not contain any data. The output file might be empty.");
            }

            


            string htmlText;

            //check the file type
            if (isMarkDown(fileName))
            {
                List<string> paragraphs = ExtractParagraphsForMarkdown(openedFile);
                htmlText = GenerateHtmlPageForMarkdown(paragraphs, stylesheetURL); 
            }
            else
            {
                List<string> paragraphs = ExtractParagraphs(openedFile);
                htmlText = GenerateHtmlPage(paragraphs, stylesheetURL, language);
            }

            DirectoryInfo dirInfo = null;

            try
            {
                dirInfo = CreateDirectory(outputDirectory);
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine($"The directory '{outputDirectory}' cannot be accessed, due to lack of reading permission. Try using another directory.");
                Environment.Exit(1);
            }
            catch (Exception e)
            {
                Console.WriteLine($"The directory '{outputDirectory}' cannot be accessed, due to an unknown error. See details below.");
                Console.WriteLine(e.Message);
                Environment.Exit(1);
            }

            InsertFileInDirectory(dirInfo, fileName, htmlText);
        }

        private static void InsertFileInDirectory(DirectoryInfo dirInfo, string originalFileName, string htmlText)
        {
            string newFileName = Path.GetFileNameWithoutExtension(originalFileName) + ".html";

            File.WriteAllText(Path.Combine(dirInfo.FullName, newFileName), htmlText);
        }

        private static DirectoryInfo CreateDirectory(string directoryName)
        {
            if (Directory.Exists(directoryName))
            {
                Directory.Delete(directoryName, true);
            }

            DirectoryInfo dirInfo = Directory.CreateDirectory(directoryName);
            return dirInfo;
        }

        private static string GenerateHtmlPage(List<string> paragraphs, string stylesheetURL, string language)
        {
            string htmlBody = "";

            foreach (var p in paragraphs)
            {
                htmlBody += $"<p>{p.Replace("\n", " ")}</p>\n";
            }

            string htmlText = $@"<!doctype html>
<html lang=""{($"{language}")}"">
 <head>
  <meta charset=""utf-8"">
  <title>Filename</title>
  {(string.IsNullOrWhiteSpace(stylesheetURL) ? "" : $"<link rel=\"stylesheet\" href=\"{stylesheetURL}\">")}
  <meta name=""viewport"" content=""width=device-width, initial-scale=1"">
 </head>
 <body>
    {htmlBody}
 </body>
 </html>";
            return htmlText;
        }

        private static string GenerateHtmlPageForMarkdown(List<string> paragraphs, string stylesheetURL)
        {
            string htmlBody = "";

            foreach (var p in paragraphs)
            {
                if (IsHeading(p))
                {
                    htmlBody += $"<h1>{p.Replace("\n", " ").Replace("#", "")}</h1>\n";
                }
                else if (IsHorizontalLine(p))
                {
                    htmlBody += $"<hr>";
                }
                else
                {
                    htmlBody += $"<p>{p.Replace("\n", " ")}</p>\n";
                }
            }

            string htmlText = $@"<!doctype html>
<html lang=""en"">
 <head>
  <meta charset=""utf-8"">
  <title>Filename</title>
  {(string.IsNullOrWhiteSpace(stylesheetURL) ? "" : $"<link rel=\"stylesheet\" href=\"{stylesheetURL}\">")}
  <meta name=""viewport"" content=""width=device-width, initial-scale=1"">
 </head>
 <body>
    {htmlBody}
 </body>
 </html>";
            return htmlText;
        }

        private static List<string> ExtractParagraphs(FileStream openedFile)
        {
            StreamReader fileReader = new StreamReader(openedFile);

            string line;
            string paragraph = "";
            List<string> paragraphs = new List<string>();

            while ((line = fileReader.ReadLine()) != null)
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
                    paragraph = "";
                }
            }

            if (!string.IsNullOrWhiteSpace(paragraph))
            {
                paragraphs.Add(paragraph);
            }

            return paragraphs;
        }

        private static List<string> ExtractParagraphsForMarkdown(FileStream openedFile)
        {
            StreamReader fileReader = new StreamReader(openedFile);

            string line;
            string paragraph = "";
            List<string> paragraphs = new List<string>();

            while ((line = fileReader.ReadLine()) != null)
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

        //check the line if it is heading
        private static bool IsHeading(string line)
        {
            return line.StartsWith("# ");
        }

        private static bool IsHorizontalLine(string line)
        {
            Regex regex = new Regex(@"^\s*---+\s*$");
            return regex.Match(line).Success;
        }
    }
}
