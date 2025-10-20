using Contracts.Common;
using Authentication.Application.DTOs.GroupDTOs;

namespace Authentication.Application.Interfaces
{
    public interface IGroupService
    {
        Task<BasePaginatedList<GroupResponse>> GetAllAsync(int page, int pageSize);
        Task<BasePaginatedList<GroupResponse>> GetAllByClassIdAsync(Guid classId, int page, int pageSize);
        Task<GroupDetailResponse> GetByIdAsync(Guid id);
        Task<GroupResponse> CreateAsync(GroupCreateRequest request);
        Task<GroupResponse> UpdateAsync(Guid id, GroupUpdateRequest request);
        Task<GroupResponse> DeleteAsync(Guid id);

        // Member Management
        Task<GroupResponse> AddMemberAsync(Guid groupId, GroupMemberRequest request);
        Task<GroupResponse> RemoveMemberAsync(Guid groupId, Guid userId);
        Task<List<GroupMemberResponse>> GetMembersAsync(Guid groupId);
    }
}