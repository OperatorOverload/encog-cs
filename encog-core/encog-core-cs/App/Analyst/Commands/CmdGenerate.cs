using System;
using System.Collections.Generic;
using System.IO;
using Encog.App.Analyst.Script.Normalize;
using Encog.App.Analyst.Script.Prop;
using Encog.App.Analyst.Util;
using Encog.ML.Factory;
using Encog.Util.CSV;
using Encog.Util.Logging;
using Encog.Util.Simple;

namespace Encog.App.Analyst.Commands
{
    /// <summary>
    /// This command is used to generate the binary EGB file from a CSV file. The
    /// resulting file can be used for training.
    /// </summary>
    ///
    public class CmdGenerate : Cmd
    {
        /// <summary>
        /// The name of this command.
        /// </summary>
        ///
        public const String COMMAND_NAME = "GENERATE";

        /// <summary>
        /// Construct this generate command.
        /// </summary>
        ///
        /// <param name="analyst">The analyst to use.</param>
        public CmdGenerate(EncogAnalyst analyst) : base(analyst)
        {
        }

        /// <inheritdoc/>
        public override String Name
        {
            get { return COMMAND_NAME; }
        }

        /// <summary>
        /// Determine the ideal fields.
        /// </summary>
        ///
        /// <param name="headerList">The headers.</param>
        /// <returns>The indexes of the ideal fields.</returns>
        private int[] DetermineIdealFields(CSVHeaders headerList)
        {
            int[] result;
            String type = Prop.GetPropertyString(
                ScriptProperties.ML_CONFIG_TYPE);

            // is it non-supervised?
            if (type.Equals(MLMethodFactory.TYPE_SOM))
            {
                result = new int[0];
                return result;
            }

            IList<Int32> fields = new List<Int32>();

            for (int currentIndex = 0; currentIndex < headerList.Size(); currentIndex++)
            {
                String baseName = headerList.GetBaseHeader(currentIndex);
                int slice = headerList.GetSlice(currentIndex);
                AnalystField field = Analyst.Script
                    .FindNormalizedField(baseName, slice);

                if (field != null && field.Output)
                {
                    fields.Add(currentIndex);
                }
            }

            // allocate result array
            result = new int[fields.Count];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = (fields[i]);
            }

            return result;
        }

        /// <summary>
        /// Determine the input fields.
        /// </summary>
        ///
        /// <param name="headerList">The headers.</param>
        /// <returns>The indexes of the input fields.</returns>
        private int[] DetermineInputFields(CSVHeaders headerList)
        {
            IList<Int32> fields = new List<Int32>();

            for (int currentIndex = 0; currentIndex < headerList.Size(); currentIndex++)
            {
                String baseName = headerList.GetBaseHeader(currentIndex);
                int slice = headerList.GetSlice(currentIndex);
                AnalystField field = Analyst.Script
                    .FindNormalizedField(baseName, slice);

                if (field != null && field.Input)
                {
                    fields.Add(currentIndex);
                }
            }

            // allocate result array
            var result = new int[fields.Count];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = (fields[i]);
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        ///
        public override sealed bool ExecuteCommand(String args)
        {
            // get filenames
            String sourceID = Prop.GetPropertyString(
                ScriptProperties.GENERATE_CONFIG_SOURCE_FILE);
            String targetID = Prop.GetPropertyString(
                ScriptProperties.GENERATE_CONFIG_TARGET_FILE);
            CSVFormat format = Analyst.Script.DetermineInputFormat(
                sourceID);

            EncogLogging.Log(EncogLogging.LEVEL_DEBUG, "Beginning generate");
            EncogLogging.Log(EncogLogging.LEVEL_DEBUG, "source file:" + sourceID);
            EncogLogging.Log(EncogLogging.LEVEL_DEBUG, "target file:" + targetID);

            FileInfo sourceFile = Script.ResolveFilename(sourceID);
            FileInfo targetFile = Script.ResolveFilename(targetID);

            // mark generated
            Script.MarkGenerated(targetID);

            // read file
            bool headers = Script.ExpectInputHeaders(sourceID);
            var headerList = new CSVHeaders(sourceFile, headers,
                                            format);

            int[] input = DetermineInputFields(headerList);
            int[] ideal = DetermineIdealFields(headerList);

            EncogUtility.ConvertCSV2Binary(sourceFile, format, targetFile, input,
                                           ideal, headers);
            return false;
        }
    }
}