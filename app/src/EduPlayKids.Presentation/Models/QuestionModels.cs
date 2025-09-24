using System.ComponentModel;
using System.Text.Json;

namespace EduPlayKids.Presentation.Models;

/// <summary>
/// Base model for all question types with common properties and functionality.
/// Provides foundation for interactive question rendering in the UI.
/// </summary>
public abstract class QuestionModelBase : INotifyPropertyChanged
{
    private bool _isAnswered;
    private bool _isCorrect;
    private bool _showExplanation;
    private bool _hintsEnabled = true;
    private bool _showHints;
    private int _attemptCount;

    public int QuestionId { get; set; }
    public string QuestionType { get; set; } = string.Empty;
    public string QuestionText { get; set; } = string.Empty;
    public string? HintText { get; set; }
    public string? ExplanationText { get; set; }
    public string? ImagePath { get; set; }
    public string? AudioPath { get; set; }
    public int Points { get; set; } = 10;
    public int MaxAttempts { get; set; } = 0; // 0 = unlimited
    public string Language { get; set; } = "en";
    public string DifficultyLevel { get; set; } = "Easy";
    public string? Metadata { get; set; }

    public bool IsAnswered
    {
        get => _isAnswered;
        set
        {
            _isAnswered = value;
            OnPropertyChanged();
        }
    }

    public bool IsCorrect
    {
        get => _isCorrect;
        set
        {
            _isCorrect = value;
            OnPropertyChanged();
        }
    }

    public bool ShowExplanation
    {
        get => _showExplanation;
        set
        {
            _showExplanation = value;
            OnPropertyChanged();
        }
    }

    public bool HintsEnabled
    {
        get => _hintsEnabled;
        set
        {
            _hintsEnabled = value;
            OnPropertyChanged();
        }
    }

    public bool ShowHints
    {
        get => _showHints;
        set
        {
            _showHints = value;
            OnPropertyChanged();
        }
    }

    public int AttemptCount
    {
        get => _attemptCount;
        set
        {
            _attemptCount = value;
            OnPropertyChanged();
            // Auto-enable hints based on attempt count and difficulty
            if (HintsEnabled && !ShowHints && ShouldShowHints())
            {
                ShowHints = true;
            }
        }
    }

    public bool HasMaxAttemptsReached => MaxAttempts > 0 && AttemptCount >= MaxAttempts;

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Determines if hints should be shown based on attempt count and question difficulty.
    /// </summary>
    public virtual bool ShouldShowHints()
    {
        return AttemptCount >= 2; // Show hints after 2 failed attempts by default
    }

    /// <summary>
    /// Validates the user's answer against the correct answer.
    /// </summary>
    public abstract bool ValidateAnswer(object userAnswer);

    /// <summary>
    /// Resets the question to its initial state for retrying.
    /// </summary>
    public virtual void Reset()
    {
        IsAnswered = false;
        IsCorrect = false;
        ShowExplanation = false;
        ShowHints = false;
        AttemptCount = 0;
    }
}

/// <summary>
/// Model for multiple choice questions with selectable options.
/// Supports single and multiple correct answers.
/// </summary>
public class MultipleChoiceQuestionModel : QuestionModelBase
{
    private int _selectedOptionIndex = -1;
    private List<int> _selectedOptionIndexes = new();

    public List<MultipleChoiceOption> Options { get; set; } = new();
    public List<MultipleChoiceOption> AnswerChoices { get; set; } = new();
    public bool AllowMultipleSelection { get; set; } = false;
    public List<int> CorrectAnswerIndexes { get; set; } = new();

    public int SelectedOptionIndex
    {
        get => _selectedOptionIndex;
        set
        {
            _selectedOptionIndex = value;
            OnPropertyChanged();

            if (!AllowMultipleSelection && value >= 0)
            {
                // Update selection state for single selection
                for (int i = 0; i < Options.Count; i++)
                {
                    Options[i].IsSelected = i == value;
                }
            }
        }
    }

    public List<int> SelectedOptionIndexes
    {
        get => _selectedOptionIndexes;
        set
        {
            _selectedOptionIndexes = value;
            OnPropertyChanged();

            if (AllowMultipleSelection)
            {
                // Update selection state for multiple selection
                for (int i = 0; i < Options.Count; i++)
                {
                    Options[i].IsSelected = value.Contains(i);
                }
            }
        }
    }

    public MultipleChoiceQuestionModel()
    {
        // Synchronize Options and AnswerChoices for backward compatibility
        AnswerChoices = Options;
    }

    public override bool ValidateAnswer(object userAnswer)
    {
        if (AllowMultipleSelection)
        {
            if (userAnswer is List<int> selectedIndexes)
            {
                return selectedIndexes.Count == CorrectAnswerIndexes.Count &&
                       selectedIndexes.All(CorrectAnswerIndexes.Contains);
            }
        }
        else
        {
            if (userAnswer is int selectedIndex)
            {
                return CorrectAnswerIndexes.Contains(selectedIndex);
            }
        }
        return false;
    }

    public void ToggleOption(int index)
    {
        if (index < 0 || index >= Options.Count) return;

        if (AllowMultipleSelection)
        {
            if (SelectedOptionIndexes.Contains(index))
            {
                var newList = SelectedOptionIndexes.ToList();
                newList.Remove(index);
                SelectedOptionIndexes = newList;
            }
            else
            {
                var newList = SelectedOptionIndexes.ToList();
                newList.Add(index);
                SelectedOptionIndexes = newList;
            }
        }
        else
        {
            SelectedOptionIndex = index;
        }
    }
}

/// <summary>
/// Model for drag and drop questions with draggable items and drop zones.
/// Supports spatial learning and categorization activities.
/// </summary>
public class DragDropQuestionModel : QuestionModelBase
{
    public List<DraggableItem> DraggableItems { get; set; } = new();
    public List<DropZone> DropZones { get; set; } = new();
    public Dictionary<int, int> ItemZoneMapping { get; set; } = new(); // ItemId -> ZoneId
    public Dictionary<int, int> CorrectMapping { get; set; } = new(); // ItemId -> ZoneId

    public override bool ValidateAnswer(object userAnswer)
    {
        if (userAnswer is Dictionary<int, int> mapping)
        {
            return mapping.Count == CorrectMapping.Count &&
                   mapping.All(kvp => CorrectMapping.ContainsKey(kvp.Key) && CorrectMapping[kvp.Key] == kvp.Value);
        }
        return false;
    }

    public void MoveItemToZone(int itemId, int zoneId)
    {
        ItemZoneMapping[itemId] = zoneId;
        OnPropertyChanged(nameof(ItemZoneMapping));
    }

    public void RemoveItemFromZone(int itemId)
    {
        ItemZoneMapping.Remove(itemId);
        OnPropertyChanged(nameof(ItemZoneMapping));
    }
}

/// <summary>
/// Model for matching questions where items need to be paired correctly.
/// Supports memory games and association learning.
/// </summary>
public class MatchingQuestionModel : QuestionModelBase
{
    public List<MatchingItem> LeftItems { get; set; } = new();
    public List<MatchingItem> RightItems { get; set; } = new();
    public Dictionary<int, int> UserMatches { get; set; } = new(); // LeftItemId -> RightItemId
    public Dictionary<int, int> CorrectMatches { get; set; } = new(); // LeftItemId -> RightItemId

    public override bool ValidateAnswer(object userAnswer)
    {
        if (userAnswer is Dictionary<int, int> matches)
        {
            return matches.Count == CorrectMatches.Count &&
                   matches.All(kvp => CorrectMatches.ContainsKey(kvp.Key) && CorrectMatches[kvp.Key] == kvp.Value);
        }
        return false;
    }

    public void CreateMatch(int leftItemId, int rightItemId)
    {
        UserMatches[leftItemId] = rightItemId;
        OnPropertyChanged(nameof(UserMatches));
    }

    public void RemoveMatch(int leftItemId)
    {
        UserMatches.Remove(leftItemId);
        OnPropertyChanged(nameof(UserMatches));
    }
}

/// <summary>
/// Model for tracing questions where children trace letters, numbers, or shapes.
/// Supports fine motor skill development and handwriting practice.
/// </summary>
public class TracingQuestionModel : QuestionModelBase
{
    private List<TracingPoint> _userTrace = new();

    public List<TracingPath> TracingPaths { get; set; } = new();
    public string TracingType { get; set; } = "Letter"; // Letter, Number, Shape
    public double AccuracyThreshold { get; set; } = 0.75; // 75% accuracy required
    public bool ShowGuideLines { get; set; } = true;
    public bool ShowStartPoint { get; set; } = true;

    public List<TracingPoint> UserTrace
    {
        get => _userTrace;
        set
        {
            _userTrace = value;
            OnPropertyChanged();
        }
    }

    public override bool ValidateAnswer(object userAnswer)
    {
        if (userAnswer is List<TracingPoint> trace)
        {
            return CalculateTracingAccuracy(trace) >= AccuracyThreshold;
        }
        return false;
    }

    public void AddTracePoint(double x, double y, DateTime timestamp)
    {
        UserTrace.Add(new TracingPoint(x, y, timestamp));
        OnPropertyChanged(nameof(UserTrace));
    }

    public void ClearTrace()
    {
        UserTrace.Clear();
    }

    private double CalculateTracingAccuracy(List<TracingPoint> trace)
    {
        if (!trace.Any() || !TracingPaths.Any()) return 0;

        // Simplified accuracy calculation - in production, use more sophisticated algorithm
        var totalPoints = trace.Count;
        var correctPoints = 0;

        foreach (var point in trace)
        {
            foreach (var path in TracingPaths)
            {
                if (IsPointOnPath(point, path))
                {
                    correctPoints++;
                    break;
                }
            }
        }

        return totalPoints > 0 ? (double)correctPoints / totalPoints : 0;
    }

    private bool IsPointOnPath(TracingPoint point, TracingPath path)
    {
        // Simplified path checking - in production, use proper geometric algorithms
        const double tolerance = 20.0; // pixels

        foreach (var pathPoint in path.Points)
        {
            var distance = Math.Sqrt(Math.Pow(point.X - pathPoint.X, 2) + Math.Pow(point.Y - pathPoint.Y, 2));
            if (distance <= tolerance)
            {
                return true;
            }
        }
        return false;
    }
}

#region Supporting Classes

/// <summary>
/// Represents an option in a multiple choice question.
/// </summary>
public class MultipleChoiceOption : INotifyPropertyChanged
{
    private bool _isSelected;

    public int Index { get; set; }
    public string Text { get; set; } = string.Empty;
    public string? ImagePath { get; set; }
    public string? AudioPath { get; set; }

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            _isSelected = value;
            OnPropertyChanged();
        }
    }

    public bool IsCorrectAnswer { get; set; }
    public bool WasSelected { get; set; }

    /// <summary>
    /// Gets a value indicating whether this option has audio content.
    /// </summary>
    public bool HasAudio => !string.IsNullOrEmpty(AudioPath);

    /// <summary>
    /// Gets a value indicating whether this option has image content.
    /// </summary>
    public bool HasImage => !string.IsNullOrEmpty(ImagePath);

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

/// <summary>
/// Represents a draggable item in a drag and drop question.
/// </summary>
public class DraggableItem
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public string? ImagePath { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public double Width { get; set; } = 80;
    public double Height { get; set; } = 80;
    public bool IsDragging { get; set; }
}

/// <summary>
/// Represents a drop zone in a drag and drop question.
/// </summary>
public class DropZone
{
    public int Id { get; set; }
    public string Label { get; set; } = string.Empty;
    public double X { get; set; }
    public double Y { get; set; }
    public double Width { get; set; } = 120;
    public double Height { get; set; } = 120;
    public bool IsHighlighted { get; set; }
    public string? BackgroundImagePath { get; set; }
}

/// <summary>
/// Represents an item in a matching question.
/// </summary>
public class MatchingItem
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public string? ImagePath { get; set; }
    public string? AudioPath { get; set; }
    public bool IsMatched { get; set; }
}

/// <summary>
/// Represents a point in a tracing path.
/// </summary>
public class TracingPoint
{
    public double X { get; set; }
    public double Y { get; set; }
    public DateTime Timestamp { get; set; }

    public TracingPoint(double x, double y, DateTime timestamp)
    {
        X = x;
        Y = y;
        Timestamp = timestamp;
    }
}

/// <summary>
/// Represents a path to be traced.
/// </summary>
public class TracingPath
{
    public int Id { get; set; }
    public List<TracingPoint> Points { get; set; } = new();
    public string Color { get; set; } = "#FF4444";
    public double StrokeWidth { get; set; } = 4.0;
    public bool ShowDirection { get; set; } = true;
}

#endregion