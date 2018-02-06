﻿using System;
using System.Collections.Generic;

namespace GRMModels
{
    public class Artist
    {
        public string Name { get; set; }
        public IEnumerable<Asset> Assets { get; set; }
    }

    public class Asset
    {
        public string Name { get; set; }
        public DistributionType Type { get; set; }
        public DateTime DistributionStart { get; set; }
        public DateTime DistributionEnd { get; set; }
    }


    public class DistributionPartner
    {
        public string Name { get; set; }
        public DistributionType Type { get; set; }
    }

    public enum DistributionType
    {
        Stream,
        Download
    }

    
    public class DistributionPartnerContract
    {
        public IEnumerable<Asset> Assets { get; set; }
        public DistributionPartner Partner { get; set; }
    }

    public class MusicContract
    {
        public Artist Artist { get; set; }

    }

    public class RightManager
    {
        public IEnumerable<MusicContract> MusicContracts { get; set; }
        public IEnumerable<DistributionPartnerContract> DistributionPartnerContracts { get; set; }
    }
}
