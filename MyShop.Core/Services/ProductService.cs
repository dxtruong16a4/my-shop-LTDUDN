using AutoMapper;
using MyShop.Core.DTOs;
using MyShop.Core.Entities;
using MyShop.Core.Extensions;
using MyShop.Core.Repositories;

namespace MyShop.Core.Services
{
    public class ProductService : IProductService
    {
        private readonly IRepository<Product> _repository;
        private readonly IMapper _mapper;

        public ProductService(IRepository<Product> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ProductDto> GetProductByIdAsync(int id)
        {
            var product = await _repository.GetByIdAsync(id);
            return product != null ? _mapper.Map<ProductDto>(product) : null;
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            var products = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<PaginatedResult<ProductDto>> GetProductsPaginatedAsync(int pageNumber, int pageSize)
        {
            var products = await _repository.GetAllAsync();
            var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);
            return productDtos.ToPaginated(pageNumber, pageSize);
        }

        public async Task<IEnumerable<ProductDto>> GetProductsByGroupAsync(int groupId)
        {
            var products = await _repository.GetAllAsync();
            var filtered = products.Where(p => p.GroupId == groupId).ToList();
            return _mapper.Map<IEnumerable<ProductDto>>(filtered);
        }

        public async Task<IEnumerable<ProductDto>> SearchProductsAsync(string keyword)
        {
            var products = await _repository.GetAllAsync();
            var searched = products.Where(p => 
                p.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                p.Description.Contains(keyword, StringComparison.OrdinalIgnoreCase)
            ).ToList();
            return _mapper.Map<IEnumerable<ProductDto>>(searched);
        }

        public async Task<PaginatedResult<ProductDto>> SearchProductsAsync(string keyword, int pageNumber, int pageSize)
        {
            var products = await _repository.GetAllAsync();
            var searched = products.Where(p => 
                p.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                p.Description.Contains(keyword, StringComparison.OrdinalIgnoreCase)
            ).ToList();
            var productDtos = _mapper.Map<IEnumerable<ProductDto>>(searched);
            return productDtos.ToPaginated(pageNumber, pageSize);
        }

        public async Task<ProductDto> CreateProductAsync(CreateProductDto dto)
        {
            var product = _mapper.Map<Product>(dto);
            var createdProduct = await _repository.AddAsync(product);
            return _mapper.Map<ProductDto>(createdProduct);
        }

        public async Task<ProductDto> UpdateProductAsync(int id, UpdateProductDto dto)
        {
            var product = await _repository.GetByIdAsync(id);
            if (product == null)
                return null;

            _mapper.Map(dto, product);
            var updatedProduct = await _repository.UpdateAsync(product);
            return _mapper.Map<ProductDto>(updatedProduct);
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }
    }
}
