using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using demo.Models;
using System.IO;
using Microsoft.AspNetCore.Http;
using demo.Services;

namespace demo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ImageAnalyzer imageAnalyzer = null;

        public HomeController(
            ImageAnalyzer imageAnalyzer)
        {
            this.imageAnalyzer = imageAnalyzer;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AnalyzeImage(IFormFile image)
        {
            if(image == null || image.Length == 0)
                return BadRequest();
               
            var result = await this.imageAnalyzer.Analyze(image.OpenReadStream());
    
            return Ok(result);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
