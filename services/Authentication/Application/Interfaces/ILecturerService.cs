using Contracts.Common;
using static Authentication.Application.DTOs.AuthenDTO;
using static Authentication.Application.DTOs.LecturerDTO;

namespace Authentication.Application.Interfaces
{
    public interface ILecturerService
    {
        Task<ApiResponse<RegisterResponse>> CreateLecturer(LecturerRequest request);
        Task<List<LecturerResponse>> GetAllLecturers();
        Task<LecturerResponse> GetLecturerById(string id);
        Task<RegisterResponse> DeleteLecturerById(Guid id);

        Task<RegisterResponse> UpdateLecturerById(Guid id, LecturerRequest request);
    }
}
