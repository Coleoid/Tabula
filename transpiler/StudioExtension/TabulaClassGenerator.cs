using System;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.Shell;
using VSLangProj80;
using Microsoft.VisualStudio.TextTemplating.VSHost;

namespace Tabula
{
    /// <summary>
    /// This is the generator class. 
    /// Set the 'Custom Tool' property of a Tabula scenario to "TabulaClassGenerator". 
    /// The GenerateCode function will be called on scenario save, receiving the text of the scenario,
    /// and returning byte[] of the text of the generated class to the VS project system.
    /// </summary>
    [ComVisible(true)]
    [Guid("19401343-E3BA-4EF5-8C14-E6F3D6AD1441")]
    [CodeGeneratorRegistration(
        typeof(TabulaClassGenerator),
        "Generate C# of Tabula scenarios",
        vsContextGuids.vsContextGuidVCSProject,
        GeneratesDesignTimeSource = true,
        GeneratorRegKeyName = "TabulaClassGenerator"
    )]
    [ProvideCodeGeneratorExtension("TabulaClassGenerator",".tab")]
    [ProvideObject(typeof(TabulaClassGenerator), RegisterUsing = RegistrationMethod.CodeBase)]
    public class TabulaClassGenerator : BaseCodeGenerator_WithOLESite
    {
        internal static string name = "TabulaClassGenerator";  //  for 'Custom Tool' property of project item

        /// <summary>
        /// Connector between VS Custom Tool extensibility point and the Tabula transpiler
        /// </summary>
        /// <param name="inputFileContent">Content of the input file</param>
        /// <returns>Generated file as a byte array</returns>
        protected override byte[] GenerateCode(string inputFileContent)
        {
            try
            {
                ThreadHelper.ThrowIfNotOnUIThread();  //  Really, Microsoft?
                CodeGeneratorProgress?.Progress(50, 100);

                string bodyText;
                var builder = new StringBuilder();
                using (StringWriter writer = new StringWriter(builder))
                {
                    GenerateText(inputFileContent, builder);
                    writer.Flush();
                    CodeGeneratorProgress?.Progress(90, 100);

                    bodyText = writer.ToString();
                }

                return Encoding.UTF8.GetBytes(bodyText);
            }
            catch (Exception e)
            {
                this.GeneratorError(4, e.ToString(), 1, 1);
                return null;
            }
            finally
            {
                CodeGeneratorProgress?.Progress(100, 100);
            }
        }

        internal void GenerateText(string scenarioText, StringBuilder builder)
        {
            var transpiler = new Transpiler();
            transpiler.Transpile(InputFilePath, scenarioText, builder);
        }
    }
}
