
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using GRMModels;

namespace GRMServices
{
    public class FileService : IFileService
    {
        // ReSharper disable InconsistentNaming
        private const int MusicColumns_Artist = 0;
        private const int MusicColumns_Asset = 1;
        private const int MusicColumns_DistributionType = 2;
        private const int MusicColumns_DistributionStart = 3;
        private const int MusicColumns_DistributionEnd = 4;

        private const int DistribColumns_Partner = 0;
        private const int DistribColumns_Usage = 1;
        // ReSharper restore InconsistentNaming

        public List<MusicContract> LoadMusicContracts(string filePath)
        {
            var fileInfo = new FileInfo(filePath);
            var rowsToProcess = GetFileContentRows(fileInfo);
            var results = new List<MusicContract>();

            foreach (var row in rowsToProcess)
            {
                var result = row.Split('|');

                var artist = ArtistFactory.CreateOrReturnFromCache(result[MusicColumns_Artist]);

                var s = result[MusicColumns_DistributionStart];
                var amendedDate = s.ReplaceTextInDate();

                DateTime.TryParse(amendedDate, out DateTime distributionStart);

                var asset = new Asset
                {
                    Name = result[MusicColumns_Asset],
                    DistributionTypes = GetDistributionTypesFromRow(result[MusicColumns_DistributionType]),
                    DistributionStart = distributionStart                   
                };

                if (!string.IsNullOrEmpty(result[MusicColumns_DistributionEnd]))
                {
                    s = result[MusicColumns_DistributionEnd];
                    amendedDate = s.ReplaceTextInDate();
                    DateTime.TryParse(amendedDate, out DateTime distributionEnd);

                    asset.DistributionEnd = distributionEnd;
                }

                artist.Assets.Add(asset);

                var contract = new MusicContract
                {
                    Artist = artist
                };

                
                results.Add(contract);
            }

            return results;
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
                        Name = result[DistribColumns_Partner],
                        Type = result[DistribColumns_Usage].Contains("download") ? 
                                            DistributionType.DigitalDownload : 
                                            DistributionType.Streaming} };

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

        private List<DistributionType> GetDistributionTypesFromRow(string distributiontypes)
        {
            // remove extraneous spaces
            distributiontypes = distributiontypes.Replace(" ", string.Empty);

            var types = distributiontypes.Split(',');
            var results = new List<DistributionType>();

            foreach (var row in types)
            {
                if(row.Equals("digitaldownload"))
                    results.Add(DistributionType.DigitalDownload);
                else if (row.Equals("streaming"))
                    results.Add(DistributionType.Streaming);
            }

            return results;
        }
    }
}
