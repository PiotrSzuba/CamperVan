using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using CamperVan.Data;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging.Console;
using RabbitMQ.Client.Events;
using CamperVan.Services.BackgroundWorkers;
using CamperVan.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);

builder.Services.AddDbContext<CamperVanContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CamperVanContext")));

builder.Services.AddLocalHostedServices();
builder.Services.AddSingleton<CamperVan.RabbitMQService>();

builder.Services.AddControllers();

var app = builder.Build();

app.UseCors(x => x
    .AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(origin => true)
    .WithOrigins("http://localhost:8100")
    .AllowCredentials());

app.MapControllers();
app.UseRouting();
app.UseWebSockets();

app.MapGet("/", () =>
{
    return "hello";
});

app.Run();
