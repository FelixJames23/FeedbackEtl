using System.Diagnostics;
using System.Net.Http.Json;
using FeedbackEtl.Domain.Entities;
using FeedbackEtl.Domain.Interfaces;
using FeedbackEtl.Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FeedbackEtl.Infrastructure.Extractors;

public class ApiExtractor : IExtractor
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly EtlSettings _settings;
    private readonly ILogger<ApiExtractor> _logger;

    public string SourceName => "REST_ApiComentarios";

    public ApiExtractor(
        IHttpClientFactory httpClientFactory,
        IOptions<EtlSettings> settings,
        ILogger<ApiExtractor> logger)
    {
        _httpClientFactory = httpClientFactory;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<ExtractionResult> ExtractAsync(CancellationToken cancellationToken = default)
    {
        var sw = Stopwatch.StartNew();
        var client = _httpClientFactory.CreateClient("CommentsClient");

        var comments = await client.GetFromJsonAsync<List<ApiCommentRecord>>(
            _settings.CommentsApiUrl, cancellationToken) ?? [];

        sw.Stop();
        _logger.LogDebug("Respuesta HTTP recibida desde {Url}.", _settings.CommentsApiUrl);
        return new ExtractionResult(SourceName, comments.Count, sw.ElapsedMilliseconds, comments);
    }
}
