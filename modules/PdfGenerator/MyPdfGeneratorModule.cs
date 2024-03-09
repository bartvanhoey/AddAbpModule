using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace PdfGenerator
{
    public class MyPdfGeneratorModule :  AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();
            Configure<PdfGeneratorSettingsOptions>(configuration.GetSection("PdfGeneratorSettings"));
            context.Services.AddTransient<IPdfGeneratorService, PdfGeneratorService>();
        }
    }
}