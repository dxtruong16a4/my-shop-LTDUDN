using Xunit;
using MyShop.Core.Helpers;

namespace MyShop.Tests;

public class PasswordHelperTests
{
    private readonly PasswordHelper _passwordHelper;

    public PasswordHelperTests()
    {
        _passwordHelper = new PasswordHelper();
    }

    [Fact]
    public void HashPassword_ShouldReturnHashedPassword()
    {
        // Arrange
        string password = "SecurePassword@123";

        // Act
        string hashedPassword = _passwordHelper.HashPassword(password);

        // Assert
        Assert.NotNull(hashedPassword);
        Assert.NotEqual(password, hashedPassword);
        Assert.True(hashedPassword.Length > 0);
    }

    [Fact]
    public void HashPassword_ShouldReturnDifferentHashesForSamePassword()
    {
        // Arrange
        string password = "SecurePassword@123";

        // Act
        string hash1 = _passwordHelper.HashPassword(password);
        string hash2 = _passwordHelper.HashPassword(password);

        // Assert
        Assert.NotEqual(hash1, hash2);
    }

    [Fact]
    public void VerifyPassword_WithCorrectPassword_ShouldReturnTrue()
    {
        // Arrange
        string password = "SecurePassword@123";
        string hashedPassword = _passwordHelper.HashPassword(password);

        // Act
        bool result = _passwordHelper.VerifyPassword(password, hashedPassword);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void VerifyPassword_WithIncorrectPassword_ShouldReturnFalse()
    {
        // Arrange
        string correctPassword = "SecurePassword@123";
        string wrongPassword = "WrongPassword@456";
        string hashedPassword = _passwordHelper.HashPassword(correctPassword);

        // Act
        bool result = _passwordHelper.VerifyPassword(wrongPassword, hashedPassword);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void VerifyPassword_WithEmptyPassword_ShouldReturnFalse()
    {
        // Arrange
        string password = "SecurePassword@123";
        string hashedPassword = _passwordHelper.HashPassword(password);

        // Act
        bool result = _passwordHelper.VerifyPassword("", hashedPassword);

        // Assert
        Assert.False(result);
    }
}
