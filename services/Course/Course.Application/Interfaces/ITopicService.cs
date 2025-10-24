using Course.Application.DTOs.TopicDTOs;
using Contracts.Common;

namespace Course.Application.Interfaces
{
    public interface ITopicService
    {
        public Task<TopicResponse> GetByIdAsync(Guid id);
        public Task<BasePaginatedList<TopicResponse>> GetAllAsync(int page, int pageSize);
        public Task<TopicResponse> CreateAsync(TopicCreateRequest request);
        public Task<TopicResponse> UpdateAsync(Guid id, TopicUpdateRequest request);
        public Task<TopicResponse> DeleteAsync(Guid id);

        public Task<BasePaginatedList<TopicMasterTopicResponse>> GetAllTopicsOfMasterTopicAsync(Guid masterTopicId, int page, int pageSize);
    }
}