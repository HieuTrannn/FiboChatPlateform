using Authentication.Application.DTOs.GroupDTOs;
using Authentication.Application.Interfaces;
using Authentication.Domain.Abstraction;
using Authentication.Domain.Entities;
using Authentication.Domain.Exceptions;
using Contracts.Common;
using Microsoft.Extensions.Logging;
using Authentication.Domain.Enum;

namespace Authentication.Application.Services
{
    public class GroupService : IGroupService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GroupService> _logger;

        public GroupService(IUnitOfWork unitOfWork, ILogger<GroupService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<BasePaginatedList<GroupResponse>> GetAllAsync(int page, int pageSize)
        {
            var groups = await _unitOfWork.GetRepository<Group>().GetAllAsync();
            var response = await Task.WhenAll(groups.Select(ToGroupResponse));
            return new BasePaginatedList<GroupResponse>(response, groups.Count, page, pageSize);
        }

        public async Task<BasePaginatedList<GroupResponse>> GetAllByClassIdAsync(Guid classId, int page, int pageSize)
        {
            var groups = await _unitOfWork.GetRepository<Group>().GetAllAsync(x => x.Where(x => x.ClassId == classId));
            var response = await Task.WhenAll(groups.Select(ToGroupResponse));
            return new BasePaginatedList<GroupResponse>(response, groups.Count, page, pageSize);
        }

        public async Task<GroupDetailResponse> GetByIdAsync(Guid id)
        {
            var group = await _unitOfWork.GetRepository<Group>().GetByIdAsync(id);
            if (group == null)
            {
                _logger.LogError("Group not found with id: {Id}", id);
                throw new CustomExceptions.NoDataFoundException("Group not found");
            }
            var response = await ToGroupDetailResponse(group);

            return response;
        }

        public async Task<GroupResponse> CreateAsync(GroupCreateRequest request)
        {
            var group = new Group
            {
                ClassId = request.ClassId,
                Name = request.Name,
                Description = request.Description,
                Status = StaticEnum.StatusEnum.Active,
            };
            await _unitOfWork.GetRepository<Group>().InsertAsync(group);
            await _unitOfWork.SaveChangeAsync();
            var response = await ToGroupResponse(group);
            return response;
        }

        public async Task<GroupResponse> UpdateAsync(Guid id, GroupUpdateRequest request)
        {
            var group = await _unitOfWork.GetRepository<Group>().GetByIdAsync(id);
            if (group == null)
            {
                _logger.LogError("Group not found with id: {Id}", id);
                throw new CustomExceptions.NoDataFoundException("Group not found");
            }
            group.Name = request.Name ?? group.Name;
            group.Description = request.Description ?? group.Description;
            await _unitOfWork.GetRepository<Group>().UpdateAsync(group);
            await _unitOfWork.SaveChangeAsync();
            var response = await ToGroupResponse(group);
            return response;
        }

        public async Task<GroupResponse> DeleteAsync(Guid id)
        {
            var group = await _unitOfWork.GetRepository<Group>().GetByIdAsync(id);
            if (group == null)
            {
                _logger.LogError("Group not found with id: {Id}", id);
                throw new CustomExceptions.NoDataFoundException("Group not found");
            }
            await _unitOfWork.GetRepository<Group>().SoftDeleteAsync(id);
            await _unitOfWork.SaveChangeAsync();
            return await ToGroupResponse(group);
        }


        // Member Management
        public async Task<GroupResponse> AddMembersToGroupAsync(Guid groupId, List<Guid> userIds)
        {
            try
            {
                _logger.LogInformation("Starting to add members to group {GroupId}", groupId);

                // Validate input
                if (userIds == null || !userIds.Any())
                {
                    _logger.LogWarning("No user IDs provided for group {GroupId}", groupId);
                    throw new CustomExceptions.ValidationException("User IDs cannot be empty");
                }

                if (userIds.Any(id => id == Guid.Empty))
                {
                    _logger.LogWarning("Invalid user ID provided for group {GroupId}", groupId);
                    throw new CustomExceptions.ValidationException("Invalid user ID provided");
                }

                // Check if group exists
                var group = await _unitOfWork.GetRepository<Group>().GetByIdAsync(groupId);
                if (group == null)
                {
                    _logger.LogError("Group not found with id: {GroupId}", groupId);
                    throw new CustomExceptions.NoDataFoundException("Group not found");
                }

                _logger.LogInformation("Group found: {GroupName} in class {ClassId}", group.Name, group.ClassId);

                var addedMembers = new List<GroupMemberResponse>();
                var updatedEnrollments = new List<ClassEnrollment>();

                foreach (var userId in userIds)
                {
                    _logger.LogInformation("Processing user {UserId} for group {GroupId}", userId, groupId);

                    // Validate user exists
                    var user = await _unitOfWork.GetRepository<Account>().GetByIdAsync(userId);
                    if (user == null)
                    {
                        _logger.LogError("User not found with id: {UserId}", userId);
                        throw new CustomExceptions.NoDataFoundException($"User not found with id: {userId}");
                    }

                    _logger.LogInformation("User found: {UserEmail}", user.Email);

                    // ✅ Check if user is enrolled in the class (required)
                    var classEnrollment = await _unitOfWork.GetRepository<ClassEnrollment>()
                        .GetAllAsync(x => x.UserId == userId && x.ClassId == group.ClassId && x.Status == ClassEnrollmentStatusEnum.Active);

                    if (!classEnrollment.Any())
                    {
                        _logger.LogError("User {UserId} is not enrolled in class {ClassId}", userId, group.ClassId);
                        throw new CustomExceptions.ValidationException($"User {userId} is not enrolled in this class. Please add user to class first.");
                    }

                    var enrollment = classEnrollment.First();
                    _logger.LogInformation("User {UserId} is enrolled in class {ClassId}", userId, group.ClassId);

                    // Check if user is already in this group
                    if (enrollment.GroupId == groupId)
                    {
                        _logger.LogInformation("User {UserId} is already in group {GroupId}", userId, groupId);
                        updatedEnrollments.Add(enrollment);
                        continue;
                    }

                    // Check if user is in another group
                    if (enrollment.GroupId.HasValue && enrollment.GroupId != groupId)
                    {
                        _logger.LogWarning("User {UserId} is already in another group {OtherGroupId}", userId, enrollment.GroupId);
                        throw new CustomExceptions.ValidationException($"User {userId} is already in another group. Remove from current group first.");
                    }

                    // Update enrollment to assign to this group
                    enrollment.GroupId = groupId;
                    await _unitOfWork.GetRepository<ClassEnrollment>().UpdateAsync(enrollment);
                    updatedEnrollments.Add(enrollment);

                    addedMembers.Add(new GroupMemberResponse
                    {
                        UserId = user.Id,
                        FirstName = user.Firstname ?? "",
                        LastName = user.Lastname ?? "",
                        Email = user.Email ?? "",
                        StudentId = user.StudentID ?? "",
                        RoleInClass = enrollment.RoleInClass,
                        Status = enrollment.Status.ToString(),
                    });

                    _logger.LogInformation("Successfully assigned user {UserId} to group {GroupId}", userId, groupId);
                }

                // Save changes
                _logger.LogInformation("Saving changes to database");
                await _unitOfWork.SaveChangeAsync();
                _logger.LogInformation("Successfully saved changes");

                _logger.LogInformation("Successfully added {Count} members to group {GroupId}", addedMembers.Count, groupId);
                return await ToGroupDetailResponse(group);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AddMembersToGroupAsync for group {GroupId}: {Message}", groupId, ex.Message);
                throw;
            }
        }

        public async Task<GroupResponse> RemoveMembersFromGroupAsync(Guid groupId, List<Guid> userIds)
        {
            try
            {
                _logger.LogInformation("Starting to remove members from group {GroupId}", groupId);

                // Validate input
                if (userIds == null || !userIds.Any())
                {
                    _logger.LogWarning("No user IDs provided for group {GroupId}", groupId);
                    throw new CustomExceptions.ValidationException("User IDs cannot be empty");
                }

                // Check if group exists
                var group = await _unitOfWork.GetRepository<Group>().GetByIdAsync(groupId);
                if (group == null)
                {
                    _logger.LogError("Group not found with id: {GroupId}", groupId);
                    throw new CustomExceptions.NoDataFoundException("Group not found");
                }

                _logger.LogInformation("Group found: {GroupName}", group.Name);

                foreach (var userId in userIds)
                {
                    _logger.LogInformation("Processing user {UserId} for removal from group {GroupId}", userId, groupId);

                    // Find enrollment for this user in this group
                    var enrollment = await _unitOfWork.GetRepository<ClassEnrollment>()
                        .GetAllAsync(x => x.UserId == userId && x.GroupId == groupId && x.Status == ClassEnrollmentStatusEnum.Active);

                    if (!enrollment.Any())
                    {
                        _logger.LogWarning("User {UserId} is not in group {GroupId}", userId, groupId);
                        continue; // Skip if not in group
                    }

                    var userEnrollment = enrollment.First();

                    // Remove from group (set GroupId to null)
                    userEnrollment.GroupId = null;
                    await _unitOfWork.GetRepository<ClassEnrollment>().UpdateAsync(userEnrollment);

                    _logger.LogInformation("Successfully removed user {UserId} from group {GroupId}", userId, groupId);
                }

                // Save changes
                _logger.LogInformation("Saving changes to database");
                await _unitOfWork.SaveChangeAsync();
                _logger.LogInformation("Successfully saved changes");

                _logger.LogInformation("Successfully removed members from group {GroupId}", groupId);
                return await ToGroupResponse(group);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in RemoveMembersFromGroupAsync for group {GroupId}: {Message}", groupId, ex.Message);
                throw;
            }
        }

        public async Task<List<GroupMemberResponse>> GetAllMembersOfGroupAsync(Guid groupId)
        {
            try
            {
                _logger.LogInformation("Getting all members of group {GroupId}", groupId);

                // Check if group exists
                var group = await _unitOfWork.GetRepository<Group>().GetByIdAsync(groupId);
                if (group == null)
                {
                    _logger.LogError("Group not found with id: {GroupId}", groupId);
                    throw new CustomExceptions.NoDataFoundException("Group not found");
                }

                _logger.LogInformation("Group found: {GroupName} in class {ClassId}", group.Name, group.ClassId);

                // Get all active enrollments for this group
                var enrollments = await _unitOfWork.GetRepository<ClassEnrollment>()
                    .GetAllAsync(x => x.GroupId == groupId && x.Status == ClassEnrollmentStatusEnum.Active);

                _logger.LogInformation("Found {Count} members in group {GroupId}", enrollments.Count, groupId);

                if (!enrollments.Any())
                {
                    _logger.LogInformation("No members found in group {GroupId}", groupId);
                    return new List<GroupMemberResponse>();
                }

                return await ToGroupMemberResponse(enrollments.ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAllMembersOfGroupAsync for group {GroupId}: {Message}", groupId, ex.Message);
                throw;
            }
        }

        private async Task<GroupResponse> ToGroupResponse(Group group)
        {
            var response = new GroupResponse
            {
                Id = group.Id,
                ClassId = group.ClassId,
                Name = group.Name,
                Description = group.Description,
                CreatedAt = group.CreatedAt
            };
            return await Task.FromResult(response);
        }

        private async Task<GroupDetailResponse> ToGroupDetailResponse(Group group)
        {
            var account = await _unitOfWork.GetRepository<Account>().GetByIdAsync(group.Enrollments.FirstOrDefault()?.UserId);
            var response = new GroupDetailResponse
            {
                Id = group.Id,
                ClassId = group.ClassId,
                Name = group.Name,
                Description = group.Description,
                CreatedAt = group.CreatedAt,
                ClassCode = group.Class.Code,
                GroupMembers = group.Enrollments.Select(e => new GroupMemberResponse
                {
                    UserId = e.UserId,
                    FirstName = account.Firstname,
                    LastName = account.Lastname,
                    Email = account.Email,
                    StudentId = account.StudentID,
                    RoleInClass = e.RoleInClass,
                    Status = e.Status.ToString()
                }).ToList() ?? new List<GroupMemberResponse>()
            };
            return await Task.FromResult(response);
        }

        private async Task<List<GroupMemberResponse>> ToGroupMemberResponse(List<ClassEnrollment> enrollments)
        {
            if (!enrollments.Any())
            {
                return new List<GroupMemberResponse>();
            }

            // Get all user IDs from enrollments
            var userIds = enrollments.Select(e => e.UserId).ToList();

            // Get all accounts in one query (more efficient)
            var accounts = await _unitOfWork.GetRepository<Account>()
                .GetAllAsync(x => userIds.Contains(x.Id));

            // Create response mapping
            var responses = enrollments.Select(enrollment =>
            {
                var account = accounts.FirstOrDefault(a => a.Id == enrollment.UserId);
                return new GroupMemberResponse
                {
                    UserId = enrollment.UserId,
                    FirstName = account?.Firstname ?? "",
                    LastName = account?.Lastname ?? "",
                    Email = account?.Email ?? "",
                    StudentId = account?.StudentID ?? "",
                    RoleInClass = enrollment.RoleInClass,
                    Status = enrollment.Status.ToString(),
                };
            }).ToList();

            _logger.LogInformation("Successfully mapped {Count} group members", responses.Count);
            return responses;
        }
    }
}