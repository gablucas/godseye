using GodsEye.API.DI;
using GodsEye.API.Hubs;
using GodsEye.API.Interfaces;
using GodsEye.API.Middlewares;
using GodsEye.Application.Services;
using GodsEye.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddMassTransit(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAPI(builder.Configuration);
builder.Services.AddDapperDI();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddAplication();


// Teste Externo
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("DevTunnel", policy =>
//    {
//        policy
//            .WithOrigins(
//                "https://rccjh4sr-7198.brs.devtunnels.ms",
//                "https://localhost:7198"
//            )
//            .AllowAnyHeader()
//            .AllowAnyMethod();
//    });
//});


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var godsEyeState = scope.ServiceProvider.GetRequiredService<IGodsEyeState>();
    // Certifique-se de que mudou o método para async na Interface e na Classe
    await godsEyeState.InitializeAsync();
}


app.UseResponseCompression();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("Default");

// Teste Externo
//app.UseCors("DevTunnel");

app.UseAuthorization();
app.UseStaticFiles();
app.MapEndpoints();
app.MapControllers();

app.MapHub<CreatedDataHub>("/createdDataHub");

app.Run();
