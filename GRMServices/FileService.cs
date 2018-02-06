
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using GRMModels;

namespace GRMServices
{
    public class FileService : IFileService
    {

        public string[] LoadMusicContracts(string filePath)
        {
            return null;
        }
        public List<DistributionPartnerContract> LoadDistributionPartnerContracts(string filePath)
        {
            var fileInfo = new FileInfo(filePath);
            var rowsToProcess = GetFileContentRows(fileInfo);
            var results = new List<DistributionPartnerContract>();

            foreach (var row in rowsToProcess)
            {
                var result = row.Split('|');

                var contract =  new DistributionPartnerContract {
                    Partner = new DistributionPartner{
                        Name = result[0],
                        Type = result[1].Contains("download") ? 
                                            DistributionType.Download : 
                                            DistributionType.Stream} };

                results.Add(contract);

            }

            return results;
        }

        private List<string> GetRowValuesFromRow(string rowString)
        {
            var rowValues = new List<string>();

            const char delimiter = '|';

            rowValues.AddRange(rowString.Split(Convert.ToChar(delimiter)));

            return rowValues;
        }

        /// <summary>
        /// Get the contents of a file as a list of each row
        /// </summary>
        /// <param name="sourceFile"></param>
        /// <returns></returns>
        private List<string> GetFileContentRows(FileInfo sourceFile)
        {
            //no action if file is null or does not exist or is empty
            if (sourceFile == null)
            {
                Debug.WriteLine("No source file passed");
                return null;
            }
            if (!sourceFile.Exists)
            {
                Debug.WriteLine("Source file {0} does not exist", sourceFile.FullName);
                return null;
            }
            if (sourceFile.Length == 0)
            {
                Debug.WriteLine("Source file {0} is empty", sourceFile.FullName);
                return null;
            }

            using (var stream = sourceFile.OpenRead())
            {
                var contents = GetFileContentRows(stream);

                return contents;
            }
        }

        /// <summary>
        /// Get the contents of a stream as a list of each row
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private List<string> GetFileContentRows(Stream stream)
        {
            var reader = new StreamReader(stream);
            var contents = new List<string>();
            var titleRow = true;

            using (reader)
            {
                string record;
                do
                {
                    record = reader.ReadLine();

                    // skip the title data row
                    if (titleRow)
                    {
                        titleRow = false;
                        continue;
                    }

                    if (!string.IsNullOrEmpty(record) && record.Contains("No data found"))
                        break;

                    if (!string.IsNullOrEmpty(record))
                        contents.Add(record.Trim());

                } while (record != null);
            }

            return contents;
        }
    }
}
