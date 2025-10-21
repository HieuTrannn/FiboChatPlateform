using Authentication.Application.Interfaces;
using Contracts.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using static Authentication.Application.DTOs.AuthenDTO;
using static Authentication.Application.DTOs.LecturerDTO;

namespace Authentication.API.Controllers
{
    [Route("api/lecturer")]
    [ApiController]
    public class LecturerController : ControllerBase
    {
        private readonly ILogger<LecturerController> _logger;
        private readonly IConfiguration _configuration;
        private readonly ILecturerService _lecturerService;
        public LecturerController(ILogger<LecturerController> logger, IConfiguration configuration, ILecturerService lecturerService)
        {
            _logger = logger;
            _configuration = configuration;
            _lecturerService = lecturerService;
        }

        /// <summary>
        /// Create a new lecturer
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateLecturer([FromForm] LecturerRequest request)
        {
            try
            {
                _logger.LogInformation("Creating lecturer with email: {Email}", request.Email);
                var response = await _lecturerService.CreateLecturer(request);

                return StatusCode(response.StatusCode, response);
            }
            catch (Exception ex)
            {
                _logger.LogError("{Classname} - Error at create lecturer async cause by {}", nameof(LecturerController), ex.Message);
                return BadRequest(ex);
            }
        }
        /// <summary>
        /// Get all lecturers
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAllLecturers()
        {
            try
            {
                _logger.LogInformation("Retrieving all lecturers");
                var response = await _lecturerService.GetAllLecturers();
                return Ok(ApiResponse<List<LecturerResponse>>.Ok(response, "Lecturers retrieved successfully", "200"));
            }
            catch (Exception ex)
            {
                _logger.LogError("{Classname} - Error at get all lecturers async cause by {}", nameof(LecturerController), ex.Message);
                return BadRequest(ex);
            }
        }

        /// <summary>
        /// Get a lecturer by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLecturerById(string id)
        {
            try
            {
                _logger.LogInformation("Retrieving lecturer with ID: {Id}", id);
                var response = await _lecturerService.GetLecturerById(id);
                return Ok(ApiResponse<RegisterResponse>.Ok(
                  null, // không có data
                 "Lecturer deleted successfully",
                nameof(HttpStatusCode.OK)
 ));

            }
            catch (Exception ex)
            {
                _logger.LogError("{Classname} - Error at get lecturer by id async cause by {}", nameof(LecturerController), ex.Message);
                return BadRequest(ex);
            }
        }

        /// <summary>
        /// Delete a lecturer by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLecturerById(Guid id)
        {
            try
            {
                _logger.LogInformation("Deleting lecturer with ID: {Id}", id);
                var response = await _lecturerService.DeleteLecturerById(id);
                return Ok(ApiResponse<RegisterResponse>.Ok(response,"Lecturer deleted successfully", "200"));
            }
            catch (Exception ex)
            {
                _logger.LogError("{Classname} - Error at delete lecturer by id async cause by {}", nameof(LecturerController), ex.Message);
                return BadRequest(ex);
            }
        }

        /// <summary>
        /// Update a lecturer by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLecturerById(Guid id, [FromForm] LecturerRequest request)
        {
            try
            {
                _logger.LogInformation("Updating lecturer with ID: {Id}", id);
                var response = await _lecturerService.UpdateLecturerById(id, request);
                return Ok(ApiResponse<RegisterResponse>.Ok(response,"Lecturer updated successfully", "200"));

            }
            catch (Exception ex)
            {
                _logger.LogError("{Classname} - Error at update lecturer by id async cause by {}", nameof(LecturerController), ex.Message);
                return BadRequest(ex);
            }
        }

    }
}

