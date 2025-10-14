using Course.Domain.DTOs.SemesterDTOs;

namespace Course.Application.Interfaces
{
    public interface ISemesterService
    {
        Task<SemesterResponse> GetByIdAsync(Guid id);
        Task<List<SemesterResponse>> GetAllAsync();
        Task<SemesterResponse> CreateAsync(SemesterCreateRequest request);
        Task<SemesterResponse> UpdateAsync(Guid id, SemesterUpdateRequest request);
        Task<SemesterResponse> DeleteAsync(Guid id);
        Task<SemesterResponse> SemesterToggleStatusAsync(Guid id);
    }
}