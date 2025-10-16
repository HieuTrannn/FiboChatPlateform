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

        public async Task<GroupResponse> AddMemberAsync(Guid groupId, GroupMemberRequest request)
        {
            var group = await _unitOfWork.GetRepository<Group>().GetByIdAsync(groupId);
            if (group == null)
            {
                _logger.LogError("Group not found with id: {Id}", groupId);
                throw new CustomExceptions.NoDataFoundException("Group not found");
            }
            var enrollment = new ClassEnrollment
            {
                UserId = request.UserId,
                GroupId = groupId,
                RoleInClass = request.RoleInClass,
                Status = ClassEnrollmentStatusEnum.Active
            };
            await _unitOfWork.GetRepository<ClassEnrollment>().InsertAsync(enrollment);
            await _unitOfWork.SaveChangeAsync();
            return await ToGroupResponse(group);
        }

        public async Task<GroupResponse> RemoveMemberAsync(Guid groupId, Guid userId)
        {
            var group = await _unitOfWork.GetRepository<Group>().GetByIdAsync(groupId);
            if (group == null)
            {
                _logger.LogError("Group not found with id: {Id}", groupId);
                throw new CustomExceptions.NoDataFoundException("Group not found");
            }
            var enrollment = group.Enrollments.ToList().FirstOrDefault(x => x.UserId == userId);
            if (enrollment == null)
            {
                _logger.LogError("Enrollment not found with user id: {UserId}", userId);
                throw new CustomExceptions.NoDataFoundException("Enrollment not found");
            }
            enrollment.Status = ClassEnrollmentStatusEnum.Disabled;
            await _unitOfWork.GetRepository<ClassEnrollment>().UpdateAsync(enrollment);
            await _unitOfWork.SaveChangeAsync();
            return await ToGroupResponse(group);
        }
        public async Task<List<GroupMemberResponse>> GetMembersAsync(Guid groupId)
        {
            var group = await _unitOfWork.GetRepository<Group>().GetByIdAsync(groupId);
            if (group == null)
            {
                _logger.LogError("Group not found with id: {Id}", groupId);
                throw new CustomExceptions.NoDataFoundException("Group not found");
            }
            return await ToGroupMemberResponse(group.Enrollments.ToList());
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
            var accounts = await _unitOfWork.GetRepository<Account>().GetAllAsync(x => x.Where(x => enrollments.Select(e => e.UserId).Contains(x.Id)));
            var responses = enrollments.Select(e => new GroupMemberResponse
            {
                UserId = e.UserId,
                FirstName = accounts.FirstOrDefault(x => x.Id == e.UserId)?.Firstname ?? "",
                LastName = accounts.FirstOrDefault(x => x.Id == e.UserId)?.Lastname ?? "",
                Email = accounts.FirstOrDefault(x => x.Id == e.UserId)?.Email ?? "",
                StudentId = accounts.FirstOrDefault(x => x.Id == e.UserId)?.StudentID ?? "",
                RoleInClass = e.RoleInClass,
                Status = e.Status.ToString()
            }).ToList();
            return await Task.FromResult(responses);
        }
    }
}