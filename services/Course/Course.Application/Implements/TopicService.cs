using Course.Domain.Exceptions;
using Contracts.Common;
using Course.Application.DTOs.TopicDTOs;
using Course.Domain.Entities;
using Course.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;

namespace Course.Application.Implements
{
    public class TopicService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<TopicService> _logger;

        public TopicService(IUnitOfWork unitOfWork, ILogger<TopicService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<TopicResponse> GetByIdAsync(Guid id)
        {
            var topic = await _unitOfWork.GetRepository<Topic>().GetByIdAsync(id);
            if (topic == null)
            {
                _logger.LogError("Topic not found with id: {Id}", id);
                throw new CustomExceptions.NoDataFoundException("Topic not found");
            }
            return await ToTopicResponse(topic);
        }
        public async Task<BasePaginatedList<TopicResponse>> GetAllAsync(int page, int pageSize)
        {
            var topics = await _unitOfWork.GetRepository<Topic>().GetAllAsync();
            var response = await Task.WhenAll(topics.Select(ToTopicResponse));
            return new BasePaginatedList<TopicResponse>(response, topics.Count, page, pageSize);
        }

        public async Task<TopicResponse> CreateAsync(TopicCreateRequest request)
        {
            var topic = new Topic
            {
                Name = request.Name,
                Description = request.Description,
            };
            await _unitOfWork.GetRepository<Topic>().InsertAsync(topic);
            await _unitOfWork.SaveChangesAsync();
            return await ToTopicResponse(topic);
        }

        public async Task<TopicResponse> UpdateAsync(Guid id, TopicUpdateRequest request)
        {
            var topic = await _unitOfWork.GetRepository<Topic>().GetByIdAsync(id);
            if (topic == null)
            {
                _logger.LogError("Topic not found with id: {Id}", id);
                throw new CustomExceptions.NoDataFoundException("Topic not found");
            }
            topic.Name = request.Name ?? topic.Name;
            topic.Description = request.Description ?? topic.Description;
            topic.Status = StaticEnum.StatusEnum.Active;
            await _unitOfWork.GetRepository<Topic>().UpdateAsync(topic);
            await _unitOfWork.SaveChangesAsync();
            return await ToTopicResponse(topic);
        }
        
        public async Task<TopicResponse> DeleteAsync(Guid id)
        {
            var topic = await _unitOfWork.GetRepository<Topic>().GetByIdAsync(id);
            if (topic == null)
            {
                _logger.LogError("Topic not found with id: {Id}", id);
                throw new CustomExceptions.NoDataFoundException("Topic not found");
            }
            await _unitOfWork.GetRepository<Topic>().SoftDeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
            return await ToTopicResponse(topic);
        }

        private async Task<TopicResponse> ToTopicResponse(Topic topic)
        {
            var response = new TopicResponse
            {
                Id = topic.Id,
                Name = topic.Name,
                Description = topic.Description,
                Status = topic.Status,
                CreatedAt = topic.CreatedAt,
            };
            return await Task.FromResult(response);
        }
    }
}