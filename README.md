# EduPlayKids - Educational Mobile App for Children

[![.NET MAUI](https://img.shields.io/badge/.NET%20MAUI-8.0-blue)](https://dotnet.microsoft.com/apps/maui)
[![Platform](https://img.shields.io/badge/Platform-Android%20%7C%20iOS-green)](https://developer.android.com/)
[![License](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![Language](https://img.shields.io/badge/Language-Spanish%20%7C%20English-red)](docs/technical/setup-and-installation/localization.md)

> 🎓 **Comprehensive educational mobile application for children aged 3-8 years**
> Built with .NET MAUI, focusing on offline-first functionality, gamification, and child-safe design principles.

## 🌟 Overview

EduPlayKids is an educational mobile application targeting children aged 3-8 years, covering pre-school and primary education curriculum. The app provides an offline-first experience with gamification features, bilingual support (Spanish/English), and a child-safe environment with no ads or external communication.

### 🎯 Key Features

- **📚 Five Core Educational Areas**: Mathematics, Reading & Phonics, Basic Concepts, Logic & Thinking, Science
- **🌐 Bilingual Support**: Spanish and English with auto-detection of system language
- **📱 Offline-First**: No internet connection required after installation
- **🎮 Gamification**: Star rating system, progressive difficulty levels, achievement tracking
- **👶 Child-Safe Design**: Large touch targets, high contrast colors, audio instructions for non-readers
- **💰 Freemium Model**: 3 days free trial, then $4.99 premium subscription

## 🏗️ Architecture

- **Framework**: .NET MAUI 8.0+ for cross-platform development
- **Pattern**: Clean Architecture + MVVM (Model-View-ViewModel)
- **Database**: SQLite with Entity Framework Core for local data storage
- **Target Platform**: Android (API 21+) with future iOS/Windows support
- **Privacy**: COPPA and GDPR-K compliant with local-only data storage

## 🚀 Quick Start

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [.NET MAUI Workload](https://docs.microsoft.com/dotnet/maui/get-started/installation)
- [Android SDK](https://developer.android.com/studio) (for Android development)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [Visual Studio Code](https://code.visualstudio.com/)

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/your-org/EduPlayKids.git
   cd EduPlayKids
   ```

2. **Install MAUI workload**
   ```bash
   dotnet workload install maui
   ```

3. **Restore packages**
   ```bash
   dotnet restore
   ```

4. **Build and run**
   ```bash
   # For Android
   dotnet build -f net8.0-android
   dotnet run -f net8.0-android
   ```

For detailed installation instructions, see [INSTALL.md](INSTALL.md).

## 📖 Documentation

### 📋 Project Documentation
- **[Requirements Documentation](1.1%20Documentación%20de%20Requisitos/)** - Complete PRD, functional specifications, and user analysis
- **[System Architecture](1.2%20Arquitectura%20del%20Sistema/)** - Software architecture document, C4 diagrams, and technical specifications
- **[Database Design](1.3%20Diseño%20de%20Base%20de%20Datos/)** - ERD, SQLite schema, and data dictionary
- **[UX/UI Design](2.1%20Diseño%20UXUI/)** - User journeys, wireframes, mockups, and design system
- **[Content Specifications](2.2%20Especificaciones%20de%20Contenido/)** - Educational taxonomy, progression matrix, and activity specs

### 🔧 Technical Documentation
- **[Setup & Installation](docs/technical/setup-and-installation/)** - Development environment setup
- **[API Documentation](docs/technical/api-documentation/)** - Service interfaces and data contracts
- **[Architecture](docs/technical/architecture/)** - Technical architecture details and patterns
- **[Development](docs/technical/development/)** - Coding standards, guidelines, and workflows
- **[Testing](docs/technical/testing/)** - Testing strategies, frameworks, and test cases
- **[Deployment](docs/technical/deployment/)** - Build and release processes
- **[Performance](docs/technical/performance/)** - Optimization guidelines and monitoring
- **[Security](docs/technical/security/)** - Security practices and compliance
- **[Troubleshooting](docs/technical/troubleshooting/)** - Common issues and solutions

### 👥 User Documentation
- **[User Guide](docs/user/)** - End-user documentation and tutorials

## 🛠️ Development

### Project Structure
```
EduPlayKids/
├── src/
│   ├── EduPlayKids.Domain/          # Core business logic and entities
│   ├── EduPlayKids.Application/     # Use cases and application services
│   ├── EduPlayKids.Infrastructure/  # Data access and external services
│   └── EduPlayKids.Presentation/    # MAUI Views and ViewModels
├── tests/
│   ├── EduPlayKids.Domain.Tests/
│   ├── EduPlayKids.Application.Tests/
│   └── EduPlayKids.Infrastructure.Tests/
├── docs/                            # Technical documentation
└── assets/                          # Educational content and multimedia
```

### Key Commands
```bash
# Build the project
dotnet build

# Run tests
dotnet test

# Clean build artifacts
dotnet clean

# Database migrations
dotnet ef migrations add InitialCreate
dotnet ef database update

# Build for release (Android)
dotnet publish -f net8.0-android -c Release
```

## 🎯 Target Market

- **Primary**: Hispanic families in USA (4.2M children aged 3-8)
- **Secondary**: Latin America (Mexico, Colombia, Argentina, Chile)
- **Age Groups**: Pre-K (3-4), Kindergarten (5), Primary (6-8 years)
- **Total Addressable Market**: 15.78M children with smartphone access

## 🤝 Contributing

We welcome contributions! Please see our [Contributing Guide](CONTRIBUTING.md) for details on how to get started.

### Development Workflow
1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Follow our coding standards and guidelines
4. Write tests for new functionality
5. Commit your changes (`git commit -m 'Add amazing feature'`)
6. Push to the branch (`git push origin feature/amazing-feature`)
7. Open a Pull Request

## 📅 Project Timeline

- **Phase 1** ✅: Requirements Documentation Complete
- **Phase 2** ✅: System Architecture Complete
- **Phase 2.5** ✅: Database Design Complete
- **Phase 3** ✅: UX/UI Design System Complete
- **Phase 3.5** ✅: Content Specifications Complete
- **Phase 4** 🚧: Implementation (In Progress)

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 📞 Contact

- **Project Lead**: [Your Name](mailto:your.email@example.com)
- **Organization**: [Your Organization]
- **Project Repository**: [https://github.com/your-org/EduPlayKids](https://github.com/your-org/EduPlayKids)

---

**🎓 Built with ❤️ for children's education**