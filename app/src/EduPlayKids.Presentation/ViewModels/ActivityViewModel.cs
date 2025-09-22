using System.Windows.Input;
using EduPlayKids.App.Services;
using Microsoft.Extensions.Logging;

namespace EduPlayKids.App.ViewModels;

/// <summary>
/// ViewModel for the activity page where children complete educational exercises.
/// This is a foundation implementation that will be expanded with specific activity types.
/// </summary>
public class ActivityViewModel : BaseViewModel
{
    private readonly IChildSafeNavigationService _navigationService;
    private int _childId;
    private int _activityId;
    private string _subjectName = string.Empty;
    private int _currentProgress;
    private int _totalSteps = 5;
    private bool _isCompleted;

    public ActivityViewModel(
        IChildSafeNavigationService navigationService,
        ILogger<ActivityViewModel> logger) : base(logger)
    {
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));

        Title = "Learning Activity";
        BusyText = "Loading your activity...";

        // Initialize commands
        GoBackCommand = new Command(async () => await GoBackAsync());
        NextStepCommand = new Command(async () => await NextStepAsync(), () => CanGoToNextStep());
        CompleteActivityCommand = new Command(async () => await CompleteActivityAsync(), () => CanCompleteActivity());
        GoHomeCommand = new Command(async () => await GoHomeAsync());
    }

    /// <summary>
    /// Gets or sets the child's ID.
    /// </summary>
    public int ChildId
    {
        get => _childId;
        set => SetProperty(ref _childId, value);
    }

    /// <summary>
    /// Gets or sets the activity ID.
    /// </summary>
    public int ActivityId
    {
        get => _activityId;
        set => SetProperty(ref _activityId, value);
    }

    /// <summary>
    /// Gets or sets the subject name for this activity.
    /// </summary>
    public string SubjectName
    {
        get => _subjectName;
        set => SetProperty(ref _subjectName, value);
    }

    /// <summary>
    /// Gets or sets the current progress step (0-based).
    /// </summary>
    public int CurrentProgress
    {
        get => _currentProgress;
        set
        {
            if (SetProperty(ref _currentProgress, value))
            {
                OnPropertyChanged(nameof(ProgressPercentage));
                OnPropertyChanged(nameof(ProgressText));
                ((Command)NextStepCommand).ChangeCanExecute();
                ((Command)CompleteActivityCommand).ChangeCanExecute();
            }
        }
    }

    /// <summary>
    /// Gets or sets the total number of steps in this activity.
    /// </summary>
    public int TotalSteps
    {
        get => _totalSteps;
        set
        {
            if (SetProperty(ref _totalSteps, value))
            {
                OnPropertyChanged(nameof(ProgressPercentage));
                OnPropertyChanged(nameof(ProgressText));
            }
        }
    }

    /// <summary>
    /// Gets or sets whether the activity is completed.
    /// </summary>
    public bool IsCompleted
    {
        get => _isCompleted;
        set
        {
            if (SetProperty(ref _isCompleted, value))
            {
                ((Command)NextStepCommand).ChangeCanExecute();
                ((Command)CompleteActivityCommand).ChangeCanExecute();
                OnPropertyChanged(nameof(CompletionMessage));
            }
        }
    }

    /// <summary>
    /// Gets the progress percentage for the progress bar.
    /// </summary>
    public double ProgressPercentage => TotalSteps > 0 ? (double)CurrentProgress / TotalSteps : 0;

    /// <summary>
    /// Gets the progress text for display.
    /// </summary>
    public string ProgressText => $"Step {Math.Min(CurrentProgress + 1, TotalSteps)} of {TotalSteps}";

    /// <summary>
    /// Gets the completion message when activity is finished.
    /// </summary>
    public string CompletionMessage =>
        IsCompleted ? $"🎉 Amazing work! You completed the {SubjectName} activity! 🌟" : string.Empty;

    /// <summary>
    /// Gets the current step content for display.
    /// </summary>
    public string CurrentStepContent => GetStepContent(CurrentProgress);

    /// <summary>
    /// Command to go back to subject selection.
    /// </summary>
    public ICommand GoBackCommand { get; }

    /// <summary>
    /// Command to proceed to the next step.
    /// </summary>
    public ICommand NextStepCommand { get; }

    /// <summary>
    /// Command to complete the entire activity.
    /// </summary>
    public ICommand CompleteActivityCommand { get; }

    /// <summary>
    /// Command to go back to home.
    /// </summary>
    public ICommand GoHomeCommand { get; }

    /// <summary>
    /// Generates step content based on the current progress.
    /// This is a placeholder implementation that will be replaced with real educational content.
    /// </summary>
    /// <param name="step">The current step number.</param>
    /// <returns>Content for the current step.</returns>
    private string GetStepContent(int step)
    {
        return SubjectName.ToLower() switch
        {
            "mathematics" => step switch
            {
                0 => "Let's count together! 🔢\nCan you count these stars? ⭐⭐⭐",
                1 => "Great job! Now let's add! ➕\nWhat is 2 + 1?",
                2 => "Awesome! Let's learn shapes! 🔺\nCan you find the triangle?",
                3 => "Perfect! More counting! 🎈\nCount these balloons: 🎈🎈🎈🎈🎈",
                4 => "Amazing work! Final challenge! 🏆\nWhat comes after the number 7?",
                _ => "Congratulations! You're a math star! ⭐"
            },
            "reading" => step switch
            {
                0 => "Welcome to reading! 📚\nLet's start with the letter 'A' - Apple! 🍎",
                1 => "Excellent! Now the letter 'B' - Ball! ⚽",
                2 => "Great job! Letter 'C' - Cat! 🐱",
                3 => "Wonderful! Can you spell 'CAT'? 🐱",
                4 => "Perfect! Let's read: 'The cat sat.' 📖",
                _ => "You're becoming a great reader! 📚⭐"
            },
            "basicconcepts" => step switch
            {
                0 => "Let's explore colors! 🌈\nCan you find something RED? 🔴",
                1 => "Fantastic! Now find something BLUE! 🔵",
                2 => "Great! What about YELLOW? 🟡",
                3 => "Perfect! Now shapes - find a CIRCLE! ⭕",
                4 => "Amazing! Last one - find a SQUARE! ⬜",
                _ => "You know your colors and shapes! 🎨⭐"
            },
            "logic" => step switch
            {
                0 => "Let's solve puzzles! 🧩\nWhich piece fits here? 🔲",
                1 => "Great thinking! Memory game! 🧠\nRemember this pattern: 🔴🔵🔴",
                2 => "Excellent! What comes next? 🔴🔵🔴__?",
                3 => "Smart! Another puzzle! 🧩\nWhich is different? 🐶🐶🐱🐶",
                4 => "Brilliant! Final brain teaser! 🤔\nIf you have 3 toys and get 2 more, how many total?",
                _ => "You're a puzzle master! 🧩⭐"
            },
            "science" => step switch
            {
                0 => "Welcome to nature! 🌱\nLet's meet animals! What sound does a cow make? 🐄",
                1 => "Moo! Perfect! 🐄 Now, where do fish live? 🐠",
                2 => "In water! Great! 💧 What do plants need to grow? ☀️💧",
                3 => "Sun and water! Smart! ☀️ Which is bigger - an elephant or a mouse? 🐘🐭",
                4 => "Elephant! Wow! 🐘 Final question: What comes from clouds? ☁️",
                _ => "You're a nature explorer! 🌱⭐"
            },
            _ => step switch
            {
                0 => "Welcome to this fun activity! 🎉",
                1 => "You're doing great! Keep going! 🌟",
                2 => "Halfway there! Amazing work! 💪",
                3 => "Almost finished! You're so smart! 🧠",
                4 => "Last step! You've got this! 🚀",
                _ => "Congratulations! Activity complete! 🎉⭐"
            }
        };
    }

    /// <summary>
    /// Proceeds to the next step in the activity.
    /// </summary>
    private async Task NextStepAsync()
    {
        if (!CanGoToNextStep()) return;

        _logger.LogInformation("Advancing to next step: {CurrentStep} -> {NextStep}", CurrentProgress, CurrentProgress + 1);

        // Provide haptic feedback
        try
        {
#if ANDROID || IOS
            HapticFeedback.Default.Perform(HapticFeedbackType.Click);
#endif
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not provide haptic feedback");
        }

        CurrentProgress++;
        OnPropertyChanged(nameof(CurrentStepContent));

        // Check if activity is completed
        if (CurrentProgress >= TotalSteps)
        {
            IsCompleted = true;
            await ShowCelebrationAsync();
        }

        // Simulate some processing time for better UX
        await Task.Delay(300);
    }

    /// <summary>
    /// Completes the entire activity.
    /// </summary>
    private async Task CompleteActivityAsync()
    {
        if (!CanCompleteActivity()) return;

        await ExecuteAsync(async () =>
        {
            _logger.LogInformation("Completing activity {ActivityId} for child {ChildId}", ActivityId, ChildId);

            // TODO: Save activity completion to database
            // TODO: Award stars/achievements
            // TODO: Update child progress

            // Simulate completion processing
            await Task.Delay(1000);

            // Show celebration
            await _navigationService.ShowCelebrationAsync(
                "Activity Complete!",
                "You earned 3 stars! Great job learning!");

            // Navigate back to subject selection
            await Task.Delay(2000);
            await _navigationService.GoBackAsync();

        }, "Saving your amazing progress...");
    }

    /// <summary>
    /// Navigates back to the previous page.
    /// </summary>
    private async Task GoBackAsync()
    {
        await _navigationService.GoBackAsync();
    }

    /// <summary>
    /// Navigates to the home page.
    /// </summary>
    private async Task GoHomeAsync()
    {
        await _navigationService.GoToHomeAsync();
    }

    /// <summary>
    /// Shows a celebration for completing a step or activity.
    /// </summary>
    private async Task ShowCelebrationAsync()
    {
        var messages = new[]
        {
            "Awesome job! 🌟",
            "You're so smart! 🧠",
            "Keep going! 💪",
            "Amazing work! 🎉",
            "You're a star! ⭐"
        };

        var randomMessage = messages[Random.Shared.Next(messages.Length)];
        await Task.Delay(500); // Brief pause for effect

        // In a real implementation, this would show animations and sounds
        _logger.LogInformation("Celebrating step completion: {Message}", randomMessage);
    }

    /// <summary>
    /// Determines if the user can proceed to the next step.
    /// </summary>
    private bool CanGoToNextStep()
    {
        return !IsCompleted && CurrentProgress < TotalSteps;
    }

    /// <summary>
    /// Determines if the activity can be completed.
    /// </summary>
    private bool CanCompleteActivity()
    {
        return IsCompleted;
    }

    /// <summary>
    /// Initializes the activity with the provided parameters.
    /// </summary>
    public void Initialize(int childId, int activityId, string subjectName)
    {
        ChildId = childId;
        ActivityId = activityId;
        SubjectName = subjectName;

        Title = $"{subjectName} Activity";
        CurrentProgress = 0;
        IsCompleted = false;

        OnPropertyChanged(nameof(CurrentStepContent));

        _logger.LogInformation("Activity initialized: Child={ChildId}, Activity={ActivityId}, Subject={SubjectName}",
            ChildId, ActivityId, SubjectName);
    }

    /// <summary>
    /// Called when the page appears.
    /// </summary>
    public override Task OnAppearingAsync()
    {
        _logger.LogDebug("Activity page appearing: {ActivityId}", ActivityId);
        OnPropertyChanged(nameof(CurrentStepContent));
        return base.OnAppearingAsync();
    }
}