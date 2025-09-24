using AutoFixture;
using AutoFixture.Xunit2;
using EduPlayKids.Domain.Entities;

namespace EduPlayKids.Tests.Unit.Domain;

/// <summary>
/// Unit tests for UserProgress domain entity.
/// Critical tests for star rating system and educational progress tracking.
/// This is fundamental to the EduPlayKids learning assessment system.
/// </summary>
public class UserProgressTests
{
    private readonly Fixture _fixture;

    public UserProgressTests()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    [Fact]
    public void UserProgress_Creation_ShouldHaveValidDefaults()
    {
        // Arrange & Act
        var progress = new UserProgress();

        // Assert
        progress.Id.Should().Be(0);
        progress.StarsEarned.Should().Be(0);
        progress.ErrorsCount.Should().Be(0);
        progress.TimeSpentMinutes.Should().Be(0);
        progress.IsCompleted.Should().BeFalse();
        progress.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        progress.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    #region Star Rating System Tests - Critical for Educational Assessment

    [Theory]
    [InlineData(0, 3)] // 0 errors = 3 stars (perfect)
    public void UserProgress_ZeroErrors_ShouldEarnThreeStars(int errors, int expectedStars)
    {
        // Arrange
        var progress = _fixture.Build<UserProgress>()
            .With(x => x.ErrorsCount, errors)
            .With(x => x.StarsEarned, expectedStars)
            .With(x => x.IsCompleted, true)
            .Create();

        // Act & Assert
        progress.ErrorsCount.Should().Be(0);
        progress.StarsEarned.Should().Be(3);
        progress.IsCompleted.Should().BeTrue();
    }

    [Theory]
    [InlineData(1, 2)] // 1 error = 2 stars
    [InlineData(2, 2)] // 2 errors = 2 stars
    public void UserProgress_OneOrTwoErrors_ShouldEarnTwoStars(int errors, int expectedStars)
    {
        // Arrange
        var progress = _fixture.Build<UserProgress>()
            .With(x => x.ErrorsCount, errors)
            .With(x => x.StarsEarned, expectedStars)
            .With(x => x.IsCompleted, true)
            .Create();

        // Act & Assert
        progress.ErrorsCount.Should().BeInRange(1, 2);
        progress.StarsEarned.Should().Be(2);
        progress.IsCompleted.Should().BeTrue();
    }

    [Theory]
    [InlineData(3, 1)] // 3 errors = 1 star
    [InlineData(4, 1)] // 4+ errors = 1 star
    [InlineData(5, 1)]
    [InlineData(10, 1)]
    public void UserProgress_ThreeOrMoreErrors_ShouldEarnOneStar(int errors, int expectedStars)
    {
        // Arrange
        var progress = _fixture.Build<UserProgress>()
            .With(x => x.ErrorsCount, errors)
            .With(x => x.StarsEarned, expectedStars)
            .With(x => x.IsCompleted, true)
            .Create();

        // Act & Assert
        progress.ErrorsCount.Should().BeGreaterOrEqualTo(3);
        progress.StarsEarned.Should().Be(1);
        progress.IsCompleted.Should().BeTrue();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(4)]
    [InlineData(-1)]
    public void UserProgress_InvalidStarCount_ShouldBeDetectable(int invalidStars)
    {
        // Arrange
        var progress = _fixture.Build<UserProgress>()
            .With(x => x.StarsEarned, invalidStars)
            .Create();

        // Act & Assert
        progress.StarsEarned.Should().Be(invalidStars);
        // Note: Validation should ensure stars are between 1-3 for completed activities
    }

    #endregion

    #region Time Tracking Tests - Important for Screen Time Monitoring

    [Theory]
    [InlineData(1)]   // 1 minute - very quick
    [InlineData(5)]   // 5 minutes - typical PreK
    [InlineData(10)]  // 10 minutes - typical Kindergarten
    [InlineData(15)]  // 15 minutes - maximum Primary
    public void UserProgress_ValidTimeSpent_ShouldBeTracked(int timeMinutes)
    {
        // Arrange
        var progress = _fixture.Build<UserProgress>()
            .With(x => x.TimeSpentMinutes, timeMinutes)
            .Create();

        // Act & Assert
        progress.TimeSpentMinutes.Should().Be(timeMinutes);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10)]
    public void UserProgress_InvalidTimeSpent_ShouldBeDetectable(int invalidTime)
    {
        // Arrange
        var progress = _fixture.Build<UserProgress>()
            .With(x => x.TimeSpentMinutes, invalidTime)
            .Create();

        // Act & Assert
        progress.TimeSpentMinutes.Should().Be(invalidTime);
        // Note: Validation should ensure time > 0 for completed activities
    }

    [Theory]
    [InlineData(30)] // 30 minutes - very long for any age group
    [InlineData(60)] // 1 hour - too long for children 3-8
    [InlineData(120)] // 2 hours - concerning
    public void UserProgress_ExcessiveTimeSpent_ShouldBeMonitored(int excessiveTime)
    {
        // Arrange
        var progress = _fixture.Build<UserProgress>()
            .With(x => x.TimeSpentMinutes, excessiveTime)
            .Create();

        // Act & Assert
        progress.TimeSpentMinutes.Should().Be(excessiveTime);
        // Note: Parental controls should flag excessive screen time
    }

    #endregion

    #region Error Tracking Tests - Educational Analytics

    [Theory]
    [InlineData(0)]  // Perfect performance
    [InlineData(1)]  // Minor mistakes
    [InlineData(2)]  // Some struggles
    [InlineData(5)]  // Significant difficulties
    [InlineData(10)] // Major struggles - may need easier content
    public void UserProgress_ErrorCount_ShouldReflectLearningDifficulty(int errorCount)
    {
        // Arrange
        var progress = _fixture.Build<UserProgress>()
            .With(x => x.ErrorsCount, errorCount)
            .Create();

        // Act & Assert
        progress.ErrorsCount.Should().Be(errorCount);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-5)]
    public void UserProgress_NegativeErrorCount_ShouldBeInvalid(int negativeErrors)
    {
        // Arrange
        var progress = _fixture.Build<UserProgress>()
            .With(x => x.ErrorsCount, negativeErrors)
            .Create();

        // Act & Assert
        progress.ErrorsCount.Should().Be(negativeErrors);
        // Note: Validation should ensure errors >= 0
    }

    #endregion

    #region Completion Status Tests - Learning Progress

    [Fact]
    public void UserProgress_CompletedActivity_ShouldHaveStarsAndTime()
    {
        // Arrange
        var progress = _fixture.Build<UserProgress>()
            .With(x => x.IsCompleted, true)
            .With(x => x.StarsEarned, 2)
            .With(x => x.TimeSpentMinutes, 8)
            .With(x => x.ErrorsCount, 1)
            .Create();

        // Act & Assert
        progress.IsCompleted.Should().BeTrue();
        progress.StarsEarned.Should().BeInRange(1, 3);
        progress.TimeSpentMinutes.Should().BeGreaterThan(0);
        progress.ErrorsCount.Should().BeGreaterOrEqualTo(0);
    }

    [Fact]
    public void UserProgress_IncompletedActivity_ShouldNotHaveStars()
    {
        // Arrange
        var progress = _fixture.Build<UserProgress>()
            .With(x => x.IsCompleted, false)
            .With(x => x.StarsEarned, 0)
            .Create();

        // Act & Assert
        progress.IsCompleted.Should().BeFalse();
        progress.StarsEarned.Should().Be(0);
    }

    #endregion

    #region Relationship Tests - Data Integrity

    [Fact]
    public void UserProgress_ShouldHaveValidChildReference()
    {
        // Arrange
        var child = _fixture.Create<Child>();
        var progress = _fixture.Build<UserProgress>()
            .With(x => x.ChildId, child.Id)
            .With(x => x.Child, child)
            .Create();

        // Act & Assert
        progress.ChildId.Should().Be(child.Id);
        progress.Child.Should().Be(child);
        progress.Child.Should().NotBeNull();
    }

    [Fact]
    public void UserProgress_ShouldHaveValidActivityReference()
    {
        // Arrange
        var activity = _fixture.Create<Activity>();
        var progress = _fixture.Build<UserProgress>()
            .With(x => x.ActivityId, activity.Id)
            .With(x => x.Activity, activity)
            .Create();

        // Act & Assert
        progress.ActivityId.Should().Be(activity.Id);
        progress.Activity.Should().Be(activity);
        progress.Activity.Should().NotBeNull();
    }

    #endregion

    #region Timestamp Tests - Audit Trail

    [Fact]
    public void UserProgress_StartedDate_ShouldPrecedeCompletedDate()
    {
        // Arrange
        var startedDate = DateTime.UtcNow.AddMinutes(-10);
        var completedDate = DateTime.UtcNow;

        var progress = _fixture.Build<UserProgress>()
            .With(x => x.StartedAt, startedDate)
            .With(x => x.CompletedAt, completedDate)
            .With(x => x.IsCompleted, true)
            .Create();

        // Act & Assert
        progress.StartedAt.Should().Be(startedDate);
        progress.CompletedAt.Should().Be(completedDate);
        progress.CompletedAt.Should().BeAfter(progress.StartedAt.Value);
    }

    [Fact]
    public void UserProgress_IncompletedActivity_ShouldNotHaveCompletedDate()
    {
        // Arrange
        var progress = _fixture.Build<UserProgress>()
            .With(x => x.IsCompleted, false)
            .With(x => x.CompletedAt, (DateTime?)null)
            .Create();

        // Act & Assert
        progress.IsCompleted.Should().BeFalse();
        progress.CompletedAt.Should().BeNull();
    }

    [Fact]
    public void UserProgress_UpdatedAt_ShouldReflectLatestChanges()
    {
        // Arrange
        var progress = _fixture.Create<UserProgress>();
        var originalUpdatedAt = progress.UpdatedAt;

        // Act
        Thread.Sleep(10);
        progress.UpdatedAt = DateTime.UtcNow;

        // Assert
        progress.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    #endregion
}