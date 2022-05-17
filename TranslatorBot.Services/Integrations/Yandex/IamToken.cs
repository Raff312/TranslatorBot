using Newtonsoft.Json;

namespace TranslatorBot.Services.Integrations.Yandex;

// https://cloud.yandex.ru/docs/iam/operations/iam-token/create
public class IamToken {
    [JsonProperty("iamToken")]
    public string? Token { get; set; }

    [JsonProperty("expiresAt")]
    public DateTime? ExpiresAt { get; set; }
}