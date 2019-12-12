using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using demo.Models;
using Microsoft.AspNetCore.Http;
using demo.Services;

namespace demo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ImageUtils imageUtils = null;
        private readonly ImageAnalyzer imageAnalyzer = null;

        public HomeController(
            ImageUtils imageUtils,
            ImageAnalyzer imageAnalyzer)
        {
            this.imageUtils = imageUtils;
            this.imageAnalyzer = imageAnalyzer;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AnalyzeImage(IFormFile image)
        {
            if (image == null || image.Length == 0)
                return BadRequest();

            var anagrafica = await this.costruisciAnagraficaViewModel(image);

            return View("Anagrafica", anagrafica);
        }

        private async Task<AnagraficaViewModel> costruisciAnagraficaViewModel(IFormFile image)
        {
            var imageResult = await this.imageAnalyzer.Analyze(image.OpenReadStream());
            var textResult = await this.imageAnalyzer.ExtractText(image.OpenReadStream());

            var viewModel = new AnagraficaViewModel();

            if (imageResult.Objects.Count > 0 && imageResult.Objects[0].ObjectProperty == "person")
            {
                var x = imageResult.Objects[0].Rectangle.X;
                var y = imageResult.Objects[0].Rectangle.Y;
                var width = imageResult.Objects[0].Rectangle.W;
                var height = imageResult.Objects[0].Rectangle.H;
                viewModel.Foto = this.imageUtils.CropAndGetBase64Image(image, x, y, width, height);
            }
            viewModel.Nome = "Michele";
            viewModel.Cognome = "Aponte";

            return viewModel;
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
