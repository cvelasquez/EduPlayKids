using EduPlayKids.Application.Interfaces.Repositories;
using EduPlayKids.Application.Services;
using EduPlayKids.Domain.Entities;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace EduPlayKids.Infrastructure.Services;

/// <summary>
/// Service implementation for delivering educational activities to children.
/// Handles dynamic content loading, age-appropriate filtering, and activity progression.
/// Integrates with repositories to provide comprehensive educational content delivery.
/// </summary>
public class ActivityDeliveryService : IActivityDeliveryService
{
    private readonly IActivityRepository _activityRepository;
    private readonly IActivityQuestionRepository _questionRepository;
    private readonly IUserProgressRepository _progressRepository;
    private readonly IChildRepository _childRepository;
    private readonly ISubjectRepository _subjectRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<ActivityDeliveryService> _logger;

    public ActivityDeliveryService(
        IActivityRepository activityRepository,
        IActivityQuestionRepository questionRepository,
        IUserProgressRepository progressRepository,
        IChildRepository childRepository,
        ISubjectRepository subjectRepository,
        IUserRepository userRepository,
        ILogger<ActivityDeliveryService> logger)
    {
        _activityRepository = activityRepository;
        _questionRepository = questionRepository;
        _progressRepository = progressRepository;
        _childRepository = childRepository;
        _subjectRepository = subjectRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    #region Activity Discovery and Loading

    public async Task<IEnumerable<Activity>> GetAvailableActivitiesAsync(int childId, int subjectId, bool includeCompleted = false, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Loading available activities for child {ChildId} in subject {SubjectId}", childId, subjectId);

            // Get child profile for age-appropriate filtering
            var child = await _childRepository.GetByIdAsync(childId, cancellationToken);
            if (child == null)
            {
                _logger.LogWarning("Child {ChildId} not found", childId);
                return Enumerable.Empty<Activity>();
            }

            // Get user for premium access checking
            var user = await _userRepository.GetByIdAsync(child.UserId, cancellationToken);
            var hasPremiumAccess = user?.HasPremiumAccess() ?? false;

            // Get completed activities for prerequisite checking
            var completedActivityIds = includeCompleted
                ? new List<int>()
                : await _progressRepository.GetCompletedActivityIdsAsync(childId, cancellationToken);

            // Get all activities for the subject
            var allActivities = await _activityRepository.GetBySubjectIdAsync(subjectId, cancellationToken);

            // Filter activities based on child's profile and progress
            var availableActivities = allActivities.Where(activity =>
                activity.IsActive &&
                activity.IsAgeAppropriate(child.Age) &&
                (!activity.RequiresPremium || hasPremiumAccess) &&
                (includeCompleted || !completedActivityIds.Contains(activity.Id)) &&
                activity.ArePrerequisitesMet(completedActivityIds)
            ).OrderBy(a => a.DisplayOrder);

            _logger.LogInformation("Found {Count} available activities for child {ChildId}", availableActivities.Count(), childId);
            return availableActivities;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading available activities for child {ChildId}", childId);
            return Enumerable.Empty<Activity>();
        }
    }

    public async Task<Activity?> LoadActivityForChildAsync(int activityId, int childId, string language = "en", CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Loading activity {ActivityId} for child {ChildId} in language {Language}", activityId, childId, language);

            var activity = await _activityRepository.GetByIdWithQuestionsAsync(activityId, cancellationToken);
            if (activity == null)
            {
                _logger.LogWarning("Activity {ActivityId} not found", activityId);
                return null;
            }

            // Personalize the activity for the specific child
            var personalizedActivity = await PersonalizeActivityForChildAsync(activity, childId, cancellationToken);

            _logger.LogInformation("Successfully loaded activity {ActivityId} with {QuestionCount} questions for child {ChildId}",
                activityId, personalizedActivity.Questions.Count, childId);

            return personalizedActivity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading activity {ActivityId} for child {ChildId}", activityId, childId);
            return null;
        }
    }

    public async Task<Activity?> GetNextRecommendedActivityAsync(int childId, int? subjectId = null, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting next recommended activity for child {ChildId} in subject {SubjectId}", childId, subjectId);

            var child = await _childRepository.GetByIdAsync(childId, cancellationToken);
            if (child == null)
            {
                _logger.LogWarning("Child {ChildId} not found", childId);
                return null;
            }

            // Get child's recent performance for difficulty assessment
            var recentProgress = await _progressRepository.GetRecentProgressAsync(childId, 10, cancellationToken);
            var averageStars = recentProgress.Any() ? recentProgress.Average(p => p.StarsEarned) : 2.0;

            // Determine optimal difficulty based on performance
            var targetDifficulty = averageStars >= 2.5 ? "Medium" : "Easy";
            if (averageStars >= 2.8) targetDifficulty = "Hard";

            // Get available activities
            var availableActivities = subjectId.HasValue
                ? await GetAvailableActivitiesAsync(childId, subjectId.Value, false, cancellationToken)
                : await GetAllAvailableActivitiesAsync(childId, cancellationToken);

            // Filter by target difficulty and select next in sequence
            var recommendedActivity = availableActivities
                .Where(a => a.DifficultyLevel == targetDifficulty)
                .OrderBy(a => a.DisplayOrder)
                .FirstOrDefault();

            // Fallback to any available activity if no difficulty match
            recommendedActivity ??= availableActivities
                .OrderBy(a => a.DisplayOrder)
                .FirstOrDefault();

            if (recommendedActivity != null)
            {
                _logger.LogInformation("Recommended activity {ActivityId} ({Difficulty}) for child {ChildId}",
                    recommendedActivity.Id, recommendedActivity.DifficultyLevel, childId);
            }
            else
            {
                _logger.LogInformation("No recommended activities found for child {ChildId}", childId);
            }

            return recommendedActivity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting next recommended activity for child {ChildId}", childId);
            return null;
        }
    }

    #endregion

    #region Question Delivery and Management

    public async Task<IEnumerable<ActivityQuestion>> GetActivityQuestionsAsync(int activityId, int childId, string language = "en", bool randomizeOrder = false, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Loading questions for activity {ActivityId}, child {ChildId}, randomize: {Randomize}", activityId, childId, randomizeOrder);

            var questions = await _questionRepository.GetByActivityIdAsync(activityId, cancellationToken);

            // Filter active questions only
            var activeQuestions = questions.Where(q => q.IsActive);

            // Apply ordering
            var orderedQuestions = randomizeOrder
                ? activeQuestions.OrderBy(q => Guid.NewGuid())
                : activeQuestions.OrderBy(q => q.DisplayOrder);

            _logger.LogInformation("Loaded {Count} questions for activity {ActivityId}", orderedQuestions.Count(), activityId);
            return orderedQuestions;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading questions for activity {ActivityId}", activityId);
            return Enumerable.Empty<ActivityQuestion>();
        }
    }

    public async Task<ActivityQuestion?> LoadQuestionForChildAsync(int questionId, int childId, string language = "en", CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Loading question {QuestionId} for child {ChildId} in language {Language}", questionId, childId, language);

            var question = await _questionRepository.GetByIdAsync(questionId, cancellationToken);
            if (question == null)
            {
                _logger.LogWarning("Question {QuestionId} not found", questionId);
                return null;
            }

            // Get child's attempt history for this question
            var attemptHistory = await _progressRepository.GetQuestionAttemptHistoryAsync(childId, questionId, cancellationToken);
            var attemptCount = attemptHistory.Count();

            // Determine if hints should be available
            var shouldShowHints = question.ShouldShowHints(attemptCount);

            // Clone question to avoid modifying the original
            var personalizedQuestion = CloneQuestion(question);

            // Apply child-specific adaptations
            if (!shouldShowHints)
            {
                personalizedQuestion.HintsEnabled = false;
            }

            _logger.LogInformation("Successfully loaded question {QuestionId} for child {ChildId}, hints enabled: {HintsEnabled}",
                questionId, childId, personalizedQuestion.HintsEnabled);

            return personalizedQuestion;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading question {QuestionId} for child {ChildId}", questionId, childId);
            return null;
        }
    }

    #endregion

    #region Content Personalization and Adaptation

    public async Task<Activity> PersonalizeActivityForChildAsync(Activity activity, int childId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Personalizing activity {ActivityId} for child {ChildId}", activity.Id, childId);

            var child = await _childRepository.GetByIdAsync(childId, cancellationToken);
            if (child == null)
            {
                _logger.LogWarning("Child {ChildId} not found for personalization", childId);
                return activity;
            }

            // Get child's performance history for this activity type
            var performanceHistory = await _progressRepository.GetActivityTypePerformanceAsync(childId, activity.ActivityType, cancellationToken);

            // Clone activity to avoid modifying the original
            var personalizedActivity = CloneActivity(activity);

            // Adjust based on age and performance
            if (child.Age <= 4)
            {
                // Younger children get more audio support and simplified instructions
                personalizedActivity.EstimatedMinutes = Math.Max(personalizedActivity.EstimatedMinutes - 2, 3);
            }
            else if (child.Age >= 7)
            {
                // Older children can handle slightly more complex content
                personalizedActivity.EstimatedMinutes = Math.Min(personalizedActivity.EstimatedMinutes + 2, 15);
            }

            // Adjust difficulty based on recent performance
            if (performanceHistory.Any())
            {
                // For now, handle dictionary format - would need proper implementation
                if (performanceHistory.ContainsKey("AverageStars") && performanceHistory["AverageStars"] is double averageStars)
                {
                    if (averageStars >= 2.8 && personalizedActivity.DifficultyLevel == "Easy")
                    {
                        personalizedActivity.DifficultyLevel = "Medium";
                    }
                    else if (averageStars <= 1.5 && personalizedActivity.DifficultyLevel == "Hard")
                    {
                        personalizedActivity.DifficultyLevel = "Medium";
                    }
                }
            }

            _logger.LogInformation("Personalized activity {ActivityId}: difficulty {Difficulty}, duration {Minutes}min",
                activity.Id, personalizedActivity.DifficultyLevel, personalizedActivity.EstimatedMinutes);

            return personalizedActivity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error personalizing activity {ActivityId} for child {ChildId}", activity.Id, childId);
            return activity;
        }
    }

    public async Task<Dictionary<string, bool>> GetSupportFeaturesForChildAsync(int childId, string activityType, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting support features for child {ChildId}, activity type {ActivityType}", childId, activityType);

            var child = await _childRepository.GetByIdAsync(childId, cancellationToken);
            var features = new Dictionary<string, bool>();

            if (child == null)
            {
                _logger.LogWarning("Child {ChildId} not found for support features", childId);
                return features;
            }

            // Age-based support features
            features["AudioInstructions"] = child.Age <= 5; // Non-readers need audio
            features["LargeButtons"] = child.Age <= 4; // Younger children need larger touch targets
            features["VisualHints"] = child.Age <= 6; // Visual hints for younger children
            features["ProgressIndicator"] = child.Age >= 5; // Older children can track progress
            features["TimeLimits"] = child.Age >= 7; // Time challenges for older children

            // Activity-type specific features
            switch (activityType.ToLower())
            {
                case "tracing":
                    features["GuideLines"] = child.Age <= 5;
                    features["StrokeValidation"] = child.Age >= 6;
                    break;
                case "draganddrop":
                    features["SnapToTarget"] = child.Age <= 5;
                    features["DropZoneHighlight"] = true;
                    break;
                case "multiplechoice":
                    features["ReadAloud"] = child.Age <= 6;
                    features["EliminateWrongAnswers"] = child.Age <= 4;
                    break;
            }

            // Performance-based adaptations
            var recentPerformance = await _progressRepository.GetRecentProgressAsync(childId, 5, cancellationToken);
            if (recentPerformance.Any())
            {
                var averageStars = recentPerformance.Average(p => p.StarsEarned);
                features["ExtraHints"] = averageStars < 2.0;
                features["CelebrationAnimations"] = averageStars >= 2.5;
            }

            _logger.LogInformation("Generated {Count} support features for child {ChildId}", features.Count, childId);
            return features;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting support features for child {ChildId}", childId);
            return new Dictionary<string, bool>();
        }
    }

    #endregion

    #region Progress and Unlocking Logic

    public async Task<(bool IsUnlocked, string? LockReason)> IsActivityUnlockedForChildAsync(int activityId, int childId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Checking unlock status for activity {ActivityId}, child {ChildId}", activityId, childId);

            var activity = await _activityRepository.GetByIdAsync(activityId, cancellationToken);
            if (activity == null)
            {
                return (false, "Activity not found");
            }

            var child = await _childRepository.GetByIdAsync(childId, cancellationToken);
            if (child == null)
            {
                return (false, "Child profile not found");
            }

            var user = await _userRepository.GetByIdAsync(child.UserId, cancellationToken);
            var hasPremiumAccess = user?.HasPremiumAccess() ?? false;

            var completedActivityIds = await _progressRepository.GetCompletedActivityIdsAsync(childId, cancellationToken);

            if (!activity.IsActive)
                return (false, "Activity is not available");

            if (!activity.IsAgeAppropriate(child.Age))
                return (false, "Activity is not age-appropriate");

            if (activity.RequiresPremium && !hasPremiumAccess)
                return (false, "Premium subscription required");

            if (!activity.ArePrerequisitesMet(completedActivityIds))
                return (false, "Prerequisites not completed");

            return (true, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking unlock status for activity {ActivityId}", activityId);
            return (false, "Error checking activity availability");
        }
    }

    public async Task<Dictionary<string, object>> GetProgressionRequirementsAsync(int childId, int subjectId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting progression requirements for child {ChildId} in subject {SubjectId}", childId, subjectId);

            var requirements = new Dictionary<string, object>();

            var child = await _childRepository.GetByIdAsync(childId, cancellationToken);
            if (child == null)
            {
                requirements["error"] = "Child not found";
                return requirements;
            }

            var allActivities = await _activityRepository.GetBySubjectIdAsync(subjectId, cancellationToken);
            var ageAppropriateActivities = allActivities.Where(a => a.IsAgeAppropriate(child.Age) && a.IsActive).ToList();

            var completedActivityIds = await _progressRepository.GetCompletedActivityIdsAsync(childId, cancellationToken);
            var completedCount = ageAppropriateActivities.Count(a => completedActivityIds.Contains(a.Id));

            var totalActivities = ageAppropriateActivities.Count;
            var completionPercentage = totalActivities > 0 ? (double)completedCount / totalActivities * 100 : 0;

            requirements["totalActivities"] = totalActivities;
            requirements["completedActivities"] = completedCount;
            requirements["completionPercentage"] = Math.Round(completionPercentage, 1);
            requirements["nextMilestoneAt"] = GetNextMilestonePercentage(completionPercentage);
            requirements["remainingToMilestone"] = GetActivitiesToNextMilestone(completionPercentage, totalActivities, completedCount);

            // Get next available activities
            var nextActivities = ageAppropriateActivities
                .Where(a => !completedActivityIds.Contains(a.Id) && a.ArePrerequisitesMet(completedActivityIds))
                .OrderBy(a => a.DisplayOrder)
                .Take(3)
                .Select(a => new { a.Id, a.TitleEn, a.DifficultyLevel })
                .ToList();

            requirements["nextAvailableActivities"] = nextActivities;

            return requirements;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting progression requirements for child {ChildId}", childId);
            return new Dictionary<string, object> { ["error"] = "Error calculating progression requirements" };
        }
    }

    public async Task<Dictionary<string, object>> ProcessActivityCompletionAsync(int childId, int activityId, Dictionary<string, object> completionData, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Processing activity completion for child {ChildId}, activity {ActivityId}", childId, activityId);

            var results = new Dictionary<string, object>();

            // Extract completion data
            var starsEarned = Convert.ToInt32(completionData.GetValueOrDefault("starsEarned", 1));
            var timeSpentSeconds = Convert.ToInt32(completionData.GetValueOrDefault("timeSpentSeconds", 0));
            var correctAnswers = Convert.ToInt32(completionData.GetValueOrDefault("correctAnswers", 0));
            var totalQuestions = Convert.ToInt32(completionData.GetValueOrDefault("totalQuestions", 1));

            // Record progress
            var timeSpentMinutes = Math.Max(1, timeSpentSeconds / 60); // Convert to minutes, minimum 1
            var errorsCount = Math.Max(0, totalQuestions - correctAnswers);
            await _progressRepository.RecordActivityCompletionAsync(childId, activityId, starsEarned,
                timeSpentMinutes, errorsCount, cancellationToken);

            // Update activity statistics
            var activity = await _activityRepository.GetByIdAsync(activityId, cancellationToken);
            if (activity != null)
            {
                activity.UpdateStatistics(timeSpentSeconds, starsEarned);
                await _activityRepository.UpdateAsync(activity, cancellationToken);
            }

            // Check for newly unlocked activities
            var newlyUnlocked = await CheckForNewlyUnlockedActivitiesAsync(childId, cancellationToken);

            // Check for achievements
            var newAchievements = await CheckForNewAchievementsAsync(childId, starsEarned, cancellationToken);

            results["starsEarned"] = starsEarned;
            results["timeSpentSeconds"] = timeSpentSeconds;
            results["accuracy"] = Math.Round((double)correctAnswers / totalQuestions * 100, 1);
            results["newlyUnlockedActivities"] = newlyUnlocked;
            results["newAchievements"] = newAchievements;
            results["completionMessage"] = GenerateCompletionMessage(starsEarned, correctAnswers, totalQuestions);

            _logger.LogInformation("Processed completion for child {ChildId}: {Stars} stars, {Unlocked} unlocked, {Achievements} achievements",
                childId, starsEarned, newlyUnlocked.Count(), newAchievements.Count());

            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing activity completion for child {ChildId}", childId);
            return new Dictionary<string, object> { ["error"] = "Error processing completion" };
        }
    }

    #endregion

    #region Crown Challenges and Advanced Content

    public async Task<Dictionary<string, object>> GetCrownChallengeEligibilityAsync(int childId, int? subjectId = null, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Checking crown challenge eligibility for child {ChildId}", childId);

            var eligibility = new Dictionary<string, object>();

            // Get recent performance (last 10 activities)
            var recentProgress = await _progressRepository.GetRecentProgressAsync(childId, 10, cancellationToken);

            if (!recentProgress.Any())
            {
                eligibility["isEligible"] = false;
                eligibility["reason"] = "Not enough completed activities";
                return eligibility;
            }

            var averageStars = recentProgress.Average(p => p.StarsEarned);
            var perfectActivities = recentProgress.Count(p => p.StarsEarned == 3);
            var perfectPercentage = (double)perfectActivities / recentProgress.Count() * 100;

            // Crown challenge eligibility criteria
            var isEligible = averageStars >= 2.7 && perfectPercentage >= 60;

            eligibility["isEligible"] = isEligible;
            eligibility["averageStars"] = Math.Round(averageStars, 2);
            eligibility["perfectPercentage"] = Math.Round(perfectPercentage, 1);
            eligibility["activitiesAnalyzed"] = recentProgress.Count();

            if (isEligible)
            {
                eligibility["congratsMessage"] = "Outstanding performance! Crown challenges are now available.";
            }
            else
            {
                eligibility["encouragementMessage"] = "Keep practicing to unlock crown challenges!";
                eligibility["targetAverageStars"] = 2.7;
                eligibility["targetPerfectPercentage"] = 60;
            }

            return eligibility;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking crown challenge eligibility for child {ChildId}", childId);
            return new Dictionary<string, object> { ["isEligible"] = false, ["error"] = "Error checking eligibility" };
        }
    }

    public async Task<IEnumerable<Activity>> GetCrownChallengeActivitiesAsync(int childId, int subjectId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Loading crown challenge activities for child {ChildId} in subject {SubjectId}", childId, subjectId);

            // Check eligibility first
            var eligibility = await GetCrownChallengeEligibilityAsync(childId, subjectId, cancellationToken);
            if (!(bool)eligibility.GetValueOrDefault("isEligible", false))
            {
                _logger.LogInformation("Child {ChildId} not eligible for crown challenges", childId);
                return Enumerable.Empty<Activity>();
            }

            var child = await _childRepository.GetByIdAsync(childId, cancellationToken);
            if (child == null)
            {
                return Enumerable.Empty<Activity>();
            }

            var user = await _userRepository.GetByIdAsync(child.UserId, cancellationToken);
            var hasPremiumAccess = user?.HasPremiumAccess() ?? false;

            var crownActivities = await _activityRepository.GetCrownChallengeActivitiesAsync(subjectId, cancellationToken);

            var availableCrownActivities = crownActivities.Where(activity =>
                activity.IsActive &&
                activity.IsAgeAppropriate(child.Age) &&
                (!activity.RequiresPremium || hasPremiumAccess)
            ).OrderBy(a => a.DisplayOrder);

            _logger.LogInformation("Found {Count} crown challenge activities for child {ChildId}", availableCrownActivities.Count(), childId);
            return availableCrownActivities;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading crown challenge activities for child {ChildId}", childId);
            return Enumerable.Empty<Activity>();
        }
    }

    #endregion

    #region Content Analytics and Optimization

    public async Task<bool> TrackContentEngagementAsync(int childId, int activityId, Dictionary<string, object> interactionData, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Tracking content engagement for child {ChildId}, activity {ActivityId}", childId, activityId);

            // Record interaction data in the audit log or analytics system
            // This could be expanded to track detailed interaction patterns

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error tracking content engagement for child {ChildId}", childId);
            return false;
        }
    }

    public async Task<Dictionary<string, object>> GetContentPerformanceMetricsAsync(int? activityId = null, string? ageGroup = null, int daysBack = 30, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting content performance metrics for activity {ActivityId}, age group {AgeGroup}", activityId, ageGroup);

            var metrics = new Dictionary<string, object>();

            // This would analyze performance data across all children
            // For now, return basic metrics structure
            metrics["analysisWindow"] = daysBack;
            metrics["totalSessions"] = 0;
            metrics["averageStars"] = 0.0;
            metrics["completionRate"] = 0.0;
            metrics["averageTimeMinutes"] = 0.0;

            return metrics;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting content performance metrics");
            return new Dictionary<string, object> { ["error"] = "Error calculating metrics" };
        }
    }

    #endregion

    #region Helper Methods

    private async Task<IEnumerable<Activity>> GetAllAvailableActivitiesAsync(int childId, CancellationToken cancellationToken)
    {
        var child = await _childRepository.GetByIdAsync(childId, cancellationToken);
        if (child == null) return Enumerable.Empty<Activity>();

        var allSubjects = await _subjectRepository.GetAllAsync(cancellationToken);
        var allActivities = new List<Activity>();

        foreach (var subject in allSubjects)
        {
            var subjectActivities = await GetAvailableActivitiesAsync(childId, subject.Id, false, cancellationToken);
            allActivities.AddRange(subjectActivities);
        }

        return allActivities;
    }

    private static Activity CloneActivity(Activity original)
    {
        // Simple cloning - in production, consider using AutoMapper or similar
        return new Activity
        {
            Id = original.Id,
            TitleEn = original.TitleEn,
            TitleEs = original.TitleEs,
            DescriptionEn = original.DescriptionEn,
            DescriptionEs = original.DescriptionEs,
            ActivityType = original.ActivityType,
            DifficultyLevel = original.DifficultyLevel,
            MinAge = original.MinAge,
            MaxAge = original.MaxAge,
            DisplayOrder = original.DisplayOrder,
            EstimatedMinutes = original.EstimatedMinutes,
            SubjectId = original.SubjectId,
            Questions = original.Questions
        };
    }

    private static ActivityQuestion CloneQuestion(ActivityQuestion original)
    {
        return new ActivityQuestion
        {
            Id = original.Id,
            QuestionTextEn = original.QuestionTextEn,
            QuestionTextEs = original.QuestionTextEs,
            QuestionType = original.QuestionType,
            DisplayOrder = original.DisplayOrder,
            Points = original.Points,
            ConfigurationData = original.ConfigurationData,
            CorrectAnswer = original.CorrectAnswer,
            HintsEnabled = original.HintsEnabled,
            ActivityId = original.ActivityId
        };
    }

    private static double GetNextMilestonePercentage(double currentPercentage)
    {
        var milestones = new[] { 25.0, 50.0, 75.0, 100.0 };
        return milestones.FirstOrDefault(m => m > currentPercentage);
    }

    private static int GetActivitiesToNextMilestone(double currentPercentage, int totalActivities, int completedCount)
    {
        var nextMilestone = GetNextMilestonePercentage(currentPercentage);
        if (nextMilestone == 0) return 0;

        var requiredForMilestone = (int)Math.Ceiling(totalActivities * nextMilestone / 100);
        return Math.Max(0, requiredForMilestone - completedCount);
    }

    private async Task<IEnumerable<object>> CheckForNewlyUnlockedActivitiesAsync(int childId, CancellationToken cancellationToken)
    {
        // Implementation would check which activities are now unlocked based on new completion
        return new List<object>();
    }

    private async Task<IEnumerable<object>> CheckForNewAchievementsAsync(int childId, int starsEarned, CancellationToken cancellationToken)
    {
        // Implementation would check for achievement criteria met
        return new List<object>();
    }

    private static string GenerateCompletionMessage(int starsEarned, int correctAnswers, int totalQuestions)
    {
        var accuracy = (double)correctAnswers / totalQuestions * 100;

        return starsEarned switch
        {
            3 => "Perfect! Outstanding work! 🌟🌟🌟",
            2 => "Great job! You're doing excellent! 🌟🌟",
            1 => "Good effort! Keep practicing! 🌟",
            _ => "Nice try! Let's keep learning together!"
        };
    }

    #endregion
}