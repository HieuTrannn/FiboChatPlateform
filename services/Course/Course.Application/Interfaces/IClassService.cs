using Course.Domain.DTOs.ClassDTOs;

namespace Course.Application.Interfaces
{
    public interface IClassService
    {
        Task<List<ClassResponse>> GetAllAsync();
        Task<ClassResponse> GetByIdAsync(Guid id);
        Task<ClassResponse> CreateAsync(ClassCreateRequest request);
        Task<ClassResponse> UpdateAsync(Guid id, ClassUpdateRequest request);
        Task<ClassResponse> DeleteAsync(Guid id);
    }
}