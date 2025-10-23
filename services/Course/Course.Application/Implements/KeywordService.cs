using Course.Domain.Exceptions;
using Contracts.Common;
using Course.Application.DTOs.KeywordDTOs;
using Course.Application.Interfaces;
using Course.Domain.Entities;
using Course.Domain.Abstraction;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Globalization;

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
            var normalizedName = NormalizeString(request.Name);

            // Tìm keyword đã tồn tại với normalized name (so sánh trực tiếp)
            var existingKeyword = await _unitOfWork.GetRepository<Keyword>()
                .GetFirstOrDefaultAsync(k => NormalizeString(k.Name) == normalizedName);

            Keyword keyword;

            if (existingKeyword != null)
            {
                keyword = existingKeyword;
                _logger.LogInformation("Reusing existing keyword: '{ExistingName}' for input: '{InputName}'",
                    existingKeyword.Name, request.Name);
            }
            else
            {
                keyword = new Keyword
                {
                    Name = request.Name.Trim() // Giữ nguyên format nhưng trim whitespace
                };
                await _unitOfWork.GetRepository<Keyword>().InsertAsync(keyword);
                await _unitOfWork.SaveChangesAsync();
            }

            // Handle N:N relationship với MasterTopic
            if (request.MasterTopicIds != null && request.MasterTopicIds.Any())
            {
                var masterTopicIds = request.MasterTopicIds
                    .Where(id => id != Guid.Empty)
                    .Distinct()
                    .ToList();

                foreach (var masterTopicId in masterTopicIds)
                {
                    // Kiểm tra relationship đã tồn tại chưa
                    var existingRelation = await _unitOfWork.GetRepository<MasterTopicKeyword>()
                        .GetFirstOrDefaultAsync(mtk => mtk.KeywordId == keyword.Id && mtk.MasterTopicId == masterTopicId);

                    if (existingRelation == null)
                    {
                        keyword.MasterTopicKeywords.Add(new MasterTopicKeyword
                        {
                            MasterTopicId = masterTopicId,
                            CreatedAt = DateTime.UtcNow
                        });
                    }
                }

                await _unitOfWork.SaveChangesAsync();
            }

            return await ToKeywordResponse(keyword);
        }

        private string NormalizeString(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            return input
                .Trim()
                .ToLowerInvariant()
                .Normalize(NormalizationForm.FormD)
                .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                .Aggregate(new StringBuilder(), (sb, c) => sb.Append(c))
                .ToString()
                .Replace(" ", "")
                .Replace("-", "")
                .Replace("_", "");
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

        public async Task DeleteAsync(Guid id)
        {
            var keyword = await _unitOfWork.GetRepository<Keyword>().GetByIdAsync(id);
            if (keyword == null)
            {
                _logger.LogError("Keyword not found with id: {Id}", id);
                throw new CustomExceptions.NoDataFoundException("Keyword not found");
            }
            await _unitOfWork.GetRepository<Keyword>().SoftDeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
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