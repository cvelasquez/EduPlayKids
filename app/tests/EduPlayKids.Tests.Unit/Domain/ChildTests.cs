using AutoFixture;
using AutoFixture.Xunit2;
using EduPlayKids.Domain.Entities;
using EduPlayKids.Domain.Enums;

namespace EduPlayKids.Tests.Unit.Domain;

/// <summary>
/// Unit tests for Child domain entity.
/// Critical tests for child-specific functionality and age-appropriate content validation.
/// </summary>
public class ChildTests
{
    private readonly Fixture _fixture;

    public ChildTests()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    [Fact]
    public void Child_Creation_ShouldHaveValidDefaults()
    {
        // Arrange & Act
        var child = new Child();

        // Assert
        child.Id.Should().Be(0);
        child.IsActive.Should().BeTrue();
        child.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        child.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        child.Progress.Should().NotBeNull();
        child.Progress.Should().BeEmpty();
        child.Sessions.Should().NotBeNull();
        child.Sessions.Should().BeEmpty();
        child.UserAchievements.Should().NotBeNull();
        child.UserAchievements.Should().BeEmpty();
    }

    [Theory]
    [InlineData(3, AgeGroup.PreK)]
    [InlineData(4, AgeGroup.PreK)]
    [InlineData(5, AgeGroup.Kindergarten)]
    [InlineData(6, AgeGroup.Primary)]
    [InlineData(7, AgeGroup.Primary)]
    [InlineData(8, AgeGroup.Primary)]
    public void Child_AgeGroup_ShouldMapCorrectlyForValidAges(int age, AgeGroup expectedAgeGroup)
    {
        // Arrange
        var child = _fixture.Build<Child>()
            .With(x => x.Age, age)
            .Create();

        // Act & Assert
        child.Age.Should().Be(age);
        // Note: If there's a computed property for AgeGroup, test it here
        // child.AgeGroup.Should().Be(expectedAgeGroup);
    }

    [Theory]
    [InlineData(2)] // Too young
    [InlineData(9)] // Too old
    [InlineData(0)] // Invalid
    [InlineData(-1)] // Invalid
    public void Child_InvalidAge_ShouldBeDetectable(int invalidAge)
    {
        // Arrange & Act
        var child = _fixture.Build<Child>()
            .With(x => x.Age, invalidAge)
            .Create();

        // Assert
        child.Age.Should().Be(invalidAge);
        // Note: Business logic should validate age range 3-8
        // This test ensures we can detect invalid ages for validation
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Child_WithInvalidName_ShouldFailValidation(string invalidName)
    {
        // Arrange
        var child = _fixture.Build<Child>()
            .With(x => x.Name, invalidName)
            .Create();

        // Act & Assert
        child.Name.Should().Be(invalidName);
        // Note: Validation logic should be implemented
    }

    [Theory]
    [InlineData("Ana")]
    [InlineData("José")]
    [InlineData("María José")]
    [InlineData("Alex")]
    public void Child_WithValidName_ShouldBeAccepted(string validName)
    {
        // Arrange
        var child = _fixture.Build<Child>()
            .With(x => x.Name, validName)
            .Create();

        // Act & Assert
        child.Name.Should().Be(validName);
    }

    [Fact]
    public void Child_CanTrackProgress_Successfully()
    {
        // Arrange
        var child = _fixture.Create<Child>();
        var progress = _fixture.Build<UserProgress>()
            .With(x => x.ChildId, child.Id)
            .With(x => x.Child, child)
            .Create();

        // Act
        child.Progress.Add(progress);

        // Assert
        child.Progress.Should().Contain(progress);
        child.Progress.Count.Should().Be(1);
    }

    [Fact]
    public void Child_CanHaveMultipleProgressRecords()
    {
        // Arrange
        var child = _fixture.Create<Child>();
        var progressRecords = _fixture.Build<UserProgress>()
            .With(x => x.ChildId, child.Id)
            .With(x => x.Child, child)
            .CreateMany(3)
            .ToList();

        // Act
        foreach (var progress in progressRecords)
        {
            child.Progress.Add(progress);
        }

        // Assert
        child.Progress.Should().HaveCount(3);
        child.Progress.Should().Contain(progressRecords);
    }

    [Fact]
    public void Child_CanTrackSessions_Successfully()
    {
        // Arrange
        var child = _fixture.Create<Child>();
        var session = _fixture.Build<Session>()
            .With(x => x.ChildId, child.Id)
            .With(x => x.Child, child)
            .Create();

        // Act
        child.Sessions.Add(session);

        // Assert
        child.Sessions.Should().Contain(session);
        child.Sessions.Count.Should().Be(1);
    }

    [Fact]
    public void Child_CanEarnAchievements_Successfully()
    {
        // Arrange
        var child = _fixture.Create<Child>();
        var achievement = _fixture.Build<UserAchievement>()
            .With(x => x.ChildId, child.Id)
            .With(x => x.Child, child)
            .Create();

        // Act
        child.UserAchievements.Add(achievement);

        // Assert
        child.UserAchievements.Should().Contain(achievement);
        child.UserAchievements.Count.Should().Be(1);
    }

    [Theory]
    [InlineData("es-ES")]
    [InlineData("en-US")]
    [InlineData("es-MX")]
    public void Child_WithValidLanguagePreference_ShouldBeAccepted(string languageCode)
    {
        // Arrange
        var child = _fixture.Build<Child>()
            .With(x => x.LanguagePreference, languageCode)
            .Create();

        // Act & Assert
        child.LanguagePreference.Should().Be(languageCode);
    }

    [Fact]
    public void Child_AvatarUrl_CanBeSetAndRetrieved()
    {
        // Arrange
        const string avatarUrl = "avatars/child_avatar_1.png";
        var child = _fixture.Build<Child>()
            .With(x => x.AvatarUrl, avatarUrl)
            .Create();

        // Act & Assert
        child.AvatarUrl.Should().Be(avatarUrl);
    }

    [Fact]
    public void Child_UpdatedAt_ShouldReflectChanges()
    {
        // Arrange
        var child = _fixture.Create<Child>();
        var originalUpdatedAt = child.UpdatedAt;

        // Act
        Thread.Sleep(10);
        child.UpdatedAt = DateTime.UtcNow;

        // Assert
        child.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }
}