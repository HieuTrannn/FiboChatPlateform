using Course.Application.Interfaces;
using Course.Domain.DTOs.ClassDTOs;
using Course.Domain.Entities;
using Course.Domain.Exceptions;
using Course.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;
using static Course.Domain.Enums.StaticEnums;

namespace Course.Application.Implements
{
    public class ClassService : IClassService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ClassService> _logger;
        private readonly IAccountsClient _accountsClient;

        public ClassService(IUnitOfWork unitOfWork, ILogger<ClassService> logger, IAccountsClient accountsClient)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _accountsClient = accountsClient;
        }

        public async Task<List<ClassResponse>> GetAllAsync()
        {
            var classes = await _unitOfWork.Classes.GetAllAsync(c => c.Where(c => c.Status == ClassStatus.Active));
            var response = await Task.WhenAll(classes.Select(ToClassResponse));
            return response.ToList();
        }

        public async Task<ClassResponse> GetByIdAsync(Guid id)
        {
            var c = await _unitOfWork.Classes.GetByIdAsync(id);
            if (c == null)
            {
                _logger.LogError("Class not found with id: {Id}", id);
                throw new CustomExceptions.NoDataFoundException("Class not found");
            }
            return await ToClassResponse(c);
        }

        public async Task<ClassResponse> CreateAsync(ClassCreateRequest request)
        {
            if (request.SemesterId == Guid.Empty)
            {
                _logger.LogError("Semester id is required");
                throw new CustomExceptions.ValidationException("Semester id is required");
            }
            if (string.IsNullOrEmpty(request.Code))
            {
                _logger.LogError("Code is required");
                throw new CustomExceptions.ValidationException("Code is required");
            }
            if (request.LecturerId == Guid.Empty)
            {
                _logger.LogError("Lecturer id is required");
                throw new CustomExceptions.ValidationException("Lecturer id is required");
            }
            var c = new Class
            {
                Code = request.Code,
                SemesterId = request.SemesterId,
                LecturerId = request.LecturerId,
                Status = ClassStatus.Active,
                CreatedAt = DateTime.UtcNow,
            };
            await _unitOfWork.Classes.InsertAsync(c);
            await _unitOfWork.SaveChangesAsync();
            return await ToClassResponse(c);
        }

        public async Task<ClassResponse> UpdateAsync(Guid id, ClassUpdateRequest request)
        {
            var c = await _unitOfWork.Classes.GetByIdAsync(id);
            if (c == null)
            {
                _logger.LogError("Class not found with id: {Id}", id);
                throw new CustomExceptions.NoDataFoundException("Class not found");
            }
            c.Code = request.Code ?? c.Code;
            c.SemesterId = request.SemesterId ?? c.SemesterId;
            c.LecturerId = request.LecturerId ?? c.LecturerId;
            await _unitOfWork.Classes.UpdateAsync(c);
            await _unitOfWork.SaveChangesAsync();
            return await ToClassResponse(c);
        }

        public async Task<ClassResponse> DeleteAsync(Guid id)
        {
            var c = await _unitOfWork.Classes.GetByIdAsync(id);
            if (c == null)
            {
                _logger.LogError("Class not found with id: {Id}", id);
                throw new CustomExceptions.NoDataFoundException("Class not found");
            }
            if (c.Status == ClassStatus.Active)
            {
                _logger.LogError("Class is active, cannot delete");
                throw new CustomExceptions.BusinessLogicException("Class is active, cannot delete");
            }   
            await _unitOfWork.Classes.DeleteStatusAsync(id);
            await _unitOfWork.SaveChangesAsync();
            return await ToClassResponse(c);
        }

        private async Task<ClassResponse> ToClassResponse(Class c)
        {
            var lecturer = c.LecturerId is Guid lid
                ? await _accountsClient.GetLecturerByIdAsync(lid)
                : null;

            var semester = await _unitOfWork.Semesters.GetByIdAsync(c.SemesterId);

            var response = new ClassResponse
            {
                Id = c.Id,
                Code = c.Code,
                Status = c.Status.ToString(),
                CreatedAt = c.CreatedAt,
                Lecturer = lecturer != null ? new LecturerResponse
                {
                    Id = lecturer.Id,
                    Firstname = lecturer.Firstname,
                    Lastname = lecturer.Lastname,
                } : null,
                Semester = semester != null ? new SemesterResponse
                {
                    Id = semester.Id,
                    Code = semester.Code,
                    Term = semester.Term,
                    Year = semester.Year,
                    CreatedAt = semester.CreatedAt,
                } : null,
            };
            return response;
        }
    }
}