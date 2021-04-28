using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

namespace UrlLoader.Repositories
{
    public class UrlScraperRepository
    {
        private string _url;
        private Uri _uri;
        private HtmlWeb _htmlWeb;
        private HtmlDocument _htmlDoc;

        #region Public Methods
        public UrlScraperRepository(string url)
        {
            _url = url;
            _uri = new Uri(url); // Will throw an exception if bad URl
            _htmlWeb = new HtmlWeb();
            _htmlDoc = _htmlWeb.Load(_url); // Will throw an exception if the URL cannot be fetched
        }
        
        /// <summary>
        /// Get a list of fully qualified image URLs from the specified URL.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetImageUrls()
        {
            // TODO: write formal tests

            var imageUrls = _htmlDoc.DocumentNode.Descendants("img")
                .Select(e => ConvertToFullyQualifiedUrl(e.GetAttributeValue("src", null)))
                .Where(s => !string.IsNullOrEmpty(s));

            return imageUrls;
        }

        /// <summary>
        /// Returns an object containing a dictionary of unique words, along with the word count.
        /// </summary>
        /// <returns></returns>
        public IOrderedEnumerable<object> GetUniqueWordCounts()
        {
            // TODO: write formal tests
            
            var allWords = new List<string>();
            
            var bodyContentNodes = _htmlDoc.DocumentNode
                .SelectNodes("//body//text()[not(parent::script)]")
                .Select(node => node.InnerText.ToLower());

            var splitWordsOnChars = new[] {' ', '.', ',', ':'};

            foreach (var text in bodyContentNodes)
            {
                var wordsInNode = text
                    .Split(splitWordsOnChars, StringSplitOptions.RemoveEmptyEntries)
                    .Where(s => char.IsLetter(s[0]))
                    .ToArray();

                allWords.AddRange(wordsInNode);
            }

            var uniqueWordCounts = allWords
                .GroupBy(word => word)
                .Select(wordGroup => new
                {
                    Word = wordGroup.Key,
                    Count = wordGroup.Select(l => l).Count()
                })
                .OrderByDescending(word => word.Count);

            return uniqueWordCounts;
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Converts URL paths to fully qualified URLs. Ex: /image.jpg => https://google.com/image.jpg
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private string ConvertToFullyQualifiedUrl(string url)
        {
            // TODO: write formal tests
            
            if (string.IsNullOrWhiteSpace(url) || url.StartsWith("http://") || url.StartsWith("https://") || url.StartsWith("//"))
            {
                return url;
            }

            var fullyQualifiedUrl = url;

            var isRelativePath = url.StartsWith("/");
            if (isRelativePath)
            {
                // TODO: leverage URI
                // Uri result;
                // Uri.TryCreate(url, UriKind.Absolute, out result);

                fullyQualifiedUrl = $"//{_uri.Host}{url}";
            }
            
            return fullyQualifiedUrl;
        }

        #endregion
    }
}