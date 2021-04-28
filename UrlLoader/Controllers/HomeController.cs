using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using UrlLoader.Models;
using UrlLoader.Repositories;
using IUrlScraperRepository = UrlLoader.Models.IUrlScraperRepository;

namespace UrlLoader.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        // TODO: create repo 
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        /// <summary>
        /// Returns data scraped from the specified URL input.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult LoadUrl(IUrlScraperRepository input)
        {
            if (!ModelState.IsValid)
            {
                return new JsonResult(new { success = false, message = "Fully qualified URL is required." });
            }

            // TODO: use dep injection instead
            UrlScraperRepository urlScraperRepository;

            try
            {
                urlScraperRepository = new UrlScraperRepository(input.Url);
            }
            catch (Exception e)
            {
                return new JsonResult(new { success = false, message = "Unable to load specified URL." });
            }

            var imageUrls = urlScraperRepository.GetImageUrls();
            var uniqueWordCounts = urlScraperRepository.GetUniqueWordCounts();
            
            var data = new
            {
                words = uniqueWordCounts,
                images = imageUrls
            };

            return new JsonResult(data);
        }
    }
}
