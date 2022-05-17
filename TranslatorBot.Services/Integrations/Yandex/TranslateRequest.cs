using Newtonsoft.Json;

namespace TranslatorBot.Services.Integrations.Yandex;

// https://cloud.yandex.ru/docs/translate/api-ref/Translation/translate
public class TranslateRequest {
    [JsonProperty("sourceLanguageCode")]
    public string? SourceLanguageCode { get; set; }

    [JsonProperty("targetLanguageCode")]
    public string? TargetLanguageCode { get; set; }

    [JsonProperty("format")]
    public string? Format { get; set; }

    [JsonProperty("texts")]
    public string[]? Texts { get; set; }

    [JsonProperty("folderId")]
    public string? FolderId { get; set; }
}