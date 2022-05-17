using TranslatorBot.Data.MongoDb.Repositories.Common;
using TranslatorBot.Entities.Data;
using TranslatorBot.Entities.Domain;

namespace TranslatorBot.Data.MongoDb.Repositories;

public class YandexCloudTokenRepository : DataRepository<YandexCloudToken>, IYandexCloudTokenRepository {
    public YandexCloudTokenRepository(IDataContext context) : base(context, "yandexTokens") {
    }
}