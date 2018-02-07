using System;
using System.IO;
using System.Linq;
using GRMModels;
using GRMServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GRMIntegrationTests
{
    [TestClass]
//    [DeploymentItem(DataFilesFolder + MusicContractsFile)]
//    [DeploymentItem(DataFilesFolder + DistributionContractsFile)]
    public class FileServiceShould
    {
        private const string DataFilesFolder = @"Resources\";
        const string DistributionContractsFile = "DistributionPartnerContracts.txt";
        const string MusicContractsFile = "MusicContracts.txt";

        [TestMethod]
        public void LoadMusicContracts()
        {
            var filePath = Path.Combine(DataFilesFolder, MusicContractsFile);

            var service = new FileService();
            var contracts = service.LoadMusicContracts(filePath);

            Assert.AreEqual(7, contracts.Count);

            Assert.AreEqual("Monkey Claw", contracts[5].Artist.Name);

            var artistAsset = contracts[5].Artist.Assets[0];
            Assert.AreEqual("Motor Mouth", artistAsset.Name);
            Assert.AreEqual(2, artistAsset.DistributionTypes.Count);
            Assert.AreEqual(DistributionType.DigitalDownload, artistAsset.DistributionTypes.First());
            Assert.AreEqual(new DateTime(2011, 3, 1), artistAsset.DistributionStart);

            artistAsset = contracts[6].Artist.Assets[0];
            Assert.AreEqual("Christmas Special", artistAsset.Name);
            Assert.AreEqual(1, artistAsset.DistributionTypes.Count);
            Assert.AreEqual(DistributionType.Streaming, artistAsset.DistributionTypes.First());
            Assert.AreEqual(new DateTime(2012, 12, 25), artistAsset.DistributionStart);
            Assert.AreEqual(new DateTime(2012, 12, 31), artistAsset.DistributionEnd);

        }

        [TestMethod]
        public void LoadDistributionPartnerContracts()
        {
            var filePath = Path.Combine(DataFilesFolder, DistributionContractsFile);

            var service = new FileService();
            var contracts = service.LoadDistributionPartnerContracts(filePath);

            Assert.AreEqual(2, contracts.Count);

            Assert.AreEqual("ITunes", contracts[0].Partner.Name);
            Assert.AreEqual(DistributionType.DigitalDownload, contracts[0].Partner.Type);
            Assert.AreEqual("YouTube", contracts[1].Partner.Name);
            Assert.AreEqual(DistributionType.Streaming, contracts[1].Partner.Type);
        }
    }
}
