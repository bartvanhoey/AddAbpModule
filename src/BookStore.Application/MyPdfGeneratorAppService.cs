using System;
using System.Threading.Tasks;
using PdfGenerator;

namespace BookStore.Application
{
    public class MyPdfGeneratorAppService : BookStoreAppService
    {
        private readonly IPdfGeneratorService _PdfGeneratorService;

        public MyPdfGeneratorAppService(IPdfGeneratorService PdfGeneratorService) => _PdfGeneratorService = PdfGeneratorService;

        public async Task PdfGeneratorServiceTest() { 
            
            Console.WriteLine("==========================================");
            await _PdfGeneratorService.GeneratePdf();
            Console.WriteLine("==========================================");

            await Task.CompletedTask;
        }

    }
}