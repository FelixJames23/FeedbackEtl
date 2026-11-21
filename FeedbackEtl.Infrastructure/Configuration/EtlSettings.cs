namespace FeedbackEtl.Infrastructure.Configuration;

public class EtlSettings
{
    public string CsvFilePath { get; set; } = string.Empty;
    public string RelationalDbConnectionString { get; set; } = string.Empty;
    public string CommentsApiUrl { get; set; } = string.Empty;
    public string StagingOutputFolder { get; set; } = "StagingOutput";
    public int PollingIntervalSeconds { get; set; } = 300;
}
