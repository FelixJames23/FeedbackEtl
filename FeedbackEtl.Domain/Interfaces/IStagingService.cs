using FeedbackEtl.Domain.Entities;

namespace FeedbackEtl.Domain.Interfaces;

public interface IStagingService
{
    Task SaveBatchAsync(ExtractionResult result, CancellationToken cancellationToken = default);
}
