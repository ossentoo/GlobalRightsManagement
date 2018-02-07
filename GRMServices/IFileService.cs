using System.Collections.Generic;
using GRMModels;

namespace GRMServices
{
    public interface IFileService
    {
        List<MusicContract> LoadMusicContracts(string filePath);
        List<DistributionPartnerContract> LoadDistributionPartnerContracts(string filePath);
    }
}