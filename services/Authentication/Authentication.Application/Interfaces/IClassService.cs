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
        // Task<PaginatedResponse<ClassResponse>> SearchAsync(ClassQueryRequest request);

        // Lecturer Management
        Task<ClassResponse> AssignLecturerAsync(Guid classId, Guid lecturerId);
        Task<ClassResponse> UnassignLecturerAsync(Guid classId);
        Task<BasePaginatedList<ClassLectrurerResponse>> GetAllClassesOfLecturerAsync(Guid lecturerId, int page, int pageSize);

        // Student Management
        Task<ClassStudentResponse> AddStudentToClassAsync(Guid classId, List<Guid> userIds);
        Task<ClassStudentResponse> RemoveStudentFromClassAsync(Guid classId, List<Guid> userIds);
        Task<List<ClassStudentResponse>> GetAllStudentsOfClassAsync(Guid classId);
    }
}