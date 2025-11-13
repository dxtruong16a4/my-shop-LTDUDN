using MyShop.Core.DTOs;
using MyShop.Core.Entities;

namespace MyShop.Core.Services
{
    public interface IUserService
    {
        Task<UserDto> GetUserByIdAsync(Guid id);
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<PaginatedResult<UserDto>> GetUsersPaginatedAsync(int pageNumber, int pageSize);
        Task<UserDto> CreateUserAsync(CreateUserDto dto);
        Task<UserDto> UpdateUserAsync(Guid id, UpdateUserDto dto);
        Task<bool> DeleteUserAsync(Guid id);
        Task<UserDto> GetUserByUsernameAsync(string username);
        Task<User> GetUserEntityByUsernameAsync(string username);
    }
}
