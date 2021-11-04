using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using CommandLine;
using CommandLine.Text;
using Newtonsoft.Json.Linq;

namespace GlazedDonut
{
    internal class Program
    {
        private const string VERSION = "0.1";
        private static readonly string VERSIONSTRING = $"Glazed Donut {VERSION}";

        private static Parser parser = new Parser(with =>
        {
            with.HelpWriter = null;
            with.AutoHelp = false;
            with.AutoVersion = false;
        });

        private static void DisplayHelp<T>(ParserResult<T> result)
        {
            var helpText = HelpText.AutoBuild(
                result,
                h =>
                {
                    h.AdditionalNewLineAfterOption = true;
                    h.Heading = VERSIONSTRING;
                    h.Copyright = "Copyright (c) 2021 Diana B.";
                    return HelpText.DefaultParsingErrorsHandler(result, h);
                },
                e => e);

            helpText.AutoVersion = false;
            helpText.AutoHelp = false;
            helpText.AddOptions(result);

            Console.WriteLine(helpText);
        }

        private static void Main(string[] args)
        {
            var result = parser.ParseArguments<Options>(args);

            string inputArgument = null;
            string outputDirectory = null;
            string stylesheetUrl = null;
            string language = null;

            result
                .WithParsed(o =>
            {
                if (o.Version)
                {
                    Console.WriteLine(VERSIONSTRING);
                    Environment.Exit(0);
                }
                else if (o.Help)
                {
                    DisplayHelp(result);
                    Environment.Exit(0);
                }
                else if (!string.IsNullOrEmpty(o.OutputConfigDirectory))
                {
                    // parse the JSON
                    string myJsonString = File.ReadAllText(o.OutputConfigDirectory);
                    JObject jObject = JObject.Parse(myJsonString);
                    foreach (KeyValuePair<string, JToken> keyValuePair in jObject)
                    {
                        switch (keyValuePair.Key)
                        {
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
            })
                .WithNotParsed(err =>
            {
                DisplayHelp(result);
                Environment.Exit(1);
            });

            StaticSiteGenerator.Generate(inputArgument, outputDirectory, stylesheetUrl, language);
        }

        private class Options
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
    }
}