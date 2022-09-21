
using PdfSharpCore.Drawing;
using PdfSharpCore.Fonts;
using PdfSharpCore.Pdf;
using PdfSharpCore.Utils;
using System;

using Microsoft.Extensions.Options;

namespace PdfGenerator
{
    public class PdfGeneratorService : IPdfGeneratorService
    {
        public async Task<byte[]> GeneratePdf()
        {

            GlobalFontSettings.FontResolver = new FontResolver();

            var document = new PdfDocument();
            var page = document.AddPage();

            var gfx = XGraphics.FromPdfPage(page);
            var font = new XFont("Arial", 20, XFontStyle.Bold);

            var textColor = XBrushes.Black;
            var layout = new XRect(20, 20, page.Width, page.Height);
            var format = XStringFormats.Center;

            gfx.DrawString("Hello World!", font, textColor, layout, format);

            byte[] fileContents = null;
            using (MemoryStream stream = new MemoryStream())
            {
                document.Save(stream, true);
                fileContents = stream.ToArray();
            }

            return await Task.FromResult(fileContents);
        }

      
    }
}