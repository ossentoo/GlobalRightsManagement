﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GRMModels;
using GRMServices.Interfaces;

namespace GRMServices
{
    public class ContractsProvider : IProvideContracts
    {
        private readonly IFileService _fileService;
        private readonly List<MusicContract> _musicContracts;
        private readonly List<DistributionContract> _distributionContracts;

        // ReSharper disable InconsistentNaming
        private const int MusicColumns_Artist = 0;
        private const int MusicColumns_Asset = 1;
        private const int MusicColumns_DistributionType = 2;
        private const int MusicColumns_DistributionStart = 3;
        private const int MusicColumns_DistributionEnd = 4;

        private const int DistribColumns_Partner = 0;
        private const int DistribColumns_Usage = 1;

        private const string TitleRow = "Artist|Title|Usage|StartDate|EndDate";
        // ReSharper restore InconsistentNaming

        public ContractsProvider(IFileService fileService)
        {
            _fileService = fileService;
            _musicContracts = new List<MusicContract>();
            _distributionContracts = new List<DistributionContract>();
            ArtistFactory.Clear();
        }

        public ContractsProvider(IFileService fileService, string musicFilePath, string distributionFilePath)
            :this(fileService)
        {
            if(string.IsNullOrEmpty(musicFilePath) || string.IsNullOrEmpty(distributionFilePath))
                throw new ArgumentNullException("Invalid file path arguments");

            _musicContracts = LoadMusicContracts(musicFilePath);
            _distributionContracts = LoadDistributionPartnerContracts(distributionFilePath);

            CreateRelationships();
        }

        private void CreateRelationships()
        {
            foreach (var contract in _distributionContracts)
            {
                var assets = _musicContracts.SelectMany(x => x.Artist.Assets).Distinct();

                contract.Assets = from a in assets
                                    from b in a.DistributionTypes
                                    where b == contract.Partner.Type
                                    select a;
            }
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

        /// <summary>
        /// Query and return data as a list of strings.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public string QueryArtistAssetsToDistribute(string query)
        {
            var querySplit = query.Split(new []{' '}, 2, StringSplitOptions.RemoveEmptyEntries);

            var distributionPartner = querySplit[0];
            var distributionStart = querySplit[1].ReplaceTextInDate();

            DateTime.TryParse(distributionStart, out DateTime startDate);

            var requiredPartners = _distributionContracts.Where(p => p.Partner.Name == distributionPartner);
            var partnersAssets = requiredPartners.SelectMany(a =>
                {
                    var o = new { a, a.Assets, a.Partner.Type};
                    return new[] {o};
                }).Distinct();

            var requiredAssets = (from a in _musicContracts.Select(m => m.Artist)
                                from b in a.Assets
                                from c in partnersAssets
                                where c.Assets.Contains(b) && b.DistributionStart < startDate
                                  select new {a, b, c}).Distinct();

            var requiredFields = from x in requiredAssets
                                    orderby x.a.Name, x.b.Name 
                                    select new FileDataRow
                                    {
                                        Artist = x.a.Name,
                                        Title = x.b.Name,
                                        Usage = x.b.DistributionTypes.First(y=>y==x.c.Type).ToDescription(),
                                        StartDate = x.b.DistributionStart.ToOrdinalDate(),
                                        EndDate = x.b.DistributionEnd?.ToOrdinalDate()
                                    };

            var fileText = CreateFileText(requiredFields);
            return fileText;
        }

        private string CreateFileText(IEnumerable<FileDataRow> dataRows)
        {
            var builder = new StringBuilder();

            builder.Append($"{TitleRow}{Environment.NewLine}");

            foreach (var row in dataRows)
            {
                builder.Append($"{row.Artist}|{row.Title}|{row.Usage}|" +
                               $"{row.StartDate}|{row.EndDate}" +
                               $"{Environment.NewLine}");
            }

            var fileText = builder.ToString();

            // remove the last newline character from the string
            return fileText.Substring(0, fileText.Length - 2);
        }
    }
}