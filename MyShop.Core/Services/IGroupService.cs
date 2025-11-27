using MyShop.Core.DTOs;

namespace MyShop.Core.Services
{
    public interface IGroupService
    {
        Task<GroupDto?> GetGroupByIdAsync(int id);
        Task<IEnumerable<GroupDto>> GetAllGroupsAsync();
        Task<PaginatedResult<GroupDto>> GetGroupsPaginatedAsync(int pageNumber, int pageSize);
        Task<IEnumerable<GroupDto>> GetGroupsByCategoryAsync(int categoryId);
        Task<GroupDto> CreateGroupAsync(CreateGroupDto dto);
        Task<GroupDto> UpdateGroupAsync(int id, UpdateGroupDto dto);
        Task<bool> DeleteGroupAsync(int id);
    }
}
