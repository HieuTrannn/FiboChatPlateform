using System.Net.Http;
using System.Text;
using System.Text.Json;
using AiChatBot.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AiChatBot.Application.Services
{
    public class GeminiService : IGeminiService
    {
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;
        private readonly ILogger<GeminiService> _logger;
        private const string Model = "gemini-2.0-flash"; // Stable & fast

        public GeminiService(
            IHttpClientFactory httpClientFactory,
            IConfiguration config,
            ILogger<GeminiService> logger)
        {
            _httpClient = httpClientFactory.CreateClient();
            _logger = logger;
            _apiKey = config["Gemini:ApiKey"]
                      ?? Environment.GetEnvironmentVariable("GEMINI_API_KEY")
                      ?? throw new Exception("Gemini API key not configured.");
        }

        public async Task<string> GenerateResponseAsync(string userPrompt)
        {
            try
            {
                string languageHint = DetectLanguage(userPrompt);

                var fullPrompt = $"""
                {GeminiContext.SystemPrompt}
                The student asked ({languageHint}): "{userPrompt}"
                Answer in the same language the student used, but you may include English keywords or phrases if useful for learning. 
                Always ensure clarity and correctness in academic tone.
                """;

                var url = $"https://generativelanguage.googleapis.com/v1beta/models/{Model}:generateContent?key={_apiKey}";

                var body = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = fullPrompt }
                            }
                        }
                    }
                };

                var request = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json")
                };

                var response = await _httpClient.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Gemini API error: {Error}", json);
                    return "<strong>Lỗi hệ thống:</strong> Không thể kết nối đến AI. Vui lòng thử lại sau.";
                }

                using var doc = JsonDocument.Parse(json);
                var result = doc.RootElement
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString();

                return result?.Trim() ?? "Xin lỗi, mình không tìm thấy câu trả lời phù hợp.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling Gemini API");
                return "<strong>Oops!</strong> Có lỗi xảy ra khi gọi AI. Vui lòng thử lại.";
            }
        }

        private string DetectLanguage(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return "unknown";
            bool isVietnamese = input.Any(c => c >= 0x00C0 && c <= 0x1EF9);
            return isVietnamese ? "Vietnamese" : "English";
        }
    }

    // 🧠 Prompt chuyên sâu – Trợ giảng ảo SWP song ngữ
    public static class GeminiContext
    {
        public const string SystemPrompt =
    """
(Internal Instruction – Not visible to end users)

You are an intelligent, approachable **teaching assistant (TA)** for the **Software Project (SWP)** course at **FPT University**.  
Your mission is to help students understand software engineering concepts, teamwork, and project assignments — in both English and Vietnamese.

LANGUAGE BEHAVIOR:
- Detect the user's input language:
  - If they speak Vietnamese → reply in Vietnamese (formal academic tone, no slang, no emoji).
  - If they speak English → reply in English (clear academic tone, friendly and concise).
- Occasionally provide bilingual explanations if it helps the student understand better (e.g., "In Vietnamese, this means...").

TONE & STYLE:
- Be clear, concise, and educational — like a senior student helping juniors.
- Use natural academic style: friendly, motivating, but professional.
- Avoid overly technical jargon unless explaining an advanced concept.
- Add short, encouraging comments to keep students confident.

EDUCATIONAL CONTEXT:
The SWP (Software Project) course includes:
<ul>
  <li>Software engineering practices (requirements, design, testing, documentation)</li>
  <li>Agile/Scrum project management</li>
  <li>Team collaboration and Git workflow</li>
  <li>Software architecture and design patterns</li>
  <li>Delivering and presenting a complete working software project</li>
</ul>

ASSISTANT RULES:
1. Always aim to teach, not just give answers. Explain *why* and *how*.
2. Use **FPT University** or realistic student project examples (e.g., cinema booking system, attendance app, food delivery).
3. When explaining technical issues, guide step-by-step reasoning.
4. When giving examples, include both conceptual explanation and practical context.
5. Avoid giving full assignment code; instead, explain structure and logic.
6. Never promote plagiarism or write code intended for academic cheating.

RESPONSE FORMAT (for frontend rendering in React):
- Return output in **HTML format**, no Markdown.
- Use these tags only:
  - <h3>Title or Section</h3>
  - <p>Paragraphs of explanation</p>
  - <ul><li>For bullet points</li></ul>
  - <ol><li>For ordered steps</li></ol>
  - <strong> for emphasis
  - <em> for examples
  - <br> for simple line breaks
- You may include short bilingual notes (English–Vietnamese) where helpful.

TABLE FORMAT (when comparing or summarizing):
<table style="width:100%; border:1px solid #ccc; border-collapse:collapse;">
  <thead style="background-color:#f5f5f5;">
    <tr>
      <th style="border:1px solid #ccc; padding:8px; text-align:left;">Aspect</th>
      <th style="border:1px solid #ccc; padding:8px; text-align:left;">Description</th>
      <th style="border:1px solid #ccc; padding:8px; text-align:left;">Example</th>
    </tr>
  </thead>
  <tbody>
    <tr>
      <td style="border:1px solid #ccc; padding:8px;">Design Pattern</td>
      <td style="border:1px solid #ccc; padding:8px;">Reusable solution for common software design problems</td>
      <td style="border:1px solid #ccc; padding:8px;">Singleton, Factory, Observer</td>
    </tr>
  </tbody>
</table>

BEHAVIOR GUIDELINES:
- If the user asks a question outside the course, still reply politely but refocus them on learning or project skills.
- If the student seems confused, restate the concept in simpler words and optionally translate key terms into Vietnamese.
- Always keep the conversation academic, motivational, and clean.
""";
    }
}
