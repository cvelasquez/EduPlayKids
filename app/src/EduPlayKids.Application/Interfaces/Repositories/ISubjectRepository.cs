using EduPlayKids.Domain.Entities;

namespace EduPlayKids.Application.Interfaces.Repositories;

/// <summary>
/// Repository interface for Subject entity operations.
/// Manages educational subjects, bilingual content, and curriculum organization.
/// Supports Spanish/English localization and age-appropriate subject categorization.
/// </summary>
public interface ISubjectRepository : IGenericRepository<Subject>
{
    #region Bilingual Content Management

    /// <summary>
    /// Gets subjects filtered by language for bilingual content delivery.
    /// Essential for Spanish/English content switching based on user preferences.
    /// </summary>
    /// <param name="language">The language code ("es" for Spanish, "en" for English)</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of subjects in the specified language</returns>
    Task<IEnumerable<Subject>> GetSubjectsByLanguageAsync(string language, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a subject with all its available language versions.
    /// Used for language switching functionality and multilingual content management.
    /// </summary>
    /// <param name="subjectCode">The subject's unique code identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of subject versions in different languages</returns>
    Task<IEnumerable<Subject>> GetSubjectLanguageVersionsAsync(string subjectCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the subject in the user's preferred language with fallback to default.
    /// Ensures content is always available even if translation is incomplete.
    /// </summary>
    /// <param name="subjectCode">The subject's unique code identifier</param>
    /// <param name="preferredLanguage">The user's preferred language</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The subject in the preferred language or default language</returns>
    Task<Subject?> GetSubjectInPreferredLanguageAsync(string subjectCode, string preferredLanguage, CancellationToken cancellationToken = default);

    #endregion

    #region Age-Appropriate Content Organization

    /// <summary>
    /// Gets subjects appropriate for a specific age range.
    /// Critical for delivering age-appropriate educational content to children 3-8.
    /// </summary>
    /// <param name="minAge">Minimum age for the subject</param>
    /// <param name="maxAge">Maximum age for the subject</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of age-appropriate subjects</returns>
    Task<IEnumerable<Subject>> GetSubjectsByAgeRangeAsync(int minAge, int maxAge, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets subjects specifically designed for Pre-K children (ages 3-4).
    /// Focuses on foundational concepts and basic skill development.
    /// </summary>
    /// <param name="language">The language code for content</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of Pre-K appropriate subjects</returns>
    Task<IEnumerable<Subject>> GetPreKSubjectsAsync(string language = "es", CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets subjects specifically designed for Kindergarten children (age 5).
    /// Focuses on school readiness and transitional learning concepts.
    /// </summary>
    /// <param name="language">The language code for content</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of Kindergarten appropriate subjects</returns>
    Task<IEnumerable<Subject>> GetKindergartenSubjectsAsync(string language = "es", CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets subjects specifically designed for Primary grade children (ages 6-8).
    /// Focuses on advanced learning concepts and curriculum standards alignment.
    /// </summary>
    /// <param name="language">The language code for content</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of Primary grade appropriate subjects</returns>
    Task<IEnumerable<Subject>> GetPrimaryGradeSubjectsAsync(string language = "es", CancellationToken cancellationToken = default);

    #endregion

    #region Curriculum Standards Alignment

    /// <summary>
    /// Gets subjects aligned with specific curriculum standards.
    /// Important for meeting educational requirements and learning objectives.
    /// </summary>
    /// <param name="curriculumStandard">The curriculum standard identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of subjects aligned with the standard</returns>
    Task<IEnumerable<Subject>> GetSubjectsByCurriculumStandardAsync(string curriculumStandard, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets core academic subjects (Mathematics, Reading, Science, etc.).
    /// Used for prioritizing essential educational content areas.
    /// </summary>
    /// <param name="language">The language code for content</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of core academic subjects</returns>
    Task<IEnumerable<Subject>> GetCoreAcademicSubjectsAsync(string language = "es", CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets enrichment subjects (Art, Music, Physical Education, etc.).
    /// Used for supplementary educational experiences and skill development.
    /// </summary>
    /// <param name="language">The language code for content</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of enrichment subjects</returns>
    Task<IEnumerable<Subject>> GetEnrichmentSubjectsAsync(string language = "es", CancellationToken cancellationToken = default);

    #endregion

    #region Subject Completion and Progress Tracking

    /// <summary>
    /// Gets subjects with their associated activities for complete subject loading.
    /// Optimized for reducing database queries when loading subject content.
    /// </summary>
    /// <param name="language">The language code for content</param>
    /// <param name="ageGroup">Optional age group filter</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of subjects with loaded activities</returns>
    Task<IEnumerable<Subject>> GetSubjectsWithActivitiesAsync(string language = "es", string? ageGroup = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets completion statistics for a subject across all children.
    /// Provides insights into subject engagement and curriculum effectiveness.
    /// </summary>
    /// <param name="subjectId">The subject's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Dictionary containing subject completion statistics</returns>
    Task<Dictionary<string, object>> GetSubjectCompletionStatisticsAsync(int subjectId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the completion rate for a specific subject.
    /// Important metric for understanding curriculum effectiveness and engagement.
    /// </summary>
    /// <param name="subjectId">The subject's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The completion rate as a percentage</returns>
    Task<double> GetSubjectCompletionRateAsync(int subjectId, CancellationToken cancellationToken = default);

    #endregion

    #region Child-Specific Subject Access

    /// <summary>
    /// Gets subjects available to a specific child based on their age and progress.
    /// Implements personalized content filtering and progressive unlocking.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="language">The language code for content</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of subjects available to the child</returns>
    Task<IEnumerable<Subject>> GetAvailableSubjectsForChildAsync(int childId, string language = "es", CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets subjects where a child has made progress.
    /// Used for tracking learning journey and educational engagement.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of subjects with child progress</returns>
    Task<IEnumerable<Subject>> GetSubjectsWithProgressByChildAsync(int childId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the next recommended subject for a child based on their learning path.
    /// Uses adaptive algorithms to suggest appropriate subject progression.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="language">The language code for content</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The recommended next subject for the child</returns>
    Task<Subject?> GetNextRecommendedSubjectForChildAsync(int childId, string language = "es", CancellationToken cancellationToken = default);

    #endregion

    #region Subject Search and Discovery

    /// <summary>
    /// Searches subjects by name or description for content discovery.
    /// Supports parent and educator subject exploration and selection.
    /// </summary>
    /// <param name="searchTerm">The search term to match against subject content</param>
    /// <param name="language">The language code for content</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of subjects matching the search criteria</returns>
    Task<IEnumerable<Subject>> SearchSubjectsAsync(string searchTerm, string language = "es", CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets popular subjects based on activity completion and engagement metrics.
    /// Used for featuring engaging educational content and trending subjects.
    /// </summary>
    /// <param name="language">The language code for content</param>
    /// <param name="topCount">Number of top subjects to return</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of most popular subjects</returns>
    Task<IEnumerable<Subject>> GetPopularSubjectsAsync(string language = "es", int topCount = 10, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets recently updated subjects for featuring fresh content.
    /// Used for highlighting updated educational materials and curriculum improvements.
    /// </summary>
    /// <param name="daysFromNow">Number of days to look back for recent updates</param>
    /// <param name="language">The language code for content</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of recently updated subjects</returns>
    Task<IEnumerable<Subject>> GetRecentlyUpdatedSubjectsAsync(int daysFromNow, string language = "es", CancellationToken cancellationToken = default);

    #endregion

    #region Subject Performance Analysis

    /// <summary>
    /// Gets subjects with high average star ratings for featuring quality content.
    /// Used for showcasing excellent educational subjects and curriculum highlights.
    /// </summary>
    /// <param name="minStarRating">Minimum average star rating</param>
    /// <param name="language">The language code for content</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of highly-rated subjects</returns>
    Task<IEnumerable<Subject>> GetHighRatedSubjectsAsync(double minStarRating, string language = "es", CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets performance analytics for all subjects.
    /// Provides comprehensive overview of curriculum effectiveness and engagement.
    /// </summary>
    /// <param name="language">The language code for content</param>
    /// <param name="ageGroup">Optional age group filter</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Dictionary containing subject performance analytics</returns>
    Task<Dictionary<string, object>> GetSubjectPerformanceAnalyticsAsync(string language = "es", string? ageGroup = null, CancellationToken cancellationToken = default);

    #endregion

    #region Cultural and Regional Adaptations

    /// <summary>
    /// Gets subjects with cultural adaptations for specific regions.
    /// Important for providing culturally relevant content to Hispanic families.
    /// </summary>
    /// <param name="countryCode">The country code for cultural adaptation</param>
    /// <param name="language">The language code for content</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of culturally adapted subjects</returns>
    Task<IEnumerable<Subject>> GetSubjectsByCountryAdaptationAsync(string countryCode, string language = "es", CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets subjects that include bilingual learning components.
    /// Used for language development and bilingual education support.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of bilingual subjects</returns>
    Task<IEnumerable<Subject>> GetBilingualSubjectsAsync(CancellationToken cancellationToken = default);

    #endregion
}