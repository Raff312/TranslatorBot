using Newtonsoft.Json;

namespace TranslatorBot.Services.Integrations.Yandex;

// https://cloud.yandex.ru/docs/iam/operations/iam-token/create
public class CreateIamTokenRequest {
    [JsonProperty("yandexPassportOauthToken")]
    public string? YandexPassportOauthToken { get; set; }
}