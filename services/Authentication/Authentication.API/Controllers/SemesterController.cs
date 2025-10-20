using Authentication.Application.Interfaces;
using Authentication.Application.DTOs.SemesterDTOs;
using Microsoft.AspNetCore.Mvc;
using Contracts.Common;

namespace Authentication.API.Controllers
{
    [ApiController]
    [Route("api/semesters")]
    public class SemesterController : ControllerBase
    {
        private readonly ISemesterService _semesterService;
        private readonly ILogger<SemesterController> _logger;

        public SemesterController(ISemesterService semesterService, ILogger<SemesterController> logger)
        {
            _semesterService = semesterService;
            _logger = logger;
        }

        /// <summary>
        /// Get all semesters
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAllSemesters(int page = 1, int pageSize = 10)
        {
            try {
                var semesters = await _semesterService.GetAllAsync(page, pageSize);
                return Ok(ApiResponse<BasePaginatedList<SemesterResponse>>.Ok(semesters, "Get all semesters successfully", "200"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error at the {Controller}: {Message}", nameof(SemesterController), ex.Message);
                return StatusCode(500, ApiResponse<string>.InternalError($"Error at the {nameof(SemesterController)}: {ex.Message}"));
            }
        }

        /// <summary>
        /// Get a semester by id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSemesterById(Guid id)
        {
            try {
                var semester = await _semesterService.GetByIdAsync(id);
                if (semester == null)
                {
                    return NotFound(ApiResponse<SemesterResponse>.NotFound("Semester not found"));
                }
                return Ok(ApiResponse<SemesterResponse>.Ok(semester, "Get semester by id successfully", "200"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error at the {Controller}: {Message}", nameof(SemesterController), ex.Message);
                return StatusCode(500, ApiResponse<string>.InternalError($"Error at the {nameof(SemesterController)}: {ex.Message}"));
            }
        }

        /// <summary>
        /// Create a new semester
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateSemester([FromForm] SemesterCreateRequest request)
        {
            try {
                var semester = await _semesterService.CreateAsync(request);
                if (semester == null)
                {
                    return BadRequest(ApiResponse<SemesterResponse>.BadRequest("Create semester failed"));
                }
                return Ok(ApiResponse<SemesterResponse>.Ok(semester, "Create semester successfully", "200"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error at the {Controller}: {Message}", nameof(SemesterController), ex.Message);
                return StatusCode(500, ApiResponse<string>.InternalError($"Error at the {nameof(SemesterController)}: {ex.Message}"));
            }
        }

        /// <summary>
        /// Update a semester
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSemester(Guid id, [FromForm] SemesterUpdateRequest request)
        {
            try {
                var semester = await _semesterService.UpdateAsync(id, request);
                if (semester == null)
                {
                    return BadRequest(ApiResponse<SemesterResponse>.BadRequest("Update semester failed"));
                }
                return Ok(ApiResponse<SemesterResponse>.Ok(semester, "Update semester successfully", "200"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error at the {Controller}: {Message}", nameof(SemesterController), ex.Message);
                return StatusCode(500, ApiResponse<string>.InternalError($"Error at the {nameof(SemesterController)}: {ex.Message}"));
            }
        }

        /// <summary>
        /// Delete a semester (active cannot be deleted)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSemester(Guid id)
        {
            try {
                var semester = await _semesterService.DeleteAsync(id);
                if (semester == null)
                {
                    return BadRequest(ApiResponse<SemesterResponse>.BadRequest("Delete semester failed"));
                }
                return Ok(ApiResponse<SemesterResponse>.Ok(semester, "Delete semester successfully", "200"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error at the {Controller}: {Message}", nameof(SemesterController), ex.Message);
                return StatusCode(500, ApiResponse<string>.InternalError($"Error at the {nameof(SemesterController)}: {ex.Message}"));
            }
        }
    }
}