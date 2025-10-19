using AiChatBot.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AIChatBot.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatbotController : ControllerBase
    {
        private readonly IGeminiService _geminiService;

        public ChatbotController(IGeminiService geminiService)
        {
            _geminiService = geminiService;
        }

       
        [HttpPost("ask")]
        public async Task<IActionResult> AskGemini([FromBody] string userPrompt)
        {
            if (string.IsNullOrWhiteSpace(userPrompt))
                return BadRequest("Prompt cannot be empty.");

            var response = await _geminiService.GenerateResponseAsync(userPrompt);
            return Ok(new { prompt = userPrompt, response });
        }
     }
}            