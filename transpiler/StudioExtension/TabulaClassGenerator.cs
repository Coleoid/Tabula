using System;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.Shell;
using VSLangProj80;
using System.Text.RegularExpressions;

namespace Tabula
{
    /// <summary>
    /// This is the generator class. 
    /// Set the 'Custom Tool' property of a Tabula scenario to "TabulaClassGenerator". 
    /// The GenerateCode function will be called on scenario save, receiving the text of the scenario,
    /// and returning byte[] of the text of the generated class to the VS project system.
    /// </summary>
    [ComVisible(true)]
    [Guid("7D7984C8-7744-48B1-9FDF-A90B2C4159F5")]
    [CodeGeneratorRegistration(typeof(TabulaClassGenerator), "Generate C# of Tabula scenarios", vsContextGuids.vsContextGuidVCSProject, GeneratesDesignTimeSource = true)]
    [ProvideObject(typeof(TabulaClassGenerator))]
    public class TabulaClassGenerator : BaseCodeGeneratorWithSite
    {
#pragma warning disable 0414
        internal static string name = "TabulaClassGenerator";  //  for 'Custom Tool' property of project item
#pragma warning restore 0414

        /// <summary>
        /// Connector between VS Custom Tool extensibility point and the Tabula transpiler
        /// </summary>
        /// <param name="inputFileContent">Content of the input file</param>
        /// <returns>Generated file as a byte array</returns>
        protected override byte[] GenerateCode(string inputFileContent)
        {
            try
            {
                CodeGeneratorProgress?.Progress(50, 100);

                Encoding enc;
                string bodyText;
                var builder = new StringBuilder();
                using (StringWriter writer = new StringWriter(builder))
                {
                    GenerateText(inputFileContent, builder);
                    writer.Flush();
                    CodeGeneratorProgress?.Progress(90, 100);

                    enc = Encoding.GetEncoding(writer.Encoding.WindowsCodePage);
                    bodyText = writer.ToString();
                }

                //  The preamble provides the byte order mark.  'Round here, we use little-endian UTF16, stranger.
                byte[] preamble = enc.GetPreamble();
                byte[] body = enc.GetBytes(bodyText);

                byte[] whole = new byte[preamble.Length + body.Length];
                Array.Copy(preamble, 0, whole, 0, preamble.Length);
                Array.Copy(body, 0, whole, preamble.Length, body.Length);

                return whole;
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
            transpiler.Transpile("foo.tab", scenarioText, builder);
        }
    }
}
