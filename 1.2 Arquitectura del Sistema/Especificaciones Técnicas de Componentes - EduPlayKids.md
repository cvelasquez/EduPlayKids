# Especificaciones Técnicas de Componentes - EduPlayKids

## 1. Resumen Ejecutivo

Este documento define las especificaciones técnicas detalladas de los componentes principales de EduPlayKids, basándose en la arquitectura Clean Architecture + MVVM definida en el Documento de Arquitectura de Software.

## 2. Componentes de la Capa de Presentación

### 2.1 MainPage (Página Principal)

**Responsabilidad**: Pantalla principal con navegación por módulos educativos

**Especificaciones Técnicas**:
- **Framework**: .NET MAUI ContentPage
- **ViewModel**: `MainPageViewModel` con implementación de `INotifyPropertyChanged`
- **Estados**: Loading, Loaded, Error
- **Performance**: Inicialización < 2 segundos
- **Memoria**: Máximo 10MB en RAM

```csharp
public class MainPageViewModel : BaseViewModel
{
    public ObservableCollection<EducationalModule> Modules { get; set; }
    public ICommand NavigateToModuleCommand { get; }
    public ICommand SwitchLanguageCommand { get; }
    
    // Progress tracking
    public int CompletedLessons { get; set; }
    public int TotalLessons { get; set; }
    public double ProgressPercentage => (double)CompletedLessons / TotalLessons * 100;
}
```

**Dependencias**:
- `IEducationalModuleService` (Application Layer)
- `IUserProgressService` (Application Layer)
- `ILocalizationService` (Application Layer)

### 2.2 LessonPage (Página de Lección)

**Responsabilidad**: Renderización de contenido educativo interactivo

**Especificaciones Técnicas**:
- **Renderizado**: Canvas personalizado para actividades interactivas
- **Audio**: MediaElement con soporte para MP3/AAC
- **Animaciones**: Lottie para elementos visuales
- **Gestos**: Tap, Drag, Pinch para interacciones
- **Performance**: 60 FPS durante interacciones

```csharp
public class LessonPageViewModel : BaseViewModel
{
    public LessonContent CurrentLesson { get; set; }
    public ObservableCollection<InteractiveElement> Elements { get; set; }
    public TimeSpan LessonDuration { get; set; }
    public int ErrorCount { get; set; }
    public int StarRating => ErrorCount == 0 ? 3 : (ErrorCount <= 2 ? 2 : 1);
}
```

**Métricas de Performance**:
- Tiempo de carga de lección: < 3 segundos
- Respuesta a interacción: < 100ms
- Uso de memoria por lección: < 50MB

### 2.3 ParentalControlPage

**Responsabilidad**: Panel de control parental con PIN de acceso

**Especificaciones Técnicas**:
- **Autenticación**: PIN de 4 dígitos con hash SHA256
- **Datos**: Progreso académico, tiempo de uso, configuraciones
- **Seguridad**: Bloqueo automático después de 3 intentos fallidos
- **Sincronización**: Datos en tiempo real desde base de datos local

```csharp
public class ParentalControlViewModel : BaseViewModel
{
    [Required]
    [StringLength(4, MinimumLength = 4)]
    public string PIN { get; set; }
    
    public UserProgress ChildProgress { get; set; }
    public TimeSpan DailyUsageTime { get; set; }
    public bool IsPremiumUser { get; set; }
    public DateTime SubscriptionExpiryDate { get; set; }
}
```

## 3. Componentes de la Capa de Aplicación

### 3.1 EducationalEngineService

**Responsabilidad**: Motor principal de la lógica educativa

**Especificaciones Técnicas**:
- **Patrón**: Service con inyección de dependencias
- **Algoritmo de Progresión**: Adaptativo basado en performance del usuario
- **Cache**: In-memory cache para contenido frecuente (LRU, 100MB máximo)
- **Concurrencia**: Thread-safe para múltiples usuarios

```csharp
public interface IEducationalEngineService
{
    Task<LessonContent> GetNextLessonAsync(int userId, string currentModule);
    Task<bool> ValidateAnswerAsync(int lessonId, string userAnswer);
    Task<UserProgress> CalculateProgressAsync(int userId);
    Task<List<Achievement>> CheckAchievementsAsync(int userId);
}

public class EducationalEngineService : IEducationalEngineService
{
    private readonly ILessonRepository _lessonRepository;
    private readonly IUserProgressRepository _progressRepository;
    private readonly IMemoryCache _cache;
    private readonly IAdaptiveAlgorithmService _adaptiveService;
}
```

**Algoritmo Adaptativo**:
- Performance > 90%: Acelerar progresión en 20%
- Performance 70-90%: Mantener ritmo normal
- Performance < 70%: Ralentizar y reforzar conceptos

### 3.2 UserProgressTrackingService

**Responsabilidad**: Seguimiento y persistencia del progreso del usuario

**Especificaciones Técnicas**:
- **Base de Datos**: Entity Framework Core con SQLite
- **Sincronización**: Batch updates cada 30 segundos
- **Backup**: Exportación JSON para migración de datos
- **Métricas**: Tiempo por lección, errores, intentos, fecha de completado

```csharp
public class UserProgressTrackingService : IUserProgressTrackingService
{
    public async Task SaveProgressAsync(UserProgress progress)
    {
        // Validación de datos
        if (!IsValidProgress(progress))
            throw new InvalidProgressDataException();
            
        // Actualización atómica
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            await UpdateProgressInDatabase(progress);
            await UpdateAchievements(progress.UserId);
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
```

### 3.3 BilingualContentService

**Responsabilidad**: Gestión de contenido bilingüe (Español/Inglés)

**Especificaciones Técnicas**:
- **Estructura de Archivos**: `/Assets/Audio/{Language}/{ModuleId}/{LessonId}.mp3`
- **Fallback**: Inglés como idioma por defecto
- **Preloading**: Precarga del siguiente audio durante lección actual
- **Compresión**: MP3 128kbps para optimizar tamaño

```csharp
public class BilingualContentService : IBilingualContentService
{
    private readonly Dictionary<string, CultureInfo> _supportedLanguages = new()
    {
        { "es", new CultureInfo("es-ES") },
        { "en", new CultureInfo("en-US") }
    };
    
    public async Task<string> GetLocalizedAudioPath(string contentId, string language)
    {
        var primaryPath = $"Assets/Audio/{language}/{contentId}.mp3";
        if (await FileExistsAsync(primaryPath))
            return primaryPath;
            
        // Fallback a inglés
        var fallbackPath = $"Assets/Audio/en/{contentId}.mp3";
        return await FileExistsAsync(fallbackPath) ? fallbackPath : null;
    }
}
```

## 4. Componentes de la Capa de Dominio

### 4.1 User (Entidad Principal)

**Especificaciones Técnicas**:
- **Identificador**: GUID único por dispositivo
- **Datos Mínimos**: Nombre, edad, idioma preferido
- **Validaciones**: Edad entre 3-8 años, nombre máximo 50 caracteres

```csharp
public class User : BaseEntity
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; }
    
    [Range(3, 8)]
    public int Age { get; set; }
    
    [Required]
    [MaxLength(2)]
    public string PreferredLanguage { get; set; } // "es" o "en"
    
    public DateTime CreatedAt { get; set; }
    public DateTime LastActiveAt { get; set; }
    public bool IsPremiumUser { get; set; }
    public DateTime? PremiumExpiryDate { get; set; }
    
    // Navegación
    public virtual ICollection<UserProgress> Progress { get; set; }
    public virtual ICollection<Achievement> Achievements { get; set; }
}
```

### 4.2 LessonContent (Contenido Educativo)

**Especificaciones Técnicas**:
- **Identificador**: Código alfanumérico único (ej: "MAT001-L01")
- **Metadatos**: Duración estimada, dificultad, prerequisitos
- **Contenido**: JSON estructurado para actividades interactivas

```csharp
public class LessonContent : BaseEntity
{
    [Required]
    [MaxLength(20)]
    public string LessonCode { get; set; }
    
    [Required]
    public string ModuleId { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string Title { get; set; }
    
    public TimeSpan EstimatedDuration { get; set; }
    
    [Range(1, 5)]
    public int DifficultyLevel { get; set; }
    
    [Column(TypeName = "TEXT")]
    public string InteractiveContentJson { get; set; }
    
    public List<string> AudioAssets { get; set; }
    public List<string> ImageAssets { get; set; }
    public List<string> Prerequisites { get; set; }
}
```

## 5. Componentes de la Capa de Infraestructura

### 5.1 SQLiteDbContext

**Especificaciones Técnicas**:
- **Base de Datos**: SQLite 3.40+
- **Ubicación**: Carpeta local de aplicación (iOS/Android)
- **Tamaño Máximo**: 100MB con compresión automática
- **Encriptación**: SQLCipher para datos sensibles

```csharp
public class EduPlayKidsDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<LessonContent> Lessons { get; set; }
    public DbSet<UserProgress> UserProgress { get; set; }
    public DbSet<Achievement> Achievements { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var dbPath = Path.Combine(
            FileSystem.AppDataDirectory, 
            "EduPlayKids.db3");
            
        optionsBuilder.UseSqlite($"Data Source={dbPath}")
                     .EnableSensitiveDataLogging(false)
                     .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Índices para performance
        modelBuilder.Entity<UserProgress>()
            .HasIndex(p => new { p.UserId, p.LessonId })
            .IsUnique();
            
        modelBuilder.Entity<LessonContent>()
            .HasIndex(l => l.ModuleId);
    }
}
```

### 5.2 AnalyticsService (Cumple COPPA/GDPR-K)

**Especificaciones Técnicas**:
- **Datos Anónimos**: No PII, solo métricas de uso agregadas
- **Frecuencia**: Envío batch cada 24 horas o 100 eventos
- **Consentimiento**: Opt-out por defecto, requiere activación parental
- **Retención**: Máximo 30 días de datos locales

```csharp
public class PrivacyCompliantAnalyticsService : IAnalyticsService
{
    private readonly Queue<AnalyticsEvent> _eventQueue = new();
    private readonly Timer _batchTimer;
    
    public async Task TrackEventAsync(string eventName, Dictionary<string, object> parameters)
    {
        if (!IsAnalyticsEnabled())
            return;
            
        var anonymizedEvent = new AnalyticsEvent
        {
            EventName = eventName,
            Timestamp = DateTime.UtcNow,
            SessionId = GetAnonymousSessionId(),
            Parameters = AnonymizeParameters(parameters)
        };
        
        _eventQueue.Enqueue(anonymizedEvent);
        
        if (_eventQueue.Count >= 100)
            await FlushEventsAsync();
    }
    
    private Dictionary<string, object> AnonymizeParameters(Dictionary<string, object> parameters)
    {
        var anonymized = new Dictionary<string, object>();
        
        foreach (var param in parameters)
        {
            if (IsAllowedParameter(param.Key))
                anonymized[param.Key] = param.Value;
        }
        
        return anonymized;
    }
}
```

## 6. Componentes de Integración

### 6.1 FreemiumManager

**Responsabilidad**: Gestión del modelo freemium y limitaciones

**Especificaciones Técnicas**:
- **Trial Period**: 3 días desde primera instalación
- **Daily Limit**: 10 lecciones para usuarios free
- **Upgrade Flow**: In-app purchase integration
- **Persistence**: Local storage con validación servidor (futuro)

```csharp
public class FreemiumManager : IFreemiumManager
{
    private readonly IPreferencesService _preferences;
    private readonly IDateTimeService _dateTime;
    
    public async Task<FreemiumStatus> GetUserStatusAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        var installDate = _preferences.Get<DateTime>("InstallDate", DateTime.UtcNow);
        var today = _dateTime.Today;
        
        if (user.IsPremiumUser && user.PremiumExpiryDate > today)
        {
            return new FreemiumStatus
            {
                IsPremium = true,
                TrialDaysRemaining = 0,
                DailyLessonsRemaining = int.MaxValue
            };
        }
        
        var trialDaysElapsed = (today - installDate).Days;
        var isTrialActive = trialDaysElapsed < 3;
        
        if (isTrialActive)
        {
            return new FreemiumStatus
            {
                IsTrialActive = true,
                TrialDaysRemaining = 3 - trialDaysElapsed,
                DailyLessonsRemaining = int.MaxValue
            };
        }
        
        // Usuario free después del trial
        var todayLessonsCount = await GetTodayLessonsCountAsync(userId);
        
        return new FreemiumStatus
        {
            IsFreeUser = true,
            DailyLessonsRemaining = Math.Max(0, 10 - todayLessonsCount)
        };
    }
}
```

### 6.2 AssetManager

**Responsabilidad**: Gestión optimizada de recursos multimedia

**Especificaciones Técnicas**:
- **Cache Strategy**: LRU con límite de 200MB
- **Preloading**: Siguiente lección durante lección actual
- **Compression**: Imágenes WebP, Audio MP3 128kbps
- **Cleanup**: Limpieza automática de assets no utilizados

```csharp
public class AssetManager : IAssetManager
{
    private readonly LRUCache<string, byte[]> _memoryCache;
    private readonly string _cacheDirectory;
    
    public async Task<Stream> GetAssetStreamAsync(string assetPath)
    {
        // 1. Check memory cache
        if (_memoryCache.TryGetValue(assetPath, out var cachedData))
            return new MemoryStream(cachedData);
        
        // 2. Check disk cache
        var diskPath = Path.Combine(_cacheDirectory, assetPath);
        if (File.Exists(diskPath))
        {
            var diskData = await File.ReadAllBytesAsync(diskPath);
            _memoryCache.Set(assetPath, diskData);
            return new MemoryStream(diskData);
        }
        
        // 3. Load from embedded resources
        var assembly = Assembly.GetExecutingAssembly();
        var resourceStream = assembly.GetManifestResourceStream($"EduPlayKids.Assets.{assetPath}");
        
        if (resourceStream != null)
        {
            var resourceData = new byte[resourceStream.Length];
            await resourceStream.ReadAsync(resourceData, 0, resourceData.Length);
            
            // Cache for future use
            _memoryCache.Set(assetPath, resourceData);
            await File.WriteAllBytesAsync(diskPath, resourceData);
            
            return new MemoryStream(resourceData);
        }
        
        throw new AssetNotFoundException(assetPath);
    }
}
```

## 7. Métricas de Performance y SLA

### 7.1 Tiempos de Respuesta Objetivo

| Componente | Tiempo Objetivo | Tiempo Crítico |
|------------|----------------|----------------|
| Inicio de aplicación | < 2s | < 5s |
| Carga de lección | < 3s | < 7s |
| Respuesta a interacción | < 100ms | < 300ms |
| Cambio de idioma | < 1s | < 3s |
| Guardado de progreso | < 500ms | < 2s |

### 7.2 Uso de Recursos

| Recurso | Límite Normal | Límite Crítico |
|---------|---------------|----------------|
| RAM Total | < 150MB | < 300MB |
| RAM por lección | < 50MB | < 100MB |
| Almacenamiento DB | < 100MB | < 200MB |
| Cache de assets | < 200MB | < 400MB |
| CPU durante interacción | < 30% | < 60% |

### 7.3 Disponibilidad y Confiabilidad

- **Uptime**: 99.9% (medido por funcionalidad offline)
- **MTBF**: Mean Time Between Failures > 100 horas
- **MTTR**: Mean Time To Recovery < 1 segundo (reinicio automático)
- **Error Rate**: < 0.1% de operaciones fallidas

## 8. Configuración de Deployment

### 8.1 Configuración de Build

```xml
<!-- EduPlayKids.csproj -->
<PropertyGroup>
    <TargetFrameworks>net8.0-android;net8.0-ios</TargetFrameworks>
    <OutputType>Exe</OutputType>
    <RootNamespace>EduPlayKids</RootNamespace>
    <UseMaui>true</UseMaui>
    <SingleProject>true</SingleProject>
    <MauiVersion>8.0.0</MauiVersion>
    
    <!-- Performance optimizations -->
    <AndroidEnableProfiledAot>true</AndroidEnableProfiledAot>
    <AndroidPackageFormat>aab</AndroidPackageFormat>
    <EnableLLVM>true</EnableLLVM>
    
    <!-- iOS optimizations -->
    <MtouchArch>ARM64</MtouchArch>
    <MtouchLink>SdkOnly</MtouchLink>
</PropertyGroup>
```

### 8.2 Configuración de Release

```json
// appsettings.Production.json
{
  "DatabaseSettings": {
    "ConnectionString": "Data Source=EduPlayKids.db3",
    "EnableLogging": false,
    "CommandTimeout": 30
  },
  "AnalyticsSettings": {
    "Enabled": false,
    "RequiresParentalConsent": true,
    "BatchSize": 100,
    "FlushIntervalHours": 24
  },
  "PerformanceSettings": {
    "CacheSize": 200,
    "PreloadNextLesson": true,
    "MaxConcurrentOperations": 3
  }
}
```

---

**Documento Técnico - EduPlayKids v1.0**
**Fecha**: 16 de Septiembre, 2024
**Estado**: Draft para Review Técnico