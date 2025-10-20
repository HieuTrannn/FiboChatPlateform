using Course.Application.DTOs.KeywordDTOs;
using Contracts.Common;

namespace Course.Application.Interfaces
{
    public interface IKeywordService
    {
        public Task<KeywordResponse> GetByIdAsync(Guid id);
        public Task<BasePaginatedList<KeywordResponse>> GetAllAsync(int page, int pageSize);
        public Task<KeywordResponse> CreateAsync(KeywordCreateRequest request);
        public Task<KeywordResponse> UpdateAsync(Guid id, KeywordUpdateRequest request);
        public Task<KeywordResponse> DeleteAsync(Guid id);
    }
}