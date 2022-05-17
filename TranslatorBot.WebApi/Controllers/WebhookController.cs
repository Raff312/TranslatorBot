using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using TranslatorBot.Services.Business;

namespace TranslatorBot.WebApi.Controllers;

public class WebhookController : ControllerBase {
    private readonly IHandleUpdateService _handleUpdateService;

    public WebhookController(IHandleUpdateService handleUpdateService) {
        _handleUpdateService = handleUpdateService;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Update update) {
        await _handleUpdateService.Handle(update);
        return Ok();
    }
}