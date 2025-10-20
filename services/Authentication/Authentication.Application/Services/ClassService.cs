using Authentication.Application.Interfaces;
using Authentication.Application.DTOs.ClassDTOs;
using Authentication.Domain.Entities;
using Microsoft.Extensions.Logging;
using Authentication.Domain.Abstraction;
using Authentication.Domain.Enum;
using Authentication.Domain.Exceptions;
using Contracts.Common;
using Microsoft.EntityFrameworkCore;

namespace Authentication.Application.Services
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

        public async Task<BasePaginatedList<ClassResponse>> GetAllAsync(int page, int pageSize)
        {
            var classes = await _unitOfWork.GetRepository<Class>().GetAllAsync();
            var response = await Task.WhenAll(classes.Select(ToClassResponse));
            return new BasePaginatedList<ClassResponse>(response.ToList(), classes.Count, page, pageSize);
        }

        public async Task<ClassResponse> GetByIdAsync(Guid id)
        {
            var c = await _unitOfWork.GetRepository<Class>().GetByIdAsync(id);
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

            // Validate Semester exists
            var semester = await _unitOfWork.GetRepository<Semester>().GetByIdAsync(request.SemesterId);
            if (semester == null)
            {
                _logger.LogError("Semester not found with id: {SemesterId}", request.SemesterId);
                throw new CustomExceptions.ValidationException("Semester not found");
            }

            var c = new Class
            {
                Code = request.Code,
                SemesterId = request.SemesterId,
                LecturerId = request.LecturerId,
                Status = ClassStatusEnum.Active,
                CreatedAt = DateTime.UtcNow,
            };
            await _unitOfWork.GetRepository<Class>().InsertAsync(c);
            await _unitOfWork.SaveChangeAsync();
            return await ToClassResponse(c);
        }

        public async Task<ClassResponse> UpdateAsync(Guid id, ClassUpdateRequest request)
        {
            var c = await _unitOfWork.GetRepository<Class>().GetByIdAsync(id);
            if (c == null)
            {
                _logger.LogError("Class not found with id: {Id}", id);
                throw new CustomExceptions.NoDataFoundException("Class not found");
            }
            c.Code = request.Code ?? c.Code;
            c.SemesterId = request.SemesterId ?? c.SemesterId;
            c.LecturerId = request.LecturerId ?? c.LecturerId;
            await _unitOfWork.GetRepository<Class>().UpdateAsync(c);
            await _unitOfWork.SaveChangeAsync();
            return await ToClassResponse(c);
        }

        public async Task<ClassResponse> DeleteAsync(Guid id)
        {
            var c = await _unitOfWork.GetRepository<Class>().GetByIdAsync(id);
            if (c == null)
            {
                _logger.LogError("Class not found with id: {Id}", id);
                throw new CustomExceptions.NoDataFoundException("Class not found");
            }
            if (c.Status == ClassStatusEnum.Active)
            {
                _logger.LogError("Class is active, cannot delete");
                throw new CustomExceptions.BusinessLogicException("Class is active, cannot delete");
            }
            await _unitOfWork.GetRepository<Class>().SoftDeleteAsync(id);
            await _unitOfWork.SaveChangeAsync();
            return await ToClassResponse(c);
        }

        public async Task<ClassResponse> AssignLecturerAsync(Guid classId, Guid lecturerId)
        {
            var c = await _unitOfWork.GetRepository<Class>().GetByIdAsync(classId);
            if (c == null)
            {
                _logger.LogError("Class not found with id: {ClassId}", classId);
                throw new CustomExceptions.NoDataFoundException("Class not found");
            }
            var lecturer = await _unitOfWork.GetRepository<Account>().GetByIdAsync(lecturerId);
            if (lecturer == null)
            {
                _logger.LogError("Lecturer not found with id: {LecturerId}", lecturerId);
                throw new CustomExceptions.NoDataFoundException("Lecturer not found");
            }
            c.LecturerId = lecturerId;
            lecturer.ClassId = classId;
            await _unitOfWork.GetRepository<Class>().UpdateAsync(c);
            await _unitOfWork.GetRepository<Account>().UpdateAsync(lecturer);
            await _unitOfWork.SaveChangeAsync();
            return await ToClassResponse(c);
        }

        public async Task<ClassResponse> UnassignLecturerAsync(Guid classId)
        {
            var c = await _unitOfWork.GetRepository<Class>().GetByIdAsync(classId);
            if (c == null)
            {
                _logger.LogError("Class not found with id: {ClassId}", classId);
                throw new CustomExceptions.NoDataFoundException("Class not found");
            }
            c.LecturerId = null;
            await _unitOfWork.GetRepository<Class>().UpdateAsync(c);
            await _unitOfWork.SaveChangeAsync();
            return await ToClassResponse(c);
        }

        // public async Task<PaginatedResponse<ClassResponse>> SearchAsync(ClassQueryRequest query)
        // {
        //     var classRepo = _unitOfWork.GetRepository<Class>();

        //     // Build base query
        //     var baseQuery = classRepo.GetQueryable();

        //     // Apply search filter
        //     if (!string.IsNullOrEmpty(query.Search))
        //     {
        //         baseQuery = baseQuery.Where(c => c.Code.Contains(query.Search));
        //     }

        //     // Apply status filter
        //     if (query.Status.HasValue)
        //     {
        //         baseQuery = baseQuery.Where(c => c.Status == query.Status.Value);
        //     }

        //     // Apply semester filter
        //     if (query.SemesterId.HasValue)
        //     {
        //         baseQuery = baseQuery.Where(c => c.SemesterId == query.SemesterId.Value);
        //     }

        //     // Apply lecturer filter
        //     if (query.LecturerId.HasValue)
        //     {
        //         baseQuery = baseQuery.Where(c => c.LecturerId == query.LecturerId.Value);
        //     }

        //     // Apply sorting
        //     baseQuery = ApplySorting(baseQuery, query.SortBy, query.SortDirection);

        //     // Get total count for pagination
        //     var totalCount = await baseQuery.CountAsync();

        //     // Apply pagination
        //     var pagedQuery = baseQuery
        //         .Skip((query.Page - 1) * query.PageSize)
        //         .Take(query.PageSize);

        //     var classes = await pagedQuery.ToListAsync();

        //     // Convert to response
        //     var responses = await Task.WhenAll(classes.Select(c => ToClassResponse(c)));

        //     return new PaginatedResponse<ClassResponse>
        //     {
        //         Data = responses.ToList(),
        //         TotalCount = totalCount,
        //         Page = query.Page,
        //         PageSize = query.PageSize,
        //         TotalPages = (int)Math.Ceiling((double)totalCount / query.PageSize)
        //     };
        // }

        // private IQueryable<Class> ApplySorting(IQueryable<Class> query, string? sortBy, string? sortDirection)
        // {
        //     var isDescending = sortDirection?.ToLower() == "desc";

        //     return sortBy?.ToLower() switch
        //     {
        //         "code" => isDescending ? query.OrderByDescending(c => c.Code) : query.OrderBy(c => c.Code),
        //         "status" => isDescending ? query.OrderByDescending(c => c.Status) : query.OrderBy(c => c.Status),
        //         "createdat" => isDescending ? query.OrderByDescending(c => c.CreatedAt) : query.OrderBy(c => c.CreatedAt),
        //         _ => query.OrderByDescending(c => c.CreatedAt) // Default sort
        //     };
        // }

        private async Task<ClassResponse> ToClassResponse(Class c)
        {
            var lecturer = await _unitOfWork.GetRepository<Lecturer>().GetByIdAsync(c.LecturerId);

            var semester = await _unitOfWork.GetRepository<Semester>().GetByIdAsync(c.SemesterId);

            var response = new ClassResponse
            {
                Id = c.Id,
                Code = c.Code,
                Status = c.Status.ToString(),
                CreatedAt = c.CreatedAt,
                Lecturer = lecturer != null ? new LecturerResponse
                {
                    Id = lecturer.LecturerId,
                    FullName = lecturer.FullName,
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