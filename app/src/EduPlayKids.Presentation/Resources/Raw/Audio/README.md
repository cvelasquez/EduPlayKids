# EduPlayKids Audio Assets

This directory contains all audio assets for the EduPlayKids educational application, organized by language and category for children aged 3-8.

## Directory Structure

```
Audio/
├── audio-manifest.json          # Audio resource manifest
├── en/                          # English audio files
│   ├── ui_button_press.mp3      # UI interaction sounds
│   ├── feedback_success_*.mp3   # Success feedback sounds
│   ├── feedback_error_*.mp3     # Error correction sounds
│   ├── completion_*.mp3         # Activity completion sounds
│   ├── welcome_*.mp3            # Welcome messages
│   ├── activity_intro_*.mp3     # Activity introductions
│   ├── encouragement_*.mp3      # Encouragement messages
│   └── background_music_*.mp3   # Background music
├── es/                          # Spanish audio files
│   ├── (same structure as en/)
└── README.md                    # This file
```

## Audio Categories

### UI Interaction Sounds (50-300ms)
- Button presses and navigation sounds
- Page transitions and menu toggles
- Drag and drop feedback
- Modal appearances

### Educational Feedback (800-2000ms)
- Success celebrations (soft, medium, high intensity)
- Gentle error corrections and encouragement
- Activity completion with star ratings

### Instructional Audio (2000-5000ms)
- Welcome messages for each page
- Activity introductions by subject
- Encouragement for struggling learners
- Navigation guidance

### Background Music (120 seconds, looped)
- Subject-specific ambient music
- Low volume, non-distracting
- Optional, parent-controlled

## Audio Requirements

### Technical Specifications
- **Format**: MP3, 44.1kHz, 128kbps
- **Volume**: Normalized, child-safe levels
- **Duration**: Optimized for attention spans ages 3-8
- **Size**: Compressed for mobile performance

### Content Guidelines
- **Child-Safe**: No sudden loud sounds
- **Encouraging**: Positive, supportive tone
- **Age-Appropriate**: Simple language, clear pronunciation
- **Culturally Sensitive**: Appropriate for Hispanic families

### Language Requirements
- **Primary**: English (US pronunciation)
- **Secondary**: Spanish (International, suitable for Latin America)
- **Fallback**: English used if Spanish not available
- **Quality**: Native speaker recordings preferred

## Voice Characteristics

### English Voice
- Female or male, warm and friendly
- Clear pronunciation, moderate pace
- Appropriate for US elementary education
- Consistent across all recordings

### Spanish Voice
- Native Spanish speaker
- Clear pronunciation, slower pace for learners
- Culturally appropriate for Hispanic families
- Consistent translation and cultural adaptation

## File Naming Convention

```
{category}_{type}_{variant}.mp3

Examples:
- ui_button_press.mp3
- feedback_success_medium.mp3
- completion_three_stars.mp3
- welcome_age_selection.mp3
- activity_intro_math.mp3
- background_music_math.mp3
```

## Integration

Audio files are managed by the `BilingualAudioManager` class, which:
- Automatically detects system language
- Falls back to English if Spanish not available
- Caches frequently used audio paths
- Validates audio availability for both languages

## Development Notes

- Use placeholder files during development
- Replace with professional recordings before release
- Test all audio on actual Android devices
- Verify volume levels are appropriate for children
- Ensure cultural accuracy for Spanish content

## COPPA Compliance

All audio content must comply with COPPA requirements:
- No data collection through audio
- Child-safe content only
- Parental control over audio settings
- No advertising or promotional content