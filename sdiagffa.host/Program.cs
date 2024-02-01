using sdiagffa.application;
using sdiagffa.core;
using System.Text.Json.Serialization;
using sdiagffa.core.models;
using Mapster;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services
    .AddCors(p =>
    {
        p.AddPolicy("Development", policyBuilder =>
        {
            policyBuilder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
        });
    })
    .AddControllers()
    .AddJsonOptions(opt => opt.JsonSerializerOptions.Converters.Add(
        new JsonStringEnumConverter()));
TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());

builder.Services.AddApplication();

var coreConfig = new CoreConfig();
builder.Configuration.Bind(coreConfig);
builder.Services.AddCore(coreConfig);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(app.Environment.EnvironmentName);
app.MapControllers();

app.Run();

public partial class Program { }
