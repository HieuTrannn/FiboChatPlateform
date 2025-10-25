using Course.Application.Interfaces;
using Course.Application.DTOs.DocumentDTOs;
using Microsoft.AspNetCore.Mvc;
using Contracts.Common;

namespace Course.Api.Controllers
{
    [ApiController]
    [Route("api/documents")]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentService _documentService;
        private readonly ILogger<DocumentController> _logger;

        public DocumentController(IDocumentService documentService, ILogger<DocumentController> logger)
        {
            _documentService = documentService;
            _logger = logger;
        }

        /// <summary>
        /// Get document by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            try
            {
                var document = await _documentService.GetByIdAsync(id);
                return Ok(ApiResponse<DocumentResponse>.Ok(document, "Get document by id successfully", "200"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting document by id: {Id}", id);
                return StatusCode(500, ApiResponse<string>.InternalError($"Error at the {nameof(DocumentController)}: {ex.Message}"));
            }
        }

        /// <summary>
        /// Get all documents (active only)
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var documents = await _documentService.GetAllAsync(page, pageSize);
                return Ok(ApiResponse<BasePaginatedList<DocumentResponse>>.Ok(documents, "Get all documents successfully", "200"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all documents");
                return StatusCode(500, ApiResponse<string>.InternalError($"Error at the {nameof(DocumentController)}: {ex.Message}"));
            }
        }

        /// <summary>
        /// Get all documents by topic id (active only)
        /// </summary>
        /// <param name="topicId"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("topic/{topicId}")]
        public async Task<IActionResult> GetAllByTopicId([FromRoute] Guid topicId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var documents = await _documentService.GetAllByTopicIdAsync(topicId, page, pageSize);
                return Ok(ApiResponse<BasePaginatedList<DocumentResponse>>.Ok(documents, "Get all documents by topic id successfully", "200"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting documents by topic id: {TopicId}", topicId);
                return StatusCode(500, ApiResponse<string>.InternalError($"Error at the {nameof(DocumentController)}: {ex.Message}"));
            }
        }

        /// <summary>
        /// Create a new document
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] DocumentCreateRequest request)
        {
            try
            {
                var document = await _documentService.CreateAsync(request);
                return Ok(ApiResponse<DocumentResponse>.Ok(document, "Create document successfully", "200"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating document");
                return StatusCode(500, ApiResponse<string>.InternalError($"Error at the {nameof(DocumentController)}: {ex.Message}"));
            }
        }

        /// <summary>
        /// Upload a new document
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] DocumentUploadRequest request)
        {
            try
            {
                var document = await _documentService.UploadAsync(request);
                return Ok(ApiResponse<DocumentResponse>.Ok(document, "Upload document successfully", "200"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading document");
                return StatusCode(500, ApiResponse<string>.InternalError($"Error at the {nameof(DocumentController)}: {ex.Message}"));
            }
        }

        /// <summary>
        /// Update a document
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromForm] DocumentUpdateRequest request)
        {
            try
            {
                var document = await _documentService.UpdateAsync(id, request);
                return Ok(ApiResponse<DocumentResponse>.Ok(document, "Update document successfully", "200"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating document: {Id}", id);
                return StatusCode(500, ApiResponse<string>.InternalError($"Error at the {nameof(DocumentController)}: {ex.Message}"));
            }
        }

        /// <summary>
        /// Delete a document
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            try
            {
                var document = await _documentService.DeleteAsync(id);
                return Ok(ApiResponse<DocumentResponse>.Ok(document, "Delete document successfully", "200"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting document: {Id}", id);
                return StatusCode(500, ApiResponse<string>.InternalError($"Error at the {nameof(DocumentController)}: {ex.Message}"));
            }
        }

        /// <summary>
        /// Publish a document
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("{id}/publish")]
        public async Task<IActionResult> Publish([FromRoute] Guid id, [FromForm] PublishRequest request)
        {
            try
            {
                var document = await _documentService.PublishAsync(id, request.VerifiedById);
                return Ok(ApiResponse<DocumentResponse>.Ok(document, "Publish document successfully", "200"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing document: {Id}", id);
                return StatusCode(500, ApiResponse<string>.InternalError($"Error at the {nameof(DocumentController)}: {ex.Message}"));
            }
        }

        /// <summary>
        /// Unpublish a document
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("{id}/unpublish")]
        public async Task<IActionResult> Unpublish([FromRoute] Guid id)
        {
            try
            {
                var document = await _documentService.UnpublishAsync(id);
                return Ok(ApiResponse<DocumentResponse>.Ok(document, "Unpublish document successfully", "200"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unpublishing document: {Id}", id);
                return StatusCode(500, ApiResponse<string>.InternalError($"Error at the {nameof(DocumentController)}: {ex.Message}"));
            }
        }

        /// <summary>
        /// Get all documents by lecturer id (active only)
        /// </summary>
        /// <param name="lecturerId"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("lecturer/{lecturerId}")]
        public async Task<IActionResult> GetAllDocumentsByLecturerId([FromRoute] Guid lecturerId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var documents = await _documentService.GetAllDocumentsByLecturerIdAsync(lecturerId, page, pageSize);
                return Ok(ApiResponse<BasePaginatedList<DocumentResponse>>.Ok(documents, "Get all documents by lecturer id successfully", "200"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting documents by lecturer id: {LecturerId}", lecturerId);
                return StatusCode(500, ApiResponse<string>.InternalError($"Error at the {nameof(DocumentController)}: {ex.Message}"));
            }
        }
    }

    public class PublishRequest
    {
        public Guid VerifiedById { get; set; }
    }
}