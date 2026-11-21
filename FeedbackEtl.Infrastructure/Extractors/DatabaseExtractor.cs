using System.Diagnostics;
using FeedbackEtl.Domain.Entities;
using FeedbackEtl.Domain.Interfaces;
using FeedbackEtl.Infrastructure.Configuration;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FeedbackEtl.Infrastructure.Extractors;

public class DatabaseExtractor : IExtractor
{
    private readonly EtlSettings _settings;
    private readonly ILogger<DatabaseExtractor> _logger;

    public string SourceName => "SQL_ResenasClientes";

    public DatabaseExtractor(IOptions<EtlSettings> settings, ILogger<DatabaseExtractor> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<ExtractionResult> ExtractAsync(CancellationToken cancellationToken = default)
    {
        var sw = Stopwatch.StartNew();
        var reviews = new List<ReviewRecord>();

        const string query = """
            SELECT ReviewId, ProductId, Rating, Comment, CreatedAt
            FROM Resenas
            WHERE CreatedAt >= DATEADD(DAY, -30, GETUTCDATE())
            """;

        await using var connection = new SqlConnection(_settings.RelationalDbConnectionString);
        await connection.OpenAsync(cancellationToken);

        await using var command = new SqlCommand(query, connection);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            reviews.Add(new ReviewRecord
            {
                ReviewId = reader.GetInt32(0),
                ProductId = reader.GetInt32(1),
                Rating = reader.GetInt32(2),
                Comment = reader.GetString(3),
                CreatedAt = reader.GetDateTime(4)
            });
        }

        sw.Stop();
        _logger.LogDebug("Consulta SQL ejecutada correctamente.");
        return new ExtractionResult(SourceName, reviews.Count, sw.ElapsedMilliseconds, reviews);
    }
}
