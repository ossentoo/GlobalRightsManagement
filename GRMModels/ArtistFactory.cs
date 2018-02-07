using System.Collections.Generic;
using System.Linq;

namespace GRMModels
{
    public static class ArtistFactory
    {
        private static IEnumerable<Artist> Artists;
        static ArtistFactory()
        {
            Artists = Enumerable.Empty<Artist>();
        }

        /// <summary>
        /// cache artists with the same name so they
        /// don't get duplicated
        /// </summary>
        /// <param name="artistName"></param>
        /// <returns></returns>
        public static Artist CreateOrReturnFromCache(string artistName)
        {
            var artist = Artists.FirstOrDefault(x => x.Name == artistName);

            if (artist==null)
            {
                artist = new Artist{Name = artistName};
                Artists = Artists.Concat(new []{new Artist() });
            }
            return artist;
        }
    }
}