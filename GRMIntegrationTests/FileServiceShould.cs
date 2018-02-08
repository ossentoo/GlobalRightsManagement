using System.IO;
using GRMServices;
using GRMTestsCommon;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GRMIntegrationTests
{
    [TestClass]
    public class FileServiceShould : BaseClass
    {
        [TestMethod]
        public void LoadMusicContracts()
        {
            var filePath = Path.Combine(DataFilesFolder, MusicContractsFile);

            var service = new FileService();
            var dataRows = service.GetFileDataRows(filePath);

            Assert.AreEqual(7, dataRows.Count);
        }

        [TestMethod]
        public void LoadDistributionPartnerContracts()
        {
            var filePath = Path.Combine(DataFilesFolder, DistributionContractsFile);

            var service = new FileService();
            var dataRows = service.GetFileDataRows(filePath);

            Assert.AreEqual(2, dataRows.Count);
        }
    }
}
