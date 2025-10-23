using Course.Domain.Abstraction;
using Microsoft.Extensions.Logging;
using Course.Application.Interfaces;
using Course.Domain.Exceptions;
using Course.Application.DTOs.DocumentDTOs;
using Course.Domain.Entities;
using Contracts.Common;

namespace Course.Application.Implements
{
    public class DocumentTypeService : IDocumentTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DocumentTypeService> _logger;

        public DocumentTypeService(IUnitOfWork unitOfWork, ILogger<DocumentTypeService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<DocumentTypeResponse> GetByIdAsync(Guid id)
        {
            var documentType = await _unitOfWork.GetRepository<DocumentType>().GetByIdAsync(id);
            if (documentType == null)
            {
                _logger.LogError("Document type not found with id: {Id}", id);
                throw new CustomExceptions.NoDataFoundException("Document type not found");
            }
            
            return await ToDocumentTypeResponse(documentType);
        }

        public async Task<BasePaginatedList<DocumentTypeResponse>> GetAllAsync(int page, int pageSize)
        {
            var documentTypes = await _unitOfWork.GetRepository<DocumentType>().GetAllAsync();
            var items = new List<DocumentTypeResponse>(documentTypes.Count);
            foreach (var documentType in documentTypes)
            {
                items.Add(await ToDocumentTypeResponse(documentType));
            }
            return new BasePaginatedList<DocumentTypeResponse>(items, documentTypes.Count, page, pageSize);
        }

        public async Task<DocumentTypeResponse> CreateAsync(DocumentTypeCreateRequest request)
        {
            var documentType = new DocumentType
            {
                Name = request.Name,
            };
            await _unitOfWork.GetRepository<DocumentType>().InsertAsync(documentType);
            await _unitOfWork.SaveChangesAsync();
            return await ToDocumentTypeResponse(documentType);
        }

        public async Task<DocumentTypeResponse> UpdateAsync(Guid id, DocumentTypeUpdateRequest request)
        {
            var documentType = await _unitOfWork.GetRepository<DocumentType>().GetByIdAsync(id);
            if (documentType == null)
            {
                _logger.LogError("Document type not found with id: {Id}", id);
                throw new CustomExceptions.NoDataFoundException("Document type not found");
            }
            documentType.Name = request.Name ?? documentType.Name;
            await _unitOfWork.GetRepository<DocumentType>().UpdateAsync(documentType);
            await _unitOfWork.SaveChangesAsync();
            return await ToDocumentTypeResponse(documentType);
        }

        public async Task DeleteAsync(Guid id)
        {
            var documentType = await _unitOfWork.GetRepository<DocumentType>().GetByIdAsync(id);
            if (documentType == null)
            {
                _logger.LogError("Document type not found with id: {Id}", id);
                throw new CustomExceptions.NoDataFoundException("Document type not found");
            }
            await _unitOfWork.GetRepository<DocumentType>().SoftDeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }

        private async Task<DocumentTypeResponse> ToDocumentTypeResponse(DocumentType documentType)
        {
            var response = new DocumentTypeResponse
            {
                Id = documentType.Id,
                Name = documentType.Name,
                Status = documentType.Status,
                CreatedAt = documentType.CreatedAt,
            };
            return await Task.FromResult(response);
        }
    }
}