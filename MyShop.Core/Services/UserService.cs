using AutoMapper;
using BCrypt.Net;
using MyShop.Core.DTOs;
using MyShop.Core.Entities;
using MyShop.Core.Extensions;
using MyShop.Core.Repositories;

namespace MyShop.Core.Services
{
    public class UserService : IUserService
    {
        private readonly IRepository<User> _repository;
        private readonly IMapper _mapper;

        public UserService(IRepository<User> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<UserDto> GetUserByIdAsync(Guid id)
        {
            var user = await _repository.GetByIdAsync(id);
            return user != null ? _mapper.Map<UserDto>(user) : null;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }

        public async Task<PaginatedResult<UserDto>> GetUsersPaginatedAsync(int pageNumber, int pageSize)
        {
            var users = await _repository.GetAllAsync();
            var userDtos = _mapper.Map<IEnumerable<UserDto>>(users);
            return userDtos.ToPaginated(pageNumber, pageSize);
        }

        public async Task<UserDto> CreateUserAsync(CreateUserDto dto)
        {
            var user = _mapper.Map<User>(dto);
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            user.Id = Guid.NewGuid();
            
            var createdUser = await _repository.AddAsync(user);
            return _mapper.Map<UserDto>(createdUser);
        }

        public async Task<UserDto> UpdateUserAsync(Guid id, UpdateUserDto dto)
        {
            var user = await _repository.GetByIdAsync(id);
            if (user == null)
                return null;

            _mapper.Map(dto, user);
            var updatedUser = await _repository.UpdateAsync(user);
            return _mapper.Map<UserDto>(updatedUser);
        }

        public async Task<bool> DeleteUserAsync(Guid id)
        {
            return await _repository.DeleteAsync(id);
        }

        public async Task<UserDto> GetUserByUsernameAsync(string username)
        {
            var users = await _repository.GetAllAsync();
            var user = users.FirstOrDefault(u => u.Username == username);
            return user != null ? _mapper.Map<UserDto>(user) : null;
        }

        public async Task<User> GetUserEntityByUsernameAsync(string username)
        {
            var users = await _repository.GetAllAsync();
            return users.FirstOrDefault(u => u.Username == username);
        }
    }
}
