using Authentication.Application.DTOs.SemesterDTOs;
using Contracts.Common;

namespace Authentication.Application.Interfaces
{
    public interface ISemesterService
    {
        Task<SemesterResponse> GetByIdAsync(Guid id);
        Task<BasePaginatedList<SemesterResponse>> GetAllAsync(int page, int pageSize);
        Task<SemesterResponse> CreateAsync(SemesterCreateRequest request);
        Task<SemesterResponse> UpdateAsync(Guid id, SemesterUpdateRequest request);
        Task<SemesterResponse> DeleteAsync(Guid id);
        Task<SemesterResponse> SemesterToggleStatusAsync(Guid id);
    }
}