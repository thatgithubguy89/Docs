using Docs.Api.Data;
using Docs.Api.Interfaces;
using Docs.Api.Profiles;
using Docs.Api.Repositories;
using Docs.Api.Services;
using Microsoft.Azure.Functions.Worker.Extensions.OpenApi.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var connectionString = Environment.GetEnvironmentVariable("DefaultConnection", EnvironmentVariableTarget.Process);
var cosmosConnectionString = Environment.GetEnvironmentVariable("CosmosConnection", EnvironmentVariableTarget.Process);

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
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });
    })
    .Build();

host.Run();
