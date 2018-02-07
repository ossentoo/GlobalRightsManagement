using System;
using System.Collections.Generic;
using GRMModels;
using GRMServices.Interfaces;

namespace GRMServices
{
    public class ContractsProvider : IProvideContracts
    {
        private readonly IFileService _fileService;
        // ReSharper disable InconsistentNaming
        private const int MusicColumns_Artist = 0;
        private const int MusicColumns_Asset = 1;
        private const int MusicColumns_DistributionType = 2;
        private const int MusicColumns_DistributionStart = 3;
        private const int MusicColumns_DistributionEnd = 4;

        private const int DistribColumns_Partner = 0;
        private const int DistribColumns_Usage = 1;
        // ReSharper restore InconsistentNaming

        public ContractsProvider(IFileService fileService)
        {
            _fileService = fileService;
        }
        public List<MusicContract> LoadMusicContracts(string filePath)
        {
            var rowsToProcess = _fileService.GetFileDataRows(filePath);
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
            var rowsToProcess = _fileService.GetFileDataRows(filePath);
            var results = new List<DistributionPartnerContract>();

            foreach (var row in rowsToProcess)
            {
                var result = row.Split('|');

                var contract = new DistributionPartnerContract
                {
                    Partner = new DistributionPartner
                    {
                        Name = result[DistribColumns_Partner],
                        Type = result[DistribColumns_Usage].Contains("download") ?
                                            DistributionType.DigitalDownload :
                                            DistributionType.Streaming
                    }
                };

                results.Add(contract);

            }

            return results;
        }

        private List<DistributionType> GetDistributionTypesFromRow(string distributiontypes)
        {
            // remove extraneous spaces
            distributiontypes = distributiontypes.Replace(" ", string.Empty);

            var types = distributiontypes.Split(',');
            var results = new List<DistributionType>();

            foreach (var row in types)
            {
                if (row.Equals("digitaldownload"))
                    results.Add(DistributionType.DigitalDownload);
                else if (row.Equals("streaming"))
                    results.Add(DistributionType.Streaming);
            }

            return results;
        }

    }
}