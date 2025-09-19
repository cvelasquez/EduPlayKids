# Installation Guide - EduPlayKids

This guide provides step-by-step instructions for setting up the EduPlayKids development environment.

## 📋 Prerequisites

Before starting, ensure your development machine meets the following requirements:

### System Requirements

#### Windows
- **Operating System**: Windows 10 version 1909 or higher, or Windows 11
- **Visual Studio**: 2022 version 17.3 or later with .NET MAUI workload
- **Memory**: Minimum 8 GB RAM (16 GB recommended)
- **Storage**: At least 10 GB free space

#### macOS
- **Operating System**: macOS 10.15 (Catalina) or later
- **Xcode**: Latest version from Mac App Store
- **Memory**: Minimum 8 GB RAM (16 GB recommended)
- **Storage**: At least 15 GB free space

#### Linux
- **Operating System**: Ubuntu 20.04+ or equivalent
- **Memory**: Minimum 8 GB RAM (16 GB recommended)
- **Storage**: At least 8 GB free space

## 🛠️ Development Tools Installation

### 1. Install .NET 8.0 SDK

Download and install the latest .NET 8.0 SDK from the official Microsoft website:

**Windows & macOS:**
```bash
# Download from: https://dotnet.microsoft.com/download/dotnet/8.0
# Or use package managers:

# Windows (using Chocolatey)
choco install dotnet-8.0-sdk

# macOS (using Homebrew)
brew install --cask dotnet-sdk
```

**Linux (Ubuntu):**
```bash
# Add Microsoft package repository
wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb

# Install .NET SDK
sudo apt-get update
sudo apt-get install -y dotnet-sdk-8.0
```

**Verify installation:**
```bash
dotnet --version
# Should output: 8.0.x or higher
```

### 2. Install .NET MAUI Workload

Install the .NET MAUI workload which includes all necessary templates and tools:

```bash
dotnet workload install maui
```

**Verify MAUI installation:**
```bash
dotnet workload list
# Should show: maui (installed)
```

### 3. Install Visual Studio or Visual Studio Code

#### Option A: Visual Studio 2022 (Recommended for Windows)

1. Download [Visual Studio 2022](https://visualstudio.microsoft.com/vs/)
2. During installation, select the following workloads:
   - **.NET Multi-platform App UI development**
   - **Mobile development with .NET**
3. Individual components to include:
   - Android SDK setup (API 33)
   - Android NDK
   - Intel Hardware Accelerated Execution Manager (HAXM)

#### Option B: Visual Studio Code (Cross-platform)

1. Download [Visual Studio Code](https://code.visualstudio.com/)
2. Install the following extensions:
   - **C# Dev Kit**
   - **.NET MAUI Extension Pack**
   - **Android iOS Emulator**

### 4. Android Development Setup

#### Install Android SDK

**If using Visual Studio 2022:**
- The Android SDK is installed automatically with the MAUI workload

**If using Visual Studio Code or command line:**
```bash
# Download Android Studio or command line tools
# Set ANDROID_HOME environment variable
export ANDROID_HOME=$HOME/Android/Sdk
export PATH=$PATH:$ANDROID_HOME/tools:$ANDROID_HOME/platform-tools
```

#### Configure Android Emulator

1. Open Android Studio or use `avdmanager`
2. Create a new Virtual Device:
   - **Device**: Pixel 5 or similar
   - **API Level**: 33 (Android 13) - Target
   - **API Level**: 21 (Android 5.0) - Minimum supported
   - **RAM**: 2048 MB or higher

**Create emulator via command line:**
```bash
# List available system images
avdmanager list targets

# Create AVD
avdmanager create avd -n "EduPlayKids_Emulator" -k "system-images;android-33;google_apis;x86_64"
```

### 5. iOS Development Setup (macOS only)

1. **Install Xcode**: Download from Mac App Store
2. **Install Xcode Command Line Tools**:
   ```bash
   xcode-select --install
   ```
3. **Accept Xcode license**:
   ```bash
   sudo xcodebuild -license accept
   ```

## 📦 Project Setup

### 1. Clone the Repository

```bash
git clone https://github.com/your-org/EduPlayKids.git
cd EduPlayKids
```

### 2. Install Dependencies

```bash
# Restore NuGet packages
dotnet restore

# Trust development certificates (first time only)
dotnet dev-certs https --trust
```

### 3. Database Setup

The project uses SQLite with Entity Framework Core. Initialize the database:

```bash
# Install EF tools globally (if not already installed)
dotnet tool install --global dotnet-ef

# Apply database migrations
dotnet ef database update --project src/EduPlayKids.Infrastructure
```

### 4. Build the Project

```bash
# Clean previous builds
dotnet clean

# Build all projects
dotnet build

# Build for specific platform
dotnet build -f net8.0-android
```

## 🚀 Running the Application

### Android

#### Using Visual Studio 2022
1. Set **EduPlayKids** as startup project
2. Select **Android** framework
3. Choose your Android emulator or connected device
4. Press **F5** or click **Run**

#### Using Command Line
```bash
# Start Android emulator (if not running)
emulator -avd EduPlayKids_Emulator

# Run the app
dotnet run -f net8.0-android
```

### iOS (macOS only)

#### Using Visual Studio for Mac
1. Set **EduPlayKids** as startup project
2. Select **iOS** framework
3. Choose iOS Simulator
4. Press **F5** or click **Run**

#### Using Command Line
```bash
dotnet run -f net8.0-ios
```

## 🧪 Running Tests

Execute the test suite to ensure everything is working correctly:

```bash
# Run all tests
dotnet test

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test project
dotnet test tests/EduPlayKids.Domain.Tests/
```

## 🔧 Development Environment Configuration

### 1. Configure User Secrets (Development)

For sensitive configuration during development:

```bash
# Initialize user secrets
dotnet user-secrets init --project src/EduPlayKids.Presentation

# Add development configuration
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Data Source=eduplaykids_dev.db" --project src/EduPlayKids.Presentation
```

### 2. Configure Logging

Edit `appsettings.Development.json` for development logging:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "EduPlayKids": "Debug",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  }
}
```

### 3. Hot Reload Setup

Enable hot reload for faster development:

```bash
# Enable hot reload for XAML
dotnet watch run -f net8.0-android
```

## 🏗️ Build Configurations

### Debug Configuration
- Full debugging symbols
- Detailed logging enabled
- No code optimization
- Development database

### Release Configuration
- Optimized code
- Minimal logging
- Production database
- Code signing for distribution

```bash
# Build for release
dotnet build -c Release -f net8.0-android

# Publish for distribution
dotnet publish -c Release -f net8.0-android
```

## 🔍 Troubleshooting

### Common Issues

#### 1. MAUI Workload Issues
```bash
# Reinstall MAUI workload
dotnet workload uninstall maui
dotnet workload install maui
```

#### 2. Android SDK Issues
- Ensure `ANDROID_HOME` environment variable is set
- Verify Android SDK tools are in PATH
- Check emulator can be started manually

#### 3. Build Errors
```bash
# Clean and rebuild
dotnet clean
dotnet restore
dotnet build
```

#### 4. Database Issues
```bash
# Reset database
dotnet ef database drop --force --project src/EduPlayKids.Infrastructure
dotnet ef database update --project src/EduPlayKids.Infrastructure
```

### Performance Optimization

For better development performance:

1. **Increase emulator RAM**: Set to 4096 MB if available
2. **Enable hardware acceleration**: Ensure Intel HAXM is installed
3. **Use SSD storage**: For faster build times
4. **Close unnecessary applications**: Free up system resources

## 📞 Support

If you encounter issues during installation:

1. Check the [Troubleshooting Guide](docs/technical/troubleshooting/)
2. Review [Common Issues](docs/technical/troubleshooting/common-issues.md)
3. Create an issue on the project repository
4. Contact the development team

## 📋 Verification Checklist

After completing installation, verify:

- [ ] .NET 8.0 SDK installed and accessible
- [ ] MAUI workload installed successfully
- [ ] Android SDK and emulator configured
- [ ] Project builds without errors
- [ ] Tests pass successfully
- [ ] App runs on Android emulator
- [ ] Database migrations applied
- [ ] Hot reload working (optional)

**🎉 Congratulations! Your EduPlayKids development environment is ready.**

---

**Next Steps**: Review the [Contributing Guide](CONTRIBUTING.md) and [Development Documentation](docs/technical/development/) to start contributing to the project.