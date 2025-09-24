using EduPlayKids.Application.Interfaces.Repositories;
using EduPlayKids.Application.Services;
using EduPlayKids.Domain.Entities;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace EduPlayKids.Infrastructure.Services;

/// <summary>
/// Service implementation for validating educational content answers and providing feedback.
/// Handles different question types, age-appropriate feedback, and learning analytics.
/// </summary>
public class AnswerValidationService : IAnswerValidationService
{
    private readonly IActivityQuestionRepository _questionRepository;
    private readonly IUserProgressRepository _progressRepository;
    private readonly IChildRepository _childRepository;
    private readonly IAchievementRepository _achievementRepository;
    private readonly ILogger<AnswerValidationService> _logger;

    public AnswerValidationService(
        IActivityQuestionRepository questionRepository,
        IUserProgressRepository progressRepository,
        IChildRepository childRepository,
        IAchievementRepository achievementRepository,
        ILogger<AnswerValidationService> logger)
    {
        _questionRepository = questionRepository;
        _progressRepository = progressRepository;
        _childRepository = childRepository;
        _achievementRepository = achievementRepository;
        _logger = logger;
    }

    #region Answer Validation

    public async Task<AnswerValidationResult> ValidateAnswerAsync(int questionId, int childId, object userAnswer, int timeSpentSeconds, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Validating answer for question {QuestionId}, child {ChildId}", questionId, childId);

            var question = await _questionRepository.GetByIdAsync(questionId, cancellationToken);
            if (question == null)
            {
                _logger.LogWarning("Question {QuestionId} not found", questionId);
                return CreateErrorResult("Question not found");
            }

            var child = await _childRepository.GetByIdAsync(childId, cancellationToken);
            if (child == null)
            {
                _logger.LogWarning("Child {ChildId} not found", childId);
                return CreateErrorResult("Child not found");
            }

            // Get attempt history for this question
            var attemptHistory = await _progressRepository.GetQuestionAttemptHistoryAsync(childId, questionId, cancellationToken);
            var attemptNumber = attemptHistory.Count() + 1;

            // Validate the answer
            var isCorrect = ValidateAnswerByType(question, userAnswer);
            var pointsEarned = isCorrect ? question.Points : 0;

            // Record the attempt
            question.RecordAttempt(isCorrect);
            await _questionRepository.UpdateAsync(question, cancellationToken);

            // Generate appropriate feedback
            var feedbackMessage = isCorrect
                ? GeneratePositiveFeedback(child.Age, question.QuestionType, attemptNumber == 1)
                : GenerateSupportiveFeedback(child.Age, question.QuestionType, attemptNumber, question.HintsEnabled);

            var result = new AnswerValidationResult
            {
                IsCorrect = isCorrect,
                PointsEarned = pointsEarned,
                FeedbackMessage = feedbackMessage,
                ExplanationMessage = question.GetLocalizedExplanation("en"), // TODO: Use child's language preference
                AttemptNumber = attemptNumber,
                TimeSpentSeconds = timeSpentSeconds,
                ValidationTime = DateTime.UtcNow,
                Analytics = new Dictionary<string, object>
                {
                    ["questionType"] = question.QuestionType,
                    ["difficulty"] = question.DifficultyLevel,
                    ["isFirstAttempt"] = attemptNumber == 1,
                    ["timeSpent"] = timeSpentSeconds,
                    ["childAge"] = child.Age
                }
            };

            _logger.LogInformation("Answer validation completed for question {QuestionId}: {IsCorrect}, attempt {AttemptNumber}",
                questionId, isCorrect, attemptNumber);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating answer for question {QuestionId}", questionId);
            return CreateErrorResult("Error validating answer");
        }
    }

    public async Task<ActivityValidationResult> ValidateActivityAnswersAsync(int activityId, int childId, IEnumerable<QuestionAnswer> answers, int totalTimeSpentSeconds, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Validating activity {ActivityId} for child {ChildId} with {AnswerCount} answers",
                activityId, childId, answers.Count());

            var child = await _childRepository.GetByIdAsync(childId, cancellationToken);
            if (child == null)
            {
                _logger.LogWarning("Child {ChildId} not found", childId);
                throw new ArgumentException("Child not found");
            }

            var questionResults = new List<AnswerValidationResult>();
            var correctAnswers = 0;

            // Validate each answer
            foreach (var answer in answers)
            {
                var questionResult = await ValidateAnswerAsync(answer.QuestionId, childId, answer.UserAnswer, answer.TimeSpentSeconds, cancellationToken);
                questionResults.Add(questionResult);

                if (questionResult.IsCorrect)
                {
                    correctAnswers++;
                }
            }

            var totalQuestions = answers.Count();
            var accuracyPercentage = totalQuestions > 0 ? (double)correctAnswers / totalQuestions * 100 : 0;

            // Check if this is the first completion
            var isFirstCompletion = !await _progressRepository.HasCompletedActivityAsync(childId, activityId, cancellationToken);

            // Calculate star rating
            var estimatedMinutes = 10; // TODO: Get from activity entity
            var starRating = CalculateStarRating(correctAnswers, totalQuestions, totalTimeSpentSeconds, estimatedMinutes, isFirstCompletion, child.Age);

            // Generate completion feedback
            var completionMessage = GenerateActivityCompletionFeedback(child.Age, starRating.Stars, correctAnswers, totalQuestions);

            var result = new ActivityValidationResult
            {
                ActivityId = activityId,
                ChildId = childId,
                CorrectAnswers = correctAnswers,
                TotalQuestions = totalQuestions,
                StarsEarned = starRating.Stars,
                TotalTimeSpentSeconds = totalTimeSpentSeconds,
                AccuracyPercentage = Math.Round(accuracyPercentage, 1),
                CompletionMessage = completionMessage,
                QuestionResults = questionResults,
                CompletionTime = DateTime.UtcNow,
                IsFirstCompletion = isFirstCompletion
            };

            _logger.LogInformation("Activity validation completed: {CorrectAnswers}/{TotalQuestions} correct, {Stars} stars",
                correctAnswers, totalQuestions, starRating.Stars);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating activity {ActivityId} for child {ChildId}", activityId, childId);
            throw;
        }
    }

    #endregion

    #region Feedback Generation

    public string GeneratePositiveFeedback(int childAge, string questionType, bool isFirstAttempt, string language = "en")
    {
        var messages = childAge <= 4 ? GetPreKPositiveMessages() :
                      childAge <= 6 ? GetKindergartenPositiveMessages() :
                      GetPrimaryPositiveMessages();

        if (isFirstAttempt)
        {
            messages = messages.Where(m => m.Contains("Perfect") || m.Contains("Amazing") || m.Contains("Excellent")).ToList();
        }

        var random = new Random();
        return messages[random.Next(messages.Count)];
    }

    public string GenerateSupportiveFeedback(int childAge, string questionType, int attemptNumber, bool hasHintsAvailable, string language = "en")
    {
        var messages = childAge <= 4 ? GetPreKSupportiveMessages() :
                      childAge <= 6 ? GetKindergartenSupportiveMessages() :
                      GetPrimarySupportiveMessages();

        // Add hint suggestion if available and multiple attempts
        if (hasHintsAvailable && attemptNumber >= 2)
        {
            messages = messages.Concat(new[] { "Try using the hint! 💡", "Need a little help? Check the hint! 🤔" }).ToList();
        }

        var random = new Random();
        return messages[random.Next(messages.Count)];
    }

    public string GenerateActivityCompletionFeedback(int childAge, int starsEarned, int correctAnswers, int totalQuestions, string language = "en")
    {
        var accuracy = (double)correctAnswers / totalQuestions * 100;

        return starsEarned switch
        {
            3 => childAge <= 4 ? "WOW! You're amazing! 🌟🌟🌟" : "Perfect! Outstanding work! You're a star! 🌟🌟🌟",
            2 => childAge <= 4 ? "Great job! You did so well! 🌟🌟" : "Excellent work! You're doing great! 🌟🌟",
            1 => childAge <= 4 ? "Good job! Keep learning! 🌟" : "Good effort! Keep practicing and you'll get even better! 🌟",
            _ => "Nice try! Every attempt helps you learn! Keep going! 😊"
        };
    }

    #endregion

    #region Learning Analytics

    public async Task<LearningPatternAnalysis> AnalyzeAnswerPatternsAsync(int childId, int? subjectId = null, int daysBack = 30, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Analyzing answer patterns for child {ChildId}", childId);

            var recentProgress = await _progressRepository.GetRecentProgressAsync(childId, daysBack * 10, cancellationToken); // Approximate

            var analysis = new LearningPatternAnalysis
            {
                ChildId = childId,
                AnalysisDate = DateTime.UtcNow
            };

            if (recentProgress.Any())
            {
                // Calculate overall progress
                analysis.OverallProgress = recentProgress.Average(p => p.StarsEarned) / 3.0 * 100;

                // Analyze strengths by subject (would need additional queries for full implementation)
                analysis.StrengthsBySubject = new Dictionary<string, double>
                {
                    ["Mathematics"] = 75.0,
                    ["Reading"] = 80.0,
                    ["Logic"] = 70.0
                };

                // Determine learning style based on performance patterns
                analysis.LearningStyle = DetermineLearningStyle(recentProgress);

                // Generate recommendations
                analysis.RecommendedTopics = GenerateTopicRecommendations(recentProgress);
            }

            return analysis;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing answer patterns for child {ChildId}", childId);
            return new LearningPatternAnalysis { ChildId = childId, AnalysisDate = DateTime.UtcNow };
        }
    }

    public async Task<ErrorPatternAnalysis> IdentifyErrorPatternsAsync(int? questionId = null, string? questionType = null, string? ageGroup = null, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Identifying error patterns for questionId {QuestionId}, type {QuestionType}, age {AgeGroup}",
                questionId, questionType, ageGroup);

            // This would analyze error patterns across all children
            // For now, return a basic structure
            return new ErrorPatternAnalysis
            {
                CommonErrors = new Dictionary<string, int>
                {
                    ["Rushing through questions"] = 15,
                    ["Confusion with similar options"] = 12,
                    ["Difficulty with reading instructions"] = 8
                },
                ImprovementSuggestions = new List<string>
                {
                    "Encourage taking time to read carefully",
                    "Provide more visual cues for similar options",
                    "Add audio instructions for non-readers"
                },
                ErrorRate = 25.5,
                MostChallengingConcept = "Number recognition above 10",
                AnalysisDate = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error identifying error patterns");
            return new ErrorPatternAnalysis { AnalysisDate = DateTime.UtcNow };
        }
    }

    #endregion

    #region Star Rating and Achievement Calculation

    public StarRatingResult CalculateStarRating(int correctAnswers, int totalQuestions, int timeSpentSeconds, int estimatedMinutes, bool isFirstAttempt, int childAge)
    {
        var accuracy = (double)correctAnswers / totalQuestions;
        var estimatedSeconds = estimatedMinutes * 60;
        var timeRatio = (double)estimatedSeconds / Math.Max(timeSpentSeconds, 1);

        // Age-appropriate accuracy thresholds
        var (threeStarThreshold, twoStarThreshold) = childAge <= 4 ? (0.8, 0.6) :
                                                    childAge <= 6 ? (0.85, 0.7) :
                                                    (0.9, 0.75);

        var stars = 1; // Minimum 1 star for completion

        if (accuracy >= threeStarThreshold)
        {
            stars = 3;
            // Bonus for completing quickly on first attempt
            if (isFirstAttempt && timeRatio >= 1.2) stars = 3;
        }
        else if (accuracy >= twoStarThreshold)
        {
            stars = 2;
        }

        return new StarRatingResult
        {
            Stars = stars,
            AccuracyScore = Math.Round(accuracy * 100, 1),
            TimeScore = Math.Round(timeRatio, 2),
            BonusScore = isFirstAttempt ? 0.1 : 0,
            Criteria = $"Age {childAge}: {threeStarThreshold * 100}% for 3⭐, {twoStarThreshold * 100}% for 2⭐",
            Reasoning = $"Accuracy: {accuracy * 100:F1}%, Time factor: {timeRatio:F2}, First attempt: {isFirstAttempt}"
        };
    }

    public async Task<IEnumerable<AchievementResult>> CheckForAchievementsAsync(int childId, ActivityValidationResult activityCompletion, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Checking achievements for child {ChildId}", childId);

            var achievements = new List<AchievementResult>();

            // Check for various achievement types
            if (activityCompletion.StarsEarned == 3 && activityCompletion.IsFirstCompletion)
            {
                achievements.Add(new AchievementResult
                {
                    AchievementId = 1,
                    Title = "Perfect Score!",
                    Description = "Got 3 stars on first try!",
                    BadgeImagePath = "achievements/perfect_first_try.png",
                    EarnedDate = DateTime.UtcNow,
                    CelebrationMessage = "Amazing! You nailed it perfectly! 🌟"
                });
            }

            // Check for streak achievements
            var recentProgress = await _progressRepository.GetRecentProgressAsync(childId, 5, cancellationToken);
            if (recentProgress.All(p => p.StarsEarned >= 2))
            {
                achievements.Add(new AchievementResult
                {
                    AchievementId = 2,
                    Title = "Learning Streak!",
                    Description = "Got 2+ stars on 5 activities in a row!",
                    BadgeImagePath = "achievements/learning_streak.png",
                    EarnedDate = DateTime.UtcNow,
                    CelebrationMessage = "You're on fire! Keep up the great learning! 🔥"
                });
            }

            return achievements;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking achievements for child {ChildId}", childId);
            return new List<AchievementResult>();
        }
    }

    #endregion

    #region Difficulty Adjustment Recommendations

    public DifficultyAdjustmentRecommendation AnalyzeDifficultyNeed(int childId, IEnumerable<AnswerValidationResult> recentAnswers, string currentDifficulty)
    {
        if (!recentAnswers.Any())
        {
            return new DifficultyAdjustmentRecommendation
            {
                CurrentDifficulty = currentDifficulty,
                RecommendedDifficulty = currentDifficulty,
                ShouldAdjust = false,
                Reasoning = "Not enough data for recommendation"
            };
        }

        var averageAccuracy = recentAnswers.Average(a => a.IsCorrect ? 1.0 : 0.0);
        var averageAttempts = recentAnswers.Average(a => a.AttemptNumber);

        var shouldIncreasedifficulty = averageAccuracy >= 0.85 && averageAttempts <= 1.5;
        var shouldDecreaseDefficulty = averageAccuracy <= 0.6 || averageAttempts >= 3.0;

        var recommendation = new DifficultyAdjustmentRecommendation
        {
            CurrentDifficulty = currentDifficulty,
            ConfidenceScore = Math.Abs(averageAccuracy - 0.75) * 2 // Higher confidence when further from 75%
        };

        if (shouldIncreasedifficulty && currentDifficulty != "Hard")
        {
            recommendation.RecommendedDifficulty = currentDifficulty == "Easy" ? "Medium" : "Hard";
            recommendation.ShouldAdjust = true;
            recommendation.Reasoning = $"High accuracy ({averageAccuracy * 100:F0}%) suggests readiness for increased challenge";
        }
        else if (shouldDecreaseDefficulty && currentDifficulty != "Easy")
        {
            recommendation.RecommendedDifficulty = currentDifficulty == "Hard" ? "Medium" : "Easy";
            recommendation.ShouldAdjust = true;
            recommendation.Reasoning = $"Lower accuracy ({averageAccuracy * 100:F0}%) suggests need for easier content";
        }
        else
        {
            recommendation.RecommendedDifficulty = currentDifficulty;
            recommendation.ShouldAdjust = false;
            recommendation.Reasoning = "Current difficulty level is appropriate";
        }

        return recommendation;
    }

    public async Task<PersonalizedLearningRecommendations> GenerateLearningRecommendationsAsync(int childId, int analysisWindowDays = 14, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Generating learning recommendations for child {ChildId}", childId);

            var child = await _childRepository.GetByIdAsync(childId, cancellationToken);
            var recentProgress = await _progressRepository.GetRecentProgressAsync(childId, analysisWindowDays * 2, cancellationToken);

            var recommendations = new PersonalizedLearningRecommendations();

            if (child != null)
            {
                // Age-based recommendations
                if (child.Age <= 4)
                {
                    recommendations.RecommendedActivityTypes.AddRange(new[] { "DragDrop", "Matching", "SimpleMultipleChoice" });
                    recommendations.SupportFeatures["AudioInstructions"] = true;
                    recommendations.SupportFeatures["LargeButtons"] = true;
                    recommendations.OptimalSessionLength = "5-8 minutes";
                }
                else if (child.Age <= 6)
                {
                    recommendations.RecommendedActivityTypes.AddRange(new[] { "MultipleChoice", "Tracing", "Matching", "SimplePuzzles" });
                    recommendations.SupportFeatures["VisualHints"] = true;
                    recommendations.OptimalSessionLength = "8-12 minutes";
                }
                else
                {
                    recommendations.RecommendedActivityTypes.AddRange(new[] { "MultipleChoice", "Tracing", "ComplexPuzzles", "Sequencing" });
                    recommendations.SupportFeatures["TimeLimits"] = true;
                    recommendations.OptimalSessionLength = "10-15 minutes";
                }

                // Performance-based recommendations
                if (recentProgress.Any())
                {
                    var averageStars = recentProgress.Average(p => p.StarsEarned);
                    if (averageStars >= 2.5)
                    {
                        recommendations.MotivationalStrategies.Add("Challenge with crown activities");
                        recommendations.FocusAreas.Add("Advanced concepts");
                    }
                    else if (averageStars < 2.0)
                    {
                        recommendations.MotivationalStrategies.Add("Encourage with easier content");
                        recommendations.FocusAreas.Add("Review fundamentals");
                    }
                }
            }

            return recommendations;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating learning recommendations for child {ChildId}", childId);
            return new PersonalizedLearningRecommendations();
        }
    }

    #endregion

    #region Helper Methods

    private bool ValidateAnswerByType(ActivityQuestion question, object userAnswer)
    {
        return question.QuestionType.ToLower() switch
        {
            "multiplechoice" => ValidateMultipleChoiceAnswer(question, userAnswer),
            "draganddrop" or "dragdrop" => ValidateDragDropAnswer(question, userAnswer),
            "matching" => ValidateMatchingAnswer(question, userAnswer),
            "tracing" => ValidateTracingAnswer(question, userAnswer),
            "truefalse" => ValidateTrueFalseAnswer(question, userAnswer),
            _ => question.IsAnswerCorrect(userAnswer?.ToString() ?? "")
        };
    }

    private bool ValidateMultipleChoiceAnswer(ActivityQuestion question, object userAnswer)
    {
        if (userAnswer is int selectedIndex)
        {
            try
            {
                var correctAnswers = JsonSerializer.Deserialize<int[]>(question.CorrectAnswer ?? "[]");
                return correctAnswers?.Contains(selectedIndex) ?? false;
            }
            catch
            {
                return question.IsAnswerCorrect(selectedIndex.ToString());
            }
        }
        else if (userAnswer is List<int> selectedIndexes)
        {
            try
            {
                var correctAnswers = JsonSerializer.Deserialize<int[]>(question.CorrectAnswer ?? "[]");
                return correctAnswers != null &&
                       selectedIndexes.Count == correctAnswers.Length &&
                       selectedIndexes.All(correctAnswers.Contains);
            }
            catch
            {
                return false;
            }
        }
        return false;
    }

    private bool ValidateDragDropAnswer(ActivityQuestion question, object userAnswer)
    {
        if (userAnswer is Dictionary<int, int> mapping)
        {
            try
            {
                var correctMapping = JsonSerializer.Deserialize<Dictionary<int, int>>(question.CorrectAnswer ?? "{}");
                return correctMapping != null &&
                       mapping.Count == correctMapping.Count &&
                       mapping.All(kvp => correctMapping.ContainsKey(kvp.Key) && correctMapping[kvp.Key] == kvp.Value);
            }
            catch
            {
                return false;
            }
        }
        return false;
    }

    private bool ValidateMatchingAnswer(ActivityQuestion question, object userAnswer)
    {
        // Similar to drag-drop validation
        return ValidateDragDropAnswer(question, userAnswer);
    }

    private bool ValidateTracingAnswer(ActivityQuestion question, object userAnswer)
    {
        // For tracing, we would implement sophisticated path analysis
        // For now, return a simplified validation
        return userAnswer != null;
    }

    private bool ValidateTrueFalseAnswer(ActivityQuestion question, object userAnswer)
    {
        if (userAnswer is bool boolAnswer)
        {
            var correctAnswer = bool.TryParse(question.CorrectAnswer, out var correct) && correct;
            return boolAnswer == correctAnswer;
        }
        return false;
    }

    private AnswerValidationResult CreateErrorResult(string message)
    {
        return new AnswerValidationResult
        {
            IsCorrect = false,
            PointsEarned = 0,
            FeedbackMessage = message,
            ValidationTime = DateTime.UtcNow
        };
    }

    private List<string> GetPreKPositiveMessages()
    {
        return new List<string>
        {
            "Yay! You did it! 🎉",
            "Amazing! Great job! ⭐",
            "Wow! You're so smart! 🌟",
            "Perfect! I'm so proud! 👏",
            "Excellent work! 🎊",
            "You're awesome! 😊"
        };
    }

    private List<string> GetKindergartenPositiveMessages()
    {
        return new List<string>
        {
            "Excellent! You got it right! 🌟",
            "Perfect! Amazing work! ⭐",
            "Outstanding! You're doing great! 🎉",
            "Wonderful! Keep it up! 👏",
            "Fantastic job! 🎊",
            "Brilliant! You're a star! ⭐"
        };
    }

    private List<string> GetPrimaryPositiveMessages()
    {
        return new List<string>
        {
            "Perfect! Outstanding work! 🌟",
            "Excellent! You nailed it! ⭐",
            "Amazing! Your hard work paid off! 🎉",
            "Fantastic! You're really learning! 👏",
            "Brilliant! Keep up the great work! 🎊",
            "Outstanding! You should be proud! ⭐"
        };
    }

    private List<string> GetPreKSupportiveMessages()
    {
        return new List<string>
        {
            "That's okay! Try again! 😊",
            "Almost! You can do it! 💪",
            "Good try! Let's try once more! 🤗",
            "Nice attempt! Keep going! 👍",
            "You're learning! Try again! 📚"
        };
    }

    private List<string> GetKindergartenSupportiveMessages()
    {
        return new List<string>
        {
            "Good effort! Let's try again! 😊",
            "Almost there! You can do it! 💪",
            "Nice try! Think about it once more! 🤔",
            "You're on the right track! Keep going! 👍",
            "Learning is about practice! Try again! 📚"
        };
    }

    private List<string> GetPrimarySupportiveMessages()
    {
        return new List<string>
        {
            "Good attempt! Let's think about this! 🤔",
            "You're getting closer! Try again! 💪",
            "Nice try! Take your time to think! ⏰",
            "Learning takes practice! Keep going! 📚",
            "Don't give up! You've got this! 👍"
        };
    }

    private string DetermineLearningStyle(IEnumerable<UserProgress> progress)
    {
        // Simplified learning style determination
        var averageTime = progress.Average(p => p.TimeSpentMinutes);
        return averageTime < 5 ? "Quick Learner" :
               averageTime < 10 ? "Steady Learner" :
               "Thoughtful Learner";
    }

    private List<string> GenerateTopicRecommendations(IEnumerable<UserProgress> progress)
    {
        // Generate recommendations based on performance gaps
        return new List<string> { "Number Recognition", "Letter Sounds", "Shape Matching" };
    }

    #endregion
}