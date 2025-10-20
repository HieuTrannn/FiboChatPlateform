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

        /// <summary>
        /// Get all groups
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAllGroups(int page = 1, int pageSize = 10)
        {
            try
            {
                var groups = await _groupService.GetAllAsync(page, pageSize);
                return Ok(ApiResponse<BasePaginatedList<GroupResponse>>.Ok(groups, "Get all groups successfully", "200"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error at the {Controller}: {Message}", nameof(GroupController), ex.Message);
                return StatusCode(500, ApiResponse<string>.InternalError($"Error at the {nameof(GroupController)}: {ex.Message}"));
            }
        }

        /// <summary>
        /// Get all groups by class id
        /// </summary>
        /// <param name="classId"></param>
        /// <returns></returns>
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
        
        /// <summary>
        /// Get a group by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Create a new group
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateGroup([FromForm] GroupCreateRequest request)
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

        /// <summary>
        /// Update a group
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGroup(Guid id, [FromForm] GroupUpdateRequest request)
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

        /// <summary>
        /// Delete a group
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGroup([FromRoute] Guid id)
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

        /// <summary>
        /// Add a member to a group
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("{groupId}/members")]
        public async Task<IActionResult> AddMemberToGroup(Guid groupId, [FromForm] GroupMemberRequest request)
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

        /// <summary>
        /// Remove a member from a group
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpDelete("{groupId}/members/{userId}")]
        public async Task<IActionResult> RemoveMemberFromGroup([FromRoute] Guid groupId, [FromRoute] Guid userId)
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

        /// <summary>
        /// Get all members of a group
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
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