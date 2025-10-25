using Contracts.Common;
using Course.Application.DTOs.DomainDTOs;
using Course.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Course.Api.Controllers
{
    [ApiController]
    [Route("api/domains")]
    public class DomainController : ControllerBase
    {
        private readonly IDomainService _domainService;
        private readonly ILogger<DomainController> _logger;

        public DomainController(IDomainService domainService, ILogger<DomainController> logger)
        {
            _domainService = domainService;
            _logger = logger;
        }

        /// <summary>
        /// Get all domains (active only)
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAllDomains([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var domains = await _domainService.GetAllAsync(page, pageSize);
                return Ok(ApiResponse<BasePaginatedList<DomainResponse>>.Ok(domains, "Get all domains successfully", "200"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error at the {Controller}: {Message}", nameof(DomainController), ex.Message);
                return StatusCode(500, ApiResponse<string>.InternalError($"Error at the {nameof(DomainController)}: {ex.Message}"));
            }
        }

        /// <summary>
        /// Get a domain by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDomainById([FromRoute] Guid id)
        {
            try
            {
                var domain = await _domainService.GetByIdAsync(id);
                return Ok(ApiResponse<DomainResponse>.Ok(domain, "Get domain by id successfully", "200"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error at the {Controller}: {Message}", nameof(DomainController), ex.Message);
                return StatusCode(500, ApiResponse<string>.InternalError($"Error at the {nameof(DomainController)}: {ex.Message}"));
            }
        }

        /// <summary>
        /// Create a new domain
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateDomain([FromForm] DomainCreateRequest request)
        {
            try
            {
                var domain = await _domainService.CreateAsync(request);
                return Ok(ApiResponse<DomainResponse>.Ok(domain, "Create domain successfully", "200"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error at the {Controller}: {Message}", nameof(DomainController), ex.Message);
                return StatusCode(500, ApiResponse<string>.InternalError($"Error at the {nameof(DomainController)}: {ex.Message}"));
            }
        }

        /// <summary>
        /// Update a domain
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDomain(Guid id, [FromForm] DomainUpdateRequest request)
        {
            try
            {
                var domain = await _domainService.UpdateAsync(id, request);
                return Ok(ApiResponse<DomainResponse>.Ok(domain, "Update domain successfully", "200"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error at the {Controller}: {Message}", nameof(DomainController), ex.Message);
                return StatusCode(500, ApiResponse<string>.InternalError($"Error at the {nameof(DomainController)}: {ex.Message}"));
            }
        }

        /// <summary>
        /// Delete a domain
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDomain([FromRoute] Guid id)
        {
            try
            {
                var domain = await _domainService.DeleteAsync(id);
                return Ok(ApiResponse<DomainResponse>.Ok(domain, "Delete domain successfully", "200"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error at the {Controller}: {Message}", nameof(DomainController), ex.Message);
                return StatusCode(500, ApiResponse<string>.InternalError($"Error at the {nameof(DomainController)}: {ex.Message}"));
            }
        }
    }
}