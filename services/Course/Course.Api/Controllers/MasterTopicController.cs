using Microsoft.AspNetCore.Mvc;
using Course.Application.Interfaces;
using Course.Application.DTOs.MasterTopicDTOs;
using Contracts.Common;

namespace Course.Api.Controllers
{
    [ApiController]
    [Route("api/master-topics")]
    public class MasterTopicController : ControllerBase
    {
        private readonly IMasterTopicService _masterTopicService;
        private readonly ILogger<MasterTopicController> _logger;

        public MasterTopicController(IMasterTopicService masterTopicService, ILogger<MasterTopicController> logger)
        {
            _masterTopicService = masterTopicService;
            _logger = logger;
        }

        /// <summary>
        /// Get all master topics
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAllMasterTopics([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var masterTopics = await _masterTopicService.GetAllAsync(page, pageSize);
                return Ok(ApiResponse<BasePaginatedList<MasterTopicResponse>>.Ok(masterTopics, "Get all master topics successfully", "200"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error at the {Controller}: {Message}", nameof(MasterTopicController), ex.Message);
                return StatusCode(500, ApiResponse<string>.InternalError($"Error at the {nameof(MasterTopicController)}: {ex.Message}"));
            }
        }

        /// <summary>
        /// Get master topic by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMasterTopicById([FromRoute] Guid id)
        {
            try
            {
                var masterTopic = await _masterTopicService.GetByIdAsync(id);
                return Ok(ApiResponse<MasterTopicResponse>.Ok(masterTopic, "Get master topic by id successfully", "200"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error at the {Controller}: {Message}", nameof(MasterTopicController), ex.Message);
                return StatusCode(500, ApiResponse<string>.InternalError($"Error at the {nameof(MasterTopicController)}: {ex.Message}"));
            }
        }

        /// <summary>
        /// Create a new master topic
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateMasterTopic([FromForm] MasterTopicCreateRequest request)
        {
            try
            {
                var masterTopic = await _masterTopicService.CreateAsync(request);
                return Ok(ApiResponse<MasterTopicResponse>.Ok(masterTopic, "Create master topic successfully", "200"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error at the {Controller}: {Message}", nameof(MasterTopicController), ex.Message);
                return StatusCode(500, ApiResponse<string>.InternalError($"Error at the {nameof(MasterTopicController)}: {ex.Message}"));
            }
        }

        /// <summary>
        /// Update a master topic
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMasterTopic(Guid id, [FromForm] MasterTopicUpdateRequest request)
        {
            try
            {
                var masterTopic = await _masterTopicService.UpdateAsync(id, request);
                return Ok(ApiResponse<MasterTopicResponse>.Ok(masterTopic, "Update master topic successfully", "200"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error at the {Controller}: {Message}", nameof(MasterTopicController), ex.Message);
                return StatusCode(500, ApiResponse<string>.InternalError($"Error at the {nameof(MasterTopicController)}: {ex.Message}"));
            }
        }

        /// <summary>
        /// Delete a master topic
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMasterTopic([FromRoute] Guid id)
        {
            try
            {
                var masterTopic = await _masterTopicService.DeleteAsync(id);
                return Ok(ApiResponse<MasterTopicResponse>.Ok(masterTopic, "Delete master topic successfully", "200"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error at the {Controller}: {Message}", nameof(MasterTopicController), ex.Message);
                return StatusCode(500, ApiResponse<string>.InternalError($"Error at the {nameof(MasterTopicController)}: {ex.Message}"));
            }
        }
    }
}