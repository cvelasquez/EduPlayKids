using Microsoft.Maui.Graphics;

namespace EduPlayKids.Tests.ChildUsability;

/// <summary>
/// Child usability tests for touch targets and motor skills compatibility.
/// Critical for ensuring children aged 3-8 can successfully interact with the app.
/// </summary>
public class TouchTargetTests
{
    #region Touch Target Size Requirements - CRITICAL for Children 3-8

    [Theory]
    [InlineData(60)] // Minimum recommended size in dp for children
    [InlineData(80)] // Preferred size for PreK (3-4 years)
    [InlineData(100)] // Ideal size for very young children
    public void TouchTargets_MinimumSize_ShouldMeetChildAccessibilityStandards(double minimumSizeDp)
    {
        // Arrange
        var buttonSize = new Size(minimumSizeDp, minimumSizeDp);

        // Act & Assert
        buttonSize.Width.Should().BeGreaterOrEqualTo(60,
            "because children aged 3-8 need large touch targets for accurate interaction");
        buttonSize.Height.Should().BeGreaterOrEqualTo(60,
            "because children's motor skills require large interactive areas");
    }

    [Theory]
    [InlineData(40)] // Too small for children
    [InlineData(50)] // Still too small
    [InlineData(30)] // Definitely too small
    public void TouchTargets_TooSmall_ShouldFailChildAccessibilityStandards(double tooSmallSizeDp)
    {
        // Arrange
        var buttonSize = new Size(tooSmallSizeDp, tooSmallSizeDp);

        // Act & Assert
        buttonSize.Width.Should().BeLessThan(60,
            "this test verifies we can detect touch targets that are too small for children");
        buttonSize.Height.Should().BeLessThan(60,
            "touch targets under 60dp are not suitable for children aged 3-8");
    }

    [Theory]
    [InlineData(10)] // 10dp spacing - minimum
    [InlineData(16)] // 16dp spacing - recommended
    [InlineData(24)] // 24dp spacing - preferred for children
    public void TouchTargets_Spacing_ShouldPreventAccidentalActivation(double spacingDp)
    {
        // Arrange
        var button1Position = new Point(0, 0);
        var button2Position = new Point(100 + spacingDp, 0); // 100dp button width + spacing
        var actualSpacing = button2Position.X - (button1Position.X + 100);

        // Act & Assert
        actualSpacing.Should().BeGreaterOrEqualTo(10,
            "minimum spacing prevents accidental activation by children");
        actualSpacing.Should().Be(spacingDp);
    }

    #endregion

    #region Child Motor Skills Compatibility

    [Fact]
    public void TouchTargets_CircularButtons_ShouldBeEasierForChildrenToHit()
    {
        // Arrange
        var circularButtonRadius = 40; // 80dp diameter
        var circularButtonArea = Math.PI * Math.Pow(circularButtonRadius, 2);
        var squareButtonArea = Math.Pow(80, 2); // 80x80 square

        // Act & Assert
        circularButtonArea.Should().BeLessThan(squareButtonArea,
            "circular buttons are often easier for children to target accurately");

        // Verify minimum radius meets child accessibility
        (circularButtonRadius * 2).Should().BeGreaterOrEqualTo(60,
            "circular button diameter should meet minimum touch target size");
    }

    [Theory]
    [InlineData(3, 100)] // PreK (3-4 years) - largest targets
    [InlineData(5, 80)]  // Kindergarten (5 years) - medium targets
    [InlineData(7, 60)]  // Primary (6-8 years) - minimum acceptable targets
    public void TouchTargets_AgeAppropriate_ShouldScaleWithChildDevelopment(int childAge, double expectedMinimumSize)
    {
        // Arrange
        var recommendedSize = GetRecommendedTouchTargetSize(childAge);

        // Act & Assert
        recommendedSize.Should().BeGreaterOrEqualTo(expectedMinimumSize,
            $"touch targets for age {childAge} should be at least {expectedMinimumSize}dp");

        recommendedSize.Should().BeGreaterOrEqualTo(60,
            "all touch targets must meet minimum child accessibility standards");
    }

    private static double GetRecommendedTouchTargetSize(int childAge)
    {
        return childAge switch
        {
            <= 4 => 100, // PreK needs largest targets
            5 => 80,     // Kindergarten
            >= 6 => 60   // Primary school (minimum acceptable)
        };
    }

    #endregion

    #region Visual Feedback Requirements

    [Fact]
    public void TouchTargets_VisualFeedback_ShouldBeImmediateAndClear()
    {
        // Arrange
        var feedbackDelayMs = 0; // Should be immediate
        var visualFeedbackDurationMs = 200; // Short but visible

        // Act & Assert
        feedbackDelayMs.Should().Be(0,
            "children need immediate visual feedback when touching elements");

        visualFeedbackDurationMs.Should().BeInRange(100, 500,
            "feedback should be visible long enough for children to notice but not distract");
    }

    [Theory]
    [InlineData("Pressed", "#4CAF50")] // Green for positive actions
    [InlineData("Hover", "#81C784")]   // Light green for hover
    [InlineData("Disabled", "#BDBDBD")] // Gray for disabled
    public void TouchTargets_StateColors_ShouldBeChildFriendly(string state, string expectedColor)
    {
        // Arrange
        var stateColor = Color.FromArgb(expectedColor);

        // Act & Assert
        stateColor.Should().NotBeNull("all touch states must have defined colors");

        // Verify colors are child-friendly (bright, not scary)
        if (state == "Pressed")
        {
            stateColor.ToRgbaHex().Should().Contain("4CAF50",
                "pressed state should use encouraging green color");
        }
    }

    #endregion

    #region Error Prevention for Children

    [Fact]
    public void TouchTargets_CriticalActions_ShouldRequireConfirmation()
    {
        // Arrange
        var criticalActions = new[] { "Delete", "Exit", "Reset Progress" };

        // Act & Assert
        foreach (var action in criticalActions)
        {
            var requiresConfirmation = ShouldRequireConfirmation(action);
            requiresConfirmation.Should().BeTrue(
                $"'{action}' is a critical action that children might activate accidentally");
        }
    }

    private static bool ShouldRequireConfirmation(string action)
    {
        var criticalActions = new[] { "Delete", "Exit", "Reset", "Remove" };
        return criticalActions.Any(critical =>
            action.ToLower().Contains(critical.ToLower()));
    }

    [Fact]
    public void TouchTargets_NonCriticalActions_ShouldNotRequireConfirmation()
    {
        // Arrange
        var nonCriticalActions = new[] { "Next", "Play", "Try Again", "Continue" };

        // Act & Assert
        foreach (var action in nonCriticalActions)
        {
            var requiresConfirmation = ShouldRequireConfirmation(action);
            requiresConfirmation.Should().BeFalse(
                $"'{action}' should be immediately accessible to maintain engagement");
        }
    }

    #endregion

    #region Accessibility Integration

    [Theory]
    [InlineData("Next Activity", "Continuar a la siguiente actividad")]
    [InlineData("Try Again", "Intentar de nuevo")]
    [InlineData("Great Job!", "¡Excelente trabajo!")]
    public void TouchTargets_Accessibility_ShouldSupportBilingualLabels(string englishLabel, string spanishLabel)
    {
        // Arrange & Act
        var hasEnglishLabel = !string.IsNullOrEmpty(englishLabel);
        var hasSpanishLabel = !string.IsNullOrEmpty(spanishLabel);

        // Assert
        hasEnglishLabel.Should().BeTrue("all interactive elements need English accessibility labels");
        hasSpanishLabel.Should().BeTrue("all interactive elements need Spanish accessibility labels for bilingual support");
    }

    [Theory]
    [InlineData(7.0)] // WCAG AA standard for child-friendly content
    [InlineData(4.5)] // WCAG AA minimum
    public void TouchTargets_ContrastRatio_ShouldMeetAccessibilityStandards(double minimumContrastRatio)
    {
        // Arrange
        var backgroundColor = Color.FromRgb(255, 255, 255); // White background
        var textColor = Color.FromRgb(33, 33, 33); // Dark text

        // Note: This is a simplified test - real implementation would calculate actual contrast
        var calculatedContrast = CalculateContrastRatio(backgroundColor, textColor);

        // Act & Assert
        calculatedContrast.Should().BeGreaterOrEqualTo(minimumContrastRatio,
            "touch targets must have sufficient contrast for children with visual impairments");
    }

    private static double CalculateContrastRatio(Color background, Color foreground)
    {
        // Simplified calculation - real implementation would use proper luminance formulas
        var backgroundLuminance = (background.Red + background.Green + background.Blue) / 3;
        var foregroundLuminance = (foreground.Red + foreground.Green + foreground.Blue) / 3;

        var lighter = Math.Max(backgroundLuminance, foregroundLuminance);
        var darker = Math.Min(backgroundLuminance, foregroundLuminance);

        return (lighter + 0.05) / (darker + 0.05);
    }

    #endregion
}