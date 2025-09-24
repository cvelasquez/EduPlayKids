using EduPlayKids.Presentation.ViewModels.Questions;

namespace EduPlayKids.Presentation.Views.Questions;

/// <summary>
/// Interactive view for multiple choice questions.
/// Supports single and multiple selection modes with child-friendly design.
/// </summary>
public partial class MultipleChoiceQuestionView : ContentView
{
    public MultipleChoiceQuestionView()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Gets or sets the multiple choice question view model.
    /// </summary>
    public MultipleChoiceQuestionViewModel? ViewModel
    {
        get => BindingContext as MultipleChoiceQuestionViewModel;
        set => BindingContext = value;
    }
}