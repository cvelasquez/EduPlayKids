# Project Structure Documentation

This document provides a comprehensive guide to the EduPlayKids project structure, explaining the organization of files, folders, and architectural layers. It serves as a reference for developers to understand where to find specific functionality and where to place new code.

## 📁 Solution Overview

```
EduPlayKids/
├── 📄 README.md
├── 📄 CLAUDE.md                          # Claude Code instructions
├── 📄 .gitignore
├── 📄 .editorconfig
├── 📄 EduPlayKids.sln                    # Visual Studio solution file
├── 📁 docs/                             # Documentation
├── 📁 src/                              # Source code
├── 📁 tests/                            # Test projects
├── 📁 tools/                            # Build and utility tools
├── 📁 assets/                           # Design assets and resources
└── 📁 .github/                          # GitHub workflows and templates
```

## 🏗️ Source Code Structure (`src/`)

The source code follows Clean Architecture principles with clear separation of concerns:

```
src/
├── 🎯 EduPlayKids.Domain/               # Core business logic
├── 🔄 EduPlayKids.Application/          # Use cases and application services
├── 🛠️ EduPlayKids.Infrastructure/       # Data access and external services
├── 📱 EduPlayKids.Presentation/         # UI and presentation layer
└── 🔗 EduPlayKids.Shared/              # Shared utilities and constants
```

### 🎯 Domain Layer (`EduPlayKids.Domain/`)

The innermost layer containing core business logic with no external dependencies.

```
EduPlayKids.Domain/
├── 📁 Entities/                        # Core business entities
│   ├── 👶 Child.cs                     # Child user entity
│   ├── 🎯 Activity.cs                  # Educational activity entity
│   ├── 📊 Progress.cs                  # Learning progress tracking
│   ├── 👥 User.cs                      # Parent/guardian entity
│   ├── 🏆 Achievement.cs               # Gamification achievements
│   ├── 📚 Subject.cs                   # Educational subjects
│   ├── 💰 Subscription.cs              # Premium subscription
│   └── 🎵 ContentAsset.cs              # Audio/visual content
│
├── 📁 ValueObjects/                    # Immutable value types
│   ├── 🎂 Age.cs                       # Age with years/months
│   ├── ⭐ Score.cs                     # Activity scoring
│   ├── 📊 Difficulty.cs               # Difficulty levels
│   ├── 🌐 Language.cs                  # Supported languages
│   ├── 🎮 ActivityType.cs              # Types of activities
│   └── 📍 ProgressStatus.cs           # Progress state
│
├── 📁 Services/                        # Domain service interfaces
│   ├── 🧮 IProgressCalculationService.cs
│   ├── 🔍 IAgeValidationService.cs
│   ├── 💡 IActivityRecommendationService.cs
│   ├── 🎯 IDifficultyAdjustmentService.cs
│   └── 🏆 IAchievementService.cs
│
├── 📁 Specifications/                  # Business rule specifications
│   ├── 🎯 ActivityAccessSpecification.cs
│   ├── 📊 ProgressValidationSpecification.cs
│   └── 🎂 AgeAppropriateContentSpecification.cs
│
├── 📁 Exceptions/                      # Domain-specific exceptions
│   ├── 🚫 ChildSafetyException.cs
│   ├── 📊 InvalidProgressException.cs
│   ├── 🔒 AccessDeniedException.cs
│   └── 💼 BusinessRuleException.cs
│
├── 📁 Events/                          # Domain events
│   ├── 🎯 ActivityStartedEvent.cs
│   ├── 📊 ProgressRecordedEvent.cs
│   ├── 🏆 AchievementUnlockedEvent.cs
│   └── 🎂 ChildAgeUpdatedEvent.cs
│
└── 📁 Common/                          # Shared domain concepts
    ├── 🏢 BaseEntity.cs                # Base entity with ID
    ├── 🎭 IAggregateRoot.cs           # Aggregate root marker
    ├── 📢 IDomainEvent.cs             # Domain event interface
    ├── 📋 ISpecification.cs           # Specification pattern
    └── 📊 Enumeration.cs              # Smart enum base class
```

#### Key Domain Entity Examples

```csharp
// Child.cs - Core business entity
public class Child : BaseEntity, IAggregateRoot
{
    public string Name { get; private set; }
    public Age Age { get; private set; }
    public Language PreferredLanguage { get; private set; }
    public List<Progress> ProgressHistory { get; private set; }
    public List<Achievement> Achievements { get; private set; }

    // Business logic methods
    public bool CanAccessActivity(Activity activity);
    public Progress RecordActivityProgress(Activity activity, Score score);
    public void UpdateAge(Age newAge);
}

// Age.cs - Value object
public record Age(int Years, int Months)
{
    public static Age FromYears(int years) => new(years, 0);
    public bool IsAtLeast(Age minimum) => /* implementation */;
    public bool IsAppropriateForActivity(Activity activity) => /* implementation */;
}
```

### 🔄 Application Layer (`EduPlayKids.Application/`)

Contains use cases, application services, and orchestration logic.

```
EduPlayKids.Application/
├── 📁 UseCases/                        # Application use cases
│   ├── 📁 Child/
│   │   ├── 👶 CreateChildUseCase.cs
│   │   ├── 📊 GetChildProgressUseCase.cs
│   │   ├── 🔄 UpdateChildProfileUseCase.cs
│   │   └── 🗑️ DeleteChildUseCase.cs
│   │
│   ├── 📁 Activity/
│   │   ├── 🚀 StartActivityUseCase.cs
│   │   ├── ✅ CompleteActivityUseCase.cs
│   │   ├── 📋 GetActivitiesForChildUseCase.cs
│   │   └── 💡 GetRecommendedActivitiesUseCase.cs
│   │
│   ├── 📁 Progress/
│   │   ├── 📊 RecordProgressUseCase.cs
│   │   ├── 📈 GetProgressReportUseCase.cs
│   │   └── 🎯 UpdateDifficultyUseCase.cs
│   │
│   └── 📁 User/
│       ├── 🔐 AuthenticateParentUseCase.cs
│       ├── ⚙️ UpdateParentalControlsUseCase.cs
│       └── 💳 ManageSubscriptionUseCase.cs
│
├── 📁 Services/                        # Application services
│   ├── 📚 IEducationalContentService.cs
│   ├── 📊 IProgressTrackingService.cs
│   ├── 🎯 IActivityExecutionService.cs
│   ├── 🔔 INotificationService.cs
│   ├── 🎵 IAudioInstructionService.cs
│   └── 🔒 IParentalControlService.cs
│
├── 📁 DTOs/                           # Data Transfer Objects
│   ├── 📁 Child/
│   │   ├── 👶 ChildDto.cs
│   │   ├── 📝 CreateChildRequest.cs
│   │   └── 🔄 UpdateChildRequest.cs
│   │
│   ├── 📁 Activity/
│   │   ├── 🎯 ActivityDto.cs
│   │   ├── 📊 ActivityResultDto.cs
│   │   └── 💡 ActivityRecommendationDto.cs
│   │
│   └── 📁 Progress/
│       ├── 📊 ProgressDto.cs
│       ├── 📈 ProgressReportDto.cs
│       └── 🎯 DifficultyAdjustmentDto.cs
│
├── 📁 Commands/                       # CQRS Command objects
│   ├── 🎯 StartActivityCommand.cs
│   ├── 📊 RecordProgressCommand.cs
│   ├── 👶 CreateChildCommand.cs
│   └── 🔒 UpdateParentalControlsCommand.cs
│
├── 📁 Queries/                        # CQRS Query objects
│   ├── 📋 GetActivitiesQuery.cs
│   ├── 📊 GetProgressReportQuery.cs
│   ├── 💡 GetRecommendationsQuery.cs
│   └── 👶 GetChildProfileQuery.cs
│
├── 📁 Handlers/                       # Command and query handlers
│   ├── 📁 Commands/
│   │   ├── 🎯 StartActivityCommandHandler.cs
│   │   ├── 📊 RecordProgressCommandHandler.cs
│   │   └── 👶 CreateChildCommandHandler.cs
│   │
│   └── 📁 Queries/
│       ├── 📋 GetActivitiesQueryHandler.cs
│       ├── 📊 GetProgressReportQueryHandler.cs
│       └── 💡 GetRecommendationsQueryHandler.cs
│
├── 📁 Interfaces/                     # Repository and service interfaces
│   ├── 📁 Repositories/
│   │   ├── 👶 IChildRepository.cs
│   │   ├── 🎯 IActivityRepository.cs
│   │   ├── 📊 IProgressRepository.cs
│   │   └── 👥 IUserRepository.cs
│   │
│   └── 📁 Services/
│       ├── 🔔 IEmailService.cs
│       ├── 🔐 IAuthenticationService.cs
│       └── 📱 IPushNotificationService.cs
│
├── 📁 Validators/                     # Input validation
│   ├── 👶 CreateChildRequestValidator.cs
│   ├── 🎯 StartActivityCommandValidator.cs
│   └── 📊 RecordProgressCommandValidator.cs
│
├── 📁 Mappings/                       # AutoMapper profiles
│   ├── 👶 ChildMappingProfile.cs
│   ├── 🎯 ActivityMappingProfile.cs
│   └── 📊 ProgressMappingProfile.cs
│
└── 📁 Common/                         # Common application concerns
    ├── 🔧 IUseCase.cs                 # Use case interface
    ├── 📄 Result.cs                   # Result pattern
    ├── ✅ ValidationResult.cs         # Validation result
    ├── 📄 PagedResult.cs              # Pagination
    └── 🔍 SearchCriteria.cs           # Search parameters
```

### 🛠️ Infrastructure Layer (`EduPlayKids.Infrastructure/`)

Implements external concerns like data access, file system, and services.

```
EduPlayKids.Infrastructure/
├── 📁 Database/                       # Entity Framework Core
│   ├── 🗄️ EduPlayKidsDbContext.cs    # Main database context
│   ├── 📁 Configurations/            # Entity configurations
│   │   ├── 👶 ChildConfiguration.cs
│   │   ├── 🎯 ActivityConfiguration.cs
│   │   ├── 📊 ProgressConfiguration.cs
│   │   └── 👥 UserConfiguration.cs
│   │
│   ├── 📁 Migrations/                 # EF Core migrations
│   │   ├── 🎬 20241001_InitialCreate.cs
│   │   ├── 🎯 20241002_AddActivities.cs
│   │   └── 📊 20241003_AddProgressTracking.cs
│   │
│   ├── 📁 Repositories/               # Repository implementations
│   │   ├── 🏢 BaseRepository.cs       # Generic repository base
│   │   ├── 👶 ChildRepository.cs
│   │   ├── 🎯 ActivityRepository.cs
│   │   ├── 📊 ProgressRepository.cs
│   │   └── 👥 UserRepository.cs
│   │
│   ├── 📁 Seeders/                    # Data seeding
│   │   ├── 🌱 DatabaseSeeder.cs       # Main seeder
│   │   ├── 🎯 ActivitySeeder.cs       # Educational activities
│   │   ├── 📚 SubjectSeeder.cs        # Subject areas
│   │   └── 🎵 ContentAssetSeeder.cs   # Audio/visual assets
│   │
│   └── 📁 Extensions/                 # EF Core extensions
│       ├── 🔧 DbContextExtensions.cs
│       └── 📊 QueryExtensions.cs
│
├── 📁 Services/                       # Infrastructure services
│   ├── 🎵 AudioService.cs             # Audio playback
│   ├── 📁 FileSystemService.cs        # File operations
│   ├── 🔐 EncryptionService.cs        # Data encryption
│   ├── 🌐 LocalizationService.cs      # Multi-language support
│   ├── 📝 LoggingService.cs           # Structured logging
│   ├── 🔔 NotificationService.cs      # Push notifications
│   └── 📊 AnalyticsService.cs         # Privacy-compliant analytics
│
├── 📁 Storage/                        # File and asset management
│   ├── 📁 Audio/                      # Audio file management
│   │   ├── 🎵 AudioFileManager.cs
│   │   ├── 🎧 AudioCacheService.cs
│   │   └── 🎚️ AudioCompressionService.cs
│   │
│   ├── 📁 Images/                     # Image asset management
│   │   ├── 🖼️ ImageFileManager.cs
│   │   ├── 🗜️ ImageCompressionService.cs
│   │   └── 📐 ImageResizingService.cs
│   │
│   └── 📁 Content/                    # Educational content storage
│       ├── 📚 ContentPackManager.cs
│       ├── 🎯 ActivityContentLoader.cs
│       └── 🌐 LocalizedContentProvider.cs
│
├── 📁 Security/                       # Security implementations
│   ├── 🔐 ChildDataProtectionService.cs
│   ├── 🔑 ParentalControlService.cs
│   ├── 🛡️ DataEncryptionService.cs
│   └── 🔒 SecureStorageService.cs
│
├── 📁 Platform/                       # Platform-specific implementations
│   ├── 📁 Android/
│   │   ├── 🎵 AndroidAudioService.cs
│   │   ├── 📳 AndroidHapticService.cs
│   │   └── 🔔 AndroidNotificationService.cs
│   │
│   ├── 📁 iOS/                        # Future iOS support
│   │   ├── 🎵 iOSAudioService.cs
│   │   └── 📳 iOSHapticService.cs
│   │
│   └── 📁 Shared/                     # Cross-platform implementations
│       ├── 🎵 CrossPlatformAudioService.cs
│       └── 🔔 CrossPlatformNotificationService.cs
│
└── 📁 Common/                         # Infrastructure utilities
    ├── 🔧 BaseRepository.cs           # Generic repository pattern
    ├── 🔄 UnitOfWork.cs               # Unit of work pattern
    ├── 🔧 ServiceCollectionExtensions.cs # DI registration
    ├── ⚙️ ConfigurationExtensions.cs   # Configuration helpers
    └── 📊 HealthCheckExtensions.cs     # Health check setup
```

### 📱 Presentation Layer (`EduPlayKids.Presentation/`)

The .NET MAUI application with UI, ViewModels, and presentation logic.

```
EduPlayKids.Presentation/
├── 📄 App.xaml                        # Application definition
├── 📄 App.xaml.cs                     # Application startup logic
├── 📄 AppShell.xaml                   # Shell navigation structure
├── 📄 AppShell.xaml.cs                # Shell navigation logic
├── 📄 MauiProgram.cs                  # Dependency injection setup
│
├── 📁 Views/                          # XAML pages and controls
│   ├── 📁 Welcome/                    # Welcome and onboarding
│   │   ├── 🏠 WelcomePage.xaml
│   │   ├── 🎂 AgeSelectionPage.xaml
│   │   └── 🌐 LanguageSelectionPage.xaml
│   │
│   ├── 📁 Dashboard/                  # Main navigation screens
│   │   ├── 🏠 DashboardPage.xaml      # Main subject selection
│   │   └── 📊 ProgressPage.xaml       # Progress overview
│   │
│   ├── 📁 Activities/                 # Educational activities
│   │   ├── 📁 Mathematics/
│   │   │   ├── 🔢 MathematicsPage.xaml
│   │   │   ├── 🔢 CountingActivityPage.xaml
│   │   │   ├── ➕ AdditionActivityPage.xaml
│   │   │   └── 🔺 ShapesActivityPage.xaml
│   │   │
│   │   ├── 📁 Reading/
│   │   │   ├── 📖 ReadingPage.xaml
│   │   │   ├── 🔤 AlphabetActivityPage.xaml
│   │   │   ├── 🔊 PhonicsPracticePage.xaml
│   │   │   └── 👁️ SightWordsPage.xaml
│   │   │
│   │   ├── 📁 Science/
│   │   │   ├── 🔬 SciencePage.xaml
│   │   │   ├── 🐛 AnimalsActivityPage.xaml
│   │   │   └── 🌱 PlantsActivityPage.xaml
│   │   │
│   │   └── 📁 Common/
│   │       ├── 🎯 ActivityExecutionPage.xaml
│   │       ├── 📊 ActivityResultPage.xaml
│   │       └── 💡 ActivityInstructionsPage.xaml
│   │
│   ├── 📁 Parental/                   # Parent/guardian screens
│   │   ├── 🔐 ParentalControlsPage.xaml
│   │   ├── 📊 ProgressReportPage.xaml
│   │   ├── ⚙️ SettingsPage.xaml
│   │   └── 💳 SubscriptionPage.xaml
│   │
│   └── 📁 Common/                     # Shared UI components
│       ├── 🎨 SplashPage.xaml
│       ├── ❌ ErrorPage.xaml
│       └── 📶 OfflinePage.xaml
│
├── 📁 ViewModels/                     # MVVM ViewModels
│   ├── 📁 Base/
│   │   ├── 🎯 BaseViewModel.cs        # Base ViewModel with common logic
│   │   ├── 🏠 BasePageViewModel.cs    # Page-specific base
│   │   └── 🔄 AsyncViewModel.cs       # Async operation support
│   │
│   ├── 📁 Welcome/
│   │   ├── 🏠 WelcomeViewModel.cs
│   │   ├── 🎂 AgeSelectionViewModel.cs
│   │   └── 🌐 LanguageSelectionViewModel.cs
│   │
│   ├── 📁 Dashboard/
│   │   ├── 🏠 DashboardViewModel.cs
│   │   └── 📊 ProgressViewModel.cs
│   │
│   ├── 📁 Activities/
│   │   ├── 🔢 MathematicsViewModel.cs
│   │   ├── 📖 ReadingViewModel.cs
│   │   ├── 🔬 ScienceViewModel.cs
│   │   ├── 🎯 ActivityExecutionViewModel.cs
│   │   └── 📊 ActivityResultViewModel.cs
│   │
│   ├── 📁 Parental/
│   │   ├── 🔐 ParentalControlsViewModel.cs
│   │   ├── 📊 ProgressReportViewModel.cs
│   │   └── ⚙️ SettingsViewModel.cs
│   │
│   └── 📁 Common/
│       ├── 🎯 ActivityViewModel.cs    # Individual activity model
│       ├── 👶 ChildViewModel.cs       # Child profile model
│       └── 📊 ProgressItemViewModel.cs # Progress item model
│
├── 📁 Controls/                       # Custom controls
│   ├── 👆 ChildFriendlyButton.cs      # Large touch targets
│   ├── 🎵 AudioInstructionPlayer.cs   # Audio instruction control
│   ├── 📊 ProgressIndicator.cs        # Visual progress indicator
│   ├── ⭐ StarRating.cs               # Star rating display
│   ├── 🎮 ActivityTile.cs             # Activity selection tile
│   ├── 🎯 TouchTargetFrame.cs         # Enhanced touch area
│   └── 🌈 ChildFriendlyEntry.cs       # Input controls for children
│
├── 📁 Converters/                     # Value converters
│   ├── 🔢 AgeToStringConverter.cs
│   ├── 📊 ScoreToStarsConverter.cs
│   ├── 🎨 DifficultyToColorConverter.cs
│   ├── 🌐 LanguageToFlagConverter.cs
│   └── ⏰ TimeSpanToStringConverter.cs
│
├── 📁 Behaviors/                      # XAML behaviors
│   ├── 👆 ChildSafeTapBehavior.cs     # Prevent accidental taps
│   ├── 🎵 AudioFeedbackBehavior.cs    # Audio on interaction
│   ├── 📳 HapticFeedbackBehavior.cs   # Tactile feedback
│   └── 🔍 ValidationBehavior.cs       # Input validation
│
├── 📁 Resources/                      # Application resources
│   ├── 📁 Styles/
│   │   ├── 🎨 Colors.xaml             # Color definitions
│   │   ├── 🔤 Fonts.xaml              # Font definitions
│   │   ├── 📏 Dimensions.xaml         # Size and spacing
│   │   ├── 🎯 Buttons.xaml            # Button styles
│   │   ├── 📄 Pages.xaml              # Page styles
│   │   └── 🎮 Activities.xaml         # Activity-specific styles
│   │
│   ├── 📁 Images/                     # Static images
│   │   ├── 🦁 leo_mascot.png          # Leo the Lion mascot
│   │   ├── 📚 subject_icons/          # Subject category icons
│   │   ├── 🏆 achievement_badges/     # Achievement icons
│   │   └── 🎨 ui_elements/            # UI decoration elements
│   │
│   ├── 📁 Raw/                        # Raw assets (audio, etc.)
│   │   ├── 📁 audio/
│   │   │   ├── 📁 en/                 # English audio files
│   │   │   │   ├── 🎵 instructions/
│   │   │   │   ├── 👏 feedback/
│   │   │   │   └── 🎶 background/
│   │   │   │
│   │   │   └── 📁 es/                 # Spanish audio files
│   │   │       ├── 🎵 instructions/
│   │   │       ├── 👏 feedback/
│   │   │       └── 🎶 background/
│   │   │
│   │   └── 📁 data/                   # Static data files
│   │       ├── 📚 educational_content.json
│   │       ├── 🎯 activity_definitions.json
│   │       └── 🌐 localization.json
│   │
│   └── 📁 Fonts/                      # Custom fonts
│       ├── 📝 Nunito-Regular.ttf      # Primary font
│       ├── 📝 Nunito-Bold.ttf         # Bold variant
│       └── 📝 Nunito-ExtraBold.ttf    # Extra bold for titles
│
├── 📁 Services/                       # Presentation services
│   ├── 🧭 NavigationService.cs        # Page navigation
│   ├── 💬 DialogService.cs            # Child-friendly dialogs
│   ├── 🔔 ToastService.cs             # Non-intrusive notifications
│   ├── 🎵 AudioFeedbackService.cs     # UI audio feedback
│   └── 📳 HapticFeedbackService.cs    # Tactile feedback
│
├── 📁 Helpers/                        # Utility classes
│   ├── 👆 TouchHelper.cs              # Touch interaction utilities
│   ├── 🎨 ColorHelper.cs              # Color manipulation
│   ├── 📏 SizeHelper.cs               # Child-appropriate sizing
│   ├── 🌐 LocalizationHelper.cs       # Localization utilities
│   └── 🎯 ActivityHelper.cs           # Activity management utilities
│
└── 📁 Platforms/                      # Platform-specific code
    ├── 📁 Android/
    │   ├── 📄 AndroidManifest.xml      # Android permissions
    │   ├── 🚀 MainActivity.cs          # Android entry point
    │   ├── 🎨 MainApplication.cs       # Android application class
    │   ├── 📁 Resources/               # Android resources
    │   └── 📁 Services/                # Android-specific services
    │
    ├── 📁 iOS/                         # Future iOS support
    │   ├── 📄 Info.plist               # iOS configuration
    │   ├── 🚀 AppDelegate.cs           # iOS entry point
    │   └── 📁 Resources/               # iOS resources
    │
    └── 📁 Windows/                     # Future Windows support
        ├── 📄 app.manifest             # Windows configuration
        └── 🚀 App.xaml                 # Windows entry point
```

## 🧪 Test Structure (`tests/`)

Comprehensive testing structure following the same architectural layers:

```
tests/
├── 📁 EduPlayKids.Domain.Tests/       # Domain layer tests
│   ├── 📁 Entities/
│   │   ├── 👶 ChildTests.cs
│   │   ├── 🎯 ActivityTests.cs
│   │   └── 📊 ProgressTests.cs
│   │
│   ├── 📁 ValueObjects/
│   │   ├── 🎂 AgeTests.cs
│   │   ├── ⭐ ScoreTests.cs
│   │   └── 📊 DifficultyTests.cs
│   │
│   └── 📁 Services/
│       ├── 🧮 ProgressCalculationServiceTests.cs
│       └── 💡 ActivityRecommendationServiceTests.cs
│
├── 📁 EduPlayKids.Application.Tests/  # Application layer tests
│   ├── 📁 UseCases/
│   │   ├── 🎯 StartActivityUseCaseTests.cs
│   │   ├── 📊 RecordProgressUseCaseTests.cs
│   │   └── 👶 CreateChildUseCaseTests.cs
│   │
│   ├── 📁 Services/
│   │   ├── 📚 EducationalContentServiceTests.cs
│   │   └── 📊 ProgressTrackingServiceTests.cs
│   │
│   └── 📁 Validators/
│       ├── 👶 CreateChildRequestValidatorTests.cs
│       └── 🎯 StartActivityCommandValidatorTests.cs
│
├── 📁 EduPlayKids.Infrastructure.Tests/ # Infrastructure tests
│   ├── 📁 Database/
│   │   ├── 🗄️ EduPlayKidsDbContextTests.cs
│   │   ├── 👶 ChildRepositoryTests.cs
│   │   └── 🎯 ActivityRepositoryTests.cs
│   │
│   ├── 📁 Services/
│   │   ├── 🎵 AudioServiceTests.cs
│   │   ├── 🔐 EncryptionServiceTests.cs
│   │   └── 🌐 LocalizationServiceTests.cs
│   │
│   └── 📁 Integration/
│       ├── 🗄️ DatabaseIntegrationTests.cs
│       └── 📁 Api/
│           └── 🌐 WebApiIntegrationTests.cs
│
├── 📁 EduPlayKids.Presentation.Tests/ # Presentation layer tests
│   ├── 📁 ViewModels/
│   │   ├── 🏠 DashboardViewModelTests.cs
│   │   ├── 🔢 MathematicsViewModelTests.cs
│   │   └── 🎯 ActivityExecutionViewModelTests.cs
│   │
│   ├── 📁 Controls/
│   │   ├── 👆 ChildFriendlyButtonTests.cs
│   │   └── ⭐ StarRatingTests.cs
│   │
│   └── 📁 Services/
│       ├── 🧭 NavigationServiceTests.cs
│       └── 💬 DialogServiceTests.cs
│
├── 📁 EduPlayKids.EndToEnd.Tests/     # End-to-end tests
│   ├── 📁 Scenarios/
│   │   ├── 🎯 CompleteActivityScenarioTests.cs
│   │   ├── 👶 ChildOnboardingScenarioTests.cs
│   │   └── 📊 ProgressTrackingScenarioTests.cs
│   │
│   └── 📁 Helpers/
│       ├── 🤖 TestAppHelper.cs
│       └── 📱 DeviceSimulatorHelper.cs
│
└── 📁 Common/                         # Shared test utilities
    ├── 🏭 TestDataFactory.cs          # Test data generation
    ├── 🎭 MockServiceProvider.cs      # Service mocking
    ├── 🧪 TestFixtureBase.cs          # Base test class
    └── 📊 AssertionExtensions.cs      # Custom assertions
```

## 🛠️ Tools and Utilities (`tools/`)

Development and build utilities:

```
tools/
├── 📁 DataSeeder/                     # Database seeding utility
│   ├── 🌱 Program.cs                  # Seeder entry point
│   ├── 🎯 ActivitySeeder.cs           # Activity data seeding
│   ├── 📚 ContentSeeder.cs            # Educational content seeding
│   └── 🎵 AudioAssetSeeder.cs         # Audio asset organization
│
├── 📁 ContentGenerator/               # Educational content generation
│   ├── 🎯 ActivityGenerator.cs        # Generate activity definitions
│   ├── 📊 ProgressionGenerator.cs     # Generate learning progressions
│   └── 🌐 LocalizationGenerator.cs    # Generate localized content
│
├── 📁 AssetOptimizer/                 # Asset optimization tools
│   ├── 🖼️ ImageOptimizer.cs           # Optimize images for mobile
│   ├── 🎵 AudioOptimizer.cs           # Compress audio files
│   └── 📦 AssetPackager.cs            # Package assets for distribution
│
├── 📁 BuildScripts/                   # Build automation
│   ├── 🔨 build.ps1                   # PowerShell build script
│   ├── 🔨 build.sh                    # Bash build script
│   ├── 🧪 test.ps1                    # Test execution script
│   └── 🚀 deploy.ps1                  # Deployment script
│
└── 📁 Analytics/                      # Development analytics
    ├── 📊 CodeMetrics.cs              # Code quality metrics
    ├── 📈 TestCoverage.cs             # Test coverage analysis
    └── 🔍 DependencyAnalyzer.cs       # Dependency analysis
```

## 📚 Documentation Structure (`docs/`)

Comprehensive documentation organized by audience and topic:

```
docs/
├── 📄 README.md                       # Documentation overview
├── 📄 DOCUMENTATION-INDEX.md          # Central navigation
│
├── 📁 technical/                      # Developer documentation
│   ├── 📁 architecture/               # System architecture
│   ├── 📁 development/                # Development guides
│   ├── 📁 setup-and-installation/     # Setup instructions
│   ├── 📁 api-documentation/          # API references
│   └── 📁 deployment/                 # Deployment guides
│
├── 📁 user-guides/                    # End-user documentation
│   ├── 📁 parents/                    # Parent guides
│   ├── 📁 teachers/                   # Educator guides
│   └── 📁 children/                   # Child-friendly instructions
│
├── 📁 compliance/                     # Legal and compliance
│   ├── 📄 COPPA-COMPLIANCE.md         # US children's privacy law
│   ├── 📄 GDPR-K-COMPLIANCE.md        # EU child data protection
│   └── 📄 PRIVACY-POLICY.md           # Privacy policy
│
├── 📁 testing/                        # QA and testing
│   ├── 📄 TEST-STRATEGY.md            # Overall testing approach
│   ├── 📄 CHILD-USABILITY-TESTING.md  # Child-specific testing
│   └── 📄 ACCESSIBILITY-TESTING.md    # Accessibility compliance
│
└── 📁 design/                         # Design documentation
    ├── 📁 ui-ux/                     # User interface design
    ├── 📁 content/                   # Educational content specs
    └── 📁 assets/                    # Design asset specifications
```

## 🔄 File Naming Conventions

### C# Files
- **Entities**: `ChildEntity.cs`, `ActivityEntity.cs`
- **Services**: `IProgressTrackingService.cs`, `ProgressTrackingService.cs`
- **ViewModels**: `DashboardViewModel.cs`, `ActivityExecutionViewModel.cs`
- **Pages**: `DashboardPage.xaml`, `ActivityExecutionPage.xaml`
- **Tests**: `ChildTests.cs`, `ProgressTrackingServiceTests.cs`

### Resource Files
- **Images**: `leo_mascot_happy.png`, `subject_mathematics_icon.svg`
- **Audio**: `instruction_count_to_ten_en.mp3`, `feedback_great_job_es.mp3`
- **Styles**: `ChildFriendlyButtons.xaml`, `EducationalColors.xaml`

### Configuration Files
- **Environment**: `appsettings.Development.json`, `appsettings.Production.json`
- **Platform**: `AndroidManifest.xml`, `Info.plist`

## 🎯 Child-Specific Architectural Considerations

### 👆 Touch-First Design
- **Large Touch Targets**: Minimum 80dp for child-friendly interaction
- **Generous Spacing**: 16dp minimum between interactive elements
- **Visual Feedback**: Immediate response to all touch interactions

### 🎵 Audio-Centric Architecture
- **Bilingual Support**: Spanish and English audio for all instructions
- **Audio Caching**: Efficient loading and memory management
- **Fallback Strategy**: Text display if audio fails

### 🔒 Privacy-by-Design
- **Local Storage**: All data stored locally on device
- **No External Communication**: Offline-first architecture
- **Parental Controls**: PIN-protected settings and progress access

### 📊 Progressive Learning
- **Age-Appropriate Content**: Activities filtered by child's age
- **Mastery-Based Progression**: Unlock new content based on completion
- **Adaptive Difficulty**: Automatic adjustment based on performance

## 🚀 Getting Started with the Structure

### For New Developers
1. **Start with Domain Layer**: Understand business entities and rules
2. **Study Application Layer**: Learn use cases and application flow
3. **Explore Presentation Layer**: See how UI connects to business logic
4. **Review Infrastructure**: Understand data access and external services

### For Feature Development
1. **Create Domain Entities**: Define new business concepts
2. **Add Application Use Cases**: Implement business workflows
3. **Build Infrastructure Services**: Add data access and external integrations
4. **Create UI Components**: Build child-friendly user interfaces
5. **Write Comprehensive Tests**: Ensure reliability and child safety

### Best Practices
- **Follow Naming Conventions**: Use consistent, descriptive names
- **Maintain Layer Separation**: Respect architectural boundaries
- **Child Safety First**: Always consider child-specific requirements
- **Document Everything**: Maintain clear, comprehensive documentation

---

**Related Documentation**:
- [Phase 4 Implementation Guide](phase-4-implementation-guide.md)
- [Developer Setup Instructions](../setup-and-installation/developer-setup.md)
- [Architecture Overview](../architecture/overview.md)
- [Coding Standards](coding-standards.md)

**Last Updated**: September 2025
**Architecture**: Clean Architecture + MVVM
**Target Platform**: .NET MAUI (Android Primary)