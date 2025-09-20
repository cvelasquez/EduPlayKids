using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduPlayKids.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Subjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NameEn = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    NameEs = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    DescriptionEn = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    DescriptionEs = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    SubjectCode = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    IconId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    PrimaryColor = table.Column<string>(type: "TEXT", maxLength: 7, nullable: true),
                    SecondaryColor = table.Column<string>(type: "TEXT", maxLength: 7, nullable: true),
                    DisplayOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    MinAge = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxAge = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    RequiresPremium = table.Column<bool>(type: "INTEGER", nullable: false),
                    CurriculumStandards = table.Column<string>(type: "TEXT", nullable: true),
                    LearningObjectives = table.Column<string>(type: "TEXT", nullable: true),
                    Prerequisites = table.Column<string>(type: "TEXT", nullable: true),
                    EstimatedCompletionHours = table.Column<decimal>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subjects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    FullName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    ParentalPin = table.Column<string>(type: "TEXT", maxLength: 4, nullable: false),
                    PreferredLanguage = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    CountryCode = table.Column<string>(type: "TEXT", maxLength: 3, nullable: true),
                    TimeZone = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    IsEmailVerified = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsPremium = table.Column<bool>(type: "INTEGER", nullable: false),
                    PremiumExpiresAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastLoginAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    FreeTrialExpiresAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ParentalControlsEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    DailyUsageLimitMinutes = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedBy = table.Column<int>(type: "INTEGER", nullable: true),
                    UpdatedBy = table.Column<int>(type: "INTEGER", nullable: true),
                    Metadata = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Achievements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NameEn = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    NameEs = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    DescriptionEn = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    DescriptionEs = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Category = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    AchievementType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    BadgeIcon = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    BadgeColor = table.Column<string>(type: "TEXT", maxLength: 7, nullable: true),
                    Points = table.Column<int>(type: "INTEGER", nullable: false),
                    Criteria = table.Column<string>(type: "TEXT", nullable: true),
                    MinAge = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxAge = table.Column<int>(type: "INTEGER", nullable: false),
                    Rarity = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsHidden = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsCrownChallenge = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsRepeatable = table.Column<bool>(type: "INTEGER", nullable: false),
                    DisplayOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    SubjectId = table.Column<int>(type: "INTEGER", nullable: true),
                    CelebrationMessageEn = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    CelebrationMessageEs = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    EarnedCount = table.Column<int>(type: "INTEGER", nullable: false),
                    Metadata = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Achievements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Achievements_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Activities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TitleEn = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    TitleEs = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    DescriptionEn = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    DescriptionEs = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    ActivityType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    DifficultyLevel = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    MinAge = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxAge = table.Column<int>(type: "INTEGER", nullable: false),
                    DisplayOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    EstimatedMinutes = table.Column<int>(type: "INTEGER", nullable: false),
                    LearningObjectives = table.Column<string>(type: "TEXT", nullable: true),
                    Prerequisites = table.Column<string>(type: "TEXT", nullable: true),
                    InstructionEn = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    InstructionEs = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    AudioInstructionEnPath = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    AudioInstructionEsPath = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    ThumbnailPath = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    RequiresPremium = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsCrownChallenge = table.Column<bool>(type: "INTEGER", nullable: false),
                    CurriculumStandards = table.Column<string>(type: "TEXT", nullable: true),
                    ConfigurationData = table.Column<string>(type: "TEXT", nullable: true),
                    SuccessCriteria = table.Column<string>(type: "TEXT", nullable: true),
                    PlayCount = table.Column<int>(type: "INTEGER", nullable: false),
                    AverageCompletionTime = table.Column<decimal>(type: "TEXT", nullable: false),
                    AverageStarRating = table.Column<decimal>(type: "TEXT", nullable: false),
                    SubjectId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Activities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Activities_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Children",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Age = table.Column<int>(type: "INTEGER", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "TEXT", nullable: true),
                    GradeLevel = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    PreferredLanguage = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    LearningProfile = table.Column<string>(type: "TEXT", nullable: true),
                    LearningStreak = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalStarsEarned = table.Column<int>(type: "INTEGER", nullable: false),
                    FavoriteSubjectId = table.Column<int>(type: "INTEGER", nullable: true),
                    LastActivityAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    TotalLearningTimeMinutes = table.Column<int>(type: "INTEGER", nullable: false),
                    CurrentLevel = table.Column<int>(type: "INTEGER", nullable: false),
                    AvatarId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    AudioInstructionsEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    NeedsExtraHelp = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsAdvanced = table.Column<bool>(type: "INTEGER", nullable: false),
                    DifficultyPreference = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedBy = table.Column<int>(type: "INTEGER", nullable: true),
                    UpdatedBy = table.Column<int>(type: "INTEGER", nullable: true),
                    Metadata = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Children", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Children_Subjects_FavoriteSubjectId",
                        column: x => x.FavoriteSubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Children_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    AppLanguage = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    AudioLanguage = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    MasterVolume = table.Column<int>(type: "INTEGER", nullable: false),
                    MusicVolume = table.Column<int>(type: "INTEGER", nullable: false),
                    SoundEffectsVolume = table.Column<int>(type: "INTEGER", nullable: false),
                    VoiceVolume = table.Column<int>(type: "INTEGER", nullable: false),
                    AudioInstructionsEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    BackgroundMusicEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    SoundEffectsEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    HapticFeedbackEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    AnimationSpeed = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    ParentalControlsEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    DailyTimeLimitMinutes = table.Column<int>(type: "INTEGER", nullable: false),
                    BedtimeRestrictionsEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    BedtimeStartHour = table.Column<int>(type: "INTEGER", nullable: false),
                    BedtimeEndHour = table.Column<int>(type: "INTEGER", nullable: false),
                    CrownChallengesEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    DefaultDifficulty = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    AdaptiveLearningEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    ProgressReportsEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    ProgressReportFrequency = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    AchievementNotificationsEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    ReminderNotificationsEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    TimeZone = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    DateFormat = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    AccessibilitySettings = table.Column<string>(type: "TEXT", nullable: true),
                    DataCollectionPreferences = table.Column<string>(type: "TEXT", nullable: true),
                    ChildSafetySettings = table.Column<string>(type: "TEXT", nullable: true),
                    CustomLearningGoals = table.Column<string>(type: "TEXT", nullable: true),
                    NotificationPreferences = table.Column<string>(type: "TEXT", nullable: true),
                    NeedsSync = table.Column<bool>(type: "INTEGER", nullable: false),
                    SettingsVersion = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedBy = table.Column<int>(type: "INTEGER", nullable: true),
                    UpdatedBy = table.Column<int>(type: "INTEGER", nullable: true),
                    Metadata = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Settings_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Subscriptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    PlanType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TrialEndDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PriceCents = table.Column<int>(type: "INTEGER", nullable: false),
                    Currency = table.Column<string>(type: "TEXT", maxLength: 3, nullable: false),
                    BillingCycle = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    NextBillingDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastPaymentDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ExternalSubscriptionId = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    PaymentProvider = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    LastTransactionId = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    AutoRenewalEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    CancelledAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CancellationReason = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    GracePeriodDays = table.Column<int>(type: "INTEGER", nullable: false),
                    GracePeriodEndDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    PaymentRetryAttempts = table.Column<int>(type: "INTEGER", nullable: false),
                    LastPaymentRetryDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    PromotionalData = table.Column<string>(type: "TEXT", nullable: true),
                    FeatureAccess = table.Column<string>(type: "TEXT", nullable: true),
                    UsageLimits = table.Column<string>(type: "TEXT", nullable: true),
                    Metadata = table.Column<string>(type: "TEXT", nullable: true),
                    NeedsSync = table.Column<bool>(type: "INTEGER", nullable: false),
                    SubscriptionVersion = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedBy = table.Column<int>(type: "INTEGER", nullable: true),
                    UpdatedBy = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subscriptions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ActivityQuestions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    QuestionTextEn = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    QuestionTextEs = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    QuestionType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    DisplayOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    Points = table.Column<int>(type: "INTEGER", nullable: false),
                    ConfigurationData = table.Column<string>(type: "TEXT", nullable: true),
                    CorrectAnswer = table.Column<string>(type: "TEXT", nullable: true),
                    ExplanationEn = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    ExplanationEs = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    QuestionAudioEnPath = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    QuestionAudioEsPath = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    ImagePath = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    MediaAssets = table.Column<string>(type: "TEXT", nullable: true),
                    HintEn = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    HintEs = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    HintAudioEnPath = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    HintAudioEsPath = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    TimeLimitSeconds = table.Column<int>(type: "INTEGER", nullable: false),
                    AllowMultipleAttempts = table.Column<bool>(type: "INTEGER", nullable: false),
                    MaxAttempts = table.Column<int>(type: "INTEGER", nullable: false),
                    HintsEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    DifficultyLevel = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    LearningTags = table.Column<string>(type: "TEXT", nullable: true),
                    AccessibilityFeatures = table.Column<string>(type: "TEXT", nullable: true),
                    CorrectAnswerCount = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalAttemptCount = table.Column<int>(type: "INTEGER", nullable: false),
                    ActivityId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivityQuestions_Activities_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "Activities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: true),
                    ChildId = table.Column<int>(type: "INTEGER", nullable: true),
                    Action = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Category = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Severity = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    EntityType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    EntityId = table.Column<int>(type: "INTEGER", nullable: true),
                    OldValues = table.Column<string>(type: "TEXT", nullable: true),
                    NewValues = table.Column<string>(type: "TEXT", nullable: true),
                    IpAddress = table.Column<string>(type: "TEXT", maxLength: 45, nullable: true),
                    UserAgent = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    AppVersion = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    Platform = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    DeviceId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    SessionId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    RequestId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ContextData = table.Column<string>(type: "TEXT", nullable: true),
                    ComplianceData = table.Column<string>(type: "TEXT", nullable: true),
                    Result = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    ErrorMessage = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    DurationMs = table.Column<int>(type: "INTEGER", nullable: true),
                    IsReviewed = table.Column<bool>(type: "INTEGER", nullable: false),
                    ReviewedBy = table.Column<int>(type: "INTEGER", nullable: true),
                    ReviewedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ReviewNotes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    RequiresAttention = table.Column<bool>(type: "INTEGER", nullable: false),
                    RetentionClass = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    RetentionExpiry = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditLogs_Children_ChildId",
                        column: x => x.ChildId,
                        principalTable: "Children",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AuditLogs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Sessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ChildId = table.Column<int>(type: "INTEGER", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DurationSeconds = table.Column<int>(type: "INTEGER", nullable: false),
                    SessionType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    DeviceId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    AppVersion = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    ActivitiesCompleted = table.Column<int>(type: "INTEGER", nullable: false),
                    QuestionsAnswered = table.Column<int>(type: "INTEGER", nullable: false),
                    CorrectAnswers = table.Column<int>(type: "INTEGER", nullable: false),
                    StarsEarned = table.Column<int>(type: "INTEGER", nullable: false),
                    PointsScored = table.Column<int>(type: "INTEGER", nullable: false),
                    AchievementsEarned = table.Column<int>(type: "INTEGER", nullable: false),
                    HintsUsed = table.Column<int>(type: "INTEGER", nullable: false),
                    SubjectsAccessed = table.Column<string>(type: "TEXT", nullable: true),
                    LanguageUsed = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    AudioPlaysCount = table.Column<int>(type: "INTEGER", nullable: false),
                    InteractionPatterns = table.Column<string>(type: "TEXT", nullable: true),
                    EmotionalStates = table.Column<string>(type: "TEXT", nullable: true),
                    PerformanceMetrics = table.Column<string>(type: "TEXT", nullable: true),
                    EndReason = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    ParentalControlsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    DailyLimitMinutes = table.Column<int>(type: "INTEGER", nullable: true),
                    PreviousDailyUsageMinutes = table.Column<int>(type: "INTEGER", nullable: false),
                    ExceededTimeLimit = table.Column<bool>(type: "INTEGER", nullable: false),
                    TechnicalMetadata = table.Column<string>(type: "TEXT", nullable: true),
                    PrivacySettings = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedBy = table.Column<int>(type: "INTEGER", nullable: true),
                    UpdatedBy = table.Column<int>(type: "INTEGER", nullable: true),
                    Metadata = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sessions_Children_ChildId",
                        column: x => x.ChildId,
                        principalTable: "Children",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserAchievements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ChildId = table.Column<int>(type: "INTEGER", nullable: false),
                    AchievementId = table.Column<int>(type: "INTEGER", nullable: false),
                    EarnedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ProgressPercentage = table.Column<int>(type: "INTEGER", nullable: false),
                    CurrentProgress = table.Column<int>(type: "INTEGER", nullable: false),
                    TargetProgress = table.Column<int>(type: "INTEGER", nullable: false),
                    IsEarned = table.Column<bool>(type: "INTEGER", nullable: false),
                    CelebrationShown = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsInProgress = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsVisible = table.Column<bool>(type: "INTEGER", nullable: false),
                    EarnedCount = table.Column<int>(type: "INTEGER", nullable: false),
                    ProgressStartedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    EarnedContext = table.Column<string>(type: "TEXT", nullable: true),
                    EmotionalReaction = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    NeedsSync = table.Column<bool>(type: "INTEGER", nullable: false),
                    PointsEarned = table.Column<int>(type: "INTEGER", nullable: false),
                    BonusMultiplier = table.Column<decimal>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedBy = table.Column<int>(type: "INTEGER", nullable: true),
                    UpdatedBy = table.Column<int>(type: "INTEGER", nullable: true),
                    Metadata = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAchievements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserAchievements_Achievements_AchievementId",
                        column: x => x.AchievementId,
                        principalTable: "Achievements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserAchievements_Children_ChildId",
                        column: x => x.ChildId,
                        principalTable: "Children",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserProgress",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ChildId = table.Column<int>(type: "INTEGER", nullable: false),
                    ActivityId = table.Column<int>(type: "INTEGER", nullable: false),
                    IsCompleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    StarsEarned = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalScore = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxPossibleScore = table.Column<int>(type: "INTEGER", nullable: false),
                    AttemptCount = table.Column<int>(type: "INTEGER", nullable: false),
                    CorrectAnswers = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalQuestions = table.Column<int>(type: "INTEGER", nullable: false),
                    TimeSpentSeconds = table.Column<int>(type: "INTEGER", nullable: false),
                    FirstAttemptAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastAttemptAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    HintsUsed = table.Column<int>(type: "INTEGER", nullable: false),
                    NeededExtraHelp = table.Column<bool>(type: "INTEGER", nullable: false),
                    DifficultyLevel = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    ProgressData = table.Column<string>(type: "TEXT", nullable: true),
                    AnalyticsData = table.Column<string>(type: "TEXT", nullable: true),
                    IsCrownChallenge = table.Column<bool>(type: "INTEGER", nullable: false),
                    EmotionalState = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    NeedsSync = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedBy = table.Column<int>(type: "INTEGER", nullable: true),
                    UpdatedBy = table.Column<int>(type: "INTEGER", nullable: true),
                    Metadata = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProgress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserProgress_Activities_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "Activities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserProgress_Children_ChildId",
                        column: x => x.ChildId,
                        principalTable: "Children",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Achievements_SubjectId",
                table: "Achievements",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_SubjectId",
                table: "Activities",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityQuestions_ActivityId",
                table: "ActivityQuestions",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_ChildId",
                table: "AuditLogs",
                column: "ChildId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_UserId",
                table: "AuditLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Children_FavoriteSubjectId",
                table: "Children",
                column: "FavoriteSubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Children_UserId",
                table: "Children",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_ChildId",
                table: "Sessions",
                column: "ChildId");

            migrationBuilder.CreateIndex(
                name: "IX_Settings_UserId",
                table: "Settings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_UserId",
                table: "Subscriptions",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserAchievements_AchievementId",
                table: "UserAchievements",
                column: "AchievementId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAchievements_ChildId",
                table: "UserAchievements",
                column: "ChildId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProgress_ActivityId",
                table: "UserProgress",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProgress_ChildId",
                table: "UserProgress",
                column: "ChildId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivityQuestions");

            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "Sessions");

            migrationBuilder.DropTable(
                name: "Settings");

            migrationBuilder.DropTable(
                name: "Subscriptions");

            migrationBuilder.DropTable(
                name: "UserAchievements");

            migrationBuilder.DropTable(
                name: "UserProgress");

            migrationBuilder.DropTable(
                name: "Achievements");

            migrationBuilder.DropTable(
                name: "Activities");

            migrationBuilder.DropTable(
                name: "Children");

            migrationBuilder.DropTable(
                name: "Subjects");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
