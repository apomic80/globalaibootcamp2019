using System;
using System.Drawing;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace demo.Services
{
    public class ImageUtils
    {
        public string CropAndGetBase64Image(IFormFile image, int x, int y, int width, int height)
        {
            Image documento = Image.FromStream(image.OpenReadStream(), true, true) as Bitmap;
            Image foto = new Bitmap(width, height);
            using (var g = Graphics.FromImage(foto))
            {
                g.DrawImage(
                    documento,
                    new Rectangle(0, 0, width, height),
                    new Rectangle(x, y, width, height),
                    GraphicsUnit.Pixel);
            }
            return "data:image/bmp;base64," + Convert.ToBase64String(this.imageToByteArray(foto));
        }

        private byte[] imageToByteArray(Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, imageIn.RawFormat);
                return ms.ToArray();
            }
        }

    }
}