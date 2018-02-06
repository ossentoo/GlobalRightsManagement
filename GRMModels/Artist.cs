using System;
using System.Collections.Generic;

namespace GRMModels
{
    public class Artist
    {
        public string Name { get; set; }
    }

    public class Asset
    {
        public Artist Artist { get; set; }

        public string Name { get; set; }
        public IEnumerable<DistributionChannel> DistributionChannels { get; set; }
    }

    public class DeliveryPartner
    {
        public string Name { get; set; }
    }

    public enum DistributionType
    {
        Stream,
        Download
    }

    public class DistributionChannel
    {
        public DistributionType Type { get; set; }
        public DateTime DistributionStart { get; set; }
    }
}
