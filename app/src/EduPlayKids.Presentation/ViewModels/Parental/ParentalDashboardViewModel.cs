using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EduPlayKids.Application.Interfaces;
using EduPlayKids.Application.Interfaces.Repositories;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace EduPlayKids.App.ViewModels.Parental;

/// <summary>
/// ViewModel for the parental dashboard displaying child progress and controls.
/// Provides comprehensive analytics and management tools for parents.
/// </summary>
public partial class ParentalDashboardViewModel : ObservableObject
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ParentalDashboardViewModel> _logger;
    private Timer? _refreshTimer;

    [ObservableProperty]
    private string _welcomeMessage = "Welcome to Parental Dashboard";

    [ObservableProperty]
    private string _lastAccessMessage = string.Empty;

    [ObservableProperty]
    private ObservableCollection<ChildSummary> _children = new();

    [ObservableProperty]
    private ChildSummary? _selectedChild;

    [ObservableProperty]
    private int _todayUsageMinutes = 0;

    [ObservableProperty]
    private string _usageStatusMessage = string.Empty;

    [ObservableProperty]
    private int _learningStreak = 0;

    [ObservableProperty]
    private string _streakMessage = string.Empty;

    [ObservableProperty]
    private ObservableCollection<SubjectProgressSummary> _subjectProgress = new();

    [ObservableProperty]
    private ObservableCollection<AchievementSummary> _recentAchievements = new();

    [ObservableProperty]
    private int _recentAchievementsCount = 0;

    [ObservableProperty]
    private bool _isLoading = false;

    public ParentalDashboardViewModel(IUnitOfWork unitOfWork, ILogger<ParentalDashboardViewModel> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // Watch for selected child changes
        PropertyChanged += OnPropertyChanged;
    }

    /// <summary>
    /// Initializes the dashboard data.
    /// </summary>
    public async Task InitializeAsync()
    {
        try
        {
            IsLoading = true;
            _logger.LogInformation("Initializing parental dashboard");

            // Load initial data
            await LoadChildrenAsync();
            await LoadWelcomeMessageAsync();

            // Select first child if available
            if (Children.Any() && SelectedChild == null)
            {
                SelectedChild = Children.First();
            }

            // Start auto-refresh timer
            StartAutoRefresh();

            _logger.LogInformation("Parental dashboard initialized successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing parental dashboard");
            throw;
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Loads children data for the current user.
    /// </summary>
    private async Task LoadChildrenAsync()
    {
        try
        {
            _logger.LogDebug("Loading children data");

            // Get current user (simplified - in real app would come from auth service)
            var users = await _unitOfWork.Users.GetAllAsync();
            var currentUser = users.FirstOrDefault();

            if (currentUser == null)
            {
                _logger.LogWarning("No current user found");
                return;
            }

            // Get children for user
            var allChildren = await _unitOfWork.Children.GetAllAsync();
            var children = allChildren.Where(c => c.UserId == currentUser.Id);

            Children.Clear();
            foreach (var child in children)
            {
                Children.Add(new ChildSummary
                {
                    Id = child.Id,
                    Name = child.Name,
                    Age = child.Age,
                    Avatar = GetChildAvatar(child.AvatarImagePath),
                    IsSelected = false
                });
            }

            _logger.LogDebug("Loaded {Count} children", Children.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading children data");
            throw;
        }
    }

    /// <summary>
    /// Loads welcome message and last access information.
    /// </summary>
    private async Task LoadWelcomeMessageAsync()
    {
        try
        {
            var users = await _unitOfWork.Users.GetAllAsync();
            var currentUser = users.FirstOrDefault();

            if (currentUser != null)
            {
                WelcomeMessage = $"Welcome back, {currentUser.Name}!";

                // Get last access from audit logs
                var allAuditLogs = await _unitOfWork.AuditLogs.GetAllAsync();
                var recentLogs = allAuditLogs.Where(a => a.UserId == currentUser.Id);
                var lastParentalAccess = recentLogs
                    .Where(log => log.Action == "PIN Verification" && log.Details?.Contains("successfully") == true)
                    .OrderByDescending(log => log.EventDate)
                    .Skip(1) // Skip current access
                    .FirstOrDefault();

                if (lastParentalAccess != null)
                {
                    LastAccessMessage = $"Last accessed: {lastParentalAccess.EventDate:MMM d, h:mm tt}";
                }
                else
                {
                    LastAccessMessage = "First time accessing parental controls";
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading welcome message");
        }
    }

    /// <summary>
    /// Loads progress data for the selected child.
    /// </summary>
    private async Task LoadChildProgressAsync()
    {
        if (SelectedChild == null) return;

        try
        {
            _logger.LogDebug("Loading progress for child {ChildId}", SelectedChild.Id);

            // Load today's usage
            await LoadTodayUsageAsync();

            // Load learning streak
            await LoadLearningStreakAsync();

            // Load subject progress
            await LoadSubjectProgressAsync();

            // Load recent achievements
            await LoadRecentAchievementsAsync();

            _logger.LogDebug("Progress data loaded for child {ChildId}", SelectedChild.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading child progress for child {ChildId}", SelectedChild.Id);
            throw;
        }
    }

    /// <summary>
    /// Loads today's usage statistics.
    /// </summary>
    private async Task LoadTodayUsageAsync()
    {
        if (SelectedChild == null) return;

        try
        {
            var today = DateTime.Today;
            var sessions = await _unitOfWork.Sessions.GetSessionsByChildAsync(SelectedChild.Id);
            var todaySessions = sessions.Where(s => s.StartedAt.Date == today);

            TodayUsageMinutes = (int)todaySessions.Sum(s => s.DurationMinutes);

            // Get daily limit from settings
            var users = await _unitOfWork.Users.GetAllAsync();
            var currentUser = users.FirstOrDefault();
            var settings = currentUser != null ? await _unitOfWork.Settings.GetByIdAsync(currentUser.Id) : null;
            var dailyLimit = settings?.DailyTimeLimitMinutes ?? 60;

            if (dailyLimit > 0)
            {
                var remaining = Math.Max(0, dailyLimit - TodayUsageMinutes);
                UsageStatusMessage = remaining > 0
                    ? $"{remaining} minutes remaining today"
                    : "Daily limit reached";
            }
            else
            {
                UsageStatusMessage = "No daily limit set";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading today's usage");
        }
    }

    /// <summary>
    /// Loads learning streak information.
    /// </summary>
    private async Task LoadLearningStreakAsync()
    {
        if (SelectedChild == null) return;

        try
        {
            var sessions = await _unitOfWork.Sessions.GetSessionsByChildAsync(SelectedChild.Id);
            var sessionDates = sessions
                .Where(s => s.CompletedAt.HasValue)
                .Select(s => s.StartedAt.Date)
                .Distinct()
                .OrderByDescending(d => d)
                .ToList();

            LearningStreak = CalculateStreak(sessionDates);
            StreakMessage = LearningStreak > 0
                ? $"Great consistency!"
                : "Start a new streak today!";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading learning streak");
        }
    }

    /// <summary>
    /// Loads subject progress overview.
    /// </summary>
    private async Task LoadSubjectProgressAsync()
    {
        if (SelectedChild == null) return;

        try
        {
            var subjects = await _unitOfWork.Subjects.GetAllAsync();
            var allProgress = await _unitOfWork.UserProgress.GetAllAsync();
            var progressData = allProgress.Where(p => p.UserId == SelectedChild.Id);

            SubjectProgress.Clear();

            foreach (var subject in subjects)
            {
                var subjectProgressData = progressData.Where(p => p.Activity.SubjectId == subject.Id);
                var allActivities = await _unitOfWork.Activities.GetAllAsync();
                var activitiesInSubject = allActivities.Where(a => a.SubjectId == subject.Id);

                var completedCount = subjectProgressData.Count(p => p.IsCompleted);
                var totalCount = activitiesInSubject.Count();
                var progressPercentage = totalCount > 0 ? (double)completedCount / totalCount : 0;

                SubjectProgress.Add(new SubjectProgressSummary
                {
                    SubjectName = subject.Name,
                    Icon = GetSubjectIcon(subject.Name),
                    CompletedActivities = completedCount,
                    TotalActivities = totalCount,
                    ProgressPercentage = progressPercentage,
                    CompletionPercentage = (int)(progressPercentage * 100),
                    ProgressText = $"{completedCount} of {totalCount} activities completed"
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading subject progress");
        }
    }

    /// <summary>
    /// Loads recent achievements.
    /// </summary>
    private async Task LoadRecentAchievementsAsync()
    {
        if (SelectedChild == null) return;

        try
        {
            var allUserAchievements = await _unitOfWork.UserAchievements.GetAllAsync();
            var userAchievements = allUserAchievements.Where(ua => ua.UserId == SelectedChild.Id);
            var recentAchievements = userAchievements
                .OrderByDescending(ua => ua.EarnedAt)
                .Take(5)
                .ToList();

            RecentAchievements.Clear();

            foreach (var userAchievement in recentAchievements)
            {
                var achievement = await _unitOfWork.Achievements.GetByIdAsync(userAchievement.AchievementId);
                if (achievement != null)
                {
                    RecentAchievements.Add(new AchievementSummary
                    {
                        Id = achievement.Id,
                        Title = achievement.Title,
                        Icon = GetAchievementIcon(achievement.Type),
                        EarnedDate = userAchievement.EarnedAt
                    });
                }
            }

            RecentAchievementsCount = RecentAchievements.Count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading recent achievements");
        }
    }

    /// <summary>
    /// Opens parental settings.
    /// </summary>
    [RelayCommand]
    private async Task OpenSettingsAsync()
    {
        try
        {
            _logger.LogInformation("Opening parental settings");
            await Shell.Current.GoToAsync("parentalsettings");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error opening settings");
        }
    }

    /// <summary>
    /// Views detailed reports.
    /// </summary>
    [RelayCommand]
    private async Task ViewReportsAsync()
    {
        try
        {
            _logger.LogInformation("Opening detailed reports");
            await Shell.Current.GoToAsync("parentalreports");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error opening reports");
        }
    }

    /// <summary>
    /// Adds a new child profile.
    /// </summary>
    [RelayCommand]
    private async Task AddChildAsync()
    {
        try
        {
            _logger.LogInformation("Adding new child");
            await Shell.Current.GoToAsync("addchild");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding child");
        }
    }

    /// <summary>
    /// Starts auto-refresh timer for real-time updates.
    /// </summary>
    public void StartAutoRefresh()
    {
        StopAutoRefresh();

        _refreshTimer = new Timer(async _ =>
        {
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                try
                {
                    await LoadChildProgressAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during auto-refresh");
                }
            });
        }, null, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));
    }

    /// <summary>
    /// Stops auto-refresh timer.
    /// </summary>
    public void StopAutoRefresh()
    {
        _refreshTimer?.Dispose();
        _refreshTimer = null;
    }

    #region Helper Methods

    private string GetChildAvatar(string? avatarPath)
    {
        if (!string.IsNullOrEmpty(avatarPath))
            return avatarPath;

        // Return default avatar based on gender or random
        var avatars = new[] { "👦", "👧", "🧒", "👶" };
        return avatars[Random.Shared.Next(avatars.Length)];
    }

    private string GetSubjectIcon(string subjectName)
    {
        return subjectName.ToLower() switch
        {
            "mathematics" or "math" => "🔢",
            "reading" or "phonics" => "📚",
            "science" => "🔬",
            "basic concepts" => "🎨",
            "logic" => "🧩",
            _ => "📖"
        };
    }

    private string GetAchievementIcon(string achievementType)
    {
        return achievementType.ToLower() switch
        {
            "completion" => "🏆",
            "streak" => "🔥",
            "perfect" => "⭐",
            "speed" => "⚡",
            "milestone" => "🎯",
            _ => "🏅"
        };
    }

    private int CalculateStreak(List<DateTime> sessionDates)
    {
        if (!sessionDates.Any()) return 0;

        var streak = 0;
        var currentDate = DateTime.Today;

        // Check if there's a session today or yesterday (to account for timezone differences)
        if (!sessionDates.Contains(currentDate) && !sessionDates.Contains(currentDate.AddDays(-1)))
        {
            return 0;
        }

        // Count consecutive days working backwards
        foreach (var date in sessionDates)
        {
            if (date == currentDate.AddDays(-streak))
            {
                streak++;
            }
            else if (date < currentDate.AddDays(-streak))
            {
                break;
            }
        }

        return streak;
    }

    private void OnPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(SelectedChild))
        {
            // Update selection status in UI
            foreach (var child in Children)
            {
                child.IsSelected = child == SelectedChild;
            }

            // Load progress for newly selected child
            if (SelectedChild != null)
            {
                Task.Run(LoadChildProgressAsync);
            }
        }
    }

    #endregion
}

#region Supporting Classes

public partial class ChildSummary : ObservableObject
{
    public int Id { get; set; }

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private int _age;

    [ObservableProperty]
    private string _avatar = "👤";

    [ObservableProperty]
    private bool _isSelected;
}

public class SubjectProgressSummary
{
    public string SubjectName { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public int CompletedActivities { get; set; }
    public int TotalActivities { get; set; }
    public double ProgressPercentage { get; set; }
    public int CompletionPercentage { get; set; }
    public string ProgressText { get; set; } = string.Empty;
}

public class AchievementSummary
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public DateTime EarnedDate { get; set; }
}

#endregion