using AiChatBot.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiChatBot.Application.Service
{
    public class GeminiService : IGeminiService
    {
        private readonly string _apiKey;
        //private readonly IRedisService _cacheService;
        private readonly HttpClient _httpClient;
    }
}
