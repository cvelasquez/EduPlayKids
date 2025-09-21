using EduPlayKids.Domain.Entities;

namespace EduPlayKids.Application.Interfaces.Repositories;

/// <summary>
/// Repository interface for AuditLog entity operations.
/// Manages COPPA compliance logging, security monitoring, and data access audit trails.
/// Essential for child privacy protection and regulatory compliance requirements.
/// </summary>
public interface IAuditLogRepository : IGenericRepository<AuditLog>
{
    #region User Activity Logging

    /// <summary>
    /// Gets all audit logs for a specific user for compliance and security monitoring.
    /// Essential for COPPA compliance and data access transparency.
    /// </summary>
    /// <param name="userId">The user's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of audit logs for the user</returns>
    Task<IEnumerable<AuditLog>> GetLogsByUserAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets audit logs for a user within a specific date range.
    /// Used for periodic compliance reports and security analysis.
    /// </summary>
    /// <param name="userId">The user's unique identifier</param>
    /// <param name="fromDate">Start date for the range</param>
    /// <param name="toDate">End date for the range</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of audit logs within the date range</returns>
    Task<IEnumerable<AuditLog>> GetLogsByUserInDateRangeAsync(int userId, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates an audit log entry for user actions and data access.
    /// Central method for maintaining COPPA-compliant activity logging.
    /// </summary>
    /// <param name="userId">The user's unique identifier</param>
    /// <param name="action">The action performed</param>
    /// <param name="entityType">The type of entity affected</param>
    /// <param name="entityId">The identifier of the affected entity</param>
    /// <param name="details">Additional details about the action</param>
    /// <param name="ipAddress">The IP address of the user</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The created audit log entry</returns>
    Task<AuditLog> LogUserActionAsync(int userId, string action, string entityType, string entityId, string? details = null, string? ipAddress = null, CancellationToken cancellationToken = default);

    #endregion

    #region Child Data Access Logging

    /// <summary>
    /// Gets audit logs related to child data access for COPPA compliance.
    /// Critical for tracking all interactions with children's personal information.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of audit logs related to the child</returns>
    Task<IEnumerable<AuditLog>> GetChildDataAccessLogsAsync(int childId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Logs access to child data with detailed information for compliance.
    /// Ensures every access to child information is properly documented.
    /// </summary>
    /// <param name="userId">The user accessing the data</param>
    /// <param name="childId">The child whose data is being accessed</param>
    /// <param name="dataType">The type of child data accessed</param>
    /// <param name="purpose">The purpose of data access</param>
    /// <param name="ipAddress">The IP address of the accessor</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The created audit log entry</returns>
    Task<AuditLog> LogChildDataAccessAsync(int userId, int childId, string dataType, string purpose, string? ipAddress = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets child data modification logs for tracking changes to child information.
    /// Important for maintaining data integrity and compliance with privacy laws.
    /// </summary>
    /// <param name="childId">The child's unique identifier</param>
    /// <param name="fromDate">Start date for the range</param>
    /// <param name="toDate">End date for the range</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of child data modification logs</returns>
    Task<IEnumerable<AuditLog>> GetChildDataModificationLogsAsync(int childId, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);

    #endregion

    #region Security Event Logging

    /// <summary>
    /// Gets security-related audit logs for threat monitoring and analysis.
    /// Includes authentication failures, suspicious activities, and security violations.
    /// </summary>
    /// <param name="fromDate">Start date for analysis</param>
    /// <param name="toDate">End date for analysis</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of security-related audit logs</returns>
    Task<IEnumerable<AuditLog>> GetSecurityLogsAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Logs security events with appropriate severity and detail levels.
    /// Critical for maintaining app security and detecting potential threats.
    /// </summary>
    /// <param name="userId">The user involved in the security event (if applicable)</param>
    /// <param name="securityEvent">The type of security event</param>
    /// <param name="severity">The severity level of the event</param>
    /// <param name="details">Detailed information about the security event</param>
    /// <param name="ipAddress">The IP address associated with the event</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The created security audit log entry</returns>
    Task<AuditLog> LogSecurityEventAsync(int? userId, string securityEvent, string severity, string details, string? ipAddress = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets failed authentication attempts for security monitoring.
    /// Helps identify potential brute force attacks and suspicious login activity.
    /// </summary>
    /// <param name="ipAddress">Optional IP address filter</param>
    /// <param name="timeWindow">Time window in hours to look back</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of failed authentication audit logs</returns>
    Task<IEnumerable<AuditLog>> GetFailedAuthenticationAttemptsAsync(string? ipAddress = null, int timeWindow = 24, CancellationToken cancellationToken = default);

    #endregion

    #region Action Type and Entity Filtering

    /// <summary>
    /// Gets audit logs filtered by action type for specific activity analysis.
    /// Useful for tracking specific operations like CREATE, UPDATE, DELETE, etc.
    /// </summary>
    /// <param name="action">The action type to filter by</param>
    /// <param name="fromDate">Start date for the range</param>
    /// <param name="toDate">End date for the range</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of audit logs for the specified action</returns>
    Task<IEnumerable<AuditLog>> GetLogsByActionAsync(string action, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets audit logs filtered by entity type for data access analysis.
    /// Tracks access patterns to specific types of data (User, Child, Activity, etc.).
    /// </summary>
    /// <param name="entityType">The entity type to filter by</param>
    /// <param name="fromDate">Start date for the range</param>
    /// <param name="toDate">End date for the range</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of audit logs for the specified entity type</returns>
    Task<IEnumerable<AuditLog>> GetLogsByEntityTypeAsync(string entityType, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets audit logs for a specific entity instance.
    /// Provides complete audit trail for individual data records.
    /// </summary>
    /// <param name="entityType">The entity type</param>
    /// <param name="entityId">The entity identifier</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of audit logs for the specific entity</returns>
    Task<IEnumerable<AuditLog>> GetLogsByEntityAsync(string entityType, string entityId, CancellationToken cancellationToken = default);

    #endregion

    #region Compliance and Privacy Reporting

    /// <summary>
    /// Gets COPPA compliance report for a specific user and time period.
    /// Provides comprehensive data access summary for legal compliance.
    /// </summary>
    /// <param name="userId">The user's unique identifier</param>
    /// <param name="fromDate">Start date for the report</param>
    /// <param name="toDate">End date for the report</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Dictionary containing COPPA compliance report data</returns>
    Task<Dictionary<string, object>> GetCOPPAComplianceReportAsync(int userId, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets data retention compliance logs for privacy regulation adherence.
    /// Tracks data lifecycle and deletion activities for GDPR-K compliance.
    /// </summary>
    /// <param name="fromDate">Start date for analysis</param>
    /// <param name="toDate">End date for analysis</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of data retention compliance logs</returns>
    Task<IEnumerable<AuditLog>> GetDataRetentionLogsAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets privacy-related audit logs for GDPR-K and privacy compliance.
    /// Includes consent changes, data exports, and privacy setting modifications.
    /// </summary>
    /// <param name="userId">The user's unique identifier</param>
    /// <param name="fromDate">Start date for the range</param>
    /// <param name="toDate">End date for the range</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of privacy-related audit logs</returns>
    Task<IEnumerable<AuditLog>> GetPrivacyComplianceLogsAsync(int userId, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);

    #endregion

    #region System and Administrative Logging

    /// <summary>
    /// Gets system-level audit logs for administrative and maintenance activities.
    /// Tracks system changes, configuration updates, and administrative actions.
    /// </summary>
    /// <param name="fromDate">Start date for analysis</param>
    /// <param name="toDate">End date for analysis</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of system audit logs</returns>
    Task<IEnumerable<AuditLog>> GetSystemLogsAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Logs system events and administrative actions for transparency.
    /// Maintains audit trail for system changes and maintenance activities.
    /// </summary>
    /// <param name="adminUserId">The administrator performing the action</param>
    /// <param name="systemAction">The system action performed</param>
    /// <param name="affectedSystem">The system component affected</param>
    /// <param name="details">Detailed information about the action</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>The created system audit log entry</returns>
    Task<AuditLog> LogSystemEventAsync(int? adminUserId, string systemAction, string affectedSystem, string details, CancellationToken cancellationToken = default);

    #endregion

    #region Analytics and Monitoring

    /// <summary>
    /// Gets audit log analytics for system monitoring and insights.
    /// Provides statistics on user activity, data access patterns, and system health.
    /// </summary>
    /// <param name="fromDate">Start date for analytics</param>
    /// <param name="toDate">End date for analytics</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Dictionary containing audit log analytics</returns>
    Task<Dictionary<string, object>> GetAuditAnalyticsAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets suspicious activity patterns for security monitoring.
    /// Identifies unusual access patterns and potential security threats.
    /// </summary>
    /// <param name="timeWindow">Time window in hours for pattern analysis</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Dictionary containing suspicious activity analysis</returns>
    Task<Dictionary<string, object>> GetSuspiciousActivityPatternsAsync(int timeWindow = 24, CancellationToken cancellationToken = default);

    #endregion

    #region Log Maintenance and Retention

    /// <summary>
    /// Archives old audit logs according to data retention policies.
    /// Maintains compliance with legal retention requirements while managing storage.
    /// </summary>
    /// <param name="archiveBeforeDate">Date before which logs should be archived</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Number of logs archived</returns>
    Task<int> ArchiveOldLogsAsync(DateTime archiveBeforeDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Permanently deletes archived logs beyond legal retention requirements.
    /// Ensures compliance with data minimization principles and storage optimization.
    /// </summary>
    /// <param name="deleteBeforeDate">Date before which archived logs should be deleted</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Number of logs permanently deleted</returns>
    Task<int> PurgeArchivedLogsAsync(DateTime deleteBeforeDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets audit log storage statistics for capacity planning and optimization.
    /// Provides insights into log volume growth and retention effectiveness.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Dictionary containing storage statistics and metrics</returns>
    Task<Dictionary<string, object>> GetLogStorageStatisticsAsync(CancellationToken cancellationToken = default);

    #endregion

    #region Search and Export Functionality

    /// <summary>
    /// Searches audit logs using flexible criteria for investigations and reports.
    /// Supports complex queries for legal and compliance requirements.
    /// </summary>
    /// <param name="searchCriteria">Dictionary containing search parameters</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Collection of audit logs matching the search criteria</returns>
    Task<IEnumerable<AuditLog>> SearchLogsAsync(Dictionary<string, object> searchCriteria, CancellationToken cancellationToken = default);

    /// <summary>
    /// Exports audit logs in compliance-ready format for legal and regulatory purposes.
    /// Provides structured data export for external audits and compliance reporting.
    /// </summary>
    /// <param name="userId">Optional user filter for export</param>
    /// <param name="fromDate">Start date for export</param>
    /// <param name="toDate">End date for export</param>
    /// <param name="format">Export format (JSON, CSV, XML)</param>
    /// <param name="cancellationToken">Cancellation token for async operation</param>
    /// <returns>Exported audit log data in the specified format</returns>
    Task<string> ExportLogsAsync(int? userId, DateTime fromDate, DateTime toDate, string format = "JSON", CancellationToken cancellationToken = default);

    #endregion
}