namespace TranslatorBot.Entities.Domain;

public interface IAuditable {
    Audit Audit { get; }
}