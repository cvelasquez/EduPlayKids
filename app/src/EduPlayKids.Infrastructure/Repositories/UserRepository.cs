using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using EduPlayKids.Application.Interfaces.Repositories;
using EduPlayKids.Domain.Entities;
using EduPlayKids.Infrastructure.Data.Context;

namespace EduPlayKids.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for User entity operations.
/// Handles parent/guardian authentication, profile management, and premium subscription features.
/// Implements COPPA-compliant parent verification and child safety requirements.
/// </summary>
public class UserRepository : GenericRepository<User>, IUserRepository
{
    /// <summary>
    /// Initializes a new instance of the UserRepository class.
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="logger">Logger for error handling and debugging</param>
    public UserRepository(EduPlayKidsDbContext context, ILogger<UserRepository> logger)
        : base(context, logger)
    {
    }

    #region Authentication and Security

    /// <inheritdoc />
    public async Task<User?> ValidateCredentialsAsync(string email, string passwordHash, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(email);
            ArgumentException.ThrowIfNullOrWhiteSpace(passwordHash);

            _logger.LogDebug("Validating credentials for email: {Email}", email);

            var user = await _dbSet
                .Where(u => !u.IsDeleted && u.Email == email && u.PasswordHash == passwordHash)
                .FirstOrDefaultAsync(cancellationToken);

            if (user != null)
            {
                _logger.LogDebug("Credentials validated successfully for user ID: {UserId}", user.Id);

                // Update last login time
                await UpdateLastLoginAsync(user.Id, DateTime.UtcNow, cancellationToken);
            }
            else
            {
                _logger.LogWarning("Invalid credentials for email: {Email}", email);
            }

            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating credentials for email: {Email}", email);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<bool> ValidateParentalPinAsync(int userId, string pin, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(pin);

            _logger.LogDebug("Validating parental PIN for user ID: {UserId}", userId);

            var user = await GetByIdAsync(userId, cancellationToken);

            if (user?.ValidateParentalPin(pin) == true)
            {
                _logger.LogDebug("Parental PIN validated successfully for user ID: {UserId}", userId);
                return true;
            }

            _logger.LogWarning("Invalid parental PIN for user ID: {UserId}", userId);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating parental PIN for user ID: {UserId}", userId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(email);

            _logger.LogDebug("Getting user by email: {Email}", email);

            return await _dbSet
                .Where(u => !u.IsDeleted && u.Email == email)
                .FirstOrDefaultAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by email: {Email}", email);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task UpdateLastLoginAsync(int userId, DateTime loginTime, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Updating last login for user ID: {UserId}", userId);

            var user = await GetByIdAsync(userId, cancellationToken);
            if (user != null)
            {
                user.LastLoginAt = loginTime;
                user.UpdatedAt = DateTime.UtcNow;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating last login for user ID: {UserId}", userId);
            throw;
        }
    }

    #endregion

    #region Multi-Child Family Management

    /// <inheritdoc />
    public async Task<User?> GetWithChildrenAsync(int userId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting user with children for user ID: {UserId}", userId);

            return await _dbSet
                .Where(u => !u.IsDeleted && u.Id == userId)
                .Include(u => u.Children.Where(c => !c.IsDeleted))
                .FirstOrDefaultAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user with children for user ID: {UserId}", userId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<User>> GetUsersWithChildrenInAgeRangeAsync(int minAge, int maxAge, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting users with children in age range: {MinAge}-{MaxAge}", minAge, maxAge);

            return await _dbSet
                .Where(u => !u.IsDeleted && u.Children.Any(c => !c.IsDeleted && c.Age >= minAge && c.Age <= maxAge))
                .Include(u => u.Children.Where(c => !c.IsDeleted && c.Age >= minAge && c.Age <= maxAge))
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting users with children in age range: {MinAge}-{MaxAge}", minAge, maxAge);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<int> GetChildrenCountAsync(int userId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting children count for user ID: {UserId}", userId);

            return await _context.Set<Child>()
                .Where(c => !c.IsDeleted && c.UserId == userId)
                .CountAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting children count for user ID: {UserId}", userId);
            throw;
        }
    }

    #endregion

    #region Premium Subscription Management

    /// <inheritdoc />
    public async Task<IEnumerable<User>> GetUsersInFreeTrialAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting users in free trial");

            var currentTime = DateTime.UtcNow;
            return await _dbSet
                .Where(u => !u.IsDeleted && !u.IsPremium && u.FreeTrialExpiresAt > currentTime)
                .OrderBy(u => u.FreeTrialExpiresAt)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting users in free trial");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<User>> GetUsersWithTrialExpiringAsync(int daysFromNow, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting users with trial expiring in {Days} days", daysFromNow);

            var currentTime = DateTime.UtcNow;
            var expirationThreshold = currentTime.AddDays(daysFromNow);

            return await _dbSet
                .Where(u => !u.IsDeleted &&
                           !u.IsPremium &&
                           u.FreeTrialExpiresAt > currentTime &&
                           u.FreeTrialExpiresAt <= expirationThreshold)
                .OrderBy(u => u.FreeTrialExpiresAt)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting users with trial expiring in {Days} days", daysFromNow);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<User>> GetPremiumUsersAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting premium users");

            var currentTime = DateTime.UtcNow;
            return await _dbSet
                .Where(u => !u.IsDeleted &&
                           u.IsPremium &&
                           (u.PremiumExpiresAt == null || u.PremiumExpiresAt > currentTime))
                .OrderBy(u => u.PremiumExpiresAt ?? DateTime.MaxValue)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting premium users");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<User>> GetUsersWithSubscriptionExpiringAsync(int daysFromNow, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting users with subscription expiring in {Days} days", daysFromNow);

            var currentTime = DateTime.UtcNow;
            var expirationThreshold = currentTime.AddDays(daysFromNow);

            return await _dbSet
                .Where(u => !u.IsDeleted &&
                           u.IsPremium &&
                           u.PremiumExpiresAt != null &&
                           u.PremiumExpiresAt > currentTime &&
                           u.PremiumExpiresAt <= expirationThreshold)
                .OrderBy(u => u.PremiumExpiresAt)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting users with subscription expiring in {Days} days", daysFromNow);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task UpdatePremiumStatusAsync(int userId, bool isPremium, DateTime? expiresAt, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Updating premium status for user ID: {UserId}, Premium: {IsPremium}", userId, isPremium);

            var user = await GetByIdAsync(userId, cancellationToken);
            if (user != null)
            {
                user.IsPremium = isPremium;
                user.PremiumExpiresAt = expiresAt;
                user.UpdatedAt = DateTime.UtcNow;

                _logger.LogDebug("Premium status updated successfully for user ID: {UserId}", userId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating premium status for user ID: {UserId}", userId);
            throw;
        }
    }

    #endregion

    #region Parental Controls and Safety

    /// <inheritdoc />
    public async Task<IEnumerable<User>> GetUsersWithParentalControlsEnabledAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting users with parental controls enabled");

            return await _dbSet
                .Where(u => !u.IsDeleted && u.ParentalControlsEnabled)
                .OrderBy(u => u.CreatedAt)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting users with parental controls enabled");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task UpdateDailyUsageLimitAsync(int userId, int dailyLimitMinutes, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Updating daily usage limit for user ID: {UserId}, Limit: {Limit} minutes", userId, dailyLimitMinutes);

            var user = await GetByIdAsync(userId, cancellationToken);
            if (user != null)
            {
                user.DailyUsageLimitMinutes = dailyLimitMinutes;
                user.UpdatedAt = DateTime.UtcNow;

                _logger.LogDebug("Daily usage limit updated successfully for user ID: {UserId}", userId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating daily usage limit for user ID: {UserId}", userId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task UpdateParentalPinAsync(int userId, string newPin, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(newPin);

            _logger.LogDebug("Updating parental PIN for user ID: {UserId}", userId);

            var user = await GetByIdAsync(userId, cancellationToken);
            if (user != null)
            {
                user.ParentalPin = newPin;
                user.UpdatedAt = DateTime.UtcNow;

                _logger.LogDebug("Parental PIN updated successfully for user ID: {UserId}", userId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating parental PIN for user ID: {UserId}", userId);
            throw;
        }
    }

    #endregion

    #region Localization and Regional Support

    /// <inheritdoc />
    public async Task<IEnumerable<User>> GetUsersByLanguageAsync(string language, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(language);

            _logger.LogDebug("Getting users by language: {Language}", language);

            return await _dbSet
                .Where(u => !u.IsDeleted && u.PreferredLanguage == language)
                .OrderBy(u => u.CreatedAt)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting users by language: {Language}", language);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<User>> GetUsersByCountryAsync(string countryCode, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(countryCode);

            _logger.LogDebug("Getting users by country: {CountryCode}", countryCode);

            return await _dbSet
                .Where(u => !u.IsDeleted && u.CountryCode == countryCode)
                .OrderBy(u => u.CreatedAt)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting users by country: {CountryCode}", countryCode);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task UpdateLocalizationSettingsAsync(int userId, string language, string? countryCode = null, string? timeZone = null, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(language);

            _logger.LogDebug("Updating localization settings for user ID: {UserId}", userId);

            var user = await GetByIdAsync(userId, cancellationToken);
            if (user != null)
            {
                user.PreferredLanguage = language;
                if (countryCode != null) user.CountryCode = countryCode;
                if (timeZone != null) user.TimeZone = timeZone;
                user.UpdatedAt = DateTime.UtcNow;

                _logger.LogDebug("Localization settings updated successfully for user ID: {UserId}", userId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating localization settings for user ID: {UserId}", userId);
            throw;
        }
    }

    #endregion

    #region Analytics and Reporting

    /// <inheritdoc />
    public async Task<IEnumerable<User>> GetNewUsersInPeriodAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting new users in period: {FromDate} to {ToDate}", fromDate, toDate);

            return await _dbSet
                .Where(u => !u.IsDeleted && u.CreatedAt >= fromDate && u.CreatedAt <= toDate)
                .OrderBy(u => u.CreatedAt)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting new users in period: {FromDate} to {ToDate}", fromDate, toDate);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<User>> GetInactiveUsersAsync(int daysInactive, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting inactive users: {DaysInactive} days", daysInactive);

            var inactiveThreshold = DateTime.UtcNow.AddDays(-daysInactive);

            return await _dbSet
                .Where(u => !u.IsDeleted &&
                           (u.LastLoginAt == null || u.LastLoginAt <= inactiveThreshold))
                .OrderBy(u => u.LastLoginAt ?? u.CreatedAt)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting inactive users: {DaysInactive} days", daysInactive);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<Dictionary<string, int>> GetUserTypeStatisticsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting user type statistics");

            var currentTime = DateTime.UtcNow;

            var statistics = new Dictionary<string, int>
            {
                ["Total"] = await _dbSet.Where(u => !u.IsDeleted).CountAsync(cancellationToken),
                ["FreeTrial"] = await _dbSet.Where(u => !u.IsDeleted && !u.IsPremium && u.FreeTrialExpiresAt > currentTime).CountAsync(cancellationToken),
                ["TrialExpired"] = await _dbSet.Where(u => !u.IsDeleted && !u.IsPremium && u.FreeTrialExpiresAt <= currentTime).CountAsync(cancellationToken),
                ["Premium"] = await _dbSet.Where(u => !u.IsDeleted && u.IsPremium && (u.PremiumExpiresAt == null || u.PremiumExpiresAt > currentTime)).CountAsync(cancellationToken),
                ["PremiumExpired"] = await _dbSet.Where(u => !u.IsDeleted && u.IsPremium && u.PremiumExpiresAt != null && u.PremiumExpiresAt <= currentTime).CountAsync(cancellationToken)
            };

            _logger.LogDebug("User type statistics retrieved successfully");
            return statistics;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user type statistics");
            throw;
        }
    }

    #endregion
}