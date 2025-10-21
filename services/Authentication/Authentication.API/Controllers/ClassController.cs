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
        /// Get all classes (active and pending only)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAllClasses()
        {
            try
            {
                var classes = await _classService.GetAllAsync();
                if (classes == null)
                {
                    return NotFound(ApiResponse<List<ClassResponse>>.NotFound("Classes not found"));
                }
                return Ok(ApiResponse<List<ClassResponse>>.Ok(classes, "Get all classes successfully", "200"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error at the {Controller}: {Message}", nameof(ClassController), ex.Message);
                return StatusCode(500, ApiResponse<string>.InternalError($"Error at the {nameof(ClassController)}: {ex.Message}"));
            }
        }

       
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

     
        [HttpPost]
        public async Task<IActionResult> CreateClass(ClassCreateRequest request)
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

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClass(Guid id, ClassUpdateRequest request)
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
        public async Task<IActionResult> DeleteClass(Guid id)
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
    }
}