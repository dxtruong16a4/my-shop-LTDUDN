using AutoMapper;
using MyShop.Core.DTOs;
using MyShop.Core.Entities;
using MyShop.Core.Extensions;
using MyShop.Core.Repositories;

namespace MyShop.Core.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IRepository<Category> _repository;
        private readonly IMapper _mapper;

        public CategoryService(IRepository<Category> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<CategoryDto> GetCategoryByIdAsync(int id)
        {
            var category = await _repository.GetByIdAsync(id);
            return category != null ? _mapper.Map<CategoryDto>(category) : null;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<CategoryDto>>(categories);
        }

        public async Task<PaginatedResult<CategoryDto>> GetCategoriesPaginatedAsync(int pageNumber, int pageSize)
        {
            var categories = await _repository.GetAllAsync();
            var categoryDtos = _mapper.Map<IEnumerable<CategoryDto>>(categories);
            return categoryDtos.ToPaginated(pageNumber, pageSize);
        }

        public async Task<IEnumerable<CategoryDto>> GetRootCategoriesAsync()
        {
            var categories = await _repository.GetAllAsync();
            var roots = categories.Where(c => c.ParentId == null).ToList();
            return _mapper.Map<IEnumerable<CategoryDto>>(roots);
        }

        public async Task<IEnumerable<CategoryDto>> GetSubCategoriesAsync(int parentId)
        {
            var categories = await _repository.GetAllAsync();
            var subs = categories.Where(c => c.ParentId == parentId).ToList();
            return _mapper.Map<IEnumerable<CategoryDto>>(subs);
        }

        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto dto)
        {
            var category = _mapper.Map<Category>(dto);
            var createdCategory = await _repository.AddAsync(category);
            return _mapper.Map<CategoryDto>(createdCategory);
        }

        public async Task<CategoryDto> UpdateCategoryAsync(int id, UpdateCategoryDto dto)
        {
            var category = await _repository.GetByIdAsync(id);
            if (category == null)
                return null;

            _mapper.Map(dto, category);
            var updatedCategory = await _repository.UpdateAsync(category);
            return _mapper.Map<CategoryDto>(updatedCategory);
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }
    }
}
