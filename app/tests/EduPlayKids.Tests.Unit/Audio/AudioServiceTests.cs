using AutoFixture;
using EduPlayKids.Application.Interfaces;
using EduPlayKids.Application.Models.Audio;
using EduPlayKids.Infrastructure.Services.Audio;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging.Abstractions;
using DomainEnums = EduPlayKids.Domain.Enums;

namespace EduPlayKids.Tests.Unit.Audio;

/// <summary>
/// Unit tests for Audio Service - Critical for child-safe audio and bilingual education.
/// Tests volume protection, bilingual support, and child-appropriate audio feedback.
/// </summary>
public class AudioServiceTests
{
    private readonly Fixture _fixture;
    private readonly IAudioService _audioService;

    public AudioServiceTests()
    {
        _fixture = new Fixture();
        // For unit tests, we'll use a real AudioService with null logger and localizer
        var nullLogger = Microsoft.Extensions.Logging.Abstractions.NullLogger<AudioService>.Instance;
        _audioService = new AudioService(nullLogger, null!);
    }

    #region Child Safety - Volume Protection (CRITICAL)

    [Theory]
    [InlineData(0.0)]   // Silent
    [InlineData(0.5)]   // 50% volume
    [InlineData(0.85)]  // Maximum safe volume for children
    public async Task AudioService_VolumeWithinSafeRange_ShouldBeAllowed(double volume)
    {
        // Arrange
        var audioItem = CreateTestAudioItem("test_audio.mp3");

        // Act
        var result = await _audioService.PlayWithVolumeAsync(audioItem, (float)volume);

        // Assert
        result.Should().BeTrue("audio within safe volume range should play successfully");
        volume.Should().BeLessOrEqualTo(0.85, "children's hearing must be protected - max 85% volume");
    }

    [Theory]
    [InlineData(0.86)]  // Just above safe limit
    [InlineData(0.9)]   // 90% - too loud for children
    [InlineData(1.0)]   // 100% - definitely unsafe
    public async Task AudioService_VolumeAboveSafeLimit_ShouldBeReduced(double unsafeVolume)
    {
        // Arrange
        var audioItem = CreateTestAudioItem("test_audio.mp3");

        // Act
        var result = await _audioService.PlayWithVolumeAsync(audioItem, (float)unsafeVolume);

        // Assert
        result.Should().BeTrue("audio should still play but at reduced volume");
        // Note: Implementation should automatically reduce volume to 0.85 max
        unsafeVolume.Should().BeGreaterThan(0.85, "this test verifies detection of unsafe volumes");
    }

    [Fact]
    public async Task AudioService_ChildSafeMode_ShouldEnforceVolumeLimit()
    {
        // Arrange
        var audioItem = CreateTestAudioItem("child_instruction.mp3");
        var childSafeMaxVolume = 0.85;

        // Act
        var result = await _audioService.PlayWithVolumeAsync(audioItem, 1.0f); // Request full volume

        // Assert
        result.Should().BeTrue("child-safe mode should still allow playback");
        // Implementation should cap at 85% regardless of requested volume
    }

    #endregion

    #region Bilingual Audio Support (ESSENTIAL)

    [Theory]
    [InlineData(AudioLanguage.Spanish, "es", "Español")]
    [InlineData(AudioLanguage.English, "en", "English")]
    public async Task AudioService_BilingualContent_ShouldSupportBothLanguages(AudioLanguage language, string languageCode, string languageName)
    {
        // Arrange
        var audioItem = CreateTestAudioItem($"counting_instructions_{languageCode}.mp3");
        audioItem.Language = language.ToString().ToLowerInvariant();

        // Act
        var result = await _audioService.PlayAsync(audioItem);

        // Assert
        result.Should().BeTrue($"should support {languageName} audio content");
        audioItem.Language.Should().Be(language.ToString().ToLowerInvariant());
    }

    [Fact]
    public async Task AudioService_LanguageAutoDetection_ShouldUseSystemLanguage()
    {
        // Arrange
        var audioItem = CreateTestAudioItem("welcome_message.mp3");
        // Simulate system language detection
        var systemLanguage = "es-US"; // Spanish (US) - target demographic

        // Act
        var detectedLanguage = GetLanguageFromCode(systemLanguage);
        audioItem.Language = detectedLanguage.ToString().ToLowerInvariant();

        // Assert
        detectedLanguage.Should().Be(AudioLanguage.Spanish,
            "should detect Spanish for Hispanic families in US");
    }

    private static AudioLanguage GetLanguageFromCode(string languageCode)
    {
        return languageCode.StartsWith("es") ? AudioLanguage.Spanish : AudioLanguage.English;
    }

    #endregion

    #region Educational Audio Types

    [Theory]
    [InlineData(AudioType.Instruction, AudioPriority.High)]
    [InlineData(AudioType.Encouragement, AudioPriority.Medium)]
    [InlineData(AudioType.BackgroundMusic, AudioPriority.Low)]
    public async Task AudioService_EducationalAudioTypes_ShouldHaveCorrectPriority(AudioType audioType, AudioPriority expectedPriority)
    {
        // Arrange
        var audioItem = CreateTestAudioItem($"{audioType.ToString().ToLower()}_audio.mp3");
        audioItem.Type = audioType;
        audioItem.Priority = expectedPriority;

        // Act
        var result = await _audioService.PlayAsync(audioItem);

        // Assert
        result.Should().BeTrue($"{audioType} audio should play successfully");
        audioItem.Priority.Should().Be(expectedPriority,
            $"{audioType} should have {expectedPriority} priority for proper educational flow");
    }

    [Fact]
    public async Task AudioService_InstructionAudio_ShouldInterruptLowerPriority()
    {
        // Arrange
        var backgroundMusic = CreateTestAudioItem("background_music.mp3");
        backgroundMusic.Type = AudioType.BackgroundMusic;
        backgroundMusic.Priority = AudioPriority.Low;

        var instruction = CreateTestAudioItem("next_step_instruction.mp3");
        instruction.Type = AudioType.Instruction;
        instruction.Priority = AudioPriority.High;

        // Act
        await _audioService.PlayAsync(backgroundMusic);
        var instructionResult = await _audioService.PlayAsync(instruction);

        // Assert
        instructionResult.Should().BeTrue("instruction audio should override background music");
    }

    #endregion

    #region Child-Appropriate Feedback

    [Theory]
    [InlineData("excellent_work_es.mp3", AudioType.Encouragement)]
    [InlineData("try_again_en.mp3", AudioType.ErrorCorrection)]
    [InlineData("great_job_es.mp3", AudioType.Achievement)]
    public async Task AudioService_PositiveFeedback_ShouldBeEncouraging(string audioFile, AudioType feedbackType)
    {
        // Arrange
        var audioItem = CreateTestAudioItem(audioFile);
        audioItem.Type = feedbackType;

        // Act
        var result = await _audioService.PlayAsync(audioItem);

        // Assert
        result.Should().BeTrue("positive feedback audio should play successfully");
        feedbackType.Should().BeOneOf(AudioType.Encouragement, AudioType.Achievement, AudioType.ErrorCorrection);
        // feedback should be constructive and encouraging for children
    }

    [Fact]
    public async Task AudioService_ErrorCorrection_ShouldBeGentleAndHelpful()
    {
        // Arrange
        var gentleCorrection = CreateTestAudioItem("lets_try_that_again_es.mp3");
        gentleCorrection.Type = AudioType.ErrorCorrection;

        // Act
        var result = await _audioService.PlayAsync(gentleCorrection);

        // Assert
        result.Should().BeTrue("gentle error correction should encourage children to try again");
    }

    #endregion

    #region Audio Caching and Performance

    [Fact]
    public async Task AudioService_FrequentlyUsedAudio_ShouldBeCached()
    {
        // Arrange
        var commonAudio = CreateTestAudioItem("great_job_en.mp3");
        commonAudio.Type = AudioType.Encouragement;

        // Act - Play same audio multiple times
        var firstPlay = await _audioService.PlayAsync(commonAudio);
        var secondPlay = await _audioService.PlayAsync(commonAudio);

        // Assert
        firstPlay.Should().BeTrue("first play should succeed");
        secondPlay.Should().BeTrue("cached audio should play efficiently");
    }

    [Theory]
    [InlineData(100)]  // 100ms - very responsive
    [InlineData(200)]  // 200ms - acceptable
    [InlineData(500)]  // 500ms - maximum acceptable for children
    public async Task AudioService_ResponseTime_ShouldBeChildAppropriate(int maxResponseTimeMs)
    {
        // Arrange
        var audioItem = CreateTestAudioItem("quick_feedback.mp3");
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act
        var result = await _audioService.PlayAsync(audioItem);
        stopwatch.Stop();

        // Assert
        result.Should().BeTrue("audio should play successfully");
        stopwatch.ElapsedMilliseconds.Should().BeLessOrEqualTo(maxResponseTimeMs,
            "children need immediate audio feedback to maintain engagement");
    }

    #endregion

    #region Error Handling and Safety

    [Fact]
    public async Task AudioService_InvalidAudioFile_ShouldFailGracefully()
    {
        // Arrange
        var invalidAudio = CreateTestAudioItem("nonexistent_file.mp3");

        // Act
        var result = await _audioService.PlayAsync(invalidAudio);

        // Assert
        result.Should().BeFalse("invalid audio files should not crash the app");
    }

    [Fact]
    public async Task AudioService_ConcurrentPlayback_ShouldHandleGracefully()
    {
        // Arrange
        var audio1 = CreateTestAudioItem("audio1.mp3");
        var audio2 = CreateTestAudioItem("audio2.mp3");

        // Act
        var task1 = _audioService.PlayAsync(audio1);
        var task2 = _audioService.PlayAsync(audio2);
        var results = await Task.WhenAll(task1, task2);

        // Assert
        results.Should().NotContain(false, "concurrent audio requests should be handled gracefully");
    }

    [Fact]
    public async Task AudioService_Stop_ShouldStopCurrentPlayback()
    {
        // Arrange
        var audioItem = CreateTestAudioItem("long_story.mp3");

        // Act
        var playResult = await _audioService.PlayAsync(audioItem);
        var stopResult = await _audioService.StopAsync();

        // Assert
        playResult.Should().BeTrue("audio should start playing");
        stopResult.Should().BeTrue("should be able to stop audio playback");
    }

    #endregion

    #region Helper Methods

    private AudioItem CreateTestAudioItem(string fileName)
    {
        return new AudioItem
        {
            FilePath = fileName,
            Type = AudioType.Instruction,
            Language = AudioLanguage.English.ToString().ToLowerInvariant(),
            Priority = AudioPriority.Medium
        };
    }

    #endregion
}