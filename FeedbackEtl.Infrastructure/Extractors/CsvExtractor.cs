using System.Diagnostics;
using System.Globalization;
using CsvHelper;
using FeedbackEtl.Domain.Entities;
using FeedbackEtl.Domain.Interfaces;
using FeedbackEtl.Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FeedbackEtl.Infrastructure.Extractors;

public class CsvExtractor : IExtractor
{
    private readonly EtlSettings _settings;
    private readonly ILogger<CsvExtractor> _logger;

    public string SourceName => "CSV_EncuestasInternas";

    public CsvExtractor(IOptions<EtlSettings> settings, ILogger<CsvExtractor> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<ExtractionResult> ExtractAsync(CancellationToken cancellationToken = default)
    {
        var sw = Stopwatch.StartNew();
        var records = new List<SurveyRecord>();

        var resolvedPath = File.Exists(_settings.CsvFilePath)
            ? _settings.CsvFilePath
            : Path.Combine(AppContext.BaseDirectory, _settings.CsvFilePath);

        if (!File.Exists(resolvedPath))
        {
            _logger.LogWarning("Archivo CSV no encontrado en la ruta: {Path}", resolvedPath);
            return new ExtractionResult(SourceName, 0, sw.ElapsedMilliseconds, records);
        }

        using var reader = new StreamReader(resolvedPath);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        await foreach (var record in csv.GetRecordsAsync<SurveyRecord>(cancellationToken))
        {
            records.Add(record);
        }

        sw.Stop();
        return new ExtractionResult(SourceName, records.Count, sw.ElapsedMilliseconds, records);
    }
}
