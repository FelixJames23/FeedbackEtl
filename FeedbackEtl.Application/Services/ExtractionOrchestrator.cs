using System.Diagnostics;
using FeedbackEtl.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace FeedbackEtl.Application.Services;

public class ExtractionOrchestrator
{
    private readonly IEnumerable<IExtractor> _extractors;
    private readonly IStagingService _stagingService;
    private readonly ILogger<ExtractionOrchestrator> _logger;

    public ExtractionOrchestrator(
        IEnumerable<IExtractor> extractors,
        IStagingService stagingService,
        ILogger<ExtractionOrchestrator> logger)
    {
        _extractors = extractors;
        _stagingService = stagingService;
        _logger = logger;
    }

    public async Task RunExtractionPipelineAsync(CancellationToken cancellationToken = default)
    {
        var globalWatch = Stopwatch.StartNew();
        _logger.LogInformation("Iniciando ciclo de extracción ETL con {Count} fuentes configuradas.", _extractors.Count());

        var extractionTasks = _extractors.Select(async extractor =>
        {
            try
            {
                var result = await extractor.ExtractAsync(cancellationToken);
                await _stagingService.SaveBatchAsync(result, cancellationToken);

                _logger.LogInformation(
                    "Fuente [{Source}] completada: {Records} registros extraídos y guardados en staging en {ElapsedMs} ms.",
                    result.SourceName, result.RecordCount, result.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error crítico durante la extracción de la fuente [{Source}].", extractor.SourceName);
            }
        });

        await Task.WhenAll(extractionTasks);

        globalWatch.Stop();
        _logger.LogInformation("Ciclo de extracción finalizado en {TotalMs} ms.", globalWatch.ElapsedMilliseconds);
    }
}
