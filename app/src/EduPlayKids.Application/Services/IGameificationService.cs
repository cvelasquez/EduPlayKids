namespace EduPlayKids.Application.Services;

/// <summary>
/// Service interface for managing gamification elements, achievements, and motivational systems.
/// Handles crown challenges, learning streaks, and child-safe recognition mechanics.
/// Essential for maintaining engagement and motivation in educational activities.
/// </summary>
public interface IGameificationService
{
    #region Achievement Management

    /// <summary>
    /// Evaluates and awards achievements for a child based on their recent activities.
    /// Checks all applicable achievement criteria and awards earned recognitions.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="triggeredByActivity">Optional activity that triggered the evaluation</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of newly awarded achievements with celebration details</returns>
    Task<IEnumerable<Dictionary<string, object>>> EvaluateAndAwardAchievementsAsync(int childId, int? triggeredByActivity = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates custom achievement milestones for individual children.
    /// Allows for personalized recognition based on unique learning goals and challenges.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="achievementName">Name of the custom achievement</param>
    /// <param name="criteria">Achievement criteria and requirements</param>
    /// <param name="celebrationMessage">Custom celebration message</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The created custom achievement with validation status</returns>
    Task<Dictionary<string, object>> CreateCustomAchievementAsync(int childId, string achievementName, Dictionary<string, object> criteria, string celebrationMessage, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets achievement progress for a child showing how close they are to earning specific recognitions.
    /// Provides motivation through progress visualization and next-step guidance.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="achievementCategory">Optional category filter</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of achievements with progress percentages and requirements</returns>
    Task<IEnumerable<Dictionary<string, object>>> GetAchievementProgressAsync(int childId, string? achievementCategory = null, CancellationToken cancellationToken = default);

    #endregion

    #region Crown Challenge System

    /// <summary>
    /// Evaluates child's eligibility for crown challenges based on performance excellence.
    /// Crown challenges are advanced activities for consistently high-performing children.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="subjectId">Optional subject filter for crown challenge evaluation</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Eligibility status with available crown challenges and requirements</returns>
    Task<Dictionary<string, object>> EvaluateCrownChallengeEligibilityAsync(int childId, int? subjectId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates personalized crown challenges for exceptional performers.
    /// Creates adaptive difficulty content that provides appropriate challenge levels.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="subjectId">The subject for crown challenge generation</param>
    /// <param name="difficultyMultiplier">Additional difficulty scaling factor</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Generated crown challenge with adaptive content and special recognition</returns>
    Task<Dictionary<string, object>> GeneratePersonalizedCrownChallengeAsync(int childId, int subjectId, double difficultyMultiplier = 1.2, CancellationToken cancellationToken = default);

    /// <summary>
    /// Tracks crown challenge completion and awards special recognition.
    /// Provides elite-level celebration and motivation for exceptional achievements.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="crownChallengeId">The crown challenge identifier</param>
    /// <param name="completionDetails">Details about how the challenge was completed</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Crown completion results with special recognition and rewards</returns>
    Task<Dictionary<string, object>> CompleteCrownChallengeAsync(int childId, int crownChallengeId, Dictionary<string, object> completionDetails, CancellationToken cancellationToken = default);

    #endregion

    #region Learning Streak Management

    /// <summary>
    /// Calculates and updates learning streaks for consistent engagement recognition.
    /// Tracks daily learning activities and maintains motivation through streak visualization.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="activityDate">Date of learning activity (default: today)</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Updated streak information with milestone recognition</returns>
    Task<Dictionary<string, object>> UpdateLearningStreakAsync(int childId, DateTime? activityDate = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets comprehensive streak analytics for motivation and pattern analysis.
    /// Includes current streak, longest streak, and streak-related achievements.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Detailed streak analytics with motivational insights</returns>
    Task<Dictionary<string, object>> GetStreakAnalyticsAsync(int childId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Provides streak recovery suggestions when streaks are broken.
    /// Helps maintain motivation and provides gentle encouragement to resume learning.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Streak recovery plan with motivational content and easy re-entry activities</returns>
    Task<Dictionary<string, object>> GenerateStreakRecoveryPlanAsync(int childId, CancellationToken cancellationToken = default);

    #endregion

    #region Star Rating and Performance Recognition

    /// <summary>
    /// Calculates star ratings for activities based on performance criteria.
    /// Implements the 3-star system: 3 stars (no errors), 2 stars (1-2 errors), 1 star (3+ errors).
    /// </summary>
    /// <param name="totalQuestions">Total number of questions in the activity</param>
    /// <param name="correctAnswers">Number of correct answers</param>
    /// <param name="timeSpentMinutes">Time spent on the activity</param>
    /// <param name="expectedTimeMinutes">Expected completion time</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Star rating calculation with detailed breakdown and feedback</returns>
    Task<Dictionary<string, object>> CalculateStarRatingAsync(int totalQuestions, int correctAnswers, int timeSpentMinutes, int expectedTimeMinutes, CancellationToken cancellationToken = default);

    /// <summary>
    /// Analyzes star rating patterns for performance insights and motivation optimization.
    /// Identifies consistent high performance and areas needing encouragement.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="analysisWindowDays">Number of days to analyze</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Star rating analysis with performance trends and motivational recommendations</returns>
    Task<Dictionary<string, object>> AnalyzeStarRatingPatternsAsync(int childId, int analysisWindowDays = 30, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates celebratory content for excellent star rating achievements.
    /// Creates personalized celebration messages and visual feedback for high performance.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="starsEarned">Number of stars earned</param>
    /// <param name="activityType">Type of activity completed</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Celebration content with personalized messages and visual elements</returns>
    Task<Dictionary<string, object>> GeneratePerformanceCelebrationAsync(int childId, int starsEarned, string activityType, CancellationToken cancellationToken = default);

    #endregion

    #region Motivational Content and Encouragement

    /// <summary>
    /// Generates personalized motivational content based on child's current state and progress.
    /// Provides appropriate encouragement, celebration, or gentle motivation as needed.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="motivationContext">Context for motivation (completion, struggle, return, etc.)</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Personalized motivational content with age-appropriate messaging</returns>
    Task<Dictionary<string, object>> GenerateMotivationalContentAsync(int childId, string motivationContext, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates encouraging messages for children who are struggling or need support.
    /// Provides positive reinforcement and suggests easier activities to build confidence.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="strugglingAreas">Areas where the child is experiencing difficulty</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Encouraging content with confidence-building suggestions</returns>
    Task<Dictionary<string, object>> GenerateEncouragementContentAsync(int childId, IEnumerable<string> strugglingAreas, CancellationToken cancellationToken = default);

    /// <summary>
    /// Provides celebration content for significant milestones and achievements.
    /// Creates special recognition for important learning accomplishments.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="milestoneType">Type of milestone achieved</param>
    /// <param name="milestoneDetails">Specific details about the milestone</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Milestone celebration content with special recognition elements</returns>
    Task<Dictionary<string, object>> CreateMilestoneCelebrationAsync(int childId, string milestoneType, Dictionary<string, object> milestoneDetails, CancellationToken cancellationToken = default);

    #endregion

    #region Family and Social Gamification

    /// <summary>
    /// Creates family-friendly competitive elements that encourage healthy learning competition.
    /// Maintains child safety while providing family engagement opportunities.
    /// </summary>
    /// <param name="userId">The parent/guardian user's identifier</param>
    /// <param name="competitionType">Type of family competition (weekly goals, achievements, etc.)</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Family competition setup with safe, encouraging competitive elements</returns>
    Task<Dictionary<string, object>> CreateFamilyCompetitionAsync(int userId, string competitionType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates family achievement summaries for parental engagement and celebration.
    /// Provides shared accomplishment recognition while maintaining individual privacy.
    /// </summary>
    /// <param name="userId">The parent/guardian user's identifier</param>
    /// <param name="summaryPeriodDays">Period for achievement summary</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Family achievement summary with shared celebration opportunities</returns>
    Task<Dictionary<string, object>> GenerateFamilyAchievementSummaryAsync(int userId, int summaryPeriodDays = 7, CancellationToken cancellationToken = default);

    #endregion

    #region Engagement Analytics and Optimization

    /// <summary>
    /// Analyzes gamification effectiveness and engagement metrics.
    /// Identifies which motivational elements work best for individual children.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="analysisWindowDays">Number of days to analyze</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Engagement analysis with personalized gamification optimization recommendations</returns>
    Task<Dictionary<string, object>> AnalyzeEngagementEffectivenessAsync(int childId, int analysisWindowDays = 30, CancellationToken cancellationToken = default);

    /// <summary>
    /// Optimizes gamification settings for individual children based on their response patterns.
    /// Adjusts achievement frequency, celebration intensity, and challenge levels.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="optimizationGoals">Specific goals for gamification optimization</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Optimized gamification settings with personalization rationale</returns>
    Task<Dictionary<string, object>> OptimizeGamificationSettingsAsync(int childId, Dictionary<string, object> optimizationGoals, CancellationToken cancellationToken = default);

    /// <summary>
    /// Identifies children who may be over-gamified or under-motivated.
    /// Helps balance gamification elements to maintain healthy learning motivation.
    /// </summary>
    /// <param name="engagementThresholds">Thresholds for identifying engagement issues</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of children with gamification adjustment recommendations</returns>
    Task<IEnumerable<Dictionary<string, object>>> IdentifyGamificationAdjustmentNeedsAsync(Dictionary<string, double> engagementThresholds, CancellationToken cancellationToken = default);

    #endregion

    #region Seasonal and Special Event Gamification

    /// <summary>
    /// Creates seasonal achievements and special event recognition.
    /// Provides timely, culturally appropriate celebration opportunities.
    /// </summary>
    /// <param name="seasonOrEvent">Season or special event identifier</param>
    /// <param name="targetAgeGroups">Age groups for the seasonal content</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Seasonal gamification content with time-limited special recognition</returns>
    Task<Dictionary<string, object>> CreateSeasonalGamificationContentAsync(string seasonOrEvent, IEnumerable<string> targetAgeGroups, CancellationToken cancellationToken = default);

    /// <summary>
    /// Manages time-limited challenges and special events.
    /// Creates excitement through limited-time recognition opportunities.
    /// </summary>
    /// <param name="eventName">Name of the special event</param>
    /// <param name="eventDuration">Duration of the event</param>
    /// <param name="specialRewards">Special rewards available during the event</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Special event configuration with time-limited challenges and rewards</returns>
    Task<Dictionary<string, object>> CreateSpecialEventChallengeAsync(string eventName, TimeSpan eventDuration, Dictionary<string, object> specialRewards, CancellationToken cancellationToken = default);

    #endregion
}