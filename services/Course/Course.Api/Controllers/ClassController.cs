using Microsoft.AspNetCore.Mvc;
using Course.Application.Interfaces;
using Course.Domain.DTOs.ClassDTOs;
using Course.Contracts.DTOs;

namespace Course.Api.Controllers
{
    [ApiController]
    [Route("api/classes")]
    public class ClassController : BaseController
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
                return Ok(ApiResponse<List<ClassResponse>>.OkResponse(classes, "Get all classes successfully", "200"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error at the {Controller}: {Message}", nameof(ClassController), ex.Message);
                return HandleException(ex, nameof(ClassController));
            }
        }

        /// <summary>
        /// Get a class by id (active and pending only)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetClassById(Guid id)
        {
            try
            {
                var c = await _classService.GetByIdAsync(id);
                return Ok(ApiResponse<ClassResponse>.OkResponse(c, "Get class by id successfully", "200"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error at the {Controller}: {Message}", nameof(ClassController), ex.Message);
                return HandleException(ex, nameof(ClassController));
            }
        }

        /// <summary>
        /// Create a new class 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateClass(ClassCreateRequest request)
        {
            try
            {
                var c = await _classService.CreateAsync(request);
                return Ok(ApiResponse<ClassResponse>.OkResponse(c, "Create class successfully", "200"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error at the {Controller}: {Message}", nameof(ClassController), ex.Message);
                return HandleException(ex, nameof(ClassController));
            }
        }

        /// <summary>
        /// Update a class
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClass(Guid id, ClassUpdateRequest request)
        {
            try
            {
                var c = await _classService.UpdateAsync(id, request);
                return Ok(ApiResponse<ClassResponse>.OkResponse(c, "Update class successfully", "200"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error at the {Controller}: {Message}", nameof(ClassController), ex.Message);
                return HandleException(ex, nameof(ClassController));
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
                return Ok(ApiResponse<ClassResponse>.OkResponse(c, "Delete class successfully", "200"));
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error at the {Controller}: {Message}", nameof(ClassController), ex.Message);
                return HandleException(ex, nameof(ClassController));
            }
        }
    }
}