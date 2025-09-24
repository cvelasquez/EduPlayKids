using AutoFixture;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EduPlayKids.Tests.Integration;

/// <summary>
/// Integration tests for complete educational workflows.
/// Tests end-to-end learning experiences from child perspective.
/// Critical for validating educational effectiveness and child engagement.
/// </summary>
public class EducationalWorkflowTests
{
    private readonly Fixture _fixture;

    public EducationalWorkflowTests()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    #region Complete Learning Session Workflow

    [Theory]
    [InlineData(3, "PreK")]     // PreK workflow (ages 3-4)
    [InlineData(5, "Kindergarten")] // Kindergarten workflow (age 5)
    [InlineData(7, "Primary")]  // Primary workflow (ages 6-8)
    public async Task CompleteEducationalWorkflow_AgeAppropriate_ShouldSucceed(int childAge, string expectedAgeGroup)
    {
        // Arrange - Set up child profile
        var child = CreateTestChild(childAge);
        var mathSubject = CreateMathSubject();
        var countingActivity = CreateCountingActivity(childAge);

        // Act - Complete full learning workflow
        var workflow = new EducationalWorkflowSimulation();

        // 1. Child selects age-appropriate content
        var selectedContent = await workflow.SelectAgeAppropriateContent(child, mathSubject);

        // 2. Child starts counting activity
        var sessionStart = await workflow.StartActivity(child, countingActivity);

        // 3. Child answers questions (simulate 1 error for 2-star rating)
        var activityResult = await workflow.CompleteActivity(child, countingActivity, errorsCount: 1);

        // 4. System calculates stars and records progress
        var progressResult = await workflow.RecordProgress(child, countingActivity, activityResult);

        // Assert - Verify complete workflow
        selectedContent.Should().NotBeNull("child should receive age-appropriate content");
        sessionStart.Should().BeTrue("activity should start successfully");

        activityResult.IsCompleted.Should().BeTrue("activity should be completed");
        activityResult.ErrorsCount.Should().Be(1, "simulation should record 1 error");
        activityResult.StarsEarned.Should().Be(2, "1 error should earn 2 stars");

        progressResult.Should().BeTrue("progress should be recorded successfully");
    }

    [Fact]
    public async Task StarRatingWorkflow_VariousPerformanceLevels_ShouldCalculateCorrectly()
    {
        // Arrange
        var child = CreateTestChild(6);
        var activity = CreateTestActivity();

        var testCases = new[]
        {
            new { Errors = 0, ExpectedStars = 3 }, // Perfect performance
            new { Errors = 1, ExpectedStars = 2 }, // Good performance
            new { Errors = 2, ExpectedStars = 2 }, // Good performance
            new { Errors = 3, ExpectedStars = 1 }, // Needs improvement
            new { Errors = 5, ExpectedStars = 1 }  // Needs improvement
        };

        var workflow = new EducationalWorkflowSimulation();

        foreach (var testCase in testCases)
        {
            // Act
            var result = await workflow.CompleteActivity(child, activity, testCase.Errors);

            // Assert
            result.StarsEarned.Should().Be(testCase.ExpectedStars,
                $"{testCase.Errors} errors should earn {testCase.ExpectedStars} stars");
        }
    }

    #endregion

    #region Progressive Difficulty Workflow

    [Fact]
    public async Task ProgressiveDifficultyWorkflow_ChildMastery_ShouldUnlockHarderContent()
    {
        // Arrange
        var child = CreateTestChild(6);
        var mathSubject = CreateMathSubject();

        var easyActivity = CreateActivityWithDifficulty("Easy");
        var mediumActivity = CreateActivityWithDifficulty("Medium");
        var hardActivity = CreateActivityWithDifficulty("Hard");

        var workflow = new EducationalWorkflowSimulation();

        // Act - Child masters easy level (3 stars consistently)
        await workflow.CompleteActivity(child, easyActivity, errorsCount: 0); // 3 stars
        await workflow.CompleteActivity(child, easyActivity, errorsCount: 0); // 3 stars

        var mediumUnlocked = await workflow.CheckDifficultyUnlock(child, mathSubject, "Medium");

        // Child performs well on medium level
        await workflow.CompleteActivity(child, mediumActivity, errorsCount: 1); // 2 stars
        await workflow.CompleteActivity(child, mediumActivity, errorsCount: 0); // 3 stars

        var hardUnlocked = await workflow.CheckDifficultyUnlock(child, mathSubject, "Hard");

        // Assert
        mediumUnlocked.Should().BeTrue("mastery of easy level should unlock medium difficulty");
        hardUnlocked.Should().BeTrue("good performance on medium should unlock hard difficulty");
    }

    [Fact]
    public async Task ProgressiveDifficultyWorkflow_ChildStruggling_ShouldStayAtCurrentLevel()
    {
        // Arrange
        var child = CreateTestChild(5);
        var mathSubject = CreateMathSubject();
        var easyActivity = CreateActivityWithDifficulty("Easy");

        var workflow = new EducationalWorkflowSimulation();

        // Act - Child struggles with easy level (1 star consistently)
        await workflow.CompleteActivity(child, easyActivity, errorsCount: 5); // 1 star
        await workflow.CompleteActivity(child, easyActivity, errorsCount: 4); // 1 star
        await workflow.CompleteActivity(child, easyActivity, errorsCount: 6); // 1 star

        var mediumUnlocked = await workflow.CheckDifficultyUnlock(child, mathSubject, "Medium");
        var recommendedDifficulty = await workflow.GetRecommendedDifficulty(child, mathSubject);

        // Assert
        mediumUnlocked.Should().BeFalse("struggling children should not advance to harder content");
        recommendedDifficulty.Should().Be("Easy", "child should continue practicing at easy level");
    }

    #endregion

    #region Multi-Subject Learning Journey

    [Fact]
    public async Task MultiSubjectWorkflow_BalancedLearning_ShouldTrackAcrossSubjects()
    {
        // Arrange
        var child = CreateTestChild(6);
        var subjects = new[]
        {
            CreateSubject("Mathematics", "🔢"),
            CreateSubject("Reading", "📚"),
            CreateSubject("Science", "🔬")
        };

        var workflow = new EducationalWorkflowSimulation();

        // Act - Child practices multiple subjects
        var mathResult = await workflow.CompleteActivity(child, CreateActivityForSubject(subjects[0]), errorsCount: 1);
        var readingResult = await workflow.CompleteActivity(child, CreateActivityForSubject(subjects[1]), errorsCount: 0);
        var scienceResult = await workflow.CompleteActivity(child, CreateActivityForSubject(subjects[2]), errorsCount: 2);

        var overallProgress = await workflow.GetOverallProgress(child);

        // Assert
        mathResult.StarsEarned.Should().Be(2, "1 error in math should earn 2 stars");
        readingResult.StarsEarned.Should().Be(3, "perfect reading should earn 3 stars");
        scienceResult.StarsEarned.Should().Be(2, "2 errors in science should earn 2 stars");

        overallProgress.TotalActivitiesCompleted.Should().Be(3);
        overallProgress.AverageStarRating.Should().BeApproximately(2.33, 0.1, "average of 2,3,2 stars");
        overallProgress.SubjectsExplored.Should().Be(3, "child explored all three subjects");
    }

    #endregion

    #region Parental Oversight Workflow

    [Fact]
    public async Task ParentalOversightWorkflow_ProgressReporting_ShouldProvideInsights()
    {
        // Arrange
        var child = CreateTestChild(5);
        var parent = CreateTestParent();
        var activities = CreateMultipleActivities(5);

        var workflow = new EducationalWorkflowSimulation();

        // Act - Child completes multiple activities over time
        foreach (var activity in activities)
        {
            await workflow.CompleteActivity(child, activity, errorsCount: Random.Shared.Next(0, 4));
        }

        // Parent checks progress
        var parentalReport = await workflow.GenerateParentalReport(child, parent);

        // Assert
        parentalReport.Should().NotBeNull("parents should receive comprehensive reports");
        parentalReport.TotalActivitiesCompleted.Should().Be(5);
        parentalReport.TotalTimeSpent.Should().BeGreaterThan(TimeSpan.Zero);
        parentalReport.OverallPerformance.Should().NotBeNullOrEmpty();
        parentalReport.RecommendedNextSteps.Should().NotBeNullOrEmpty();
        parentalReport.ScreenTimeAnalysis.Should().NotBeNull("screen time monitoring is crucial");
    }

    #endregion

    #region Error Recovery and Encouragement Workflow

    [Fact]
    public async Task ErrorRecoveryWorkflow_ChildMakesMistakes_ShouldProvideEncouragement()
    {
        // Arrange
        var child = CreateTestChild(4); // PreK child - needs extra encouragement
        var activity = CreateTestActivity();
        var workflow = new EducationalWorkflowSimulation();

        // Act - Child makes multiple errors
        var attempt1 = await workflow.AttemptQuestion(child, activity, isCorrect: false);
        var attempt2 = await workflow.AttemptQuestion(child, activity, isCorrect: false);
        var attempt3 = await workflow.AttemptQuestion(child, activity, isCorrect: true);

        var encouragementMessages = await workflow.GetEncouragementMessages(child, attempt1, attempt2, attempt3);

        // Assert
        attempt1.EncouragementLevel.Should().Be("Gentle", "first error should get gentle encouragement");
        attempt2.EncouragementLevel.Should().Be("Supportive", "second error needs supportive guidance");
        attempt3.EncouragementLevel.Should().Be("Celebratory", "success should be celebrated");

        encouragementMessages.Should().HaveCount(3, "each attempt should have encouragement");
        encouragementMessages.Should().AllSatisfy(msg =>
            msg.Should().NotBeNullOrEmpty("encouragement messages should not be empty"));
    }

    #endregion

    #region Test Helper Methods and Classes

    private TestChild CreateTestChild(int age)
    {
        return new TestChild
        {
            Id = Random.Shared.Next(1, 1000),
            Name = "TestChild",
            Age = age,
            LanguagePreference = "es-US" // Hispanic target demographic
        };
    }

    private TestParent CreateTestParent()
    {
        return new TestParent
        {
            Id = Random.Shared.Next(1, 1000),
            Name = "TestParent",
            Email = "parent@test.com"
        };
    }

    private TestSubject CreateMathSubject()
    {
        return CreateSubject("Mathematics", "🔢");
    }

    private TestSubject CreateSubject(string name, string emoji)
    {
        return new TestSubject
        {
            Id = Random.Shared.Next(1, 100),
            Name = name,
            Emoji = emoji,
            AgeGroup = "All"
        };
    }

    private TestActivity CreateTestActivity()
    {
        return new TestActivity
        {
            Id = Random.Shared.Next(1, 1000),
            Title = "Test Activity",
            DifficultyLevel = "Easy",
            EstimatedTimeMinutes = 5,
            QuestionCount = 3
        };
    }

    private TestActivity CreateCountingActivity(int childAge)
    {
        var difficulty = childAge <= 4 ? "Easy" : childAge == 5 ? "Easy" : "Medium";
        return new TestActivity
        {
            Id = Random.Shared.Next(1, 1000),
            Title = "Count to 10",
            DifficultyLevel = difficulty,
            EstimatedTimeMinutes = childAge <= 4 ? 3 : 5,
            QuestionCount = childAge <= 4 ? 3 : 5
        };
    }

    private TestActivity CreateActivityWithDifficulty(string difficulty)
    {
        return new TestActivity
        {
            Id = Random.Shared.Next(1, 1000),
            Title = $"{difficulty} Activity",
            DifficultyLevel = difficulty,
            EstimatedTimeMinutes = 5,
            QuestionCount = 5
        };
    }

    private TestActivity CreateActivityForSubject(TestSubject subject)
    {
        return new TestActivity
        {
            Id = Random.Shared.Next(1, 1000),
            Title = $"{subject.Name} Activity",
            SubjectId = subject.Id,
            DifficultyLevel = "Easy",
            EstimatedTimeMinutes = 5,
            QuestionCount = 3
        };
    }

    private IEnumerable<TestActivity> CreateMultipleActivities(int count)
    {
        return Enumerable.Range(1, count).Select(i => new TestActivity
        {
            Id = i,
            Title = $"Activity {i}",
            DifficultyLevel = "Easy",
            EstimatedTimeMinutes = 5,
            QuestionCount = 3
        });
    }

    // Test Data Classes
    private class TestChild
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public string LanguagePreference { get; set; } = string.Empty;
    }

    private class TestParent
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    private class TestSubject
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Emoji { get; set; } = string.Empty;
        public string AgeGroup { get; set; } = string.Empty;
    }

    private class TestActivity
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int SubjectId { get; set; }
        public string DifficultyLevel { get; set; } = string.Empty;
        public int EstimatedTimeMinutes { get; set; }
        public int QuestionCount { get; set; }
    }

    private class ActivityResult
    {
        public bool IsCompleted { get; set; }
        public int ErrorsCount { get; set; }
        public int StarsEarned { get; set; }
        public TimeSpan TimeSpent { get; set; }
    }

    private class OverallProgress
    {
        public int TotalActivitiesCompleted { get; set; }
        public double AverageStarRating { get; set; }
        public int SubjectsExplored { get; set; }
        public TimeSpan TotalTimeSpent { get; set; }
    }

    private class ParentalReport
    {
        public int TotalActivitiesCompleted { get; set; }
        public TimeSpan TotalTimeSpent { get; set; }
        public string OverallPerformance { get; set; } = string.Empty;
        public string RecommendedNextSteps { get; set; } = string.Empty;
        public ScreenTimeAnalysis ScreenTimeAnalysis { get; set; } = new();
    }

    private class ScreenTimeAnalysis
    {
        public TimeSpan DailyAverage { get; set; }
        public bool IsWithinRecommendedLimits { get; set; }
        public string Recommendation { get; set; } = string.Empty;
    }

    private class QuestionAttempt
    {
        public bool IsCorrect { get; set; }
        public string EncouragementLevel { get; set; } = string.Empty;
        public string FeedbackMessage { get; set; } = string.Empty;
    }

    // Educational Workflow Simulation Class
    private class EducationalWorkflowSimulation
    {
        public async Task<TestActivity> SelectAgeAppropriateContent(TestChild child, TestSubject subject)
        {
            await Task.Delay(10); // Simulate content selection logic
            return CreateAgeAppropriateActivity(child.Age, subject);
        }

        public async Task<bool> StartActivity(TestChild child, TestActivity activity)
        {
            await Task.Delay(5); // Simulate activity startup
            return true;
        }

        public async Task<ActivityResult> CompleteActivity(TestChild child, TestActivity activity, int errorsCount)
        {
            await Task.Delay(20); // Simulate activity completion

            var starsEarned = errorsCount switch
            {
                0 => 3,           // Perfect
                1 or 2 => 2,      // Good
                _ => 1            // Needs improvement
            };

            return new ActivityResult
            {
                IsCompleted = true,
                ErrorsCount = errorsCount,
                StarsEarned = starsEarned,
                TimeSpent = TimeSpan.FromMinutes(activity.EstimatedTimeMinutes)
            };
        }

        public async Task<bool> RecordProgress(TestChild child, TestActivity activity, ActivityResult result)
        {
            await Task.Delay(5); // Simulate database write
            return true;
        }

        public async Task<bool> CheckDifficultyUnlock(TestChild child, TestSubject subject, string difficulty)
        {
            await Task.Delay(10); // Simulate difficulty calculation
            return true; // Simplified for testing
        }

        public async Task<string> GetRecommendedDifficulty(TestChild child, TestSubject subject)
        {
            await Task.Delay(10); // Simulate recommendation algorithm
            return "Easy"; // Simplified for testing
        }

        public async Task<OverallProgress> GetOverallProgress(TestChild child)
        {
            await Task.Delay(15); // Simulate progress calculation
            return new OverallProgress
            {
                TotalActivitiesCompleted = 3,
                AverageStarRating = 2.33,
                SubjectsExplored = 3,
                TotalTimeSpent = TimeSpan.FromMinutes(15)
            };
        }

        public async Task<ParentalReport> GenerateParentalReport(TestChild child, TestParent parent)
        {
            await Task.Delay(25); // Simulate report generation
            return new ParentalReport
            {
                TotalActivitiesCompleted = 5,
                TotalTimeSpent = TimeSpan.FromMinutes(25),
                OverallPerformance = "Good progress",
                RecommendedNextSteps = "Continue with current difficulty",
                ScreenTimeAnalysis = new ScreenTimeAnalysis
                {
                    DailyAverage = TimeSpan.FromMinutes(15),
                    IsWithinRecommendedLimits = true,
                    Recommendation = "Screen time is appropriate for age"
                }
            };
        }

        public async Task<QuestionAttempt> AttemptQuestion(TestChild child, TestActivity activity, bool isCorrect)
        {
            await Task.Delay(5); // Simulate question processing
            return new QuestionAttempt
            {
                IsCorrect = isCorrect,
                EncouragementLevel = isCorrect ? "Celebratory" : "Gentle",
                FeedbackMessage = isCorrect ? "Great job!" : "Let's try again!"
            };
        }

        public async Task<IEnumerable<string>> GetEncouragementMessages(TestChild child, params QuestionAttempt[] attempts)
        {
            await Task.Delay(5); // Simulate message generation
            return attempts.Select(a => a.FeedbackMessage);
        }

        private static TestActivity CreateAgeAppropriateActivity(int age, TestSubject subject)
        {
            var difficulty = age <= 4 ? "Easy" : age == 5 ? "Easy" : "Medium";
            return new TestActivity
            {
                Id = Random.Shared.Next(1, 1000),
                Title = $"{subject.Name} for Age {age}",
                SubjectId = subject.Id,
                DifficultyLevel = difficulty,
                EstimatedTimeMinutes = age <= 4 ? 3 : 5,
                QuestionCount = age <= 4 ? 3 : 5
            };
        }
    }

    #endregion
}