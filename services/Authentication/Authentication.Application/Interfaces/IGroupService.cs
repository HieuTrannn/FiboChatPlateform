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
        Task<GroupResponse> AddMembersToGroupAsync(Guid groupId, List<Guid> userIds);
        Task<GroupResponse> RemoveMembersFromGroupAsync(Guid groupId, List<Guid> userIds);
        Task<List<GroupMemberResponse>> GetAllMembersOfGroupAsync(Guid groupId);
    }
}