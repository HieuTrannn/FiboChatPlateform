using Course.Application.DTOs.DomainDTOs;
using Contracts.Common;

namespace Course.Application.Interfaces
{
    public interface IDomainService
    {
        public Task<DomainResponse> GetByIdAsync(Guid id);
        public Task<BasePaginatedList<DomainResponse>> GetAllAsync(int page, int pageSize);
        public Task<DomainResponse> CreateAsync(DomainCreateRequest request);
        public Task<DomainResponse> UpdateAsync(Guid id, DomainUpdateRequest request);
        public Task<DomainResponse> DeleteAsync(Guid id);
    }
}