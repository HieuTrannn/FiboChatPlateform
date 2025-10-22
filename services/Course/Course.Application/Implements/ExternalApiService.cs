using Contracts.Common;
using Course.Application.DTOs.MasterTopicDTOs;
using Course.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Course.Application.Implements
{
    public class ExternalApiService : IExternalApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ExternalApiService> _logger;
        private readonly string _gatewayBaseUrl;

        public ExternalApiService(HttpClient httpClient, IConfiguration configuration, ILogger<ExternalApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _gatewayBaseUrl = configuration["Gateway:BaseUrl"] ?? throw new ArgumentNullException("Gateway:BaseUrl");
        }

        public async Task<SemesterResponse?> GetSemesterByIdAsync(string semesterId)
        {
            try
            {
                _logger.LogInformation("Calling semester API: {Url}", $"{_gatewayBaseUrl}/auth/api/semesters/{semesterId}");
                var response = await _httpClient.GetAsync($"{_gatewayBaseUrl}/auth/api/semesters/{semesterId}");
                
                _logger.LogInformation("Semester API response status: {StatusCode}", response.StatusCode);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("Semester API response: {Content}", content); // Debug log
                    
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse<SemesterResponse>>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    
                    return apiResponse?.Data;
                }
                
                _logger.LogWarning("Failed to get semester {SemesterId}. Status: {StatusCode}", semesterId, response.StatusCode);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting semester {SemesterId}", semesterId);
                return null;
            }
        }

        public async Task<LecturerResponse?> GetLecturerByIdAsync(string lecturerId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_gatewayBaseUrl}/auth/api/lecturers/{lecturerId}");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse<LecturerResponse>>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    
                    return apiResponse?.Data;
                }
                
                _logger.LogWarning("Failed to get lecturer {LecturerId}. Status: {StatusCode}", lecturerId, response.StatusCode);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting lecturer {LecturerId}", lecturerId);
                return null;
            }
        }
    }
}