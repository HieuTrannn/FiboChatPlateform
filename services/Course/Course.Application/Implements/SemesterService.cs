using Course.Application.Interfaces;
using Course.Domain.DTOs.SemesterDTOs;
using Course.Domain.Entities;
using Course.Domain.Exceptions;
using static Course.Domain.Enums.StaticEnums;
using Course.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;

namespace Course.Application.Implements
{
    public class SemesterService : ISemesterService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SemesterService> _logger;

        public SemesterService(IUnitOfWork unitOfWork, ILogger<SemesterService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<SemesterResponse> GetByIdAsync(Guid id)
        {
            var semester = await _unitOfWork.Semesters.GetByIdAsync(id);
            if (semester == null)
            {
                _logger.LogError("Semester not found with id: {Id}", id);
                throw new CustomExceptions.NoDataFoundException("Semester not found");
            }
            var response = await ToSemesterResponse(semester);

            return response;
        }

        public async Task<List<SemesterResponse>> GetAllAsync()
        {
            var semesters = await _unitOfWork.Semesters.GetAllAsync(x => x.Where(x => x.Status == SemesterStatus.Active));
            var responses = await Task.WhenAll(semesters.Select(ToSemesterResponse));
            return responses.ToList();
        }

        public async Task<SemesterResponse> CreateAsync(SemesterCreateRequest request)
        {
            // Check if semester with same code already exists
            var existingSemester = await _unitOfWork.Semesters.ExistsAsync(x => x.Code == request.Code);
            if (existingSemester)
            {
                _logger.LogError("Semester with code: {Code} already exists", request.Code);
                throw new CustomExceptions.AlreadyExistsException("Semester with this code already exists");
            }

            // Check start date and end date
            if (request.StartDate >= request.EndDate)
            {
                _logger.LogError("Start date must be before end date");
                throw new CustomExceptions.ValidationException("Start date must be before end date");
            }

            var semester = new Semester
            {
                Code = request.Code,
                Term = request.Term,
                Year = request.Year,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
            };

            await _unitOfWork.Semesters.InsertAsync(semester);
            await _unitOfWork.SaveChangesAsync();

            return await ToSemesterResponse(semester);
        }

        public async Task<SemesterResponse> UpdateAsync(Guid id, SemesterUpdateRequest request)
        {
            var semester = await _unitOfWork.Semesters.GetByIdAsync(id);
            if (semester == null)
            {
                _logger.LogError("Semester not found with id: {Id}", id);
                throw new CustomExceptions.NoDataFoundException("Semester not found");
            }
            semester.Code = request.Code ?? semester.Code;
            semester.Term = request.Term ?? semester.Term;
            semester.Year = request.Year ?? semester.Year;
            semester.StartDate = request.StartDate ?? semester.StartDate;
            semester.EndDate = request.EndDate ?? semester.EndDate;

            await _unitOfWork.Semesters.UpdateAsync(semester);
            await _unitOfWork.SaveChangesAsync();

            return await ToSemesterResponse(semester);
        }

        public async Task<SemesterResponse> DeleteAsync(Guid id)
        {
            var semester = await _unitOfWork.Semesters.GetByIdAsync(id);
            if (semester == null)
            {
                _logger.LogError("Semester not found with id: {Id}", id);
                throw new CustomExceptions.NoDataFoundException("Semester not found");
            }
            if (semester.Status == SemesterStatus.Active)
            {
                _logger.LogError("Semester is active, cannot delete");
                throw new CustomExceptions.BusinessLogicException("Semester is active, cannot delete");
            }
            await _unitOfWork.Semesters.DeleteStatusAsync(id);
            await _unitOfWork.SaveChangesAsync();
            return await ToSemesterResponse(semester);
        }

        public async Task<SemesterResponse> SemesterToggleStatusAsync(Guid id)
        {
            var semester = await _unitOfWork.Semesters.GetByIdAsync(id);
            if (semester == null)
            {
                _logger.LogError("Semester not found with id: {Id}", id);
                throw new CustomExceptions.NoDataFoundException("Semester not found");
            }

            semester.Status = semester.Status == SemesterStatus.Active
                ? SemesterStatus.Pending
                : SemesterStatus.Active;

            await _unitOfWork.Semesters.UpdateAsync(semester);
            await _unitOfWork.SaveChangesAsync();
            return await ToSemesterResponse(semester);
        }

        private async Task<SemesterResponse> ToSemesterResponse(Semester semester)
        {
            var response = new SemesterResponse
            {
                Id = semester.Id,
                Code = semester.Code,
                Term = semester.Term,
                Year = semester.Year,
                Status = semester.Status,
                StartDate = semester.StartDate ?? DateTime.UtcNow,
                EndDate = semester.EndDate ?? DateTime.UtcNow,
                CreatedAt = semester.CreatedAt,
            };
            return await Task.FromResult(response);
        }
    }
}