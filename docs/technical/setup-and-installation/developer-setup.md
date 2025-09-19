# Developer Setup Instructions

This guide provides comprehensive step-by-step instructions for setting up the EduPlayKids development environment. It's designed for developers joining the project or setting up a new development machine.

## 📋 Prerequisites Checklist

Before starting, ensure you have:
- [ ] **Windows 10/11** (Version 1903 or later) or **macOS 12** or later
- [ ] **16GB RAM** minimum (32GB recommended for optimal Android emulator performance)
- [ ] **50GB free disk space** for tools and emulators
- [ ] **Stable internet connection** for downloading tools and packages
- [ ] **Administrator privileges** on your development machine

## 🛠️ Development Tools Installation

### Step 1: Install Visual Studio 2022

#### Windows Installation
1. **Download Visual Studio 2022 Community/Professional**
   - Go to [Visual Studio Downloads](https://visualstudio.microsoft.com/downloads/)
   - Choose **Visual Studio 2022** (Community is free)

2. **Select Workloads During Installation**
   ```
   Required Workloads:
   ✅ .NET Multi-platform App UI development
   ✅ Mobile development with .NET

   Optional but Recommended:
   ✅ Azure development
   ✅ .NET desktop development
   ```

3. **Individual Components (Verify these are selected)**
   ```
   ✅ .NET 8.0 Runtime (Latest)
   ✅ .NET SDK (Latest)
   ✅ Android SDK setup (API 21-33)
   ✅ Android SDK Platform Tools
   ✅ Intel Hardware Accelerated Execution Manager (HAXM)
   ✅ Google Android Emulator
   ```

#### macOS Installation (Alternative)
1. **Install Visual Studio for Mac**
   - Download from Microsoft website
   - Select the same workloads as Windows version

2. **Alternative: VS Code + Extensions**
   ```bash
   # Install VS Code
   brew install --cask visual-studio-code

   # Install .NET MAUI extension
   code --install-extension ms-dotnettools.dotnet-maui
   ```

### Step 2: Install .NET 8 SDK

#### Verify Installation
```bash
# Check .NET version
dotnet --version
# Should show 8.0.x or later

# List installed SDKs
dotnet --list-sdks
# Should include .NET 8.0.x

# Check MAUI workload
dotnet workload list
# Should show 'maui' as installed
```

#### Install/Update MAUI Workload
```bash
# Install MAUI workload
dotnet workload install maui

# Update to latest version
dotnet workload update

# Restore workloads (if needed)
dotnet workload restore
```

### Step 3: Android Development Setup

#### Android SDK Configuration
1. **Open Visual Studio**
2. **Go to Tools → Android → Android SDK Manager**
3. **Install Required SDK Platforms**:
   ```
   ✅ Android 13.0 (API 33) - Target platform
   ✅ Android 12.0 (API 31)
   ✅ Android 10.0 (API 29)
   ✅ Android 5.0 (API 21) - Minimum supported
   ```

4. **Install SDK Tools**:
   ```
   ✅ Android SDK Build-Tools (33.0.0 or later)
   ✅ Android SDK Platform-Tools
   ✅ Android SDK Tools
   ✅ Google Play Services
   ✅ Intel x86 Emulator Accelerator (HAXM installer)
   ```

#### Create Android Emulator
1. **Open Android Device Manager** (Tools → Android → Android Device Manager)
2. **Create New Device**:
   ```
   Device: Pixel 7 or Pixel 5
   Target: Android 13.0 (API 33)
   CPU/ABI: x86_64 (for Intel) or ARM64 (for Apple Silicon)
   RAM: 4GB
   Storage: 32GB
   ```

3. **Enable Hardware Acceleration**:
   ```
   Graphics: Hardware - GLES 2.0
   Boot option: Cold boot
   Advanced: Enable hardware keyboard
   ```

### Step 4: Additional Development Tools

#### Git Configuration
```bash
# Configure Git (replace with your details)
git config --global user.name "Your Name"
git config --global user.email "your.email@company.com"

# Set default branch name
git config --global init.defaultBranch main

# Enable long paths (Windows)
git config --global core.longpaths true
```

#### SQLite Browser (Database Inspection)
```bash
# Windows (using Chocolatey)
choco install sqlitebrowser

# macOS (using Homebrew)
brew install --cask db-browser-for-sqlite

# Or download from: https://sqlitebrowser.org/
```

#### Android Debugging Tools
```bash
# Verify ADB is working
adb version

# List connected devices/emulators
adb devices

# Install APK to device (for testing)
adb install path/to/app.apk
```

## 🚀 Project Setup

### Step 1: Clone the Repository

```bash
# Clone the repository
git clone https://github.com/your-org/EduPlayKids.git
cd EduPlayKids

# Checkout the development branch
git checkout develop

# Create your feature branch
git checkout -b feature/your-feature-name
```

### Step 2: Restore Project Dependencies

```bash
# Navigate to project root
cd src/EduPlayKids

# Restore NuGet packages
dotnet restore

# Verify project builds
dotnet build

# Run initial tests
dotnet test
```

### Step 3: Database Setup

#### Initial Database Migration
```bash
# Navigate to the infrastructure project
cd src/EduPlayKids.Infrastructure

# Create initial migration (if not exists)
dotnet ef migrations add InitialCreate --startup-project ../EduPlayKids.Presentation

# Update database
dotnet ef database update --startup-project ../EduPlayKids.Presentation

# Verify database was created
ls $HOME/.local/share/  # Look for eduplaykids.db
```

#### Seed Educational Content
```bash
# Run the application once to trigger data seeding
dotnet run --project src/EduPlayKids.Presentation

# Or run seeding script directly
dotnet run --project tools/DataSeeder
```

### Step 4: Verify Audio Assets

```bash
# Check audio files are present
ls src/EduPlayKids.Presentation/Resources/Raw/audio/

# Should contain:
# ├── en/
# │   ├── welcome.mp3
# │   ├── instructions/
# │   └── feedback/
# └── es/
#     ├── welcome.mp3
#     ├── instructions/
#     └── feedback/
```

## 🧪 Verify Installation

### Build and Run Tests
```bash
# Clean and rebuild everything
dotnet clean
dotnet build

# Run all unit tests
dotnet test --verbosity normal

# Run with coverage (optional)
dotnet test --collect:"XPlat Code Coverage"
```

### Run the Application

#### Using Visual Studio
1. **Set Startup Project**: `EduPlayKids.Presentation`
2. **Select Target**: Android Emulator or Physical Device
3. **Press F5** or click **Start Debugging**

#### Using Command Line
```bash
# Start Android emulator first
emulator -avd Pixel_7_API_33

# Build and deploy to emulator
dotnet build -f net8.0-android
dotnet run -f net8.0-android

# Or for release build
dotnet publish -f net8.0-android -c Release
```

### Verify Core Features
- [ ] **Welcome Screen** loads with age selection
- [ ] **Audio Instructions** play in both languages
- [ ] **Subject Selection** shows math, reading, etc.
- [ ] **Database** stores child progress
- [ ] **Parental Controls** PIN screen works
- [ ] **Touch Targets** are appropriately sized for children

## 🔧 Development Workflow

### Recommended VS Extensions
```
Essential Extensions:
✅ C# Dev Kit
✅ .NET MAUI Extension
✅ SQLite Viewer
✅ Thunder Client (API testing)
✅ GitLens

Optional Extensions:
✅ Markdown All in One
✅ Auto Rename Tag
✅ Bracket Pair Colorizer
✅ Error Lens
```

### Code Quality Tools

#### EditorConfig Setup
The project includes a `.editorconfig` file. Ensure your editor respects these settings:
```ini
# Already included in project root
root = true

[*.cs]
indent_style = space
indent_size = 4
charset = utf-8
trim_trailing_whitespace = true
insert_final_newline = true
```

#### Code Analysis
```bash
# Run code analysis
dotnet build --verbosity normal

# Check for style violations
dotnet format --verify-no-changes

# Fix style issues automatically
dotnet format
```

### Debugging Configuration

#### Recommended VS Code Launch Configuration
Create `.vscode/launch.json`:
```json
{
    "version": "0.2.0",
    "configurations": [
        {
            "name": "Debug Android",
            "type": "dotnet-maui",
            "request": "launch",
            "preLaunchTask": "dotnet: build",
            "program": "${workspaceFolder}/src/EduPlayKids.Presentation/bin/Debug/net8.0-android/EduPlayKids.dll"
        }
    ]
}
```

#### Debug Settings for Child Testing
```csharp
// In appsettings.Development.json
{
    "Logging": {
        "LogLevel": {
            "Default": "Information",
            "EduPlayKids": "Debug",
            "Microsoft": "Warning"
        }
    },
    "ChildSafetySettings": {
        "EnableTestMode": true,
        "BypassAgeValidation": true,
        "MaxSessionTimeMinutes": 60
    }
}
```

## 🚨 Troubleshooting Common Issues

### Android Emulator Issues

#### Emulator Won't Start
```bash
# Check HAXM installation (Windows/Intel)
sc query intelhaxm

# Restart ADB server
adb kill-server
adb start-server

# Cold boot emulator
emulator -avd Pixel_7_API_33 -wipe-data
```

#### Deployment Failures
```bash
# Clear app data
adb shell pm clear com.eduplaykids.app

# Uninstall previous version
adb uninstall com.eduplaykids.app

# Check available space
adb shell df /data
```

### .NET MAUI Build Issues

#### Workload Problems
```bash
# Clean workload installation
dotnet workload uninstall maui
dotnet workload install maui

# Update all workloads
dotnet workload update

# Repair if corrupted
dotnet workload repair
```

#### NuGet Package Issues
```bash
# Clear NuGet cache
dotnet nuget locals all --clear

# Restore packages
dotnet restore --force

# Use verbose logging to debug
dotnet restore --verbosity diagnostic
```

### Database Issues

#### Migration Conflicts
```bash
# Reset database (development only)
rm ~/.local/share/eduplaykids.db

# Drop all migrations and recreate
rm -rf Migrations/
dotnet ef migrations add InitialCreate
dotnet ef database update
```

#### Permission Issues
```bash
# Fix database file permissions (Linux/macOS)
chmod 644 ~/.local/share/eduplaykids.db

# Check SQLite installation
sqlite3 --version
```

### Audio Playback Issues

#### Missing Audio Files
```bash
# Verify audio assets are embedded
ls -la src/EduPlayKids.Presentation/Resources/Raw/audio/

# Check build action in .csproj
grep -A 10 "audio" src/EduPlayKids.Presentation/EduPlayKids.Presentation.csproj
```

#### Android Audio Permissions
Ensure `Platforms/Android/AndroidManifest.xml` includes:
```xml
<uses-permission android:name="android.permission.MODIFY_AUDIO_SETTINGS" />
<uses-permission android:name="android.permission.RECORD_AUDIO" />
```

## 🔄 Development Best Practices

### Git Workflow
```bash
# Daily workflow
git pull origin develop
git checkout -b feature/new-feature
# Make changes
git add .
git commit -m "feat: add new educational activity"
git push origin feature/new-feature
# Create Pull Request
```

### Testing Strategy
```bash
# Run tests before committing
dotnet test

# Run specific test category
dotnet test --filter Category=Unit

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Performance Monitoring
```bash
# Profile app startup time
dotnet run --configuration Release --verbosity diagnostic

# Monitor memory usage
dotnet-counters monitor --name EduPlayKids --counters System.Runtime
```

## 📱 Device Testing Setup

### Physical Android Device Setup
1. **Enable Developer Options**:
   - Go to Settings → About Phone
   - Tap "Build Number" 7 times
   - Go back to Settings → Developer Options

2. **Enable USB Debugging**:
   - In Developer Options, enable "USB Debugging"
   - Connect device via USB
   - Accept debugging dialog on device

3. **Verify Device Connection**:
   ```bash
   adb devices
   # Should show your device as "device" not "unauthorized"
   ```

### Child Testing Considerations
- **Use Child-Appropriate Test Devices**: Tablets with larger screens when possible
- **Test Touch Sensitivity**: Ensure buttons respond well to child touches
- **Volume Testing**: Check audio levels are comfortable for children
- **Session Limits**: Verify parental time controls work correctly

## 🆘 Getting Help

### Internal Resources
- **Technical Documentation**: `docs/technical/`
- **Architecture Guide**: `docs/technical/architecture/overview.md`
- **Coding Standards**: `docs/technical/development/coding-standards.md`
- **Troubleshooting Guide**: `docs/technical/troubleshooting/`

### External Resources
- **[.NET MAUI Documentation](https://docs.microsoft.com/dotnet/maui/)**
- **[Entity Framework Core Docs](https://docs.microsoft.com/ef/core/)**
- **[Android Developer Guide](https://developer.android.com/docs)**
- **[MAUI Community Toolkit](https://github.com/CommunityToolkit/Maui)**

### Support Channels
1. **GitHub Issues**: Report bugs and feature requests
2. **Team Chat**: Daily development discussions
3. **Code Reviews**: Technical guidance during PR process
4. **Architecture Reviews**: Weekly technical architecture discussions

---

**Next Steps**: After completing setup, proceed to:
1. [Project Structure Documentation](project-structure.md)
2. [Database Setup Guide](database-setup.md)
3. [Phase 4 Implementation Guide](phase-4-implementation-guide.md)

**Setup Verification**: ✅ All green checkmarks above indicate successful setup
**Last Updated**: September 2025