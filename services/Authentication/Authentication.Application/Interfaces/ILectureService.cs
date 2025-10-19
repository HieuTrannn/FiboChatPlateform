using Contracts.Common;
using static Authentication.Application.DTOs.AuthenDTO;
using static Authentication.Application.DTOs.LectureDTO;

namespace Authentication.Application.Interfaces
{
    public interface ILectureService
    {
        Task<ApiResponse<RegisterResponse>> CreateLecturer(LectureRequest request);
        Task<List<LectureResponse>> GetAllLecturers();
        Task<LectureResponse> GetLecturerById(string id);
        Task<RegisterResponse> DeleteLecturerById(Guid id);

        Task<RegisterResponse> UpdateLecturerById(Guid id, LectureRequest request);
    }
}
