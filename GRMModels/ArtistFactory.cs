using System.Collections.Generic;
using System.Linq;

namespace GRMModels
{
    public static class ArtistFactory
    {
        private static List<Artist> Artists;
        static ArtistFactory()
        {
            Artists = new List<Artist>();
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
                Artists.Add(artist);
            }
            return artist;
        }
    }
}