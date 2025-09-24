using EduPlayKids.Domain.Entities;

namespace EduPlayKids.Application.Services;

/// <summary>
/// Service interface for managing educational content progression and unlocking logic.
/// Handles sequential learning paths, prerequisite validation, and adaptive content delivery.
/// </summary>
public interface IContentProgressionService
{
    #region Content Unlocking Logic

    /// <summary>
    /// Determines which activities are unlocked for a specific child based on their progress.
    /// Considers prerequisites, age appropriateness, and premium access.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="subjectId">Optional subject filter for unlocked content</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of unlocked activities with unlock criteria</returns>
    Task<IEnumerable<UnlockedActivityInfo>> GetUnlockedActivitiesAsync(int childId, int? subjectId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a specific activity is unlocked for a child and provides detailed reasoning.
    /// Used for activity access validation and UI state management.
    /// </summary>
    /// <param name="activityId">The activity's unique identifier</param>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Unlock status with detailed reasoning and requirements</returns>
    Task<ActivityUnlockStatus> CheckActivityUnlockStatusAsync(int activityId, int childId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Processes activity completion and updates progression state.
    /// Unlocks new content, triggers achievements, and adjusts learning path.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="activityId">The completed activity's identifier</param>
    /// <param name="completionResults">Activity completion data and performance metrics</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Progression update results with newly unlocked content</returns>
    Task<ProgressionUpdateResult> ProcessActivityCompletionAsync(int childId, int activityId, ActivityCompletionData completionResults, CancellationToken cancellationToken = default);

    #endregion

    #region Learning Path Management

    /// <summary>
    /// Generates a personalized learning path for a child based on their current progress.
    /// Uses adaptive algorithms to recommend optimal activity sequence.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="subjectId">Optional subject focus for the learning path</param>
    /// <param name="pathLength">Number of activities to include in the path</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Ordered sequence of recommended activities with reasoning</returns>
    Task<PersonalizedLearningPath> GenerateLearningPathAsync(int childId, int? subjectId = null, int pathLength = 10, CancellationToken cancellationToken = default);

    /// <summary>
    /// Identifies the next milestone activity for a child in their learning journey.
    /// Focuses on key learning objectives and curriculum progression.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="subjectId">The subject to find the next milestone in</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Next milestone activity with learning objectives and requirements</returns>
    Task<MilestoneActivity?> GetNextMilestoneActivityAsync(int childId, int subjectId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Tracks progress toward curriculum completion for a specific subject.
    /// Provides detailed metrics on learning advancement and remaining objectives.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="subjectId">The subject to track progress for</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Comprehensive progress tracking with completion percentages</returns>
    Task<SubjectProgressTracking> TrackSubjectProgressAsync(int childId, int subjectId, CancellationToken cancellationToken = default);

    #endregion

    #region Adaptive Difficulty and Content Selection

    /// <summary>
    /// Determines optimal difficulty level for new activities based on child's performance.
    /// Analyzes recent completion patterns and adjusts challenge level accordingly.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="activityType">Type of activity to determine difficulty for</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Recommended difficulty level with confidence score and reasoning</returns>
    Task<DifficultyRecommendation> DetermineOptimalDifficultyAsync(int childId, string activityType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Selects the most appropriate next activity considering multiple factors.
    /// Balances learning objectives, difficulty progression, and child preferences.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="availableActivities">Pool of available activities to choose from</param>
    /// <param name="selectionCriteria">Criteria for activity selection</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Best matching activity with selection reasoning</returns>
    Task<ActivitySelectionResult> SelectOptimalNextActivityAsync(int childId, IEnumerable<Activity> availableActivities, ActivitySelectionCriteria selectionCriteria, CancellationToken cancellationToken = default);

    #endregion

    #region Crown Challenges and Advanced Content

    /// <summary>
    /// Evaluates eligibility for crown challenge activities based on mastery levels.
    /// Analyzes performance patterns to determine readiness for advanced content.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="subjectId">Subject to evaluate crown challenge eligibility for</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Crown challenge eligibility with performance analysis</returns>
    Task<CrownChallengeEligibility> EvaluateCrownChallengeEligibilityAsync(int childId, int subjectId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Unlocks crown challenge activities when mastery criteria are met.
    /// Triggers special celebration and motivational messaging.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="subjectId">Subject where crown challenges should be unlocked</param>
    /// <param name="masteryEvidence">Evidence of mastery that triggered the unlock</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Crown challenge unlock results with celebration content</returns>
    Task<CrownChallengeUnlockResult> UnlockCrownChallengesAsync(int childId, int subjectId, MasteryEvidence masteryEvidence, CancellationToken cancellationToken = default);

    #endregion

    #region Progress Analytics and Insights

    /// <summary>
    /// Analyzes learning velocity and predicts completion timelines.
    /// Provides insights into learning pace and projected milestone dates.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="analysisWindowDays">Number of days to analyze for velocity calculation</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Learning velocity analysis with timeline predictions</returns>
    Task<LearningVelocityAnalysis> AnalyzeLearningVelocityAsync(int childId, int analysisWindowDays = 30, CancellationToken cancellationToken = default);

    /// <summary>
    /// Identifies potential learning gaps or areas needing reinforcement.
    /// Analyzes completion patterns to suggest remediation activities.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="subjectId">Optional subject filter for gap analysis</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Learning gap analysis with remediation recommendations</returns>
    Task<LearningGapAnalysis> IdentifyLearningGapsAsync(int childId, int? subjectId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates progress reports for parent dashboard display.
    /// Provides comprehensive insights into child's learning journey.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="reportPeriodDays">Number of days to include in the report</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Detailed progress report with visualizable data</returns>
    Task<ProgressReport> GenerateProgressReportAsync(int childId, int reportPeriodDays = 30, CancellationToken cancellationToken = default);

    #endregion

    #region Motivational Features

    /// <summary>
    /// Tracks learning streaks and consecutive achievement patterns.
    /// Identifies opportunities for motivation and celebration.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Current streak status and motivational opportunities</returns>
    Task<LearningStreakStatus> TrackLearningStreaksAsync(int childId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Determines when to show motivational content and celebrations.
    /// Balances encouragement with authentic achievement recognition.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="recentActivity">Recent activity completion data</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Motivational content recommendations with timing</returns>
    Task<MotivationalContent> GenerateMotivationalContentAsync(int childId, ActivityCompletionData recentActivity, CancellationToken cancellationToken = default);

    #endregion
}

#region Data Models

/// <summary>
/// Information about an unlocked activity with unlock criteria.
/// </summary>
public class UnlockedActivityInfo
{
    public Activity Activity { get; set; } = null!;
    public DateTime UnlockedDate { get; set; }
    public string UnlockReason { get; set; } = string.Empty;
    public bool IsNewlyUnlocked { get; set; }
    public int RecommendationScore { get; set; }
}

/// <summary>
/// Detailed status of activity unlock with requirements.
/// </summary>
public class ActivityUnlockStatus
{
    public int ActivityId { get; set; }
    public bool IsUnlocked { get; set; }
    public string Status { get; set; } = string.Empty; // "Unlocked", "Locked", "Premium", "Age Restricted"
    public List<string> Requirements { get; set; } = new();
    public List<string> CompletedRequirements { get; set; } = new();
    public int ProgressPercentage { get; set; }
    public DateTime? EstimatedUnlockDate { get; set; }
}

/// <summary>
/// Results of processing activity completion for progression.
/// </summary>
public class ProgressionUpdateResult
{
    public int ChildId { get; set; }
    public int CompletedActivityId { get; set; }
    public List<UnlockedActivityInfo> NewlyUnlockedActivities { get; set; } = new();
    public List<Achievement> NewAchievements { get; set; } = new();
    public SubjectProgressTracking UpdatedProgress { get; set; } = null!;
    public bool UnlockedCrownChallenges { get; set; }
    public string CelebrationMessage { get; set; } = string.Empty;
}

/// <summary>
/// Activity completion data for progression processing.
/// </summary>
public class ActivityCompletionData
{
    public int ActivityId { get; set; }
    public int StarsEarned { get; set; }
    public int CorrectAnswers { get; set; }
    public int TotalQuestions { get; set; }
    public int TimeSpentSeconds { get; set; }
    public bool IsFirstCompletion { get; set; }
    public DateTime CompletionTime { get; set; }
    public Dictionary<string, object> PerformanceMetrics { get; set; } = new();
}

/// <summary>
/// Personalized learning path with recommended activities.
/// </summary>
public class PersonalizedLearningPath
{
    public int ChildId { get; set; }
    public List<PathActivity> RecommendedActivities { get; set; } = new();
    public string PathReasoning { get; set; } = string.Empty;
    public Dictionary<string, object> LearningObjectives { get; set; } = new();
    public int EstimatedCompletionDays { get; set; }
    public DateTime GeneratedDate { get; set; }
}

/// <summary>
/// Activity in a learning path with context.
/// </summary>
public class PathActivity
{
    public Activity Activity { get; set; } = null!;
    public int SequenceNumber { get; set; }
    public string RecommendationReason { get; set; } = string.Empty;
    public List<string> LearningObjectives { get; set; } = new();
    public string DifficultyJustification { get; set; } = string.Empty;
}

/// <summary>
/// Milestone activity representing a key learning objective.
/// </summary>
public class MilestoneActivity
{
    public Activity Activity { get; set; } = null!;
    public string MilestoneDescription { get; set; } = string.Empty;
    public List<string> KeyLearningObjectives { get; set; } = new();
    public int RequiredPrerequisites { get; set; }
    public int CompletedPrerequisites { get; set; }
    public DateTime? EstimatedReachDate { get; set; }
}

/// <summary>
/// Subject progress tracking with detailed metrics.
/// </summary>
public class SubjectProgressTracking
{
    public int ChildId { get; set; }
    public int SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public int TotalActivities { get; set; }
    public int CompletedActivities { get; set; }
    public double CompletionPercentage { get; set; }
    public double AverageStarRating { get; set; }
    public int CurrentStreak { get; set; }
    public List<string> MasteredConcepts { get; set; } = new();
    public List<string> ConceptsInProgress { get; set; } = new();
    public DateTime LastActivityDate { get; set; }
}

/// <summary>
/// Difficulty recommendation with confidence scoring.
/// </summary>
public class DifficultyRecommendation
{
    public string RecommendedDifficulty { get; set; } = string.Empty;
    public double ConfidenceScore { get; set; }
    public string Reasoning { get; set; } = string.Empty;
    public Dictionary<string, double> DifficultyScores { get; set; } = new();
    public bool ShouldAdjustFromCurrent { get; set; }
}

/// <summary>
/// Activity selection criteria for optimization.
/// </summary>
public class ActivitySelectionCriteria
{
    public string? PreferredDifficulty { get; set; }
    public string? PreferredActivityType { get; set; }
    public List<string> FocusAreas { get; set; } = new();
    public bool PrioritizeWeakAreas { get; set; } = true;
    public bool AvoidRecentlyCompleted { get; set; } = true;
    public int MaxTimeMinutes { get; set; } = 15;
}

/// <summary>
/// Result of activity selection with reasoning.
/// </summary>
public class ActivitySelectionResult
{
    public Activity? SelectedActivity { get; set; }
    public double SelectionScore { get; set; }
    public string SelectionReasoning { get; set; } = string.Empty;
    public List<ActivityCandidate> AlternativeCandidates { get; set; } = new();
}

/// <summary>
/// Alternative activity candidate with scoring.
/// </summary>
public class ActivityCandidate
{
    public Activity Activity { get; set; } = null!;
    public double Score { get; set; }
    public string Reasoning { get; set; } = string.Empty;
}

/// <summary>
/// Crown challenge eligibility evaluation.
/// </summary>
public class CrownChallengeEligibility
{
    public int ChildId { get; set; }
    public int SubjectId { get; set; }
    public bool IsEligible { get; set; }
    public double MasteryScore { get; set; }
    public List<string> MasteredConcepts { get; set; } = new();
    public List<string> RequiredMasteries { get; set; } = new();
    public string EligibilityReason { get; set; } = string.Empty;
}

/// <summary>
/// Evidence of mastery for crown challenge unlocking.
/// </summary>
public class MasteryEvidence
{
    public List<int> PerfectActivityIds { get; set; } = new();
    public double RecentAverageStars { get; set; }
    public int ConsecutivePerfectActivities { get; set; }
    public List<string> MasteredSkills { get; set; } = new();
    public DateTime EvidenceDate { get; set; }
}

/// <summary>
/// Crown challenge unlock result with celebration.
/// </summary>
public class CrownChallengeUnlockResult
{
    public int ChildId { get; set; }
    public int SubjectId { get; set; }
    public List<Activity> UnlockedChallenges { get; set; } = new();
    public string CelebrationTitle { get; set; } = string.Empty;
    public string CelebrationMessage { get; set; } = string.Empty;
    public string BadgeImagePath { get; set; } = string.Empty;
    public DateTime UnlockDate { get; set; }
}

/// <summary>
/// Learning velocity analysis with predictions.
/// </summary>
public class LearningVelocityAnalysis
{
    public int ChildId { get; set; }
    public double ActivitiesPerDay { get; set; }
    public double AverageStarsPerActivity { get; set; }
    public TimeSpan AverageSessionLength { get; set; }
    public Dictionary<string, double> SubjectVelocities { get; set; } = new();
    public DateTime EstimatedCurriculumCompletion { get; set; }
    public string VelocityTrend { get; set; } = string.Empty; // "Accelerating", "Steady", "Slowing"
}

/// <summary>
/// Learning gap analysis with remediation suggestions.
/// </summary>
public class LearningGapAnalysis
{
    public int ChildId { get; set; }
    public List<LearningGap> IdentifiedGaps { get; set; } = new();
    public List<Activity> RemediationActivities { get; set; } = new();
    public string OverallAssessment { get; set; } = string.Empty;
    public DateTime AnalysisDate { get; set; }
}

/// <summary>
/// Individual learning gap with severity.
/// </summary>
public class LearningGap
{
    public string ConceptName { get; set; } = string.Empty;
    public string GapDescription { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty; // "Minor", "Moderate", "Significant"
    public double PerformanceScore { get; set; }
    public List<int> ProblematicActivityIds { get; set; } = new();
}

/// <summary>
/// Comprehensive progress report for parents.
/// </summary>
public class ProgressReport
{
    public int ChildId { get; set; }
    public DateTime ReportDate { get; set; }
    public int ReportPeriodDays { get; set; }
    public Dictionary<string, SubjectProgressTracking> SubjectProgress { get; set; } = new();
    public LearningVelocityAnalysis VelocityAnalysis { get; set; } = null!;
    public List<Achievement> RecentAchievements { get; set; } = new();
    public string OverallSummary { get; set; } = string.Empty;
    public List<string> ParentRecommendations { get; set; } = new();
}

/// <summary>
/// Learning streak status and motivation.
/// </summary>
public class LearningStreakStatus
{
    public int ChildId { get; set; }
    public int CurrentStreak { get; set; }
    public int LongestStreak { get; set; }
    public DateTime StreakStartDate { get; set; }
    public string StreakType { get; set; } = string.Empty; // "Daily", "Perfect", "Subject"
    public bool IsStreakActive { get; set; }
    public int DaysUntilNextMilestone { get; set; }
}

/// <summary>
/// Motivational content for encouragement.
/// </summary>
public class MotivationalContent
{
    public string MessageType { get; set; } = string.Empty; // "Celebration", "Encouragement", "Milestone"
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? ImagePath { get; set; }
    public string? AudioPath { get; set; }
    public bool ShouldShowImmediately { get; set; }
    public TimeSpan? ShowAfterDelay { get; set; }
}

#endregion