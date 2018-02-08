using System;
using System.IO;
using GRMServices;
using GRMTestsCommon;

namespace GRMConsole
{
    class Program : BaseClass
    {
        static void Main(string[] args)
        {
            var partnerPath = Path.Combine(DataFilesFolder, DistributionContractsFile);
            var musicPath = Path.Combine(DataFilesFolder, MusicContractsFile);
            var fileServer = new FileService();

            var provider = new ContractsProvider(fileServer, musicPath, partnerPath);

            Console.WriteLine("Enter next query\n");
            var query = Console.ReadLine();

            try
            {
                var data = provider.QueryArtistAssetsToDistribute(query);

                Console.WriteLine("Result is:\n");
                Console.WriteLine(data);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"{ex.Message}\n");
            }

            Console.WriteLine("\nPress a key to exit");
            Console.ReadLine();
        }
    }
}
