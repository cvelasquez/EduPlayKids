using EduPlayKids.Application.Interfaces.Repositories;
using EduPlayKids.Application.Services;
using EduPlayKids.Domain.Entities;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace EduPlayKids.Infrastructure.Services;

/// <summary>
/// Service implementation for managing educational content progression and unlocking logic.
/// Handles sequential learning paths, prerequisite validation, and adaptive content delivery.
/// </summary>
public class ContentProgressionService : IContentProgressionService
{
    private readonly IActivityRepository _activityRepository;
    private readonly IUserProgressRepository _progressRepository;
    private readonly IChildRepository _childRepository;
    private readonly ISubjectRepository _subjectRepository;
    private readonly IAchievementRepository _achievementRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<ContentProgressionService> _logger;

    public ContentProgressionService(
        IActivityRepository activityRepository,
        IUserProgressRepository progressRepository,
        IChildRepository childRepository,
        ISubjectRepository subjectRepository,
        IAchievementRepository achievementRepository,
        IUserRepository userRepository,
        ILogger<ContentProgressionService> logger)
    {
        _activityRepository = activityRepository;
        _progressRepository = progressRepository;
        _childRepository = childRepository;
        _subjectRepository = subjectRepository;
        _achievementRepository = achievementRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    #region Content Unlocking Logic

    public async Task<IEnumerable<UnlockedActivityInfo>> GetUnlockedActivitiesAsync(int childId, int? subjectId = null, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting unlocked activities for child {ChildId}, subject {SubjectId}", childId, subjectId);

            var child = await _childRepository.GetByIdAsync(childId, cancellationToken);
            if (child == null)
            {
                _logger.LogWarning("Child {ChildId} not found", childId);
                return Enumerable.Empty<UnlockedActivityInfo>();
            }

            var user = await _userRepository.GetByIdAsync(child.UserId, cancellationToken);
            var hasPremiumAccess = user?.HasPremiumAccess() ?? false;

            var completedActivityIds = await _progressRepository.GetCompletedActivityIdsAsync(childId, cancellationToken);

            IEnumerable<Activity> activities;
            if (subjectId.HasValue)
            {
                activities = await _activityRepository.GetBySubjectIdAsync(subjectId.Value, cancellationToken);
            }
            else
            {
                activities = await _activityRepository.GetAllAsync(cancellationToken);
            }

            var unlockedActivities = new List<UnlockedActivityInfo>();

            foreach (var activity in activities.Where(a => a.IsActive))
            {
                var unlockStatus = await CheckActivityUnlockStatusAsync(activity.Id, childId, cancellationToken);

                if (unlockStatus.IsUnlocked)
                {
                    var info = new UnlockedActivityInfo
                    {
                        Activity = activity,
                        UnlockedDate = DateTime.UtcNow, // In practice, track actual unlock dates
                        UnlockReason = unlockStatus.Status,
                        IsNewlyUnlocked = false, // Would check against previous state
                        RecommendationScore = CalculateRecommendationScore(activity, child, completedActivityIds)
                    };
                    unlockedActivities.Add(info);
                }
            }

            _logger.LogInformation("Found {Count} unlocked activities for child {ChildId}", unlockedActivities.Count, childId);
            return unlockedActivities.OrderByDescending(a => a.RecommendationScore);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting unlocked activities for child {ChildId}", childId);
            return Enumerable.Empty<UnlockedActivityInfo>();
        }
    }

    public async Task<ActivityUnlockStatus> CheckActivityUnlockStatusAsync(int activityId, int childId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Checking unlock status for activity {ActivityId}, child {ChildId}", activityId, childId);

            var activity = await _activityRepository.GetByIdAsync(activityId, cancellationToken);
            if (activity == null)
            {
                return new ActivityUnlockStatus
                {
                    ActivityId = activityId,
                    IsUnlocked = false,
                    Status = "Not Found",
                    Requirements = new List<string> { "Activity does not exist" }
                };
            }

            var child = await _childRepository.GetByIdAsync(childId, cancellationToken);
            if (child == null)
            {
                return new ActivityUnlockStatus
                {
                    ActivityId = activityId,
                    IsUnlocked = false,
                    Status = "Child Not Found",
                    Requirements = new List<string> { "Child profile not found" }
                };
            }

            var user = await _userRepository.GetByIdAsync(child.UserId, cancellationToken);
            var hasPremiumAccess = user?.HasPremiumAccess() ?? false;

            var completedActivityIds = await _progressRepository.GetCompletedActivityIdsAsync(childId, cancellationToken);

            var status = new ActivityUnlockStatus { ActivityId = activityId };
            var requirements = new List<string>();
            var completedRequirements = new List<string>();

            // Check age appropriateness
            if (!activity.IsAgeAppropriate(child.Age))
            {
                requirements.Add($"Age must be between {activity.MinAge} and {activity.MaxAge}");
                status.Status = "Age Restricted";
            }
            else
            {
                completedRequirements.Add("Age appropriate");
            }

            // Check premium requirements
            if (activity.RequiresPremium)
            {
                requirements.Add("Premium subscription required");
                if (hasPremiumAccess)
                {
                    completedRequirements.Add("Premium access");
                }
                else
                {
                    status.Status = "Premium Required";
                }
            }
            else
            {
                completedRequirements.Add("No premium required");
            }

            // Check prerequisites
            if (!string.IsNullOrEmpty(activity.Prerequisites))
            {
                try
                {
                    var prerequisiteIds = JsonSerializer.Deserialize<int[]>(activity.Prerequisites) ?? Array.Empty<int>();
                    var completedPrereqs = prerequisiteIds.Where(id => completedActivityIds.Contains(id)).ToList();
                    var missingPrereqs = prerequisiteIds.Except(completedPrereqs).ToList();

                    foreach (var prereqId in prerequisiteIds)
                    {
                        var prereqActivity = await _activityRepository.GetByIdAsync(prereqId, cancellationToken);
                        var prereqName = prereqActivity?.TitleEn ?? $"Activity {prereqId}";

                        if (completedActivityIds.Contains(prereqId))
                        {
                            completedRequirements.Add($"Completed: {prereqName}");
                        }
                        else
                        {
                            requirements.Add($"Complete: {prereqName}");
                        }
                    }

                    if (missingPrereqs.Any())
                    {
                        status.Status = "Prerequisites Required";
                    }

                    status.ProgressPercentage = prerequisiteIds.Length > 0
                        ? (int)((double)completedPrereqs.Count / prerequisiteIds.Length * 100)
                        : 100;
                }
                catch (JsonException)
                {
                    _logger.LogWarning("Invalid prerequisites JSON for activity {ActivityId}", activityId);
                }
            }
            else
            {
                completedRequirements.Add("No prerequisites");
                status.ProgressPercentage = 100;
            }

            // Determine final unlock status
            if (requirements.Count == 0)
            {
                status.IsUnlocked = true;
                status.Status = "Unlocked";
            }
            else if (string.IsNullOrEmpty(status.Status))
            {
                status.Status = "Locked";
            }

            status.Requirements = requirements;
            status.CompletedRequirements = completedRequirements;

            // Estimate unlock date for prerequisites
            if (requirements.Any(r => r.Contains("Complete:")))
            {
                status.EstimatedUnlockDate = EstimateUnlockDate(childId, requirements.Count);
            }

            return status;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking unlock status for activity {ActivityId}", activityId);
            return new ActivityUnlockStatus
            {
                ActivityId = activityId,
                IsUnlocked = false,
                Status = "Error",
                Requirements = new List<string> { "Error checking requirements" }
            };
        }
    }

    public async Task<ProgressionUpdateResult> ProcessActivityCompletionAsync(int childId, int activityId, ActivityCompletionData completionResults, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Processing activity completion for child {ChildId}, activity {ActivityId}", childId, activityId);

            var result = new ProgressionUpdateResult
            {
                ChildId = childId,
                CompletedActivityId = activityId
            };

            // Get all activities to check for newly unlocked ones
            var allActivities = await _activityRepository.GetAllAsync(cancellationToken);
            var previouslyUnlocked = await GetUnlockedActivitiesAsync(childId, null, cancellationToken);
            var previouslyUnlockedIds = previouslyUnlocked.Select(u => u.Activity.Id).ToHashSet();

            // Record the completion
            var timeSpentMinutes = (int)Math.Ceiling(completionResults.TimeSpentSeconds / 60.0);
            var errorsCount = completionResults.TotalQuestions - completionResults.CorrectAnswers;
            await _progressRepository.RecordActivityCompletionAsync(
                childId, activityId, completionResults.StarsEarned,
                timeSpentMinutes, errorsCount, cancellationToken);

            // Check for newly unlocked activities
            var currentlyUnlocked = await GetUnlockedActivitiesAsync(childId, null, cancellationToken);
            var newlyUnlockedActivities = currentlyUnlocked.Where(u => !previouslyUnlockedIds.Contains(u.Activity.Id)).ToList();

            foreach (var unlockedActivity in newlyUnlockedActivities)
            {
                unlockedActivity.IsNewlyUnlocked = true;
                unlockedActivity.UnlockedDate = DateTime.UtcNow;
                unlockedActivity.UnlockReason = $"Unlocked by completing Activity {activityId}";
            }

            result.NewlyUnlockedActivities = newlyUnlockedActivities.ToList();

            // Check for crown challenge eligibility
            var activity = await _activityRepository.GetByIdAsync(activityId, cancellationToken);
            if (activity != null)
            {
                var crownEligibility = await EvaluateCrownChallengeEligibilityAsync(childId, activity.SubjectId, cancellationToken);
                result.UnlockedCrownChallenges = crownEligibility.IsEligible;

                // Update subject progress
                result.UpdatedProgress = await TrackSubjectProgressAsync(childId, activity.SubjectId, cancellationToken);
            }

            // Generate celebration message
            result.CelebrationMessage = GenerateCompletionCelebration(completionResults, newlyUnlockedActivities.Count);

            _logger.LogInformation("Processed completion: {NewActivities} newly unlocked, crown challenges: {CrownUnlocked}",
                newlyUnlockedActivities.Count, result.UnlockedCrownChallenges);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing activity completion for child {ChildId}", childId);
            return new ProgressionUpdateResult { ChildId = childId, CompletedActivityId = activityId };
        }
    }

    #endregion

    #region Learning Path Management

    public async Task<PersonalizedLearningPath> GenerateLearningPathAsync(int childId, int? subjectId = null, int pathLength = 10, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Generating learning path for child {ChildId}, subject {SubjectId}, length {PathLength}",
                childId, subjectId, pathLength);

            var child = await _childRepository.GetByIdAsync(childId, cancellationToken);
            if (child == null)
            {
                return new PersonalizedLearningPath { ChildId = childId, GeneratedDate = DateTime.UtcNow };
            }

            var unlockedActivities = await GetUnlockedActivitiesAsync(childId, subjectId, cancellationToken);
            var recentProgress = await _progressRepository.GetRecentProgressAsync(childId, 10, cancellationToken);

            // Determine optimal difficulty based on recent performance
            var averageStars = recentProgress.Any() ? recentProgress.Average(p => p.StarsEarned) : 2.0;
            var targetDifficulty = averageStars >= 2.5 ? "Medium" : "Easy";
            if (averageStars >= 2.8) targetDifficulty = "Hard";

            // Filter and rank activities
            var candidateActivities = new List<UnlockedActivityInfo>();
            foreach (var activity in unlockedActivities)
            {
                var isCompleted = await _progressRepository.HasCompletedActivityAsync(childId, activity.Activity.Id, cancellationToken);
                if (!isCompleted)
                {
                    candidateActivities.Add(activity);
                }
            }

            candidateActivities = candidateActivities
                .OrderByDescending(u => u.RecommendationScore)
                .Take(pathLength * 2) // Get more candidates than needed
                .ToList();

            var pathActivities = new List<PathActivity>();
            var sequenceNumber = 1;

            foreach (var candidate in candidateActivities.Take(pathLength))
            {
                var pathActivity = new PathActivity
                {
                    Activity = candidate.Activity,
                    SequenceNumber = sequenceNumber++,
                    RecommendationReason = candidate.UnlockReason,
                    LearningObjectives = ExtractLearningObjectives(candidate.Activity),
                    DifficultyJustification = $"Matches target difficulty '{targetDifficulty}' based on recent performance"
                };
                pathActivities.Add(pathActivity);
            }

            var learningPath = new PersonalizedLearningPath
            {
                ChildId = childId,
                RecommendedActivities = pathActivities,
                PathReasoning = GeneratePathReasoning(child, averageStars, pathActivities.Count),
                LearningObjectives = ExtractPathLearningObjectives(pathActivities),
                EstimatedCompletionDays = EstimateCompletionDays(pathActivities.Count, child.Age),
                GeneratedDate = DateTime.UtcNow
            };

            _logger.LogInformation("Generated learning path with {ActivityCount} activities for child {ChildId}",
                pathActivities.Count, childId);

            return learningPath;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating learning path for child {ChildId}", childId);
            return new PersonalizedLearningPath { ChildId = childId, GeneratedDate = DateTime.UtcNow };
        }
    }

    public async Task<MilestoneActivity?> GetNextMilestoneActivityAsync(int childId, int subjectId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting next milestone for child {ChildId}, subject {SubjectId}", childId, subjectId);

            var unlockedActivities = await GetUnlockedActivitiesAsync(childId, subjectId, cancellationToken);
            var completedActivityIds = await _progressRepository.GetCompletedActivityIdsAsync(childId, cancellationToken);

            // Find activities that represent key milestones (every 5th activity in sequence)
            var milestoneActivity = unlockedActivities
                .Where(u => !completedActivityIds.Contains(u.Activity.Id))
                .Where(u => u.Activity.DisplayOrder % 5 == 0) // Milestone activities
                .OrderBy(u => u.Activity.DisplayOrder)
                .FirstOrDefault();

            if (milestoneActivity == null) return null;

            var prerequisites = await GetActivityPrerequisites(milestoneActivity.Activity.Id, cancellationToken);
            var completedPrereqs = prerequisites.Count(id => completedActivityIds.Contains(id));

            return new MilestoneActivity
            {
                Activity = milestoneActivity.Activity,
                MilestoneDescription = $"Complete key learning objectives in {milestoneActivity.Activity.TitleEn}",
                KeyLearningObjectives = ExtractLearningObjectives(milestoneActivity.Activity),
                RequiredPrerequisites = prerequisites.Count,
                CompletedPrerequisites = completedPrereqs,
                EstimatedReachDate = EstimateReachDate(childId, prerequisites.Count - completedPrereqs)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting next milestone for child {ChildId}", childId);
            return null;
        }
    }

    public async Task<SubjectProgressTracking> TrackSubjectProgressAsync(int childId, int subjectId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Tracking subject progress for child {ChildId}, subject {SubjectId}", childId, subjectId);

            var child = await _childRepository.GetByIdAsync(childId, cancellationToken);
            var subject = await _subjectRepository.GetByIdAsync(subjectId, cancellationToken);

            if (child == null || subject == null)
            {
                return new SubjectProgressTracking { ChildId = childId, SubjectId = subjectId };
            }

            var allSubjectActivities = await _activityRepository.GetBySubjectIdAsync(subjectId, cancellationToken);
            var ageAppropriateActivities = allSubjectActivities.Where(a => a.IsAgeAppropriate(child.Age) && a.IsActive).ToList();

            var completedActivityIds = await _progressRepository.GetCompletedActivityIdsAsync(childId, cancellationToken);
            var completedSubjectActivities = ageAppropriateActivities.Where(a => completedActivityIds.Contains(a.Id)).ToList();

            var subjectProgress = await _progressRepository.GetSubjectProgressAsync(childId, subjectId, cancellationToken);
            var averageStars = subjectProgress.Any() ? subjectProgress.Average(p => p.StarsEarned) : 0.0;

            var completionPercentage = ageAppropriateActivities.Count > 0
                ? (double)completedSubjectActivities.Count / ageAppropriateActivities.Count * 100
                : 0;

            var tracking = new SubjectProgressTracking
            {
                ChildId = childId,
                SubjectId = subjectId,
                SubjectName = subject.NameEn,
                TotalActivities = ageAppropriateActivities.Count,
                CompletedActivities = completedSubjectActivities.Count,
                CompletionPercentage = Math.Round(completionPercentage, 1),
                AverageStarRating = Math.Round(averageStars, 2),
                CurrentStreak = await CalculateCurrentStreak(childId, subjectId, cancellationToken),
                MasteredConcepts = ExtractMasteredConcepts(completedSubjectActivities, averageStars),
                ConceptsInProgress = ExtractConceptsInProgress(ageAppropriateActivities, completedActivityIds),
                LastActivityDate = subjectProgress.LastOrDefault()?.CreatedAt ?? DateTime.MinValue
            };

            return tracking;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error tracking subject progress for child {ChildId}", childId);
            return new SubjectProgressTracking { ChildId = childId, SubjectId = subjectId };
        }
    }

    #endregion

    #region Adaptive Difficulty and Content Selection

    public async Task<DifficultyRecommendation> DetermineOptimalDifficultyAsync(int childId, string activityType, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Determining optimal difficulty for child {ChildId}, activity type {ActivityType}", childId, activityType);

            var recentProgress = await _progressRepository.GetActivityTypePerformanceAsync(childId, activityType, cancellationToken);

            if (!recentProgress.Any())
            {
                return new DifficultyRecommendation
                {
                    RecommendedDifficulty = "Easy",
                    ConfidenceScore = 0.5,
                    Reasoning = "No previous performance data available, starting with Easy difficulty",
                    DifficultyScores = new Dictionary<string, double> { ["Easy"] = 0.5, ["Medium"] = 0.3, ["Hard"] = 0.2 }
                };
            }

            // Handle dictionary format - would need proper implementation
            var averageStars = recentProgress.ContainsKey("AverageStars") && recentProgress["AverageStars"] is double avgStars ? avgStars : 2.0;
            var averageTime = recentProgress.ContainsKey("AverageTime") && recentProgress["AverageTime"] is double avgTime ? avgTime : 5.0;
            var completionRate = recentProgress.ContainsKey("CompletionRate") && recentProgress["CompletionRate"] is double compRate ? compRate : 0.8;

            var difficultyScores = new Dictionary<string, double>
            {
                ["Easy"] = CalculateEasyScore(averageStars, completionRate),
                ["Medium"] = CalculateMediumScore(averageStars, completionRate),
                ["Hard"] = CalculateHardScore(averageStars, completionRate)
            };

            var recommendedDifficulty = difficultyScores.OrderByDescending(kvp => kvp.Value).First();

            return new DifficultyRecommendation
            {
                RecommendedDifficulty = recommendedDifficulty.Key,
                ConfidenceScore = recommendedDifficulty.Value,
                Reasoning = GenerateDifficultyReasoning(averageStars, completionRate, recommendedDifficulty.Key),
                DifficultyScores = difficultyScores,
                ShouldAdjustFromCurrent = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error determining optimal difficulty for child {ChildId}", childId);
            return new DifficultyRecommendation
            {
                RecommendedDifficulty = "Easy",
                ConfidenceScore = 0.0,
                Reasoning = "Error occurred, defaulting to Easy difficulty"
            };
        }
    }

    public async Task<ActivitySelectionResult> SelectOptimalNextActivityAsync(int childId, IEnumerable<Activity> availableActivities, ActivitySelectionCriteria selectionCriteria, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Selecting optimal next activity for child {ChildId} from {ActivityCount} candidates",
                childId, availableActivities.Count());

            var child = await _childRepository.GetByIdAsync(childId, cancellationToken);
            if (child == null)
            {
                return new ActivitySelectionResult();
            }

            var recentProgress = await _progressRepository.GetRecentProgressAsync(childId, 5, cancellationToken);
            var completedActivityIds = await _progressRepository.GetCompletedActivityIdsAsync(childId, cancellationToken);

            var candidates = new List<ActivityCandidate>();

            foreach (var activity in availableActivities)
            {
                // Skip recently completed if specified
                if (selectionCriteria.AvoidRecentlyCompleted && completedActivityIds.Contains(activity.Id))
                    continue;

                // Check time constraints
                if (activity.EstimatedMinutes > selectionCriteria.MaxTimeMinutes)
                    continue;

                var score = CalculateActivitySelectionScore(activity, child, recentProgress, selectionCriteria);
                var reasoning = GenerateSelectionReasoning(activity, score, selectionCriteria);

                candidates.Add(new ActivityCandidate
                {
                    Activity = activity,
                    Score = score,
                    Reasoning = reasoning
                });
            }

            var bestCandidate = candidates.OrderByDescending(c => c.Score).FirstOrDefault();
            var alternatives = candidates.Where(c => c != bestCandidate).OrderByDescending(c => c.Score).Take(3).ToList();

            return new ActivitySelectionResult
            {
                SelectedActivity = bestCandidate?.Activity,
                SelectionScore = bestCandidate?.Score ?? 0,
                SelectionReasoning = bestCandidate?.Reasoning ?? "No suitable activity found",
                AlternativeCandidates = alternatives
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error selecting optimal next activity for child {ChildId}", childId);
            return new ActivitySelectionResult();
        }
    }

    #endregion

    #region Crown Challenges and Advanced Content

    public async Task<CrownChallengeEligibility> EvaluateCrownChallengeEligibilityAsync(int childId, int subjectId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Evaluating crown challenge eligibility for child {ChildId}, subject {SubjectId}", childId, subjectId);

            var recentProgress = await _progressRepository.GetSubjectProgressAsync(childId, subjectId, cancellationToken);
            var recentActivities = recentProgress.TakeLast(10).ToList();

            if (recentActivities.Count < 5)
            {
                return new CrownChallengeEligibility
                {
                    ChildId = childId,
                    SubjectId = subjectId,
                    IsEligible = false,
                    MasteryScore = 0,
                    EligibilityReason = "Not enough completed activities in this subject"
                };
            }

            var averageStars = recentActivities.Average(p => p.StarsEarned);
            var perfectActivities = recentActivities.Count(p => p.StarsEarned == 3);
            var masteryScore = (averageStars / 3.0) * 100;

            var isEligible = averageStars >= 2.7 && perfectActivities >= 6;

            return new CrownChallengeEligibility
            {
                ChildId = childId,
                SubjectId = subjectId,
                IsEligible = isEligible,
                MasteryScore = Math.Round(masteryScore, 1),
                MasteredConcepts = ExtractMasteredConceptsFromProgress(recentActivities),
                RequiredMasteries = GetRequiredMasteriesForCrown(subjectId),
                EligibilityReason = isEligible
                    ? "Excellent performance qualifies for crown challenges!"
                    : $"Need {2.7 - averageStars:F1} more average stars and {Math.Max(0, 6 - perfectActivities)} more perfect activities"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error evaluating crown challenge eligibility for child {ChildId}", childId);
            return new CrownChallengeEligibility
            {
                ChildId = childId,
                SubjectId = subjectId,
                IsEligible = false,
                EligibilityReason = "Error evaluating eligibility"
            };
        }
    }

    public async Task<CrownChallengeUnlockResult> UnlockCrownChallengesAsync(int childId, int subjectId, MasteryEvidence masteryEvidence, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Unlocking crown challenges for child {ChildId}, subject {SubjectId}", childId, subjectId);

            var crownActivities = await _activityRepository.GetCrownChallengeActivitiesAsync(subjectId, cancellationToken);
            var child = await _childRepository.GetByIdAsync(childId, cancellationToken);

            var unlockedChallenges = crownActivities
                .Where(a => child != null && a.IsAgeAppropriate(child.Age))
                .ToList();

            var result = new CrownChallengeUnlockResult
            {
                ChildId = childId,
                SubjectId = subjectId,
                UnlockedChallenges = unlockedChallenges,
                CelebrationTitle = "🏆 Crown Challenges Unlocked! 🏆",
                CelebrationMessage = "Amazing mastery! You've unlocked special crown challenges with extra exciting activities!",
                BadgeImagePath = "badges/crown_master.png",
                UnlockDate = DateTime.UtcNow
            };

            // TODO: Record crown challenge unlock in database

            _logger.LogInformation("Unlocked {Count} crown challenges for child {ChildId}", unlockedChallenges.Count, childId);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unlocking crown challenges for child {ChildId}", childId);
            return new CrownChallengeUnlockResult { ChildId = childId, SubjectId = subjectId, UnlockDate = DateTime.UtcNow };
        }
    }

    #endregion

    #region Progress Analytics and Insights

    public async Task<LearningVelocityAnalysis> AnalyzeLearningVelocityAsync(int childId, int analysisWindowDays = 30, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Analyzing learning velocity for child {ChildId}", childId);

            var recentProgress = await _progressRepository.GetRecentProgressAsync(childId, analysisWindowDays * 2, cancellationToken);
            var windowStart = DateTime.UtcNow.AddDays(-analysisWindowDays);
            var windowProgress = recentProgress.Where(p => p.CreatedAt >= windowStart).ToList();

            if (!windowProgress.Any())
            {
                return new LearningVelocityAnalysis
                {
                    ChildId = childId,
                    ActivitiesPerDay = 0,
                    AverageStarsPerActivity = 0,
                    AverageSessionLength = TimeSpan.Zero,
                    VelocityTrend = "No Data"
                };
            }

            var activitiesPerDay = (double)windowProgress.Count / analysisWindowDays;
            var averageStars = windowProgress.Average(p => p.StarsEarned);
            var averageSessionLength = TimeSpan.FromMinutes(windowProgress.Average(p => p.TimeSpentMinutes));

            // Calculate velocity trend
            var firstHalf = windowProgress.Take(windowProgress.Count / 2);
            var secondHalf = windowProgress.Skip(windowProgress.Count / 2);
            var trend = "Steady";

            if (secondHalf.Any() && firstHalf.Any())
            {
                var firstHalfAvg = firstHalf.Average(p => p.StarsEarned);
                var secondHalfAvg = secondHalf.Average(p => p.StarsEarned);

                if (secondHalfAvg > firstHalfAvg + 0.2) trend = "Accelerating";
                else if (secondHalfAvg < firstHalfAvg - 0.2) trend = "Slowing";
            }

            return new LearningVelocityAnalysis
            {
                ChildId = childId,
                ActivitiesPerDay = Math.Round(activitiesPerDay, 2),
                AverageStarsPerActivity = Math.Round(averageStars, 2),
                AverageSessionLength = averageSessionLength,
                SubjectVelocities = await CalculateSubjectVelocities(childId, analysisWindowDays, cancellationToken),
                EstimatedCurriculumCompletion = EstimateCurriculumCompletion(activitiesPerDay),
                VelocityTrend = trend
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing learning velocity for child {ChildId}", childId);
            return new LearningVelocityAnalysis { ChildId = childId, VelocityTrend = "Error" };
        }
    }

    public async Task<LearningGapAnalysis> IdentifyLearningGapsAsync(int childId, int? subjectId = null, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Identifying learning gaps for child {ChildId}", childId);

            var progress = subjectId.HasValue
                ? await _progressRepository.GetSubjectProgressAsync(childId, subjectId.Value, cancellationToken)
                : await _progressRepository.GetRecentProgressAsync(childId, 50, cancellationToken);

            var gaps = new List<LearningGap>();

            // Identify activities with consistently low performance
            var lowPerformanceActivities = progress
                .GroupBy(p => p.ActivityId)
                .Where(g => g.Average(p => p.StarsEarned) < 2.0)
                .ToList();

            foreach (var activityGroup in lowPerformanceActivities)
            {
                var activity = await _activityRepository.GetByIdAsync(activityGroup.Key, cancellationToken);
                if (activity != null)
                {
                    var averageStars = activityGroup.Average(p => p.StarsEarned);
                    var severity = averageStars < 1.5 ? "Significant" : averageStars < 2.0 ? "Moderate" : "Minor";

                    gaps.Add(new LearningGap
                    {
                        ConceptName = activity.TitleEn,
                        GapDescription = $"Difficulty with {activity.ActivityType} activities",
                        Severity = severity,
                        PerformanceScore = averageStars,
                        ProblematicActivityIds = activityGroup.Select(p => p.ActivityId).ToList()
                    });
                }
            }

            return new LearningGapAnalysis
            {
                ChildId = childId,
                IdentifiedGaps = gaps,
                RemediationActivities = await GetRemediationActivities(gaps, cancellationToken),
                OverallAssessment = GenerateGapAssessment(gaps),
                AnalysisDate = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error identifying learning gaps for child {ChildId}", childId);
            return new LearningGapAnalysis { ChildId = childId, AnalysisDate = DateTime.UtcNow };
        }
    }

    public async Task<ProgressReport> GenerateProgressReportAsync(int childId, int reportPeriodDays = 30, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Generating progress report for child {ChildId}", childId);

            var subjects = await _subjectRepository.GetAllAsync(cancellationToken);
            var subjectProgress = new Dictionary<string, SubjectProgressTracking>();

            foreach (var subject in subjects)
            {
                var tracking = await TrackSubjectProgressAsync(childId, subject.Id, cancellationToken);
                subjectProgress[subject.NameEn] = tracking;
            }

            var velocityAnalysis = await AnalyzeLearningVelocityAsync(childId, reportPeriodDays, cancellationToken);
            var recentAchievements = await GetRecentAchievements(childId, reportPeriodDays, cancellationToken);

            return new ProgressReport
            {
                ChildId = childId,
                ReportDate = DateTime.UtcNow,
                ReportPeriodDays = reportPeriodDays,
                SubjectProgress = subjectProgress,
                VelocityAnalysis = velocityAnalysis,
                RecentAchievements = recentAchievements,
                OverallSummary = GenerateOverallSummary(subjectProgress, velocityAnalysis),
                ParentRecommendations = GenerateParentRecommendations(subjectProgress, velocityAnalysis)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating progress report for child {ChildId}", childId);
            return new ProgressReport { ChildId = childId, ReportDate = DateTime.UtcNow };
        }
    }

    #endregion

    #region Motivational Features

    public async Task<LearningStreakStatus> TrackLearningStreaksAsync(int childId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Tracking learning streaks for child {ChildId}", childId);

            var recentProgress = await _progressRepository.GetRecentProgressAsync(childId, 30, cancellationToken);
            var currentStreak = await CalculateCurrentStreak(childId, null, cancellationToken);
            var longestStreak = await CalculateLongestStreak(childId, cancellationToken);

            return new LearningStreakStatus
            {
                ChildId = childId,
                CurrentStreak = currentStreak,
                LongestStreak = longestStreak,
                StreakStartDate = DateTime.UtcNow.AddDays(-currentStreak),
                StreakType = "Daily",
                IsStreakActive = currentStreak > 0,
                DaysUntilNextMilestone = CalculateDaysToNextMilestone(currentStreak)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error tracking learning streaks for child {ChildId}", childId);
            return new LearningStreakStatus { ChildId = childId };
        }
    }

    public async Task<MotivationalContent> GenerateMotivationalContentAsync(int childId, ActivityCompletionData recentActivity, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Generating motivational content for child {ChildId}", childId);

            var child = await _childRepository.GetByIdAsync(childId, cancellationToken);
            if (child == null)
            {
                return new MotivationalContent { MessageType = "Error", Message = "Child not found" };
            }

            if (recentActivity.StarsEarned == 3 && recentActivity.IsFirstCompletion)
            {
                return new MotivationalContent
                {
                    MessageType = "Celebration",
                    Title = "Perfect First Try! 🌟",
                    Message = child.Age <= 4
                        ? "WOW! You're amazing! 🎉"
                        : "Outstanding! Perfect score on your first try! 🏆",
                    ImagePath = "celebrations/perfect_first_try.gif",
                    ShouldShowImmediately = true
                };
            }

            var streakStatus = await TrackLearningStreaksAsync(childId, cancellationToken);
            if (streakStatus.CurrentStreak >= 5)
            {
                return new MotivationalContent
                {
                    MessageType = "Milestone",
                    Title = $"🔥 {streakStatus.CurrentStreak} Day Streak! 🔥",
                    Message = "You're on fire! Keep up the amazing learning!",
                    ImagePath = "celebrations/learning_streak.gif",
                    ShouldShowImmediately = false,
                    ShowAfterDelay = TimeSpan.FromSeconds(3)
                };
            }

            return new MotivationalContent
            {
                MessageType = "Encouragement",
                Title = "Great Job! 👏",
                Message = "You're doing wonderful! Keep learning and having fun!",
                ShouldShowImmediately = false
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating motivational content for child {ChildId}", childId);
            return new MotivationalContent { MessageType = "Error", Message = "Error generating content" };
        }
    }

    #endregion

    #region Helper Methods

    private int CalculateRecommendationScore(Activity activity, Child child, IEnumerable<int> completedActivityIds)
    {
        var score = 100;

        // Age appropriateness boost
        if (activity.IsAgeAppropriate(child.Age)) score += 20;

        // Difficulty progression
        // TODO: Consider child's performance level

        // Sequence order (prefer earlier activities)
        score -= activity.DisplayOrder;

        // Crown challenge penalty for regular flow
        if (activity.IsCrownChallenge) score -= 50;

        return Math.Max(0, score);
    }

    private DateTime? EstimateUnlockDate(int childId, int remainingRequirements)
    {
        // Simple estimation: 1 activity per day
        return DateTime.UtcNow.AddDays(remainingRequirements);
    }

    private string GenerateCompletionCelebration(ActivityCompletionData completion, int newlyUnlockedCount)
    {
        var message = completion.StarsEarned switch
        {
            3 => "🌟 Perfect! Amazing work! 🌟",
            2 => "⭐ Great job! Excellent! ⭐",
            1 => "⭐ Good effort! Keep learning! ⭐",
            _ => "Nice try! Keep practicing! 😊"
        };

        if (newlyUnlockedCount > 0)
        {
            message += $" Plus {newlyUnlockedCount} new activities unlocked! 🎉";
        }

        return message;
    }

    private List<string> ExtractLearningObjectives(Activity activity)
    {
        if (string.IsNullOrEmpty(activity.LearningObjectives))
            return new List<string>();

        try
        {
            return JsonSerializer.Deserialize<List<string>>(activity.LearningObjectives) ?? new List<string>();
        }
        catch
        {
            return new List<string> { activity.LearningObjectives };
        }
    }

    private Dictionary<string, object> ExtractPathLearningObjectives(List<PathActivity> pathActivities)
    {
        var objectives = new Dictionary<string, object>();
        var allObjectives = pathActivities.SelectMany(pa => pa.LearningObjectives).Distinct().ToList();

        objectives["primaryObjectives"] = allObjectives.Take(5).ToList();
        objectives["totalObjectives"] = allObjectives.Count;
        objectives["skillAreas"] = pathActivities.Select(pa => pa.Activity.ActivityType).Distinct().ToList();

        return objectives;
    }

    private string GeneratePathReasoning(Child child, double averageStars, int activityCount)
    {
        var performance = averageStars >= 2.5 ? "excellent" : averageStars >= 2.0 ? "good" : "developing";
        return $"Personalized path for {child.Name} based on {performance} recent performance. " +
               $"Includes {activityCount} age-appropriate activities to support continued learning.";
    }

    private int EstimateCompletionDays(int activityCount, int childAge)
    {
        var activitiesPerDay = childAge <= 4 ? 1 : childAge <= 6 ? 2 : 3;
        return (int)Math.Ceiling((double)activityCount / activitiesPerDay);
    }

    private async Task<List<int>> GetActivityPrerequisites(int activityId, CancellationToken cancellationToken)
    {
        var activity = await _activityRepository.GetByIdAsync(activityId, cancellationToken);
        if (activity == null || string.IsNullOrEmpty(activity.Prerequisites))
            return new List<int>();

        try
        {
            return JsonSerializer.Deserialize<List<int>>(activity.Prerequisites) ?? new List<int>();
        }
        catch
        {
            return new List<int>();
        }
    }

    private DateTime? EstimateReachDate(int childId, int remainingActivities)
    {
        // Simple estimation: 1 activity per day
        return remainingActivities > 0 ? DateTime.UtcNow.AddDays(remainingActivities) : DateTime.UtcNow;
    }

    private async Task<int> CalculateCurrentStreak(int childId, int? subjectId, CancellationToken cancellationToken)
    {
        var recentProgress = await _progressRepository.GetRecentProgressAsync(childId, 30, cancellationToken);

        if (subjectId.HasValue)
        {
            recentProgress = await _progressRepository.GetSubjectProgressAsync(childId, subjectId.Value, cancellationToken);
        }

        // Calculate consecutive days with activities
        var progressByDay = recentProgress
            .GroupBy(p => p.CreatedAt.Date)
            .OrderByDescending(g => g.Key)
            .ToList();

        var streak = 0;
        var currentDate = DateTime.UtcNow.Date;

        foreach (var dayGroup in progressByDay)
        {
            if (dayGroup.Key == currentDate.AddDays(-streak))
            {
                streak++;
            }
            else
            {
                break;
            }
        }

        return streak;
    }

    private List<string> ExtractMasteredConcepts(List<Activity> completedActivities, double averageStars)
    {
        if (averageStars < 2.5) return new List<string>();

        return completedActivities
            .Where(a => !string.IsNullOrEmpty(a.LearningObjectives))
            .SelectMany(a => ExtractLearningObjectives(a))
            .Distinct()
            .Take(5)
            .ToList();
    }

    private List<string> ExtractConceptsInProgress(List<Activity> allActivities, IEnumerable<int> completedIds)
    {
        return allActivities
            .Where(a => !completedIds.Contains(a.Id) && !string.IsNullOrEmpty(a.LearningObjectives))
            .SelectMany(a => ExtractLearningObjectives(a))
            .Distinct()
            .Take(3)
            .ToList();
    }

    private double CalculateEasyScore(double averageStars, double completionRate)
    {
        // High score for easy if performance is low
        return averageStars < 2.0 ? 0.8 : averageStars < 2.5 ? 0.6 : 0.3;
    }

    private double CalculateMediumScore(double averageStars, double completionRate)
    {
        // High score for medium if performance is moderate
        return averageStars >= 2.0 && averageStars < 2.8 ? 0.8 : 0.5;
    }

    private double CalculateHardScore(double averageStars, double completionRate)
    {
        // High score for hard if performance is high
        return averageStars >= 2.7 && completionRate >= 0.9 ? 0.8 : 0.2;
    }

    private string GenerateDifficultyReasoning(double averageStars, double completionRate, string recommendedDifficulty)
    {
        return $"Based on {averageStars:F1} average stars and {completionRate:P0} completion rate, " +
               $"{recommendedDifficulty} difficulty provides optimal challenge level.";
    }

    private double CalculateActivitySelectionScore(Activity activity, Child child, IEnumerable<UserProgress> recentProgress, ActivitySelectionCriteria criteria)
    {
        var score = 100.0;

        // Age appropriateness
        if (activity.IsAgeAppropriate(child.Age)) score += 20;

        // Difficulty preference
        if (!string.IsNullOrEmpty(criteria.PreferredDifficulty) && activity.DifficultyLevel == criteria.PreferredDifficulty)
            score += 15;

        // Activity type preference
        if (!string.IsNullOrEmpty(criteria.PreferredActivityType) && activity.ActivityType == criteria.PreferredActivityType)
            score += 10;

        // Time constraints
        if (activity.EstimatedMinutes <= criteria.MaxTimeMinutes)
            score += 5;

        return score;
    }

    private string GenerateSelectionReasoning(Activity activity, double score, ActivitySelectionCriteria criteria)
    {
        return $"Score: {score:F1} - {activity.DifficultyLevel} {activity.ActivityType} activity " +
               $"estimated at {activity.EstimatedMinutes} minutes.";
    }

    private List<string> ExtractMasteredConceptsFromProgress(List<UserProgress> progress)
    {
        // Extract concepts from high-performing activities
        return new List<string> { "Number Recognition", "Letter Sounds", "Basic Counting" };
    }

    private List<string> GetRequiredMasteriesForCrown(int subjectId)
    {
        // Return subject-specific mastery requirements
        return subjectId switch
        {
            1 => new List<string> { "Advanced Counting", "Number Patterns", "Basic Addition" }, // Math
            2 => new List<string> { "Letter Recognition", "Phonics", "Sight Words" }, // Reading
            _ => new List<string> { "Pattern Recognition", "Problem Solving" }
        };
    }

    private async Task<Dictionary<string, double>> CalculateSubjectVelocities(int childId, int days, CancellationToken cancellationToken)
    {
        var subjects = await _subjectRepository.GetAllAsync(cancellationToken);
        var velocities = new Dictionary<string, double>();

        foreach (var subject in subjects)
        {
            var subjectProgress = await _progressRepository.GetSubjectProgressAsync(childId, subject.Id, cancellationToken);
            var windowStart = DateTime.UtcNow.AddDays(-days);
            var recentProgress = subjectProgress.Where(p => p.CreatedAt >= windowStart);

            velocities[subject.NameEn] = (double)recentProgress.Count() / days;
        }

        return velocities;
    }

    private DateTime EstimateCurriculumCompletion(double activitiesPerDay)
    {
        // Estimate total curriculum size and calculate completion date
        var estimatedTotalActivities = 200; // Placeholder
        var remainingDays = activitiesPerDay > 0 ? estimatedTotalActivities / activitiesPerDay : 365;
        return DateTime.UtcNow.AddDays(remainingDays);
    }

    private async Task<List<Activity>> GetRemediationActivities(List<LearningGap> gaps, CancellationToken cancellationToken)
    {
        // Return easier activities that target the identified gaps
        return new List<Activity>(); // Placeholder - would implement gap-specific activity selection
    }

    private string GenerateGapAssessment(List<LearningGap> gaps)
    {
        if (!gaps.Any()) return "No significant learning gaps identified. Great progress!";

        var significantGaps = gaps.Count(g => g.Severity == "Significant");
        return significantGaps > 0
            ? $"{significantGaps} areas need focused attention with additional practice."
            : "Minor gaps identified. Regular practice will help strengthen these areas.";
    }

    private async Task<List<Achievement>> GetRecentAchievements(int childId, int days, CancellationToken cancellationToken)
    {
        // Get achievements earned in the last X days
        return new List<Achievement>(); // Placeholder - would query achievement records
    }

    private string GenerateOverallSummary(Dictionary<string, SubjectProgressTracking> subjectProgress, LearningVelocityAnalysis velocity)
    {
        var avgCompletion = subjectProgress.Values.Average(s => s.CompletionPercentage);
        var avgStars = subjectProgress.Values.Average(s => s.AverageStarRating);

        return $"Overall progress: {avgCompletion:F1}% complete with {avgStars:F1} average stars. " +
               $"Learning velocity: {velocity.ActivitiesPerDay:F1} activities per day.";
    }

    private List<string> GenerateParentRecommendations(Dictionary<string, SubjectProgressTracking> subjectProgress, LearningVelocityAnalysis velocity)
    {
        var recommendations = new List<string>();

        var slowestSubject = subjectProgress.OrderBy(kvp => kvp.Value.CompletionPercentage).FirstOrDefault();
        if (slowestSubject.Value != null)
        {
            recommendations.Add($"Consider spending extra time on {slowestSubject.Key} activities.");
        }

        if (velocity.ActivitiesPerDay < 1.0)
        {
            recommendations.Add("Try to establish a daily learning routine for consistent progress.");
        }

        if (velocity.VelocityTrend == "Accelerating")
        {
            recommendations.Add("Great momentum! Keep up the excellent learning routine.");
        }

        return recommendations;
    }

    private async Task<int> CalculateLongestStreak(int childId, CancellationToken cancellationToken)
    {
        // Calculate the longest learning streak ever achieved
        return 7; // Placeholder - would implement full streak calculation
    }

    private int CalculateDaysToNextMilestone(int currentStreak)
    {
        var milestones = new[] { 3, 7, 14, 30, 60 };
        var nextMilestone = milestones.FirstOrDefault(m => m > currentStreak);
        return nextMilestone > 0 ? nextMilestone - currentStreak : 0;
    }

    #endregion
}