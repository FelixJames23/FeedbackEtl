using System.Text.Json;
using FeedbackEtl.Domain.Entities;
using FeedbackEtl.Domain.Interfaces;
using FeedbackEtl.Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace FeedbackEtl.Infrastructure.Persistence;

public class JsonStagingService : IStagingService
{
    private readonly EtlSettings _settings;
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    public JsonStagingService(IOptions<EtlSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task SaveBatchAsync(ExtractionResult result, CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(_settings.StagingOutputFolder);

        var fileName = $"{result.SourceName}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.json";
        var fullPath = Path.Combine(_settings.StagingOutputFolder, fileName);

        await using var stream = File.Create(fullPath);
        await JsonSerializer.SerializeAsync(stream, result, JsonOptions, cancellationToken);
    }
}
