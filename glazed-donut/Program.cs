using System;
using System.Collections.Generic;
using System.IO;
using CommandLine;

namespace glazed_donut
{
    class Program
    {
        const string VERSION = "0.1";

        static Parser parser = new Parser(with =>
        {
            with.AutoHelp = false;
            with.AutoVersion = false;
        });

        class Options
        {
            [Option('v', "version", HelpText = "Displays the version of the program.")]
            public bool Version { get; set; }
            
            [Option('h', "help", HelpText = "Provides how to use the command with the options.")]
            public bool Help { get; set; }

            [Option('i', "input", HelpText = "Accepts a path to a single file or a directory to generate the static site files.")]
            public string Input { get; set; }
        }

        static void Main(string[] args)
        {
            var result = parser.ParseArguments<Options>(args);
            
            string inputArgument = null;

            // TODO: Cover WithNotParsed branch
            result.WithParsed(o => 
            { 
                if (o.Version)
                {                   
                    Console.WriteLine($"Glazed Donut {VERSION}.");
                    Environment.Exit(0);
                }
                else if (o.Help)
                {
                    Console.WriteLine("Command              Definition                                                          Usage");
                    Console.WriteLine("-v or --version      Displays the version of the software.                               type -v or --version");
                    Console.WriteLine("-h or --help         Displays this helpful message.                                      type -h or --help");
                    Console.WriteLine("-i or --input        Accepts a text file or a directory name to convert into HTML.       type -i or --input and then either a folder name or a text file name next to it.");
                    Environment.Exit(0);
                }
                else if (!string.IsNullOrWhiteSpace(o.Input))
                {
                    inputArgument = o.Input;
                }
            }).WithNotParsed(err => { });

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
                mainDirectoryCase(inputArgument);
            }
            else
            {
                mainSingleFileCase(inputArgument);
            }
        }

        static bool IsFileNameValid(string fileName)
        {
            return fileName.EndsWith(".txt");
        }

        static void mainDirectoryCase(string directoryName)
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

            DirectoryInfo dirInfo = CreateDirectory("dist");

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
                string htmlText = GenerateHtmlPage(paragraphs);
                InsertFileInDirectory(dirInfo, fileName, htmlText);
            }
        }

        static void mainSingleFileCase(string fileName)
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

            List<string> paragraphs = ExtractParagraphs(openedFile);


            string htmlText = GenerateHtmlPage(paragraphs);

            DirectoryInfo dirInfo = CreateDirectory("dist");
            InsertFileInDirectory(dirInfo, fileName, htmlText);
        }

        private static void InsertFileInDirectory(DirectoryInfo dirInfo, string originalFileName, string htmlText)
        {
            string newFileName = Path.GetFileNameWithoutExtension(originalFileName) + ".html";

            File.WriteAllText(Path.Combine(dirInfo.Name, newFileName), htmlText);
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

        private static string GenerateHtmlPage(List<string> paragraphs)
        {
            string htmlBody = "";

            foreach (var p in paragraphs)
            {
                htmlBody += $"<p>{p.Replace("\n", "<br/>")}</p>\n";
            }

            string htmlText = $@"<!doctype html>
<html lang=""en"">
 <head>
  <meta charset=""utf-8"">
  <title>Filename</title>
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
    }
}
