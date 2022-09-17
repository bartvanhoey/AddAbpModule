## Adding a Module to an ABP project made simple

## Create a new ABP Framework application

```bash
    abp new BookStore -u blazor -o BookStore --preview
```

Run migrations
Start API and Blazor project

## Create a PdfGenerator ABP Module

Open a command prompt in the src folder of the project and add a new class library

```bash
    dotnet new classlib -n PdfGenerator
```

## Add PdfGenerator class project to solution

Go to the root of your ABP project and run the command below:

```bash
    dotnet sln add src\PdfGenerator\PdfGenerator.csproj
```

### Add Volo.Abp.Core nuget package

Open a command prompt in the PdfGenerator class library and install Volo.Abp.Core nuget package.

```bash
    abp add-package Volo.Abp.Core
```

## Add a PdfGeneratorSettings section to the appsettings.json file in hte HttpApi.Host project

```bash
  "PdfGeneratorSettings" : { 
    "UserName" : "your-username", 
    "Password" : "MyPassword1!",
    "EmailAddress" : "your-username@hotmail.com"
  }

```

## Add a PdfGeneratorSettingsOptions class to the PdfGenerator class library project

```csharp
namespace PdfGenerator
{
    public class PdfGeneratorSettingsOptions
    {
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? EmailAddress { get; set; }
    }
}
```

## Add a MyPdfGeneratorModule to the PdfGenerator class library project

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

## Add an IPdfGeneratorService interface and a PdfGeneratorService class to the PdfGenerator class library project

```csharp
namespace PdfGenerator
{
    public interface IPdfGeneratorService
    {
        Task GeneratePdf();
    }
}

```

```csharp
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

```

### Add a project reference to the PdfGeneratorModule class library project in the Application project

Open a command prompt in the Application project

```bash
    dotnet add reference ../../src/PdfGenerator/PdfGenerator.csproj
```

### Attribute DependsOn in the BookStoreApplicationModule class in the Application project

Add a typeof(MyPdfGeneratorModule) entry in the DependsOn Attribute

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

### Add A MyPdfGeneratorAppService class to test the Application project

```csharp
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
```

## Run API and test api/app/my-pdf-generator/pdf-generator-service-test endpoint


