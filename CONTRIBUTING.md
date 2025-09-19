# Contributing to EduPlayKids

Thank you for your interest in contributing to EduPlayKids! This educational mobile application for children aged 3-8 is built with .NET MAUI and follows Clean Architecture principles. We welcome contributions that help improve the learning experience for young children.

## 📋 Table of Contents

- [Code of Conduct](#code-of-conduct)
- [Getting Started](#getting-started)
- [Development Workflow](#development-workflow)
- [Coding Standards](#coding-standards)
- [Architecture Guidelines](#architecture-guidelines)
- [Testing Requirements](#testing-requirements)
- [Pull Request Process](#pull-request-process)
- [Child Safety Considerations](#child-safety-considerations)
- [Educational Content Guidelines](#educational-content-guidelines)

## 🤝 Code of Conduct

This project adheres to a code of conduct that ensures a welcoming environment for all contributors. By participating, you agree to:

- Use welcoming and inclusive language
- Respect differing viewpoints and experiences
- Accept constructive criticism gracefully
- Focus on what's best for the educational community
- Show empathy towards young learners and their families

## 🚀 Getting Started

### Prerequisites

Before contributing, ensure you have:

1. **Development Environment**: Follow the [Installation Guide](INSTALL.md)
2. **Project Knowledge**: Review the [Architecture Documentation](1.2%20Arquitectura%20del%20Sistema/)
3. **Child Development Understanding**: Familiarity with early childhood education principles
4. **Privacy Awareness**: Understanding of COPPA and GDPR-K requirements

### First Steps

1. **Fork the repository** to your GitHub account
2. **Clone your fork** locally:
   ```bash
   git clone https://github.com/YOUR_USERNAME/EduPlayKids.git
   cd EduPlayKids
   ```
3. **Set up the development environment** following [INSTALL.md](INSTALL.md)
4. **Create a feature branch**:
   ```bash
   git checkout -b feature/your-feature-name
   ```

## 🔄 Development Workflow

### Branch Naming Convention

Use descriptive branch names that follow this pattern:
- `feature/add-mathematics-counting` - New features
- `bugfix/fix-audio-playback-issue` - Bug fixes
- `docs/update-installation-guide` - Documentation updates
- `refactor/improve-database-performance` - Code improvements
- `test/add-unit-tests-for-progress` - Test additions

### Commit Message Format

Follow the conventional commit format:

```
type(scope): brief description

Detailed explanation of the change (if needed).

Closes #issue-number
```

**Types:**
- `feat`: New feature
- `fix`: Bug fix
- `docs`: Documentation changes
- `style`: Code style changes (formatting, etc.)
- `refactor`: Code refactoring
- `test`: Adding or updating tests
- `chore`: Maintenance tasks

**Examples:**
```
feat(math): add counting game for numbers 1-10

Implement interactive counting game with audio feedback
for children aged 3-4. Includes visual number recognition
and touch-based interaction.

Closes #45
```

```
fix(audio): resolve playback issues on Android API 21

Fixed audio initialization timing issue that prevented
sounds from playing on older Android devices.

Closes #78
```

## 📏 Coding Standards

### C# Guidelines

Follow Microsoft's C# coding conventions and these project-specific guidelines:

#### 1. File Organization
```csharp
// File header (required for new files)
// Copyright (c) 2024 EduPlayKids. All rights reserved.
// This file is part of the EduPlayKids educational application.

using System;
using System.Collections.Generic;
// System usings first, then external packages, then project usings

namespace EduPlayKids.Domain.Entities;

/// <summary>
/// Comprehensive XML documentation required for all public members
/// </summary>
public class EducationalActivity
{
    // Public properties first, then private fields
    // Methods ordered: constructors, public methods, private methods
}
```

#### 2. Naming Conventions
- **Classes**: PascalCase (`EducationalActivity`)
- **Methods**: PascalCase (`CalculateProgress`)
- **Properties**: PascalCase (`CurrentLevel`)
- **Fields**: camelCase with underscore prefix (`_repository`)
- **Constants**: PascalCase (`MaxAttemptsAllowed`)
- **Interfaces**: PascalCase with 'I' prefix (`IProgressTracker`)

#### 3. Code Documentation
```csharp
/// <summary>
/// Tracks a child's progress through educational activities.
/// </summary>
/// <param name="childId">Unique identifier for the child</param>
/// <param name="activityId">Unique identifier for the activity</param>
/// <param name="score">Score achieved (0-100)</param>
/// <returns>True if progress was saved successfully</returns>
/// <exception cref="ArgumentException">Thrown when childId or activityId is invalid</exception>
public async Task<bool> RecordProgressAsync(int childId, int activityId, int score)
{
    // Implementation
}
```

### XAML Guidelines

#### 1. Layout Standards
```xml
<!-- Use consistent indentation (2 spaces) -->
<ContentPage x:Class="EduPlayKids.Presentation.Views.MathematicsPage"
             xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:viewmodels="clr-namespace:EduPlayKids.Presentation.ViewModels"
             Title="Mathematics Activities">

  <!-- Child-friendly touch targets (minimum 60dp) -->
  <Button Text="Start Counting"
          HeightRequest="80"
          WidthRequest="200"
          FontSize="18"
          BackgroundColor="{StaticResource PrimaryColor}" />
</ContentPage>
```

#### 2. Resource Organization
- Use StaticResource for colors, styles, and templates
- Follow naming convention: `{Component}{Property}` (e.g., `ButtonPrimaryStyle`)
- Group related resources in separate ResourceDictionaries

### Child-Safe Design Requirements

#### 1. Touch Targets
- **Minimum size**: 60dp (approximately 9.6mm)
- **Recommended size**: 80dp for primary actions
- **Spacing**: Minimum 8dp between interactive elements

#### 2. Visual Design
- **High contrast**: Minimum 7:1 ratio for text
- **Color accessibility**: Support for color blindness
- **Font size**: Minimum 18sp for readable text
- **Animation**: Smooth, not overwhelming (max 300ms duration)

#### 3. Audio Requirements
- **Format**: MP3 or WAV, optimized for mobile
- **Volume**: Normalized levels across all audio files
- **Localization**: Both Spanish and English versions required
- **Child-friendly**: Clear pronunciation, appropriate pace

## 🏗️ Architecture Guidelines

### Clean Architecture Layers

#### 1. Domain Layer (`EduPlayKids.Domain`)
- **Entities**: Core business objects (Child, Activity, Progress)
- **Value Objects**: Immutable objects (Score, Age, Level)
- **Interfaces**: Repository and service contracts
- **Domain Services**: Core business logic
- **No dependencies** on external frameworks

```csharp
// Example Domain Entity
namespace EduPlayKids.Domain.Entities;

public class Child : BaseEntity
{
    public string Name { get; private set; }
    public Age Age { get; private set; }
    public LanguagePreference Language { get; private set; }
    public List<Progress> ProgressHistory { get; private set; }

    // Domain methods that encapsulate business rules
    public bool CanAccessActivity(Activity activity)
    {
        return activity.MinimumAge <= Age &&
               HasCompletedPrerequisites(activity);
    }
}
```

#### 2. Application Layer (`EduPlayKids.Application`)
- **Use Cases**: Application-specific business rules
- **DTOs**: Data transfer objects for communication
- **Interfaces**: Service contracts
- **Services**: Application services orchestrating domain objects

```csharp
// Example Use Case
namespace EduPlayKids.Application.UseCases.Progress;

public class RecordActivityCompletionUseCase
{
    private readonly IProgressRepository _progressRepository;
    private readonly IChildRepository _childRepository;

    public async Task<ProgressResult> ExecuteAsync(RecordProgressCommand command)
    {
        // Orchestrate domain objects to complete the use case
    }
}
```

#### 3. Infrastructure Layer (`EduPlayKids.Infrastructure`)
- **Data Access**: Entity Framework Core repositories
- **External Services**: Audio playback, file system access
- **Configuration**: Database context, dependency injection setup

#### 4. Presentation Layer (`EduPlayKids.Presentation`)
- **Views**: XAML pages and controls
- **ViewModels**: MVVM pattern implementation
- **Converters**: Value converters for data binding
- **Behaviors**: Reusable UI behaviors

### MVVM Implementation

```csharp
// Example ViewModel
namespace EduPlayKids.Presentation.ViewModels;

public partial class MathematicsViewModel : BaseViewModel
{
    [ObservableProperty]
    private ObservableCollection<ActivityViewModel> _activities;

    [RelayCommand]
    private async Task StartActivityAsync(ActivityViewModel activity)
    {
        // Command implementation with proper error handling
        try
        {
            await _navigationService.NavigateToAsync(activity.NavigationRoute);
        }
        catch (Exception ex)
        {
            await ShowErrorMessageAsync("Unable to start activity. Please try again.");
            _logger.LogError(ex, "Failed to start activity {ActivityId}", activity.Id);
        }
    }
}
```

## 🧪 Testing Requirements

### Unit Tests

Every new feature must include comprehensive unit tests:

```csharp
// Example unit test
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
    [InlineData(3, 4, false)]
    [InlineData(4, 4, true)]
    [InlineData(5, 4, true)]
    public void CanAccessActivity_VariousAges_ReturnsExpectedResult(
        int childAge, int minimumAge, bool expected)
    {
        // Test implementation
    }
}
```

### Integration Tests

Test the interaction between components:

```csharp
// Example integration test
namespace EduPlayKids.Infrastructure.Tests;

public class ProgressRepositoryTests : IClassFixture<DatabaseFixture>
{
    [Fact]
    public async Task SaveProgressAsync_ValidProgress_PersistsToDatabase()
    {
        // Test database operations
    }
}
```

### UI Tests

For critical user flows, especially child interactions:

```csharp
// Example UI test using .NET MAUI Testing
namespace EduPlayKids.Presentation.Tests;

public class MathematicsPageTests
{
    [Fact]
    public async Task StartCountingButton_WhenTapped_NavigatesToCountingActivity()
    {
        // UI automation test
    }
}
```

### Test Coverage Requirements

- **Domain Layer**: 95% code coverage
- **Application Layer**: 90% code coverage
- **Infrastructure Layer**: 80% code coverage
- **Presentation Layer**: 70% code coverage (focus on ViewModels)

```bash
# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Generate coverage report
dotnet tool install -g dotnet-reportgenerator-globaltool
reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:"coverage-report"
```

## 🔍 Pull Request Process

### Before Submitting

1. **Run all tests** and ensure they pass:
   ```bash
   dotnet test
   ```

2. **Check code formatting**:
   ```bash
   dotnet format
   ```

3. **Verify build succeeds** on all platforms:
   ```bash
   dotnet build -f net8.0-android
   dotnet build -f net8.0-ios
   ```

4. **Update documentation** if needed

### Pull Request Template

When creating a pull request, use this template:

```markdown
## Description
Brief description of changes and their purpose.

## Type of Change
- [ ] Bug fix (non-breaking change that fixes an issue)
- [ ] New feature (non-breaking change that adds functionality)
- [ ] Breaking change (fix or feature that causes existing functionality to change)
- [ ] Documentation update

## Testing
- [ ] Unit tests added/updated
- [ ] Integration tests added/updated
- [ ] Manual testing completed
- [ ] Child safety considerations verified

## Educational Impact
- [ ] Age-appropriate content (specify age range: 3-4, 5, 6-8)
- [ ] Curriculum alignment verified
- [ ] Accessibility considerations addressed
- [ ] Bilingual support included (if applicable)

## Checklist
- [ ] Code follows project style guidelines
- [ ] Self-review completed
- [ ] Tests pass locally
- [ ] Documentation updated
- [ ] No sensitive information included
```

### Review Process

1. **Automated checks** must pass (build, tests, formatting)
2. **Code review** by at least one maintainer
3. **Educational review** for content changes
4. **Child safety review** for UI/UX changes
5. **Final approval** and merge

## 🛡️ Child Safety Considerations

### Privacy Requirements

- **No external communication**: All data stays on device
- **No personal information collection**: Only necessary app data
- **Parental controls**: PIN-protected settings and statistics
- **COPPA compliance**: No data collection from children under 13

### Content Guidelines

- **Age-appropriate**: Content suitable for 3-8 year olds
- **Educational value**: Aligned with curriculum standards
- **Positive messaging**: Encouraging, supportive language
- **Cultural sensitivity**: Appropriate for diverse backgrounds

### Technical Safety

- **Input validation**: Prevent any unsafe user input
- **Error handling**: Child-friendly error messages
- **Navigation safety**: Prevent accidental app exits
- **Content filtering**: No inappropriate content possible

## 📚 Educational Content Guidelines

### Curriculum Alignment

Content must align with:
- **US Elementary Education Standards**
- **Age-appropriate learning objectives**
- **Progressive difficulty levels**
- **Mastery-based progression**

### Subject Areas

1. **Mathematics**: Numbers, counting, basic operations, shapes
2. **Reading & Phonics**: Alphabet, phonics, sight words
3. **Basic Concepts**: Colors, shapes, patterns
4. **Logic & Thinking**: Puzzles, memory games
5. **Science**: Animals, plants, weather

### Localization Requirements

- **Bilingual support**: Spanish and English
- **Cultural adaptation**: Appropriate for Hispanic families
- **Audio localization**: Native speaker recordings
- **Visual localization**: Culturally appropriate imagery

## 🆘 Getting Help

### Documentation
- [System Architecture](1.2%20Arquitectura%20del%20Sistema/)
- [Database Design](1.3%20Diseño%20de%20Base%20de%20Datos/)
- [UX/UI Guidelines](2.1%20Diseño%20UXUI/)
- [Technical Documentation](docs/technical/)

### Communication Channels
- **Issues**: GitHub Issues for bugs and feature requests
- **Discussions**: GitHub Discussions for questions and ideas
- **Email**: [development@eduplaykids.com](mailto:development@eduplaykids.com)

### Response Times
- **Bug reports**: 1-2 business days
- **Feature requests**: 3-5 business days
- **Pull reviews**: 2-3 business days

## 📄 License

By contributing to EduPlayKids, you agree that your contributions will be licensed under the same license as the project.

---

**Thank you for contributing to children's education! 🎓**

Your contributions help create better learning experiences for children aged 3-8 years. Every improvement makes a difference in a child's educational journey.