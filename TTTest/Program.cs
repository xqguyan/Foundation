using Microsoft.VisualStudio.TextTemplating;
using System;
using System.CodeDom.Compiler;
using System.IO;

namespace TTTest
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                ProcessTemplate(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.WriteLine("输出完成");
            Console.Read();
        }
        static void ProcessTemplate(string[] args)
        {
            string templateFileName = "TextTemplate.tt";
            CustomCmdLineHost host = new CustomCmdLineHost();
            Engine engine = new Engine();
            host.TemplateFileValue = templateFileName;
            //Read the text template.  
            string input = File.ReadAllText(templateFileName);
            //Transform the text template.  
            string output = engine.ProcessTemplate(input, host);
            string outputFileName = Path.GetFileNameWithoutExtension(templateFileName);
            outputFileName = Path.Combine(Path.GetDirectoryName(templateFileName), outputFileName);
            outputFileName = outputFileName + "1" + host.FileExtension;
            File.WriteAllText(outputFileName, output, host.FileEncoding);

            foreach (CompilerError error in host.Errors)
            {
                Console.WriteLine(error.ToString());
            }
        }
    }
}
