using GodsEye.WEB;
using GodsEye.WEB.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using System.Reflection;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// SERVICES
builder.Services.AddScoped<PersonService>();
builder.Services.AddScoped<CameraWebService>();
builder.Services.AddScoped<SectorWebService>();
builder.Services.AddScoped<GodsEyeWebService>();
builder.Services.AddScoped<SignalRService>();
builder.Services.AddScoped<EnvironmentMonitoringWebService>();
builder.Services.AddScoped<IncidentRecordingWebService>();
builder.Services.AddScoped<NotificationGroupWebService>();
builder.Services.AddScoped<DialogWebService>();
builder.Services.AddScoped<MediaMtxWebService>();
builder.Services.AddScoped<AccessScheduleWebService>();
builder.Services.AddScoped<AccessLevelWebService>();

builder.Services.AddScoped<ComplianceWebService>();
builder.Services.AddScoped<ComplianceViolationWebService>();
builder.Services.AddScoped<NewDialogWebService>();

builder.Services.AddAutoMapper(cfg => { }, Assembly.GetExecutingAssembly());
builder.Services.AddMudServices();

// Teste Externo
//builder.Services.AddScoped(sp =>
//    new HttpClient { BaseAddress = new Uri("https://rccjh4sr-7010.brs.devtunnels.ms") });

var apiUrl = builder.Configuration["ApiUrl"];

builder.Services.AddScoped(sp =>
    new HttpClient { BaseAddress = new Uri(apiUrl) });

await builder.Build().RunAsync();