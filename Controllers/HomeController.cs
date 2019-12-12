using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using demo.Models;
using Microsoft.AspNetCore.Http;
using demo.Services;
using System.Linq;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System;
using System.Collections.Generic;

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
            var tipoDocumento = await this.imageAnalyzer.GetCustomLabel(image.OpenReadStream());
            Tuple<string, IList<double>>[] lines = textResult[0].Lines.Select(x => new Tuple<string, IList<double>>(x.Text.Trim().ToUpper(), x.BoundingBox)).ToArray();

            var viewModel = new AnagraficaViewModel();
            viewModel.Foto = this.getFoto(imageResult, image);
            viewModel.Documento = this.getTipoDocumento(tipoDocumento);
            viewModel.Nome = this.getValue(CampiDocumento.Nome, viewModel.Documento, lines);
            viewModel.Cognome = this.getValue(CampiDocumento.Cognome, viewModel.Documento, lines);
            viewModel.Indirizzo = this.getValue(CampiDocumento.Indirizzo, viewModel.Documento, lines);

            return viewModel;
        }

        private string getValue(CampiDocumento campo, TipoDocumento documento, Tuple<string, IList<double>>[] lines)
        {
            var label = this.getLabel(campo, documento);
            if(string.IsNullOrEmpty(label)) return null;
            var labelLine = lines.FirstOrDefault(x => x.Item1.StartsWith(label));
            if(labelLine == null) return null;

            if(documento == TipoDocumento.Patente) 
            {
                return labelLine.Item1.Replace(label, string.Empty).Trim();
            } 
            else if(documento == TipoDocumento.CartaIdentitaCartacea) 
            {
                return labelLine.Item1.Replace(label, string.Empty).Replace(".", string.Empty).Trim();
            }
            else 
            {
                return lines
                    .Select(x => new { d = this.calcolaDistanzaDaLabel(labelLine, x), Text = x.Item1})
                    .OrderBy(x => x.d)
                    .FirstOrDefault()?.Text;
            }
        }

        private string getLabel(CampiDocumento campo, TipoDocumento documento)
        {
            switch (campo)
            {
                case CampiDocumento.Nome:
                    return documento == TipoDocumento.Patente ? "2. " : "NOME";
                case CampiDocumento.Cognome:
                    return documento == TipoDocumento.Patente ? "1. " : "COGNOME";
                case CampiDocumento.Indirizzo:
                    switch (documento)
                    {
                        case TipoDocumento.CartaIdentitaCartacea:
                            return "VIA";
                        case TipoDocumento.CartaIdentitaDigitale:
                            return "INDIRIZZO";
                        default:
                            return string.Empty;
                    }
                default:
                    return string.Empty;
            }
        }

        private object calcolaDistanzaDaLabel(Tuple<string, IList<double>> labelLine, Tuple<string, IList<double>> x)
        {
            return Math.Pow(x.Item2[0] - labelLine.Item2[6], 2) + Math.Pow(x.Item2[1] - labelLine.Item2[7], 2);
        }

        private string getFoto(ImageAnalysis imageResult, IFormFile image)
        {
            if (imageResult.Objects.Count > 0 && imageResult.Objects[0].ObjectProperty == "person")
            {
                var x = imageResult.Objects[0].Rectangle.X;
                var y = imageResult.Objects[0].Rectangle.Y;
                var width = imageResult.Objects[0].Rectangle.W;
                var height = imageResult.Objects[0].Rectangle.H;
                return this.imageUtils.CropAndGetBase64Image(image, x, y, width, height);
            }
            return null;
        }

        private TipoDocumento getTipoDocumento(string tipoDocumento) 
        {
            switch (tipoDocumento)
            {
                case "cartaidentitacartacea":
                    return TipoDocumento.CartaIdentitaCartacea;
                
                case "cartaidentitadigitale":
                    return TipoDocumento.CartaIdentitaDigitale;

                case "patente":
                    return TipoDocumento.Patente;
                
                case "passaporto":
                    return TipoDocumento.Passaporto;
                
                default:
                    return TipoDocumento.CartaIdentitaCartacea;
            }
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
