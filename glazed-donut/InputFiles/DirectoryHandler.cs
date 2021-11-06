using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GlazedDonut.FileParsing;

namespace GlazedDonut.InputFiles
{
    public class DirectoryHandler : ISourceGenerator
    {
        /// <inheritdoc/>
        public void Generate(string inputArgument, string outputDirectory, string stylesheetUrl, string language)
        {
            if (!Directory.Exists(inputArgument))
            {
                Console.WriteLine($"The directory {inputArgument} does not exist or could not be found. Please, make sure that the directory that you specified exists.");
                Environment.Exit(1);
            }

            IEnumerable<string> fileNames = null;

            try
            {
                fileNames = Directory.EnumerateFiles(inputArgument, "*.txt");
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine($"The directory {inputArgument} cannot be accessed.");
                Environment.Exit(1);
            }
            catch (Exception e)
            {
                Console.WriteLine($"There was an error when accessing the folder {inputArgument}. See details below.");
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
                InputFile file = InputFile.AccessInputFile(fileName, stylesheetUrl, language);
                InsertFileInDirectory(dirInfo, fileName, file.EmitHtmlPage());
            }
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
    }
}