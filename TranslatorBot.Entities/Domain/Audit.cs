namespace TranslatorBot.Entities.Domain;

public class Audit {
    public DateTime CreatedOn { get; protected set; }
    public DateTime ModifiedOn { get; protected set; }

    public void PerformCreateAudit(DateTime now) {
        CreatedOn = now;
        ModifiedOn = now;
    }

    public void PerformModifyAudit(DateTime now) {
        ModifiedOn = now;
    }
}