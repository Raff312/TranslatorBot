using TranslatorBot.Entities.Data;
using TranslatorBot.Entities.Domain;
using TranslatorBot.Services.Integrations.Yandex;

namespace TranslatorBot.Services.Business;

public class YandexService : IYandexService {
    private readonly IYandexCloudTokenRepository _repository;
    private readonly IYandexClient _client;

    public YandexService(
        IYandexCloudTokenRepository repository,
        IYandexClient client
    ) {
        _repository = repository;
        _client = client;
    }

    public async Task UpdateOrCreateIamToken() {
        var newIamToken = await _client.GetIamTokenAsync();
        var yandexCloudToken = (await _repository.ListAllAsync()).FirstOrDefault();
        if (yandexCloudToken == null) {
            yandexCloudToken = new YandexCloudToken();
            yandexCloudToken.SetToken(newIamToken?.Token);
            await _repository.CreateAsync(yandexCloudToken);
        } else {
            yandexCloudToken.SetToken(newIamToken?.Token);
            await _repository.UpdateAsync(yandexCloudToken);
        }
    }
}