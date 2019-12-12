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

        public async Task<IList<TextRecognitionResult>> ExtractText(Stream image)
        {
            var result = await this.client.BatchReadFileInStreamAsync(image);
            string operationId = result.OperationLocation.Substring(result.OperationLocation.Length - 36);
            
            int i = 0;
            int maxRetries = 10;
            ReadOperationResult operation = await client.GetReadOperationResultAsync(operationId);
            while ((operation.Status == TextOperationStatusCodes.Running ||
                    operation.Status == TextOperationStatusCodes.NotStarted) 
                    && i++ < maxRetries)
            {
                await Task.Delay(1000);
                operation = await client.GetReadOperationResultAsync(operationId);
            }

            return operation.RecognitionResults;
        }
    }
}