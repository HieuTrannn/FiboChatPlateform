using Authentication.Application.DTOs.ClassDTOs;
using Contracts.Common;

namespace Authentication.Application.Interfaces
{
    public interface IClassService
    {
        Task<BasePaginatedList<ClassResponse>> GetAllAsync(int page, int pageSize);
        Task<ClassResponse> GetByIdAsync(Guid id);
        Task<ClassResponse> CreateAsync(ClassCreateRequest request);
        Task<ClassResponse> UpdateAsync(Guid id, ClassUpdateRequest request);
        Task<ClassResponse> DeleteAsync(Guid id);
        Task<ClassResponse> AssignLecturerAsync(Guid classId, Guid lecturerId);
        Task<ClassResponse> UnassignLecturerAsync(Guid classId);
        // Task<PaginatedResponse<ClassResponse>> SearchAsync(ClassQueryRequest request);
    }
}