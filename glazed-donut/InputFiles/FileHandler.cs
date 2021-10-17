using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using glazed_donut.FileParsing;

namespace glazed_donut.InputFiles
{
    public class FileHandler : ISourceGenerator
    {
        public void Generate(string inputArgument, string outputDirectory, string stylesheetUrl, string language)
        {
            InputFile file = InputFile.AccessInputFile(inputArgument, stylesheetUrl, language);

            if (file == null)
            {
                return;
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

            InsertFileInDirectory(dirInfo, inputArgument, file.EmitHtmlPage());
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

        private static void InsertFileInDirectory(DirectoryInfo dirInfo, string originalFileName, string htmlText)
        {
            string newFileName = Path.GetFileNameWithoutExtension(originalFileName) + ".html";

            File.WriteAllText(Path.Combine(dirInfo.FullName, newFileName), htmlText);
        }
    }
}
