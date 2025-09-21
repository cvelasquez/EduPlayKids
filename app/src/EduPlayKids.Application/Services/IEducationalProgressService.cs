namespace EduPlayKids.Application.Services;

/// <summary>
/// Service interface for managing complex educational progress operations.
/// Handles learning analytics, adaptive difficulty adjustment, and curriculum progression.
/// Essential for personalized learning experiences and educational effectiveness tracking.
/// </summary>
public interface IEducationalProgressService
{
    #region Learning Progress Management

    /// <summary>
    /// Processes activity completion with comprehensive progress tracking and achievement validation.
    /// Coordinates progress updates, difficulty adjustments, and achievement awards.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="activityId">The activity's unique identifier</param>
    /// <param name="responses">Collection of child's responses to activity questions</param>
    /// <param name="timeSpentMinutes">Total time spent on the activity</param>
    /// <param name="sessionId">The current learning session identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Dictionary containing completion results, star rating, and any achievements earned</returns>
    Task<Dictionary<string, object>> ProcessActivityCompletionAsync(int childId, int activityId, IEnumerable<Dictionary<string, object>> responses, int timeSpentMinutes, int sessionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets personalized learning path for a child based on their progress and performance.
    /// Uses adaptive algorithms to recommend next activities and difficulty levels.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="subjectId">Optional subject filter for targeted recommendations</param>
    /// <param name="maxActivities">Maximum number of activities to recommend</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Ordered collection of recommended activities with reasoning</returns>
    Task<IEnumerable<Dictionary<string, object>>> GetPersonalizedLearningPathAsync(int childId, int? subjectId = null, int maxActivities = 10, CancellationToken cancellationToken = default);

    /// <summary>
    /// Analyzes child's learning patterns and provides detailed progress insights.
    /// Includes strengths, areas for improvement, and learning velocity metrics.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="analysisWindowDays">Number of days to analyze (default 30)</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Comprehensive learning analytics and insights</returns>
    Task<Dictionary<string, object>> AnalyzeLearningPatternsAsync(int childId, int analysisWindowDays = 30, CancellationToken cancellationToken = default);

    #endregion

    #region Adaptive Difficulty Management

    /// <summary>
    /// Calculates optimal difficulty level for a child in a specific subject.
    /// Considers recent performance, error patterns, and learning velocity.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="subjectId">The subject's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Recommended difficulty level with confidence score and reasoning</returns>
    Task<Dictionary<string, object>> CalculateOptimalDifficultyAsync(int childId, int subjectId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adjusts difficulty preferences for a child based on performance data.
    /// Automatically adapts learning content to maintain optimal challenge level.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="performanceThreshold">Performance threshold for difficulty adjustment</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Dictionary containing adjustment results and new difficulty settings</returns>
    Task<Dictionary<string, object>> AdjustDifficultyPreferencesAsync(int childId, double performanceThreshold = 0.75, CancellationToken cancellationToken = default);

    /// <summary>
    /// Identifies children who may need difficulty adjustments based on performance patterns.
    /// Helps proactively optimize learning experiences across the user base.
    /// </summary>
    /// <param name="performanceWindow">Days to analyze for performance patterns</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of children with recommended difficulty adjustments</returns>
    Task<IEnumerable<Dictionary<string, object>>> IdentifyDifficultyAdjustmentCandidatesAsync(int performanceWindow = 14, CancellationToken cancellationToken = default);

    #endregion

    #region Curriculum Progression Tracking

    /// <summary>
    /// Tracks curriculum progression for a child across all subjects.
    /// Provides detailed view of educational milestones and learning objectives completion.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Comprehensive curriculum progression report with completion percentages</returns>
    Task<Dictionary<string, object>> TrackCurriculumProgressionAsync(int childId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Identifies curriculum gaps where a child may need additional practice.
    /// Analyzes learning objectives and recommends targeted activities for improvement.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="gapThreshold">Performance threshold below which gaps are identified</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of identified gaps with recommended remediation activities</returns>
    Task<IEnumerable<Dictionary<string, object>>> IdentifyCurriculumGapsAsync(int childId, double gapThreshold = 0.6, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets next curriculum milestone for a child in a specific subject.
    /// Provides clear learning objectives and activities needed for progression.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="subjectId">The subject's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Next milestone information with required activities and estimated completion time</returns>
    Task<Dictionary<string, object>?> GetNextCurriculumMilestoneAsync(int childId, int subjectId, CancellationToken cancellationToken = default);

    #endregion

    #region Performance Analytics and Insights

    /// <summary>
    /// Generates comprehensive performance report for parent dashboard.
    /// Includes progress summaries, achievement highlights, and learning recommendations.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="reportPeriodDays">Number of days to include in report</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Detailed performance report with visualizable data and insights</returns>
    Task<Dictionary<string, object>> GeneratePerformanceReportAsync(int childId, int reportPeriodDays = 30, CancellationToken cancellationToken = default);

    /// <summary>
    /// Compares child's performance against age-appropriate benchmarks.
    /// Provides context for parents on child's relative progress and achievements.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Benchmark comparison with percentile rankings and developmental insights</returns>
    Task<Dictionary<string, object>> CompareToBenchmarksAsync(int childId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Identifies learning trends and patterns for educational optimization.
    /// Analyzes optimal learning times, session lengths, and content preferences.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="trendAnalysisWindowDays">Days to analyze for trend identification</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Learning trends analysis with optimization recommendations</returns>
    Task<Dictionary<string, object>> IdentifyLearningTrendsAsync(int childId, int trendAnalysisWindowDays = 60, CancellationToken cancellationToken = default);

    #endregion

    #region Family and Multi-Child Analytics

    /// <summary>
    /// Generates family progress overview for multi-child households.
    /// Provides comparative insights while maintaining individual child privacy.
    /// </summary>
    /// <param name="userId">The parent/guardian user's identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Family overview with individual and comparative progress metrics</returns>
    Task<Dictionary<string, object>> GenerateFamilyProgressOverviewAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Identifies sibling learning dynamics and interaction patterns.
    /// Helps optimize learning experiences for children in the same household.
    /// </summary>
    /// <param name="userId">The parent/guardian user's identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Sibling dynamics analysis with recommendations for family learning</returns>
    Task<Dictionary<string, object>> AnalyzeSiblingLearningDynamicsAsync(int userId, CancellationToken cancellationToken = default);

    #endregion

    #region Predictive Analytics and Recommendations

    /// <summary>
    /// Predicts child's learning trajectory based on current progress patterns.
    /// Provides estimated timeline for curriculum completion and skill mastery.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="predictionHorizonDays">Number of days ahead to predict</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Learning trajectory predictions with confidence intervals</returns>
    Task<Dictionary<string, object>> PredictLearningTrajectoryAsync(int childId, int predictionHorizonDays = 90, CancellationToken cancellationToken = default);

    /// <summary>
    /// Recommends optimal learning schedule based on child's performance patterns.
    /// Considers peak learning times, session length preferences, and energy patterns.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Personalized learning schedule recommendations with optimal timing</returns>
    Task<Dictionary<string, object>> RecommendOptimalLearningScheduleAsync(int childId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Identifies at-risk learners who may need additional support or intervention.
    /// Uses performance patterns and engagement metrics to flag potential issues.
    /// </summary>
    /// <param name="riskThreshold">Performance threshold below which children are considered at-risk</param>
    /// <param name="analysisWindowDays">Days to analyze for risk assessment</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of at-risk children with recommended intervention strategies</returns>
    Task<IEnumerable<Dictionary<string, object>>> IdentifyAtRiskLearnersAsync(double riskThreshold = 0.5, int analysisWindowDays = 21, CancellationToken cancellationToken = default);

    #endregion

    #region Content Effectiveness Analysis

    /// <summary>
    /// Analyzes effectiveness of educational content across all children.
    /// Identifies high-performing and problematic activities for content optimization.
    /// </summary>
    /// <param name="analysisWindowDays">Days to analyze for content effectiveness</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Content effectiveness analysis with improvement recommendations</returns>
    Task<Dictionary<string, object>> AnalyzeContentEffectivenessAsync(int analysisWindowDays = 30, CancellationToken cancellationToken = default);

    /// <summary>
    /// Recommends content improvements based on child performance data.
    /// Provides specific suggestions for activity modifications and curriculum adjustments.
    /// </summary>
    /// <param name="activityId">Optional specific activity to analyze</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Content improvement recommendations with prioritization</returns>
    Task<Dictionary<string, object>> RecommendContentImprovementsAsync(int? activityId = null, CancellationToken cancellationToken = default);

    #endregion
}