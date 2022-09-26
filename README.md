## Adding a Module to an ABP project made simple

As you can read in the ABP Framework documentation about [Modularity](https://docs.abp.io/en/abp/6.0/Module-Development-Basics), the ABP Framework was designed to support to build fully [modular](https://www.techopedia.com/definition/24772/modularity) applications.

In today's blog post I will show you how to  create and integrate your own module, in this case a simple PdfGenerator, into an ABP Framework application.

### Create a new ABP Framework application

```bash
    abp new BookStore -u blazor -o BookStore
```

* Run the DbMigrator project to apply the database migrations
* Start both the HttpApi.Host project and Blazor project
* Stop both the HttpApi.Host project and Blazor project

### Create a PdfGenerator ABP Module ()

Open a **command prompt** in the **src** folder of the project and **add a new class library**

```bash
    dotnet new classlib -n PdfGenerator
```

### Add PdfGenerator class project to solution

Go to the **root of your ABP project** and run the command below:

```bash
    dotnet sln add src\PdfGenerator\PdfGenerator.csproj
```

### Install Nuget packages

Open a **command prompt** in the **root of the PdfGenerator** class library and  install the nuget packages below.

```bash
    abp add-package Volo.Abp.Core
    dotnet add package PdfSharpCore
```

### Add a PdfGeneratorSettings section to the appsettings.json file in hte HttpApi.Host project

```bash
  "PdfGeneratorSettings": {
        "UserName": "your-username",
        "EmailAddress": "your-username@hotmail.com"
    }
```

### Add a PdfGeneratorSettingsOptions class to the PdfGenerator class library project

```csharp
namespace PdfGenerator
{
    public class PdfGeneratorSettingsOptions
    {
        public string? UserName { get; set; }
        public string? EmailAddress { get; set; }
    }
}
```

### Add a MyPdfGeneratorModule class to the PdfGenerator class library project

```csharp
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace PdfGenerator
{
    public class MyPdfGeneratorModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();
            Configure<PdfGeneratorSettingsOptions>(configuration.GetSection("PdfGeneratorSettings"));

            context.Services.AddTransient<IPdfGeneratorService, PdfGeneratorService>();
        }
    }
}
```

### Add an IPdfGeneratorService interface and a PdfGeneratorService class to the PdfGenerator class library project

```csharp
using Volo.Abp.DependencyInjection;
using Microsoft.Extensions.Options;
using PdfSharpCore.Drawing;
using PdfSharpCore.Fonts;
using PdfSharpCore.Pdf;
using PdfSharpCore.Utils;

namespace PdfGenerator
{
    public interface IPdfGeneratorService : ITransientDependency
    {
        Task<byte[]> Generate();
    }

    public class PdfGeneratorService : IPdfGeneratorService
    {
        private readonly PdfGeneratorSettingsOptions _options;
        public PdfGeneratorService(IOptions<PdfGeneratorSettingsOptions> options) => _options = options.Value;
        
        public async Task<byte[]> Generate()
        {

            if (GlobalFontSettings.FontResolver is not FontResolver)
            {
                GlobalFontSettings.FontResolver = new FontResolver();
            }

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
```

### Add a project reference to the PdfGeneratorModule class library project in the Application project

Open a **command prompt** in the **root of the Application** project

```bash
   dotnet add reference ../../src/PdfGenerator/PdfGenerator.csproj
```

### Attribute DependsOn in the BookStoreApplicationModule class in the Application project

Add a **typeof(MyPdfGeneratorModule)** entry in the **DependsOnAttribute**

```csharp
[DependsOn(
    typeof(BookStoreDomainModule),
    typeof(AbpAccountApplicationModule),
    typeof(BookStoreApplicationContractsModule),
    typeof(AbpIdentityApplicationModule),
    typeof(AbpPermissionManagementApplicationModule),
    typeof(AbpTenantManagementApplicationModule),
    typeof(AbpFeatureManagementApplicationModule),
    typeof(AbpSettingManagementApplicationModule),
    typeof(MyPdfGeneratorModule)
    )]

### Add A IExportPdfAppService interface the Application.Contracts project

```csharp
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace BookStore.Application.Contracts
{
    public interface IExportPdfAppService : IApplicationService
    {
        Task<byte[]> GeneratePdf();
    }
}
```

### Add A ExportPdfAppService class to test the Application project

```csharp
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
```

### add a exporttopdf.js file to the wwwroot/js folder of the Blazor project

```bash
function saveAsFile(filename, bytesBase64) {
    var link = document.createElement('a');
    link.download = filename;
    link.href = 'data:application/octet-stream;base64,' + bytesBase64;
    document.body.appendChild(link); // Needed for Firefox
    link.click();
    document.body.removeChild(link);
}
```

### add a reference to exporttopdf.js in the index.html file in the Blazor project

```html
<!DOCTYPE html>
<html>
    <!-- other code here ... -->
    <script src="js/exporttopdf.js"></script>
</body>
</html>
```

### Replace contents from Index.razor page in the Blazor project

```html
@page "/"
@using BookStore.Application.Contracts
@inject IExportPdfAppService ExportPdfAppService
@inject IJSRuntime JsRuntime

<Row Class="d-flex px-0 mx-0 mb-1">
    <Button Clicked="@(ExportPdf)" class="p-0 ml-auto mr-2" style="background-color: transparent"
            title="Download">
        <span class="fa fa-file-pdf fa-lg m-0" style="color: #008000; background-color: white;"
              aria-hidden="true">
        </span>
        Generate Pdf!
    </Button>
</Row>

@code {
    private async Task ExportPdf()
    {
        var pdfBytes = await ExportPdfAppService.GeneratePdf();
        await JsRuntime.InvokeVoidAsync("saveAsFile", $"test_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.pdf",
            Convert.ToBase64String(pdfBytes));
    }
}
```

### Test the Pdf

Start both the **Blazor** and the **HttpApi.Host** project to run the application
and test out the **PdfGenerator module** you just created.

Get the source code on GitHub.

Enjoy and have fun!
