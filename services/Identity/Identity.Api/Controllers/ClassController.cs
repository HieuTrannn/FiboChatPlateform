using Identity.Application.Interfaces;
using Identity.Contracts.DTOs;
using Identity.Contracts.DTOs.ClassDTOs;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers
{
    [ApiController]
    [Route("identity/[controller]")]
    public class ClassController : BaseController
    {
        private readonly IClassService _classService;
        private readonly ILogger<ClassController> _logger;

        public ClassController(IClassService classService, ILogger<ClassController> logger)
        {
            _classService = classService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllClasses()
        {
            try {
                var classes = await _classService.GetAllClassesAsync();
                return Ok(ApiResponse<List<ClassResponse>>.OkResponse(classes, "Get all classes successfully", "200"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error at the {Controller}: {Message}", nameof(ClassController), ex.Message);
                return HandleException(ex, nameof(ClassController));
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetClassById(Guid id)
        {
            try {
                var c = await _classService.GetClassByIdAsync(id);
                if (c == null)
                {
                    return NotFound(ApiResponse<ClassResponse>.NotFoundResponse("Class not found"));
                }
                return Ok(ApiResponse<ClassResponse>.OkResponse(c, "Get class by id successfully", "200"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error at the {Controller}: {Message}", nameof(ClassController), ex.Message);
                return HandleException(ex, nameof(ClassController));
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateClass(ClassCreateRequest request)
        {
            try {
                var c = await _classService.CreateClassAsync(request);
                if (c == null)
                {
                    return BadRequest(ApiResponse<ClassResponse>.BadRequestResponse("Create class failed"));
                }
                return Ok(ApiResponse<ClassResponse>.OkResponse(c, "Create class successfully", "200"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error at the {Controller}: {Message}", nameof(ClassController), ex.Message);
                return HandleException(ex, nameof(ClassController));
            }
        }
        
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClass(Guid id, ClassUpdateRequest request)
        {
            try {
                var c = await _classService.UpdateClassAsync(id, request);
                if (c == null)
                {
                    return BadRequest(ApiResponse<ClassResponse>.BadRequestResponse("Update class failed"));
                }
                return Ok(ApiResponse<ClassResponse>.OkResponse(c, "Update class successfully", "200"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error at the {Controller}: {Message}", nameof(ClassController), ex.Message);
                return HandleException(ex, nameof(ClassController));
            }
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClass(Guid id)
        {
            try {
                var c = await _classService.DeleteClassAsync(id);
                if (c == null)
                {
                    return BadRequest(ApiResponse<ClassResponse>.BadRequestResponse("Delete class failed"));
                }
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