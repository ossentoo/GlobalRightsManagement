using System.IO;
using GRMModels;
using GRMServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GRMIntegrationTests
{
    [TestClass]
    public class FileServiceShould
    {
        private const string DataFilesFolder = @"Resources\";
        const string DistributionFile = "DistributionPartnerContracts.txt";

        [DeploymentItem(DataFilesFolder + "MusicContracts.txt"),
            DeploymentItem(DataFilesFolder + DistributionFile)]
        [TestMethod]
        public void LoadMusicContracts()
        {
        }

        [TestMethod]
        public void LoadDistributionPartnerContracts()
        {
            var di = new DirectoryInfo(DataFilesFolder);
            var filePath = Path.Combine(DataFilesFolder, DistributionFile);

            var service = new FileService();
            var contracts = service.LoadDistributionPartnerContracts(filePath);

            Assert.AreEqual(2, contracts.Count);

            Assert.AreEqual("ITunes", contracts[0].Partner.Name);
            Assert.AreEqual(DistributionType.Download, contracts[0].Partner.Type);
            Assert.AreEqual("YouTube", contracts[1].Partner.Name);
            Assert.AreEqual(DistributionType.Stream, contracts[1].Partner.Type);
        }
    }
}
