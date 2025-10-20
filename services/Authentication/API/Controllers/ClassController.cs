using Microsoft.AspNetCore.Mvc;
using Authentication.Application.Interfaces;
using Authentication.Application.DTOs.ClassDTOs;
using Contracts.Common;

namespace Authentication.API.Controllers
{
    [ApiController]
    [Route("api/classes")]
    public class ClassController : ControllerBase
    {
        private readonly IClassService _classService;
        private readonly ILogger<ClassController> _logger;

        public ClassController(IClassService classService, ILogger<ClassController> logger)
        {
            _classService = classService;
            _logger = logger;
        }

        /// <summary>
        /// Get all classes
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAllClasses(int page = 1, int pageSize = 10)
        {
            try
            {
                var classes = await _classService.GetAllAsync(page, pageSize);
                return Ok(ApiResponse<BasePaginatedList<ClassResponse>>.Ok(classes, "Get all classes successfully", "200"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error at the {Controller}: {Message}", nameof(ClassController), ex.Message);
                return StatusCode(500, ApiResponse<string>.InternalError($"Error at the {nameof(ClassController)}: {ex.Message}"));
            }
        }

        /// <summary>
        /// Get a class by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetClassById(Guid id)
        {
            try
            {
                var c = await _classService.GetByIdAsync(id);
                if (c == null)
                {
                    return NotFound(ApiResponse<ClassResponse>.NotFound("Class not found"));
                }
                return Ok(ApiResponse<ClassResponse>.Ok(c, "Get class by id successfully", "200"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error at the {Controller}: {Message}", nameof(ClassController), ex.Message);
                return StatusCode(500, ApiResponse<string>.InternalError($"Error at the {nameof(ClassController)}: {ex.Message}"));
            }
        }

        /// <summary>
        /// Create a new class 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateClass([FromForm] ClassCreateRequest request)
        {
            try
            {
                var c = await _classService.CreateAsync(request);
                if (c == null)
                {
                    return BadRequest(ApiResponse<ClassResponse>.BadRequest("Create class failed"));
                }
                return Ok(ApiResponse<ClassResponse>.Ok(c, "Create class successfully", "200"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error at the {Controller}: {Message}", nameof(ClassController), ex.Message);
                return StatusCode(500, ApiResponse<string>.InternalError($"Error at the {nameof(ClassController)}: {ex.Message}"));
            }
        }

        /// <summary>
        /// Update a class
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClass(Guid id, [FromForm] ClassUpdateRequest request)
        {
            try
            {
                var c = await _classService.UpdateAsync(id, request);
                if (c == null)
                {
                    return BadRequest(ApiResponse<ClassResponse>.BadRequest("Update class failed"));
                }
                return Ok(ApiResponse<ClassResponse>.Ok(c, "Update class successfully", "200"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error at the {Controller}: {Message}", nameof(ClassController), ex.Message);
                return StatusCode(500, ApiResponse<string>.InternalError($"Error at the {nameof(ClassController)}: {ex.Message}"));
            }
        }

        /// <summary>
        /// Delete a class (active cannot be deleted)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClass([FromRoute] Guid id)
        {
            try
            {
                var c = await _classService.DeleteAsync(id);
                if (c == null)
                {
                    return BadRequest(ApiResponse<ClassResponse>.BadRequest("Delete class failed"));
                }
                return Ok(ApiResponse<ClassResponse>.Ok(c, "Delete class successfully", "200"));
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error at the {Controller}: {Message}", nameof(ClassController), ex.Message);
                return StatusCode(500, ApiResponse<string>.InternalError($"Error at the {nameof(ClassController)}: {ex.Message}"));
            }
        }

        /// <summary>
        /// Assign a lecturer to a class
        /// </summary>
        /// <param name="classId"></param>
        /// <param name="lecturerId"></param>
        /// <returns></returns>
        [HttpPost("{classId}/assign-lecturer")]
        public async Task<IActionResult> AssignLecturer(Guid classId, Guid lecturerId)
        {
            try
            {
                var c = await _classService.AssignLecturerAsync(classId, lecturerId);
                if (c == null)
                {
                    return BadRequest(ApiResponse<ClassResponse>.BadRequest("Assign lecturer failed"));
                }
                return Ok(ApiResponse<ClassResponse>.Ok(c, "Assign lecturer successfully", "200"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error at the {Controller}: {Message}", nameof(ClassController), ex.Message);
                return StatusCode(500, ApiResponse<string>.InternalError($"Error at the {nameof(ClassController)}: {ex.Message}"));
            }
        }

        /// <summary>
        /// Unassign a lecturer from a class
        /// </summary>
        /// <param name="classId"></param>
        /// <returns></returns>
        [HttpPost("{classId}/unassign-lecturer")]
        public async Task<IActionResult> UnassignLecturer(Guid classId)
        {
            try
            {
                var c = await _classService.UnassignLecturerAsync(classId);
                if (c == null)
                {
                    return BadRequest(ApiResponse<ClassResponse>.BadRequest("Unassign lecturer failed"));
                }
                return Ok(ApiResponse<ClassResponse>.Ok(c, "Unassign lecturer successfully", "200"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error at the {Controller}: {Message}", nameof(ClassController), ex.Message);
                return StatusCode(500, ApiResponse<string>.InternalError($"Error at the {nameof(ClassController)}: {ex.Message}"));
            }
        }

        /// Search classes
        /// </summary>
        /// <param name="request">Search query parameters</param>
        /// <returns></returns>
        [HttpGet("search")]
        public async Task<IActionResult> SearchClasses([FromQuery] ClassQueryRequest request)
        {
            try
            {
                var classes = await _classService.SearchAsync(request);
                if (classes == null)
                {
                    return NotFound(ApiResponse<PaginatedResponse<ClassResponse>>.NotFound("No classes found"));
                }
                return Ok(ApiResponse<PaginatedResponse<ClassResponse>>.Ok(classes, "Search classes successfully", "200"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error at the {Controller}: {Message}", nameof(ClassController), ex.Message);
                return StatusCode(500, ApiResponse<string>.InternalError($"Error at the {nameof(ClassController)}: {ex.Message}"));
            }
        }
    }
}