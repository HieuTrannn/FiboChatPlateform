using Authentication.Application.Interfaces;
using Authentication.Application.DTOs.ClassDTOs;
using Authentication.Domain.Entities;
using Microsoft.Extensions.Logging;
using Authentication.Domain.Abstraction;
using Authentication.Domain.Enum;
using Authentication.Domain.Exceptions;
using Contracts.Common;

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

            var items = new List<ClassResponse>(classes.Count);
            foreach (var c in classes)
            {
                items.Add(await ToClassResponse(c)); // sequential awaits => no concurrent DbContext ops
            }

            return new BasePaginatedList<ClassResponse>(items, classes.Count, page, pageSize);
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

        public async Task<ClassStudentResponse> AddStudentToClassAsync(Guid classId, List<Guid>? userIds)
        {
            try
            {
                _logger.LogInformation("Starting to add students to class {ClassId}", classId);

                // Validate input
                if (userIds == null || !userIds.Any())
                {
                    _logger.LogWarning("No user IDs provided for class {ClassId}", classId);
                    throw new CustomExceptions.ValidationException("User IDs cannot be empty");
                }

                if (userIds.Any(id => id == Guid.Empty))
                {
                    _logger.LogWarning("Invalid user ID provided for class {ClassId}", classId);
                    throw new CustomExceptions.ValidationException("Invalid user ID provided");
                }

                // Check if class exists
                var c = await _unitOfWork.GetRepository<Class>().GetByIdAsync(classId);
                if (c == null)
                {
                    _logger.LogError("Class not found with id: {ClassId}", classId);
                    throw new CustomExceptions.NoDataFoundException("Class not found");
                }

                _logger.LogInformation("Class found: {ClassCode}", c.Code);

                var addedStudents = new List<StudentResponse>();
                var existingEnrollments = new List<ClassEnrollment>();
                var newEnrollments = new List<ClassEnrollment>();

                foreach (var userId in userIds)
                {
                    _logger.LogInformation("Processing user {UserId} for class {ClassId}", userId, classId);

                    // Validate user exists
                    var user = await _unitOfWork.GetRepository<Account>().GetByIdAsync(userId);
                    if (user == null)
                    {
                        _logger.LogError("User not found with id: {UserId}", userId);
                        throw new CustomExceptions.NoDataFoundException($"User not found with id: {userId}");
                    }

                    _logger.LogInformation("User found: {UserEmail}", user.Email);

                    // Check if user is already enrolled
                    var existingEnrollment = await _unitOfWork.GetRepository<ClassEnrollment>()
                        .GetAllAsync(x => x.UserId == userId && x.ClassId == classId && x.Status == ClassEnrollmentStatusEnum.Active);

                    if (existingEnrollment.Any())
                    {
                        existingEnrollments.Add(existingEnrollment.First());
                        _logger.LogInformation("User {UserId} is already enrolled in class {ClassId}", userId, classId);
                        continue;
                    }

                    // Create new enrollment with GroupId = null (chưa có nhóm)
                    var enrollment = new ClassEnrollment
                    {
                        UserId = userId,
                        ClassId = classId,
                        GroupId = null, // ✅ Null vì học sinh chưa được phân nhóm
                        RoleInClass = "student",
                        Status = ClassEnrollmentStatusEnum.Active,
                        CreatedAt = DateTime.UtcNow
                    };

                    newEnrollments.Add(enrollment);
                    addedStudents.Add(new StudentResponse
                    {
                        Id = user.Id,
                        FirstName = user.Firstname ?? "",
                        LastName = user.Lastname ?? "",
                        Email = user.Email ?? "",
                        StudentId = user.StudentID ?? "",
                        RoleInClass = "student",
                        Status = ClassEnrollmentStatusEnum.Active.ToString()
                    });

                    _logger.LogInformation("Prepared enrollment for user {UserId} in class {ClassId} (no group assigned yet)", userId, classId);
                }

                // Insert all new enrollments at once
                if (newEnrollments.Any())
                {
                    _logger.LogInformation("Inserting {Count} new enrollments with GroupId = null", newEnrollments.Count);
                    await _unitOfWork.GetRepository<ClassEnrollment>().InsertRangeAsync(newEnrollments);
                    existingEnrollments.AddRange(newEnrollments);
                    _logger.LogInformation("Successfully inserted enrollments");
                }

                // Save changes only once
                _logger.LogInformation("Saving changes to database");
                await _unitOfWork.SaveChangeAsync();
                _logger.LogInformation("Successfully saved changes");

                _logger.LogInformation("Successfully added {Count} students to class {ClassId} (no groups assigned)", addedStudents.Count, classId);
                return await ToClassStudentResponse(c, existingEnrollments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AddStudentToClassAsync for class {ClassId}: {Message}", classId, ex.Message);
                throw;
            }
        }

        public async Task<ClassStudentResponse> RemoveStudentFromClassAsync(Guid classId, List<Guid> userIds)
        {
            var c = await _unitOfWork.GetRepository<Class>().GetByIdAsync(classId);
            if (c == null)
            {
                _logger.LogError("Class not found with id: {ClassId}", classId);
                throw new CustomExceptions.NoDataFoundException("Class not found");
            }

            var remainingEnrollments = new List<ClassEnrollment>();

            foreach (var userId in userIds)
            {
                var enrollments = await _unitOfWork.GetRepository<ClassEnrollment>()
                    .GetAllAsync(x => x.UserId == userId && x.ClassId == classId && x.Status == ClassEnrollmentStatusEnum.Active);

                if (enrollments.Any())
                {
                    await _unitOfWork.GetRepository<ClassEnrollment>().DeleteAsync(enrollments.First().Id);
                }
            }

            // Get remaining active enrollments
            var activeEnrollments = await _unitOfWork.GetRepository<ClassEnrollment>()
                .GetAllAsync(x => x.ClassId == classId && x.Status == ClassEnrollmentStatusEnum.Active);

            await _unitOfWork.SaveChangeAsync();
            return await ToClassStudentResponse(c, activeEnrollments.ToList());
        }

        public async Task<List<ClassStudentResponse>> GetAllStudentsOfClassAsync(Guid classId)
        {
            var c = await _unitOfWork.GetRepository<Class>().GetByIdAsync(classId);
            if (c == null)
            {
                _logger.LogError("Class not found with id: {ClassId}", classId);
                throw new CustomExceptions.NoDataFoundException("Class not found");
            }

            var enrollments = await _unitOfWork.GetRepository<ClassEnrollment>()
                .GetAllAsync(x => x.ClassId == classId && x.Status == ClassEnrollmentStatusEnum.Active);

            var result = new List<ClassStudentResponse>();

            // Group enrollments by class (in case there are multiple classes)
            var groupedEnrollments = enrollments.GroupBy(e => e.ClassId);

            foreach (var group in groupedEnrollments)
            {
                var classEnrollments = group.ToList();
                result.Add(await ToClassStudentResponse(c, classEnrollments));
            }

            return result;
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

        private async Task<ClassStudentResponse> ToClassStudentResponse(Class c, List<ClassEnrollment> enrollments)
        {
            var lecturer = await _unitOfWork.GetRepository<Lecturer>().GetByIdAsync(c.LecturerId);

            var students = new List<StudentResponse>();

            foreach (var enrollment in enrollments)
            {
                var user = await _unitOfWork.GetRepository<Account>().GetByIdAsync(enrollment.UserId);
                if (user != null)
                {
                    students.Add(new StudentResponse
                    {
                        Id = user.Id,
                        FirstName = user.Firstname ?? "",
                        LastName = user.Lastname ?? "",
                        Email = user.Email ?? "",
                        StudentId = user.StudentID ?? "",
                        RoleInClass = enrollment.RoleInClass,
                        Status = enrollment.Status.ToString()
                    });
                }
            }

            var response = new ClassStudentResponse
            {
                Id = c.Id,
                Code = c.Code,
                Status = c.Status.ToString(),
                CreatedAt = c.CreatedAt,
                Lecturer = lecturer != null ? new ClassLecturerResponse
                {
                    Id = lecturer.LecturerId,
                    FullName = lecturer.FullName,
                } : null,
                Students = students
            };

            return response;
        }
    }
}