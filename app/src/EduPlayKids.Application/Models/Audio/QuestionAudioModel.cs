namespace EduPlayKids.Application.Models.Audio;

/// <summary>
/// Model representing audio content for educational questions with support for different question types
/// and accessibility features for children aged 3-8.
/// </summary>
public class QuestionAudioModel
{
    /// <summary>
    /// Unique identifier for the question audio.
    /// </summary>
    public string QuestionId { get; set; } = string.Empty;

    /// <summary>
    /// The question text to be narrated.
    /// </summary>
    public string QuestionText { get; set; } = string.Empty;

    /// <summary>
    /// Type of question (multiple_choice, drag_drop, matching, etc.).
    /// </summary>
    public string QuestionType { get; set; } = string.Empty;

    /// <summary>
    /// Audio narration for the question text in different languages.
    /// Key: language code, Value: audio file path or TTS text
    /// </summary>
    public Dictionary<string, string> QuestionAudio { get; set; } = new();

    /// <summary>
    /// Audio for answer choices if applicable (for multiple choice questions).
    /// Key: choice index, Value: dictionary of language-specific audio
    /// </summary>
    public Dictionary<int, Dictionary<string, string>> AnswerChoiceAudio { get; set; } = new();

    /// <summary>
    /// Audio hints for the question in progressive difficulty levels.
    /// Key: hint level (1=subtle, 2=moderate, 3=direct), Value: language-specific audio
    /// </summary>
    public Dictionary<int, Dictionary<string, string>> HintAudio { get; set; } = new();

    /// <summary>
    /// Visual description audio for accessibility support.
    /// Describes images, diagrams, or visual elements in the question.
    /// </summary>
    public Dictionary<string, string> VisualDescriptionAudio { get; set; } = new();

    /// <summary>
    /// Instructions specific to this question type (how to interact).
    /// </summary>
    public Dictionary<string, string> InteractionInstructions { get; set; } = new();

    /// <summary>
    /// Age group this question audio is optimized for.
    /// </summary>
    public AgeGroup TargetAgeGroup { get; set; }

    /// <summary>
    /// Subject area this question belongs to (math, reading, science, etc.).
    /// </summary>
    public string Subject { get; set; } = string.Empty;

    /// <summary>
    /// Difficulty level of the question.
    /// </summary>
    public string DifficultyLevel { get; set; } = string.Empty;

    /// <summary>
    /// Whether this question requires slower speech for younger children.
    /// </summary>
    public bool RequiresSlowSpeech { get; set; }

    /// <summary>
    /// Whether this question needs repeated narration support.
    /// </summary>
    public bool SupportsRepetition { get; set; } = true;

    /// <summary>
    /// Audio playback settings specific to this question.
    /// </summary>
    public QuestionAudioSettings AudioSettings { get; set; } = new();

    /// <summary>
    /// Metadata for organizing and searching question audio.
    /// </summary>
    public Dictionary<string, string> Metadata { get; set; } = new();

    /// <summary>
    /// Gets the question audio for the specified language.
    /// </summary>
    /// <param name="languageCode">Language code (e.g., "en", "es")</param>
    /// <returns>Audio file path or null if not available</returns>
    public string? GetQuestionAudio(string languageCode)
    {
        return QuestionAudio.TryGetValue(languageCode, out var audio) ? audio : null;
    }

    /// <summary>
    /// Gets answer choice audio for a specific choice and language.
    /// </summary>
    /// <param name="choiceIndex">Zero-based index of the answer choice</param>
    /// <param name="languageCode">Language code (e.g., "en", "es")</param>
    /// <returns>Audio file path or null if not available</returns>
    public string? GetAnswerChoiceAudio(int choiceIndex, string languageCode)
    {
        if (AnswerChoiceAudio.TryGetValue(choiceIndex, out var choiceAudio))
        {
            return choiceAudio.TryGetValue(languageCode, out var audio) ? audio : null;
        }
        return null;
    }

    /// <summary>
    /// Gets all answer choice audio for the specified language.
    /// </summary>
    /// <param name="languageCode">Language code (e.g., "en", "es")</param>
    /// <returns>List of audio file paths in choice order</returns>
    public List<string> GetAllAnswerChoiceAudio(string languageCode)
    {
        var audioList = new List<string>();
        for (int i = 0; i < AnswerChoiceAudio.Count; i++)
        {
            var audio = GetAnswerChoiceAudio(i, languageCode);
            if (!string.IsNullOrEmpty(audio))
            {
                audioList.Add(audio);
            }
        }
        return audioList;
    }

    /// <summary>
    /// Gets hint audio for a specific level and language.
    /// </summary>
    /// <param name="hintLevel">Hint level (1=subtle, 2=moderate, 3=direct)</param>
    /// <param name="languageCode">Language code (e.g., "en", "es")</param>
    /// <returns>Audio file path or null if not available</returns>
    public string? GetHintAudio(int hintLevel, string languageCode)
    {
        if (HintAudio.TryGetValue(hintLevel, out var levelAudio))
        {
            return levelAudio.TryGetValue(languageCode, out var audio) ? audio : null;
        }
        return null;
    }

    /// <summary>
    /// Gets visual description audio for the specified language.
    /// </summary>
    /// <param name="languageCode">Language code (e.g., "en", "es")</param>
    /// <returns>Visual description audio or null if not available</returns>
    public string? GetVisualDescriptionAudio(string languageCode)
    {
        return VisualDescriptionAudio.TryGetValue(languageCode, out var audio) ? audio : null;
    }

    /// <summary>
    /// Gets interaction instructions for the specified language.
    /// </summary>
    /// <param name="languageCode">Language code (e.g., "en", "es")</param>
    /// <returns>Interaction instructions audio or null if not available</returns>
    public string? GetInteractionInstructions(string languageCode)
    {
        return InteractionInstructions.TryGetValue(languageCode, out var instructions) ? instructions : null;
    }

    /// <summary>
    /// Checks if this question audio is appropriate for the specified age.
    /// </summary>
    /// <param name="childAge">Child's age in years</param>
    /// <returns>True if appropriate for the age, false otherwise</returns>
    public bool IsAppropriateForAge(int childAge)
    {
        return TargetAgeGroup switch
        {
            AgeGroup.PreK => childAge >= 3 && childAge <= 4,
            AgeGroup.Kindergarten => childAge == 5,
            AgeGroup.Primary => childAge >= 6 && childAge <= 8,
            _ => true
        };
    }

    /// <summary>
    /// Creates a copy of this question audio model with age-specific adjustments.
    /// </summary>
    /// <param name="childAge">Child's age for optimization</param>
    /// <returns>Optimized question audio model</returns>
    public QuestionAudioModel OptimizeForAge(int childAge)
    {
        var optimized = new QuestionAudioModel
        {
            QuestionId = QuestionId,
            QuestionText = QuestionText,
            QuestionType = QuestionType,
            QuestionAudio = new Dictionary<string, string>(QuestionAudio),
            AnswerChoiceAudio = new Dictionary<int, Dictionary<string, string>>(),
            HintAudio = new Dictionary<int, Dictionary<string, string>>(),
            VisualDescriptionAudio = new Dictionary<string, string>(VisualDescriptionAudio),
            InteractionInstructions = new Dictionary<string, string>(InteractionInstructions),
            TargetAgeGroup = TargetAgeGroup,
            Subject = Subject,
            DifficultyLevel = DifficultyLevel,
            SupportsRepetition = SupportsRepetition,
            Metadata = new Dictionary<string, string>(Metadata)
        };

        // Copy nested dictionaries
        foreach (var kvp in AnswerChoiceAudio)
        {
            optimized.AnswerChoiceAudio[kvp.Key] = new Dictionary<string, string>(kvp.Value);
        }

        foreach (var kvp in HintAudio)
        {
            optimized.HintAudio[kvp.Key] = new Dictionary<string, string>(kvp.Value);
        }

        // Adjust settings based on age
        optimized.RequiresSlowSpeech = childAge <= 4;
        optimized.AudioSettings = AudioSettings.OptimizeForAge(childAge);

        return optimized;
    }
}

/// <summary>
/// Audio playback settings specific to question narration.
/// </summary>
public class QuestionAudioSettings
{
    /// <summary>
    /// Speech rate for question narration (0.5 = half speed, 1.0 = normal, 2.0 = double speed).
    /// </summary>
    public float SpeechRate { get; set; } = 1.0f;

    /// <summary>
    /// Volume level for question audio (0.0 to 1.0).
    /// </summary>
    public float Volume { get; set; } = 0.8f;

    /// <summary>
    /// Pause duration between question and answer choices in milliseconds.
    /// </summary>
    public int QuestionToChoicePause { get; set; } = 1000;

    /// <summary>
    /// Pause duration between answer choices in milliseconds.
    /// </summary>
    public int BetweenChoicesPause { get; set; } = 800;

    /// <summary>
    /// Whether to automatically repeat the question after answer choices.
    /// </summary>
    public bool AutoRepeatQuestion { get; set; } = false;

    /// <summary>
    /// Number of times to automatically repeat for younger children.
    /// </summary>
    public int AutoRepeatCount { get; set; } = 1;

    /// <summary>
    /// Delay before starting question audio in milliseconds.
    /// </summary>
    public int InitialDelay { get; set; } = 500;

    /// <summary>
    /// Whether to use gentle fade-in for question audio.
    /// </summary>
    public bool UseFadeIn { get; set; } = true;

    /// <summary>
    /// Duration of fade-in effect in milliseconds.
    /// </summary>
    public int FadeInDuration { get; set; } = 300;

    /// <summary>
    /// Optimizes audio settings for a specific child age.
    /// </summary>
    /// <param name="childAge">Child's age in years</param>
    /// <returns>Optimized audio settings</returns>
    public QuestionAudioSettings OptimizeForAge(int childAge)
    {
        return new QuestionAudioSettings
        {
            SpeechRate = childAge <= 4 ? 0.8f : (childAge == 5 ? 0.9f : 1.0f),
            Volume = Volume,
            QuestionToChoicePause = childAge <= 4 ? 1500 : (childAge == 5 ? 1200 : 1000),
            BetweenChoicesPause = childAge <= 4 ? 1200 : (childAge == 5 ? 1000 : 800),
            AutoRepeatQuestion = childAge <= 4,
            AutoRepeatCount = childAge <= 4 ? 2 : 1,
            InitialDelay = childAge <= 4 ? 800 : 500,
            UseFadeIn = UseFadeIn,
            FadeInDuration = FadeInDuration
        };
    }
}