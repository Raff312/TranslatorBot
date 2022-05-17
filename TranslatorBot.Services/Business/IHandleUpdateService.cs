using Telegram.Bot.Types;

namespace TranslatorBot.Services.Business;

public interface IHandleUpdateService {
    Task Handle(Update update);
}