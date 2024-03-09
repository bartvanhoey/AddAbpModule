using Microsoft.Extensions.Options;
using PdfSharpCore.Drawing;
using PdfSharpCore.Fonts;
using PdfSharpCore.Pdf;
using PdfSharpCore.Utils;

namespace PdfGenerator
{
    public interface IPdfGeneratorService
    {
        Task<byte[]> Generate();
    }

    public class PdfGeneratorService(IOptions<PdfGeneratorSettingsOptions> options) : IPdfGeneratorService
    {
        private readonly PdfGeneratorSettingsOptions _options = options.Value;

        public async Task<byte[]> Generate()
        {
            if (GlobalFontSettings.FontResolver is not FontResolver)
                GlobalFontSettings.FontResolver = new FontResolver();

            var document = new PdfDocument();
            var page = document.AddPage();

            var gfx = XGraphics.FromPdfPage(page);
            var font = new XFont("Arial", 20, XFontStyle.Bold);

            var textColor = XBrushes.Black;
            var layout = new XRect(20, 20, page.Width, page.Height);
            var format = XStringFormats.Center;

            gfx.DrawString($"Pdf created by {_options.UserName}!", font, textColor, layout, format);

            byte[] fileContents;
            using (var stream = new MemoryStream())
            {
                document.Save(stream, true);
                fileContents = stream.ToArray();
            }
            return await Task.FromResult(fileContents);
        }
    }
}