using Identity.Contracts.DTOs.ClassDTOs;

namespace Identity.Application.Interfaces
{
    public interface IClassService
    {
        Task<List<ClassResponse>> GetAllClassesAsync();
        Task<ClassResponse> GetClassByIdAsync(Guid id);
        Task<ClassResponse> CreateClassAsync(ClassCreateRequest request);
        Task<ClassResponse> UpdateClassAsync(Guid id, ClassUpdateRequest request);
        Task<ClassResponse> DeleteClassAsync(Guid id);
    }
}