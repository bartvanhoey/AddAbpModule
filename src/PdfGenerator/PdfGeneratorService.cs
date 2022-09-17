using System;
using Microsoft.Extensions.Options;

namespace PdfGenerator
{
    public class PdfGeneratorService : IPdfGeneratorService
    {
        private readonly PdfGeneratorSettingsOptions _options;

        public PdfGeneratorService(IOptions<PdfGeneratorSettingsOptions> options) => _options = options.Value;

        public async Task GeneratePdf()
        {

            Console.WriteLine($"username: {_options.UserName} - email: {_options.EmailAddress}");
            await Task.CompletedTask;
        }
    }
}