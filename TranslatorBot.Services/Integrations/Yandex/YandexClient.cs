using System.Net.Http.Headers;
using System.Text;
using TranslatorBot.Entities.Data;
using TranslatorBot.Entities.Options;

namespace TranslatorBot.Services.Integrations.Yandex;

public class YandexClient : IYandexClient {
    private readonly HttpClient _client;
    private readonly IYandexCloudTokenRepository _repository;
    private readonly YandexTranslateOptions _options;

    public YandexClient(
        IYandexCloudTokenRepository repository,
        YandexTranslateOptions options,
        HttpClient client
    ) {
        _repository = repository;
        _options = options;
        _client = client;
    }

    public async Task<Language?> DetectLanguageAsync(DetectLanguageRequest request) {
        return await TranslateApiPostAsync<DetectLanguageRequest, Language>(request, "detect");
    }

    public async Task<TranslateResult?> TranslateAsync(TranslateRequest request) {
        return await TranslateApiPostAsync<TranslateRequest, TranslateResult>(request, "translate");
    }

    public async Task<IamToken?> GetIamTokenAsync() {
        var request = new CreateIamTokenRequest {
            YandexPassportOauthToken = _options.OauthToken
        };
        return await IamApiPostAsync<CreateIamTokenRequest, IamToken>(request);
    }

    private async Task<TResult?> IamApiPostAsync<TRequest, TResult>(TRequest request) {
        var jsonContent = CreateJsonContent(request);
        var response = await _client.PostAsync("https://iam.api.cloud.yandex.net/iam/v1/tokens", jsonContent);
        var responseJson = await response.Content.ReadAsStringAsync();
        return YandexJson.DeserializeObject<TResult>(responseJson);
    }

    private async Task<TResult?> TranslateApiPostAsync<TRequest, TResult>(TRequest request, string method) {
        var jsonContent = CreateJsonContent(request);
        await ConfigureRequestHeaders();
        var response = await _client.PostAsync($"https://translate.api.cloud.yandex.net/translate/v2/{method}", jsonContent);
        var responseJson = await response.Content.ReadAsStringAsync();
        return YandexJson.DeserializeObject<TResult>(responseJson);
    }

    private static HttpContent CreateJsonContent<T>(T request) {
        var json = YandexJson.SerializeObject(request);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }

    private async Task ConfigureRequestHeaders() {
        var token = (await _repository.ListAllAsync()).FirstOrDefault();
        _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token?.Token ?? string.Empty);
    }
}