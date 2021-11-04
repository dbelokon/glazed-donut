using System;
using System.Collections.Generic;
using System.Text;

namespace GlazedDonut.InputFiles
{
    public interface ISourceGenerator
    {
        void Generate(string inputArgument, string outputDirectory, string stylesheetUrl, string language);
    }
}