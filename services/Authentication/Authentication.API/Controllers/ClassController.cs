using Microsoft.AspNetCore.Mvc;
using Authentication.Application.Interfaces;
using Authentication.Application.DTOs.ClassDTOs;
using Contracts.Common;
using Authentication.Domain.Exceptions;

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
        /// Get all classes of a lecturer
        /// </summary>
        /// <param name="lecturerId"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("lecturer/{lecturerId}")]
        public async Task<IActionResult> GetAllClassesOfLecturer(Guid lecturerId, int page = 1, int pageSize = 10)
        {
            try
            {
                var classes = await _classService.GetAllClassesOfLecturerAsync(lecturerId, page, pageSize);
                return Ok(ApiResponse<BasePaginatedList<ClassLectrurerResponse>>.Ok(classes, "Get all classes of lecturer successfully", "200"));
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
        // [HttpGet("search")]
        // public async Task<IActionResult> SearchClasses([FromQuery] ClassQueryRequest request)
        // {
        //     try
        //     {
        //         var classes = await _classService.SearchAsync(request);
        //         if (classes == null)
        //         {
        //             return NotFound(ApiResponse<PaginatedResponse<ClassResponse>>.NotFound("No classes found"));
        //         }
        //         return Ok(ApiResponse<PaginatedResponse<ClassResponse>>.Ok(classes, "Search classes successfully", "200"));
        //     }
        //     catch (Exception ex)
        //     {
        //         _logger.LogError(ex, "Error at the {Controller}: {Message}", nameof(ClassController), ex.Message);
        //         return StatusCode(500, ApiResponse<string>.InternalError($"Error at the {nameof(ClassController)}: {ex.Message}"));
        //     }
        // }


        /// <summary>
        /// Add students to a class
        /// </summary>
        /// <param name="classId"></param>
        /// <param name="userIds"></param>
        /// <returns></returns>
        [HttpPost("{classId}/add-students")]
        public async Task<IActionResult> AddStudentsToClass([FromRoute] Guid classId, [FromForm] List<Guid> userIds)
        {
            try
            {
                // Validation
                if (userIds == null || !userIds.Any())
                {
                    _logger.LogWarning("No user IDs provided for class {ClassId}", classId);
                    return BadRequest(ApiResponse<ClassStudentResponse>.BadRequest("User IDs cannot be empty"));
                }

                if (userIds.Any(id => id == Guid.Empty))
                {
                    _logger.LogWarning("Invalid user ID provided for class {ClassId}", classId);
                    return BadRequest(ApiResponse<ClassStudentResponse>.BadRequest("Invalid user ID provided"));
                }

                _logger.LogInformation("Adding {Count} students to class {ClassId}", userIds.Count, classId);

                var result = await _classService.AddStudentToClassAsync(classId, userIds);
                if (result == null)
                {
                    _logger.LogError("Failed to add students to class {ClassId}", classId);
                    return BadRequest(ApiResponse<ClassStudentResponse>.BadRequest("Add students to class failed"));
                }

                _logger.LogInformation("Successfully added students to class {ClassId}", classId);
                return Ok(ApiResponse<ClassStudentResponse>.Ok(result, "Add students to class successfully", "200"));
            }
            catch (CustomExceptions.NoDataFoundException ex)
            {
                _logger.LogWarning(ex, "Data not found for class {ClassId}: {Message}", classId, ex.Message);
                return NotFound(ApiResponse<ClassStudentResponse>.NotFound(ex.Message));
            }
            catch (CustomExceptions.ValidationException ex)
            {
                _logger.LogWarning(ex, "Validation error for class {ClassId}: {Message}", classId, ex.Message);
                return BadRequest(ApiResponse<ClassStudentResponse>.BadRequest(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error at the {Controller}: {Message}", nameof(ClassController), ex.Message);
                return StatusCode(500, ApiResponse<string>.InternalError($"Error at the {nameof(ClassController)}: {ex.Message}"));
            }
        }

        /// <summary>
        /// Remove students from a class
        /// </summary>
        /// <param name="classId"></param>
        /// <param name="userIds"></param>
        /// <returns></returns>
        [HttpDelete("{classId}/remove-students")]
        public async Task<IActionResult> RemoveStudentsFromClass([FromRoute] Guid classId, [FromForm] List<Guid> userIds)
        {
            try
            {
                var c = await _classService.RemoveStudentFromClassAsync(classId, userIds);
                if (c == null)
                {
                    return BadRequest(ApiResponse<ClassStudentResponse>.BadRequest("Remove students from class failed"));
                }
                return Ok(ApiResponse<ClassStudentResponse>.Ok(c, "Remove students from class successfully", "200"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error at the {Controller}: {Message}", nameof(ClassController), ex.Message);
                return StatusCode(500, ApiResponse<string>.InternalError($"Error at the {nameof(ClassController)}: {ex.Message}"));
            }
        }

        /// <summary>
        /// Get all students of a class
        /// </summary>
        /// <param name="classId"></param>
        /// <returns></returns>
        [HttpGet("{classId}/students")]
        public async Task<IActionResult> GetAllStudentsOfClass([FromRoute] Guid classId)
        {
            try
            {
                var c = await _classService.GetAllStudentsOfClassAsync(classId);
                if (c == null)
                {
                    return BadRequest(ApiResponse<List<ClassStudentResponse>>.BadRequest("Get all students of class failed"));
                }
                return Ok(ApiResponse<List<ClassStudentResponse>>.Ok(c, "Get all students of class successfully", "200"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error at the {Controller}: {Message}", nameof(ClassController), ex.Message);
                return StatusCode(500, ApiResponse<string>.InternalError($"Error at the {nameof(ClassController)}: {ex.Message}"));
            }
        }
    }
}