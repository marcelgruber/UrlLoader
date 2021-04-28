using System.Collections.Generic;

namespace UrlLoader.Repositories
{
    public interface IUrlScraperRepository
    {
        IEnumerable<string> GetImageUrls();
        IEnumerable<string> GetWordCounts();
    }
}