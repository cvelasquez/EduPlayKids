namespace EduPlayKids.Application.Models.Audio;

/// <summary>
/// Defines the types of audio content in the EduPlayKids application.
/// Used for categorizing audio for appropriate volume control, priority handling, and parental settings.
/// </summary>
public enum AudioType
{
    /// <summary>
    /// UI interaction sounds like button clicks, page transitions, and navigation feedback.
    /// Typically short, low volume sounds that provide immediate interaction feedback.
    /// </summary>
    UIInteraction,

    /// <summary>
    /// Educational content narration including activity instructions and question reading.
    /// High priority audio that should interrupt other sounds for clear educational delivery.
    /// </summary>
    Instruction,

    /// <summary>
    /// Positive feedback sounds for correct answers and successful interactions.
    /// Encouraging audio designed to reinforce learning and boost child confidence.
    /// </summary>
    SuccessFeedback,

    /// <summary>
    /// Gentle correction sounds for incorrect answers or mistakes.
    /// Supportive audio that guides children without discouraging continued learning.
    /// </summary>
    ErrorFeedback,

    /// <summary>
    /// Activity completion celebrations including star ratings and achievement sounds.
    /// Celebratory audio that provides closure and satisfaction for completed learning tasks.
    /// </summary>
    Completion,

    /// <summary>
    /// Background music that plays during educational activities.
    /// Ambient audio that enhances learning environment without distracting from content.
    /// </summary>
    BackgroundMusic,

    /// <summary>
    /// Achievement unlock notifications and special recognition sounds.
    /// Special audio for milestones, level completions, and significant accomplishments.
    /// </summary>
    Achievement,

    /// <summary>
    /// Mascot (Leo the Lion) voice content and character interactions.
    /// Character-specific audio that provides personality and engagement to the learning experience.
    /// </summary>
    Mascot,

    /// <summary>
    /// Encouraging audio to motivate children and boost confidence during learning activities.
    /// Positive reinforcement sounds that support continued effort and engagement.
    /// </summary>
    Encouragement,

    /// <summary>
    /// Gentle error correction audio that guides children without discouraging learning.
    /// Supportive audio feedback for incorrect answers that promotes learning from mistakes.
    /// </summary>
    ErrorCorrection
}

/// <summary>
/// Defines the priority levels for audio playback management.
/// Higher priority audio can interrupt lower priority audio to ensure important content is heard.
/// </summary>
public enum AudioPriority
{
    /// <summary>
    /// Low priority audio that can be easily interrupted (background music, ambient sounds).
    /// </summary>
    Low = 1,

    /// <summary>
    /// Normal priority audio for standard interactions and feedback sounds.
    /// </summary>
    Normal = 2,

    /// <summary>
    /// Medium priority audio for educational content and moderate feedback.
    /// </summary>
    Medium = 2,

    /// <summary>
    /// High priority audio for important feedback and completion celebrations.
    /// </summary>
    High = 3,

    /// <summary>
    /// Critical priority audio for instructions and educational content that must be heard.
    /// </summary>
    Critical = 4
}

/// <summary>
/// Defines the intensity levels for audio feedback.
/// Used to match feedback intensity to the child's achievement level and emotional needs.
/// </summary>
public enum FeedbackIntensity
{
    /// <summary>
    /// Soft, gentle feedback for subtle encouragement or minor corrections.
    /// Appropriate for sensitive children or early learning stages.
    /// </summary>
    Soft,

    /// <summary>
    /// Medium intensity feedback for standard learning interactions.
    /// Balanced approach suitable for most educational activities.
    /// </summary>
    Medium,

    /// <summary>
    /// High intensity feedback for major achievements and celebrations.
    /// Enthusiastic audio for significant accomplishments and breakthroughs.
    /// </summary>
    High
}

/// <summary>
/// Defines the types of UI interactions that trigger audio feedback.
/// Ensures appropriate audio response for different user interface elements.
/// </summary>
public enum UIInteractionType
{
    /// <summary>
    /// Standard button press or tap interactions.
    /// </summary>
    ButtonPress,

    /// <summary>
    /// Navigation between pages or sections.
    /// </summary>
    PageTransition,

    /// <summary>
    /// Menu opening or closing actions.
    /// </summary>
    MenuToggle,

    /// <summary>
    /// Item selection in lists or grids.
    /// </summary>
    ItemSelection,

    /// <summary>
    /// Drag and drop interactions during educational activities.
    /// </summary>
    DragDrop,

    /// <summary>
    /// Touch and hold gestures for special functions.
    /// </summary>
    LongPress,

    /// <summary>
    /// Swipe gestures for navigation or interaction.
    /// </summary>
    Swipe,

    /// <summary>
    /// Modal dialog appearance or dismissal.
    /// </summary>
    ModalDialog
}

/// <summary>
/// Defines the current playback state of audio content.
/// Used for managing audio conflicts and coordinating user interface states.
/// </summary>
public enum AudioPlaybackState
{
    /// <summary>
    /// No audio is currently loaded or playing.
    /// </summary>
    Stopped,

    /// <summary>
    /// Audio is currently playing.
    /// </summary>
    Playing,

    /// <summary>
    /// Audio is paused and can be resumed from current position.
    /// </summary>
    Paused,

    /// <summary>
    /// Audio is loading or buffering before playback.
    /// </summary>
    Loading,

    /// <summary>
    /// Audio encountered an error and cannot play.
    /// </summary>
    Error
}

/// <summary>
/// Defines types of audio interruptions that can occur on mobile devices.
/// Used for proper handling of platform-specific audio interruption scenarios.
/// </summary>
public enum AudioInterruption
{
    /// <summary>
    /// Incoming phone call requiring audio to be paused.
    /// </summary>
    PhoneCall,

    /// <summary>
    /// System notification with audio content.
    /// </summary>
    Notification,

    /// <summary>
    /// Another app requesting audio focus.
    /// </summary>
    OtherApp,

    /// <summary>
    /// System audio like alarms or emergency alerts.
    /// </summary>
    SystemAudio,

    /// <summary>
    /// Hardware events like headphone disconnection.
    /// </summary>
    HardwareChange,

    /// <summary>
    /// Application moving to background state.
    /// </summary>
    AppBackground,

    /// <summary>
    /// Audio interruption has ended and app can resume.
    /// </summary>
    InterruptionEnded
}

/// <summary>
/// Defines types of audio sessions for platform-specific configuration.
/// Optimizes audio behavior for different types of educational content delivery.
/// </summary>
public enum AudioSessionType
{
    /// <summary>
    /// Standard playback session for general audio content.
    /// </summary>
    Playback,

    /// <summary>
    /// Playback with speech content that should duck other audio.
    /// </summary>
    PlaybackWithSpeech,

    /// <summary>
    /// Ambient audio that should mix with other apps.
    /// </summary>
    Ambient,

    /// <summary>
    /// Educational content that requires exclusive audio focus.
    /// </summary>
    Educational,

    /// <summary>
    /// Game audio with low latency requirements.
    /// </summary>
    Gaming
}

/// <summary>
/// Defines language codes supported by the audio system.
/// Used for bilingual audio content selection and localization.
/// </summary>
public enum SupportedLanguage
{
    /// <summary>
    /// English (United States) - Primary language.
    /// </summary>
    English,

    /// <summary>
    /// Spanish (International) - Secondary language for Hispanic families.
    /// </summary>
    Spanish
}

/// <summary>
/// Alias for SupportedLanguage to maintain compatibility with existing code.
/// Defines the audio language settings for bilingual content delivery.
/// </summary>
public enum AudioLanguage
{
    /// <summary>
    /// English (United States) - Primary language.
    /// </summary>
    English = 0,

    /// <summary>
    /// Spanish (International) - Secondary language for Hispanic families.
    /// </summary>
    Spanish = 1
}

/// <summary>
/// Defines age groups for age-appropriate audio content selection.
/// Ensures audio complexity and pacing matches developmental stages.
/// </summary>
public enum AgeGroup
{
    /// <summary>
    /// Pre-K ages 3-4: Simple language, slower pacing, more repetition.
    /// </summary>
    PreK,

    /// <summary>
    /// Kindergarten age 5: Developing vocabulary, moderate pacing.
    /// </summary>
    Kindergarten,

    /// <summary>
    /// Primary grades 6-8: Advanced vocabulary, normal pacing.
    /// </summary>
    Primary
}