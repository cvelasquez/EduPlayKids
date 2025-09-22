using System.Collections.ObjectModel;
using System.Windows.Input;
using EduPlayKids.App.Services;
using EduPlayKids.Application.Interfaces;
using EduPlayKids.Application.Models.Audio;
using Microsoft.Extensions.Logging;

namespace EduPlayKids.App.ViewModels;

/// <summary>
/// ViewModel for the age selection page with audio feedback support.
/// Allows parents to set up their child's profile by selecting the appropriate age.
/// Age determines curriculum level and content difficulty.
/// Provides child-friendly audio guidance for the age selection process.
/// </summary>
public class AgeSelectionViewModel : AudioAwareBaseViewModel
{
    private readonly IChildSafeNavigationService _navigationService;
    private AgeOption? _selectedAge;
    private string _childName = string.Empty;

    public AgeSelectionViewModel(
        IChildSafeNavigationService navigationService,
        ILogger<AgeSelectionViewModel> logger,
        IAudioService? audioService = null) : base(logger, audioService)
    {
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));

        Title = "How old is your child?";
        BusyText = "Setting up your child's profile...";

        // Initialize age options for the target demographic (3-8 years)
        InitializeAgeOptions();

        // Initialize commands
        SelectAgeCommand = new Command<AgeOption>(async (age) => await SelectAgeAsync(age));
        ContinueCommand = new Command(async () => await ContinueAsync(), () => CanContinue());
        GoBackCommand = new Command(async () => await GoBackAsync());
    }

    /// <summary>
    /// Collection of available age options for child selection.
    /// </summary>
    public ObservableCollection<AgeOption> AgeOptions { get; } = new();

    /// <summary>
    /// Gets or sets the currently selected age option.
    /// </summary>
    public AgeOption? SelectedAge
    {
        get => _selectedAge;
        set
        {
            if (SetProperty(ref _selectedAge, value))
            {
                ((Command)ContinueCommand).ChangeCanExecute();
                OnPropertyChanged(nameof(SelectedAgeDescription));
            }
        }
    }

    /// <summary>
    /// Gets or sets the child's name.
    /// </summary>
    public string ChildName
    {
        get => _childName;
        set
        {
            if (SetProperty(ref _childName, value))
            {
                ((Command)ContinueCommand).ChangeCanExecute();
            }
        }
    }

    /// <summary>
    /// Gets a description of the selected age for display to parents.
    /// </summary>
    public string SelectedAgeDescription => SelectedAge?.Description ?? string.Empty;

    /// <summary>
    /// Command to select an age option.
    /// </summary>
    public ICommand SelectAgeCommand { get; }

    /// <summary>
    /// Command to continue with the selected age.
    /// </summary>
    public ICommand ContinueCommand { get; }

    /// <summary>
    /// Command to go back to the previous page.
    /// </summary>
    public ICommand GoBackCommand { get; }

    /// <summary>
    /// Initializes the available age options.
    /// </summary>
    private void InitializeAgeOptions()
    {
        AgeOptions.Clear();

        AgeOptions.Add(new AgeOption
        {
            Age = 3,
            DisplayText = "3 years old",
            Description = "Perfect for early learning! Your child will explore colors, shapes, and basic counting with lots of fun animations.",
            GradeLevel = "PreK",
            DifficultyLevel = "Easy",
            IconEmoji = "🌱"
        });

        AgeOptions.Add(new AgeOption
        {
            Age = 4,
            DisplayText = "4 years old",
            Description = "Great for curious minds! Your child will learn letters, numbers, and simple problem-solving through engaging activities.",
            GradeLevel = "PreK",
            DifficultyLevel = "Easy",
            IconEmoji = "🌸"
        });

        AgeOptions.Add(new AgeOption
        {
            Age = 5,
            DisplayText = "5 years old",
            Description = "Ready for kindergarten! Your child will practice reading, writing, and math skills to prepare for school.",
            GradeLevel = "Kindergarten",
            DifficultyLevel = "Medium",
            IconEmoji = "🌟"
        });

        AgeOptions.Add(new AgeOption
        {
            Age = 6,
            DisplayText = "6 years old",
            Description = "Building strong foundations! Your child will develop reading fluency and mathematical thinking skills.",
            GradeLevel = "Grade1",
            DifficultyLevel = "Medium",
            IconEmoji = "🚀"
        });

        AgeOptions.Add(new AgeOption
        {
            Age = 7,
            DisplayText = "7 years old",
            Description = "Growing confidence! Your child will tackle more complex stories and math problems with guided support.",
            GradeLevel = "Grade2",
            DifficultyLevel = "Hard",
            IconEmoji = "🏆"
        });

        AgeOptions.Add(new AgeOption
        {
            Age = 8,
            DisplayText = "8 years old",
            Description = "Advanced learner! Your child will explore challenging content and develop critical thinking skills.",
            GradeLevel = "Grade2",
            DifficultyLevel = "Hard",
            IconEmoji = "🎯"
        });
    }

    /// <summary>
    /// Handles age selection with audio feedback.
    /// </summary>
    /// <param name="ageOption">The selected age option.</param>
    private async Task SelectAgeAsync(AgeOption ageOption)
    {
        if (ageOption == null) return;

        _logger.LogInformation("Age selected: {Age} years old", ageOption.Age);
        SelectedAge = ageOption;

        // Play selection audio feedback
        await PlayUIFeedbackAsync(UIInteractionType.ItemSelection);

        // Play age-specific confirmation audio
        await Task.Delay(200); // Brief pause
        await PlayInstructionAsync($"age_selected_{ageOption.Age}");

        // Provide haptic feedback for selection (on supported devices)
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
    }

    /// <summary>
    /// Handles continue action to proceed with the selected age with audio feedback.
    /// </summary>
    private async Task ContinueAsync()
    {
        if (!CanContinue()) return;

        // Play confirmation audio
        await PlaySuccessFeedbackAsync(FeedbackIntensity.Medium);

        await ExecuteAsync(async () =>
        {
            _logger.LogInformation("Creating child profile: Name={ChildName}, Age={Age}", ChildName, SelectedAge!.Age);

            // Play profile creation audio
            await PlayInstructionAsync("creating_profile");

            // TODO: Create child profile using application services
            // For now, we'll simulate profile creation and navigate

            // Simulate profile creation
            await Task.Delay(1000);

            // Play completion audio
            await PlayCompletionFeedbackAsync(3, "profile_creation");

            // Navigate to subject selection with child information
            var parameters = new Dictionary<string, object>
            {
                ["childId"] = "1", // Temporary child ID
                ["childAge"] = SelectedAge!.Age.ToString(),
                ["childName"] = Uri.EscapeDataString(ChildName)
            };

            await PlayNavigationAudioAsync("subjects");
            await _navigationService.NavigateToAsync("//subjects", parameters);

        }, "Creating your child's profile...");
    }

    /// <summary>
    /// Handles going back to the previous page with audio feedback.
    /// </summary>
    private async Task GoBackAsync()
    {
        await PlayBackNavigationAudioAsync();
        await _navigationService.GoBackAsync();
    }

    /// <summary>
    /// Determines if the continue action can be executed.
    /// </summary>
    /// <returns>True if continue is allowed, false otherwise.</returns>
    private bool CanContinue()
    {
        return SelectedAge != null && !string.IsNullOrWhiteSpace(ChildName);
    }

    /// <summary>
    /// Called when the page appears with audio introduction.
    /// </summary>
    public override async Task OnAppearingAsync()
    {
        _logger.LogDebug("Age selection page appearing");

        // Reset selection when page appears
        SelectedAge = null;
        ChildName = string.Empty;

        await base.OnAppearingAsync();
    }

    /// <summary>
    /// Plays introduction audio specific to the age selection page.
    /// </summary>
    protected override async Task PlayPageIntroductionAudio()
    {
        // Play welcome message for age selection
        await PlayInstructionAsync("age_selection_welcome");

        // Brief pause, then play instructions
        await Task.Delay(1000);
        await PlayInstructionAsync("age_selection_instructions");
    }
}

/// <summary>
/// Represents an age option for child profile creation.
/// </summary>
public class AgeOption
{
    /// <summary>
    /// The age in years.
    /// </summary>
    public int Age { get; set; }

    /// <summary>
    /// Display text for the age option.
    /// </summary>
    public string DisplayText { get; set; } = string.Empty;

    /// <summary>
    /// Description of what the child will learn at this age.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// The academic grade level corresponding to this age.
    /// </summary>
    public string GradeLevel { get; set; } = string.Empty;

    /// <summary>
    /// The difficulty level for content at this age.
    /// </summary>
    public string DifficultyLevel { get; set; } = string.Empty;

    /// <summary>
    /// Emoji icon to display for this age option.
    /// </summary>
    public string IconEmoji { get; set; } = string.Empty;
}