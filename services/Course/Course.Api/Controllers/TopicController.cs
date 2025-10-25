using Microsoft.AspNetCore.Mvc;

using Course.Application.Interfaces;
using Contracts.Common;
using Course.Application.DTOs.TopicDTOs;

namespace Course.Api.Controllers
{
    [ApiController]
    [Route("api/topics")]
    public class TopicController : ControllerBase
    {
        private readonly ITopicService _topicService;
        private readonly ILogger<TopicController> _logger;

        public TopicController(ITopicService topicService, ILogger<TopicController> logger)
        {
            _topicService = topicService;
            _logger = logger;
        }

        /// <summary>
        /// Get all topics (active only)
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAllTopics([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var topics = await _topicService.GetAllAsync(page, pageSize);
                return Ok(ApiResponse<BasePaginatedList<TopicResponse>>.Ok(topics, "Get all topics successfully", "200"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error at the {Controller}: {Message}", nameof(TopicController), ex.Message);
                return StatusCode(500, ApiResponse<string>.InternalError($"Error at the {nameof(TopicController)}: {ex.Message}"));
            }
        }

        /// <summary>
        /// Get topic by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTopicById([FromRoute] Guid id)
        {
            try
            {
                var topic = await _topicService.GetByIdAsync(id);
                return Ok(ApiResponse<TopicResponse>.Ok(topic, "Get topic by id successfully", "200"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error at the {Controller}: {Message}", nameof(TopicController), ex.Message);
                return StatusCode(500, ApiResponse<string>.InternalError($"Error at the {nameof(TopicController)}: {ex.Message}"));
            }
        }

        /// <summary>
        /// Create a new topic
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateTopic([FromForm] TopicCreateRequest request)
        {
            try
            {
                var topic = await _topicService.CreateAsync(request);
                return Ok(ApiResponse<TopicResponse>.Ok(topic, "Create topic successfully", "200"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error at the {Controller}: {Message}", nameof(TopicController), ex.Message);
                return StatusCode(500, ApiResponse<string>.InternalError($"Error at the {nameof(TopicController)}: {ex.Message}"));
            }
        }

        /// <summary>
        /// Update a topic
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTopic(Guid id, [FromForm] TopicUpdateRequest request)
        {
            try
            {
                var topic = await _topicService.UpdateAsync(id, request);
                return Ok(ApiResponse<TopicResponse>.Ok(topic, "Update topic successfully", "200"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error at the {Controller}: {Message}", nameof(TopicController), ex.Message);
                return StatusCode(500, ApiResponse<string>.InternalError($"Error at the {nameof(TopicController)}: {ex.Message}"));
            }
        }

        /// <summary>
        /// Delete a topic
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTopic([FromRoute] Guid id)
        {
            try
            {
                var topic = await _topicService.DeleteAsync(id);
                return Ok(ApiResponse<TopicResponse>.Ok(topic, "Delete topic successfully", "200"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error at the {Controller}: {Message}", nameof(TopicController), ex.Message);
                return StatusCode(500, ApiResponse<string>.InternalError($"Error at the {nameof(TopicController)}: {ex.Message}"));
            }
        }

        /// <summary>
        /// Get all topics of a master topic
        /// </summary>
        /// <param name="masterTopicId"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("master-topic/{masterTopicId}")]
        public async Task<IActionResult> GetAllTopicsOfMasterTopic([FromRoute] Guid masterTopicId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var topics = await _topicService.GetAllTopicsOfMasterTopicAsync(masterTopicId, page, pageSize);
                return Ok(ApiResponse<BasePaginatedList<TopicMasterTopicResponse>>.Ok(topics, "Get all topics of master topic successfully", "200"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error at the {Controller}: {Message}", nameof(TopicController), ex.Message);
                return StatusCode(500, ApiResponse<string>.InternalError($"Error at the {nameof(TopicController)}: {ex.Message}"));
            }
        }
    }
}