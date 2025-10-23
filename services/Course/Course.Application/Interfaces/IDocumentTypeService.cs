using Course.Application.DTOs.DocumentDTOs;
using Contracts.Common;

namespace Course.Application.Interfaces
{
    public interface IDocumentTypeService
    {
        public Task<DocumentTypeResponse> GetByIdAsync(Guid id);
        public Task<BasePaginatedList<DocumentTypeResponse>> GetAllAsync(int page, int pageSize);
        public Task<DocumentTypeResponse> CreateAsync(DocumentTypeCreateRequest request);
        public Task<DocumentTypeResponse> UpdateAsync(Guid id, DocumentTypeUpdateRequest request);
        public Task DeleteAsync(Guid id);
    }
}