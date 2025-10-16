using Authentication.Application.Interfaces;
using Authentication.Application.DTOs.GroupDTOs;
using Contracts.Common;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/groups")]
    public class GroupController : ControllerBase
    {
        private readonly IGroupService _groupService;
        private readonly ILogger<GroupController> _logger;

        public GroupController(IGroupService groupService, ILogger<GroupController> logger)
        {
            _groupService = groupService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllGroups()
        {
            try
            {
                var groups = await _groupService.GetAllAsync(1, 10);
                return Ok(ApiResponse<BasePaginatedList<GroupResponse>>.Ok(groups, "Get all groups successfully", "200"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error at the {Controller}: {Message}", nameof(GroupController), ex.Message);
                return StatusCode(500, ApiResponse<string>.InternalError($"Error at the {nameof(GroupController)}: {ex.Message}"));
            }
        }

        [HttpGet("class/{classId}")]
        public async Task<IActionResult> GetAllGroupsByClassId(Guid classId)
        {
            try
            {
                var groups = await _groupService.GetAllByClassIdAsync(classId, 1, 10);
                return Ok(ApiResponse<BasePaginatedList<GroupResponse>>.Ok(groups, "Get all groups by class id successfully", "200"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error at the {Controller}: {Message}", nameof(GroupController), ex.Message);
                return StatusCode(500, ApiResponse<string>.InternalError($"Error at the {nameof(GroupController)}: {ex.Message}"));
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetGroupById(Guid id)
        {
            try
            {
                var group = await _groupService.GetByIdAsync(id);
                return Ok(ApiResponse<GroupResponse>.Ok(group, "Get group by id successfully", "200"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error at the {Controller}: {Message}", nameof(GroupController), ex.Message);
                return StatusCode(500, ApiResponse<string>.InternalError($"Error at the {nameof(GroupController)}: {ex.Message}"));
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateGroup(GroupCreateRequest request)
        {
            try
            {
                var group = await _groupService.CreateAsync(request);
                return Ok(ApiResponse<GroupResponse>.CreateResponse("Create group successfully"));
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error at the {Controller}: {Message}", nameof(GroupController), ex.Message);
                return StatusCode(500, ApiResponse<string>.InternalError($"Error at the {nameof(GroupController)}: {ex.Message}"));
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGroup(Guid id, GroupUpdateRequest request)
        {
            try
            {
                var group = await _groupService.UpdateAsync(id, request);
                return Ok(ApiResponse<GroupResponse>.Ok(group, "Update group successfully", "200"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error at the {Controller}: {Message}", nameof(GroupController), ex.Message);
                return StatusCode(500, ApiResponse<string>.InternalError($"Error at the {nameof(GroupController)}: {ex.Message}"));
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGroup(Guid id)
        {
            try
            {
                var group = await _groupService.DeleteAsync(id);
                return Ok(ApiResponse<GroupResponse>.Ok(group, "Delete group successfully", "200"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error at the {Controller}: {Message}", nameof(GroupController), ex.Message);
                return StatusCode(500, ApiResponse<string>.InternalError($"Error at the {nameof(GroupController)}: {ex.Message}"));
            }
        }

        [HttpPost("{groupId}/members")]
        public async Task<IActionResult> AddMemberToGroup(Guid groupId, GroupMemberRequest request)
        {
            try
            {
                var group = await _groupService.AddMemberAsync(groupId, request);
                return Ok(ApiResponse<GroupResponse>.Ok(group, "Add member to group successfully", "200"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error at the {Controller}: {Message}", nameof(GroupController), ex.Message);
                return StatusCode(500, ApiResponse<string>.InternalError($"Error at the {nameof(GroupController)}: {ex.Message}"));
            }
        }

        [HttpDelete("{groupId}/members/{userId}")]
        public async Task<IActionResult> RemoveMemberFromGroup(Guid groupId, Guid userId)
        {
            try
            {
                var group = await _groupService.RemoveMemberAsync(groupId, userId);
                return Ok(ApiResponse<GroupResponse>.Ok(group, "Remove member from group successfully", "200"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error at the {Controller}: {Message}", nameof(GroupController), ex.Message);
                return StatusCode(500, ApiResponse<string>.InternalError($"Error at the {nameof(GroupController)}: {ex.Message}"));
            }
        }

        [HttpGet("{groupId}/members")]
        public async Task<IActionResult> GetMembersOfGroup(Guid groupId)
        {
            try
            {
                var members = await _groupService.GetMembersAsync(groupId);
                return Ok(ApiResponse<List<GroupMemberResponse>>.Ok(members, "Get members of group successfully", "200"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error at the {Controller}: {Message}", nameof(GroupController), ex.Message);
                return StatusCode(500, ApiResponse<string>.InternalError($"Error at the {nameof(GroupController)}: {ex.Message}"));
            }
        }
    }
}