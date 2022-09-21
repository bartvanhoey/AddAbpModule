using System;
using System.IO;
using System.Threading.Tasks;
using BookStore.Application.Contracts;
using PdfGenerator;
using PdfSharpCore.Drawing;
using PdfSharpCore.Fonts;
using PdfSharpCore.Pdf;
using PdfSharpCore.Utils;

namespace BookStore.Application
{
    public class ExportPdfAppService : BookStoreAppService, IExportPdfAppService
    {
        private readonly IPdfGeneratorService _pdf;

        public ExportPdfAppService(IPdfGeneratorService pdfGeneratorService) => _pdf = pdfGeneratorService;


        public async Task<byte[]> GeneratePdf()
        {
            var pdf =  await _pdf.GeneratePdf();
            return pdf;
        }
    }
}