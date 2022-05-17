namespace TranslatorBot.JobManager.Infrastructure;

public class JobConfiguration {
    public string Type { get; set; } = string.Empty;
    public string Cron { get; set; } = string.Empty;

    public Type? JobType => System.Type.GetType(Type);
}