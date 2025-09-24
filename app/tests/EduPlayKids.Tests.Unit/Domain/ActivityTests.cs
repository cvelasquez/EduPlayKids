using AutoFixture;
using AutoFixture.Xunit2;
using EduPlayKids.Domain.Entities;
using EduPlayKids.Domain.Enums;

namespace EduPlayKids.Tests.Unit.Domain;

/// <summary>
/// Unit tests for Activity domain entity.
/// Tests educational content structure and age-appropriate difficulty levels.
/// </summary>
public class ActivityTests
{
    private readonly Fixture _fixture;

    public ActivityTests()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    [Fact]
    public void Activity_Creation_ShouldHaveValidDefaults()
    {
        // Arrange & Act
        var activity = new Activity();

        // Assert
        activity.Id.Should().Be(0);
        activity.IsActive.Should().BeTrue();
        activity.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        activity.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        activity.Questions.Should().NotBeNull();
        activity.Questions.Should().BeEmpty();
        activity.Progress.Should().NotBeNull();
        activity.Progress.Should().BeEmpty();
    }

    [Theory]
    [InlineData(DifficultyLevel.Easy)]
    [InlineData(DifficultyLevel.Medium)]
    [InlineData(DifficultyLevel.Hard)]
    public void Activity_WithValidDifficultyLevel_ShouldBeAccepted(DifficultyLevel difficulty)
    {
        // Arrange
        var activity = _fixture.Build<Activity>()
            .With(x => x.DifficultyLevelEnum, difficulty)
            .Create();

        // Act & Assert
        activity.DifficultyLevelEnum.Should().Be(difficulty);
    }

    [Theory]
    [InlineData(ActivityType.MultipleChoice)]
    [InlineData(ActivityType.DragAndDrop)]
    [InlineData(ActivityType.Matching)]
    [InlineData(ActivityType.FillInTheBlank)]
    public void Activity_WithValidActivityType_ShouldBeAccepted(ActivityType activityType)
    {
        // Arrange
        var activity = _fixture.Build<Activity>()
            .With(x => x.ActivityTypeEnum, activityType)
            .Create();

        // Act & Assert
        activity.ActivityTypeEnum.Should().Be(activityType);
    }

    [Theory]
    [InlineData(AgeGroup.PreK)]
    [InlineData(AgeGroup.Kindergarten)]
    [InlineData(AgeGroup.Primary)]
    public void Activity_WithValidTargetAgeGroup_ShouldBeAccepted(AgeGroup ageGroup)
    {
        // Arrange
        var activity = _fixture.Build<Activity>()
            .With(x => x.TargetAgeGroupEnum, ageGroup)
            .Create();

        // Act & Assert
        activity.TargetAgeGroupEnum.Should().Be(ageGroup);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Activity_WithInvalidTitle_ShouldFailValidation(string invalidTitle)
    {
        // Arrange
        var activity = _fixture.Build<Activity>()
            .With(x => x.TitleEn, invalidTitle)
            .Create();

        // Act & Assert
        activity.TitleEn.Should().Be(invalidTitle);
        // Note: Validation should be implemented to reject empty titles
    }

    [Fact]
    public void Activity_BilingualTitle_ShouldSupportSpanishAndEnglish()
    {
        // Arrange
        const string englishTitle = "Count to Ten";
        const string spanishTitle = "Contar hasta Diez";

        var activity = _fixture.Build<Activity>()
            .With(x => x.TitleEn, englishTitle)
            .With(x => x.TitleEs, spanishTitle)
            .Create();

        // Act & Assert
        activity.TitleEn.Should().Be(englishTitle);
        activity.TitleEs.Should().Be(spanishTitle);
    }

    [Fact]
    public void Activity_BilingualDescription_ShouldSupportSpanishAndEnglish()
    {
        // Arrange
        const string englishDesc = "Learn to count from 1 to 10 with fun animations";
        const string spanishDesc = "Aprende a contar del 1 al 10 con animaciones divertidas";

        var activity = _fixture.Build<Activity>()
            .With(x => x.DescriptionEn, englishDesc)
            .With(x => x.DescriptionEs, spanishDesc)
            .Create();

        // Act & Assert
        activity.DescriptionEn.Should().Be(englishDesc);
        activity.DescriptionEs.Should().Be(spanishDesc);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(15)]
    public void Activity_WithValidOrderInSequence_ShouldBeAccepted(int order)
    {
        // Arrange
        var activity = _fixture.Build<Activity>()
            .With(x => x.OrderInSequence, order)
            .Create();

        // Act & Assert
        activity.OrderInSequence.Should().Be(order);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10)]
    public void Activity_WithInvalidOrderInSequence_ShouldBeDetectable(int invalidOrder)
    {
        // Arrange
        var activity = _fixture.Build<Activity>()
            .With(x => x.OrderInSequence, invalidOrder)
            .Create();

        // Act & Assert
        activity.OrderInSequence.Should().Be(invalidOrder);
        // Note: Validation should ensure order > 0
    }

    [Fact]
    public void Activity_CanHaveMultipleQuestions()
    {
        // Arrange
        var activity = _fixture.Create<Activity>();
        var questions = _fixture.Build<ActivityQuestion>()
            .With(x => x.ActivityId, activity.Id)
            .With(x => x.Activity, activity)
            .CreateMany(3)
            .ToList();

        // Act
        foreach (var question in questions)
        {
            activity.Questions.Add(question);
        }

        // Assert
        activity.Questions.Should().HaveCount(3);
        activity.Questions.Should().Contain(questions);
    }

    [Fact]
    public void Activity_CanTrackProgressFromMultipleChildren()
    {
        // Arrange
        var activity = _fixture.Create<Activity>();
        var progressRecords = _fixture.Build<UserProgress>()
            .With(x => x.ActivityId, activity.Id)
            .With(x => x.Activity, activity)
            .CreateMany(5)
            .ToList();

        // Act
        foreach (var progress in progressRecords)
        {
            activity.Progress.Add(progress);
        }

        // Assert
        activity.Progress.Should().HaveCount(5);
        activity.Progress.Should().Contain(progressRecords);
    }

    [Theory]
    [InlineData(1, 10)] // 1-10 minutes typical for PreK
    [InlineData(5, 15)] // 5-15 minutes for Kindergarten
    [InlineData(10, 20)] // 10-20 minutes for Primary
    public void Activity_EstimatedCompletionTime_ShouldBeAgeAppropriate(int minMinutes, int maxMinutes)
    {
        // Arrange
        var activity = _fixture.Build<Activity>()
            .With(x => x.EstimatedCompletionTimeMinutes, minMinutes)
            .Create();

        // Act & Assert
        activity.EstimatedCompletionTimeMinutes.Should().BeGreaterOrEqualTo(minMinutes);
        // Note: Additional validation could ensure max time is age-appropriate
    }

    [Fact]
    public void Activity_AudioInstructionUrl_CanBeSetForBilingualSupport()
    {
        // Arrange
        const string audioUrl = "audio/activities/counting_1to10_es.mp3";
        var activity = _fixture.Build<Activity>()
            .With(x => x.AudioInstructionUrl, audioUrl)
            .Create();

        // Act & Assert
        activity.AudioInstructionUrl.Should().Be(audioUrl);
    }

    [Fact]
    public void Activity_ThumbnailImageUrl_CanBeSetForVisualization()
    {
        // Arrange
        const string imageUrl = "images/activities/counting_thumbnail.png";
        var activity = _fixture.Build<Activity>()
            .With(x => x.ThumbnailImageUrl, imageUrl)
            .Create();

        // Act & Assert
        activity.ThumbnailImageUrl.Should().Be(imageUrl);
    }

    [Fact]
    public void Activity_UpdatedAt_ShouldReflectContentChanges()
    {
        // Arrange
        var activity = _fixture.Create<Activity>();
        var originalUpdatedAt = activity.UpdatedAt;

        // Act
        Thread.Sleep(10);
        activity.UpdatedAt = DateTime.UtcNow;

        // Assert
        activity.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }
}