using Docs.Api.Interfaces;
using Docs.Api.Profiles;
using Docs.Api.Repositories;
using Docs.Api.Services;
using Microsoft.Azure.Functions.Worker.Extensions.OpenApi.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults(worker => worker.UseNewtonsoftJson())
    .ConfigureOpenApi()
    .ConfigureServices(services =>
    {
        services.AddAutoMapper(typeof(MappingProfile));
        services.AddScoped<IDocumentRepository, DocumentRepository>();
        services.AddScoped<IPdfService, PdfService>();
        services.AddScoped(typeof(IPagingService<,>), typeof(PagingService<,>));
        services.AddScoped(typeof(ICachingService<>), typeof(CachingService<>));
        services.AddMemoryCache();
    })
    .Build();

host.Run();
