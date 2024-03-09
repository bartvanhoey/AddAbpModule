using System.Threading.Tasks;
using PdfGenerator;
using Volo.Abp.Application.Services;

namespace BookStore
{
    public class ExportPdfAppService(IPdfGeneratorService pdfGeneratorService) : ApplicationService, IExportPdfAppService
    {
        private readonly IPdfGeneratorService _pdfService = pdfGeneratorService;
        public async Task<byte[]> GeneratePdf() => await _pdfService.Generate();
    }
}