using Course.Application.DTOs.MasterTopicDTOs;

namespace Course.Application.Interfaces
{
    public interface IExternalApiService
    {
        Task<SemesterResponse?> GetSemesterByIdAsync(string semesterId);
        Task<LecturerResponse?> GetLecturerByIdAsync(string lecturerId);
    }
}