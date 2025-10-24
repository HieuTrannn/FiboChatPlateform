using Course.Application.DTOs.DocumentDTOs;
using Contracts.Common;

namespace Course.Application.Interfaces
{
    public interface IDocumentService
    {
        Task<DocumentResponse> GetByIdAsync(Guid id);
        Task<BasePaginatedList<DocumentResponse>> GetAllAsync(int page, int pageSize);
        Task<BasePaginatedList<DocumentResponse>> GetAllByTopicIdAsync(Guid topicId, int page, int pageSize);
        Task<DocumentResponse> CreateAsync(DocumentCreateRequest request);
        Task<DocumentResponse> UploadAsync(DocumentUploadRequest request);
        Task<DocumentResponse> UpdateAsync(Guid id, DocumentUpdateRequest request);
        Task<DocumentResponse> DeleteAsync(Guid id);
        Task<DocumentResponse> PublishAsync(Guid id, Guid verifiedById);
        Task<DocumentResponse> UnpublishAsync(Guid id);
    }
}