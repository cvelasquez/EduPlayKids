using EduPlayKids.Domain.Entities;
using EduPlayKids.Presentation.Models;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace EduPlayKids.Presentation.Services;

/// <summary>
/// Factory service for creating UI question models from domain entities.
/// Handles conversion and configuration of interactive question components.
/// </summary>
public class QuestionModelFactory
{
    private readonly ILogger<QuestionModelFactory> _logger;

    public QuestionModelFactory(ILogger<QuestionModelFactory> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Creates a question model from an ActivityQuestion entity asynchronously.
    /// Handles different question types and configures UI-specific properties.
    /// </summary>
    /// <param name="question">The database question entity</param>
    /// <param name="childAge">Child's age for age-appropriate adaptations</param>
    /// <param name="language">Language preference ("en" or "es")</param>
    /// <param name="supportFeatures">Support features to enable for this child</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Configured question model ready for UI rendering</returns>
    public async Task<QuestionModelBase?> CreateQuestionModelAsync(ActivityQuestion question, int childAge, string language, Dictionary<string, bool> supportFeatures, CancellationToken cancellationToken = default)
    {
        await Task.Delay(1, cancellationToken); // Make it properly async
        return CreateQuestionModel(question, childAge, language, supportFeatures);
    }

    /// <summary>
    /// Creates a question model from an ActivityQuestion entity.
    /// Handles different question types and configures UI-specific properties.
    /// </summary>
    /// <param name="question">The database question entity</param>
    /// <param name="childAge">Child's age for age-appropriate adaptations</param>
    /// <param name="language">Language preference ("en" or "es")</param>
    /// <param name="supportFeatures">Support features to enable for this child</param>
    /// <returns>Configured question model ready for UI rendering</returns>
    public QuestionModelBase? CreateQuestionModel(ActivityQuestion question, int childAge, string language, Dictionary<string, bool> supportFeatures)
    {
        try
        {
            _logger.LogInformation("Creating question model for question {QuestionId}, type {QuestionType}",
                question.Id, question.QuestionType);

            QuestionModelBase baseModel = question.QuestionType.ToLower() switch
            {
                "multiplechoice" => CreateMultipleChoiceModel(question, childAge, language),
                "draganddrop" or "dragdrop" => CreateDragDropModel(question, childAge, language),
                "matching" => CreateMatchingModel(question, childAge, language),
                "tracing" => CreateTracingModel(question, childAge, language),
                _ => throw new NotSupportedException($"Question type '{question.QuestionType}' is not supported")
            };

            if (baseModel != null)
            {
                ConfigureBaseProperties(baseModel, question, childAge, language, supportFeatures);
            }

            _logger.LogInformation("Successfully created {QuestionType} model for question {QuestionId}",
                question.QuestionType, question.Id);

            return baseModel;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating question model for question {QuestionId}", question.Id);
            return null;
        }
    }

    #region Question Type Creators

    private MultipleChoiceQuestionModel CreateMultipleChoiceModel(ActivityQuestion question, int childAge, string language)
    {
        var model = new MultipleChoiceQuestionModel();

        if (string.IsNullOrEmpty(question.ConfigurationData))
        {
            _logger.LogWarning("No configuration data found for multiple choice question {QuestionId}", question.Id);
            return model;
        }

        try
        {
            var config = JsonSerializer.Deserialize<MultipleChoiceConfig>(question.ConfigurationData);
            if (config != null)
            {
                model.AllowMultipleSelection = config.AllowMultipleSelection;
                model.Options = config.Options.Select((option, index) => new MultipleChoiceOption
                {
                    Index = index,
                    Text = language == "es" ? option.TextEs ?? option.TextEn : option.TextEn,
                    ImagePath = option.ImagePath,
                    AudioPath = language == "es" ? option.AudioPathEs : option.AudioPathEn
                }).ToList();

                model.CorrectAnswerIndexes = config.CorrectAnswerIndexes;

                // Age-appropriate adaptations
                if (childAge <= 4)
                {
                    // Limit options for younger children
                    model.Options = model.Options.Take(Math.Min(3, model.Options.Count)).ToList();
                    // Adjust correct answer indexes if needed
                    model.CorrectAnswerIndexes = model.CorrectAnswerIndexes.Where(i => i < model.Options.Count).ToList();
                }
            }
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error parsing configuration data for multiple choice question {QuestionId}", question.Id);
        }

        return model;
    }

    private DragDropQuestionModel CreateDragDropModel(ActivityQuestion question, int childAge, string language)
    {
        var model = new DragDropQuestionModel();

        if (string.IsNullOrEmpty(question.ConfigurationData))
        {
            _logger.LogWarning("No configuration data found for drag drop question {QuestionId}", question.Id);
            return model;
        }

        try
        {
            var config = JsonSerializer.Deserialize<DragDropConfig>(question.ConfigurationData);
            if (config != null)
            {
                model.DraggableItems = config.DraggableItems.Select(item => new DraggableItem
                {
                    Id = item.Id,
                    Text = language == "es" ? item.TextEs ?? item.TextEn : item.TextEn,
                    ImagePath = item.ImagePath,
                    X = item.InitialX,
                    Y = item.InitialY,
                    Width = childAge <= 4 ? 100 : 80, // Larger for younger children
                    Height = childAge <= 4 ? 100 : 80
                }).ToList();

                model.DropZones = config.DropZones.Select(zone => new DropZone
                {
                    Id = zone.Id,
                    Label = language == "es" ? zone.LabelEs ?? zone.LabelEn : zone.LabelEn,
                    X = zone.X,
                    Y = zone.Y,
                    Width = childAge <= 4 ? 140 : 120, // Larger for younger children
                    Height = childAge <= 4 ? 140 : 120,
                    BackgroundImagePath = zone.BackgroundImagePath
                }).ToList();

                model.CorrectMapping = config.CorrectMapping;
            }
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error parsing configuration data for drag drop question {QuestionId}", question.Id);
        }

        return model;
    }

    private MatchingQuestionModel CreateMatchingModel(ActivityQuestion question, int childAge, string language)
    {
        var model = new MatchingQuestionModel();

        if (string.IsNullOrEmpty(question.ConfigurationData))
        {
            _logger.LogWarning("No configuration data found for matching question {QuestionId}", question.Id);
            return model;
        }

        try
        {
            var config = JsonSerializer.Deserialize<MatchingConfig>(question.ConfigurationData);
            if (config != null)
            {
                model.LeftItems = config.LeftItems.Select(item => new MatchingItem
                {
                    Id = item.Id,
                    Text = language == "es" ? item.TextEs ?? item.TextEn : item.TextEn,
                    ImagePath = item.ImagePath,
                    AudioPath = language == "es" ? item.AudioPathEs : item.AudioPathEn
                }).ToList();

                model.RightItems = config.RightItems.Select(item => new MatchingItem
                {
                    Id = item.Id,
                    Text = language == "es" ? item.TextEs ?? item.TextEn : item.TextEn,
                    ImagePath = item.ImagePath,
                    AudioPath = language == "es" ? item.AudioPathEs : item.AudioPathEn
                }).ToList();

                model.CorrectMatches = config.CorrectMatches;

                // Age-appropriate adaptations
                if (childAge <= 4)
                {
                    // Limit pairs for younger children
                    var maxPairs = Math.Min(3, model.LeftItems.Count);
                    model.LeftItems = model.LeftItems.Take(maxPairs).ToList();
                    model.RightItems = model.RightItems.Take(maxPairs).ToList();

                    // Update correct matches accordingly
                    var validLeftIds = model.LeftItems.Select(i => i.Id).ToHashSet();
                    var validRightIds = model.RightItems.Select(i => i.Id).ToHashSet();
                    model.CorrectMatches = model.CorrectMatches
                        .Where(kvp => validLeftIds.Contains(kvp.Key) && validRightIds.Contains(kvp.Value))
                        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                }
            }
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error parsing configuration data for matching question {QuestionId}", question.Id);
        }

        return model;
    }

    private TracingQuestionModel CreateTracingModel(ActivityQuestion question, int childAge, string language)
    {
        var model = new TracingQuestionModel();

        if (string.IsNullOrEmpty(question.ConfigurationData))
        {
            _logger.LogWarning("No configuration data found for tracing question {QuestionId}", question.Id);
            return model;
        }

        try
        {
            var config = JsonSerializer.Deserialize<TracingConfig>(question.ConfigurationData);
            if (config != null)
            {
                model.TracingType = config.TracingType;
                model.AccuracyThreshold = childAge <= 4 ? 0.6 : 0.75; // Lower threshold for younger children
                model.ShowGuideLines = childAge <= 5 || config.ShowGuideLines;
                model.ShowStartPoint = true;

                model.TracingPaths = config.Paths.Select(path => new TracingPath
                {
                    Id = path.Id,
                    Points = path.Points.Select(p => new TracingPoint(p.X, p.Y, DateTime.Now)).ToList(),
                    Color = path.Color ?? "#FF4444",
                    StrokeWidth = childAge <= 4 ? 6.0 : 4.0, // Thicker lines for younger children
                    ShowDirection = path.ShowDirection
                }).ToList();
            }
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error parsing configuration data for tracing question {QuestionId}", question.Id);
        }

        return model;
    }

    #endregion

    #region Configuration and Support

    private void ConfigureBaseProperties(QuestionModelBase model, ActivityQuestion question, int childAge, string language, Dictionary<string, bool> supportFeatures)
    {
        model.QuestionId = question.Id;
        model.QuestionType = question.QuestionType;
        model.QuestionText = question.GetLocalizedQuestionText(language) ?? "";
        model.HintText = question.GetLocalizedHint(language);
        model.ExplanationText = question.GetLocalizedExplanation(language);
        model.ImagePath = question.ImagePath;
        model.AudioPath = question.GetLocalizedQuestionAudioPath(language);
        model.Points = question.Points;
        model.MaxAttempts = question.MaxAttempts;
        model.Language = language;

        // Configure hints based on question settings and support features
        model.HintsEnabled = question.HintsEnabled &&
                           supportFeatures.GetValueOrDefault("VisualHints", true);

        // Age-specific adaptations
        if (childAge <= 4)
        {
            // Younger children get more attempts and easier hint thresholds
            if (model.MaxAttempts == 0) model.MaxAttempts = 5;
        }
        else if (childAge >= 7)
        {
            // Older children can handle more challenging settings
            if (model.MaxAttempts == 0) model.MaxAttempts = 3;
        }
    }

    #endregion
}

#region Configuration Classes

/// <summary>
/// Configuration data structure for multiple choice questions.
/// </summary>
public class MultipleChoiceConfig
{
    public bool AllowMultipleSelection { get; set; } = false;
    public List<MultipleChoiceOptionConfig> Options { get; set; } = new();
    public List<int> CorrectAnswerIndexes { get; set; } = new();
}

/// <summary>
/// Configuration for a multiple choice option.
/// </summary>
public class MultipleChoiceOptionConfig
{
    public string TextEn { get; set; } = string.Empty;
    public string? TextEs { get; set; }
    public string? ImagePath { get; set; }
    public string? AudioPathEn { get; set; }
    public string? AudioPathEs { get; set; }
}

/// <summary>
/// Configuration data structure for drag and drop questions.
/// </summary>
public class DragDropConfig
{
    public List<DraggableItemConfig> DraggableItems { get; set; } = new();
    public List<DropZoneConfig> DropZones { get; set; } = new();
    public Dictionary<int, int> CorrectMapping { get; set; } = new(); // ItemId -> ZoneId
}

/// <summary>
/// Configuration for a draggable item.
/// </summary>
public class DraggableItemConfig
{
    public int Id { get; set; }
    public string TextEn { get; set; } = string.Empty;
    public string? TextEs { get; set; }
    public string? ImagePath { get; set; }
    public double InitialX { get; set; }
    public double InitialY { get; set; }
}

/// <summary>
/// Configuration for a drop zone.
/// </summary>
public class DropZoneConfig
{
    public int Id { get; set; }
    public string LabelEn { get; set; } = string.Empty;
    public string? LabelEs { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public string? BackgroundImagePath { get; set; }
}

/// <summary>
/// Configuration data structure for matching questions.
/// </summary>
public class MatchingConfig
{
    public List<MatchingItemConfig> LeftItems { get; set; } = new();
    public List<MatchingItemConfig> RightItems { get; set; } = new();
    public Dictionary<int, int> CorrectMatches { get; set; } = new(); // LeftItemId -> RightItemId
}

/// <summary>
/// Configuration for a matching item.
/// </summary>
public class MatchingItemConfig
{
    public int Id { get; set; }
    public string TextEn { get; set; } = string.Empty;
    public string? TextEs { get; set; }
    public string? ImagePath { get; set; }
    public string? AudioPathEn { get; set; }
    public string? AudioPathEs { get; set; }
}

/// <summary>
/// Configuration data structure for tracing questions.
/// </summary>
public class TracingConfig
{
    public string TracingType { get; set; } = "Letter"; // Letter, Number, Shape
    public List<TracingPathConfig> Paths { get; set; } = new();
    public bool ShowGuideLines { get; set; } = true;
}

/// <summary>
/// Configuration for a tracing path.
/// </summary>
public class TracingPathConfig
{
    public int Id { get; set; }
    public List<TracingPointConfig> Points { get; set; } = new();
    public string? Color { get; set; }
    public bool ShowDirection { get; set; } = true;
}

/// <summary>
/// Configuration for a tracing point.
/// </summary>
public class TracingPointConfig
{
    public double X { get; set; }
    public double Y { get; set; }
}

#endregion