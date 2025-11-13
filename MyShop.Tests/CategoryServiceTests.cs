using Xunit;
using Moq;
using MyShop.Core.Services;
using MyShop.Core.Entities;
using MyShop.Core.Repositories;
using MyShop.Core.DTOs;
using AutoMapper;

namespace MyShop.Tests;

public class CategoryServiceTests
{
    private readonly Mock<IRepository<Category>> _categoryRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CategoryService _categoryService;

    public CategoryServiceTests()
    {
        _categoryRepositoryMock = new Mock<IRepository<Category>>();
        _mapperMock = new Mock<IMapper>();
        _categoryService = new CategoryService(_categoryRepositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task GetAllCategoriesAsync_ShouldReturnAllCategories()
    {
        // Arrange
        var categoryDtos = new List<CategoryDto>
        {
            new CategoryDto { Id = 1, Name = "Electronics" },
            new CategoryDto { Id = 2, Name = "Fashion" }
        };

        var categories = new List<Category>
        {
            new Category { Id = 1, Name = "Electronics", OrderIndex = 1, ParentId = null },
            new Category { Id = 2, Name = "Fashion", OrderIndex = 2, ParentId = null }
        };

        _categoryRepositoryMock
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(categories);

        _mapperMock
            .Setup(m => m.Map<IEnumerable<CategoryDto>>(It.IsAny<IEnumerable<Category>>()))
            .Returns(categoryDtos);

        // Act
        var result = await _categoryService.GetAllCategoriesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _categoryRepositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetCategoryByIdAsync_WithValidId_ShouldReturnCategory()
    {
        // Arrange
        int categoryId = 1;
        var category = new Category { Id = categoryId, Name = "Electronics", OrderIndex = 1, ParentId = null };
        var categoryDto = new CategoryDto { Id = categoryId, Name = "Electronics" };

        _categoryRepositoryMock
            .Setup(r => r.GetByIdAsync(categoryId))
            .ReturnsAsync(category);

        _mapperMock
            .Setup(m => m.Map<CategoryDto>(It.IsAny<Category>()))
            .Returns(categoryDto);

        // Act
        var result = await _categoryService.GetCategoryByIdAsync(categoryId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(categoryId, result.Id);
        Assert.Equal("Electronics", result.Name);
    }

    [Fact]
    public async Task GetCategoryByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        int invalidId = 999;
        _categoryRepositoryMock
            .Setup(r => r.GetByIdAsync(invalidId))
            .ReturnsAsync(null as Category);

        // Act
        var result = await _categoryService.GetCategoryByIdAsync(invalidId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteCategoryAsync_ShouldDeleteCategory()
    {
        // Arrange
        int categoryId = 1;
        _categoryRepositoryMock
            .Setup(r => r.DeleteAsync(categoryId))
            .ReturnsAsync(true);

        // Act
        var result = await _categoryService.DeleteCategoryAsync(categoryId);

        // Assert
        Assert.True(result);
        _categoryRepositoryMock.Verify(r => r.DeleteAsync(categoryId), Times.Once);
    }

    [Fact]
    public async Task GetRootCategoriesAsync_ShouldReturnOnlyRootCategories()
    {
        // Arrange
        var rootCategories = new List<Category>
        {
            new Category { Id = 1, Name = "Electronics", OrderIndex = 1, ParentId = null },
            new Category { Id = 2, Name = "Fashion", OrderIndex = 2, ParentId = null }
        };

        var categoryDtos = new List<CategoryDto>
        {
            new CategoryDto { Id = 1, Name = "Electronics" },
            new CategoryDto { Id = 2, Name = "Fashion" }
        };

        _categoryRepositoryMock
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(rootCategories);

        _mapperMock
            .Setup(m => m.Map<IEnumerable<CategoryDto>>(It.IsAny<IEnumerable<Category>>()))
            .Returns(categoryDtos);

        // Act
        var result = await _categoryService.GetRootCategoriesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }
}
