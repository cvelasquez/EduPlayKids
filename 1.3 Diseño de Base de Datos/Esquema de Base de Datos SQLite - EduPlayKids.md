# Esquema de Base de Datos SQLite - EduPlayKids

## 1. Resumen Ejecutivo

Este documento define el esquema completo de la base de datos SQLite para EduPlayKids MVP, optimizado para dispositivos móviles con funcionalidad offline-first. El esquema soporta múltiples usuarios por dispositivo, contenido bilingüe, progreso educativo detallado y cumplimiento de regulaciones de privacidad.

## 2. Configuración de Base de Datos

### 2.1 Configuración Inicial de SQLite

```sql
-- Configuración de SQLite para performance móvil
PRAGMA foreign_keys = ON;
PRAGMA journal_mode = WAL;
PRAGMA synchronous = NORMAL;
PRAGMA cache_size = -64000; -- 64MB cache
PRAGMA temp_store = MEMORY;
PRAGMA mmap_size = 268435456; -- 256MB mmap
PRAGMA optimize;
```

### 2.2 Información de Versión

```sql
-- Tabla para control de versiones del schema
CREATE TABLE schema_migrations (
    version TEXT PRIMARY KEY,
    description TEXT NOT NULL,
    applied_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    rollback_sql TEXT
);

-- Insertar versión inicial
INSERT INTO schema_migrations (version, description) 
VALUES ('1.0.0', 'Initial EduPlayKids database schema');
```

## 3. Esquema Completo de Tablas

### 3.1 Tabla USERS (Usuarios del Dispositivo)

```sql
CREATE TABLE users (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    name TEXT NOT NULL CHECK(length(name) <= 50),
    age INTEGER NOT NULL CHECK(age >= 3 AND age <= 8),
    preferred_language TEXT NOT NULL DEFAULT 'es' CHECK(preferred_language IN ('es', 'en')),
    is_premium_user BOOLEAN NOT NULL DEFAULT 0,
    premium_expiry_date DATETIME,
    purchase_transaction_id TEXT,
    purchase_date DATETIME,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    last_active_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Índices para performance
CREATE INDEX idx_users_name ON users(name);
CREATE INDEX idx_users_last_active ON users(last_active_at DESC);
CREATE INDEX idx_users_premium ON users(is_premium_user, premium_expiry_date);
```

### 3.2 Tabla EDUCATIONAL_MODULES (Módulos Educativos)

```sql
CREATE TABLE educational_modules (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    module_code TEXT NOT NULL UNIQUE,
    name_key_en TEXT NOT NULL,
    name_key_es TEXT NOT NULL,
    description_key_en TEXT,
    description_key_es TEXT,
    sort_order INTEGER NOT NULL,
    min_age INTEGER NOT NULL CHECK(min_age >= 3),
    max_age INTEGER NOT NULL CHECK(max_age <= 8),
    curriculum_standard TEXT,
    icon_path TEXT,
    is_active BOOLEAN NOT NULL DEFAULT 1,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    CHECK(max_age >= min_age)
);

-- Índices para queries frecuentes
CREATE UNIQUE INDEX idx_modules_code ON educational_modules(module_code);
CREATE INDEX idx_modules_active_sort ON educational_modules(is_active, sort_order);
CREATE INDEX idx_modules_age_range ON educational_modules(min_age, max_age);
```

### 3.3 Tabla LESSON_CONTENT (Contenido de Lecciones)

```sql
CREATE TABLE lesson_content (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    lesson_code TEXT NOT NULL UNIQUE,
    module_id INTEGER NOT NULL,
    title_key_en TEXT NOT NULL,
    title_key_es TEXT NOT NULL,
    instructions_key_en TEXT,
    instructions_key_es TEXT,
    sort_order INTEGER NOT NULL,
    difficulty_level INTEGER NOT NULL CHECK(difficulty_level >= 1 AND difficulty_level <= 5),
    estimated_duration INTEGER NOT NULL, -- en segundos
    interactive_content_json TEXT, -- Contenido interactivo estructurado
    audio_assets_en TEXT, -- JSON array de rutas de audio en inglés
    audio_assets_es TEXT, -- JSON array de rutas de audio en español
    image_assets TEXT, -- JSON array de rutas de imágenes
    prerequisites TEXT, -- JSON array de lesson_codes prerequisitos
    learning_objectives TEXT, -- JSON array de objetivos
    is_active BOOLEAN NOT NULL DEFAULT 1,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    FOREIGN KEY (module_id) REFERENCES educational_modules(id)
);

-- Índices para navigation y performance
CREATE UNIQUE INDEX idx_lessons_code ON lesson_content(lesson_code);
CREATE INDEX idx_lessons_module_sort ON lesson_content(module_id, sort_order);
CREATE INDEX idx_lessons_difficulty ON lesson_content(difficulty_level, is_active);
CREATE INDEX idx_lessons_active ON lesson_content(is_active);
```

### 3.4 Tabla USER_PROGRESS (Progreso del Usuario)

```sql
CREATE TABLE user_progress (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    user_id INTEGER NOT NULL,
    lesson_id INTEGER NOT NULL,
    started_at DATETIME,
    completed_at DATETIME,
    attempt_count INTEGER NOT NULL DEFAULT 1,
    error_count INTEGER NOT NULL DEFAULT 0,
    hint_count INTEGER NOT NULL DEFAULT 0,
    completion_time INTEGER, -- en segundos
    star_rating INTEGER CHECK(star_rating >= 1 AND star_rating <= 3),
    answers_json TEXT, -- Respuestas detalladas del usuario
    session_data TEXT, -- Datos de sesión para continuidad
    is_completed BOOLEAN NOT NULL DEFAULT 0,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
    FOREIGN KEY (lesson_id) REFERENCES lesson_content(id),
    
    -- Unique constraint para evitar duplicados
    UNIQUE(user_id, lesson_id)
);

-- Índices críticos para performance
CREATE INDEX idx_progress_user_completed ON user_progress(user_id, completed_at DESC);
CREATE INDEX idx_progress_user_lesson ON user_progress(user_id, lesson_id);
CREATE INDEX idx_progress_completed ON user_progress(is_completed, completed_at DESC);
CREATE INDEX idx_progress_star_rating ON user_progress(star_rating, completed_at DESC);
```

### 3.5 Tabla USER_ACHIEVEMENTS (Logros del Usuario)

```sql
CREATE TABLE user_achievements (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    user_id INTEGER NOT NULL,
    achievement_type TEXT NOT NULL, -- 'lesson_streak', 'perfect_score', 'module_complete'
    achievement_code TEXT NOT NULL,
    title_key_en TEXT NOT NULL,
    title_key_es TEXT NOT NULL,
    description_key_en TEXT,
    description_key_es TEXT,
    icon_path TEXT,
    metadata_json TEXT, -- Datos específicos del logro
    earned_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
    
    -- Evitar logros duplicados
    UNIQUE(user_id, achievement_code)
);

-- Índices para queries de achievements
CREATE INDEX idx_achievements_user_earned ON user_achievements(user_id, earned_at DESC);
CREATE INDEX idx_achievements_type ON user_achievements(achievement_type);
CREATE INDEX idx_achievements_code ON user_achievements(achievement_code);
```

### 3.6 Tabla DAILY_USAGE (Uso Diario)

```sql
CREATE TABLE daily_usage (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    user_id INTEGER NOT NULL,
    usage_date DATE NOT NULL,
    lessons_completed INTEGER NOT NULL DEFAULT 0,
    total_time_spent INTEGER NOT NULL DEFAULT 0, -- en segundos
    session_count INTEGER NOT NULL DEFAULT 0,
    first_session_at DATETIME,
    last_session_at DATETIME,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
    
    -- Un registro por usuario por día
    UNIQUE(user_id, usage_date)
);

-- Índices para queries de uso diario
CREATE INDEX idx_daily_usage_user_date ON daily_usage(user_id, usage_date DESC);
CREATE INDEX idx_daily_usage_date ON daily_usage(usage_date DESC);
```

### 3.7 Tabla USER_PREFERENCES (Preferencias del Usuario)

```sql
CREATE TABLE user_preferences (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    user_id INTEGER NOT NULL,
    preference_key TEXT NOT NULL,
    preference_value TEXT NOT NULL,
    data_type TEXT NOT NULL DEFAULT 'string' CHECK(data_type IN ('string', 'integer', 'boolean', 'json')),
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
    
    -- Una preferencia por usuario por clave
    UNIQUE(user_id, preference_key)
);

-- Índices para acceso rápido a preferencias
CREATE INDEX idx_preferences_user_key ON user_preferences(user_id, preference_key);
```

### 3.8 Tabla APP_SETTINGS (Configuración de Aplicación)

```sql
CREATE TABLE app_settings (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    setting_key TEXT NOT NULL UNIQUE,
    setting_value TEXT NOT NULL,
    description TEXT,
    data_type TEXT NOT NULL DEFAULT 'string' CHECK(data_type IN ('string', 'integer', 'boolean', 'json')),
    is_user_configurable BOOLEAN NOT NULL DEFAULT 0,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Índice para acceso rápido por clave
CREATE UNIQUE INDEX idx_settings_key ON app_settings(setting_key);
```

### 3.9 Tabla ANALYTICS_EVENTS (Eventos de Analytics)

```sql
CREATE TABLE analytics_events (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    session_id TEXT NOT NULL, -- ID anónimo que rota cada launch
    event_name TEXT NOT NULL,
    event_category TEXT NOT NULL, -- 'education', 'navigation', 'system'
    parameters_json TEXT, -- Parámetros anónimos del evento
    event_timestamp DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    is_synced BOOLEAN NOT NULL DEFAULT 0,
    synced_at DATETIME,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Índices para processing y sync
CREATE INDEX idx_analytics_sync ON analytics_events(is_synced, event_timestamp);
CREATE INDEX idx_analytics_session ON analytics_events(session_id, event_timestamp);
CREATE INDEX idx_analytics_category ON analytics_events(event_category, event_timestamp);
```

### 3.10 Tabla CONTENT_LOCALIZATION (Localización de Contenido)

```sql
CREATE TABLE content_localization (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    localization_key TEXT NOT NULL,
    language TEXT NOT NULL CHECK(language IN ('es', 'en')),
    localized_text TEXT NOT NULL,
    context TEXT, -- Contexto para traducción
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    -- Una traducción por clave por idioma
    UNIQUE(localization_key, language)
);

-- Índices para localización rápida
CREATE INDEX idx_localization_key_lang ON content_localization(localization_key, language);
CREATE INDEX idx_localization_lang ON content_localization(language);
```

## 4. Triggers para Mantenimiento Automático

### 4.1 Trigger de Updated_At

```sql
-- Crear trigger para actualizar updated_at automáticamente
CREATE TRIGGER update_users_timestamp 
    AFTER UPDATE ON users
    FOR EACH ROW
    BEGIN
        UPDATE users SET updated_at = CURRENT_TIMESTAMP WHERE id = NEW.id;
    END;

CREATE TRIGGER update_modules_timestamp 
    AFTER UPDATE ON educational_modules
    FOR EACH ROW
    BEGIN
        UPDATE educational_modules SET updated_at = CURRENT_TIMESTAMP WHERE id = NEW.id;
    END;

CREATE TRIGGER update_lessons_timestamp 
    AFTER UPDATE ON lesson_content
    FOR EACH ROW
    BEGIN
        UPDATE lesson_content SET updated_at = CURRENT_TIMESTAMP WHERE id = NEW.id;
    END;

CREATE TRIGGER update_progress_timestamp 
    AFTER UPDATE ON user_progress
    FOR EACH ROW
    BEGIN
        UPDATE user_progress SET updated_at = CURRENT_TIMESTAMP WHERE id = NEW.id;
    END;

CREATE TRIGGER update_daily_usage_timestamp 
    AFTER UPDATE ON daily_usage
    FOR EACH ROW
    BEGIN
        UPDATE daily_usage SET updated_at = CURRENT_TIMESTAMP WHERE id = NEW.id;
    END;

CREATE TRIGGER update_preferences_timestamp 
    AFTER UPDATE ON user_preferences
    FOR EACH ROW
    BEGIN
        UPDATE user_preferences SET updated_at = CURRENT_TIMESTAMP WHERE id = NEW.id;
    END;

CREATE TRIGGER update_settings_timestamp 
    AFTER UPDATE ON app_settings
    FOR EACH ROW
    BEGIN
        UPDATE app_settings SET updated_at = CURRENT_TIMESTAMP WHERE id = NEW.id;
    END;

CREATE TRIGGER update_localization_timestamp 
    AFTER UPDATE ON content_localization
    FOR EACH ROW
    BEGIN
        UPDATE content_localization SET updated_at = CURRENT_TIMESTAMP WHERE id = NEW.id;
    END;
```

### 4.2 Trigger para Daily Usage

```sql
-- Trigger para mantener actualizado daily_usage
CREATE TRIGGER update_daily_usage_on_progress
    AFTER INSERT ON user_progress
    WHEN NEW.is_completed = 1
    BEGIN
        INSERT OR REPLACE INTO daily_usage (
            user_id, 
            usage_date, 
            lessons_completed,
            total_time_spent,
            session_count,
            first_session_at,
            last_session_at,
            updated_at
        ) VALUES (
            NEW.user_id,
            date(NEW.completed_at),
            COALESCE((SELECT lessons_completed FROM daily_usage WHERE user_id = NEW.user_id AND usage_date = date(NEW.completed_at)), 0) + 1,
            COALESCE((SELECT total_time_spent FROM daily_usage WHERE user_id = NEW.user_id AND usage_date = date(NEW.completed_at)), 0) + COALESCE(NEW.completion_time, 0),
            COALESCE((SELECT session_count FROM daily_usage WHERE user_id = NEW.user_id AND usage_date = date(NEW.completed_at)), 0) + 1,
            COALESCE((SELECT first_session_at FROM daily_usage WHERE user_id = NEW.user_id AND usage_date = date(NEW.completed_at)), NEW.completed_at),
            NEW.completed_at,
            CURRENT_TIMESTAMP
        );
    END;
```

## 5. Views para Queries Comunes

### 5.1 View: User Stats

```sql
CREATE VIEW user_stats AS
SELECT 
    u.id,
    u.name,
    u.age,
    u.preferred_language,
    u.is_premium_user,
    COUNT(DISTINCT up.lesson_id) as lessons_completed,
    AVG(up.star_rating) as avg_star_rating,
    SUM(up.completion_time) as total_study_time,
    MAX(up.completed_at) as last_lesson_date,
    COUNT(ua.id) as achievements_count
FROM users u
LEFT JOIN user_progress up ON u.id = up.user_id AND up.is_completed = 1
LEFT JOIN user_achievements ua ON u.id = ua.user_id
GROUP BY u.id;
```

### 5.2 View: Lesson Progress

```sql
CREATE VIEW lesson_progress_view AS
SELECT 
    lc.id,
    lc.lesson_code,
    lc.title_key_en,
    lc.title_key_es,
    em.module_code,
    em.name_key_en as module_name_en,
    em.name_key_es as module_name_es,
    lc.difficulty_level,
    lc.estimated_duration,
    up.user_id,
    up.completed_at,
    up.star_rating,
    up.error_count,
    up.is_completed
FROM lesson_content lc
JOIN educational_modules em ON lc.module_id = em.id
LEFT JOIN user_progress up ON lc.id = up.lesson_id
WHERE lc.is_active = 1 AND em.is_active = 1;
```

### 5.3 View: Daily Usage Summary

```sql
CREATE VIEW daily_usage_summary AS
SELECT 
    du.usage_date,
    COUNT(DISTINCT du.user_id) as active_users,
    SUM(du.lessons_completed) as total_lessons,
    AVG(du.lessons_completed) as avg_lessons_per_user,
    SUM(du.total_time_spent) as total_time,
    AVG(du.total_time_spent) as avg_time_per_user
FROM daily_usage du
GROUP BY du.usage_date
ORDER BY du.usage_date DESC;
```

## 6. Datos Iniciales (Seed Data)

### 6.1 Configuraciones de Aplicación

```sql
-- Configuraciones iniciales de la aplicación
INSERT INTO app_settings (setting_key, setting_value, description, data_type, is_user_configurable) VALUES
('app_version', '1.0.0', 'Current application version', 'string', 0),
('database_version', '1.0.0', 'Current database schema version', 'string', 0),
('analytics_enabled', '0', 'Analytics collection enabled (requires parental consent)', 'boolean', 1),
('sound_effects_enabled', '1', 'Enable sound effects', 'boolean', 1),
('background_music_enabled', '1', 'Enable background music', 'boolean', 1),
('daily_lesson_limit_free', '10', 'Daily lesson limit for free users', 'integer', 0),
('trial_period_days', '3', 'Trial period duration in days', 'integer', 0),
('premium_price_usd', '4.99', 'Premium upgrade price in USD', 'string', 0),
('content_language_fallback', 'en', 'Fallback language when content not available', 'string', 0),
('session_timeout_minutes', '30', 'Session timeout in minutes', 'integer', 0);
```

### 6.2 Módulos Educativos Básicos

```sql
-- Módulos educativos iniciales
INSERT INTO educational_modules (module_code, name_key_en, name_key_es, description_key_en, description_key_es, sort_order, min_age, max_age, curriculum_standard, icon_path) VALUES
('MATH_BASIC', 'math.basic.title', 'math.basic.title', 'math.basic.description', 'math.basic.description', 1, 3, 6, 'Common Core K-2', 'modules/math_basic.png'),
('READING_PHONICS', 'reading.phonics.title', 'reading.phonics.title', 'reading.phonics.description', 'reading.phonics.description', 2, 4, 7, 'Common Core K-2 Reading', 'modules/reading_phonics.png'),
('SCIENCE_NATURE', 'science.nature.title', 'science.nature.title', 'science.nature.description', 'science.nature.description', 3, 5, 8, 'NGSS K-2', 'modules/science_nature.png'),
('ART_CREATIVITY', 'art.creativity.title', 'art.creativity.title', 'art.creativity.description', 'art.creativity.description', 4, 3, 8, 'National Art Standards', 'modules/art_creativity.png'),
('LOGIC_PUZZLES', 'logic.puzzles.title', 'logic.puzzles.title', 'logic.puzzles.description', 'logic.puzzles.description', 5, 5, 8, 'Critical Thinking Standards', 'modules/logic_puzzles.png');
```

### 6.3 Localizaciones Básicas

```sql
-- Localizaciones básicas para UI
INSERT INTO content_localization (localization_key, language, localized_text, context) VALUES
-- Módulos
('math.basic.title', 'en', 'Basic Math', 'Module title'),
('math.basic.title', 'es', 'Matemáticas Básicas', 'Module title'),
('math.basic.description', 'en', 'Learn numbers, counting, and basic operations', 'Module description'),
('math.basic.description', 'es', 'Aprende números, contar y operaciones básicas', 'Module description'),

('reading.phonics.title', 'en', 'Reading & Phonics', 'Module title'),
('reading.phonics.title', 'es', 'Lectura y Fonética', 'Module title'),
('reading.phonics.description', 'en', 'Learn letters, sounds, and reading skills', 'Module description'),
('reading.phonics.description', 'es', 'Aprende letras, sonidos y habilidades de lectura', 'Module description'),

-- UI Common
('ui.welcome', 'en', 'Welcome to EduPlayKids!', 'Welcome message'),
('ui.welcome', 'es', '¡Bienvenido a EduPlayKids!', 'Welcome message'),
('ui.start_learning', 'en', 'Start Learning', 'Button text'),
('ui.start_learning', 'es', 'Comenzar a Aprender', 'Button text'),
('ui.continue_lesson', 'en', 'Continue Lesson', 'Button text'),
('ui.continue_lesson', 'es', 'Continuar Lección', 'Button text'),
('ui.excellent', 'en', 'Excellent!', 'Praise message'),
('ui.excellent', 'es', '¡Excelente!', 'Praise message'),
('ui.try_again', 'en', 'Try Again', 'Encouragement message'),
('ui.try_again', 'es', 'Inténtalo de Nuevo', 'Encouragement message');
```

## 7. Queries de Administración

### 7.1 Database Health Check

```sql
-- Verificar integridad de la base de datos
PRAGMA integrity_check;

-- Estadísticas de la base de datos
SELECT 
    name as table_name,
    (SELECT COUNT(*) FROM sqlite_master WHERE type='index' AND tbl_name=m.name) as index_count
FROM sqlite_master m 
WHERE type='table' 
ORDER BY name;

-- Tamaño de tablas
SELECT 
    name,
    COUNT(*) as row_count
FROM sqlite_master m
LEFT JOIN (
    SELECT 'users' as tbl, COUNT(*) as cnt FROM users
    UNION ALL SELECT 'user_progress', COUNT(*) FROM user_progress
    UNION ALL SELECT 'lesson_content', COUNT(*) FROM lesson_content
    UNION ALL SELECT 'educational_modules', COUNT(*) FROM educational_modules
) counts ON m.name = counts.tbl
WHERE m.type = 'table'
ORDER BY row_count DESC;
```

### 7.2 Performance Analysis

```sql
-- Analizar queries lentas (requiere EXPLAIN QUERY PLAN)
-- Ejemplo para analizar progreso de usuario
EXPLAIN QUERY PLAN 
SELECT up.*, lc.title_key_en, em.name_key_en
FROM user_progress up
JOIN lesson_content lc ON up.lesson_id = lc.id
JOIN educational_modules em ON lc.module_id = em.id
WHERE up.user_id = 1 AND up.is_completed = 1
ORDER BY up.completed_at DESC;
```

### 7.3 Maintenance Tasks

```sql
-- Limpieza de datos antiguos (ejecutar periódicamente)
DELETE FROM analytics_events 
WHERE created_at < datetime('now', '-30 days');

-- Optimización de base de datos
PRAGMA optimize;
VACUUM;

-- Reconstruir estadísticas
ANALYZE;
```

## 8. Consideraciones de Performance

### 8.1 Optimizaciones Implementadas

**Índices Estratégicos**:
- Índices compuestos para queries frecuentes
- Índices parciales para datos activos
- Índices sobre foreign keys

**Query Optimization**:
- Views para queries complejas comunes
- Triggers para mantener datos calculados
- JSON para datos semi-estructurados únicamente

**Storage Optimization**:
- Tipos de datos apropiados (INTEGER vs TEXT)
- Constraints para integridad de datos
- Normalización balanceada

### 8.2 Monitoring Queries

```sql
-- Query para monitorear performance
SELECT 
    'Database Size (MB)' as metric,
    CAST((page_count * page_size / 1024.0 / 1024.0) AS INTEGER) as value
FROM pragma_page_count(), pragma_page_size()

UNION ALL

SELECT 
    'Total Users' as metric,
    COUNT(*) as value
FROM users

UNION ALL

SELECT 
    'Lessons Completed Today' as metric,
    COALESCE(SUM(lessons_completed), 0) as value
FROM daily_usage 
WHERE usage_date = date('now');
```

---

**Documento Schema SQLite - EduPlayKids v1.0**
**Fecha**: 16 de Septiembre, 2024
**Estado**: Ready for Implementation
**Database Version**: 1.0.0