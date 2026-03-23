using GodsEye.API.Hubs;
using GodsEye.API.Middlewares;
using GodsEye.API.Services;
using GodsEye.Application.Services;
using GodsEye.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAPI();
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
app.MapControllers();

app.MapHub<CreatedDataHub>("/createdDataHub");

app.Run();
