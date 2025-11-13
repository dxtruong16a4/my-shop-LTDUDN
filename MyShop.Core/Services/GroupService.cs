using AutoMapper;
using MyShop.Core.DTOs;
using MyShop.Core.Entities;
using MyShop.Core.Extensions;
using MyShop.Core.Repositories;

namespace MyShop.Core.Services
{
    public class GroupService : IGroupService
    {
        private readonly IRepository<Group> _repository;
        private readonly IMapper _mapper;

        public GroupService(IRepository<Group> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<GroupDto> GetGroupByIdAsync(int id)
        {
            var group = await _repository.GetByIdAsync(id);
            return group != null ? _mapper.Map<GroupDto>(group) : null;
        }

        public async Task<IEnumerable<GroupDto>> GetAllGroupsAsync()
        {
            var groups = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<GroupDto>>(groups);
        }

        public async Task<PaginatedResult<GroupDto>> GetGroupsPaginatedAsync(int pageNumber, int pageSize)
        {
            var groups = await _repository.GetAllAsync();
            var groupDtos = _mapper.Map<IEnumerable<GroupDto>>(groups);
            return groupDtos.ToPaginated(pageNumber, pageSize);
        }

        public async Task<IEnumerable<GroupDto>> GetGroupsByCategoryAsync(int categoryId)
        {
            var groups = await _repository.GetAllAsync();
            var filtered = groups.Where(g => g.CategoryId == categoryId).ToList();
            return _mapper.Map<IEnumerable<GroupDto>>(filtered);
        }

        public async Task<GroupDto> CreateGroupAsync(CreateGroupDto dto)
        {
            var group = _mapper.Map<Group>(dto);
            var createdGroup = await _repository.AddAsync(group);
            return _mapper.Map<GroupDto>(createdGroup);
        }

        public async Task<GroupDto> UpdateGroupAsync(int id, UpdateGroupDto dto)
        {
            var group = await _repository.GetByIdAsync(id);
            if (group == null)
                return null;

            _mapper.Map(dto, group);
            var updatedGroup = await _repository.UpdateAsync(group);
            return _mapper.Map<GroupDto>(updatedGroup);
        }

        public async Task<bool> DeleteGroupAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }
    }
}
