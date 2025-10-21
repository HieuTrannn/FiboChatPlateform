using Course.Application.DTOs.MasterTopicDTOs;
using Contracts.Common;

namespace Course.Application.Interfaces
{
    public interface IMasterTopicService
    {
        public Task<MasterTopicResponse> GetByIdAsync(Guid id);
        public Task<BasePaginatedList<MasterTopicResponse>> GetAllAsync(int page, int pageSize);
        public Task<MasterTopicResponse> CreateAsync(MasterTopicCreateRequest request);
        public Task<MasterTopicResponse> UpdateAsync(Guid id, MasterTopicUpdateRequest request);
        public Task<MasterTopicResponse> DeleteAsync(Guid id);
    }
}