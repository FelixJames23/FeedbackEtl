using FeedbackEtl.Application.Services;
using FeedbackEtl.Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace FeedbackEtl.Worker;

public class Worker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly EtlSettings _settings;
    private readonly ILogger<Worker> _logger;

    public Worker(
        IServiceScopeFactory scopeFactory,
        IOptions<EtlSettings> settings,
        ILogger<Worker> logger)
    {
        _scopeFactory = scopeFactory;
        _settings = settings.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker Service ETL iniciado.");

        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var orchestrator = scope.ServiceProvider.GetRequiredService<ExtractionOrchestrator>();
                await orchestrator.RunExtractionPipelineAsync(stoppingToken);
            }

            await Task.Delay(TimeSpan.FromSeconds(_settings.PollingIntervalSeconds), stoppingToken);
        }
    }
}
