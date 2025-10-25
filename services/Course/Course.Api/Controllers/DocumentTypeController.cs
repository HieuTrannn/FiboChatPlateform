
using Contracts.Common;
using Course.Application.DTOs.DocumentDTOs;
using Course.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Course.Api.Controllers
{
    [ApiController]
    [Route("api/document-types")]
    public class DocumentTypeController : ControllerBase
    {
        private readonly IDocumentTypeService _documentTypeService;
        private readonly ILogger<DocumentTypeController> _logger;

        public DocumentTypeController(IDocumentTypeService documentTypeService, ILogger<DocumentTypeController> logger)
        {
            _documentTypeService = documentTypeService;
            _logger = logger;
        }

        /// <summary>
        /// Get all document types (active only)
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAllDocumentTypes([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var documentTypes = await _documentTypeService.GetAllAsync(page, pageSize);
                return Ok(ApiResponse<BasePaginatedList<DocumentTypeResponse>>.Ok(documentTypes, "Get all document types successfully", "200"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error at the {Controller}: {Message}", nameof(DocumentTypeController), ex.Message);
                return StatusCode(500, ApiResponse<string>.InternalError($"Error at the {nameof(DocumentTypeController)}: {ex.Message}"));
            }
        }

        /// <summary>
        /// Get document type by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDocumentTypeById([FromRoute] Guid id)
        {
            try
            {
                var documentType = await _documentTypeService.GetByIdAsync(id);
                return Ok(ApiResponse<DocumentTypeResponse>.Ok(documentType, "Get document type by id successfully", "200"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error at the {Controller}: {Message}", nameof(DocumentTypeController), ex.Message);
                return StatusCode(500, ApiResponse<string>.InternalError($"Error at the {nameof(DocumentTypeController)}: {ex.Message}"));
            }
        }

        /// <summary>
        /// Create a new document type
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateDocumentType([FromForm] DocumentTypeCreateRequest request)
        {
            try
            {
                var documentType = await _documentTypeService.CreateAsync(request);
                return Ok(ApiResponse<DocumentTypeResponse>.Ok(documentType, "Create document type successfully", "200"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error at the {Controller}: {Message}", nameof(DocumentTypeController), ex.Message);
                return StatusCode(500, ApiResponse<string>.InternalError($"Error at the {nameof(DocumentTypeController)}: {ex.Message}"));
            }
        }

        /// <summary>
        /// Update a document type
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDocumentType([FromRoute] Guid id, [FromForm] DocumentTypeUpdateRequest request)
        {
            try
            {
                var documentType = await _documentTypeService.UpdateAsync(id, request);
                return Ok(ApiResponse<DocumentTypeResponse>.Ok(documentType, "Update document type successfully", "200"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error at the {Controller}: {Message}", nameof(DocumentTypeController), ex.Message);
                return StatusCode(500, ApiResponse<string>.InternalError($"Error at the {nameof(DocumentTypeController)}: {ex.Message}"));
            }
        }

        /// <summary>
        /// Delete a document type
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDocumentType([FromRoute] Guid id)
        {
            try
            {
                await _documentTypeService.DeleteAsync(id);
                return Ok(ApiResponse<string>.Ok(null, "Delete document type successfully", "200"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error at the {Controller}: {Message}", nameof(DocumentTypeController), ex.Message);
                return StatusCode(500, ApiResponse<string>.InternalError($"Error at the {nameof(DocumentTypeController)}: {ex.Message}"));
            }
        }
    }
}