using Course.Domain.Exceptions;
using Contracts.Common;
using Course.Application.DTOs.KeywordDTOs;
using Course.Application.Interfaces;
using Course.Domain.Entities;
using Course.Domain.Abstraction;
using Microsoft.Extensions.Logging;

namespace Course.Application.Implements
{
    public class KeywordService : IKeywordService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<KeywordService> _logger;

        public KeywordService(IUnitOfWork unitOfWork, ILogger<KeywordService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<KeywordResponse> GetByIdAsync(Guid id)
        {
            var keyword = await _unitOfWork.GetRepository<Keyword>().GetByIdAsync(id);
            if (keyword == null)
            {
                _logger.LogError("Keyword not found with id: {Id}", id);
                throw new CustomExceptions.NoDataFoundException("Keyword not found");
            }
            return await ToKeywordResponse(keyword);
        }

        public async Task<BasePaginatedList<KeywordResponse>> GetAllAsync(int page, int pageSize)
        {
            var keywords = await _unitOfWork.GetRepository<Keyword>().GetAllAsync();
            var response = await Task.WhenAll(keywords.Select(ToKeywordResponse));
            return new BasePaginatedList<KeywordResponse>(response, keywords.Count, page, pageSize);
        }

        public async Task<KeywordResponse> CreateAsync(KeywordCreateRequest request)
        {
            var keyword = new Keyword
            {
                Name = request.Name,
            };
            await _unitOfWork.GetRepository<Keyword>().InsertAsync(keyword);
            await _unitOfWork.SaveChangesAsync();
            return await ToKeywordResponse(keyword);
        }

        public async Task<KeywordResponse> UpdateAsync(Guid id, KeywordUpdateRequest request)
        {
            var keyword = await _unitOfWork.GetRepository<Keyword>().GetByIdAsync(id);
            if (keyword == null)
            {
                _logger.LogError("Keyword not found with id: {Id}", id);
                throw new CustomExceptions.NoDataFoundException("Keyword not found");
            }
            keyword.Name = request.Name ?? keyword.Name;
            keyword.Status = StaticEnum.StatusEnum.Active;
            await _unitOfWork.GetRepository<Keyword>().UpdateAsync(keyword);
            await _unitOfWork.SaveChangesAsync();
            return await ToKeywordResponse(keyword);
        }

        public async Task<KeywordResponse> DeleteAsync(Guid id)
        {
            var keyword = await _unitOfWork.GetRepository<Keyword>().GetByIdAsync(id);
            if (keyword == null)
            {
                _logger.LogError("Keyword not found with id: {Id}", id);
                throw new CustomExceptions.NoDataFoundException("Keyword not found");
            }
            await _unitOfWork.GetRepository<Keyword>().SoftDeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
            return await ToKeywordResponse(keyword);
        }

        private async Task<KeywordResponse> ToKeywordResponse(Keyword keyword)
        {
            var response = new KeywordResponse
            {
                Id = keyword.Id,
                Name = keyword.Name,
                Status = keyword.Status,
                CreatedAt = keyword.CreatedAt,
            };
            return await Task.FromResult(response);
        }
    }
}