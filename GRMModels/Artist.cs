using System;
using System.Collections.Generic;

namespace GRMModels
{
    public class Artist
    {
        public Artist()
        {
            Assets = new List<Asset>();
            Id = Guid.NewGuid();
        }

        public Guid Id { get; }
        public string Name { get; set; }
        public List<Asset> Assets { get; set; }
    }

    public class Asset
    {
        public string Name { get; set; }
        public List<DistributionType> DistributionTypes { get; set; }
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
        Streaming,
        DigitalDownload
    }

    
    public class DistributionContract
    {
        public IEnumerable<Asset> Assets { get; set; }
        public DistributionPartner Partner { get; set; }
    }

    public class MusicContract
    {
        public Artist Artist { get; set; }

    }
}
