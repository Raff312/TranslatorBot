using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TranslatorBot.Services.Integrations.Yandex;

public static class YandexJson {
    public static string SerializeObject<T>(T value) {
        return JsonConvert.SerializeObject(value, new StringEnumConverter());
    }

    public static T? DeserializeObject<T>(string value) {
        return JsonConvert.DeserializeObject<T>(value, new StringEnumConverter());
    }
}