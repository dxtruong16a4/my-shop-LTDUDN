using MyShop.Core.DTOs;

namespace MyShop.Core.Services
{
    public interface IProductService
    {
        Task<ProductDto> GetProductByIdAsync(int id);
        Task<IEnumerable<ProductDto>> GetAllProductsAsync();
        Task<PaginatedResult<ProductDto>> GetProductsPaginatedAsync(int pageNumber, int pageSize);
        Task<IEnumerable<ProductDto>> GetProductsByGroupAsync(int groupId);
        Task<PaginatedResult<ProductDto>> SearchProductsAsync(string keyword, int pageNumber, int pageSize);
        Task<ProductDto> CreateProductAsync(CreateProductDto dto);
        Task<ProductDto> UpdateProductAsync(int id, UpdateProductDto dto);
        Task<bool> DeleteProductAsync(int id);
    }
}
