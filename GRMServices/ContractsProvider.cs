﻿using System;
using System.Collections.Generic;
using System.Linq;
using GRMModels;
using GRMServices.Interfaces;

namespace GRMServices
{
    public class ContractsProvider : IProvideContracts
    {
        private readonly IFileService _fileService;
        private readonly List<MusicContract> _musicContracts;
        private List<DistributionContract> _distributionContracts;

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
            _musicContracts = new List<MusicContract>();
        }

        public List<MusicContract> LoadMusicContracts(string filePath)
        {
            var rowsToProcess = _fileService.GetFileDataRows(filePath);

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


                _musicContracts.Add(contract);
            }

            return _musicContracts;
        }


        public List<DistributionContract> LoadDistributionPartnerContracts(string filePath)
        {
            var rowsToProcess = _fileService.GetFileDataRows(filePath);
            _distributionContracts = new List<DistributionContract>();

            foreach (var row in rowsToProcess)
            {
                var result = row.Split('|');

                var contract = new DistributionContract
                {
                    Partner = new DistributionPartner
                    {
                        Name = result[DistribColumns_Partner],
                        Type = result[DistribColumns_Usage].Contains("download") ?
                                            DistributionType.DigitalDownload :
                                            DistributionType.Streaming
                    }
                };

                _distributionContracts.Add(contract);

            }

            return _distributionContracts;
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

        public List<string> QueryArtistAssetsToDistribute(string query)
        {

            var querySplit = query.Split(new []{' '}, 2, StringSplitOptions.RemoveEmptyEntries);
            // _musicContracts.Where();

            return null;
        }
    }
}