using Course.Application.Interfaces;
using Course.Application.DTOs.DocumentDTOs;
using Course.Domain.Entities;
using Course.Domain.Abstraction;
using Course.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using Contracts.Common;
using static Course.Domain.Enums.StaticEnums;

namespace Course.Application.Implements
{
    public class DocumentService : IDocumentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFirebaseService _firebaseService;
        private readonly ILogger<DocumentService> _logger;
        private readonly IExternalApiService _externalApiService;

        public DocumentService(IUnitOfWork unitOfWork, IFirebaseService firebaseService, ILogger<DocumentService> logger, IExternalApiService externalApiService)
        {
            _unitOfWork = unitOfWork;
            _firebaseService = firebaseService;
            _logger = logger;
            _externalApiService = externalApiService;
        }

        public async Task<DocumentResponse> GetByIdAsync(Guid id)
        {
            var document = await _unitOfWork.GetRepository<Document>().GetByIdAsync(id);
            if (document == null)
            {
                _logger.LogError("Document not found with id: {Id}", id);
                throw new CustomExceptions.NoDataFoundException("Document not found");
            }
            return await ToDocumentResponse(document);
        }

        public async Task<BasePaginatedList<DocumentResponse>> GetAllAsync(int page, int pageSize)
        {
            var documents = await _unitOfWork.GetRepository<Document>().GetAllAsync();
            var response = await Task.WhenAll(documents.Select(ToDocumentResponse));
            return new BasePaginatedList<DocumentResponse>(response, documents.Count, page, pageSize);
        }

        public async Task<BasePaginatedList<DocumentResponse>> GetAllByTopicIdAsync(Guid topicId, int page, int pageSize)
        {
            var documents = await _unitOfWork.GetRepository<Document>()
                .FilterByAsync(d => d.TopicId == topicId);
            var response = await Task.WhenAll(documents.Select(ToDocumentResponse));
            return new BasePaginatedList<DocumentResponse>(response, documents.Count, page, pageSize);
        }

        public async Task<DocumentResponse> CreateAsync(DocumentCreateRequest request)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                _logger.LogError("Document title is required");
                throw new CustomExceptions.ValidationException("Document title is required");
            }

            if (request.TopicId == Guid.Empty)
            {
                _logger.LogError("TopicId is required");
                throw new CustomExceptions.ValidationException("TopicId is required");
            }

            if (request.DocumentTypeId == Guid.Empty)
            {
                _logger.LogError("DocumentTypeId is required");
                throw new CustomExceptions.ValidationException("DocumentTypeId is required");
            }

            // Validate Topic exists
            var topic = await _unitOfWork.GetRepository<Topic>().GetByIdAsync(request.TopicId);
            if (topic == null)
            {
                _logger.LogError("Topic not found with id: {TopicId}", request.TopicId);
                throw new CustomExceptions.NoDataFoundException("Topic not found");
            }

            // Validate DocumentType exists
            var documentType = await _unitOfWork.GetRepository<DocumentType>().GetByIdAsync(request.DocumentTypeId);
            if (documentType == null)
            {
                _logger.LogError("DocumentType not found with id: {DocumentTypeId}", request.DocumentTypeId);
                throw new CustomExceptions.NoDataFoundException("DocumentType not found");
            }

            var document = new Document
            {
                TopicId = request.TopicId,
                DocumentTypeId = request.DocumentTypeId,
                Title = request.Title,
                Version = request.Version,
                Status = DocumentStatus.Draft,
                VerifiedById = request.VerifiedById,
                UpdatedById = request.UpdatedById,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.GetRepository<Document>().InsertAsync(document);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Document created successfully with id: {DocumentId}", document.Id);
            return await ToDocumentResponse(document);
        }

        public async Task<DocumentResponse> UploadAsync(DocumentUploadRequest request)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                _logger.LogError("Document title is required");
                throw new CustomExceptions.ValidationException("Document title is required");
            }

            if (request.File == null || request.File.Length == 0)
            {
                _logger.LogError("File is required");
                throw new CustomExceptions.ValidationException("File is required");
            }

            if (request.TopicId == Guid.Empty)
            {
                _logger.LogError("TopicId is required");
                throw new CustomExceptions.ValidationException("TopicId is required");
            }

            if (request.DocumentTypeId == Guid.Empty)
            {
                _logger.LogError("DocumentTypeId is required");
                throw new CustomExceptions.ValidationException("DocumentTypeId is required");
            }

            if (request.UpdatedById == Guid.Empty)
            {
                _logger.LogError("UpdatedById (Lecturer ID) is required");
                throw new CustomExceptions.ValidationException("UpdatedById (Lecturer ID) is required");
            }

            if (request.VerifiedById == Guid.Empty)
            {
                _logger.LogError("VerifiedById (Lecturer ID) is required");
                throw new CustomExceptions.ValidationException("VerifiedById (Lecturer ID) is required");
            }

            // Validate Topic exists
            var topic = await _unitOfWork.GetRepository<Topic>().GetByIdAsync(request.TopicId);
            if (topic == null)
            {
                _logger.LogError("Topic not found with id: {TopicId}", request.TopicId);
                throw new CustomExceptions.NoDataFoundException("Topic not found");
            }

            // Validate DocumentType exists
            var documentType = await _unitOfWork.GetRepository<DocumentType>().GetByIdAsync(request.DocumentTypeId);
            if (documentType == null)
            {
                _logger.LogError("DocumentType not found with id: {DocumentTypeId}", request.DocumentTypeId);
                throw new CustomExceptions.NoDataFoundException("DocumentType not found");
            }

            // Validate Lecturer exists (UpdatedById)
            var lecturer = await _externalApiService.GetLecturerByIdAsync(request.UpdatedById.ToString());
            if (lecturer == null)
            {
                _logger.LogError("Lecturer not found with id: {LecturerId}", request.UpdatedById);
                throw new CustomExceptions.NoDataFoundException("Lecturer not found");
            }

            // Validate Verifier exists (VerifiedById)
            var verifier = await _externalApiService.GetLecturerByIdAsync(request.VerifiedById.ToString());
            if (verifier == null)
            {
                _logger.LogError("Verifier not found with id: {VerifierId}", request.VerifiedById);
                throw new CustomExceptions.NoDataFoundException("Verifier not found");
            }

            try
            {
                // Upload file to Firebase
                var fileUrl = await _firebaseService.UploadDocumentAsync(request.File.OpenReadStream(), request.File.FileName);

                // Create File entity - Lecturer is the owner
                var file = new Domain.Entities.File
                {
                    OwnerUserId = request.UpdatedById, // Lecturer ID - người upload
                    FileName = request.File.FileName,
                    FilePath = fileUrl,
                    FileContentType = request.File.ContentType,
                    FileSize = request.File.Length,
                    FileUrl = fileUrl,
                    FileKey = Guid.NewGuid().ToString(),
                    FileBucket = "fibochatplatform.firebasestorage.app",
                    FileRegion = "us-central1",
                    FileAcl = "public-read",
                    Status = "active",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _unitOfWork.GetRepository<Domain.Entities.File>().InsertAsync(file);
                await _unitOfWork.SaveChangesAsync();

                // Create Document entity
                var document = new Document
                {
                    TopicId = request.TopicId,
                    DocumentTypeId = request.DocumentTypeId,
                    FileId = file.Id,
                    Title = request.Title,
                    Version = 1,
                    Status = DocumentStatus.Draft,
                    VerifiedById = request.VerifiedById, // Lecturer ID - người sẽ verify
                    UpdatedById = request.UpdatedById, // Lecturer ID - người upload
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _unitOfWork.GetRepository<Document>().InsertAsync(document);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Document uploaded by Lecturer {LecturerId} successfully with id: {DocumentId}", request.UpdatedById, document.Id);
                return await ToDocumentResponse(document);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading document: {Message}", ex.Message);
                throw new CustomExceptions.BusinessLogicException("Error uploading document", ex);
            }
        }

        public async Task<DocumentResponse> UpdateAsync(Guid id, DocumentUpdateRequest request)
        {
            var document = await _unitOfWork.GetRepository<Document>().GetByIdAsync(id);
            if (document == null)
            {
                _logger.LogError("Document not found with id: {Id}", id);
                throw new CustomExceptions.NoDataFoundException("Document not found");
            }

            // Update only if values are provided
            if (!string.IsNullOrWhiteSpace(request.Title))
            {
                document.Title = request.Title;
            }

            if (request.Version.HasValue)
            {
                document.Version = request.Version.Value;
            }

            if (request.Status.HasValue)
            {
                document.Status = request.Status.Value;
            }

            if (request.VerifiedById.HasValue)
            {
                document.VerifiedById = request.VerifiedById.Value;
            }

            if (request.UpdatedById.HasValue)
            {
                document.UpdatedById = request.UpdatedById.Value;
            }

            document.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.GetRepository<Document>().UpdateAsync(document);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Document updated successfully with id: {DocumentId}", document.Id);
            return await ToDocumentResponse(document);
        }

        public async Task<DocumentResponse> DeleteAsync(Guid id)
        {
            var document = await _unitOfWork.GetRepository<Document>().GetByIdAsync(id);
            if (document == null)
            {
                _logger.LogError("Document not found with id: {Id}", id);
                throw new CustomExceptions.NoDataFoundException("Document not found");
            }

            await _unitOfWork.GetRepository<Document>().SoftDeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Document deleted successfully with id: {DocumentId}", document.Id);
            return await ToDocumentResponse(document);
        }

        public async Task<DocumentResponse> PublishAsync(Guid id, Guid verifiedById)
        {
            var document = await _unitOfWork.GetRepository<Document>().GetByIdAsync(id);
            if (document == null)
            {
                _logger.LogError("Document not found with id: {Id}", id);
                throw new CustomExceptions.NoDataFoundException("Document not found");
            }

            document.Status = DocumentStatus.Published;
            document.VerifiedById = verifiedById;
            document.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.GetRepository<Document>().UpdateAsync(document);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Document published successfully with id: {DocumentId}", document.Id);
            return await ToDocumentResponse(document);
        }

        public async Task<DocumentResponse> UnpublishAsync(Guid id)
        {
            var document = await _unitOfWork.GetRepository<Document>().GetByIdAsync(id);
            if (document == null)
            {
                _logger.LogError("Document not found with id: {Id}", id);
                throw new CustomExceptions.NoDataFoundException("Document not found");
            }

            document.Status = DocumentStatus.Draft;
            document.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.GetRepository<Document>().UpdateAsync(document);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Document unpublished successfully with id: {DocumentId}", document.Id);
            return await ToDocumentResponse(document);
        }

        private async Task<DocumentResponse> ToDocumentResponse(Document document)
        {
            var topic = await _unitOfWork.GetRepository<Topic>().GetByIdAsync(document.TopicId);
            var documentType = await _unitOfWork.GetRepository<DocumentType>().GetByIdAsync(document.DocumentTypeId);
            var file = await _unitOfWork.GetRepository<Domain.Entities.File>().GetByIdAsync(document.FileId);

            return new DocumentResponse
            {
                Id = document.Id,
                TopicId = document.TopicId,
                DocumentTypeId = document.DocumentTypeId,
                FileId = document.FileId,
                Title = document.Title,
                Version = document.Version,
                Status = document.Status,
                VerifiedById = document.VerifiedById,
                UpdatedById = document.UpdatedById,
                CreatedAt = document.CreatedAt,
                UpdatedAt = document.UpdatedAt,
                Topic = topic != null ? new TopicResponse
                {
                    Id = topic.Id,
                    Name = topic.Name,
                    Description = topic.Description,
                    MasterTopicId = topic.MasterTopicId,
                    Status = topic.Status,
                    CreatedAt = topic.CreatedAt
                } : null,
                DocumentType = documentType != null ? new DocumentTypeResponse
                {
                    Id = documentType.Id,
                    Name = documentType.Name,
                    Status = documentType.Status,
                    CreatedAt = documentType.CreatedAt
                } : null,
                File = file != null ? new FileResponse
                {
                    Id = file.Id,
                    OwnerUserId = file.OwnerUserId,
                    FileName = file.FileName,
                    FilePath = file.FilePath,
                    FileContentType = file.FileContentType,
                    FileSize = file.FileSize,
                    FileUrl = file.FileUrl,
                    FileKey = file.FileKey,
                    FileBucket = file.FileBucket,
                    FileRegion = file.FileRegion,
                    FileAcl = file.FileAcl,
                    CreatedAt = file.CreatedAt,
                    UpdatedAt = file.UpdatedAt,
                    Status = file.Status
                } : null,
            };
        }
    }
}