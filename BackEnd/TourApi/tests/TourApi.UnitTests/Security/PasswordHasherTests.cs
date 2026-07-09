using TourApi.Services;
using Xunit;

namespace TourApi.UnitTests.Security;

public sealed class PasswordHasherTests
{
    [Fact]
    public void Hash_ShouldReturnExpectedBinaryLength()
    {
        var hasher = new PasswordHasher();

        var hash = hasher.Hash("Test123!");

        Assert.Equal(64, hash.Length);
    }

    [Fact]
    public void Verify_WithCorrectPassword_ShouldReturnTrue()
    {
        var hasher = new PasswordHasher();
        var hash = hasher.Hash("Test123!");

        var valid = hasher.Verify("Test123!", hash);

        Assert.True(valid);
    }

    [Fact]
    public void Verify_WithWrongPassword_ShouldReturnFalse()
    {
        var hasher = new PasswordHasher();
        var hash = hasher.Hash("Test123!");

        var valid = hasher.Verify("WrongPassword", hash);

        Assert.False(valid);
    }

    [Fact]
    public void Verify_WithInvalidStoredHashLength_ShouldReturnFalse()
    {
        var hasher = new PasswordHasher();

        var valid = hasher.Verify("Test123!", new byte[10]);

        Assert.False(valid);
    }
}
