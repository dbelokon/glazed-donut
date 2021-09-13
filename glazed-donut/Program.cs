using System;
using System.Collections.Generic;
using System.IO;
using CommandLine;

namespace glazed_donut
{
    class Program
    {
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
            var parser = new Parser(with =>
            {
                with.AutoHelp = false;
                with.AutoVersion = false;
            });

            var result = parser.ParseArguments<Options>(args);

            result.WithParsed(o =>
            { 
                if (o.Version)
                {
                    Console.WriteLine("This line should print the version.");
                }
                else if (o.Help)
                {
                    Console.WriteLine("This line should print the help.");
                }
                else if (!string.IsNullOrWhiteSpace(o.Input))
                {
                    Console.WriteLine("This is the argument passed: {0}", o.Input);
                }
            }).WithNotParsed(err => { });
        }

        static void main()
        {
            // Validate file name STEP

            const string fileName = "halyo.txt";
            FileStream openedFile = null; 

            if (!fileName.EndsWith(".txt"))
            {
                Console.WriteLine($"The file {fileName} does not have the correct extension. It has to be a text file, please make sure that the file name ends with .txt.");

                Environment.Exit(1);
            }

            // Open input file and validating it STEP

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

            // Prepare the paragraphs from the output file STEP

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

            // Create the HTML text from the paragraphs STEP

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

            // create distribution folder and output file STEP
            string folderName = "dist";

            if (Directory.Exists(folderName))
            {
                Directory.Delete(folderName, true);
            }

            DirectoryInfo dirInfo = Directory.CreateDirectory(folderName);

            File.WriteAllText(Path.Combine(dirInfo.Name, "halyo.html"), htmlText);
        }
    }
}
