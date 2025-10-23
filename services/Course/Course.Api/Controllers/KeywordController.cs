using Contracts.Common;
using Course.Application.DTOs.KeywordDTOs;
using Course.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Course.Api.Controllers
{
    [ApiController]
    [Route("api/keywords")]
    public class KeywordController : ControllerBase
    {
        private readonly IKeywordService _keywordService;
        private readonly ILogger<KeywordController> _logger;

        public KeywordController(IKeywordService keywordService, ILogger<KeywordController> logger)
        {
            _keywordService = keywordService;
            _logger = logger;
        }

        /// <summary>
        /// Get all keywords
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAllKeywords([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var keywords = await _keywordService.GetAllAsync(page, pageSize);
                return Ok(ApiResponse<BasePaginatedList<KeywordResponse>>.Ok(keywords, "Get all keywords successfully", "200"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error at the {Controller}: {Message}", nameof(KeywordController), ex.Message);
                return StatusCode(500, ApiResponse<string>.InternalError($"Error at the {nameof(KeywordController)}: {ex.Message}"));
            }
        }

        /// <summary>
        /// Get keyword by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetKeywordById([FromRoute] Guid id)
        {
            try
            {
                var keyword = await _keywordService.GetByIdAsync(id);
                return Ok(ApiResponse<KeywordResponse>.Ok(keyword, "Get keyword by id successfully", "200"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error at the {Controller}: {Message}", nameof(KeywordController), ex.Message);
                return StatusCode(500, ApiResponse<string>.InternalError($"Error at the {nameof(KeywordController)}: {ex.Message}"));
            }
        }

        /// <summary>
        /// Create a new keyword
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateKeyword([FromForm] KeywordCreateRequest request)
        {
            try
            {
                var keyword = await _keywordService.CreateAsync(request);
                return Ok(ApiResponse<KeywordResponse>.Ok(keyword, "Create keyword successfully", "200"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error at the {Controller}: {Message}", nameof(KeywordController), ex.Message);
                return StatusCode(500, ApiResponse<string>.InternalError($"Error at the {nameof(KeywordController)}: {ex.Message}"));
            }
        }

        /// <summary>
        /// Update a keyword
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateKeyword(Guid id, [FromForm] KeywordUpdateRequest request)
        {
            try
            {
                var keyword = await _keywordService.UpdateAsync(id, request);
                return Ok(ApiResponse<KeywordResponse>.Ok(keyword, "Update keyword successfully", "200"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error at the {Controller}: {Message}", nameof(KeywordController), ex.Message);
                return StatusCode(500, ApiResponse<string>.InternalError($"Error at the {nameof(KeywordController)}: {ex.Message}"));
            }
        }

        /// <summary>
        /// Delete a keyword
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteKeyword([FromRoute] Guid id)
        {
            try
            {
                await _keywordService.DeleteAsync(id);
                return Ok(ApiResponse<string>.OkResponse("Delete keyword successfully", "200"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error at the {Controller}: {Message}", nameof(KeywordController), ex.Message);
                return StatusCode(500, ApiResponse<string>.InternalError($"Error at the {nameof(KeywordController)}: {ex.Message}"));
            }
        }
    }
}