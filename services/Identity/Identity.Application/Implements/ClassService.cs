using Identity.Application.Interfaces;
using Identity.Contracts.DTOs.ClassDTOs;
using Identity.Domain.Entities;
using Identity.Domain.Exceptions;
using Identity.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Implements
{
    public class ClassService : IClassService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ClassService> _logger;

        public ClassService(IUnitOfWork unitOfWork, ILogger<ClassService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<List<ClassResponse>> GetAllClassesAsync()
        {
            var classes = await _unitOfWork.Classes.GetAllAsync();
            var response = classes.Select(c => new ClassResponse
            {
                Id = c.Id,
                Name = c.Name,
                Code = c.Code,
                SemesterId = c.SemesterId,
                LecturerId = c.LecturerId,
                Status = c.Status,
                CreatedAt = c.CreatedAt,
            }).ToList();

            return response;
        }

        public async Task<ClassResponse> GetClassByIdAsync(Guid id)
        {
            var c = await _unitOfWork.Classes.GetByIdAsync(id);
            if (c == null)
            {
                _logger.LogError("Class not found with id: {Id}", id);
                throw new CustomExceptions.NoDataFoundException("Class not found");
            }

            var response = new ClassResponse
            {
                Id = c.Id,
                Name = c.Name,
                Code = c.Code,
                SemesterId = c.SemesterId,
                LecturerId = c.LecturerId,
                Status = c.Status,
                CreatedAt = c.CreatedAt,
            };

            return response;
        }

        public async Task<ClassResponse> CreateClassAsync(ClassCreateRequest request)
        {
            var c = new Class(request.SemesterId, request.Code, request.Name, request.LecturerId);
            await _unitOfWork.Classes.InsertAsync(c);
            await _unitOfWork.SaveChangesAsync();

            var response = new ClassResponse
            {
                Id = c.Id,
                Name = c.Name,
                Code = c.Code,
                SemesterId = c.SemesterId,
                LecturerId = c.LecturerId,
                Status = c.Status,
                CreatedAt = c.CreatedAt,
            };

            return response;
        }

        public async Task<ClassResponse> UpdateClassAsync(Guid id, ClassUpdateRequest request)
        {
            var c = await _unitOfWork.Classes.GetByIdAsync(id);
            if (c == null)
            {
                _logger.LogError("Class not found with id: {Id}", id);
                throw new CustomExceptions.NoDataFoundException("Class not found");
            }

            c.Name = request.Name ?? c.Name;
            c.Code = request.Code ?? c.Code;
            c.SemesterId = request.SemesterId;
            c.LecturerId = request.LecturerId ?? c.LecturerId;
            await _unitOfWork.Classes.UpdateAsync(c);
            await _unitOfWork.SaveChangesAsync();

            var response = new ClassResponse
            {
                Id = c.Id,
                Name = c.Name,
                Code = c.Code,
                SemesterId = c.SemesterId,
                LecturerId = c.LecturerId,
                Status = c.Status,
                CreatedAt = c.CreatedAt,
            };

            return response;
        }

        public async Task<ClassResponse> DeleteClassAsync(Guid id)
        {
            var c = await _unitOfWork.Classes.GetByIdAsync(id);
            if (c == null)
            {
                _logger.LogError("Class not found with id: {Id}", id);
                throw new CustomExceptions.NoDataFoundException("Class not found");
            }

            await _unitOfWork.Classes.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();

            var response = new ClassResponse
            {
                Id = c.Id,
            };

            return response;
        }
    }
}