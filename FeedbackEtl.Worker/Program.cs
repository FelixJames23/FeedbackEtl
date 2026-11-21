using FeedbackEtl.Application.Services;
using FeedbackEtl.Domain.Interfaces;
using FeedbackEtl.Infrastructure.Configuration;
using FeedbackEtl.Infrastructure.Extractors;
using FeedbackEtl.Infrastructure.Persistence;
using FeedbackEtl.Worker;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<EtlSettings>(builder.Configuration.GetSection("EtlSettings"));

builder.Services.AddHttpClient("CommentsClient", client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddScoped<IExtractor, CsvExtractor>();
builder.Services.AddScoped<IExtractor, DatabaseExtractor>();
builder.Services.AddScoped<IExtractor, ApiExtractor>();
builder.Services.AddScoped<IStagingService, JsonStagingService>();
builder.Services.AddScoped<ExtractionOrchestrator>();

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
