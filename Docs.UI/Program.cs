using Docs.UI;
using Docs.UI.Interfaces;
using Docs.UI.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddScoped<IDownloadService, DownloadService>();

builder.Services.AddRadzenComponents();

await builder.Build().RunAsync();
