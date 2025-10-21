using Microsoft.Extensions.Logging;
using Course.Domain.Abstraction;
using Course.Application.DTOs.MasterTopicDTOs;
using Course.Domain.Exceptions;
using Course.Domain.Entities;
using Contracts.Common;
using Course.Application.Interfaces;

namespace Course.Application.Implements
{
    public class MasterTopicService : IMasterTopicService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<MasterTopicService> _logger;

        public MasterTopicService(IUnitOfWork unitOfWork, ILogger<MasterTopicService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<MasterTopicResponse> GetByIdAsync(Guid id)
        {
            var masterTopic = await _unitOfWork.GetRepository<MasterTopic>().GetByIdAsync(id);
            if (masterTopic == null)
            {
                _logger.LogError("Master topic not found with id: {Id}", id);
                throw new CustomExceptions.NoDataFoundException("Master topic not found");
            }
            return await ToMasterTopicResponse(masterTopic);
        }

        public async Task<BasePaginatedList<MasterTopicResponse>> GetAllAsync(int page, int pageSize)
        {
            var masterTopics = await _unitOfWork.GetRepository<MasterTopic>().GetAllAsync();
            var response = await Task.WhenAll(masterTopics.Select(ToMasterTopicResponse));
            return new BasePaginatedList<MasterTopicResponse>(response, masterTopics.Count, page, pageSize);
        }

        public async Task<MasterTopicResponse> CreateAsync(MasterTopicCreateRequest request)
        {
            var masterTopic = new MasterTopic
            {
                DomainId = request.DomainId,
                SemesterId = request.SemesterId,
                LecturerId = request.LecturerId,
                Name = request.Name,
                Description = request.Description,
                Status = StaticEnum.StatusEnum.Active,
            };
            await _unitOfWork.GetRepository<MasterTopic>().InsertAsync(masterTopic);
            await _unitOfWork.SaveChangesAsync();
            return await ToMasterTopicResponse(masterTopic);
        }

        public async Task<MasterTopicResponse> UpdateAsync(Guid id, MasterTopicUpdateRequest request)
        {
            var masterTopic = await _unitOfWork.GetRepository<MasterTopic>().GetByIdAsync(id);
            if (masterTopic == null)
            {
                _logger.LogError("Master topic not found with id: {Id}", id);
                throw new CustomExceptions.NoDataFoundException("Master topic not found");
            }
            masterTopic.Name = request.Name ?? masterTopic.Name;
            masterTopic.Description = request.Description ?? masterTopic.Description;
            masterTopic.DomainId = request.DomainId;
            masterTopic.SemesterId = request.SemesterId;
            masterTopic.LecturerId = request.LecturerId;
            masterTopic.Status = StaticEnum.StatusEnum.Active;
            await _unitOfWork.GetRepository<MasterTopic>().UpdateAsync(masterTopic);
            await _unitOfWork.SaveChangesAsync();
            return await ToMasterTopicResponse(masterTopic);
        }
        
        public async Task<MasterTopicResponse> DeleteAsync(Guid id)
        {
            var masterTopic = await _unitOfWork.GetRepository<MasterTopic>().GetByIdAsync(id);
            if (masterTopic == null)
            {
                _logger.LogError("Master topic not found with id: {Id}", id);
                throw new CustomExceptions.NoDataFoundException("Master topic not found");
            }
            await _unitOfWork.GetRepository<MasterTopic>().SoftDeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
            return await ToMasterTopicResponse(masterTopic);
        }

        private async Task<MasterTopicResponse> ToMasterTopicResponse(MasterTopic masterTopic)
        {
            var domain = await _unitOfWork.GetRepository<Domain.Entities.Domain>().GetByIdAsync(masterTopic.DomainId);
            var domainResponse = new DomainResponse
            {
                Id = domain.Id,
                Name = domain.Name,
                Description = domain.Description,
                Status = (StaticEnum.StatusEnum)Enum.Parse(typeof(StaticEnum.StatusEnum), domain.Status.ToString()),
                CreatedAt = domain.CreatedAt,
            };
            var response = new MasterTopicResponse
            {
                Id = masterTopic.Id,
                Domain = domainResponse,
                SemesterId = masterTopic.SemesterId,
                LecturerId = masterTopic.LecturerId,
                Name = masterTopic.Name,
                Description = masterTopic.Description,
                Status = masterTopic.Status,
                CreatedAt = masterTopic.CreatedAt,
            };
            return await Task.FromResult(response);
        }
    }
}