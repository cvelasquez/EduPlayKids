# Especificaciones de Actividades - EduPlayKids

## Información del Documento
- **Proyecto**: EduPlayKids - Aplicación Educativa Móvil
- **Versión**: 1.0
- **Fecha**: Septiembre 2025
- **Autor**: Equipo de Contenido Educativo
- **Total Actividades**: 450+ actividades planificadas

---

## Resumen Ejecutivo

Este documento define las especificaciones técnicas, pedagógicas y de experiencia de usuario para todas las actividades educativas de EduPlayKids. Cada actividad está diseñada siguiendo principios de gamificación educativa, accesibilidad infantil y aprendizaje adaptativo, con especial atención al desarrollo cognitivo por edad y la progresión curricular estructurada.

---

## Estructura General de Actividades

### Metadatos Obligatorios
```yaml
# Identificación
activity_id: "AREA_NIVEL_UNIDAD_ACTIVIDAD"
title_es: "Título en español"
title_en: "English title"
description_es: "Descripción breve en español"
description_en: "Brief description in English"

# Clasificación Curricular
area: "mathematics|reading|science|art|logic|social_emotional"
domain: "subdomain_specific"
competence_code: "MAT.1.1.001"
curriculum_standard: "CCSS.MATH.K.CC.1"

# Configuración por Edad
age_group: "pre_k_3_4|kindergarten_5|grades_1_2"
difficulty_level: "easy|medium|hard"
estimated_duration: 300 # segundos
max_duration: 600 # segundos límite

# Configuración Técnica
activity_type: "recognition|production|interactive|assessment"
interaction_mode: "tap|drag_drop|trace|multi_touch|voice"
device_orientation: "portrait|landscape|both"
offline_capable: true
```

### Metadatos de Progresión
```yaml
# Prerequisites y Secuenciación
prerequisites: ["MAT.1.1.001", "MAT.1.1.002"]
unlocks: ["MAT.1.1.004"]
suggested_next: ["MAT.1.1.003"]

# Configuración de Dificultad Adaptativa
adaptive_difficulty: true
difficulty_adjustment_factors:
  - success_rate_threshold: 0.9  # Si >90% correcto, incrementar dificultad
  - failure_threshold: 0.3       # Si <30% correcto, reducir dificultad
  - time_threshold: 1.5          # Si toma >150% tiempo esperado

# Sistema de Puntuación
scoring_system:
  max_score: 100
  star_thresholds: [60, 80, 95]  # 1⭐, 2⭐, 3⭐
  perfect_bonus: 10              # Bonus por completar sin errores
  speed_bonus: 5                 # Bonus por completar rápido
```

---

## Tipos de Actividades por Metodología

### 1. 👁️ Actividades de Reconocimiento (Recognition)

#### 1.1 Selección Múltiple
```yaml
type: "multiple_choice"
question_format: "visual|audio|text"
options_count: 3  # Para Pre-K, 4 para K+
correct_answers: 1
randomize_options: true
show_images: true
```

**Ejemplo: Reconocer Números**
```yaml
activity_id: "MAT_PK_L1_U1_001"
title_es: "¿Cuál es el número 3?"
question_audio_es: "encuentra_numero_tres.mp3"
options:
  - image: "number_1.png"
    value: 1
    correct: false
  - image: "number_3.png"
    value: 3
    correct: true
  - image: "number_5.png"
    value: 5
    correct: false
feedback_correct: "¡Excelente! Encontraste el 3"
feedback_incorrect: "Intenta otra vez. El 3 parece como esto..."
```

#### 1.2 Emparejar (Matching)
```yaml
type: "matching"
pairs_count: 4  # Pre-K: 3-4, K: 4-6, G1-2: 6-8
matching_strategy: "one_to_one|one_to_many|category"
visual_connection: "lines|drag_drop|tap_sequence"
```

**Ejemplo: Emparejar Animales con Hábitats**
```yaml
activity_id: "CIE_K_L1_U2_003"
title_es: "Conecta cada animal con su hogar"
left_items:
  - id: "fish"
    image: "fish.png"
    audio: "fish_sound.mp3"
  - id: "bird"
    image: "bird.png"
    audio: "bird_sound.mp3"
right_items:
  - id: "water"
    image: "water_habitat.png"
    matches: ["fish"]
  - id: "tree"
    image: "tree_habitat.png"
    matches: ["bird"]
```

### 2. ✏️ Actividades de Producción (Production)

#### 2.1 Trazado (Tracing)
```yaml
type: "tracing"
trace_target: "letter|number|shape|line"
guide_dots: true
stroke_width: 8  # píxeles
tolerance: 20    # margen de error en píxeles
show_direction_arrows: true
```

**Ejemplo: Trazar Letra A**
```yaml
activity_id: "LEC_PK_L2_U1_005"
title_es: "Traza la letra A"
instruction_audio: "traza_letra_a.mp3"
trace_path:
  - type: "line"
    start: [100, 200]
    end: [150, 100]
    stroke_order: 1
  - type: "line"
    start: [150, 100]
    end: [200, 200]
    stroke_order: 2
  - type: "line"
    start: [125, 150]
    end: [175, 150]
    stroke_order: 3
success_criteria:
  accuracy_threshold: 0.8
  completion_threshold: 0.95
```

#### 2.2 Construcción (Building)
```yaml
type: "construction"
components: ["blocks", "shapes", "patterns"]
target_configuration: "specific|freeform|guided"
physics_enabled: false  # Para mayor control
snap_to_grid: true
```

**Ejemplo: Construir Patrón con Formas**
```yaml
activity_id: "LOG_K_L2_U3_007"
title_es: "Completa el patrón"
pattern_sequence: ["circle", "square", "circle", "square", "?", "?"]
available_pieces:
  - shape: "circle"
    color: "red"
    count: 3
  - shape: "square"
    color: "blue"
    count: 3
target_pattern: ["circle", "square", "circle", "square", "circle", "square"]
```

### 3. 🎮 Actividades Interactivas (Interactive)

#### 3.1 Arrastrar y Soltar (Drag & Drop)
```yaml
type: "drag_drop"
drag_items: "objects|words|numbers"
drop_zones: "containers|categories|sequences"
magnetic_snap: true
return_on_wrong: true
visual_feedback: "highlight|shadow|animation"
```

**Ejemplo: Clasificar por Color**
```yaml
activity_id: "ART_PK_L1_U1_002"
title_es: "Arrastra cada objeto a su color"
drag_items:
  - id: "apple"
    image: "red_apple.png"
    category: "red"
  - id: "banana"
    image: "yellow_banana.png"
    category: "yellow"
drop_zones:
  - id: "red_box"
    image: "red_container.png"
    accepts: ["red"]
  - id: "yellow_box"
    image: "yellow_container.png"
    accepts: ["yellow"]
```

#### 3.2 Secuenciación (Sequencing)
```yaml
type: "sequencing"
sequence_type: "temporal|logical|numerical|alphabetical"
items_count: 4  # Pre-K: 3-4, K: 4-5, G1-2: 5-7
randomize_start: true
show_numbers: false  # Para Pre-K
```

**Ejemplo: Ordenar Ciclo de Vida**
```yaml
activity_id: "CIE_G12_L1_U3_004"
title_es: "Ordena el ciclo de vida de la mariposa"
sequence_items:
  - id: "egg"
    image: "butterfly_egg.png"
    order: 1
    description: "Huevo"
  - id: "caterpillar"
    image: "caterpillar.png"
    order: 2
    description: "Oruga"
  - id: "chrysalis"
    image: "chrysalis.png"
    order: 3
    description: "Crisálida"
  - id: "butterfly"
    image: "butterfly.png"
    order: 4
    description: "Mariposa"
```

### 4. 📊 Actividades de Evaluación (Assessment)

#### 4.1 Quiz Adaptativo
```yaml
type: "adaptive_quiz"
question_pool_size: 10
questions_to_ask: 5
difficulty_adaptation: true
stop_on_mastery: true
mastery_threshold: 0.9
```

**Ejemplo: Evaluación de Números 1-10**
```yaml
activity_id: "MAT_K_L1_ASSESSMENT_001"
title_es: "¿Qué tan bien conoces los números?"
question_types:
  - "number_recognition"
  - "counting_objects"
  - "number_sequence"
difficulty_progression:
  easy: ["1-5_recognition", "count_to_3"]
  medium: ["6-10_recognition", "count_to_7"]
  hard: ["mixed_recognition", "count_to_10"]
```

---

## Especificaciones por Área de Conocimiento

### 🔢 MATEMÁTICAS (MAT)

#### Actividades de Números y Conteo

**MAT-001: Reconocimiento de Números**
```yaml
activity_family: "number_recognition"
variations: 45  # 15 por grupo de edad
progression:
  pre_k: [1-3, 1-5, 1-10]
  kindergarten: [1-10, 11-15, 16-20]
  grades_1_2: [1-20, 21-50, 51-100]

interaction_types:
  - "tap_correct_number"
  - "drag_number_to_position"
  - "trace_number_shape"
  - "count_and_select"

visual_elements:
  - number_fonts: ["Comic Sans", "OpenDyslexic", "Nunito Bold"]
  - colors: ["high_contrast", "colorful", "themed"]
  - backgrounds: ["plain", "themed", "contextual"]
```

**MAT-002: Conteo de Objetos**
```yaml
activity_family: "object_counting"
object_types: ["animals", "toys", "food", "shapes", "vehicles"]
count_ranges:
  pre_k: [1-5]
  kindergarten: [1-10, 6-15]
  grades_1_2: [10-20, 15-25]

interaction_modes:
  - "tap_to_count": "Tocar cada objeto para contar"
  - "point_and_count": "Señalar mientras cuenta en voz alta"
  - "group_and_count": "Agrupar objetos y contar grupos"

animations:
  count_feedback: "number_appears_above_object"
  completion: "objects_celebrate_dance"
  error: "gentle_shake_incorrect_objects"
```

#### Actividades de Operaciones Básicas

**MAT-003: Suma Simple**
```yaml
activity_family: "simple_addition"
number_ranges:
  kindergarten: [1+1, 2+2, 3+2, 4+1, 5+0]
  grades_1_2: [up_to_10, up_to_20]

representation_modes:
  - "concrete_objects": "Manzanas, bloques, juguetes"
  - "semi_concrete": "Dibujos de objetos"
  - "abstract": "Solo números"

scaffolding:
  visual_support: "show_objects_being_combined"
  audio_support: "step_by_step_narration"
  interactive_support: "drag_objects_together"
```

### 📚 LECTURA Y FONÉTICA (LEC)

#### Actividades de Conciencia Fonológica

**LEC-001: Identificación de Rimas**
```yaml
activity_family: "rhyme_identification"
word_pairs: 
  spanish: ["gato-pato", "casa-masa", "sol-col"]
  english: ["cat-hat", "sun-fun", "dog-log"]

difficulty_levels:
  easy: "identical_ending_sounds"
  medium: "similar_ending_sounds"
  hard: "internal_rhymes"

interaction_types:
  - "select_rhyming_pair"
  - "drag_rhyming_words_together"
  - "complete_rhyming_sentence"

audio_components:
  word_pronunciation: "clear_native_speaker"
  background_music: "soft_instrumental"
  sound_effects: "success_chime_failure_buzz"
```

**LEC-002: Reconocimiento de Letras**
```yaml
activity_family: "letter_recognition"
letter_sets:
  pre_k: ["A", "E", "I", "O", "U", "M", "P", "S"]
  kindergarten: "complete_alphabet"
  grades_1_2: "mixed_case_recognition"

presentation_modes:
  - "isolated_letter": "Letra sola en pantalla"
  - "letter_in_word": "Identificar en contexto"
  - "letter_sequence": "Ordenar alfabéticamente"

visual_styles:
  - "print_style": "Estilo de imprenta"
  - "handwriting_style": "Estilo manuscrito"
  - "decorative_style": "Con temas visuales"
```

#### Actividades de Decodificación

**LEC-003: Formación de Palabras**
```yaml
activity_family: "word_building"
word_complexity:
  kindergarten: ["CV", "CVC"]  # ma, sol
  grades_1_2: ["CVCC", "CCVC", "CVCC"]  # casa, plan, tren

scaffolding_levels:
  maximum: "show_letter_outlines_with_audio"
  medium: "show_first_letter_only"
  minimum: "audio_pronunciation_only"

success_feedback:
  visual: "word_becomes_picture"
  audio: "word_pronunciation_plus_definition"
  kinesthetic: "device_gentle_vibration"
```

### 🌱 CIENCIAS (CIE)

#### Actividades de Seres Vivos

**CIE-001: Clasificación de Animales**
```yaml
activity_family: "animal_classification"
classification_criteria:
  pre_k: ["domestic_vs_wild", "big_vs_small"]
  kindergarten: ["habitat_based", "diet_based"]
  grades_1_2: ["taxonomic_groups", "characteristics"]

animal_datasets:
  common_animals: ["dog", "cat", "bird", "fish", "elephant"]
  farm_animals: ["cow", "pig", "chicken", "sheep", "horse"]
  wild_animals: ["lion", "tiger", "bear", "monkey", "giraffe"]

interactive_elements:
  habitat_scenes: "animated_environments"
  animal_sounds: "realistic_audio_clips"
  movement_patterns: "characteristic_animations"
```

### 🎨 ARTE Y CREATIVIDAD (ART)

#### Actividades de Color

**ART-001: Teoría del Color**
```yaml
activity_family: "color_theory"
color_concepts:
  pre_k: ["primary_colors", "color_names"]
  kindergarten: ["secondary_colors", "warm_cool"]
  grades_1_2: ["color_mixing", "color_harmony"]

interaction_modes:
  - "color_mixing_lab": "Mezclar colores virtualmente"
  - "color_matching": "Emparejar colores iguales"
  - "color_sorting": "Clasificar por temperatura"

creative_elements:
  digital_paint: "finger_painting_simulation"
  palette_tools: "eyedropper_color_picker"
  save_artwork: "gallery_of_creations"
```

---

## Sistema de Feedback y Retroalimentación

### Feedback Inmediato

#### Feedback Positivo (Respuesta Correcta)
```yaml
visual_feedback:
  - animation: "sparkles_burst"
  - color_change: "green_highlight"
  - icon: "checkmark_with_bounce"
  - progress: "progress_bar_advance"

audio_feedback:
  - success_sound: "pleasant_chime"
  - voice_praise: ["¡Excelente!", "¡Muy bien!", "¡Correcto!"]
  - character_voice: "mascot_celebration"

haptic_feedback:
  - gentle_vibration: "success_pattern"
  - duration: "150ms"
```

#### Feedback Correctivo (Respuesta Incorrecta)
```yaml
visual_feedback:
  - animation: "gentle_shake"
  - color_change: "soft_red_highlight"
  - hint_reveal: "partial_answer_shown"

audio_feedback:
  - error_sound: "soft_buzz_non_punitive"
  - voice_encouragement: ["Intenta otra vez", "Casi lo tienes"]
  - hint_audio: "guiding_question"

educational_support:
  - show_correct_answer: "after_3_attempts"
  - provide_explanation: "why_this_is_correct"
  - offer_similar_practice: "reinforcement_activity"
```

### Feedback de Progreso

#### Durante la Actividad
```yaml
progress_indicators:
  - activity_progress: "1_of_5_questions"
  - time_remaining: "visual_timer_countdown"
  - lives_remaining: "hearts_or_stars"
  - current_score: "points_accumulated"

encouragement_triggers:
  - mid_activity: "¡Vas muy bien!"
  - struggling: "¡Tú puedes hacerlo!"
  - near_completion: "¡Ya casi terminas!"
```

#### Al Completar Actividad
```yaml
completion_summary:
  - final_score: "points_and_star_rating"
  - time_taken: "completion_time"
  - accuracy: "percentage_correct"
  - areas_mastered: "competencies_achieved"

celebration_elements:
  - success_animation: "confetti_and_fireworks"
  - mascot_celebration: "character_dance"
  - unlock_notification: "new_content_available"
  - progress_update: "overall_progress_advancement"
```

---

## Adaptación por Edad y Habilidad

### Pre-K (3-4 años) - Especificaciones

#### Limitaciones Cognitivas
- **Memoria de trabajo**: Máximo 2-3 elementos simultáneos
- **Atención sostenida**: 3-5 minutos máximo
- **Capacidad de seguir instrucciones**: 1-2 pasos simples
- **Reconocimiento de símbolos**: Prefieren imágenes a texto

#### Adaptaciones de Diseño
```yaml
interface_modifications:
  button_size: "minimum_60dp"
  font_size: "minimum_24sp"
  color_contrast: "maximum_contrast_7_1"
  animation_speed: "slow_and_predictable"

interaction_simplifications:
  - single_tap_only: "no_double_tap_or_long_press"
  - large_touch_targets: "accommodate_imprecise_motor_skills"
  - immediate_feedback: "no_delayed_responses"
  - error_forgiveness: "large_margin_for_error"

content_adaptations:
  - visual_dominance: "90_percent_visual_10_percent_text"
  - familiar_contexts: "home_family_pets_food"
  - repetitive_patterns: "consistent_interaction_patterns"
```

### Kindergarten (5 años) - Especificaciones

#### Capacidades Cognitivas
- **Memoria de trabajo**: 3-4 elementos
- **Atención sostenida**: 5-8 minutos
- **Seguir instrucciones**: 2-3 pasos secuenciales
- **Simbolismo**: Inicio de comprensión abstracta

#### Adaptaciones de Diseño
```yaml
complexity_increases:
  - multi_step_activities: "2_to_3_sequential_steps"
  - choice_options: "3_to_4_alternatives"
  - pattern_recognition: "simple_AB_patterns"
  - basic_reasoning: "cause_and_effect_relationships"

scaffolding_systems:
  - visual_cues: "arrows_and_highlighting"
  - audio_instructions: "clear_step_by_step_guidance"
  - progress_tracking: "visual_progress_indicators"
```

### Grados 1-2 (6-8 años) - Especificaciones

#### Capacidades Cognitivas Avanzadas
- **Memoria de trabajo**: 4-5 elementos
- **Atención sostenida**: 8-12 minutos
- **Seguir instrucciones**: 3-5 pasos complejos
- **Pensamiento abstracto**: Desarrollo de conceptos abstractos

#### Adaptaciones de Diseño
```yaml
advanced_interactions:
  - problem_solving: "multi_step_challenges"
  - strategy_development: "planning_ahead_activities"
  - metacognition: "reflect_on_learning_process"
  - transfer_skills: "apply_knowledge_new_contexts"

independence_support:
  - self_paced_learning: "student_controlled_progression"
  - choice_in_activities: "multiple_paths_to_mastery"
  - peer_comparison: "anonymous_progress_comparisons"
```

---

## Especificaciones Técnicas de Implementación

### Arquitectura de Actividades

#### Estructura de Archivos
```
activities/
├── metadata/
│   ├── activity_MAT_PK_L1_U1_001.yaml
│   └── activity_LEC_K_L2_U3_005.yaml
├── assets/
│   ├── images/
│   │   ├── numbers/
│   │   ├── letters/
│   │   └── objects/
│   ├── audio/
│   │   ├── instructions/
│   │   ├── pronunciations/
│   │   └── feedback/
│   └── animations/
│       ├── lottie/
│       └── sprite_sheets/
└── logic/
    ├── scoring_algorithms/
    ├── adaptive_difficulty/
    └── progress_tracking/
```

#### Configuración de Actividad
```csharp
public class ActivityConfiguration
{
    public string ActivityId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    
    public AgeGroup TargetAgeGroup { get; set; }
    public DifficultyLevel Difficulty { get; set; }
    public ActivityType Type { get; set; }
    
    public List<string> Prerequisites { get; set; }
    public List<string> Unlocks { get; set; }
    
    public ScoringConfiguration Scoring { get; set; }
    public FeedbackConfiguration Feedback { get; set; }
    public AdaptiveConfiguration Adaptive { get; set; }
}
```

### Métricas y Analíticas

#### Eventos de Seguimiento
```yaml
activity_events:
  - activity_started
  - question_answered
  - hint_requested
  - activity_paused
  - activity_completed
  - activity_abandoned

performance_metrics:
  - response_time_per_question
  - accuracy_rate
  - hints_used
  - attempts_per_question
  - total_duration
  - completion_rate

engagement_metrics:
  - replay_frequency
  - voluntary_continuation
  - time_spent_beyond_minimum
  - exploration_behaviors
```

#### Dashboard de Analíticas
```csharp
public class ActivityAnalytics
{
    public string ActivityId { get; set; }
    public DateTime SessionStart { get; set; }
    public DateTime SessionEnd { get; set; }
    
    public int TotalQuestions { get; set; }
    public int CorrectAnswers { get; set; }
    public int HintsUsed { get; set; }
    
    public TimeSpan AverageResponseTime { get; set; }
    public List<QuestionResponse> DetailedResponses { get; set; }
    
    public double AccuracyRate => (double)CorrectAnswers / TotalQuestions;
    public int StarRating => CalculateStarRating();
}
```

---

## Localización y Multiculturalidad

### Adaptación Cultural

#### Contenido Culturalmente Relevante
```yaml
cultural_adaptations:
  food_examples: ["tacos", "empanadas", "arroz_con_pollo"]
  family_structures: ["nuclear", "extended", "single_parent"]
  celebrations: ["dia_de_los_muertos", "navidad", "quinceañera"]
  music_styles: ["mariachi", "salsa", "cumbia"]

representation:
  character_diversity: "multiple_ethnicities_abilities_family_types"
  name_diversity: "common_hispanic_names_pronunciation_guide"
  context_diversity: "urban_rural_suburban_settings"
```

#### Bilingüismo
```yaml
language_switching:
  dynamic_language: "switch_during_activity"
  parallel_content: "same_activity_both_languages"
  progressive_introduction: "gradual_second_language_exposure"

audio_localization:
  native_speakers: "professional_voice_actors"
  regional_accents: "neutral_mexican_spanish_american_english"
  pronunciation_guides: "phonetic_breakdown_difficult_words"
```

---

## Control de Calidad y Testing

### Proceso de Validación de Actividades

#### Testing Técnico
1. **Funcionalidad**: Todas las interacciones funcionan correctamente
2. **Performance**: Carga rápida y respuesta fluida
3. **Compatibilidad**: Funciona en diferentes dispositivos Android
4. **Accesibilidad**: Cumple estándares WCAG 2.1 AA

#### Testing Pedagógico
1. **Alineación curricular**: Corresponde a objetivos de aprendizaje
2. **Progresión lógica**: Dificultad apropiada para el nivel
3. **Engagement**: Mantiene atención del grupo objetivo
4. **Efectividad**: Produce aprendizaje medible

#### Testing con Usuario Final
```yaml
testing_protocol:
  participants: "6_children_per_age_group"
  session_duration: "30_minutes_maximum"
  environment: "controlled_tablet_setup"
  metrics:
    - completion_rate
    - error_frequency
    - help_requests
    - engagement_indicators
    - learning_outcomes
```

### Criterios de Aprobación

#### Métricas Mínimas para Publicación
- **Completion Rate**: >80% de niños completan la actividad
- **Accuracy Rate**: >60% de respuestas correctas en primer intento
- **Engagement**: <10% de abandono antes del 50%
- **Technical**: 0 bugs críticos, <3 bugs menores
- **Performance**: Carga en <3 segundos, 60fps constante

---

*Especificaciones de Actividades para EduPlayKids v1.0 - Septiembre 2025*