using ImageConvertor.web.Models;
using ImageProcessor.Plugins.WebP.Imaging.Formats;
using ImageProcessor;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ImageConvertor.web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        [RequestFormLimits(MultipartBodyLengthLimit = 209715200)]
        public IActionResult Convertor(IFormFile image,string format)
        {
            if (image == null)
            {
                ModelState.AddModelError(string.Empty, "The image should not be empty!");
                return View("Index");
            }
            string imageurl = image.FileName;
            string imageExt = Path.GetExtension(imageurl);
            if(imageExt == format)
            {
                ModelState.AddModelError(string.Empty, "The selected image format should not be the same as the selected target format!");
                return View("Index");
            }
            //var deletepath1 = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img", image.FileName);
            //if (System.IO.File.Exists(deletepath1))
            //{
            //    System.IO.File.Delete(deletepath1);
            //}
            var filename1 = Guid.NewGuid().ToString() + DateTime.Now.ToString("yyyymmddMMss") + format;
            imageurl = filename1;
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img", filename1);
            using (var stream = new FileStream(path, FileMode.Create))
            {
                using (ImageFactory imageFactory = new ImageFactory(preserveExifData: false))
                {
                    imageFactory.Load(image.OpenReadStream())
                                .Format(new WebPFormat())
                                .Quality(80)
                                .Save(stream);
                }
            }
            TempData["BlogSuccess"] = "Upload File Success";
            return View("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}