using System;
using System.IO;
using System.Linq;
using GRMModels;
using GRMServices;
using GRMTestsCommon;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GRMUnitTests
{
    [TestClass]
    public class ContractsProviderShould : BaseTests
    {
        [TestMethod]
        public void LoadMusicContracts()
        {
            var filePath = Path.Combine(DataFilesFolder, MusicContractsFile);

            var service = new FileService();
            var provider = new ContractsProvider(service);
            var contracts = provider.LoadMusicContracts(filePath);

            Assert.AreEqual(7, contracts.Count);

            Assert.AreEqual("Monkey Claw", contracts[5].Artist.Name);

            var artistAsset = contracts[5].Artist.Assets.First(x => x.Name == "Motor Mouth");
            Assert.AreEqual("Motor Mouth", artistAsset.Name);
            Assert.AreEqual(2, artistAsset.DistributionTypes.Count);
            Assert.AreEqual(DistributionType.DigitalDownload, artistAsset.DistributionTypes.First());
            Assert.AreEqual(new DateTime(2011, 3, 1), artistAsset.DistributionStart);

            artistAsset = contracts[6].Artist.Assets.First(x => x.Name == "Christmas Special");
            Assert.AreEqual("Christmas Special", artistAsset.Name);
            Assert.AreEqual(1, artistAsset.DistributionTypes.Count);
            Assert.AreEqual(DistributionType.Streaming, artistAsset.DistributionTypes.First());
            Assert.AreEqual(new DateTime(2012, 12, 25), artistAsset.DistributionStart);
            Assert.AreEqual(new DateTime(2012, 12, 31), artistAsset.DistributionEnd);

            Assert.AreEqual(3, contracts[0].Artist.Assets.Count);
            Assert.AreEqual(4, contracts[3].Artist.Assets.Count);

        }

        [TestMethod]
        public void LoadDistributionPartnerContracts()
        {
            var filePath = Path.Combine(DataFilesFolder, DistributionContractsFile);

            var service = new FileService();
            var provider = new ContractsProvider(service);
            var contracts = provider.LoadDistributionPartnerContracts(filePath);

            Assert.AreEqual(2, contracts.Count);

            Assert.AreEqual("ITunes", contracts[0].Partner.Name);
            Assert.AreEqual(DistributionType.DigitalDownload, contracts[0].Partner.Type);
            Assert.AreEqual("YouTube", contracts[1].Partner.Name);
            Assert.AreEqual(DistributionType.Streaming, contracts[1].Partner.Type);
        }
    }

}
