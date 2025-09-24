using EduPlayKids.Presentation.ViewModels.Questions;

namespace EduPlayKids.Presentation.Views.Questions;

/// <summary>
/// Base view for all question types with common functionality.
/// Provides shared UI elements and interaction patterns.
/// </summary>
public partial class BaseQuestionView : ContentView
{
    public BaseQuestionView()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Sets the interactive content for the specific question type.
    /// Called by derived question views to embed their specific UI.
    /// </summary>
    /// <param name="content">The interactive content view</param>
    public void SetInteractiveContent(View content)
    {
        InteractiveContentArea.Content = content;
    }

    /// <summary>
    /// Gets or sets the question view model.
    /// </summary>
    public BaseQuestionViewModel? QuestionViewModel
    {
        get => BindingContext as BaseQuestionViewModel;
        set => BindingContext = value;
    }
}