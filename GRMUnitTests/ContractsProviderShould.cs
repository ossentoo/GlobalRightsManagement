using System;
using System.Collections.Generic;
using System.Linq;
using GRMModels;
using GRMServices;
using GRMServices.Interfaces;
using GRMTestsCommon;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace GRMUnitTests
{
    [TestClass]
    public class ContractsProviderShould : BaseTests
    {
        private Mock<IFileService> _fileServer;

        [TestInitialize]
        public void Initialize()
        {
            _fileServer = new Mock<IFileService>();
        }

        [TestMethod]
        public void LoadMusicContracts()
        {
            var provider = new ContractsProvider(_fileServer.Object);
            _fileServer.Setup(x => x.GetFileDataRows(It.IsAny<string>())).Returns(SplitData(Constants.MusicContractsFileMock));

            var contracts = provider.LoadMusicContracts(string.Empty);

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
            var provider = new ContractsProvider(_fileServer.Object);
            _fileServer.Setup(x => x.GetFileDataRows(It.IsAny<string>())).Returns(SplitData(Constants.DistributionContractsFileMock));

            var contracts = provider.LoadDistributionPartnerContracts(String.Empty);

            Assert.AreEqual(2, contracts.Count);

            Assert.AreEqual("ITunes", contracts[0].Partner.Name);
            Assert.AreEqual(DistributionType.DigitalDownload, contracts[0].Partner.Type);
            Assert.AreEqual("YouTube", contracts[1].Partner.Name);
            Assert.AreEqual(DistributionType.Streaming, contracts[1].Partner.Type);
        }


        [TestMethod]
        public void ReturnValueFromQueryForITunes1March2012()
        {
            var partnerPath = "Partner";
            var musicPath = "Music";

            _fileServer.Setup(x => x.GetFileDataRows(It.Is<string>(y => y.Equals(partnerPath))))
                .Returns(SplitData(Constants.DistributionContractsFileMock));

            _fileServer.Setup(x => x.GetFileDataRows(It.Is<string>(y => y.Equals(musicPath))))
                .Returns(SplitData(Constants.MusicContractsFileMock));

            var provider = new ContractsProvider(_fileServer.Object, musicPath, partnerPath);

            var data = provider.QueryArtistAssetsToDistribute("ITunes 1st March 2012");

            var result = data.Split(new[]{Environment.NewLine}, StringSplitOptions.None);

            Assert.AreEqual(5, result.Length);
            Assert.AreEqual("Artist|Title|Usage|StartDate|EndDate", result[0]);
            Assert.AreEqual("Monkey Claw|Black Mountain|digital download|1st Feb 2012|", result[1]);
            Assert.AreEqual("Monkey Claw|Motor Mouth|digital download|1st Mar 2011|", result[2]);
            Assert.AreEqual("Tinie Tempah|Frisky(Live from SoHo)|digital download|1st Feb 2012|", result[3]);
            Assert.AreEqual("Tinie Tempah|Miami 2 Ibiza|digital download|1st Feb 2012|", result[4]);

        }

        [TestMethod]
        public void ReturnValueFromQueryForYoutube1April2012()
        {
            var partnerPath = "Partner";
            var musicPath = "Music";

            _fileServer.Setup(x => x.GetFileDataRows(It.Is<string>(y => y.Equals(partnerPath))))
                .Returns(SplitData(Constants.DistributionContractsFileMock));

            _fileServer.Setup(x => x.GetFileDataRows(It.Is<string>(y => y.Equals(musicPath))))
                .Returns(SplitData(Constants.MusicContractsFileMock));

            var provider = new ContractsProvider(_fileServer.Object, musicPath, partnerPath);

            var data = provider.QueryArtistAssetsToDistribute("YouTube 1st April 2012");

            var result = data.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            Assert.AreEqual(3, result.Length);
            Assert.AreEqual("Artist|Title|Usage|StartDate|EndDate", result[0]);
            Assert.AreEqual("Monkey Claw|Motor Mouth|streaming|1st Mar 2011|", result[1]);
            Assert.AreEqual("Tinie Tempah|Frisky(Live from SoHo)|streaming|1st Feb 2012|", result[2]);
        }

        [TestMethod]
        public void ReturnValueFromQueryForYoutube27Dec2012()
        {
            var partnerPath = "Partner";
            var musicPath = "Music";

            _fileServer.Setup(x => x.GetFileDataRows(It.Is<string>(y => y.Equals(partnerPath))))
                .Returns(SplitData(Constants.DistributionContractsFileMock));

            _fileServer.Setup(x => x.GetFileDataRows(It.Is<string>(y => y.Equals(musicPath))))
                .Returns(SplitData(Constants.MusicContractsFileMock));

            var provider = new ContractsProvider(_fileServer.Object, musicPath, partnerPath);

            var data = provider.QueryArtistAssetsToDistribute("YouTube 27th Dec 2012");

            var result = data.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            Assert.AreEqual(5, result.Length);
            Assert.AreEqual("Artist|Title|Usage|StartDate|EndDate", result[0]);
            Assert.AreEqual("Monkey Claw|Christmas Special|streaming|25th Dec 2012|31st Dec 2012", result[1]);
            Assert.AreEqual("Monkey Claw|Iron Horse|streaming|1st Jun 2012|", result[2]);
            Assert.AreEqual("Monkey Claw|Motor Mouth|streaming|1st Mar 2011|", result[3]);
            Assert.AreEqual("Tinie Tempah|Frisky(Live from SoHo)|streaming|1st Feb 2012|", result[4]);
        }

        private List<string> SplitData(string data)
        {
            var items = data.Split(new[]{Environment.NewLine}, StringSplitOptions.None);
            return items.ToList();
;        }
    }

}
