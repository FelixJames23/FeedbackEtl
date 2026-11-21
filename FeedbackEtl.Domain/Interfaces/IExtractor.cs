using FeedbackEtl.Domain.Entities;

namespace FeedbackEtl.Domain.Interfaces;

public interface IExtractor
{
    string SourceName { get; }
    Task<ExtractionResult> ExtractAsync(CancellationToken cancellationToken = default);
}
