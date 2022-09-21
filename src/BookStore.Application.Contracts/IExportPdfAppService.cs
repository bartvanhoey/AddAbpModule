using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace BookStore.Application.Contracts
{
    public interface IExportPdfAppService : IApplicationService
    {
        Task<byte[]> GeneratePdf();
    }
}
