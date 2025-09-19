# Coding Standards

This document defines the coding standards and conventions for the EduPlayKids project. Following these standards ensures consistency, maintainability, and readability across the codebase.

## 📋 General Principles

### 1. Code Quality Principles
- **SOLID Principles**: Single Responsibility, Open/Closed, Liskov Substitution, Interface Segregation, Dependency Inversion
- **DRY (Don't Repeat Yourself)**: Eliminate code duplication through abstraction
- **KISS (Keep It Simple, Stupid)**: Prefer simple, readable solutions
- **YAGNI (You Aren't Gonna Need It)**: Don't implement features until they're needed
- **Fail Fast**: Validate inputs early and provide clear error messages

### 2. Child-Safety First
- All user inputs must be validated and sanitized
- Error messages must be child-friendly (no technical jargon)
- No external communication capabilities
- Privacy-by-design implementation

## 🔤 Naming Conventions

### C# Naming Rules

#### Classes and Interfaces
```csharp
// ✅ Correct - PascalCase
public class EducationalActivity
public interface IProgressTracker
public abstract class BaseViewModel

// ❌ Incorrect
public class educationalActivity
public interface progressTracker
```

#### Methods and Properties
```csharp
// ✅ Correct - PascalCase
public string ChildName { get; set; }
public async Task<bool> SaveProgressAsync(int childId)
public void CalculateScore()

// ❌ Incorrect
public string childName { get; set; }
public async Task<bool> saveProgressAsync(int childId)
```

#### Fields and Variables
```csharp
// ✅ Correct - camelCase for locals, underscore prefix for private fields
private readonly IProgressRepository _progressRepository;
private int _currentScore;
public int attemptCount = 0;

// ❌ Incorrect
private readonly IProgressRepository ProgressRepository;
private int CurrentScore;
```

#### Constants and Enums
```csharp
// ✅ Correct - PascalCase
public const int MaxAttemptsAllowed = 3;
public const string DefaultLanguage = "es-US";

public enum DifficultyLevel
{
    Easy,
    Medium,
    Hard
}
```

### File and Directory Naming
```
✅ Correct Structure:
src/EduPlayKids.Domain/
├── Entities/
│   ├── Child.cs
│   ├── Activity.cs
│   └── Progress.cs
├── ValueObjects/
│   ├── Age.cs
│   └── Score.cs
└── Services/
    └── ProgressCalculationService.cs

❌ Incorrect:
src/eduplaykids.domain/
├── entities/
│   ├── child.cs
│   └── activity_class.cs
```

## 📝 Code Documentation

### XML Documentation Requirements
All public APIs must include comprehensive XML documentation:

```csharp
/// <summary>
/// Represents a child user in the educational system.
/// Tracks learning progress and manages age-appropriate content access.
/// </summary>
/// <remarks>
/// This class is designed specifically for children aged 3-8 years and includes
/// safety mechanisms to ensure age-appropriate content delivery.
/// </remarks>
public class Child : BaseEntity
{
    /// <summary>
    /// Gets the child's display name used throughout the application.
    /// </summary>
    /// <value>
    /// A non-empty string containing the child's name. Maximum length is 50 characters.
    /// </value>
    public string Name { get; private set; }

    /// <summary>
    /// Determines whether the child can access a specific educational activity.
    /// </summary>
    /// <param name="activity">The activity to check access for. Cannot be null.</param>
    /// <returns>
    /// <c>true</c> if the child meets the age requirements and has completed
    /// all prerequisite activities; otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="activity"/> is null.
    /// </exception>
    /// <example>
    /// <code>
    /// var child = new Child("Maria", Age.FromYears(5));
    /// var countingActivity = new Activity("Count to 10", Age.FromYears(4));
    /// bool canAccess = child.CanAccessActivity(countingActivity); // Returns true
    /// </code>
    /// </example>
    public bool CanAccessActivity(Activity activity)
    {
        ArgumentNullException.ThrowIfNull(activity);
        return activity.MinimumAge <= Age && HasCompletedPrerequisites(activity);
    }
}
```

### Inline Comments
Use inline comments sparingly, focusing on explaining **why**, not **what**:

```csharp
// ✅ Good - Explains business rule
// Children under 4 years need audio instructions for all activities
if (child.Age.Years < 4)
{
    activity.EnableAudioInstructions = true;
}

// ✅ Good - Explains complex algorithm
// Use exponential backoff to prevent overwhelming young children
// who might tap repeatedly when frustrated
var retryDelay = Math.Min(1000 * Math.Pow(2, attemptCount), 5000);

// ❌ Bad - States the obvious
// Increment the counter
counter++;

// ❌ Bad - Should be extracted to method with meaningful name
// Check if user is premium and activity is locked
if (user.IsPremium == false && activity.RequiresPremium == true)
{
    return false;
}
```

## 🏗️ Code Structure and Organization

### File Header Template
```csharp
// Copyright (c) 2024 EduPlayKids. All rights reserved.
// This file is part of the EduPlayKids educational application.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
// System usings first

using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
// External package usings second

using EduPlayKids.Domain.Entities;
using EduPlayKids.Domain.ValueObjects;
// Project usings last

namespace EduPlayKids.Application.Services;
```

### Class Organization
```csharp
public class ProgressTrackingService : IProgressTrackingService
{
    #region Private Fields
    private readonly IProgressRepository _progressRepository;
    private readonly ILogger<ProgressTrackingService> _logger;
    #endregion

    #region Constructor
    public ProgressTrackingService(
        IProgressRepository progressRepository,
        ILogger<ProgressTrackingService> logger)
    {
        _progressRepository = progressRepository ?? throw new ArgumentNullException(nameof(progressRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    #endregion

    #region Public Methods
    public async Task<ProgressResult> RecordProgressAsync(RecordProgressCommand command)
    {
        // Implementation
    }
    #endregion

    #region Private Methods
    private bool ValidateProgressData(RecordProgressCommand command)
    {
        // Implementation
    }
    #endregion
}
```

## 🔧 Method Guidelines

### Method Signature Standards
```csharp
// ✅ Good - Clear, descriptive name with proper async suffix
public async Task<ActivityResult> StartEducationalActivityAsync(
    int childId,
    int activityId,
    CancellationToken cancellationToken = default)

// ✅ Good - Overloads for common scenarios
public Task<ActivityResult> StartEducationalActivityAsync(int childId, int activityId)
    => StartEducationalActivityAsync(childId, activityId, CancellationToken.None);

// ❌ Bad - Unclear name, missing async suffix
public async Task<ActivityResult> Start(int id1, int id2)
```

### Parameter Validation
```csharp
public async Task<ProgressResult> RecordProgressAsync(RecordProgressCommand command)
{
    // ✅ Always validate parameters at method entry
    ArgumentNullException.ThrowIfNull(command);

    if (command.ChildId <= 0)
        throw new ArgumentException("Child ID must be positive", nameof(command));

    if (command.Score < 0 || command.Score > 100)
        throw new ArgumentOutOfRangeException(nameof(command),
            "Score must be between 0 and 100");

    // Method implementation...
}
```

### Return Value Guidelines
```csharp
// ✅ Good - Use Result pattern for operations that can fail
public class ProgressResult
{
    public bool IsSuccess { get; }
    public string ErrorMessage { get; }
    public ProgressData Data { get; }

    public static ProgressResult Success(ProgressData data) => new(true, null, data);
    public static ProgressResult Failure(string error) => new(false, error, null);
}

// ✅ Good - Use nullable returns for optional data
public async Task<Child?> FindChildByNameAsync(string name)

// ❌ Bad - Throwing exceptions for business logic failures
public async Task<Child> GetChildByNameAsync(string name)
{
    var child = await _repository.FindByNameAsync(name);
    if (child == null)
        throw new ChildNotFoundException(); // ❌ Should return null or Result
    return child;
}
```

## 🎯 Child-Specific Coding Standards

### Touch Interaction Guidelines
```csharp
// ✅ Good - Large touch targets for children
public static class ChildFriendlyDimensions
{
    public const double MinimumTouchTarget = 60; // dp
    public const double RecommendedTouchTarget = 80; // dp
    public const double MinimumSpacing = 8; // dp between elements
}

// ✅ Good - Clear visual feedback
[RelayCommand]
private async Task OnActivityButtonPressed(ActivityViewModel activity)
{
    // Provide immediate visual feedback
    activity.IsPressed = true;

    // Add haptic feedback for tactile response
    HapticFeedback.Perform(HapticFeedbackType.Click);

    // Execute action with proper error handling
    try
    {
        await StartActivityAsync(activity);
    }
    catch (Exception ex)
    {
        // Child-friendly error message
        await ShowChildFriendlyErrorAsync("Oops! Let's try that again.");
        _logger.LogError(ex, "Failed to start activity {ActivityId}", activity.Id);
    }
    finally
    {
        activity.IsPressed = false;
    }
}
```

### Audio Implementation Standards
```csharp
// ✅ Good - Consistent audio patterns
public class AudioInstructionService
{
    public async Task PlayInstructionAsync(string instructionKey, Language language)
    {
        var audioFile = GetLocalizedAudioFile(instructionKey, language);

        // Ensure audio is played with child-appropriate settings
        var playbackOptions = new AudioPlaybackOptions
        {
            Volume = 0.8f, // Comfortable volume for children
            PlaybackRate = 0.9f, // Slightly slower for comprehension
            Loop = false
        };

        await _audioService.PlayAsync(audioFile, playbackOptions);
    }

    private string GetLocalizedAudioFile(string key, Language language)
    {
        // Return path to localized audio file
        return language switch
        {
            Language.Spanish => $"audio/es/{key}.mp3",
            Language.English => $"audio/en/{key}.mp3",
            _ => $"audio/en/{key}.mp3" // English as fallback
        };
    }
}
```

## 🔒 Security and Privacy Standards

### Data Validation
```csharp
// ✅ Good - Comprehensive input validation
public class ChildRegistrationValidator
{
    public ValidationResult Validate(ChildRegistrationData data)
    {
        var errors = new List<string>();

        // Name validation - child safety focused
        if (string.IsNullOrWhiteSpace(data.Name))
            errors.Add("Child name is required");
        else if (data.Name.Length > 50)
            errors.Add("Child name must be 50 characters or less");
        else if (ContainsInappropriateContent(data.Name))
            errors.Add("Please use a different name");

        // Age validation - ensure appropriate content
        if (data.Age == null)
            errors.Add("Child age is required");
        else if (data.Age.Years < 3 || data.Age.Years > 8)
            errors.Add("This app is designed for children aged 3-8 years");

        return errors.Any()
            ? ValidationResult.Failure(errors)
            : ValidationResult.Success();
    }

    private bool ContainsInappropriateContent(string input)
    {
        // Implement child-safe content filtering
        // This is a simplified example
        var inappropriateWords = LoadInappropriateWordsList();
        return inappropriateWords.Any(word =>
            input.Contains(word, StringComparison.OrdinalIgnoreCase));
    }
}
```

### Privacy-Compliant Logging
```csharp
// ✅ Good - Privacy-safe logging
_logger.LogInformation("Child completed activity. ChildAge: {ChildAge}, ActivityType: {ActivityType}, Score: {Score}",
    child.Age.Years, // OK - Educational metric
    activity.Type,   // OK - Educational metric
    progress.Score); // OK - Educational metric

// ❌ Bad - Contains PII
_logger.LogInformation("Child {ChildName} from {Location} completed activity",
    child.Name,    // ❌ Personal information
    child.Location); // ❌ Location data
```

## 🧪 Testing Standards

### Unit Test Organization
```csharp
// ✅ Good - Clear test structure following AAA pattern
namespace EduPlayKids.Domain.Tests.Entities;

public class ChildTests
{
    [Fact]
    public void CanAccessActivity_WhenAgeRequirementMet_ReturnsTrue()
    {
        // Arrange
        var child = new Child("Test Child", Age.FromYears(5));
        var activity = new Activity("Counting 1-10", minimumAge: Age.FromYears(4));

        // Act
        var canAccess = child.CanAccessActivity(activity);

        // Assert
        Assert.True(canAccess);
    }

    [Theory]
    [InlineData(3, 4, false)] // Too young
    [InlineData(4, 4, true)]  // Exact age
    [InlineData(5, 4, true)]  // Above age
    public void CanAccessActivity_VariousAges_ReturnsExpectedResult(
        int childAge, int minimumAge, bool expected)
    {
        // Arrange
        var child = new Child("Test Child", Age.FromYears(childAge));
        var activity = new Activity("Test Activity", Age.FromYears(minimumAge));

        // Act
        var result = child.CanAccessActivity(activity);

        // Assert
        Assert.Equal(expected, result);
    }
}
```

### Test Naming Convention
```csharp
// ✅ Good - Method_Condition_ExpectedResult pattern
public void CalculateScore_WhenAllAnswersCorrect_Returns100()
public void ValidateAge_WhenAgeBelowMinimum_ThrowsArgumentException()
public void PlayAudio_WhenAudioFileNotFound_ReturnsFailureResult()

// ❌ Bad - Unclear test names
public void TestScore()
public void ValidateAge()
public void Test1()
```

## 📊 Performance Standards

### Async/Await Guidelines
```csharp
// ✅ Good - Proper async implementation
public async Task<List<Activity>> GetActivitiesForChildAsync(int childId)
{
    var child = await _childRepository.GetByIdAsync(childId);
    var allActivities = await _activityRepository.GetAllAsync();

    return allActivities
        .Where(activity => child.CanAccessActivity(activity))
        .ToList();
}

// ✅ Good - ConfigureAwait for library code
public async Task<ProgressResult> SaveProgressAsync(Progress progress)
{
    await _repository.SaveAsync(progress).ConfigureAwait(false);
    return ProgressResult.Success();
}

// ❌ Bad - Blocking async calls
public List<Activity> GetActivitiesForChild(int childId)
{
    return GetActivitiesForChildAsync(childId).Result; // ❌ Deadlock risk
}
```

### Memory Management
```csharp
// ✅ Good - Proper disposal patterns
public class AudioService : IDisposable
{
    private readonly MediaPlayer _mediaPlayer;
    private bool _disposed = false;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _mediaPlayer?.Release();
            _mediaPlayer?.Dispose();
        }
        _disposed = true;
    }
}

// ✅ Good - Using statements for resources
public async Task<byte[]> LoadActivityImageAsync(string imagePath)
{
    using var stream = await FileSystem.Current.OpenAppPackageFileAsync(imagePath);
    using var memoryStream = new MemoryStream();
    await stream.CopyToAsync(memoryStream);
    return memoryStream.ToArray();
}
```

## 🔧 Development Tools Configuration

### EditorConfig Settings
Create `.editorconfig` in project root:
```ini
root = true

[*]
charset = utf-8
end_of_line = crlf
insert_final_newline = true
trim_trailing_whitespace = true

[*.cs]
indent_style = space
indent_size = 4

# .NET Code Style Rules
dotnet_sort_system_directives_first = true
dotnet_separate_import_directive_groups = false

# C# Code Style Rules
csharp_prefer_braces = true:warning
csharp_prefer_simple_using_statement = true:suggestion
csharp_style_namespace_declarations = file_scoped:warning
```

### Code Analysis Rules
Enable in `.csproj` files:
```xml
<PropertyGroup>
  <EnableNETAnalyzers>true</EnableNETAnalyzers>
  <AnalysisLevel>latest</AnalysisLevel>
  <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
  <WarningsNotAsErrors>CS1591</WarningsNotAsErrors> <!-- Missing XML comments -->
</PropertyGroup>

<ItemGroup>
  <PackageReference Include="StyleCop.Analyzers" Version="1.1.118">
    <PrivateAssets>all</PrivateAssets>
    <IncludeAssets>runtime; build; native; contentfiles; analyzers</IncludeAssets>
  </PackageReference>
</ItemGroup>
```

## ✅ Code Review Checklist

Before submitting code for review, ensure:

### Functionality
- [ ] Code meets acceptance criteria
- [ ] All tests pass
- [ ] Child safety considerations addressed
- [ ] Age-appropriate content and interactions

### Code Quality
- [ ] Follows naming conventions
- [ ] Includes comprehensive XML documentation
- [ ] Uses appropriate design patterns
- [ ] Implements proper error handling

### Performance
- [ ] Uses async/await correctly
- [ ] Implements proper disposal patterns
- [ ] Considers mobile device constraints
- [ ] Optimizes database queries

### Security & Privacy
- [ ] Validates all inputs
- [ ] No personal information in logs
- [ ] Follows privacy-by-design principles
- [ ] Implements child-safe features

---

**Next**: [XAML Guidelines](xaml-guidelines.md) | [Child-Safe Design](child-safe-design.md)