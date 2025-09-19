# Scripts de Migración - EduPlayKids

## 1. Resumen Ejecutivo

Este documento contiene todos los scripts de migración necesarios para crear, actualizar y mantener la base de datos SQLite de EduPlayKids. Los scripts están organizados por versiones y incluyen tanto la creación inicial como futuras actualizaciones del esquema.

## 2. Sistema de Control de Versiones

### 2.1 Convención de Versioning

**Formato**: `MAJOR.MINOR.PATCH`
- **MAJOR**: Cambios incompatibles de schema
- **MINOR**: Nuevas funcionalidades compatibles hacia atrás
- **PATCH**: Correcciones de bugs y optimizaciones

### 2.2 Tabla de Control de Migraciones

```sql
-- Tabla para rastrear migraciones aplicadas
CREATE TABLE IF NOT EXISTS schema_migrations (
    version TEXT PRIMARY KEY,
    description TEXT NOT NULL,
    applied_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    rollback_sql TEXT,
    checksum TEXT -- Para verificar integridad
);
```

## 3. Migración Inicial - Versión 1.0.0

### 3.1 Script: `001_initial_schema.sql`

```sql
-- ================================================
-- EduPlayKids Database Schema v1.0.0
-- Initial database creation
-- ================================================

PRAGMA foreign_keys = ON;
PRAGMA journal_mode = WAL;
PRAGMA synchronous = NORMAL;
PRAGMA cache_size = -64000;
PRAGMA temp_store = MEMORY;

-- Control de migraciones
CREATE TABLE IF NOT EXISTS schema_migrations (
    version TEXT PRIMARY KEY,
    description TEXT NOT NULL,
    applied_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    rollback_sql TEXT,
    checksum TEXT
);

-- ================================================
-- TABLA: users
-- ================================================
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

-- Índices para users
CREATE INDEX idx_users_name ON users(name);
CREATE INDEX idx_users_last_active ON users(last_active_at DESC);
CREATE INDEX idx_users_premium ON users(is_premium_user, premium_expiry_date);

-- ================================================
-- TABLA: educational_modules
-- ================================================
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

-- Índices para educational_modules
CREATE UNIQUE INDEX idx_modules_code ON educational_modules(module_code);
CREATE INDEX idx_modules_active_sort ON educational_modules(is_active, sort_order);
CREATE INDEX idx_modules_age_range ON educational_modules(min_age, max_age);

-- ================================================
-- TABLA: lesson_content
-- ================================================
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
    estimated_duration INTEGER NOT NULL,
    interactive_content_json TEXT,
    audio_assets_en TEXT,
    audio_assets_es TEXT,
    image_assets TEXT,
    prerequisites TEXT,
    learning_objectives TEXT,
    is_active BOOLEAN NOT NULL DEFAULT 1,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    FOREIGN KEY (module_id) REFERENCES educational_modules(id)
);

-- Índices para lesson_content
CREATE UNIQUE INDEX idx_lessons_code ON lesson_content(lesson_code);
CREATE INDEX idx_lessons_module_sort ON lesson_content(module_id, sort_order);
CREATE INDEX idx_lessons_difficulty ON lesson_content(difficulty_level, is_active);
CREATE INDEX idx_lessons_active ON lesson_content(is_active);

-- ================================================
-- TABLA: user_progress
-- ================================================
CREATE TABLE user_progress (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    user_id INTEGER NOT NULL,
    lesson_id INTEGER NOT NULL,
    started_at DATETIME,
    completed_at DATETIME,
    attempt_count INTEGER NOT NULL DEFAULT 1,
    error_count INTEGER NOT NULL DEFAULT 0,
    hint_count INTEGER NOT NULL DEFAULT 0,
    completion_time INTEGER,
    star_rating INTEGER CHECK(star_rating >= 1 AND star_rating <= 3),
    answers_json TEXT,
    session_data TEXT,
    is_completed BOOLEAN NOT NULL DEFAULT 0,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
    FOREIGN KEY (lesson_id) REFERENCES lesson_content(id),
    
    UNIQUE(user_id, lesson_id)
);

-- Índices para user_progress
CREATE INDEX idx_progress_user_completed ON user_progress(user_id, completed_at DESC);
CREATE INDEX idx_progress_user_lesson ON user_progress(user_id, lesson_id);
CREATE INDEX idx_progress_completed ON user_progress(is_completed, completed_at DESC);
CREATE INDEX idx_progress_star_rating ON user_progress(star_rating, completed_at DESC);

-- ================================================
-- TABLA: user_achievements
-- ================================================
CREATE TABLE user_achievements (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    user_id INTEGER NOT NULL,
    achievement_type TEXT NOT NULL,
    achievement_code TEXT NOT NULL,
    title_key_en TEXT NOT NULL,
    title_key_es TEXT NOT NULL,
    description_key_en TEXT,
    description_key_es TEXT,
    icon_path TEXT,
    metadata_json TEXT,
    earned_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
    
    UNIQUE(user_id, achievement_code)
);

-- Índices para user_achievements
CREATE INDEX idx_achievements_user_earned ON user_achievements(user_id, earned_at DESC);
CREATE INDEX idx_achievements_type ON user_achievements(achievement_type);
CREATE INDEX idx_achievements_code ON user_achievements(achievement_code);

-- ================================================
-- TABLA: daily_usage
-- ================================================
CREATE TABLE daily_usage (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    user_id INTEGER NOT NULL,
    usage_date DATE NOT NULL,
    lessons_completed INTEGER NOT NULL DEFAULT 0,
    total_time_spent INTEGER NOT NULL DEFAULT 0,
    session_count INTEGER NOT NULL DEFAULT 0,
    first_session_at DATETIME,
    last_session_at DATETIME,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
    
    UNIQUE(user_id, usage_date)
);

-- Índices para daily_usage
CREATE INDEX idx_daily_usage_user_date ON daily_usage(user_id, usage_date DESC);
CREATE INDEX idx_daily_usage_date ON daily_usage(usage_date DESC);

-- ================================================
-- TABLA: user_preferences
-- ================================================
CREATE TABLE user_preferences (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    user_id INTEGER NOT NULL,
    preference_key TEXT NOT NULL,
    preference_value TEXT NOT NULL,
    data_type TEXT NOT NULL DEFAULT 'string' CHECK(data_type IN ('string', 'integer', 'boolean', 'json')),
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
    
    UNIQUE(user_id, preference_key)
);

-- Índices para user_preferences
CREATE INDEX idx_preferences_user_key ON user_preferences(user_id, preference_key);

-- ================================================
-- TABLA: app_settings
-- ================================================
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

-- Índices para app_settings
CREATE UNIQUE INDEX idx_settings_key ON app_settings(setting_key);

-- ================================================
-- TABLA: analytics_events
-- ================================================
CREATE TABLE analytics_events (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    session_id TEXT NOT NULL,
    event_name TEXT NOT NULL,
    event_category TEXT NOT NULL,
    parameters_json TEXT,
    event_timestamp DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    is_synced BOOLEAN NOT NULL DEFAULT 0,
    synced_at DATETIME,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Índices para analytics_events
CREATE INDEX idx_analytics_sync ON analytics_events(is_synced, event_timestamp);
CREATE INDEX idx_analytics_session ON analytics_events(session_id, event_timestamp);
CREATE INDEX idx_analytics_category ON analytics_events(event_category, event_timestamp);

-- ================================================
-- TABLA: content_localization
-- ================================================
CREATE TABLE content_localization (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    localization_key TEXT NOT NULL,
    language TEXT NOT NULL CHECK(language IN ('es', 'en')),
    localized_text TEXT NOT NULL,
    context TEXT,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    UNIQUE(localization_key, language)
);

-- Índices para content_localization
CREATE INDEX idx_localization_key_lang ON content_localization(localization_key, language);
CREATE INDEX idx_localization_lang ON content_localization(language);

-- Registrar migración
INSERT INTO schema_migrations (version, description) 
VALUES ('1.0.0', 'Initial database schema creation');
```

### 3.2 Script: `002_create_triggers.sql`

```sql
-- ================================================
-- EduPlayKids Triggers v1.0.0
-- Automatización de timestamps y business logic
-- ================================================

-- ================================================
-- TRIGGERS: updated_at automático
-- ================================================

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

-- ================================================
-- TRIGGER: Daily Usage automático
-- ================================================

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

-- Actualizar last_active_at en users cuando hay progreso
CREATE TRIGGER update_user_last_active_on_progress
    AFTER INSERT ON user_progress
    BEGIN
        UPDATE users SET last_active_at = CURRENT_TIMESTAMP WHERE id = NEW.user_id;
    END;

-- Registrar migración de triggers
INSERT INTO schema_migrations (version, description) 
VALUES ('1.0.0-triggers', 'Database triggers for automation');
```

### 3.3 Script: `003_create_views.sql`

```sql
-- ================================================
-- EduPlayKids Views v1.0.0
-- Views para queries comunes y reporting
-- ================================================

-- ================================================
-- VIEW: user_stats
-- Estadísticas generales por usuario
-- ================================================
CREATE VIEW user_stats AS
SELECT 
    u.id,
    u.name,
    u.age,
    u.preferred_language,
    u.is_premium_user,
    u.created_at as joined_at,
    u.last_active_at,
    COUNT(DISTINCT up.lesson_id) as lessons_completed,
    ROUND(AVG(up.star_rating), 2) as avg_star_rating,
    SUM(up.completion_time) as total_study_time_seconds,
    MAX(up.completed_at) as last_lesson_date,
    COUNT(ua.id) as achievements_count,
    COALESCE(SUM(du.lessons_completed), 0) as total_lessons_ever,
    COALESCE(MAX(du.usage_date), date('1900-01-01')) as last_usage_date
FROM users u
LEFT JOIN user_progress up ON u.id = up.user_id AND up.is_completed = 1
LEFT JOIN user_achievements ua ON u.id = ua.user_id
LEFT JOIN daily_usage du ON u.id = du.user_id
GROUP BY u.id, u.name, u.age, u.preferred_language, u.is_premium_user, u.created_at, u.last_active_at;

-- ================================================
-- VIEW: lesson_progress_view
-- Progreso de lecciones con información de módulos
-- ================================================
CREATE VIEW lesson_progress_view AS
SELECT 
    lc.id as lesson_id,
    lc.lesson_code,
    lc.title_key_en,
    lc.title_key_es,
    lc.sort_order as lesson_order,
    lc.difficulty_level,
    lc.estimated_duration,
    em.id as module_id,
    em.module_code,
    em.name_key_en as module_name_en,
    em.name_key_es as module_name_es,
    em.sort_order as module_order,
    up.user_id,
    up.started_at,
    up.completed_at,
    up.star_rating,
    up.error_count,
    up.attempt_count,
    up.completion_time,
    up.is_completed,
    CASE 
        WHEN up.is_completed = 1 THEN 'completed'
        WHEN up.started_at IS NOT NULL THEN 'in_progress'
        ELSE 'not_started'
    END as progress_status
FROM lesson_content lc
JOIN educational_modules em ON lc.module_id = em.id
LEFT JOIN user_progress up ON lc.id = up.lesson_id
WHERE lc.is_active = 1 AND em.is_active = 1
ORDER BY em.sort_order, lc.sort_order;

-- ================================================
-- VIEW: daily_usage_summary
-- Resumen de uso diario agregado
-- ================================================
CREATE VIEW daily_usage_summary AS
SELECT 
    du.usage_date,
    COUNT(DISTINCT du.user_id) as active_users,
    SUM(du.lessons_completed) as total_lessons,
    ROUND(AVG(du.lessons_completed), 1) as avg_lessons_per_user,
    SUM(du.total_time_spent) as total_time_seconds,
    ROUND(AVG(du.total_time_spent), 0) as avg_time_per_user_seconds,
    SUM(du.session_count) as total_sessions,
    ROUND(AVG(du.session_count), 1) as avg_sessions_per_user
FROM daily_usage du
GROUP BY du.usage_date
ORDER BY du.usage_date DESC;

-- ================================================
-- VIEW: module_completion_stats
-- Estadísticas de completado por módulo
-- ================================================
CREATE VIEW module_completion_stats AS
SELECT 
    em.id as module_id,
    em.module_code,
    em.name_key_en,
    em.name_key_es,
    COUNT(lc.id) as total_lessons,
    COUNT(DISTINCT up.user_id) as users_started,
    COUNT(CASE WHEN up.is_completed = 1 THEN 1 END) as total_completions,
    ROUND(AVG(up.star_rating), 2) as avg_star_rating,
    ROUND(AVG(up.completion_time), 0) as avg_completion_time,
    ROUND(AVG(up.error_count), 1) as avg_error_count
FROM educational_modules em
LEFT JOIN lesson_content lc ON em.id = lc.module_id AND lc.is_active = 1
LEFT JOIN user_progress up ON lc.id = up.lesson_id
WHERE em.is_active = 1
GROUP BY em.id, em.module_code, em.name_key_en, em.name_key_es
ORDER BY em.sort_order;

-- ================================================
-- VIEW: freemium_status
-- Estado freemium por usuario
-- ================================================
CREATE VIEW freemium_status AS
SELECT 
    u.id as user_id,
    u.name,
    u.is_premium_user,
    u.premium_expiry_date,
    u.created_at as install_date,
    CASE 
        WHEN u.is_premium_user = 1 AND (u.premium_expiry_date IS NULL OR u.premium_expiry_date > datetime('now')) 
        THEN 'premium'
        WHEN julianday('now') - julianday(u.created_at) <= 3 
        THEN 'trial'
        ELSE 'free'
    END as user_status,
    CASE 
        WHEN julianday('now') - julianday(u.created_at) <= 3 
        THEN CAST(3 - (julianday('now') - julianday(u.created_at)) AS INTEGER)
        ELSE 0
    END as trial_days_remaining,
    COALESCE(du.lessons_completed, 0) as today_lessons_completed,
    CASE 
        WHEN u.is_premium_user = 1 THEN 999999
        WHEN julianday('now') - julianday(u.created_at) <= 3 THEN 999999
        ELSE GREATEST(0, 10 - COALESCE(du.lessons_completed, 0))
    END as daily_lessons_remaining
FROM users u
LEFT JOIN daily_usage du ON u.id = du.user_id AND du.usage_date = date('now');

-- Registrar migración de views
INSERT INTO schema_migrations (version, description) 
VALUES ('1.0.0-views', 'Database views for common queries');
```

### 3.4 Script: `004_seed_data.sql`

```sql
-- ================================================
-- EduPlayKids Seed Data v1.0.0
-- Datos iniciales necesarios para la aplicación
-- ================================================

-- ================================================
-- APP SETTINGS: Configuraciones iniciales
-- ================================================
INSERT OR REPLACE INTO app_settings (setting_key, setting_value, description, data_type, is_user_configurable) VALUES
-- Configuraciones de sistema
('app_version', '1.0.0', 'Current application version', 'string', 0),
('database_version', '1.0.0', 'Current database schema version', 'string', 0),
('install_date', datetime('now'), 'First installation timestamp', 'string', 0),

-- Configuraciones de freemium
('daily_lesson_limit_free', '10', 'Daily lesson limit for free users', 'integer', 0),
('trial_period_days', '3', 'Trial period duration in days', 'integer', 0),
('premium_price_usd', '4.99', 'Premium upgrade price in USD', 'string', 0),

-- Configuraciones de usuario
('analytics_enabled', '0', 'Analytics collection enabled (requires parental consent)', 'boolean', 1),
('sound_effects_enabled', '1', 'Enable sound effects', 'boolean', 1),
('background_music_enabled', '1', 'Enable background music', 'boolean', 1),
('haptic_feedback_enabled', '1', 'Enable haptic feedback', 'boolean', 1),

-- Configuraciones de contenido
('content_language_fallback', 'en', 'Fallback language when content not available', 'string', 0),
('session_timeout_minutes', '30', 'Session timeout in minutes', 'integer', 0),
('auto_save_progress', '1', 'Automatically save progress', 'boolean', 1),

-- Configuraciones de performance
('cache_size_mb', '64', 'Cache size in megabytes', 'integer', 0),
('preload_next_lesson', '1', 'Preload next lesson content', 'boolean', 0),
('image_quality', 'high', 'Image quality setting (low, medium, high)', 'string', 1);

-- ================================================
-- EDUCATIONAL MODULES: Módulos básicos
-- ================================================
INSERT OR REPLACE INTO educational_modules (module_code, name_key_en, name_key_es, description_key_en, description_key_es, sort_order, min_age, max_age, curriculum_standard, icon_path, is_active) VALUES
('MATH_BASIC', 'module.math.basic.title', 'module.math.basic.title', 'module.math.basic.description', 'module.math.basic.description', 1, 3, 6, 'Common Core K-2 Math', 'modules/math_basic.png', 1),
('READING_PHONICS', 'module.reading.phonics.title', 'module.reading.phonics.title', 'module.reading.phonics.description', 'module.reading.phonics.description', 2, 4, 7, 'Common Core K-2 Reading', 'modules/reading_phonics.png', 1),
('SCIENCE_NATURE', 'module.science.nature.title', 'module.science.nature.title', 'module.science.nature.description', 'module.science.nature.description', 3, 5, 8, 'NGSS K-2 Science', 'modules/science_nature.png', 1),
('ART_CREATIVITY', 'module.art.creativity.title', 'module.art.creativity.title', 'module.art.creativity.description', 'module.art.creativity.description', 4, 3, 8, 'National Art Standards', 'modules/art_creativity.png', 1),
('LOGIC_PUZZLES', 'module.logic.puzzles.title', 'module.logic.puzzles.title', 'module.logic.puzzles.description', 'module.logic.puzzles.description', 5, 5, 8, 'Critical Thinking', 'modules/logic_puzzles.png', 1);

-- ================================================
-- CONTENT LOCALIZATION: Localizaciones básicas
-- ================================================

-- Módulos - Títulos
INSERT OR REPLACE INTO content_localization (localization_key, language, localized_text, context) VALUES
('module.math.basic.title', 'en', 'Basic Math', 'Module title'),
('module.math.basic.title', 'es', 'Matemáticas Básicas', 'Module title'),
('module.math.basic.description', 'en', 'Learn numbers, counting, and basic operations through fun games', 'Module description'),
('module.math.basic.description', 'es', 'Aprende números, contar y operaciones básicas con juegos divertidos', 'Module description'),

('module.reading.phonics.title', 'en', 'Reading & Phonics', 'Module title'),
('module.reading.phonics.title', 'es', 'Lectura y Fonética', 'Module title'),
('module.reading.phonics.description', 'en', 'Master letters, sounds, and reading skills step by step', 'Module description'),
('module.reading.phonics.description', 'es', 'Domina letras, sonidos y habilidades de lectura paso a paso', 'Module description'),

('module.science.nature.title', 'en', 'Science & Nature', 'Module title'),
('module.science.nature.title', 'es', 'Ciencias y Naturaleza', 'Module title'),
('module.science.nature.description', 'en', 'Explore the world around us with scientific discovery', 'Module description'),
('module.science.nature.description', 'es', 'Explora el mundo que nos rodea con descubrimientos científicos', 'Module description'),

('module.art.creativity.title', 'en', 'Art & Creativity', 'Module title'),
('module.art.creativity.title', 'es', 'Arte y Creatividad', 'Module title'),
('module.art.creativity.description', 'en', 'Express creativity through colors, shapes, and imagination', 'Module description'),
('module.art.creativity.description', 'es', 'Expresa creatividad con colores, formas e imaginación', 'Module description'),

('module.logic.puzzles.title', 'en', 'Logic & Puzzles', 'Module title'),
('module.logic.puzzles.title', 'es', 'Lógica y Rompecabezas', 'Module title'),
('module.logic.puzzles.description', 'en', 'Develop critical thinking with engaging puzzles and challenges', 'Module description'),
('module.logic.puzzles.description', 'es', 'Desarrolla pensamiento crítico con rompecabezas y desafíos', 'Module description'),

-- UI Común
('ui.welcome', 'en', 'Welcome to EduPlayKids!', 'Welcome message'),
('ui.welcome', 'es', '¡Bienvenido a EduPlayKids!', 'Welcome message'),
('ui.lets_learn', 'en', 'Let''s Learn!', 'CTA button'),
('ui.lets_learn', 'es', '¡Vamos a Aprender!', 'CTA button'),
('ui.continue', 'en', 'Continue', 'Continue button'),
('ui.continue', 'es', 'Continuar', 'Continue button'),
('ui.start', 'en', 'Start', 'Start button'),
('ui.start', 'es', 'Comenzar', 'Start button'),
('ui.next', 'en', 'Next', 'Next button'),
('ui.next', 'es', 'Siguiente', 'Next button'),
('ui.back', 'en', 'Back', 'Back button'),
('ui.back', 'es', 'Atrás', 'Back button'),
('ui.retry', 'en', 'Try Again', 'Retry button'),
('ui.retry', 'es', 'Intentar de Nuevo', 'Retry button'),

-- Feedback y Motivación
('feedback.excellent', 'en', 'Excellent!', 'Praise for perfect performance'),
('feedback.excellent', 'es', '¡Excelente!', 'Praise for perfect performance'),
('feedback.great_job', 'en', 'Great Job!', 'Praise for good performance'),
('feedback.great_job', 'es', '¡Buen Trabajo!', 'Praise for good performance'),
('feedback.good_try', 'en', 'Good Try!', 'Encouragement after mistakes'),
('feedback.good_try', 'es', '¡Buen Intento!', 'Encouragement after mistakes'),
('feedback.keep_trying', 'en', 'Keep Trying!', 'Motivation to continue'),
('feedback.keep_trying', 'es', '¡Sigue Intentando!', 'Motivation to continue'),

-- Sistema de Estrellas
('stars.three', 'en', 'Perfect! 3 Stars!', '3 star achievement'),
('stars.three', 'es', '¡Perfecto! ¡3 Estrellas!', '3 star achievement'),
('stars.two', 'en', 'Great! 2 Stars!', '2 star achievement'),
('stars.two', 'es', '¡Genial! ¡2 Estrellas!', '2 star achievement'),
('stars.one', 'en', 'Good! 1 Star!', '1 star achievement'),
('stars.one', 'es', '¡Bien! ¡1 Estrella!', '1 star achievement'),

-- Navegación y Menús
('menu.home', 'en', 'Home', 'Home menu item'),
('menu.home', 'es', 'Inicio', 'Home menu item'),
('menu.progress', 'en', 'Progress', 'Progress menu item'),
('menu.progress', 'es', 'Progreso', 'Progress menu item'),
('menu.settings', 'en', 'Settings', 'Settings menu item'),
('menu.settings', 'es', 'Configuración', 'Settings menu item'),
('menu.parent_area', 'en', 'Parent Area', 'Parent area menu'),
('menu.parent_area', 'es', 'Área de Padres', 'Parent area menu'),

-- Freemium y Premium
('freemium.trial_remaining', 'en', 'Free Trial: {0} days left', 'Trial countdown'),
('freemium.trial_remaining', 'es', 'Prueba Gratis: {0} días restantes', 'Trial countdown'),
('freemium.lessons_remaining', 'en', '{0} lessons remaining today', 'Daily limit countdown'),
('freemium.lessons_remaining', 'es', '{0} lecciones restantes hoy', 'Daily limit countdown'),
('freemium.limit_reached', 'en', 'Daily limit reached! Upgrade for unlimited learning.', 'Limit reached message'),
('freemium.limit_reached', 'es', '¡Límite diario alcanzado! Mejora para aprendizaje ilimitado.', 'Limit reached message'),
('freemium.upgrade_now', 'en', 'Upgrade Now', 'Upgrade CTA'),
('freemium.upgrade_now', 'es', 'Mejorar Ahora', 'Upgrade CTA');

-- ================================================
-- LESSON CONTENT: Lecciones de ejemplo (Módulo Math Basic)
-- ================================================
INSERT OR REPLACE INTO lesson_content (lesson_code, module_id, title_key_en, title_key_es, instructions_key_en, instructions_key_es, sort_order, difficulty_level, estimated_duration, interactive_content_json, audio_assets_en, audio_assets_es, image_assets, learning_objectives, is_active) VALUES
-- Módulo Math Basic (id = 1)
('MATH001_L01', 1, 'lesson.math001.l01.title', 'lesson.math001.l01.title', 'lesson.math001.l01.instructions', 'lesson.math001.l01.instructions', 1, 1, 180, 
'{"type":"counting","range":{"min":1,"max":5},"objects":["apple","ball","star"],"interactions":["tap_count","drag_drop"]}',
'["math001_l01_intro.mp3","math001_l01_count.mp3","math001_l01_success.mp3"]',
'["math001_l01_intro.mp3","math001_l01_contar.mp3","math001_l01_exito.mp3"]',
'["objects/apple.png","objects/ball.png","objects/star.png","backgrounds/garden.png"]',
'["Count objects 1-5","Recognize numerals 1-5","One-to-one correspondence"]',
1),

('MATH001_L02', 1, 'lesson.math001.l02.title', 'lesson.math001.l02.title', 'lesson.math001.l02.instructions', 'lesson.math001.l02.instructions', 2, 1, 240,
'{"type":"number_recognition","numbers":[1,2,3,4,5],"activities":["match_numeral","trace_number","find_number"]}',
'["math001_l02_intro.mp3","math001_l02_numbers.mp3","math001_l02_trace.mp3"]',
'["math001_l02_intro.mp3","math001_l02_numeros.mp3","math001_l02_trazar.mp3"]',
'["numbers/1.png","numbers/2.png","numbers/3.png","numbers/4.png","numbers/5.png"]',
'["Recognize written numerals 1-5","Trace numbers","Match quantity to numeral"]',
1),

('MATH001_L03', 1, 'lesson.math001.l03.title', 'lesson.math001.l03.title', 'lesson.math001.l03.instructions', 'lesson.math001.l03.instructions', 3, 2, 300,
'{"type":"simple_addition","range":{"min":1,"max":3},"visual_aids":["blocks","fingers","dots"],"max_sum":5}',
'["math001_l03_intro.mp3","math001_l03_add.mp3","math001_l03_celebrate.mp3"]',
'["math001_l03_intro.mp3","math001_l03_sumar.mp3","math001_l03_celebrar.mp3"]',
'["manipulatives/blocks.png","manipulatives/fingers.png","manipulatives/dots.png"]',
'["Understand addition concept","Add numbers up to 5","Use visual aids for math"]',
1);

-- Localizaciones para lecciones Math
INSERT OR REPLACE INTO content_localization (localization_key, language, localized_text, context) VALUES
('lesson.math001.l01.title', 'en', 'Counting Fun 1-5', 'First counting lesson'),
('lesson.math001.l01.title', 'es', 'Contar Divertido 1-5', 'First counting lesson'),
('lesson.math001.l01.instructions', 'en', 'Count the objects and tap the correct number!', 'Counting instructions'),
('lesson.math001.l01.instructions', 'es', '¡Cuenta los objetos y toca el número correcto!', 'Counting instructions'),

('lesson.math001.l02.title', 'en', 'Number Recognition', 'Number recognition lesson'),
('lesson.math001.l02.title', 'es', 'Reconocer Números', 'Number recognition lesson'),
('lesson.math001.l02.instructions', 'en', 'Find and trace the numbers from 1 to 5!', 'Number tracing instructions'),
('lesson.math001.l02.instructions', 'es', '¡Encuentra y traza los números del 1 al 5!', 'Number tracing instructions'),

('lesson.math001.l03.title', 'en', 'My First Addition', 'First addition lesson'),
('lesson.math001.l03.title', 'es', 'Mi Primera Suma', 'First addition lesson'),
('lesson.math001.l03.instructions', 'en', 'Add the blocks together and find the total!', 'Addition instructions'),
('lesson.math001.l03.instructions', 'es', '¡Suma los bloques juntos y encuentra el total!', 'Addition instructions');

-- Registrar migración de datos iniciales
INSERT INTO schema_migrations (version, description) 
VALUES ('1.0.0-seed', 'Initial seed data for application');
```

## 4. Scripts de Mantenimiento

### 4.1 Script: `maintenance_cleanup.sql`

```sql
-- ================================================
-- EduPlayKids Maintenance Scripts
-- Limpieza y optimización periódica
-- ================================================

-- ================================================
-- LIMPIEZA DE DATOS ANTIGUOS
-- ================================================

-- Limpiar eventos de analytics antiguos (>30 días)
DELETE FROM analytics_events 
WHERE created_at < datetime('now', '-30 days');

-- Limpiar datos de daily_usage muy antiguos (>1 año)
DELETE FROM daily_usage 
WHERE usage_date < date('now', '-365 days');

-- ================================================
-- OPTIMIZACIÓN DE BASE DE DATOS
-- ================================================

-- Reconstruir estadísticas
ANALYZE;

-- Optimización automática
PRAGMA optimize;

-- Compactación de base de datos (usar con cuidado)
-- VACUUM;

-- ================================================
-- VERIFICACIÓN DE INTEGRIDAD
-- ================================================

-- Verificar integridad de la base de datos
PRAGMA integrity_check;

-- Verificar foreign keys
PRAGMA foreign_key_check;
```

### 4.2 Script: `backup_restore.sql`

```sql
-- ================================================
-- EduPlayKids Backup & Restore
-- Scripts para backup y restauración
-- ================================================

-- ================================================
-- EXPORTAR DATOS DE USUARIO (BACKUP)
-- ================================================

-- Crear tablas temporales para backup
CREATE TABLE IF NOT EXISTS backup_users AS SELECT * FROM users WHERE 1=0;
CREATE TABLE IF NOT EXISTS backup_user_progress AS SELECT * FROM user_progress WHERE 1=0;
CREATE TABLE IF NOT EXISTS backup_user_achievements AS SELECT * FROM user_achievements WHERE 1=0;
CREATE TABLE IF NOT EXISTS backup_daily_usage AS SELECT * FROM daily_usage WHERE 1=0;
CREATE TABLE IF NOT EXISTS backup_user_preferences AS SELECT * FROM user_preferences WHERE 1=0;

-- Función de backup (ejecutar por usuario específico)
-- Ejemplo para user_id = 1:
/*
INSERT INTO backup_users SELECT * FROM users WHERE id = 1;
INSERT INTO backup_user_progress SELECT * FROM user_progress WHERE user_id = 1;
INSERT INTO backup_user_achievements SELECT * FROM user_achievements WHERE user_id = 1;
INSERT INTO backup_daily_usage SELECT * FROM daily_usage WHERE user_id = 1;
INSERT INTO backup_user_preferences SELECT * FROM user_preferences WHERE user_id = 1;
*/

-- ================================================
-- RESTAURAR DATOS DE USUARIO
-- ================================================

-- Función de restauración (requiere datos en tablas backup_*)
/*
INSERT OR REPLACE INTO users SELECT * FROM backup_users;
INSERT OR REPLACE INTO user_progress SELECT * FROM backup_user_progress;
INSERT OR REPLACE INTO user_achievements SELECT * FROM backup_user_achievements;
INSERT OR REPLACE INTO daily_usage SELECT * FROM backup_daily_usage;
INSERT OR REPLACE INTO user_preferences SELECT * FROM backup_user_preferences;
*/

-- Limpiar tablas de backup después de restaurar
/*
DROP TABLE IF EXISTS backup_users;
DROP TABLE IF EXISTS backup_user_progress;
DROP TABLE IF EXISTS backup_user_achievements;
DROP TABLE IF EXISTS backup_daily_usage;
DROP TABLE IF EXISTS backup_user_preferences;
*/
```

## 5. Scripts de Migración Futuros

### 5.1 Plantilla para Nuevas Migraciones

```sql
-- ================================================
-- EduPlayKids Migration Template
-- Version: X.Y.Z
-- Description: [Descripción del cambio]
-- ================================================

-- Verificar versión actual antes de aplicar
SELECT version FROM schema_migrations ORDER BY applied_at DESC LIMIT 1;

-- ================================================
-- CAMBIOS DE SCHEMA
-- ================================================

-- Ejemplo: Agregar nueva columna
-- ALTER TABLE users ADD COLUMN avatar_path TEXT;

-- Ejemplo: Crear nueva tabla
-- CREATE TABLE new_feature (...);

-- Ejemplo: Crear nuevos índices
-- CREATE INDEX idx_new_feature ON new_table(column);

-- ================================================
-- MIGRACIÓN DE DATOS
-- ================================================

-- Ejemplo: Migrar datos existentes
-- UPDATE users SET avatar_path = 'default_avatar.png' WHERE avatar_path IS NULL;

-- ================================================
-- REGISTRO DE MIGRACIÓN
-- ================================================

INSERT INTO schema_migrations (version, description, rollback_sql) 
VALUES ('X.Y.Z', '[Descripción del cambio]', '[SQL para rollback]');

-- Verificar migración aplicada
SELECT * FROM schema_migrations WHERE version = 'X.Y.Z';
```

### 5.2 Ejemplo: Migración 1.1.0 - Multi-Device Support

```sql
-- ================================================
-- EduPlayKids Migration 1.1.0
-- Multi-Device Support (Futuro)
-- ================================================

-- Agregar device_id para preparar sync
ALTER TABLE users ADD COLUMN device_id TEXT DEFAULT (hex(randomblob(16)));

-- Nueva tabla para sync status
CREATE TABLE sync_status (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    table_name TEXT NOT NULL,
    record_id INTEGER NOT NULL,
    last_synced_at DATETIME,
    sync_hash TEXT,
    needs_sync BOOLEAN DEFAULT 1,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    
    UNIQUE(table_name, record_id)
);

CREATE INDEX idx_sync_status_needs_sync ON sync_status(needs_sync, table_name);

-- Trigger para marcar cambios que necesitan sync
CREATE TRIGGER mark_users_for_sync
    AFTER UPDATE ON users
    FOR EACH ROW
    BEGIN
        INSERT OR REPLACE INTO sync_status (table_name, record_id, needs_sync)
        VALUES ('users', NEW.id, 1);
    END;

-- Registrar migración
INSERT INTO schema_migrations (version, description, rollback_sql) 
VALUES ('1.1.0', 'Add multi-device sync preparation', 
        'DROP TRIGGER mark_users_for_sync; DROP TABLE sync_status; -- Note: Cannot easily remove column in SQLite');
```

## 6. Scripts de Verificación

### 6.1 Script: `verify_schema.sql`

```sql
-- ================================================
-- EduPlayKids Schema Verification
-- Verificación de integridad y consistencia
-- ================================================

-- ================================================
-- VERIFICAR ESTRUCTURA DE TABLAS
-- ================================================

-- Verificar que todas las tablas esperadas existan
SELECT 
    name,
    CASE WHEN name IN (
        'users', 'educational_modules', 'lesson_content', 'user_progress', 
        'user_achievements', 'daily_usage', 'user_preferences', 'app_settings',
        'analytics_events', 'content_localization', 'schema_migrations'
    ) THEN '✓ OK' ELSE '✗ MISSING' END as status
FROM sqlite_master 
WHERE type='table' 
ORDER BY name;

-- ================================================
-- VERIFICAR ÍNDICES CRÍTICOS
-- ================================================

-- Verificar índices esenciales
SELECT 
    name as index_name,
    tbl_name as table_name,
    CASE WHEN name LIKE 'idx_%' THEN '✓ OK' ELSE '? Check' END as status
FROM sqlite_master 
WHERE type='index' AND name NOT LIKE 'sqlite_%'
ORDER BY tbl_name, name;

-- ================================================
-- VERIFICAR DATOS BÁSICOS
-- ================================================

-- Verificar configuraciones críticas
SELECT 
    setting_key,
    setting_value,
    CASE 
        WHEN setting_key = 'database_version' AND setting_value = '1.0.0' THEN '✓ OK'
        WHEN setting_key IN ('daily_lesson_limit_free', 'trial_period_days') AND CAST(setting_value AS INTEGER) > 0 THEN '✓ OK'
        ELSE '? Check'
    END as status
FROM app_settings 
WHERE setting_key IN ('database_version', 'daily_lesson_limit_free', 'trial_period_days')
ORDER BY setting_key;

-- Verificar módulos básicos
SELECT 
    module_code,
    name_key_en,
    is_active,
    CASE WHEN is_active = 1 THEN '✓ Active' ELSE '⚠ Inactive' END as status
FROM educational_modules 
ORDER BY sort_order;

-- ================================================
-- VERIFICAR INTEGRIDAD REFERENCIAL
-- ================================================

-- Verificar foreign keys
PRAGMA foreign_key_check;

-- Verificar integridad general
PRAGMA integrity_check;

-- ================================================
-- ESTADÍSTICAS DE BASE DE DATOS
-- ================================================

SELECT 
    'Database Size (KB)' as metric,
    CAST(page_count * page_size / 1024.0 AS INTEGER) as value
FROM pragma_page_count(), pragma_page_size()

UNION ALL

SELECT 'Total Tables', COUNT(*) 
FROM sqlite_master WHERE type='table'

UNION ALL

SELECT 'Total Indexes', COUNT(*) 
FROM sqlite_master WHERE type='index' AND name NOT LIKE 'sqlite_%'

UNION ALL

SELECT 'Users Count', COUNT(*) FROM users

UNION ALL

SELECT 'Modules Count', COUNT(*) FROM educational_modules

UNION ALL

SELECT 'Lessons Count', COUNT(*) FROM lesson_content

UNION ALL

SELECT 'Localization Keys', COUNT(*) FROM content_localization;
```

## 7. Gestión de Migraciones en Código

### 7.1 Ejemplo en C# (Entity Framework Core)

```csharp
public class DatabaseMigrationService
{
    private readonly string _connectionString;
    private readonly ILogger<DatabaseMigrationService> _logger;
    
    public DatabaseMigrationService(string connectionString, ILogger<DatabaseMigrationService> logger)
    {
        _connectionString = connectionString;
        _logger = logger;
    }
    
    public async Task<bool> EnsureDatabaseCreatedAsync()
    {
        try
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            
            // Verificar si existe la tabla de migraciones
            var migrationTableExists = await CheckMigrationTableExistsAsync(connection);
            
            if (!migrationTableExists)
            {
                _logger.LogInformation("Creating database schema...");
                await ApplyMigrationAsync(connection, "001_initial_schema.sql");
                await ApplyMigrationAsync(connection, "002_create_triggers.sql");
                await ApplyMigrationAsync(connection, "003_create_views.sql");
                await ApplyMigrationAsync(connection, "004_seed_data.sql");
            }
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create database");
            return false;
        }
    }
    
    private async Task ApplyMigrationAsync(SqliteConnection connection, string migrationFile)
    {
        var sql = await File.ReadAllTextAsync(Path.Combine("Migrations", migrationFile));
        using var command = new SqliteCommand(sql, connection);
        await command.ExecuteNonQueryAsync();
        
        _logger.LogInformation($"Applied migration: {migrationFile}");
    }
}
```

---

**Documento Scripts de Migración - EduPlayKids v1.0**
**Fecha**: 16 de Septiembre, 2024
**Estado**: Production Ready - Complete Migration Scripts
**Database Version**: 1.0.0