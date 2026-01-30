using GodsEye.WEB;
using GodsEye.WEB.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// SERVICES
builder.Services.AddScoped<PersonService>();
builder.Services.AddScoped<CameraWebService>();
builder.Services.AddScoped<SectorService>();
builder.Services.AddScoped<FeatureWebService>();
builder.Services.AddScoped<GodsEyeWebService>();
builder.Services.AddScoped<SignalRService>();
builder.Services.AddScoped<EnvironmentMonitoringWebService>();
builder.Services.AddScoped<IncidentRecordingWebService>();
builder.Services.AddScoped<DwellTimeMonitoringWebService>();
builder.Services.AddScoped<NotificationGroupWebService>();


builder.Services.AddMudServices();

builder.Services.AddScoped(sp =>
    new HttpClient { BaseAddress = new Uri("https://localhost:7010") });

await builder.Build().RunAsync();
