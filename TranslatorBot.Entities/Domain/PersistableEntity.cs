namespace TranslatorBot.Entities.Domain;

public abstract class PersistableEntity {
    public Guid Id { get; protected set; }
}