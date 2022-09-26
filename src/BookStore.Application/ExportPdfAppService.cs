using System.Threading.Tasks;
using BookStore.Application.Contracts;
using PdfGenerator;

namespace BookStore.Application
{
    public class ExportPdfAppService : BookStoreAppService, IExportPdfAppService
    {    
        private readonly IPdfGeneratorService _pdfService;

        public ExportPdfAppService(IPdfGeneratorService pdfGeneratorService) 
            => _pdfService = pdfGeneratorService;

        public async Task<byte[]> GeneratePdf() => await _pdfService.Generate();
    }
}