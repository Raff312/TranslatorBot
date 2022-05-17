using Telegram.Bot;
using TranslatorBot.Data.MongoDb;
using TranslatorBot.Entities.Options;
using TranslatorBot.Services.Business;
using TranslatorBot.Services.Infrastructure;
using TranslatorBot.Services.Infrastructure.IoC;
using TranslatorBot.Services.Integrations.Yandex;

namespace TranslatorBot.WebApi;

public class Startup {
    public static IConfigurationRoot Configuration { get; private set; } = default!;

    public Startup() {
        BuildConfiguration();
    }

    private static void BuildConfiguration() {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFiles();

        Configuration = builder.Build();
    }

    public static void ConfigureServices(IServiceCollection services) {
        DataLayerConfiguration.Configure();

        services
            .AddSingleton(Configuration.GetSection("Database").Get<MongoDatabaseOptions>())
            .AddSingleton(Configuration.GetSection("YandexTranslateOptions").Get<YandexTranslateOptions>())
            .AddMongoDb()
            .AddRepositories();

        services
            .AddHttpClient("tg")
            .AddTypedClient<ITelegramBotClient>(httpClient => new TelegramBotClient(Configuration["BotConfiguration:Token"], httpClient));

        services
            .AddHttpClient("ynd")
            .AddTypedClient<IYandexClient, YandexClient>();

        services.AddScoped<IHandleUpdateService, HandleUpdateService>();

        services
            .AddControllers()
            .AddNewtonsoftJson();
    }

    public static void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
        if (env.IsDevelopment()) {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();
        app.UseCors();

        app.UseEndpoints(endpoints => {
            var token = Configuration["BotConfiguration:Token"];
            endpoints.MapControllerRoute(
                name: "tgwebhook",
                pattern: $"bot/{token}",
                new { controller = "Webhook", action = "Post" });
            endpoints.MapControllers();
        });
    }
}