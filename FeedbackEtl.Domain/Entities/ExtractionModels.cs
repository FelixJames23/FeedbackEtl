namespace FeedbackEtl.Domain.Entities;

public class SurveyRecord
{
    public int SurveyId { get; set; }
    public string CustomerEmail { get; set; } = string.Empty;
    public int SatisfactionScore { get; set; }
    public string FeedbackText { get; set; } = string.Empty;
    public DateTime SubmittedAt { get; set; }
}

public class ReviewRecord
{
    public int ReviewId { get; set; }
    public int ProductId { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class ApiCommentRecord
{
    public int Id { get; set; }
    public int PostId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
}

public record ExtractionResult(
    string SourceName,
    int RecordCount,
    long ElapsedMilliseconds,
    IEnumerable<object> Records
);
