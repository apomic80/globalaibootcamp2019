using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

namespace demo.Services
{
    public class ImageAnalyzer
    {
        private readonly ComputerVisionClient client = null;
        public List<VisualFeatureTypes> features = null;

        public ImageAnalyzer(string key, string endpoint)
        {
            var credentials = new ApiKeyServiceClientCredentials(key);
            this.client = new ComputerVisionClient(credentials)
                { Endpoint = endpoint };

            this.features = new List<VisualFeatureTypes>()
            {
                VisualFeatureTypes.Categories, VisualFeatureTypes.Description,
                VisualFeatureTypes.Faces, VisualFeatureTypes.ImageType,
                VisualFeatureTypes.Tags, VisualFeatureTypes.Adult,
                VisualFeatureTypes.Color, VisualFeatureTypes.Brands,
                VisualFeatureTypes.Objects
            };
        }

        public async Task<ImageAnalysis> Analyze(Stream image)
        {
            return await this.client.AnalyzeImageInStreamAsync(image, features);
        }
    }
}