using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GlazedDonut.InputFiles;

namespace GlazedDonut
{
    public class StaticSiteGenerator
    {
        // TODO: refactor method to reduce the number of arguments
        public static void Generate(string inputArgument, string outputDirectory, string stylesheetUrl, string language)
        {
            FileAttributes? attributes = GetFileAttributes(inputArgument);

            if (!attributes.HasValue)
            {
                return;
            }

            ISourceGenerator generator = ConstructGenerator(attributes.Value);

            generator.Generate(inputArgument, outputDirectory, stylesheetUrl, language);
        }

        private static FileAttributes? GetFileAttributes(string inputArgument)
        {
            if (inputArgument == null)
            {
                Console.WriteLine("A file name needs to be provided.");
                return null;
            }

            FileAttributes? attr = null;
            try
            {
                attr = File.GetAttributes(inputArgument);
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"The file {inputArgument} does not exist or wasn't found.");
            }
            catch (DirectoryNotFoundException)
            {
                Console.WriteLine($"The directory {inputArgument} does not exist or wasn't found. ");
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine($"The file or directory cannot be read. There is no read permission for {inputArgument} file or directory. ");
            }
            catch (Exception e)
            {
                Console.WriteLine($"There was an error. Please check the message below for more details: ");
                Console.WriteLine(e.Message);
            }

            return attr;
        }

        private static ISourceGenerator ConstructGenerator(FileAttributes inputFileAttributes)
        {
            if (inputFileAttributes.HasFlag(FileAttributes.Directory))
            {
                return new DirectoryHandler();
            }

            return new FileHandler();
        }
    }
}