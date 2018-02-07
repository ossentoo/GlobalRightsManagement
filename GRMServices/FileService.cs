using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using GRMServices.Interfaces;

namespace GRMServices
{
    public class FileService : IFileService
    {

        public List<string> GetFileDataRows(string filePath)
        {
            var fileInfo = new FileInfo(filePath);

            return GetFileContentRows(fileInfo);
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
