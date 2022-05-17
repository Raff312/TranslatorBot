namespace TranslatorBot.Entities.Domain;

public class YandexCloudToken : AuditableEntity {
    public string? Token { get; private set; }

    public void SetToken(string? token) {
        Token = token;
    }
}