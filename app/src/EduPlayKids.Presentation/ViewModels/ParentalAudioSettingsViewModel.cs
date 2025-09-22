using EduPlayKids.Application.Interfaces;
using EduPlayKids.Application.Models.Audio;
using EduPlayKids.Infrastructure.Services.Audio;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace EduPlayKids.App.ViewModels;

/// <summary>
/// ViewModel for parental audio settings page.
/// Provides comprehensive audio configuration options for parents to customize their child's learning experience.
/// </summary>
public class ParentalAudioSettingsViewModel : AudioAwareBaseViewModel
{
    #region Private Fields

    private float _masterVolume = 0.7f;
    private bool _autoPauseEnabled = true;
    private bool _fadeTransitionsEnabled = true;
    private bool _hearingProtectionEnabled = true;
    private AudioLanguageOption? _selectedAudioLanguage;
    private string _audioCacheStatus = "Loading...";
    private double _cacheUsagePercentage = 0.0;
    private bool _canClearCache = false;
    private readonly AudioConfiguration _audioConfiguration;

    #endregion

    #region Constructor

    public ParentalAudioSettingsViewModel(
        ILogger<ParentalAudioSettingsViewModel> logger,
        IAudioService? audioService = null,
        AudioConfiguration? audioConfiguration = null)
        : base(logger, audioService)
    {
        _audioConfiguration = audioConfiguration ?? AudioConfiguration.Default;

        Title = "Audio Settings";
        BusyText = "Loading audio settings...";

        InitializeCommands();
        InitializeAvailableLanguages();
        _ = Task.Run(LoadSettingsAsync);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the master volume level.
    /// </summary>
    public float MasterVolume
    {
        get => _masterVolume;
        set
        {
            if (SetProperty(ref _masterVolume, Math.Clamp(value, 0.0f, GetMaxVolumeLimit())))
            {
                OnPropertyChanged(nameof(MasterVolumePercentage));
                _ = Task.Run(ApplyMasterVolumeAsync);
            }
        }
    }

    /// <summary>
    /// Gets the master volume as a percentage string.
    /// </summary>
    public string MasterVolumePercentage => $"{MasterVolume:P0}";

    /// <summary>
    /// Gets or sets whether auto-pause is enabled.
    /// </summary>
    public bool AutoPauseEnabled
    {
        get => _autoPauseEnabled;
        set => SetProperty(ref _autoPauseEnabled, value);
    }

    /// <summary>
    /// Gets or sets whether fade transitions are enabled.
    /// </summary>
    public bool FadeTransitionsEnabled
    {
        get => _fadeTransitionsEnabled;
        set => SetProperty(ref _fadeTransitionsEnabled, value);
    }

    /// <summary>
    /// Gets or sets whether hearing protection is enabled.
    /// </summary>
    public bool HearingProtectionEnabled
    {
        get => _hearingProtectionEnabled;
        set
        {
            if (SetProperty(ref _hearingProtectionEnabled, value))
            {
                UpdateVolumeConstraints();
            }
        }
    }

    /// <summary>
    /// Gets or sets the selected audio language.
    /// </summary>
    public AudioLanguageOption? SelectedAudioLanguage
    {
        get => _selectedAudioLanguage;
        set
        {
            if (SetProperty(ref _selectedAudioLanguage, value) && value != null)
            {
                _ = Task.Run(() => ApplyLanguageChangeAsync(value.LanguageCode));
            }
        }
    }

    /// <summary>
    /// Gets the collection of available audio languages.
    /// </summary>
    public ObservableCollection<AudioLanguageOption> AvailableLanguages { get; } = new();

    /// <summary>
    /// Gets or sets the audio cache status text.
    /// </summary>
    public string AudioCacheStatus
    {
        get => _audioCacheStatus;
        set => SetProperty(ref _audioCacheStatus, value);
    }

    /// <summary>
    /// Gets or sets the cache usage percentage for progress display.
    /// </summary>
    public double CacheUsagePercentage
    {
        get => _cacheUsagePercentage;
        set => SetProperty(ref _cacheUsagePercentage, value);
    }

    /// <summary>
    /// Gets or sets whether the clear cache button is enabled.
    /// </summary>
    public bool CanClearCache
    {
        get => _canClearCache;
        set => SetProperty(ref _canClearCache, value);
    }

    #endregion

    #region Commands

    public ICommand MuteAllCommand { get; private set; } = null!;
    public ICommand SetLowVolumeCommand { get; private set; } = null!;
    public ICommand SetHighVolumeCommand { get; private set; } = null!;
    public ICommand ClearAudioCacheCommand { get; private set; } = null!;
    public ICommand ResetAllSettingsCommand { get; private set; } = null!;
    public ICommand SaveSettingsCommand { get; private set; } = null!;

    #endregion

    #region Initialization

    private void InitializeCommands()
    {
        MuteAllCommand = new Command(async () => await MuteAllAudioAsync());
        SetLowVolumeCommand = new Command(() => MasterVolume = 0.3f);
        SetHighVolumeCommand = new Command(() => MasterVolume = GetMaxVolumeLimit());
        ClearAudioCacheCommand = new Command(async () => await ClearAudioCacheAsync(), () => CanClearCache);
        ResetAllSettingsCommand = new Command(async () => await ResetAllSettingsAsync());
        SaveSettingsCommand = new Command(async () => await SaveSettingsAsync());
    }

    private void InitializeAvailableLanguages()
    {
        AvailableLanguages.Clear();
        AvailableLanguages.Add(new AudioLanguageOption
        {
            LanguageCode = "en",
            DisplayName = "English",
            NativeName = "English",
            Flag = "🇺🇸"
        });
        AvailableLanguages.Add(new AudioLanguageOption
        {
            LanguageCode = "es",
            DisplayName = "Spanish",
            NativeName = "Español",
            Flag = "🇪🇸"
        });

        // Set default to English
        SelectedAudioLanguage = AvailableLanguages.FirstOrDefault(l => l.LanguageCode == "en");
    }

    #endregion

    #region Settings Management

    private async Task LoadSettingsAsync()
    {
        try
        {
            if (_audioService == null) return;

            // Load current volume levels
            MasterVolume = _audioService.GetVolumeLevel(AudioType.UIInteraction);

            // Load current language
            var currentLanguage = _audioService.GetCurrentAudioLanguage();
            SelectedAudioLanguage = AvailableLanguages.FirstOrDefault(l => l.LanguageCode == currentLanguage)
                                   ?? AvailableLanguages.First();

            // Load configuration settings
            AutoPauseEnabled = _audioConfiguration.ChildSafety.AutoPauseOnInactivity;
            FadeTransitionsEnabled = _audioConfiguration.ChildSafety.RequireFadeTransitions;
            HearingProtectionEnabled = _audioConfiguration.ChildSafety.EnableHearingProtection;

            // Load cache information
            await UpdateCacheInformationAsync();

            _logger.LogInformation("Audio settings loaded successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading audio settings");
            await ShowErrorAsync("Settings Error", "Could not load audio settings. Using default values.");
        }
    }

    private async Task SaveSettingsAsync()
    {
        await ExecuteAsync(async () =>
        {
            try
            {
                // Save configuration changes
                _audioConfiguration.ChildSafety.AutoPauseOnInactivity = AutoPauseEnabled;
                _audioConfiguration.ChildSafety.RequireFadeTransitions = FadeTransitionsEnabled;
                _audioConfiguration.ChildSafety.EnableHearingProtection = HearingProtectionEnabled;

                if (HearingProtectionEnabled)
                {
                    _audioConfiguration.ApplyChildSafetyConstraints();
                }

                // Apply language change
                if (SelectedAudioLanguage != null && _audioService != null)
                {
                    await _audioService.SetAudioLanguageAsync(SelectedAudioLanguage.LanguageCode);
                }

                // Save settings to persistent storage (would implement actual persistence)
                await Task.Delay(500); // Simulate save operation

                await ShowConfirmationAsync("Settings Saved", "Audio settings have been saved successfully!");

                _logger.LogInformation("Audio settings saved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving audio settings");
                throw;
            }
        }, "Saving settings...");
    }

    private async Task ResetAllSettingsAsync()
    {
        var confirm = await ShowConfirmationAsync(
            "Reset Settings",
            "Are you sure you want to reset all audio settings to default values?",
            "Reset", "Cancel");

        if (!confirm) return;

        await ExecuteAsync(async () =>
        {
            try
            {
                // Reset to default configuration
                var defaultConfig = AudioConfiguration.Default;

                MasterVolume = 0.7f;
                AutoPauseEnabled = defaultConfig.ChildSafety.AutoPauseOnInactivity;
                FadeTransitionsEnabled = defaultConfig.ChildSafety.RequireFadeTransitions;
                HearingProtectionEnabled = defaultConfig.ChildSafety.EnableHearingProtection;

                // Reset language to system default
                SelectedAudioLanguage = AvailableLanguages.FirstOrDefault(l => l.LanguageCode == "en");

                // Reset all individual audio type volumes
                if (_audioService != null)
                {
                    foreach (AudioType audioType in Enum.GetValues<AudioType>())
                    {
                        var defaultVolume = defaultConfig.DefaultVolumes.GetValueOrDefault(audioType, 0.7f);
                        await _audioService.SetVolumeLevelAsync(audioType, defaultVolume);
                    }
                }

                await Task.Delay(500); // Simulate reset operation

                _logger.LogInformation("Audio settings reset to defaults");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting audio settings");
                throw;
            }
        }, "Resetting settings...");
    }

    #endregion

    #region Volume Management

    private async Task ApplyMasterVolumeAsync()
    {
        if (_audioService == null) return;

        try
        {
            // Apply master volume to all audio types proportionally
            foreach (AudioType audioType in Enum.GetValues<AudioType>())
            {
                var defaultVolume = _audioConfiguration.DefaultVolumes.GetValueOrDefault(audioType, 0.7f);
                var adjustedVolume = defaultVolume * MasterVolume;
                await _audioService.SetVolumeLevelAsync(audioType, adjustedVolume);
            }

            _logger.LogDebug("Applied master volume: {Volume}", MasterVolume);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error applying master volume");
        }
    }

    private async Task MuteAllAudioAsync()
    {
        if (_audioService == null) return;

        try
        {
            MasterVolume = 0.0f;
            await StopAllAudioAsync();

            _logger.LogInformation("All audio muted");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error muting all audio");
        }
    }

    private float GetMaxVolumeLimit()
    {
        return HearingProtectionEnabled ? 0.85f : 1.0f;
    }

    private void UpdateVolumeConstraints()
    {
        var maxVolume = GetMaxVolumeLimit();

        // Adjust current volume if it exceeds new limit
        if (MasterVolume > maxVolume)
        {
            MasterVolume = maxVolume;
        }
    }

    #endregion

    #region Language Management

    private async Task ApplyLanguageChangeAsync(string languageCode)
    {
        if (_audioService == null) return;

        try
        {
            var success = await _audioService.SetAudioLanguageAsync(languageCode);
            if (success)
            {
                // Play test audio in new language
                await Task.Delay(500);
                await PlayInstructionAsync("language_changed");

                _logger.LogInformation("Audio language changed to: {Language}", languageCode);
            }
            else
            {
                _logger.LogWarning("Failed to change audio language to: {Language}", languageCode);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing audio language to: {Language}", languageCode);
        }
    }

    #endregion

    #region Cache Management

    private async Task UpdateCacheInformationAsync()
    {
        try
        {
            if (_audioService == null)
            {
                AudioCacheStatus = "Audio service not available";
                CanClearCache = false;
                return;
            }

            var cacheInfo = _audioService.GetCacheInfo();

            var sizeMB = cacheInfo.TotalCacheSizeBytes / (1024.0 * 1024.0);
            var maxSizeMB = cacheInfo.MaxCacheSizeBytes / (1024.0 * 1024.0);

            AudioCacheStatus = $"{cacheInfo.CachedFileCount} files, {sizeMB:F1}MB / {maxSizeMB:F1}MB";
            CacheUsagePercentage = cacheInfo.CacheUtilization;
            CanClearCache = cacheInfo.CachedFileCount > 0;

            ((Command)ClearAudioCacheCommand).ChangeCanExecute();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating cache information");
            AudioCacheStatus = "Error loading cache info";
            CanClearCache = false;
        }
    }

    private async Task ClearAudioCacheAsync()
    {
        if (_audioService == null) return;

        var confirm = await ShowConfirmationAsync(
            "Clear Audio Cache",
            "This will clear cached audio files. They will be reloaded when needed. Continue?",
            "Clear", "Cancel");

        if (!confirm) return;

        await ExecuteAsync(async () =>
        {
            try
            {
                var freedBytes = await _audioService.ClearAudioCacheAsync(keepRecent: false);
                var freedMB = freedBytes / (1024.0 * 1024.0);

                await UpdateCacheInformationAsync();

                await ShowConfirmationAsync(
                    "Cache Cleared",
                    $"Successfully cleared {freedMB:F1}MB of cached audio files.",
                    "OK");

                _logger.LogInformation("Audio cache cleared: {FreedMB}MB", freedMB);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing audio cache");
                throw;
            }
        }, "Clearing audio cache...");
    }

    #endregion

    #region Page Lifecycle

    public override async Task OnAppearingAsync()
    {
        await base.OnAppearingAsync();
        await UpdateCacheInformationAsync();
    }

    protected override async Task PlayPageIntroductionAudio()
    {
        await PlayInstructionAsync("parental_settings_welcome");
    }

    #endregion
}

/// <summary>
/// Represents an available audio language option.
/// </summary>
public class AudioLanguageOption
{
    /// <summary>
    /// Gets or sets the language code (e.g., "en", "es").
    /// </summary>
    public string LanguageCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the display name in English.
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the native name of the language.
    /// </summary>
    public string NativeName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the flag emoji for the language.
    /// </summary>
    public string Flag { get; set; } = string.Empty;

    /// <summary>
    /// Gets the formatted display text for the picker.
    /// </summary>
    public string FormattedDisplayName => $"{Flag} {DisplayName} ({NativeName})";
}