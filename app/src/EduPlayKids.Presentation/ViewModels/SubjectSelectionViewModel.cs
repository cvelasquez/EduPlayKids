using System.Collections.ObjectModel;
using System.Windows.Input;
using EduPlayKids.App.Services;
using Microsoft.Extensions.Logging;

namespace EduPlayKids.App.ViewModels;

/// <summary>
/// ViewModel for the subject selection page.
/// Allows children to choose which subject they want to learn about.
/// Subjects are age-appropriate and aligned with US elementary education standards.
/// </summary>
public class SubjectSelectionViewModel : BaseViewModel
{
    private readonly IChildSafeNavigationService _navigationService;
    private int _childId;
    private int _childAge;
    private string _childName = string.Empty;
    private Subject? _selectedSubject;

    public SubjectSelectionViewModel(
        IChildSafeNavigationService navigationService,
        ILogger<SubjectSelectionViewModel> logger) : base(logger)
    {
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));

        Title = "What do you want to learn today?";
        BusyText = "Loading your learning adventure...";

        // Initialize commands
        SelectSubjectCommand = new Command<Subject>(async (subject) => await SelectSubjectAsync(subject));
        StartLearningCommand = new Command(async () => await StartLearningAsync(), () => CanStartLearning());
        GoBackCommand = new Command(async () => await GoBackAsync());
        GoToParentalControlsCommand = new Command(async () => await GoToParentalControlsAsync());
    }

    /// <summary>
    /// Collection of available subjects for the child's age group.
    /// </summary>
    public ObservableCollection<Subject> Subjects { get; } = new();

    /// <summary>
    /// Gets or sets the child's ID.
    /// </summary>
    public int ChildId
    {
        get => _childId;
        set => SetProperty(ref _childId, value);
    }

    /// <summary>
    /// Gets or sets the child's age.
    /// </summary>
    public int ChildAge
    {
        get => _childAge;
        set
        {
            if (SetProperty(ref _childAge, value))
            {
                LoadSubjectsForAge();
            }
        }
    }

    /// <summary>
    /// Gets or sets the child's name for personalization.
    /// </summary>
    public string ChildName
    {
        get => _childName;
        set => SetProperty(ref _childName, value);
    }

    /// <summary>
    /// Gets or sets the currently selected subject.
    /// </summary>
    public Subject? SelectedSubject
    {
        get => _selectedSubject;
        set
        {
            if (SetProperty(ref _selectedSubject, value))
            {
                ((Command)StartLearningCommand).ChangeCanExecute();
            }
        }
    }

    /// <summary>
    /// Gets a personalized greeting message for the child.
    /// </summary>
    public string GreetingMessage
    {
        get
        {
            if (string.IsNullOrEmpty(ChildName))
                return "What do you want to learn today?";

            var timeOfDay = DateTime.Now.Hour switch
            {
                >= 5 and < 12 => "Good morning",
                >= 12 and < 17 => "Good afternoon",
                _ => "Good evening"
            };

            return $"{timeOfDay}, {ChildName}! 🌟";
        }
    }

    /// <summary>
    /// Command to select a subject.
    /// </summary>
    public ICommand SelectSubjectCommand { get; }

    /// <summary>
    /// Command to start learning with the selected subject.
    /// </summary>
    public ICommand StartLearningCommand { get; }

    /// <summary>
    /// Command to go back to the previous page.
    /// </summary>
    public ICommand GoBackCommand { get; }

    /// <summary>
    /// Command to access parental controls.
    /// </summary>
    public ICommand GoToParentalControlsCommand { get; }

    /// <summary>
    /// Loads subjects appropriate for the child's age.
    /// </summary>
    private void LoadSubjectsForAge()
    {
        Subjects.Clear();

        // Mathematics - Available for all ages with different focus
        Subjects.Add(new Subject
        {
            Id = 1,
            Name = "Mathematics",
            Title = ChildAge <= 4 ? "Numbers & Counting" : ChildAge == 5 ? "Math Fun" : "Math Adventures",
            Description = ChildAge <= 4
                ? "Learn numbers, counting, and basic shapes with fun games!"
                : ChildAge == 5
                ? "Explore numbers, addition, and subtraction through play!"
                : "Master math skills with challenging problems and puzzles!",
            IconEmoji = "🔢",
            Color = "#FF6B6B",
            DifficultyLevel = ChildAge <= 4 ? "Easy" : ChildAge == 5 ? "Medium" : "Hard",
            EstimatedDurationMinutes = ChildAge <= 4 ? 10 : ChildAge == 5 ? 15 : 20,
            ActivityCount = ChildAge <= 4 ? 20 : ChildAge == 5 ? 30 : 40
        });

        // Reading & Phonics - Adapted by age
        Subjects.Add(new Subject
        {
            Id = 2,
            Name = "Reading",
            Title = ChildAge <= 4 ? "Letters & Sounds" : ChildAge == 5 ? "Reading Fun" : "Reading Adventures",
            Description = ChildAge <= 4
                ? "Discover letters, sounds, and your first words!"
                : ChildAge == 5
                ? "Learn to read with phonics and sight words!"
                : "Improve reading skills with stories and comprehension!",
            IconEmoji = "📚",
            Color = "#4ECDC4",
            DifficultyLevel = ChildAge <= 4 ? "Easy" : ChildAge == 5 ? "Medium" : "Hard",
            EstimatedDurationMinutes = ChildAge <= 4 ? 8 : ChildAge == 5 ? 12 : 18,
            ActivityCount = ChildAge <= 4 ? 25 : ChildAge == 5 ? 35 : 45
        });

        // Basic Concepts - More important for younger children
        if (ChildAge <= 6)
        {
            Subjects.Add(new Subject
            {
                Id = 3,
                Name = "BasicConcepts",
                Title = "Colors & Shapes",
                Description = ChildAge <= 4
                    ? "Explore colors, shapes, and patterns in fun ways!"
                    : "Master colors, shapes, patterns, and basic concepts!",
                IconEmoji = "🎨",
                Color = "#FFE66D",
                DifficultyLevel = ChildAge <= 4 ? "Easy" : "Medium",
                EstimatedDurationMinutes = ChildAge <= 4 ? 10 : 15,
                ActivityCount = ChildAge <= 4 ? 15 : 25
            });
        }

        // Logic & Thinking - For all ages
        Subjects.Add(new Subject
        {
            Id = 4,
            Name = "Logic",
            Title = ChildAge <= 4 ? "Puzzle Time" : ChildAge == 5 ? "Brain Games" : "Logic Challenges",
            Description = ChildAge <= 4
                ? "Simple puzzles and memory games for little minds!"
                : ChildAge == 5
                ? "Fun brain games and logical thinking activities!"
                : "Advanced puzzles and critical thinking challenges!",
            IconEmoji = "🧩",
            Color = "#A8E6CF",
            DifficultyLevel = ChildAge <= 4 ? "Easy" : ChildAge == 5 ? "Medium" : "Hard",
            EstimatedDurationMinutes = ChildAge <= 4 ? 8 : ChildAge == 5 ? 12 : 16,
            ActivityCount = ChildAge <= 4 ? 18 : ChildAge == 5 ? 28 : 38
        });

        // Science - Simplified for younger children
        Subjects.Add(new Subject
        {
            Id = 5,
            Name = "Science",
            Title = ChildAge <= 4 ? "Animals & Nature" : ChildAge == 5 ? "Science Fun" : "Science Explorers",
            Description = ChildAge <= 4
                ? "Meet animals, learn about nature, and explore the world!"
                : ChildAge == 5
                ? "Discover animals, plants, weather, and nature!"
                : "Explore science with experiments and discoveries!",
            IconEmoji = "🌱",
            Color = "#88D8B0",
            DifficultyLevel = ChildAge <= 4 ? "Easy" : ChildAge == 5 ? "Medium" : "Hard",
            EstimatedDurationMinutes = ChildAge <= 4 ? 10 : ChildAge == 5 ? 15 : 20,
            ActivityCount = ChildAge <= 4 ? 20 : ChildAge == 5 ? 30 : 40
        });

        // Social Skills - Important for all ages
        if (ChildAge >= 5)
        {
            Subjects.Add(new Subject
            {
                Id = 6,
                Name = "Social",
                Title = "Social Skills",
                Description = ChildAge == 5
                    ? "Learn about friendship, kindness, and cooperation!"
                    : "Develop empathy, communication, and social skills!",
                IconEmoji = "🤝",
                Color = "#DDA0DD",
                DifficultyLevel = ChildAge == 5 ? "Medium" : "Hard",
                EstimatedDurationMinutes = ChildAge == 5 ? 12 : 15,
                ActivityCount = ChildAge == 5 ? 20 : 30
            });
        }

        _logger.LogInformation("Loaded {Count} subjects for age {Age}", Subjects.Count, ChildAge);
    }

    /// <summary>
    /// Handles subject selection.
    /// </summary>
    /// <param name="subject">The selected subject.</param>
    private async Task SelectSubjectAsync(Subject subject)
    {
        if (subject == null) return;

        _logger.LogInformation("Subject selected: {SubjectName} for child {ChildId}", subject.Name, ChildId);
        SelectedSubject = subject;

        // Provide haptic feedback for selection
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

        // Auto-advance after selection (child-friendly UX)
        await Task.Delay(500); // Brief delay for visual feedback
        await StartLearningAsync();
    }

    /// <summary>
    /// Starts learning with the selected subject.
    /// </summary>
    private async Task StartLearningAsync()
    {
        if (!CanStartLearning()) return;

        await ExecuteAsync(async () =>
        {
            _logger.LogInformation("Starting learning: Subject={SubjectName}, Child={ChildId}", SelectedSubject!.Name, ChildId);

            // Navigate to the first activity for this subject
            var parameters = new Dictionary<string, object>
            {
                ["childId"] = ChildId.ToString(),
                ["activityId"] = "1", // First activity
                ["subjectName"] = Uri.EscapeDataString(SelectedSubject!.Name)
            };

            await _navigationService.NavigateToAsync("//activity", parameters);

        }, $"Loading {SelectedSubject!.Title} activities...");
    }

    /// <summary>
    /// Handles going back to age selection.
    /// </summary>
    private async Task GoBackAsync()
    {
        await _navigationService.GoBackAsync();
    }

    /// <summary>
    /// Navigates to parental controls.
    /// </summary>
    private async Task GoToParentalControlsAsync()
    {
        await _navigationService.GoToParentalControlsAsync();
    }

    /// <summary>
    /// Determines if learning can be started.
    /// </summary>
    /// <returns>True if a subject is selected, false otherwise.</returns>
    private bool CanStartLearning()
    {
        return SelectedSubject != null;
    }

    /// <summary>
    /// Called when the page appears.
    /// </summary>
    public override Task OnAppearingAsync()
    {
        _logger.LogDebug("Subject selection page appearing for child {ChildId}, age {ChildAge}", ChildId, ChildAge);

        // Update greeting message when page appears
        OnPropertyChanged(nameof(GreetingMessage));

        // Load subjects if not already loaded
        if (!Subjects.Any() && ChildAge > 0)
        {
            LoadSubjectsForAge();
        }

        return base.OnAppearingAsync();
    }

    /// <summary>
    /// Initializes the view model with child information.
    /// </summary>
    /// <param name="childId">The child's ID.</param>
    /// <param name="childAge">The child's age.</param>
    /// <param name="childName">The child's name.</param>
    public void Initialize(int childId, int childAge, string childName)
    {
        ChildId = childId;
        ChildAge = childAge;
        ChildName = childName;

        Title = string.IsNullOrEmpty(childName)
            ? "What do you want to learn today?"
            : $"Hi {childName}! What do you want to learn?";

        LoadSubjectsForAge();
    }
}

/// <summary>
/// Represents a learning subject in the EduPlayKids app.
/// </summary>
public class Subject
{
    /// <summary>
    /// The unique identifier for the subject.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The internal name of the subject.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The display title for the subject (age-appropriate).
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Description of what the child will learn in this subject.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Emoji icon representing the subject.
    /// </summary>
    public string IconEmoji { get; set; } = string.Empty;

    /// <summary>
    /// Hex color code for the subject's theme.
    /// </summary>
    public string Color { get; set; } = string.Empty;

    /// <summary>
    /// Difficulty level appropriate for the child's age.
    /// </summary>
    public string DifficultyLevel { get; set; } = string.Empty;

    /// <summary>
    /// Estimated duration per activity in minutes.
    /// </summary>
    public int EstimatedDurationMinutes { get; set; }

    /// <summary>
    /// Number of activities available in this subject.
    /// </summary>
    public int ActivityCount { get; set; }

    /// <summary>
    /// Gets the duration display text.
    /// </summary>
    public string DurationText => $"~{EstimatedDurationMinutes} min per activity";

    /// <summary>
    /// Gets the activity count display text.
    /// </summary>
    public string ActivityCountText => $"{ActivityCount} fun activities";
}