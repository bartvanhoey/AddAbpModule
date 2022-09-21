using Volo.Abp.DependencyInjection;

namespace PdfGenerator
{
    public interface IPdfGeneratorService :  ITransientDependency
    {
        Task<byte[]> GeneratePdf();
    }
}