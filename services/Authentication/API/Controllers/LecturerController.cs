using Authentication.Application.Interfaces;
using Contracts.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Authentication.Application.DTOs.AuthenDTO;
using static Authentication.Application.DTOs.LectureDTO;

namespace Authentication.API.Controllers
{
    [Route("api/lecturer")]
    [ApiController]
    public class LecturerController : ControllerBase
    {
        private readonly ILogger<LecturerController> _logger;
        private readonly IConfiguration _configuration;
        private readonly ILectureService _lectureService;
        public LecturerController(ILogger<LecturerController> logger, IConfiguration configuration, ILectureService lectureService)
        {
            _logger = logger;
            _configuration = configuration;
            _lectureService = lectureService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateLecturer([FromBody] LectureRequest request)
        {
            try
            {
                _logger.LogInformation("Creating lecturer with email: {Email}", request.Email);
                var response = await _lectureService.CreateLecturer(request);

                return StatusCode(response.StatusCode, response);
            }
            catch (Exception ex)
            {
                _logger.LogError("{Classname} - Error at create lecturer async cause by {}", nameof(LecturerController), ex.Message);
                return BadRequest(ex);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllLecturers()
        {
            try
            {
                _logger.LogInformation("Retrieving all lecturers");
                var response = await _lectureService.GetAllLecturers();
                return Ok(ApiResponse<List<LectureResponse>>.Ok("Lecturers retrieved successfully", response));
            }
            catch (Exception ex)
            {
                _logger.LogError("{Classname} - Error at get all lecturers async cause by {}", nameof(LecturerController), ex.Message);
                return BadRequest(ex);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetLecturerById([FromBody] string id)
        {
            try
            {
                _logger.LogInformation("Retrieving lecturer with ID: {Id}", id);
                var response = await _lectureService.GetLecturerById(id);
                return Ok(ApiResponse<LectureResponse>.Ok("Lecturer retrieved successfully", response));
            }
            catch (Exception ex)
            {
                _logger.LogError("{Classname} - Error at get lecturer by id async cause by {}", nameof(LecturerController), ex.Message);
                return BadRequest(ex);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLecturerById([FromBody] Guid id)
        {
            try
            {
                _logger.LogInformation("Deleting lecturer with ID: {Id}", id);
                var response = await _lectureService.DeleteLecturerById(id);
                return Ok(ApiResponse<RegisterResponse>.Ok("Lecturer deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError("{Classname} - Error at delete lecturer by id async cause by {}", nameof(LecturerController), ex.Message);
                return BadRequest(ex);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLecturerById([FromForm] Guid id, [FromBody] LectureRequest request)
        {
            try
            {
                _logger.LogInformation("Updating lecturer with ID: {Id}", id);
                var response = await _lectureService.UpdateLecturerById(id, request);
                return Ok(ApiResponse<RegisterResponse>.Ok("Lecturer updated successfully"));

            }
            catch (Exception ex)
            {
                _logger.LogError("{Classname} - Error at update lecturer by id async cause by {}", nameof(LecturerController), ex.Message);
                return BadRequest(ex);
            }
        }

        }
    }

