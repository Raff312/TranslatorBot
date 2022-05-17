using MongoDB.Driver;
using MongoDBMigrations;
using Version = MongoDBMigrations.Version;

namespace TranslatorBot.Initializer.Migrations;

public class M_0_0_1_Init : IMigration {
    public Version Version => new(0, 0, 1);
    public string Name => "Init collections";

    public void Up(IMongoDatabase database) {
        try {
            database.CreateCollection("yandexTokens");
        } catch (Exception ex) {
            Console.WriteLine(ex.Message);
        }
    }

    public void Down(IMongoDatabase database) {
        try {
            database.DropCollection("yandexTokens");
        } catch (Exception ex) {
            Console.WriteLine(ex.Message);
        }
    }
}