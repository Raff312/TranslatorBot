namespace TranslatorBot.Entities.Domain;

public abstract class AuditableEntity : PersistableEntity, IAuditable {
    public Audit Audit { get; protected set; } = new Audit();
}