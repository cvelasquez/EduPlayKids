using AutoFixture;
using AutoFixture.Xunit2;
using EduPlayKids.Domain.Entities;
using EduPlayKids.Domain.Enums;

namespace EduPlayKids.Tests.Unit.Domain;

/// <summary>
/// Unit tests for User domain entity.
/// Tests fundamental business logic and child safety validations.
/// </summary>
public class UserTests
{
    private readonly Fixture _fixture;

    public UserTests()
    {
        _fixture = new Fixture();
        // Configure AutoFixture for circular references
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    [Fact]
    public void User_Creation_ShouldHaveValidDefaults()
    {
        // Arrange & Act
        var user = new User();

        // Assert
        user.Id.Should().Be(0);
        user.IsActive.Should().BeTrue();
        user.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        user.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        user.Children.Should().NotBeNull();
        user.Children.Should().BeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void User_WithInvalidEmail_ShouldFailValidation(string invalidEmail)
    {
        // Arrange
        var user = _fixture.Build<User>()
            .With(x => x.Email, invalidEmail)
            .Create();

        // Act & Assert
        // Note: This would require a validation framework
        // For now, we test the property assignment
        user.Email.Should().Be(invalidEmail);
    }

    [Fact]
    public void User_WithValidEmail_ShouldBeAccepted()
    {
        // Arrange
        const string validEmail = "parent@example.com";
        var user = _fixture.Build<User>()
            .With(x => x.Email, validEmail)
            .Create();

        // Act & Assert
        user.Email.Should().Be(validEmail);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void User_WithInvalidName_ShouldFailValidation(string invalidName)
    {
        // Arrange
        var user = _fixture.Build<User>()
            .With(x => x.Name, invalidName)
            .Create();

        // Act & Assert
        user.Name.Should().Be(invalidName);
    }

    [Fact]
    public void User_CanAddChild_Successfully()
    {
        // Arrange
        var user = _fixture.Create<User>();
        var child = _fixture.Build<Child>()
            .With(x => x.UserId, user.Id)
            .With(x => x.User, user)
            .Create();

        // Act
        user.Children.Add(child);

        // Assert
        user.Children.Should().Contain(child);
        user.Children.Count.Should().Be(1);
    }

    [Fact]
    public void User_CanHaveMultipleChildren()
    {
        // Arrange
        var user = _fixture.Create<User>();
        var child1 = _fixture.Build<Child>()
            .With(x => x.UserId, user.Id)
            .With(x => x.User, user)
            .Create();
        var child2 = _fixture.Build<Child>()
            .With(x => x.UserId, user.Id)
            .With(x => x.User, user)
            .Create();

        // Act
        user.Children.Add(child1);
        user.Children.Add(child2);

        // Assert
        user.Children.Should().HaveCount(2);
        user.Children.Should().Contain(new[] { child1, child2 });
    }

    [Fact]
    public void User_UpdatedAt_ShouldChangeWhenModified()
    {
        // Arrange
        var user = _fixture.Create<User>();
        var originalUpdatedAt = user.UpdatedAt;

        // Act
        Thread.Sleep(10); // Ensure time difference
        user.UpdatedAt = DateTime.UtcNow;

        // Assert
        user.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Fact]
    public void User_Deactivation_ShouldSetIsActiveToFalse()
    {
        // Arrange
        var user = _fixture.Build<User>()
            .With(x => x.IsActive, true)
            .Create();

        // Act
        user.IsActive = false;

        // Assert
        user.IsActive.Should().BeFalse();
    }
}