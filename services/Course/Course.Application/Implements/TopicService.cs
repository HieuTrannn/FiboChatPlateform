using Course.Domain.Exceptions;
using Contracts.Common;
using Course.Application.DTOs.TopicDTOs;
using Course.Domain.Entities;
using Course.Domain.Abstraction;
using Course.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Course.Application.Implements
{
    public class TopicService : ITopicService
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
            // Validate input
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                _logger.LogError("Topic name is required");
                throw new CustomExceptions.ValidationException("Topic name is required");
            }

            if (request.MasterTopicId == Guid.Empty)
            {
                _logger.LogError("MasterTopicId is required");
                throw new CustomExceptions.ValidationException("MasterTopicId is required");
            }

            // Validate MasterTopic exists
            var masterTopic = await _unitOfWork.GetRepository<MasterTopic>().GetByIdAsync(request.MasterTopicId);
            if (masterTopic == null)
            {
                _logger.LogError("MasterTopic not found with id: {MasterTopicId}", request.MasterTopicId);
                throw new CustomExceptions.NoDataFoundException("MasterTopic not found");
            }

            // Check if topic with same name already exists in this MasterTopic
            var existingTopic = await _unitOfWork.GetRepository<Topic>()
                .FilterByAsync(t => t.Name == request.Name && t.MasterTopicId == request.MasterTopicId && t.Status == StaticEnum.StatusEnum.Active);

            if (existingTopic.Any())
            {
                _logger.LogError("Topic with name '{Name}' already exists in MasterTopic {MasterTopicId}", request.Name, request.MasterTopicId);
                throw new CustomExceptions.BusinessLogicException("Topic with this name already exists in the MasterTopic");
            }

            var topic = new Topic
            {
                Name = request.Name,
                Description = request.Description,
                MasterTopicId = request.MasterTopicId, // Đảm bảo MasterTopicId tồn tại
                Status = StaticEnum.StatusEnum.Active,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.GetRepository<Topic>().InsertAsync(topic);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Topic created successfully with id: {TopicId}", topic.Id);
            return await ToTopicResponse(topic);
        }

        public async Task<TopicResponse> UpdateAsync(Guid id, TopicUpdateRequest request)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                _logger.LogError("Topic name is required");
                throw new CustomExceptions.ValidationException("Topic name is required");
            }

            var topic = await _unitOfWork.GetRepository<Topic>().GetByIdAsync(id);
            if (topic == null)
            {
                _logger.LogError("Topic not found with id: {Id}", id);
                throw new CustomExceptions.NoDataFoundException("Topic not found");
            }

            // Check if topic is being updated to a name that already exists in the same MasterTopic
            if (request.Name != topic.Name)
            {
                var existingTopic = await _unitOfWork.GetRepository<Topic>()
                    .FilterByAsync(t => t.Name == request.Name && t.MasterTopicId == topic.MasterTopicId && t.Status == StaticEnum.StatusEnum.Active && t.Id != id);

                if (existingTopic.Any())
                {
                    _logger.LogError("Topic with name '{Name}' already exists in MasterTopic {MasterTopicId}", request.Name, topic.MasterTopicId);
                    throw new CustomExceptions.BusinessLogicException("Topic with this name already exists in the MasterTopic");
                }
            }

            // Update only if values are provided
            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                topic.Name = request.Name;
            }

            if (request.Description != null)
            {
                topic.Description = request.Description;
            }

            await _unitOfWork.GetRepository<Topic>().UpdateAsync(topic);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Topic updated successfully with id: {TopicId}", topic.Id);
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

        public async Task<BasePaginatedList<TopicMasterTopicResponse>> GetAllTopicsOfMasterTopicAsync(Guid masterTopicId, int page, int pageSize)
        {
            var masterTopic = await _unitOfWork.GetRepository<MasterTopic>().GetByIdAsync(masterTopicId);
            if (masterTopic == null)
            {
                _logger.LogError("Master topic not found with id: {Id}", masterTopicId);
                throw new CustomExceptions.NoDataFoundException("Master topic not found");
            }
            var topics = await _unitOfWork.GetRepository<Topic>().GetAllAsync(includeProperties: "MasterTopic");
            var items = new List<TopicMasterTopicResponse>(topics.Count);
            foreach (var topic in topics)
            {
                items.Add(await ToTopicMasterTopicResponse(masterTopic));
            }
            return new BasePaginatedList<TopicMasterTopicResponse>(items, topics.Count, page, pageSize);
        }

        private async Task<TopicMasterTopicResponse> ToTopicMasterTopicResponse(MasterTopic masterTopic)
        {
            var topics = await _unitOfWork.GetRepository<Topic>().GetAllAsync(includeProperties: "MasterTopic");
            var items = new List<TopicResponse>(topics.Count);
            foreach (var topic in topics)
            {
                items.Add(await ToTopicResponse(topic));
            }
            var response = new TopicMasterTopicResponse
            {
                Id = masterTopic.Id,
                Name = masterTopic.Name,
                Description = masterTopic.Description,
                Status = masterTopic.Status,
                CreatedAt = masterTopic.CreatedAt,
                Topics = items,
            };
            return await Task.FromResult(response);
        }
    }
}