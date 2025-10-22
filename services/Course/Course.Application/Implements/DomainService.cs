using Course.Application.Interfaces;
using Course.Application.DTOs.DomainDTOs;
using Course.Domain.Abstraction;
using Microsoft.Extensions.Logging;
using Contracts.Common;
using Course.Domain.Exceptions;

namespace Course.Application.Implements
{
    public class DomainService : IDomainService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DomainService> _logger;

        public DomainService(IUnitOfWork unitOfWork, ILogger<DomainService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<DomainResponse> GetByIdAsync(Guid id)
        {
            var domain = await _unitOfWork.GetRepository<Domain.Entities.Domain>().GetByIdAsync(id);
            if (domain == null)
            {
                _logger.LogError("Domain not found with id: {Id}", id);
                throw new CustomExceptions.NoDataFoundException("Domain not found");
            }
            return await ToDomainResponse(domain);
        }

        public async Task<BasePaginatedList<DomainResponse>> GetAllAsync(int page, int pageSize)
        {
            var domains = await _unitOfWork.GetRepository<Domain.Entities.Domain>().GetAllAsync();
            var response = await Task.WhenAll(domains.Select(ToDomainResponse));
            return new BasePaginatedList<DomainResponse>(response, domains.Count, page, pageSize);
        }

        public async Task<DomainResponse> CreateAsync(DomainCreateRequest request)
        {
            var domain = new Domain.Entities.Domain
            {
                Name = request.Name,
                Description = request.Description,
            };
            await _unitOfWork.GetRepository<Domain.Entities.Domain>().InsertAsync(domain);
            await _unitOfWork.SaveChangesAsync();
            return await ToDomainResponse(domain);
        }

        public async Task<DomainResponse> UpdateAsync(Guid id, DomainUpdateRequest request)
        {
            var domain = await _unitOfWork.GetRepository<Domain.Entities.Domain>().GetByIdAsync(id);
            if (domain == null)
            {
                _logger.LogError("Domain not found with id: {Id}", id);
                throw new CustomExceptions.NoDataFoundException("Domain not found");
            }
            domain.Name = request.Name ?? domain.Name;
            domain.Description = request.Description ?? domain.Description;
            await _unitOfWork.GetRepository<Domain.Entities.Domain>().UpdateAsync(domain);
            await _unitOfWork.SaveChangesAsync();
            return await ToDomainResponse(domain);
        }

        public async Task<DomainResponse> DeleteAsync(Guid id)
        {
            var domain = await _unitOfWork.GetRepository<Domain.Entities.Domain>().GetByIdAsync(id);
            if (domain == null)
            {
                _logger.LogError("Domain not found with id: {Id}", id);
                throw new CustomExceptions.NoDataFoundException("Domain not found");
            }
            await _unitOfWork.GetRepository<Domain.Entities.Domain>().SoftDeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
            return await ToDomainResponse(domain);
        }

        private async Task<DomainResponse> ToDomainResponse(Domain.Entities.Domain domain)
        {
            var response = new DomainResponse
            {
                Id = domain.Id,
                Name = domain.Name,
                Description = domain.Description,
                Status = domain.Status,
                CreatedAt = domain.CreatedAt,
            };
            return await Task.FromResult(response);
        }
    }
}