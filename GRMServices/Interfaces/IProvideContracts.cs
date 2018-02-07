using System.Collections.Generic;
using GRMModels;

namespace GRMServices.Interfaces
{
    public interface IProvideContracts
    {
        List<MusicContract> LoadMusicContracts(string filePath);
        List<DistributionPartnerContract> LoadDistributionPartnerContracts(string filePath);
    }
}