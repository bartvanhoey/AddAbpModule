using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace BookStore
{
    public interface IExportPdfAppService :  IApplicationService
    {
        Task<byte[]> GeneratePdf();
    }
}