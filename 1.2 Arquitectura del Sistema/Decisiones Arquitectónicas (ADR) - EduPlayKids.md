# Decisiones Arquitectónicas (ADR) - EduPlayKids

## Índice de Decisiones

1. [ADR-001: Adopción de .NET MAUI como Framework Principal](#adr-001)
2. [ADR-002: Clean Architecture + MVVM como Patrón Arquitectónico](#adr-002)
3. [ADR-003: SQLite con Entity Framework Core para Persistencia](#adr-003)
4. [ADR-004: Estrategia Offline-First](#adr-004)
5. [ADR-005: Modelo de Datos Bilingüe](#adr-005)
6. [ADR-006: Sistema de Analytics Privacy-Compliant](#adr-006)
7. [ADR-007: Modelo Freemium con Limitaciones Locales](#adr-007)
8. [ADR-008: GitHub Actions para CI/CD](#adr-008)

---

## ADR-001: Adopción de .NET MAUI como Framework Principal {#adr-001}

### Estado
✅ **Aceptada** - 15 Septiembre 2024

### Contexto
EduPlayKids requiere una aplicación móvil multiplataforma (iOS/Android) con alto rendimiento para contenido educativo interactivo, desarrollo rápido, y capacidad de distribución por un equipo pequeño.

### Decisión
Adoptar **.NET MAUI (Multi-platform App UI)** como framework principal de desarrollo.

### Justificación

**Ventajas**:
- **Un solo codebase** para iOS y Android (90% código compartido)
- **Performance nativa** comparable a desarrollo nativo
- **Ecosistema maduro** con NuGet packages y herramientas Microsoft
- **Hot Reload** para desarrollo rápido
- **Soporte oficial** de Microsoft con roadmap a largo plazo
- **Integración natural** con Entity Framework Core y ecosystem .NET

**Desventajas Consideradas**:
- Curva de aprendizaje para equipo sin experiencia .NET
- Menor flexibilidad vs desarrollo nativo puro
- Dependencia de Microsoft para evolución del framework

**Alternativas Evaluadas**:
- **React Native**: Descartado por performance en animaciones educativas
- **Flutter**: Descartado por preferencia del equipo hacia C# vs Dart
- **Xamarin.Forms**: Descartado (deprecated, migración obligatoria a MAUI)
- **Desarrollo Nativo**: Descartado por costos de mantener 2 codebases

### Consecuencias

**Positivas**:
- Desarrollo 40% más rápido vs nativo dual
- Mantenimiento unificado de business logic
- Acceso completo a APIs nativas cuando necesario
- Facilita implementación de Clean Architecture

**Negativas**:
- Dependencia de ciclo de releases de Microsoft
- Debugging puede ser más complejo en scenarios específicos
- Algunas funcionalidades avanzadas requieren platform-specific code

### Métricas de Éxito
- [ ] Time to market < 6 meses
- [ ] Performance: 60 FPS en interacciones
- [ ] Código compartido > 85%
- [ ] App size < 50MB por plataforma

---

## ADR-002: Clean Architecture + MVVM como Patrón Arquitectónico {#adr-002}

### Estado
✅ **Aceptada** - 15 Septiembre 2024

### Contexto
La aplicación requiere alta testabilidad, separación de responsabilidades clara, y capacidad de evolución independiente de componentes para facilitar el mantenimiento a largo plazo.

### Decisión
Implementar **Clean Architecture de Robert Martin combinada con patrón MVVM** para la organización del código.

### Justificación

**Clean Architecture (4 capas)**:
1. **Presentation Layer**: Views + ViewModels (MAUI + MVVM)
2. **Application Layer**: Use Cases + Services + DTOs
3. **Domain Layer**: Entities + Business Rules + Interfaces
4. **Infrastructure Layer**: Data Access + External Services + Platform-specific

**MVVM para Presentation**:
- **Model**: Domain entities y DTOs
- **View**: MAUI Pages y Controls
- **ViewModel**: Presentation logic con data binding

**Ventajas**:
- **Testabilidad**: Cada capa testeable independientemente
- **Separation of Concerns**: Responsabilidades claramente definidas
- **Dependency Inversion**: Core business logic independiente de frameworks
- **Mantenibilidad**: Cambios aislados por capa
- **Team Collaboration**: Diferentes desarrolladores pueden trabajar en diferentes capas

**Desventajas Consideradas**:
- Mayor complejidad inicial vs arquitectura simple
- Más archivos y boilerplate code
- Curva de aprendizaje para junior developers

### Estructura del Proyecto

```
EduPlayKids/
├── EduPlayKids.Presentation/          # MAUI App
│   ├── Pages/                         # Views
│   ├── ViewModels/                    # ViewModels
│   └── Converters/                    # Value Converters
├── EduPlayKids.Application/           # Use Cases
│   ├── Services/                      # Application Services
│   ├── DTOs/                         # Data Transfer Objects
│   └── Interfaces/                   # Service Contracts
├── EduPlayKids.Domain/               # Business Logic
│   ├── Entities/                     # Domain Models
│   ├── ValueObjects/                 # Value Objects
│   └── Repositories/                 # Repository Interfaces
└── EduPlayKids.Infrastructure/       # External Concerns
    ├── Data/                         # EF Core + SQLite
    ├── Analytics/                    # Privacy Analytics
    └── Platform/                     # Platform-specific code
```

### Consecuencias

**Positivas**:
- Unit testing coverage > 90% achievable
- Business logic completamente independiente de UI
- Facilita implementación de nuevas features
- Onboarding más estructurado para nuevos developers

**Negativas**:
- Overhead inicial en setup y boilerplate
- Posible over-engineering para features simples
- Requiere disciplina del equipo para mantener separación

### Métricas de Éxito
- [ ] Code coverage > 85%
- [ ] Cyclomatic complexity < 10 por método
- [ ] Dependency violations = 0
- [ ] Time para nuevas features consistente

---

## ADR-003: SQLite con Entity Framework Core para Persistencia {#adr-003}

### Estado
✅ **Aceptada** - 15 Septiembre 2024

### Contexto
La aplicación necesita almacenamiento local robusto para progreso del usuario, contenido educativo, y funcionalidad offline completa. Debe ser rápido, confiable y multiplataforma.

### Decisión
Utilizar **SQLite como base de datos** con **Entity Framework Core** como ORM.

### Justificación

**SQLite Ventajas**:
- **Cross-platform**: Funciona idénticamente en iOS y Android
- **File-based**: No requiere servidor, ideal para apps móviles
- **ACID compliance**: Transacciones confiables para progreso de usuario
- **Performance**: Excelente para read-heavy workloads educativos
- **Small footprint**: ~600KB adicionales al app bundle
- **Mature**: 20+ años de desarrollo, extremadamente estable

**Entity Framework Core Ventajas**:
- **Code-First**: Migrations automáticas y versionado de schema
- **LINQ**: Queries type-safe y expresivas
- **Change Tracking**: Optimización automática de updates
- **Lazy Loading**: Carga eficiente de datos relacionados
- **Integration**: Seamless con .NET ecosystem

**Alternativas Evaluadas**:
- **SQLite.NET-PCL**: Más lightweight pero menos features
- **Realm**: Descartado por complejidad de migrations
- **LiteDB**: Descartado por ser menos maduro
- **JSON Files**: Descartado por falta de transaccionalidad

### Modelo de Datos Principal

```csharp
// Domain Entities
public class User : BaseEntity
{
    public string Name { get; set; }
    public int Age { get; set; }
    public string PreferredLanguage { get; set; }
    public bool IsPremiumUser { get; set; }
    public DateTime? PremiumExpiryDate { get; set; }
    
    // Navigation
    public virtual ICollection<UserProgress> Progress { get; set; }
    public virtual ICollection<Achievement> Achievements { get; set; }
}

public class LessonContent : BaseEntity
{
    public string LessonCode { get; set; }
    public string ModuleId { get; set; }
    public string Title { get; set; }
    public TimeSpan EstimatedDuration { get; set; }
    public int DifficultyLevel { get; set; }
    public string InteractiveContentJson { get; set; }
}

public class UserProgress : BaseEntity
{
    public int UserId { get; set; }
    public int LessonId { get; set; }
    public DateTime CompletedAt { get; set; }
    public int ErrorCount { get; set; }
    public TimeSpan CompletionTime { get; set; }
    public int StarRating { get; set; }
    
    // Navigation
    public virtual User User { get; set; }
    public virtual LessonContent Lesson { get; set; }
}
```

### Configuración de Performance

```csharp
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    optionsBuilder
        .UseSqlite(connectionString)
        .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking) // Read performance
        .EnableSensitiveDataLogging(false)  // Security
        .ConfigureWarnings(w => w.Ignore(RelationalEventId.AmbientTransactionWarning));
}

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // Índices para performance
    modelBuilder.Entity<UserProgress>()
        .HasIndex(p => new { p.UserId, p.LessonId })
        .IsUnique();
        
    modelBuilder.Entity<LessonContent>()
        .HasIndex(l => l.ModuleId);
        
    // Configuración de precision para TimeSpan
    modelBuilder.Entity<UserProgress>()
        .Property(p => p.CompletionTime)
        .HasConversion<long>();
}
```

### Estrategia de Migrations

1. **Development**: Code-First migrations con `Add-Migration`
2. **Production**: Migrations embebidas en app bundle
3. **Versioning**: Schema version tracking para upgrade paths
4. **Backup**: Export JSON antes de major migrations

### Consecuencias

**Positivas**:
- Queries type-safe con LINQ
- Migrations automáticas y versionadas
- Excelente tooling para debugging
- Integration natural con Clean Architecture
- Performance predictable y optimizable

**Negativas**:
- App bundle size +2MB (EF Core + SQLite)
- Learning curve para developers sin EF experience
- Potential N+1 query issues si no se usa correctamente

### Métricas de Éxito
- [ ] Query response time < 50ms para 95% queries
- [ ] App startup time impact < 500ms
- [ ] Database file size < 100MB después 1 año uso
- [ ] Zero data loss incidents

---

## ADR-004: Estrategia Offline-First {#adr-004}

### Estado
✅ **Aceptada** - 15 Septiembre 2024

### Contexto
Los usuarios objetivo (familias hispanohablantes) pueden tener conectividad inconsistente. La aplicación debe funcionar completamente offline mientras proporciona una experiencia fluida.

### Decisión
Implementar estrategia **Offline-First** donde toda la funcionalidad core está disponible sin conexión, con sincronización opcional cuando hay conectividad.

### Justificación

**Requerimientos**:
- Uso educativo en áreas con conectividad limitada
- Cero dependencia de internet para funcionalidad core
- Experiencia consistente independiente de conectividad
- Sincronización de progreso cuando sea posible

**Componentes Offline**:
1. **Contenido Educativo**: Pre-bundled en app installation
2. **Progreso Usuario**: SQLite local como source of truth
3. **Audio/Imágenes**: Embedded resources con fallbacks
4. **Configuraciones**: Local preferences storage

**Funcionalidades Online (Opcionales)**:
1. **Analytics**: Batch upload cuando disponible
2. **Premium Validation**: Local validation con server sync
3. **Content Updates**: Future feature para new content

### Arquitectura Offline-First

```csharp
public interface IConnectivityService
{
    bool IsOnline { get; }
    event EventHandler<ConnectivityEventArgs> ConnectivityChanged;
}

public class OfflineFirstDataService : IDataService
{
    private readonly ILocalRepository _localRepo;
    private readonly IRemoteRepository _remoteRepo;
    private readonly IConnectivityService _connectivity;
    
    public async Task<T> GetDataAsync<T>(string key)
    {
        // Always try local first
        var localData = await _localRepo.GetAsync<T>(key);
        if (localData != null)
            return localData;
            
        // Fallback to remote only if online
        if (_connectivity.IsOnline)
        {
            try
            {
                var remoteData = await _remoteRepo.GetAsync<T>(key);
                // Cache locally for future offline use
                await _localRepo.SaveAsync(key, remoteData);
                return remoteData;
            }
            catch (Exception ex)
            {
                // Log but don't fail - continue offline
                _logger.LogWarning($"Remote fetch failed: {ex.Message}");
            }
        }
        
        return default(T);
    }
}
```

### Sincronización de Datos

**Patrón**: Eventually Consistent con Local-First Priority

```csharp
public class ProgressSyncService : IProgressSyncService
{
    public async Task SyncProgressAsync()
    {
        if (!_connectivity.IsOnline)
            return;
            
        try
        {
            var localProgress = await _localRepo.GetPendingSyncAsync();
            var syncBatch = localProgress.Take(50); // Batch processing
            
            foreach (var progress in syncBatch)
            {
                await _remoteRepo.SaveProgressAsync(progress);
                progress.IsSynced = true;
                await _localRepo.UpdateAsync(progress);
            }
        }
        catch (Exception ex)
        {
            // Sync failure doesn't affect local functionality
            _logger.LogError($"Sync failed: {ex.Message}");
        }
    }
}
```

### Asset Management Offline

```csharp
public class OfflineAssetManager : IAssetManager
{
    public async Task<Stream> GetAssetAsync(string assetPath)
    {
        // 1. Check embedded resources (always available)
        var assembly = Assembly.GetExecutingAssembly();
        var resourceStream = assembly.GetManifestResourceStream($"EduPlayKids.Assets.{assetPath}");
        if (resourceStream != null)
            return resourceStream;
            
        // 2. Check local cache
        var cachePath = Path.Combine(_cacheDir, assetPath);
        if (File.Exists(cachePath))
            return File.OpenRead(cachePath);
            
        // 3. Download and cache if online (future feature)
        if (_connectivity.IsOnline)
        {
            var remoteStream = await DownloadAssetAsync(assetPath);
            await CacheAssetAsync(assetPath, remoteStream);
            return remoteStream;
        }
        
        // 4. Fallback to placeholder
        return GetPlaceholderAsset(assetPath);
    }
}
```

### Consecuencias

**Positivas**:
- **Zero dependency** en conectividad para core features
- **Predictable performance** independiente de red
- **Better user experience** en áreas con conectividad pobre
- **Reduced server costs** al minimizar API calls

**Negativas**:
- **Larger app bundle** con todo el contenido embebido
- **Complex sync logic** para features futuras
- **Limited real-time features** (por diseño)
- **Storage management** requerido para cache

### Estrategia de Contenido

**V1.0 (Launch)**:
- Todo el contenido educativo embebido (~30MB)
- Audio files comprimidos (MP3 128kbps)
- Imágenes optimizadas (WebP cuando disponible)

**V2.0 (Future)**:
- Content packs descargables
- Delta updates para nuevo contenido
- User-generated content sync

### Métricas de Éxito
- [ ] 100% funcionalidad core disponible offline
- [ ] App bundle size < 50MB
- [ ] Sync success rate > 95% cuando online
- [ ] Zero crashes por connectivity issues

---

## ADR-005: Modelo de Datos Bilingüe {#adr-005}

### Estado
✅ **Aceptada** - 15 Septiembre 2024

### Contexto
La aplicación debe soportar español e inglés de manera nativa, permitiendo cambio dinámico de idioma sin reiniciar la app, y manteniendo coherencia en el contenido educativo.

### Decisión
Implementar **modelo de datos bilingüe con asset management separado** por idioma y sistema de fallbacks.

### Justificación

**Requerimientos**:
- Cambio de idioma instantáneo sin restart
- Contenido educativo completamente localizado
- Soporte para familias bilingües (niños prefieren inglés, padres español)
- Fallback a inglés cuando contenido en español no disponible

**Alternativas Evaluadas**:
1. **Resx Files (.NET standard)**: Descartado por complejidad con audio/multimedia
2. **JSON localization**: Descartado por falta de structure validation
3. **Database-driven**: Descartado por overhead de queries para UI strings
4. **Asset-based separation**: ✅ **Seleccionado**

### Arquitectura Bilingüe

**Estructura de Directorios**:
```
Assets/
├── Strings/
│   ├── es/
│   │   ├── ui_strings.json
│   │   └── lesson_content.json
│   └── en/
│       ├── ui_strings.json
│       └── lesson_content.json
├── Audio/
│   ├── es/
│   │   ├── MAT001/
│   │   │   ├── intro.mp3
│   │   │   └── instruction.mp3
│   │   └── SPA001/
│   └── en/
│       ├── MAT001/
│       └── SPA001/
└── Images/
    ├── universal/  # Language-agnostic images
    ├── es/         # Text-containing images
    └── en/
```

### Servicio de Localización

```csharp
public interface ILocalizationService
{
    string CurrentLanguage { get; }
    Task SetLanguageAsync(string languageCode);
    string GetString(string key);
    Task<Stream> GetLocalizedAudioAsync(string audioPath);
    Task<Stream> GetLocalizedImageAsync(string imagePath);
    event EventHandler<LanguageChangedEventArgs> LanguageChanged;
}

public class LocalizationService : ILocalizationService
{
    private readonly Dictionary<string, Dictionary<string, string>> _stringResources = new();
    private string _currentLanguage = "es"; // Default para mercado hispanohablante
    
    public async Task SetLanguageAsync(string languageCode)
    {
        if (_currentLanguage == languageCode) return;
        
        await LoadStringResourcesAsync(languageCode);
        _currentLanguage = languageCode;
        
        // Save preference
        await _preferences.SetAsync("PreferredLanguage", languageCode);
        
        // Notify UI for immediate update
        LanguageChanged?.Invoke(this, new LanguageChangedEventArgs(languageCode));
    }
    
    public string GetString(string key)
    {
        // Try current language first
        if (_stringResources.ContainsKey(_currentLanguage) &&
            _stringResources[_currentLanguage].ContainsKey(key))
        {
            return _stringResources[_currentLanguage][key];
        }
        
        // Fallback to English
        if (_stringResources.ContainsKey("en") &&
            _stringResources["en"].ContainsKey(key))
        {
            return _stringResources["en"][key];
        }
        
        // Return key if not found (for debugging)
        return $"[{key}]";
    }
    
    public async Task<Stream> GetLocalizedAudioAsync(string audioPath)
    {
        // Try current language
        var localizedPath = $"Assets.Audio.{_currentLanguage}.{audioPath}";
        var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(localizedPath);
        
        if (stream != null)
            return stream;
            
        // Fallback to English
        var englishPath = $"Assets.Audio.en.{audioPath}";
        return Assembly.GetExecutingAssembly().GetManifestResourceStream(englishPath);
    }
}
```

### Integration con ViewModels

```csharp
public class BaseViewModel : INotifyPropertyChanged
{
    protected readonly ILocalizationService _localization;
    
    public BaseViewModel(ILocalizationService localization)
    {
        _localization = localization;
        _localization.LanguageChanged += OnLanguageChanged;
    }
    
    protected virtual void OnLanguageChanged(object sender, LanguageChangedEventArgs e)
    {
        // Force refresh of all localized properties
        OnPropertyChanged(string.Empty);
    }
    
    protected string L(string key) => _localization.GetString(key);
}

public class MainPageViewModel : BaseViewModel
{
    public string WelcomeText => L("welcome_message");
    public string StartGameText => L("start_game_button");
    public string ParentalControlText => L("parental_control_button");
    
    // Command for language switching
    public ICommand SwitchLanguageCommand => new AsyncRelayCommand<string>(async (lang) =>
    {
        await _localization.SetLanguageAsync(lang);
        // UI will update automatically via PropertyChanged
    });
}
```

### Gestión de Contenido Educativo

```csharp
public class BilingualContentService : IBilingualContentService
{
    public async Task<LessonContent> GetLocalizedLessonAsync(string lessonId, string language)
    {
        // Load lesson metadata (universal)
        var lesson = await _lessonRepository.GetByIdAsync(lessonId);
        if (lesson == null) return null;
        
        // Localize content
        lesson.Title = _localization.GetString($"lesson_{lessonId}_title");
        lesson.Instructions = _localization.GetString($"lesson_{lessonId}_instructions");
        
        // Update audio paths for current language
        lesson.AudioAssets = lesson.AudioAssets.Select(audio => 
            $"Assets/Audio/{language}/{lessonId}/{audio}").ToList();
            
        // Localize interactive content if needed
        if (!string.IsNullOrEmpty(lesson.InteractiveContentJson))
        {
            var content = JsonSerializer.Deserialize<InteractiveContent>(lesson.InteractiveContentJson);
            content = await LocalizeInteractiveContent(content, language);
            lesson.InteractiveContentJson = JsonSerializer.Serialize(content);
        }
        
        return lesson;
    }
}
```

### Persisted Language Preference

```csharp
public class UserPreferencesService : IUserPreferencesService
{
    public async Task<string> GetPreferredLanguageAsync()
    {
        // Check user's explicit preference
        var userPref = await _preferences.GetAsync<string>("PreferredLanguage");
        if (!string.IsNullOrEmpty(userPref))
            return userPref;
            
        // Fallback to system language
        var systemLang = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        return systemLang == "es" ? "es" : "en"; // Default to English for non-Spanish
    }
    
    public async Task SetPreferredLanguageAsync(string language)
    {
        await _preferences.SetAsync("PreferredLanguage", language);
        
        // Update user entity if exists
        var currentUser = await _userService.GetCurrentUserAsync();
        if (currentUser != null)
        {
            currentUser.PreferredLanguage = language;
            await _userService.UpdateUserAsync(currentUser);
        }
    }
}
```

### Consecuencias

**Positivas**:
- **Instant language switching** sin restart
- **Complete localization** incluyendo multimedia
- **Graceful fallbacks** cuando contenido no disponible
- **Family-friendly** para households bilingües
- **Scalable** para adicionar nuevos idiomas en futuro

**Negativas**:
- **Doubled asset size** por duplicación de contenido
- **Complex asset management** requiere careful organization
- **Testing complexity** para validar ambos idiomas
- **Maintenance overhead** para mantener paridad de contenido

### Asset Size Management

**Optimizaciones**:
- Audio MP3 128kbps (balance quality/size)
- Imágenes WebP cuando posible
- Shared assets para contenido universal
- Lazy loading de assets no utilizados

**Estimated Sizes**:
- UI Strings: ~50KB por idioma
- Audio Content: ~15MB por idioma
- Localized Images: ~3MB por idioma
- **Total por idioma**: ~18MB
- **Total bilingüe**: ~36MB

### Métricas de Éxito
- [ ] Language switch time < 1 segundo
- [ ] Asset size < 40MB total para ambos idiomas
- [ ] 100% feature parity entre español e inglés
- [ ] Zero crashes durante language switching

---

## ADR-006: Sistema de Analytics Privacy-Compliant {#adr-006}

### Estado
✅ **Aceptada** - 15 Septiembre 2024

### Contexto
La aplicación necesita analytics para mejora de producto y métricas de engagement, pero debe cumplir COPPA (niños < 13 años) y GDPR-K (GDPR para niños) que requieren protecciones especiales.

### Decisión
Implementar **sistema de analytics completamente anónimo** con opt-out por defecto y sin recolección de PII.

### Justificación

**Compliance Requirements**:
- **COPPA**: No PII de niños < 13 años sin consentimiento parental verificable
- **GDPR-K**: Protecciones especiales para niños < 16 años
- **Privacy-by-Design**: Mínimo datos necesarios, máxima protección

**Alternative Approaches Rejected**:
- **Firebase Analytics**: Rechazado por recolectar device IDs
- **Facebook Analytics**: Rechazado por tracking cross-app
- **Custom Server Analytics**: Rechazado por costos y compliance complexity
- **No Analytics**: Considerado pero rechazado por necesidad de product insights

### Arquitectura Privacy-First

**Principios de Diseño**:
1. **Anonymous-Only**: Cero identificadores persistentes
2. **Local-First**: Procesamiento local, agregación antes de envío
3. **Opt-Out Default**: Analytics deshabilitado por defecto
4. **Parental Control**: Solo padres pueden habilitar
5. **Transparent**: Usuario puede ver exactamente qué datos se envían

```csharp
public class PrivacyCompliantAnalyticsService : IAnalyticsService
{
    private readonly Queue<AnonymousEvent> _eventQueue = new(maxSize: 100);
    private readonly Timer _batchTimer;
    private bool _isAnalyticsEnabled = false;
    
    public async Task<bool> RequestAnalyticsConsentAsync()
    {
        // Only parents can consent (PIN required)
        var hasParentalConsent = await _parentalService.RequestConsentAsync(
            title: "Analytics Opt-In",
            message: "Would you like to help improve EduPlayKids by sharing anonymous usage data? No personal information is collected.",
            consentType: ConsentType.Analytics);
            
        _isAnalyticsEnabled = hasParentalConsent;
        await _preferences.SetAsync("AnalyticsEnabled", hasParentalConsent);
        
        return hasParentalConsent;
    }
    
    public async Task TrackEventAsync(string eventName, Dictionary<string, object> parameters = null)
    {
        if (!_isAnalyticsEnabled) return;
        
        var anonymousEvent = new AnonymousEvent
        {
            EventName = eventName,
            Timestamp = DateTime.UtcNow,
            SessionId = GetAnonymousSessionId(), // Rotates every app launch
            Parameters = AnonymizeParameters(parameters ?? new())
        };
        
        _eventQueue.Enqueue(anonymousEvent);
        
        // Batch send when queue full or timer expires
        if (_eventQueue.Count >= 50)
            await FlushEventsAsync();
    }
}
```

### Anonymous Data Model

```csharp
public class AnonymousEvent
{
    public string EventName { get; set; }
    public DateTime Timestamp { get; set; }
    public string SessionId { get; set; } // Ephemeral, changes each launch
    public Dictionary<string, object> Parameters { get; set; }
    
    // NO user identifiers
    // NO device identifiers  
    // NO location data
    // NO personal information
}

// Allowed anonymous parameters only
public static class AllowedParameters
{
    public static readonly HashSet<string> Whitelist = new()
    {
        // Educational metrics
        "lesson_module",        // "math", "reading", etc.
        "lesson_difficulty",    // 1-5
        "completion_time_sec",  // Performance metric
        "error_count",          // Educational effectiveness
        "star_rating",          // 1-3
        
        // Usage patterns
        "session_duration_sec", // App engagement
        "lessons_per_session",  // Usage intensity
        "language_used",        // "es" or "en"
        
        // Technical metrics
        "app_version",          // For debugging
        "os_version",           // Compatibility
        "device_type",          // "phone" or "tablet"
        "crash_type"            // Error tracking
    };
}

private Dictionary<string, object> AnonymizeParameters(Dictionary<string, object> parameters)
{
    var anonymized = new Dictionary<string, object>();
    
    foreach (var param in parameters)
    {
        if (AllowedParameters.Whitelist.Contains(param.Key))
        {
            // Additional anonymization for sensitive values
            var value = param.Value;
            
            if (param.Key == "completion_time_sec")
            {
                // Round to nearest 10 seconds to prevent timing-based identification
                value = ((int)value / 10) * 10;
            }
            else if (param.Key == "session_duration_sec")
            {
                // Round to nearest minute
                value = ((int)value / 60) * 60;
            }
            
            anonymized[param.Key] = value;
        }
    }
    
    return anonymized;
}
```

### Local Aggregation Before Send

```csharp
public class AnalyticsAggregator
{
    public async Task<List<AggregatedMetric>> AggregateEventsAsync(List<AnonymousEvent> events)
    {
        var metrics = new List<AggregatedMetric>();
        
        // Aggregate lesson completion metrics
        var lessonEvents = events.Where(e => e.EventName == "lesson_completed").ToList();
        if (lessonEvents.Any())
        {
            metrics.Add(new AggregatedMetric
            {
                MetricName = "lesson_completion_rate",
                Count = lessonEvents.Count,
                AverageValue = lessonEvents.Average(e => (int)e.Parameters["completion_time_sec"]),
                AggregationPeriod = TimeSpan.FromDays(1)
            });
        }
        
        // Aggregate error patterns (no user identification)
        var errorEvents = events.Where(e => e.EventName == "lesson_error").ToList();
        if (errorEvents.Any())
        {
            var errorsByModule = errorEvents
                .GroupBy(e => e.Parameters["lesson_module"])
                .Select(g => new AggregatedMetric
                {
                    MetricName = $"error_rate_{g.Key}",
                    Count = g.Count(),
                    Dimension = g.Key.ToString()
                });
                
            metrics.AddRange(errorsByModule);
        }
        
        return metrics;
    }
}
```

### Parental Transparency Dashboard

```csharp
public class AnalyticsTransparencyService
{
    public async Task<AnalyticsReport> GenerateParentalReportAsync()
    {
        var events = await _localAnalytics.GetRecentEventsAsync(TimeSpan.FromDays(7));
        
        return new AnalyticsReport
        {
            IsAnalyticsEnabled = await _preferences.GetAsync<bool>("AnalyticsEnabled"),
            EventsCollectedLastWeek = events.Count,
            DataTypesCollected = GetDataTypesCollected(events),
            LastSentDate = await _preferences.GetAsync<DateTime?>("LastAnalyticsSend"),
            
            // Sample of exactly what gets sent
            SampleData = events.Take(3).Select(e => new
            {
                e.EventName,
                e.Timestamp,
                Parameters = e.Parameters
            }).ToList()
        };
    }
    
    public async Task DisableAnalyticsAsync()
    {
        _isAnalyticsEnabled = false;
        await _preferences.SetAsync("AnalyticsEnabled", false);
        
        // Clear any pending events
        _eventQueue.Clear();
        
        // Optionally delete historical local data
        await _localAnalytics.ClearAllDataAsync();
    }
}
```

### Data Retention and Purging

```csharp
public class AnalyticsDataManager
{
    public async Task PurgeOldDataAsync()
    {
        // Local data retention: 30 days maximum
        var cutoffDate = DateTime.UtcNow.AddDays(-30);
        await _localDb.AnonymousEvents
            .Where(e => e.Timestamp < cutoffDate)
            .DeleteAsync();
            
        // Clear sent events immediately
        await _localDb.AnonymousEvents
            .Where(e => e.WasSent)
            .DeleteAsync();
    }
}
```

### Consecuencias

**Positivas**:
- **Legal Compliance**: Cumple COPPA y GDPR-K completamente
- **Parent Trust**: Transparencia total sobre datos recolectados
- **User Privacy**: Cero riesgo de identificación de usuarios
- **Product Insights**: Suficientes datos para mejora de producto
- **Cost Effective**: No necesita infraestructura compliance compleja

**Negativas**:
- **Limited Analytics**: No user journey tracking o cohort analysis
- **No A/B Testing**: Sin identificadores no se pueden hacer tests personalizados
- **Reduced Insights**: Métricas agregadas vs comportamiento individual
- **Implementation Complexity**: Sistema custom vs analytics platforms

### Analytics Metrics Disponibles

**Educational Effectiveness**:
- Completion rates por módulo
- Error patterns por tipo de lección
- Tiempo promedio de completado
- Distribution de star ratings

**App Usage Patterns**:
- Session duration patterns
- Popular modules/lessons
- Language preference distribution
- Device type usage

**Technical Performance**:
- Crash rates por OS version
- App version adoption
- Performance metrics aggregated

### Métricas de Éxito
- [ ] Zero PII collection verified por audit
- [ ] Parental opt-in rate > 15%
- [ ] Analytics insights útiles para product decisions
- [ ] Zero compliance violations

---

## ADR-007: Modelo Freemium con Limitaciones Locales {#adr-007}

### Estado
✅ **Aceptada** - 15 Septiembre 2024

### Contexto
EduPlayKids necesita un modelo de monetización sostenible que permita acceso inicial gratuito para familias hispanohablantes con diferentes niveles socioeconómicos, mientras genera ingresos para desarrollo continuo.

### Decisión
Implementar **modelo freemium con trial de 3 días** seguido de **limitación a 10 lecciones por día** para usuarios no premium, con validación completamente local.

### Justificación

**Business Model Rationale**:
- **Market Research**: Familias hispanas priorizan educación pero son price-sensitive
- **Trial Period**: 3 días permite evaluar valor antes de pagar
- **Daily Limits**: 10 lecciones ≈ 30-45 minutos de uso educativo apropiado
- **Price Point**: $4.99 USD accesible vs competidores ($9.99-15.99)
- **Local Validation**: No server requerido, funciona offline

**Alternatives Considered**:
- **Ads-Based**: Rechazado por COPPA compliance complexity
- **Subscription**: Rechazado por preferencia de pago único en target market
- **Feature Restrictions**: Rechazado por impacto negativo en experiencia educativa
- **Time-Based**: Rechazado por ser menos flexible que lesson-based

### Implementación Freemium

```csharp
public interface IFreemiumManager
{
    Task<FreemiumStatus> GetUserStatusAsync(int userId);
    Task<bool> CanAccessLessonAsync(int userId);
    Task IncrementDailyLessonCountAsync(int userId);
    Task<PurchaseResult> ProcessPremiumPurchaseAsync(string purchaseToken);
}

public class FreemiumManager : IFreemiumManager
{
    private readonly IUserRepository _userRepository;
    private readonly IPreferencesService _preferences;
    private readonly IDateTimeService _dateTime;
    
    public async Task<FreemiumStatus> GetUserStatusAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        var installDate = await _preferences.GetAsync<DateTime>("InstallDate", DateTime.UtcNow);
        var today = _dateTime.Today;
        
        // Check premium status first
        if (user.IsPremiumUser && user.PremiumExpiryDate > today)
        {
            return new FreemiumStatus
            {
                StatusType = FreemiumStatusType.Premium,
                DailyLessonsRemaining = int.MaxValue,
                Message = "Premium User - Unlimited Access"
            };
        }
        
        // Check trial period
        var daysSinceInstall = (today - installDate).Days;
        if (daysSinceInstall < 3)
        {
            return new FreemiumStatus
            {
                StatusType = FreemiumStatusType.Trial,
                TrialDaysRemaining = 3 - daysSinceInstall,
                DailyLessonsRemaining = int.MaxValue,
                Message = $"Free Trial - {3 - daysSinceInstall} days remaining"
            };
        }
        
        // Free user with daily limits
        var todayLessonsCount = await GetTodayLessonCountAsync(userId);
        var lessonsRemaining = Math.Max(0, 10 - todayLessonsCount);
        
        return new FreemiumStatus
        {
            StatusType = FreemiumStatusType.Free,
            DailyLessonsRemaining = lessonsRemaining,
            Message = lessonsRemaining > 0 
                ? $"{lessonsRemaining} lessons remaining today"
                : "Daily limit reached. Upgrade for unlimited access!"
        };
    }
    
    public async Task<bool> CanAccessLessonAsync(int userId)
    {
        var status = await GetUserStatusAsync(userId);
        
        return status.StatusType switch
        {
            FreemiumStatusType.Premium => true,
            FreemiumStatusType.Trial => true,
            FreemiumStatusType.Free => status.DailyLessonsRemaining > 0,
            _ => false
        };
    }
    
    private async Task<int> GetTodayLessonCountAsync(int userId)
    {
        var today = _dateTime.Today;
        var tomorrow = today.AddDays(1);
        
        return await _userProgressRepository.CountAsync(p => 
            p.UserId == userId && 
            p.CompletedAt >= today && 
            p.CompletedAt < tomorrow);
    }
}
```

### Purchase Integration

```csharp
public class InAppPurchaseService : IInAppPurchaseService
{
    public async Task<PurchaseResult> PurchasePremiumAsync()
    {
        try
        {
            // Using Microsoft.Maui.Essentials or Plugin.InAppBilling
            var purchaseResult = await CrossInAppBilling.Current.PurchaseAsync(
                productId: "eduplaykids_premium",
                itemType: ItemType.InAppPurchase);
                
            if (purchaseResult?.State == PurchaseState.Purchased)
            {
                // Validate purchase locally (no server validation needed for v1)
                var isValid = await ValidatePurchaseAsync(purchaseResult);
                
                if (isValid)
                {
                    await UpgradeUserToPremiumAsync(purchaseResult);
                    return new PurchaseResult { Success = true, TransactionId = purchaseResult.Id };
                }
            }
            
            return new PurchaseResult { Success = false, Error = "Purchase validation failed" };
        }
        catch (InAppBillingPurchaseException ex)
        {
            return new PurchaseResult { Success = false, Error = ex.Message };
        }
    }
    
    private async Task UpgradeUserToPremiumAsync(InAppBillingPurchase purchase)
    {
        var currentUser = await _userService.GetCurrentUserAsync();
        
        currentUser.IsPremiumUser = true;
        currentUser.PremiumExpiryDate = DateTime.UtcNow.AddYears(10); // Lifetime purchase
        currentUser.PurchaseTransactionId = purchase.Id;
        currentUser.PurchaseDate = DateTime.UtcNow;
        
        await _userService.UpdateUserAsync(currentUser);
        
        // Clear any daily usage tracking
        await ResetDailyLimitsAsync(currentUser.Id);
        
        // Track successful purchase (anonymous)
        await _analytics.TrackEventAsync("premium_purchase_completed", new Dictionary<string, object>
        {
            { "purchase_amount", 4.99 },
            { "trial_days_used", (DateTime.UtcNow - currentUser.CreatedAt).Days }
        });
    }
}
```

### UI/UX para Freemium

```csharp
public class LessonAccessViewModel : BaseViewModel
{
    public FreemiumStatus Status { get; set; }
    public bool ShowUpgradePrompt { get; set; }
    public string StatusMessage { get; set; }
    
    public async Task CheckLessonAccessAsync()
    {
        Status = await _freemiumManager.GetUserStatusAsync(_currentUser.Id);
        StatusMessage = Status.Message;
        
        if (Status.StatusType == FreemiumStatusType.Free && Status.DailyLessonsRemaining == 0)
        {
            ShowUpgradePrompt = true;
        }
    }
    
    public ICommand UpgradeToPremiumCommand => new AsyncRelayCommand(async () =>
    {
        var result = await _purchaseService.PurchasePremiumAsync();
        
        if (result.Success)
        {
            await ShowSuccessMessageAsync("Welcome to EduPlayKids Premium! 🎉");
            await CheckLessonAccessAsync(); // Refresh status
        }
        else
        {
            await ShowErrorMessageAsync("Purchase failed. Please try again.");
        }
    });
}
```

### Gentle Upgrade Prompts

```csharp
public class UpgradePromptService
{
    public async Task<bool> ShouldShowUpgradePromptAsync(int userId)
    {
        var status = await _freemiumManager.GetUserStatusAsync(userId);
        
        // Show prompt when user hits daily limit
        if (status.StatusType == FreemiumStatusType.Free && 
            status.DailyLessonsRemaining == 0)
        {
            var lastPrompt = await _preferences.GetAsync<DateTime?>("LastUpgradePrompt");
            var now = DateTime.UtcNow;
            
            // Don't spam - maximum once per day
            if (lastPrompt == null || (now - lastPrompt.Value).Hours >= 24)
            {
                await _preferences.SetAsync("LastUpgradePrompt", now);
                return true;
            }
        }
        
        return false;
    }
    
    public async Task ShowUpgradePromptAsync()
    {
        var options = new[]
        {
            "Upgrade Now ($4.99)",
            "Try Tomorrow (Free)",
            "Learn More"
        };
        
        var result = await _dialogService.ShowActionSheetAsync(
            title: "Daily Lesson Limit Reached",
            message: "Your child has completed 10 lessons today! 🌟\n\nUpgrade to Premium for unlimited daily lessons and support continued learning.",
            options: options);
            
        switch (result)
        {
            case "Upgrade Now ($4.99)":
                await _purchaseService.PurchasePremiumAsync();
                break;
            case "Learn More":
                await _navigationService.NavigateToAsync("PremiumBenefitsPage");
                break;
            // "Try Tomorrow" just dismisses
        }
    }
}
```

### Premium Benefits Page

```csharp
public class PremiumBenefitsViewModel : BaseViewModel
{
    public List<PremiumBenefit> Benefits { get; } = new()
    {
        new("🚀", "Unlimited Daily Lessons", "No daily limits - learn as much as you want!"),
        new("📚", "All Future Content", "Get new lessons and modules as they're released"),
        new("🎯", "Advanced Progress Tracking", "Detailed reports on your child's learning"),
        new("❤️", "Support Education", "Help us create more quality educational content"),
        new("🔒", "One-Time Purchase", "No subscriptions - pay once, use forever"),
        new("👨‍👩‍👧‍👦", "Family Friendly", "Safe, ad-free learning environment")
    };
    
    public string PriceText => "$4.99 USD - One-time purchase";
    public string ComparisonText => "Compare: Other educational apps charge $9.99-15.99/month";
}

public record PremiumBenefit(string Icon, string Title, string Description);
```

### Data Tracking para Freemium

```csharp
public class FreemiumAnalytics
{
    public async Task TrackTrialProgressAsync(int userId)
    {
        var status = await _freemiumManager.GetUserStatusAsync(userId);
        
        if (status.StatusType == FreemiumStatusType.Trial)
        {
            await _analytics.TrackEventAsync("trial_day_active", new Dictionary<string, object>
            {
                { "trial_days_remaining", status.TrialDaysRemaining },
                { "lessons_completed_in_trial", await GetTrialLessonsCompletedAsync(userId) }
            });
        }
    }
    
    public async Task TrackDailyLimitReachedAsync(int userId)
    {
        await _analytics.TrackEventAsync("daily_limit_reached", new Dictionary<string, object>
        {
            { "user_type", "free" },
            { "days_since_install", (DateTime.UtcNow - await GetInstallDateAsync()).Days }
        });
    }
    
    public async Task TrackUpgradePromptShownAsync()
    {
        await _analytics.TrackEventAsync("upgrade_prompt_shown", new Dictionary<string, object>
        {
            { "prompt_trigger", "daily_limit_reached" }
        });
    }
}
```

### Consecuencias

**Positivas**:
- **Accessible Entry Point**: 3 días permiten evaluar valor
- **Educational Appropriate**: 10 lecciones ≈ tiempo de uso saludable
- **Offline Compatible**: No server validation requerida
- **Simple Pricing**: Una sola compra vs subscriptions complejas
- **Family Budget Friendly**: $4.99 accesible para target market

**Negativas**:
- **Limited Revenue**: Depende de conversion rate vs recurring revenue
- **No Server Validation**: Potencial para piracy (acceptable risk para v1)
- **Support Complexity**: Manejar refunds y purchase issues
- **Market Education**: Usuarios deben entender value proposition

### Projected Metrics

**Conversion Funnel**:
- Install → Trial: 100%
- Trial → End of 3 days: 60%
- End of Trial → Purchase: 10-15%
- **Overall Conversion**: 6-9%

**Revenue Projections** (conservative):
- 10,000 monthly downloads
- 600 trial completions
- 60-90 purchases
- **Monthly Revenue**: $300-450

### Métricas de Éxito
- [ ] Trial completion rate > 50%
- [ ] Trial-to-paid conversion > 10%
- [ ] Average daily lessons during trial > 15
- [ ] Premium user satisfaction > 4.5/5

---

## ADR-008: GitHub Actions para CI/CD {#adr-008}

### Estado
✅ **Aceptada** - 15 Septiembre 2024

### Contexto
EduPlayKids necesita un pipeline de CI/CD automatizado para asegurar calidad de código, testing automatizado, y deployment a App Store/Play Store de manera eficiente y confiable.

### Decisión
Implementar **GitHub Actions** como plataforma principal de CI/CD con workflows separados para testing, building, y deployment.

### Justificación

**GitHub Actions Advantages**:
- **Native Integration**: Integración perfecta con repositorio GitHub
- **Cost Effective**: 2000 minutos gratis + reasonably priced
- **Pre-built Actions**: Ecosystem rico para .NET MAUI, mobile deployment
- **Parallel Execution**: Multiple jobs simultáneos para efficiency
- **Secrets Management**: Secure storage para certificates y API keys

**Alternatives Considered**:
- **Azure DevOps**: Más potente pero más complejo para team pequeño
- **Jenkins**: Rechazado por overhead de self-hosting
- **CircleCI**: Buena opción pero preferencia por GitHub integration
- **Bitrise**: Especializado en mobile pero más costoso

### Architecture Overview

```yaml
# .github/workflows/main.yml
name: EduPlayKids CI/CD

on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main ]
  release:
    types: [ published ]

env:
  DOTNET_VERSION: '8.0.x'
  MAUI_VERSION: '8.0.0'

jobs:
  # Stage 1: Code Quality & Testing
  test:
    name: "Tests & Code Quality"
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: ${{ env.DOTNET_VERSION }}
          
      - name: Install MAUI Workload
        run: dotnet workload install maui
        
      - name: Restore Dependencies
        run: dotnet restore
        
      - name: Run Unit Tests
        run: dotnet test --configuration Release --collect:"XPlat Code Coverage"
        
      - name: Code Coverage Report
        uses: codecov/codecov-action@v3
        
      - name: Static Code Analysis
        run: dotnet format --verify-no-changes
        
  # Stage 2: Build Android
  build-android:
    name: "Build Android"
    runs-on: ubuntu-latest
    needs: test
    if: github.event_name == 'push' || github.event_name == 'release'
    
    steps:
      - uses: actions/checkout@v4
      
      - name: Setup .NET & MAUI
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: ${{ env.DOTNET_VERSION }}
          
      - name: Install MAUI Workload
        run: dotnet workload install maui
        
      - name: Restore Dependencies
        run: dotnet restore
        
      - name: Build Android
        run: |
          dotnet publish -c Release -f net8.0-android \
            /p:AndroidSigningKeyStore=${{ secrets.ANDROID_KEYSTORE_PATH }} \
            /p:AndroidSigningStorePass=${{ secrets.ANDROID_KEYSTORE_PASSWORD }} \
            /p:AndroidSigningKeyAlias=${{ secrets.ANDROID_KEY_ALIAS }} \
            /p:AndroidSigningKeyPass=${{ secrets.ANDROID_KEY_PASSWORD }}
            
      - name: Upload Android Artifact
        uses: actions/upload-artifact@v3
        with:
          name: android-app
          path: "**/*.aab"
          
  # Stage 3: Build iOS
  build-ios:
    name: "Build iOS"
    runs-on: macos-latest
    needs: test
    if: github.event_name == 'push' || github.event_name == 'release'
    
    steps:
      - uses: actions/checkout@v4
      
      - name: Setup .NET & MAUI
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: ${{ env.DOTNET_VERSION }}
          
      - name: Install MAUI Workload
        run: dotnet workload install maui
        
      - name: Import Code Signing Certificate
        uses: apple-actions/import-codesign-certs@v1
        with:
          p12-file-base64: ${{ secrets.IOS_CERTIFICATE_P12 }}
          p12-password: ${{ secrets.IOS_CERTIFICATE_PASSWORD }}
          
      - name: Install Provisioning Profile
        uses: apple-actions/download-provisioning-profiles@v1
        with:
          bundle-id: com.eduplaykids.app
          profile-type: IOS_APP_STORE
          issuer-id: ${{ secrets.APPSTORE_ISSUER_ID }}
          api-key-id: ${{ secrets.APPSTORE_KEY_ID }}
          api-private-key: ${{ secrets.APPSTORE_PRIVATE_KEY }}
          
      - name: Build iOS
        run: |
          dotnet publish -c Release -f net8.0-ios \
            /p:RuntimeIdentifier=ios-arm64 \
            /p:CodesignKey="${{ secrets.IOS_CODESIGN_KEY }}" \
            /p:CodesignProvision="${{ secrets.IOS_PROVISION_NAME }}"
            
      - name: Upload iOS Artifact
        uses: actions/upload-artifact@v3
        with:
          name: ios-app
          path: "**/*.ipa"
```

### Deployment Workflows

```yaml
# .github/workflows/deploy-android.yml
name: Deploy to Google Play Store

on:
  release:
    types: [ published ]

jobs:
  deploy-android:
    name: "Deploy Android to Play Store"
    runs-on: ubuntu-latest
    needs: build-android
    
    steps:
      - name: Download Android Artifact
        uses: actions/download-artifact@v3
        with:
          name: android-app
          
      - name: Upload to Play Store (Internal Track)
        uses: r0adkll/upload-google-play@v1
        with:
          serviceAccountJsonPlainText: ${{ secrets.GOOGLE_PLAY_SERVICE_ACCOUNT }}
          packageName: com.eduplaykids.app
          releaseFiles: "**/*.aab"
          track: internal
          status: completed
          
      - name: Promote to Production (if main branch)
        if: github.ref == 'refs/heads/main'
        uses: r0adkll/upload-google-play@v1
        with:
          serviceAccountJsonPlainText: ${{ secrets.GOOGLE_PLAY_SERVICE_ACCOUNT }}
          packageName: com.eduplaykids.app
          track: production
          inAppUpdatePriority: 2
```

```yaml
# .github/workflows/deploy-ios.yml  
name: Deploy to App Store

on:
  release:
    types: [ published ]

jobs:
  deploy-ios:
    name: "Deploy iOS to App Store"
    runs-on: macos-latest
    needs: build-ios
    
    steps:
      - name: Download iOS Artifact
        uses: actions/download-artifact@v3
        with:
          name: ios-app
          
      - name: Upload to App Store Connect
        uses: apple-actions/upload-testflight@v1
        with:
          app-path: "**/*.ipa"
          issuer-id: ${{ secrets.APPSTORE_ISSUER_ID }}
          api-key-id: ${{ secrets.APPSTORE_KEY_ID }}
          api-private-key: ${{ secrets.APPSTORE_PRIVATE_KEY }}
          
      - name: Submit for Review (Production)
        if: github.ref == 'refs/heads/main' && contains(github.event.release.tag_name, 'stable')
        run: |
          # Use App Store Connect API to submit for review
          # Custom script using xcrun altool or App Store Connect API
```

### Quality Gates & Testing

```yaml
# .github/workflows/pull-request.yml
name: Pull Request Quality Check

on:
  pull_request:
    branches: [ main, develop ]

jobs:
  quality-check:
    name: "PR Quality Gate"
    runs-on: ubuntu-latest
    
    steps:
      - uses: actions/checkout@v4
        with:
          fetch-depth: 0  # Para SonarQube analysis
          
      - name: Setup .NET & MAUI
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: ${{ env.DOTNET_VERSION }}
          
      - name: Restore & Build
        run: |
          dotnet workload install maui
          dotnet restore
          dotnet build --configuration Release
          
      - name: Run Unit Tests with Coverage
        run: |
          dotnet test --configuration Release \
            --collect:"XPlat Code Coverage" \
            --results-directory:"./coverage"
            
      - name: SonarQube Analysis
        uses: sonarqube-quality-gate-action@master
        env:
          GITHUB_TOKEN: ${{ secrets.GITHUB_TOKEN }}
          SONAR_TOKEN: ${{ secrets.SONAR_TOKEN }}
          
      - name: Security Scan
        uses: securecodewarrior/github-action-add-sarif@v1
        with:
          sarif-file: 'security-scan-results.sarif'
          
      - name: Performance Tests
        run: |
          # Custom performance benchmarks
          dotnet run --project EduPlayKids.Performance.Tests
          
      - name: Update PR with Results
        uses: actions/github-script@v6
        with:
          script: |
            // Post test results as PR comment
            const fs = require('fs');
            const testResults = fs.readFileSync('test-results.xml', 'utf8');
            // Parse and comment on PR
```

### Environment-Specific Deployments

```yaml
# .github/workflows/deploy-staging.yml
name: Deploy to Staging

on:
  push:
    branches: [ develop ]

env:
  ENVIRONMENT: staging
  APP_SUFFIX: .staging

jobs:
  deploy-staging:
    name: "Deploy to Staging Environment"
    runs-on: ubuntu-latest
    
    steps:
      - uses: actions/checkout@v4
      
      - name: Setup Environment Variables
        run: |
          echo "APP_NAME=EduPlayKids Staging" >> $GITHUB_ENV
          echo "PACKAGE_NAME=com.eduplaykids.app.staging" >> $GITHUB_ENV
          echo "APP_VERSION=${GITHUB_RUN_NUMBER}" >> $GITHUB_ENV
          
      - name: Build with Staging Config
        run: |
          dotnet publish -c Release -f net8.0-android \
            /p:ApplicationTitle="${{ env.APP_NAME }}" \
            /p:ApplicationId="${{ env.PACKAGE_NAME }}" \
            /p:ApplicationVersion="${{ env.APP_VERSION }}"
            
      - name: Deploy to Firebase App Distribution
        uses: wzieba/Firebase-Distribution-Github-Action@v1
        with:
          appId: ${{ secrets.FIREBASE_APP_ID_ANDROID_STAGING }}
          serviceCredentialsFileContent: ${{ secrets.FIREBASE_SERVICE_ACCOUNT }}
          groups: testers,internal-qa
          file: "**/*.apk"
          releaseNotes: |
            Staging Build - Branch: ${{ github.ref_name }}
            Commit: ${{ github.sha }}
            Changes: ${{ github.event.head_commit.message }}
```

### Secrets Management

**Required GitHub Secrets**:

```bash
# Android Signing
ANDROID_KEYSTORE_PATH          # Base64 encoded keystore
ANDROID_KEYSTORE_PASSWORD      # Keystore password
ANDROID_KEY_ALIAS              # Key alias name
ANDROID_KEY_PASSWORD           # Key password

# iOS Signing
IOS_CERTIFICATE_P12            # Base64 encoded P12 certificate
IOS_CERTIFICATE_PASSWORD       # Certificate password
IOS_PROVISION_NAME             # Provisioning profile name
IOS_CODESIGN_KEY              # Code signing identity

# App Store Connect
APPSTORE_ISSUER_ID            # App Store Connect API issuer ID
APPSTORE_KEY_ID               # API key ID
APPSTORE_PRIVATE_KEY          # Private key for API

# Google Play Console
GOOGLE_PLAY_SERVICE_ACCOUNT    # Service account JSON

# Quality & Analytics
SONAR_TOKEN                   # SonarQube token
CODECOV_TOKEN                # Code coverage token

# Firebase (for staging)
FIREBASE_APP_ID_ANDROID_STAGING
FIREBASE_SERVICE_ACCOUNT
```

### Monitoring & Notifications

```yaml
# .github/workflows/notify-deployment.yml
name: Deployment Notifications

on:
  workflow_run:
    workflows: ["Deploy to Google Play Store", "Deploy to App Store"]
    types: [completed]

jobs:
  notify:
    runs-on: ubuntu-latest
    
    steps:
      - name: Notify Slack on Success
        if: github.event.workflow_run.conclusion == 'success'
        uses: rtCamp/action-slack-notify@v2
        env:
          SLACK_WEBHOOK: ${{ secrets.SLACK_WEBHOOK }}
          SLACK_MESSAGE: |
            🎉 EduPlayKids deployed successfully!
            Version: ${{ github.event.workflow_run.head_branch }}
            Stores: App Store & Google Play
            
      - name: Notify on Failure
        if: github.event.workflow_run.conclusion == 'failure'
        uses: rtCamp/action-slack-notify@v2
        env:
          SLACK_WEBHOOK: ${{ secrets.SLACK_WEBHOOK }}
          SLACK_COLOR: ${{ job.status }}
          SLACK_MESSAGE: |
            ❌ EduPlayKids deployment failed!
            Branch: ${{ github.event.workflow_run.head_branch }}
            Check: ${{ github.event.workflow_run.html_url }}
```

### Performance & Cost Optimization

**Caching Strategy**:
```yaml
      - name: Cache Dependencies
        uses: actions/cache@v3
        with:
          path: ~/.nuget/packages
          key: ${{ runner.os }}-nuget-${{ hashFiles('**/*.csproj') }}
          
      - name: Cache MAUI Workload
        uses: actions/cache@v3
        with:
          path: ~/.dotnet/packs
          key: ${{ runner.os }}-maui-${{ env.MAUI_VERSION }}
```

**Matrix Builds** (when needed):
```yaml
strategy:
  matrix:
    os: [ubuntu-latest, macos-latest]
    dotnet-version: ['8.0.x']
  fail-fast: false
```

### Consecuencias

**Positivas**:
- **Automated Quality**: Tests, linting, security scans automáticos
- **Fast Feedback**: PR validation en ~10-15 minutos
- **Reliable Deployments**: Consistent builds independientes de developer machine
- **Cost Effective**: ~$50-100/month vs alternatives más costosas
- **Transparent**: Todo el team puede ver build status y deployment progress

**Negativas**:
- **GitHub Lock-in**: Dependencia de GitHub ecosystem
- **Learning Curve**: Team debe aprender YAML workflow syntax
- **macOS Costs**: iOS builds requieren macOS runners (más costosos)
- **Secrets Management**: Cuidadosa gestión de certificates y keys

### Workflow Execution Times

**Typical Execution Times**:
- Unit Tests: 3-5 minutos
- Android Build: 8-12 minutos
- iOS Build: 15-20 minutos (incluye signing)
- Deployment: 5-10 minutos adicionales
- **Total Pipeline**: 30-45 minutos

**Monthly Usage** (estimate):
- ~100 pushes/PRs × 15 min = 1,500 minutos
- ~20 releases × 45 min = 900 minutos
- **Total**: ~2,400 minutos (~$24/month)

### Métricas de Éxito
- [ ] Build success rate > 95%
- [ ] Average build time < 15 minutos
- [ ] Deployment success rate > 98%
- [ ] Time to production < 2 horas (post-merge)

---

## Resumen Ejecutivo de Decisiones

### Decisiones Críticas de Alto Impacto

1. **ADR-001 (.NET MAUI)**: Permite desarrollo cross-platform eficiente
2. **ADR-002 (Clean Architecture + MVVM)**: Asegura mantenibilidad y testabilidad
3. **ADR-003 (SQLite + EF Core)**: Garantiza persistencia robusta offline-first
4. **ADR-004 (Offline-First)**: Critical para target market con conectividad variable

### Decisiones de Compliance y Business

5. **ADR-005 (Bilingüe)**: Diferenciador clave para mercado hispanohablante
6. **ADR-006 (Privacy Analytics)**: Compliance legal y trust parental
7. **ADR-007 (Freemium Local)**: Modelo de monetización accesible y offline
8. **ADR-008 (GitHub Actions)**: Automatización y quality assurance

### Risk Mitigation

**Technical Risks Addressed**:
- Dependencia de .NET MAUI → Plan de migración a alternatives si necesario
- SQLite limitations → Schema versioning y migration strategy
- iOS/Android fragmentation → Extensive testing matrix

**Business Risks Addressed**:
- Legal compliance → Privacy-first design desde inicio
- Market penetration → Freemium accessible + trial period
- Competition → Offline-first + bilingual differentiators

### Next Phase: Implementation

Con estas decisiones arquitectónicas establecidas, el equipo puede proceder con confidence hacia la implementación, sabiendo que:

1. **Technical Foundation** está sólida y escalable
2. **Business Model** está validado y aligned con market research
3. **Legal Compliance** está built-in desde el diseño
4. **Development Process** está automatizado y quality-assured

---

**Documento ADR - EduPlayKids v1.0**  
**Fecha**: 16 de Septiembre, 2024  
**Estado**: Aprobado para Implementation Phase  
**Próxima Revisión**: Post-MVP Launch (Q1 2025)