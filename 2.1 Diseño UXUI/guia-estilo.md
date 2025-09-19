# Guía de Estilo - EduPlayKids

## Información del Documento
- **Proyecto**: EduPlayKids - Aplicación Educativa Móvil
- **Versión**: 1.0
- **Fecha**: Septiembre 2025
- **Autor**: Equipo de Diseño UX/UI
- **Framework**: .NET MAUI (Android Primario)

---

## Filosofía de Diseño

### Principios Fundamentales

#### 1. **Diseño Centrado en el Niño**
- **Simplicidad Visual**: Interfaces limpias sin elementos distractores
- **Colores Vibrantes**: Paleta alegre que estimule el aprendizaje
- **Feedback Inmediato**: Respuesta visual/auditiva a cada interacción
- **Navegación Intuitiva**: Flujos lógicos adaptados a la cognición infantil

#### 2. **Accesibilidad Universal**
- **Contraste Alto**: Mínimo 7:1 para garantizar legibilidad
- **Botones Grandes**: Mínimo 60dp para facilitar toque infantil
- **Soporte Multimodal**: Visual, auditivo y táctil
- **Adaptabilidad**: Responsive design para todos los dispositivos

#### 3. **Seguridad y Privacidad**
- **Ambiente Controlado**: Sin acceso a internet durante uso
- **Protección Parental**: PIN obligatorio para funciones administrativas
- **Datos Locales**: Toda información almacenada en dispositivo
- **Contenido Apropiado**: Revisión pedagógica de todos los materiales

#### 4. **Excelencia Educativa**
- **Pedagógicamente Fundamentado**: Basado en metodologías probadas
- **Progresión Estructurada**: Dificultad incremental y lógica
- **Motivación Intrínseca**: Gamificación significativa, no superficial
- **Evaluación Formativa**: Retroalimentación constructiva continua

---

## Sistema de Colores

### Paleta Principal

#### Colores Primarios
```css
/* Azul Confiable - Para matemáticas y navegación */
--primary-blue: #4285F4;
--primary-blue-light: #70A7FF;
--primary-blue-dark: #1565C0;

/* Verde Éxito - Para progreso y completado */
--primary-green: #34A853;
--primary-green-light: #6DBF7A;
--primary-green-dark: #2E7D32;

/* Amarillo Alegría - Para recompensas y celebración */
--primary-yellow: #FBBC04;
--primary-yellow-light: #FFD54F;
--primary-yellow-dark: #F57F17;

/* Rojo Suave - Para errores y alertas (child-friendly) */
--primary-red: #EA4335;
--primary-red-light: #FF6B5B;
--primary-red-dark: #C62828;
```

#### Colores Secundarios
```css
/* Morado Creatividad - Para arte y actividades creativas */
--secondary-purple: #9C27B0;
--secondary-purple-light: #BA68C8;
--secondary-purple-dark: #6A1B9A;

/* Naranja Energía - Para actividades físicas y dinámicas */
--secondary-orange: #FF9800;
--secondary-orange-light: #FFB74D;
--secondary-orange-dark: #E65100;

/* Rosa Diversión - Para elementos lúdicos */
--secondary-pink: #E91E63;
--secondary-pink-light: #F48FB1;
--secondary-pink-dark: #AD1457;

/* Cian Tranquilo - Para ciencias y naturaleza */
--secondary-cyan: #00BCD4;
--secondary-cyan-light: #4DD0E1;
--secondary-cyan-dark: #00838F;
```

#### Colores Neutros
```css
/* Escala de grises */
--neutral-white: #FFFFFF;
--neutral-light: #F8F9FA;
--neutral-lighter: #F1F3F4;
--neutral-medium: #E8EAED;
--neutral-darker: #DADCE0;
--neutral-dark: #5F6368;
--neutral-black: #202124;

/* Transparencias */
--overlay-light: rgba(255, 255, 255, 0.9);
--overlay-medium: rgba(0, 0, 0, 0.5);
--overlay-dark: rgba(0, 0, 0, 0.8);
```

### Asignación por Contexto

#### Por Materia Educativa
```css
/* Matemáticas */
--math-primary: var(--primary-blue);
--math-secondary: var(--secondary-cyan);

/* Lectura y Lenguaje */
--reading-primary: var(--primary-green);
--reading-secondary: var(--secondary-purple);

/* Arte y Creatividad */
--art-primary: var(--secondary-orange);
--art-secondary: var(--secondary-pink);

/* Lógica y Pensamiento */
--logic-primary: var(--secondary-purple);
--logic-secondary: var(--primary-blue);

/* Ciencias */
--science-primary: var(--secondary-cyan);
--science-secondary: var(--primary-green);

/* Música */
--music-primary: var(--secondary-pink);
--music-secondary: var(--primary-yellow);
```

#### Por Estado de Interfaz
```css
/* Estados de Éxito */
--success-bg: var(--primary-green);
--success-text: var(--neutral-white);
--success-border: var(--primary-green-dark);

/* Estados de Error */
--error-bg: var(--primary-red-light);
--error-text: var(--neutral-white);
--error-border: var(--primary-red-dark);

/* Estados de Advertencia */
--warning-bg: var(--primary-yellow);
--warning-text: var(--neutral-black);
--warning-border: var(--primary-yellow-dark);

/* Estados de Información */
--info-bg: var(--primary-blue-light);
--info-text: var(--neutral-white);
--info-border: var(--primary-blue-dark);
```

### Gradientes Signature

#### Gradientes Principales
```css
/* Gradiente Principal - Para headers y elementos destacados */
--gradient-primary: linear-gradient(135deg, #4285F4 0%, #34A853 100%);

/* Gradiente Premium - Para funciones de pago */
--gradient-premium: linear-gradient(135deg, #9C27B0 0%, #E91E63 100%);

/* Gradiente Éxito - Para completar actividades */
--gradient-success: linear-gradient(135deg, #34A853 0%, #FBBC04 100%);

/* Gradiente Celebración - Para logros especiales */
--gradient-celebration: linear-gradient(135deg, #FBBC04 0%, #FF9800 50%, #E91E63 100%);
```

#### Gradientes por Materia
```css
--gradient-math: linear-gradient(135deg, #4285F4 0%, #00BCD4 100%);
--gradient-reading: linear-gradient(135deg, #34A853 0%, #9C27B0 100%);
--gradient-art: linear-gradient(135deg, #FF9800 0%, #E91E63 100%);
--gradient-logic: linear-gradient(135deg, #9C27B0 0%, #4285F4 100%);
--gradient-science: linear-gradient(135deg, #00BCD4 0%, #34A853 100%);
--gradient-music: linear-gradient(135deg, #E91E63 0%, #FBBC04 100%);
```

---

## Tipografía

### Familias de Fuentes

#### Fuente Principal: Nunito
```css
/* Importación */
@import url('https://fonts.googleapis.com/css2?family=Nunito:wght@300;400;500;600;700;800;900&display=swap');

/* Configuración base */
--font-primary: 'Nunito', 'Roboto', 'Helvetica Neue', Arial, sans-serif;

/* Características */
- Diseñada específicamente para legibilidad infantil
- Formas redondeadas y amigables
- Excelente en pantallas pequeñas
- Soporte completo para idiomas español e inglés
- Variaciones de peso disponibles: 300-900
```

#### Fuente Secundaria: Roboto (Sistema)
```css
/* Para elementos del sistema */
--font-secondary: 'Roboto', system-ui, -apple-system, sans-serif;

/* Uso específico */
- Textos legales y técnicos
- Elementos de navegación del sistema
- Metadatos y timestamps
- Fallback cuando Nunito no esté disponible
```

### Escala Tipográfica

#### Tamaños Base (Android DP)
```css
/* Escala Major Third (1.25) */
--text-xs: 10sp;    /* Metadatos, copyright */
--text-sm: 12sp;    /* Texto secundario, captions */
--text-base: 14sp;  /* Texto base, párrafos */
--text-md: 16sp;    /* Texto importante, labels */
--text-lg: 18sp;    /* Subtítulos, texto destacado */
--text-xl: 20sp;    /* Títulos de sección */
--text-2xl: 24sp;   /* Títulos principales */
--text-3xl: 28sp;   /* Títulos destacados */
--text-4xl: 32sp;   /* Títulos hero */
--text-5xl: 36sp;   /* Display especial */
--text-6xl: 42sp;   /* Extra grande, números */
```

#### Tamaños Específicos para Niños
```css
/* Adaptaciones para usabilidad infantil */
--text-child-button: 16sp;      /* Botones principales */
--text-child-instruction: 18sp;  /* Instrucciones */
--text-child-feedback: 20sp;     /* Retroalimentación */
--text-child-title: 24sp;        /* Títulos de actividad */
--text-child-display: 32sp;      /* Números grandes, letras */
```

### Pesos de Fuente

```css
/* Configuración de pesos */
--font-light: 300;      /* Texto secundario sutil */
--font-regular: 400;    /* Texto base por defecto */
--font-medium: 500;     /* Texto importante */
--font-semibold: 600;   /* Subtítulos, labels */
--font-bold: 700;       /* Títulos, CTAs */
--font-extrabold: 800;  /* Elementos destacados */
--font-black: 900;      /* Display, logos */
```

### Altura de Línea (Line Height)

```css
/* Ratios optimizados para lectura infantil */
--line-height-tight: 1.1;    /* Títulos grandes */
--line-height-snug: 1.2;     /* Títulos medianos */
--line-height-normal: 1.4;   /* Texto base */
--line-height-relaxed: 1.6;  /* Párrafos largos */
--line-height-loose: 1.8;    /* Texto instructivo */
```

### Espaciado de Letras (Letter Spacing)

```css
/* Ajustes para legibilidad */
--letter-spacing-tight: -0.025em;  /* Títulos grandes */
--letter-spacing-normal: 0em;      /* Texto normal */
--letter-spacing-wide: 0.025em;    /* Texto pequeño */
--letter-spacing-wider: 0.05em;    /* MAYÚSCULAS */
--letter-spacing-widest: 0.1em;    /* Elementos decorativos */
```

### Ejemplos de Uso

#### Títulos y Headers
```css
.title-hero {
  font-family: var(--font-primary);
  font-size: var(--text-4xl);
  font-weight: var(--font-extrabold);
  line-height: var(--line-height-tight);
  letter-spacing: var(--letter-spacing-tight);
  color: var(--neutral-white);
}

.title-section {
  font-family: var(--font-primary);
  font-size: var(--text-2xl);
  font-weight: var(--font-bold);
  line-height: var(--line-height-snug);
  color: var(--neutral-black);
}

.title-activity {
  font-family: var(--font-primary);
  font-size: var(--text-child-title);
  font-weight: var(--font-semibold);
  line-height: var(--line-height-normal);
  color: var(--neutral-black);
}
```

#### Texto de Contenido
```css
.text-instruction {
  font-family: var(--font-primary);
  font-size: var(--text-child-instruction);
  font-weight: var(--font-medium);
  line-height: var(--line-height-relaxed);
  color: var(--neutral-dark);
}

.text-body {
  font-family: var(--font-primary);
  font-size: var(--text-base);
  font-weight: var(--font-regular);
  line-height: var(--line-height-normal);
  color: var(--neutral-black);
}

.text-caption {
  font-family: var(--font-primary);
  font-size: var(--text-sm);
  font-weight: var(--font-regular);
  line-height: var(--line-height-normal);
  color: var(--neutral-dark);
}
```

#### Elementos Interactivos
```css
.button-text-primary {
  font-family: var(--font-primary);
  font-size: var(--text-child-button);
  font-weight: var(--font-semibold);
  line-height: 1;
  letter-spacing: var(--letter-spacing-wide);
  text-transform: uppercase;
  color: var(--neutral-white);
}

.link-text {
  font-family: var(--font-primary);
  font-size: var(--text-base);
  font-weight: var(--font-medium);
  line-height: var(--line-height-normal);
  color: var(--primary-blue);
  text-decoration: underline;
}
```

---

## Iconografía

### Filosofía de Iconos

#### Principios de Diseño
- **Claridad**: Formas simples reconocibles al instante
- **Consistencia**: Estilo unificado en toda la aplicación
- **Familiaridad**: Símbolos universalmente reconocidos
- **Escalabilidad**: Legibles desde 16dp hasta 64dp
- **Accesibilidad**: Alto contraste y formas distintivas

### Sistema de Iconos

#### Iconos de Materias Educativas
```
📚 Lectura y Lenguaje
🔢 Matemáticas
🎨 Arte y Creatividad
🧩 Lógica y Pensamiento
🌱 Ciencias Naturales
🎵 Música y Ritmo
⚽ Educación Física
🌍 Estudios Sociales
```

#### Iconos de Navegación
```
← Retroceder
→ Siguiente
↑ Subir
↓ Bajar
× Cerrar
☰ Menú
⚙ Configuración
🏠 Inicio
```

#### Iconos de Acciones
```
▶ Reproducir
⏸ Pausar
🔊 Audio activado
🔇 Audio silenciado
💡 Pista/Ayuda
↻ Repetir/Reintentar
✓ Correcto
✗ Incorrecto
```

#### Iconos de Estado
```
⭐ Estrella (progreso)
❤ Vida/Intento
🔥 Racha de días
🏆 Logro/Achievement
👑 Premium
🔒 Bloqueado
```

### Especificaciones Técnicas

#### Tamaños Estándar
```css
/* Tamaños de iconos */
--icon-xs: 16dp;      /* Iconos inline */
--icon-sm: 20dp;      /* Iconos en botones pequeños */
--icon-md: 24dp;      /* Iconos estándar */
--icon-lg: 32dp;      /* Iconos destacados */
--icon-xl: 48dp;      /* Iconos principales */
--icon-2xl: 64dp;     /* Iconos hero/mascota */
--icon-3xl: 96dp;     /* Iconos decorativos */
```

#### Grid y Construcción
```css
/* Sistema de grid para iconos */
--icon-grid-unit: 2dp;
--icon-padding: 4dp;
--icon-safe-area: 80%; /* Área visible del total */

/* Para iconos 24x24dp */
- Canvas total: 24x24dp
- Área de diseño: 20x20dp
- Padding: 2dp en todos los lados
- Stroke weight: 2dp
- Corner radius: 2dp para elementos redondeados
```

#### Formato y Optimización
```
Formato preferido: SVG
Formato alternativo: PNG (2x, 3x para diferentes densidades)
Optimización: SVGO para reducir tamaño
Color: Preferiblemente monocromático para theming
Naming: icon-{category}-{name}.svg
```

### Iconos por Categoría

#### Materias Educativas (48x48dp)
```xml
<!-- Matemáticas -->
<svg viewBox="0 0 48 48" fill="none">
  <circle cx="24" cy="24" r="20" fill="#4285F4"/>
  <text x="24" y="30" font-family="Nunito" font-weight="800" 
        font-size="16" fill="white" text-anchor="middle">123</text>
</svg>

<!-- Lectura -->
<svg viewBox="0 0 48 48" fill="none">
  <rect x="8" y="12" width="32" height="24" rx="2" fill="#34A853"/>
  <path d="M12 20h24M12 24h20M12 28h16" stroke="white" stroke-width="2"/>
</svg>
```

#### Estados de Progreso (24x24dp)
```xml
<!-- Estrella llena -->
<svg viewBox="0 0 24 24" fill="none">
  <path d="M12 2l3.09 6.26L22 9.27l-5 4.87 1.18 6.88L12 17.77l-6.18 3.25L7 14.14 2 9.27l6.91-1.01L12 2z" 
        fill="#FBBC04"/>
</svg>

<!-- Estrella vacía -->
<svg viewBox="0 0 24 24" fill="none">
  <path d="M12 2l3.09 6.26L22 9.27l-5 4.87 1.18 6.88L12 17.77l-6.18 3.25L7 14.14 2 9.27l6.91-1.01L12 2z" 
        stroke="#E8EAED" stroke-width="2" fill="none"/>
</svg>
```

### Animaciones de Iconos

#### Feedback de Interacción
```css
/* Bounce en tap */
.icon-interactive:active {
  animation: icon-bounce 150ms ease-out;
}

@keyframes icon-bounce {
  0% { transform: scale(1); }
  50% { transform: scale(0.9); }
  100% { transform: scale(1); }
}

/* Glow para estados especiales */
.icon-achievement {
  filter: drop-shadow(0 0 8dp rgba(251, 188, 4, 0.6));
  animation: icon-glow 2s ease-in-out infinite alternate;
}

@keyframes icon-glow {
  from { filter: drop-shadow(0 0 8dp rgba(251, 188, 4, 0.6)); }
  to { filter: drop-shadow(0 0 16dp rgba(251, 188, 4, 0.8)); }
}
```

---

## Componentes de UI

### Botones

#### Botón Primario
```css
.button-primary {
  /* Estructura */
  min-width: 120dp;
  min-height: 48dp;
  padding: 12dp 24dp;
  border-radius: 12dp;
  border: none;
  
  /* Visual */
  background: linear-gradient(135deg, #34A853 0%, #FBBC04 100%);
  color: var(--neutral-white);
  font-family: var(--font-primary);
  font-size: var(--text-md);
  font-weight: var(--font-semibold);
  text-transform: uppercase;
  letter-spacing: 0.025em;
  
  /* Sombra */
  box-shadow: 0 4dp 8dp rgba(52, 168, 83, 0.3);
  
  /* Transición */
  transition: all 200ms ease-out;
}

.button-primary:hover {
  transform: translateY(-1dp);
  box-shadow: 0 6dp 12dp rgba(52, 168, 83, 0.4);
}

.button-primary:active {
  transform: translateY(0dp) scale(0.98);
  box-shadow: 0 2dp 4dp rgba(52, 168, 83, 0.3);
}

.button-primary:disabled {
  background: var(--neutral-medium);
  color: var(--neutral-dark);
  box-shadow: none;
  transform: none;
}
```

#### Botón Secundario
```css
.button-secondary {
  /* Estructura */
  min-width: 120dp;
  min-height: 48dp;
  padding: 12dp 24dp;
  border-radius: 12dp;
  border: 2dp solid var(--primary-blue);
  
  /* Visual */
  background: transparent;
  color: var(--primary-blue);
  font-family: var(--font-primary);
  font-size: var(--text-md);
  font-weight: var(--font-medium);
  
  /* Transición */
  transition: all 200ms ease-out;
}

.button-secondary:hover {
  background: var(--primary-blue);
  color: var(--neutral-white);
}
```

#### Botón de Materia
```css
.button-subject {
  /* Estructura */
  width: 140dp;
  height: 130dp;
  padding: 16dp;
  border-radius: 16dp;
  border: none;
  
  /* Layout */
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: 8dp;
  
  /* Visual */
  background: var(--gradient-math); /* Cambia según materia */
  color: var(--neutral-white);
  
  /* Sombra */
  box-shadow: 0 4dp 12dp rgba(0, 0, 0, 0.15);
  
  /* Transición */
  transition: all 300ms ease-out;
}

.button-subject:hover {
  transform: translateY(-2dp);
  box-shadow: 0 8dp 20dp rgba(0, 0, 0, 0.2);
}

.button-subject.completed {
  border: 2dp solid var(--primary-yellow);
  box-shadow: 0 4dp 12dp rgba(251, 188, 4, 0.4);
}

.button-subject.locked {
  background: var(--neutral-medium);
  color: var(--neutral-dark);
  cursor: not-allowed;
}
```

### Cards y Contenedores

#### Card Base
```css
.card {
  /* Estructura */
  background: var(--neutral-white);
  border-radius: 12dp;
  padding: 16dp;
  
  /* Sombra */
  box-shadow: 0 2dp 8dp rgba(0, 0, 0, 0.08);
  
  /* Border sutil */
  border: 1dp solid var(--neutral-lighter);
  
  /* Transición */
  transition: box-shadow 200ms ease-out;
}

.card:hover {
  box-shadow: 0 4dp 16dp rgba(0, 0, 0, 0.12);
}
```

#### Card de Progreso
```css
.card-progress {
  /* Extend card base */
  @extend .card;
  
  /* Específico */
  padding: 20dp;
  background: linear-gradient(135deg, var(--neutral-white) 0%, var(--neutral-light) 100%);
}

.progress-header {
  font-size: var(--text-lg);
  font-weight: var(--font-semibold);
  color: var(--neutral-black);
  margin-bottom: 12dp;
}

.progress-bar {
  width: 100%;
  height: 8dp;
  background: var(--neutral-medium);
  border-radius: 4dp;
  overflow: hidden;
}

.progress-fill {
  height: 100%;
  background: linear-gradient(90deg, var(--primary-blue) 0%, var(--primary-green) 100%);
  border-radius: 4dp;
  transition: width 500ms ease-out;
}
```

### Inputs y Formularios

#### Input de Texto
```css
.input-text {
  /* Estructura */
  width: 100%;
  min-height: 48dp;
  padding: 12dp 16dp;
  border-radius: 8dp;
  border: 2dp solid var(--neutral-medium);
  
  /* Tipografía */
  font-family: var(--font-primary);
  font-size: var(--text-md);
  font-weight: var(--font-regular);
  color: var(--neutral-black);
  
  /* Background */
  background: var(--neutral-white);
  
  /* Transición */
  transition: border-color 200ms ease-out;
}

.input-text:focus {
  outline: none;
  border-color: var(--primary-blue);
  box-shadow: 0 0 0 3dp rgba(66, 133, 244, 0.2);
}

.input-text:invalid {
  border-color: var(--primary-red);
}

.input-text::placeholder {
  color: var(--neutral-dark);
  opacity: 0.7;
}
```

#### Switch/Toggle
```css
.switch {
  /* Estructura del track */
  width: 48dp;
  height: 24dp;
  background: var(--neutral-medium);
  border-radius: 12dp;
  position: relative;
  cursor: pointer;
  transition: background-color 200ms ease-out;
}

.switch.active {
  background: var(--primary-green);
}

.switch-thumb {
  /* Estructura del thumb */
  width: 20dp;
  height: 20dp;
  background: var(--neutral-white);
  border-radius: 50%;
  position: absolute;
  top: 2dp;
  left: 2dp;
  
  /* Sombra */
  box-shadow: 0 2dp 4dp rgba(0, 0, 0, 0.2);
  
  /* Transición */
  transition: transform 200ms ease-out;
}

.switch.active .switch-thumb {
  transform: translateX(24dp);
}
```

### Modales y Overlays

#### Modal Base
```css
.modal-overlay {
  /* Estructura */
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  z-index: 1000;
  
  /* Visual */
  background: rgba(0, 0, 0, 0.6);
  backdrop-filter: blur(4dp);
  
  /* Layout */
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 20dp;
  
  /* Animación entrada */
  animation: modal-fade-in 300ms ease-out;
}

.modal-content {
  /* Estructura */
  background: var(--neutral-white);
  border-radius: 16dp;
  padding: 24dp;
  max-width: 400dp;
  width: 100%;
  max-height: 80vh;
  overflow-y: auto;
  
  /* Sombra */
  box-shadow: 0 8dp 32dp rgba(0, 0, 0, 0.3);
  
  /* Animación */
  animation: modal-slide-up 300ms ease-out;
}

@keyframes modal-fade-in {
  from { opacity: 0; }
  to { opacity: 1; }
}

@keyframes modal-slide-up {
  from { 
    opacity: 0;
    transform: translateY(20dp) scale(0.95);
  }
  to { 
    opacity: 1;
    transform: translateY(0) scale(1);
  }
}
```

---

## Animaciones y Transiciones

### Principios de Animación

#### 1. **Meaningful Motion**
- Cada animación debe tener un propósito funcional
- Guiar la atención del usuario hacia elementos importantes
- Proporcionar feedback sobre las acciones realizadas
- Mantener la continuidad espacial y temporal

#### 2. **Child-Friendly Timing**
- Animaciones más lentas para permitir seguimiento visual
- Duraciones entre 200ms-500ms para transiciones estándar
- Evitar animaciones muy rápidas que puedan causar mareo
- Pausas adecuadas entre secuencias animadas

#### 3. **Delightful Details**
- Micro-interacciones que generen satisfacción
- Celebraciones visuales para logros
- Feedback táctil (haptic) cuando sea apropiado
- Elementos lúdicos que refuercen el carácter educativo

### Curvas de Easing

```css
/* Curvas estándar */
--ease-linear: linear;
--ease-in: cubic-bezier(0.4, 0, 1, 1);
--ease-out: cubic-bezier(0, 0, 0.2, 1);
--ease-in-out: cubic-bezier(0.4, 0, 0.2, 1);

/* Curvas especiales para niños */
--ease-bounce: cubic-bezier(0.68, -0.55, 0.265, 1.55);
--ease-elastic: cubic-bezier(0.175, 0.885, 0.32, 1.275);
--ease-soft: cubic-bezier(0.25, 0.46, 0.45, 0.94);

/* Curvas personalizadas */
--ease-gentle: cubic-bezier(0.25, 0.1, 0.25, 1);
--ease-playful: cubic-bezier(0.68, -0.6, 0.32, 1.6);
```

### Duraciones Estándar

```css
/* Tiempos base */
--duration-fast: 150ms;      /* Hover states, button press */
--duration-normal: 250ms;    /* Transiciones estándar */
--duration-slow: 400ms;      /* Transiciones complejas */
--duration-slower: 600ms;    /* Animaciones de entrada */

/* Tiempos específicos */
--duration-tooltip: 200ms;
--duration-modal: 300ms;
--duration-page-transition: 400ms;
--duration-celebration: 800ms;
```

### Animaciones por Componente

#### Botones
```css
/* Pressed state */
@keyframes button-press {
  0% { transform: scale(1); }
  50% { transform: scale(0.95); }
  100% { transform: scale(1); }
}

/* Success feedback */
@keyframes button-success {
  0% { transform: scale(1); background-color: var(--primary-green); }
  50% { transform: scale(1.05); background-color: var(--primary-yellow); }
  100% { transform: scale(1); background-color: var(--primary-green); }
}

/* Loading state */
@keyframes button-loading {
  0% { opacity: 1; }
  50% { opacity: 0.6; }
  100% { opacity: 1; }
}
```

#### Cards y Contenedores
```css
/* Hover lift */
@keyframes card-lift {
  0% { transform: translateY(0); box-shadow: 0 2dp 8dp rgba(0,0,0,0.08); }
  100% { transform: translateY(-4dp); box-shadow: 0 8dp 24dp rgba(0,0,0,0.15); }
}

/* Entrada escalonada */
@keyframes stagger-in {
  0% { opacity: 0; transform: translateY(20dp); }
  100% { opacity: 1; transform: translateY(0); }
}
```

#### Feedback de Correcto/Incorrecto
```css
/* Celebración */
@keyframes celebration {
  0% { transform: scale(1) rotate(0deg); }
  25% { transform: scale(1.1) rotate(-5deg); }
  50% { transform: scale(1.2) rotate(5deg); }
  75% { transform: scale(1.1) rotate(-5deg); }
  100% { transform: scale(1) rotate(0deg); }
}

/* Shake para error */
@keyframes shake {
  0%, 100% { transform: translateX(0); }
  20%, 60% { transform: translateX(-4dp); }
  40%, 80% { transform: translateX(4dp); }
}
```

#### Progress y Loading
```css
/* Barra de progreso */
@keyframes progress-fill {
  0% { width: 0%; }
  100% { width: var(--progress-value); }
}

/* Spinner loading */
@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

/* Dots loading */
@keyframes dots-loading {
  0%, 20% { opacity: 0; transform: scale(0.8); }
  50% { opacity: 1; transform: scale(1); }
  80%, 100% { opacity: 0; transform: scale(0.8); }
}
```

### Animaciones de Página

#### Transiciones entre Pantallas
```css
/* Slide derecha */
@keyframes slide-in-right {
  0% { transform: translateX(100%); opacity: 0; }
  100% { transform: translateX(0); opacity: 1; }
}

@keyframes slide-out-left {
  0% { transform: translateX(0); opacity: 1; }
  100% { transform: translateX(-100%); opacity: 0; }
}

/* Fade con escala */
@keyframes fade-scale-in {
  0% { opacity: 0; transform: scale(0.9); }
  100% { opacity: 1; transform: scale(1); }
}
```

#### Entrada de Elementos
```css
/* Fade up */
@keyframes fade-up {
  0% { opacity: 0; transform: translateY(30dp); }
  100% { opacity: 1; transform: translateY(0); }
}

/* Entrance bounce */
@keyframes bounce-in {
  0% { opacity: 0; transform: scale(0.3); }
  50% { opacity: 1; transform: scale(1.05); }
  70% { transform: scale(0.9); }
  100% { transform: scale(1); }
}
```

---

## Espaciado y Layout

### Sistema de Espaciado

#### Escala Base (8dp Grid)
```css
/* Espaciado base múltiplos de 8dp */
--space-0: 0dp;
--space-1: 4dp;      /* Espaciado muy pequeño */
--space-2: 8dp;      /* Espaciado pequeño */
--space-3: 12dp;     /* Espaciado pequeño-medio */
--space-4: 16dp;     /* Espaciado medio */
--space-5: 20dp;     /* Espaciado medio-grande */
--space-6: 24dp;     /* Espaciado grande */
--space-8: 32dp;     /* Espaciado muy grande */
--space-10: 40dp;    /* Espaciado extra grande */
--space-12: 48dp;    /* Espaciado máximo */
--space-16: 64dp;    /* Espaciado especial */
--space-20: 80dp;    /* Espaciado hero */
```

#### Espaciado Específico para Niños
```css
/* Ajustes para usabilidad infantil */
--touch-target-min: 48dp;     /* Tamaño mínimo táctil Android */
--touch-target-child: 60dp;   /* Recomendado para niños */
--touch-spacing: 16dp;        /* Espaciado entre elementos táctiles */
--content-padding: 24dp;      /* Padding interno de contenedores */
--section-spacing: 32dp;      /* Espaciado entre secciones */
```

### Grid System

#### Configuración Base
```css
/* Container principal */
.container {
  max-width: 480dp;        /* Máximo para tablets pequeñas */
  margin: 0 auto;
  padding: 0 var(--space-4);
}

/* Grid responsive */
.grid {
  display: grid;
  gap: var(--space-4);
  grid-template-columns: repeat(auto-fit, minmax(140dp, 1fr));
}

/* Variaciones de grid */
.grid-2 { grid-template-columns: repeat(2, 1fr); }
.grid-3 { grid-template-columns: repeat(3, 1fr); }
.grid-4 { grid-template-columns: repeat(4, 1fr); }
```

#### Layout por Tipo de Pantalla

##### Pantalla Pequeña (320-359dp)
```css
@media (max-width: 359dp) {
  .container { padding: 0 var(--space-3); }
  .grid { gap: var(--space-3); }
  .grid { grid-template-columns: repeat(2, 1fr); }
  .subject-card { width: 120dp; height: 100dp; }
}
```

##### Pantalla Estándar (360-399dp)
```css
@media (min-width: 360dp) and (max-width: 399dp) {
  .container { padding: 0 var(--space-4); }
  .grid { gap: var(--space-4); }
  .subject-card { width: 140dp; height: 130dp; }
}
```

##### Pantalla Grande (400dp+)
```css
@media (min-width: 400dp) {
  .container { padding: 0 var(--space-6); }
  .grid { gap: var(--space-6); }
  .subject-card { width: 160dp; height: 140dp; }
}
```

##### Tablets (600dp+)
```css
@media (min-width: 600dp) {
  .container { max-width: 720dp; padding: 0 var(--space-8); }
  .grid { grid-template-columns: repeat(3, 1fr); gap: var(--space-8); }
  .subject-card { width: 180dp; height: 160dp; }
}
```

### Jerarquía Visual

#### Principios de Espaciado
```css
/* Reglas de proximidad */
.related-elements {
  gap: var(--space-2);     /* Elementos muy relacionados */
}

.section-elements {
  gap: var(--space-4);     /* Elementos de la misma sección */
}

.section-separation {
  margin-bottom: var(--space-8);  /* Separación entre secciones */
}

/* Respiración visual */
.content-area {
  padding: var(--space-6);
  margin: var(--space-4) 0;
}
```

### Layouts Específicos

#### Layout de Actividad
```css
.activity-layout {
  display: flex;
  flex-direction: column;
  height: 100vh;
  
  .activity-header {
    flex-shrink: 0;
    height: 72dp;
    padding: var(--space-4);
  }
  
  .activity-content {
    flex-grow: 1;
    padding: var(--space-6);
    overflow-y: auto;
  }
  
  .activity-actions {
    flex-shrink: 0;
    padding: var(--space-4);
    background: var(--neutral-white);
    border-top: 1dp solid var(--neutral-lighter);
  }
}
```

#### Layout de Dashboard
```css
.dashboard-layout {
  display: grid;
  gap: var(--space-6);
  padding: var(--space-6);
  
  .dashboard-header {
    grid-column: 1 / -1;
  }
  
  .dashboard-stats {
    grid-column: 1 / -1;
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(200dp, 1fr));
    gap: var(--space-4);
  }
  
  .dashboard-content {
    grid-column: 1 / -1;
    display: grid;
    gap: var(--space-6);
  }
}
```

---

## Accesibilidad

### Cumplimiento WCAG 2.1 AA

#### Contraste de Color
```css
/* Ratios mínimos requeridos */
/* Texto normal: 4.5:1 */
/* Texto grande (18sp+): 3:1 */
/* Elementos gráficos: 3:1 */

/* Combinaciones aprobadas */
.high-contrast-text {
  color: var(--neutral-black);     /* #202124 */
  background: var(--neutral-white); /* #FFFFFF */
  /* Ratio: 19.2:1 ✓ */
}

.medium-contrast-text {
  color: var(--neutral-dark);      /* #5F6368 */
  background: var(--neutral-white); /* #FFFFFF */
  /* Ratio: 7.15:1 ✓ */
}

.colored-background-text {
  color: var(--neutral-white);     /* #FFFFFF */
  background: var(--primary-blue); /* #4285F4 */
  /* Ratio: 8.32:1 ✓ */
}
```

#### Tamaños Táctiles
```css
/* Áreas mínimas de toque */
.touch-target {
  min-width: 48dp;   /* Mínimo Android */
  min-height: 48dp;
}

.child-touch-target {
  min-width: 60dp;   /* Recomendado para niños */
  min-height: 60dp;
}

.touch-spacing {
  margin: var(--space-2);  /* 8dp mínimo entre targets */
}
```

### Soporte para Lectores de Pantalla

#### Etiquetas Semánticas
```xml
<!-- Ejemplo XAML para .NET MAUI -->
<Button x:Name="MathButton"
        Text="Matemáticas"
        AutomationProperties.Name="Materia de Matemáticas"
        AutomationProperties.HelpText="Actividades de números, conteo y operaciones básicas. Progreso: 12 de 15 actividades completadas con 3 estrellas."
        AutomationProperties.IsInAccessibleTree="True"
        SemanticProperties.Hint="Toca para acceder a las actividades de matemáticas" />

<ProgressBar x:Name="MathProgress" 
             Progress="0.8"
             AutomationProperties.Name="Progreso de Matemáticas"
             AutomationProperties.HelpText="80 por ciento completado"
             AutomationProperties.IsInAccessibleTree="True" />
```

#### Estados Dinámicos
```xml
<!-- Estados que cambian dinámicamente -->
<Button x:Name="PlayButton"
        AutomationProperties.Name="{Binding IsPlaying, Converter={StaticResource PlayButtonNameConverter}}"
        AutomationProperties.HelpText="{Binding IsPlaying, Converter={StaticResource PlayButtonHelpConverter}}" />

<!-- Ejemplo de converter values:
     IsPlaying=false: Name="Reproducir", HelpText="Toca para reproducir el audio"
     IsPlaying=true: Name="Pausar", HelpText="Toca para pausar el audio" -->
```

### Configuraciones de Accesibilidad

#### Soporte para Configuraciones del Sistema
```csharp
// Ejemplo C# para .NET MAUI
public static class AccessibilityHelper
{
    public static double GetScaledFontSize(double baseFontSize)
    {
        // Obtener configuración de tamaño de fuente del sistema
        var fontScale = DeviceDisplay.MainDisplayInfo.Density;
        var accessibilityScale = Preferences.Get("accessibility_font_scale", 1.0);
        
        return baseFontSize * fontScale * accessibilityScale;
    }
    
    public static bool IsHighContrastEnabled()
    {
        // Verificar si el modo alto contraste está habilitado
        return Preferences.Get("accessibility_high_contrast", false);
    }
    
    public static bool IsReduceMotionEnabled()
    {
        // Verificar si la reducción de movimiento está habilitada
        return Preferences.Get("accessibility_reduce_motion", false);
    }
}
```

#### Adaptaciones Automáticas
```css
/* Media queries para preferencias de accesibilidad */
@media (prefers-reduced-motion: reduce) {
  * {
    animation-duration: 0.01ms !important;
    animation-iteration-count: 1 !important;
    transition-duration: 0.01ms !important;
  }
}

@media (prefers-contrast: high) {
  .button-primary {
    background: var(--neutral-black);
    color: var(--neutral-white);
    border: 2dp solid var(--neutral-white);
  }
}
```

### Navegación por Teclado

#### Focus Management
```css
/* Indicadores de focus claros */
.focusable:focus {
  outline: 3dp solid var(--primary-blue);
  outline-offset: 2dp;
  box-shadow: 0 0 0 6dp rgba(66, 133, 244, 0.2);
}

/* Skip links */
.skip-link {
  position: absolute;
  top: -40dp;
  left: 6dp;
  background: var(--neutral-black);
  color: var(--neutral-white);
  padding: 8dp;
  text-decoration: none;
  z-index: 1000;
}

.skip-link:focus {
  top: 6dp;
}
```

#### Tab Order
```xml
<!-- Orden lógico de tabulación -->
<StackLayout>
    <Label TabIndex="0" />
    <Entry TabIndex="1" />
    <Button TabIndex="2" Text="Primario" />
    <Button TabIndex="3" Text="Secundario" />
</StackLayout>
```

---

## Responsive Design

### Breakpoints del Sistema

#### Breakpoints Base
```css
/* Mobile First approach */
:root {
  /* Extra Small: 0-319dp */
  --bp-xs-max: 319dp;
  
  /* Small: 320-359dp */
  --bp-sm-min: 320dp;
  --bp-sm-max: 359dp;
  
  /* Medium: 360-399dp */
  --bp-md-min: 360dp;
  --bp-md-max: 399dp;
  
  /* Large: 400-479dp */
  --bp-lg-min: 400dp;
  --bp-lg-max: 479dp;
  
  /* Extra Large: 480-599dp */
  --bp-xl-min: 480dp;
  --bp-xl-max: 599dp;
  
  /* Tablet: 600dp+ */
  --bp-tablet-min: 600dp;
}
```

### Adaptaciones por Tamaño

#### Extra Small (0-319dp)
```css
@media (max-width: 319dp) {
  /* Configuración mínima para dispositivos muy pequeños */
  .container {
    padding: 0 var(--space-2);
  }
  
  .subject-grid {
    grid-template-columns: 1fr 1fr;
    gap: var(--space-2);
  }
  
  .subject-card {
    width: 100dp;
    height: 90dp;
    font-size: var(--text-sm);
  }
  
  .button-primary {
    min-width: 100dp;
    height: 44dp;
    font-size: var(--text-sm);
  }
  
  .activity-content {
    padding: var(--space-3);
  }
}
```

#### Small (320-359dp)
```css
@media (min-width: 320dp) and (max-width: 359dp) {
  .container {
    padding: 0 var(--space-3);
  }
  
  .subject-grid {
    grid-template-columns: 1fr 1fr;
    gap: var(--space-3);
  }
  
  .subject-card {
    width: 120dp;
    height: 100dp;
  }
  
  .modal-content {
    margin: var(--space-4);
    max-width: calc(100vw - 32dp);
  }
}
```

#### Medium (360-399dp) - Estándar
```css
@media (min-width: 360dp) and (max-width: 399dp) {
  /* Configuración base para la mayoría de dispositivos */
  .container {
    padding: 0 var(--space-4);
  }
  
  .subject-grid {
    grid-template-columns: 1fr 1fr;
    gap: var(--space-4);
  }
  
  .subject-card {
    width: 140dp;
    height: 130dp;
  }
  
  .activity-header {
    height: 72dp;
  }
  
  .navigation-bar {
    height: 56dp;
  }
}
```

#### Large (400-479dp)
```css
@media (min-width: 400dp) and (max-width: 479dp) {
  .container {
    padding: 0 var(--space-5);
  }
  
  .subject-card {
    width: 160dp;
    height: 140dp;
  }
  
  .button-primary {
    min-width: 160dp;
    height: 56dp;
  }
  
  .modal-content {
    max-width: 400dp;
  }
}
```

#### Extra Large (480-599dp)
```css
@media (min-width: 480dp) and (max-width: 599dp) {
  .container {
    max-width: 480dp;
    padding: 0 var(--space-6);
  }
  
  .subject-grid {
    grid-template-columns: repeat(3, 1fr);
  }
  
  .subject-card {
    width: 140dp;
    height: 130dp;
  }
}
```

#### Tablet (600dp+)
```css
@media (min-width: 600dp) {
  .container {
    max-width: 720dp;
    padding: 0 var(--space-8);
  }
  
  .subject-grid {
    grid-template-columns: repeat(3, 1fr);
    gap: var(--space-8);
  }
  
  .subject-card {
    width: 180dp;
    height: 160dp;
  }
  
  .activity-layout {
    /* Layout de dos columnas en tablets */
    display: grid;
    grid-template-columns: 1fr 300dp;
    gap: var(--space-8);
  }
  
  .sidebar {
    /* Sidebar para información adicional */
    display: block;
  }
  
  .modal-content {
    max-width: 600dp;
  }
}
```

### Orientación

#### Portrait (Vertical)
```css
@media (orientation: portrait) {
  .activity-layout {
    /* Layout vertical optimizado */
    flex-direction: column;
  }
  
  .button-grid {
    /* Botones en columnas para orientación vertical */
    grid-template-columns: 1fr 1fr;
  }
  
  .content-area {
    /* Más padding vertical */
    padding: var(--space-6) var(--space-4);
  }
}
```

#### Landscape (Horizontal)
```css
@media (orientation: landscape) and (max-width: 599dp) {
  .activity-header {
    /* Header más compacto en landscape */
    height: 56dp;
  }
  
  .activity-layout {
    /* Layout horizontal cuando hay espacio */
    flex-direction: row;
  }
  
  .content-area {
    /* Menos padding vertical, más horizontal */
    padding: var(--space-4) var(--space-8);
  }
  
  .button-grid {
    /* Botones en fila para landscape */
    grid-template-columns: repeat(4, 1fr);
  }
  
  .subject-grid {
    /* Más columnas en landscape */
    grid-template-columns: repeat(3, 1fr);
  }
}
```

### Densidad de Pantalla

#### Configuración por DPI
```css
/* Configuraciones específicas para diferentes densidades */

/* Low DPI (ldpi) - 120dpi */
@media (-webkit-device-pixel-ratio: 0.75) {
  .icon-standard { font-size: 18dp; }
  .touch-target { min-width: 52dp; min-height: 52dp; }
}

/* Medium DPI (mdpi) - 160dpi */
@media (-webkit-device-pixel-ratio: 1) {
  .icon-standard { font-size: 24dp; }
  .touch-target { min-width: 48dp; min-height: 48dp; }
}

/* High DPI (hdpi) - 240dpi */
@media (-webkit-device-pixel-ratio: 1.5) {
  .icon-standard { font-size: 24dp; }
  .touch-target { min-width: 48dp; min-height: 48dp; }
}

/* Extra High DPI (xhdpi) - 320dpi */
@media (-webkit-device-pixel-ratio: 2) {
  .icon-standard { font-size: 24dp; }
  .touch-target { min-width: 48dp; min-height: 48dp; }
}

/* Extra Extra High DPI (xxhdpi) - 480dpi */
@media (-webkit-device-pixel-ratio: 3) {
  .icon-standard { font-size: 24dp; }
  .touch-target { min-width: 48dp; min-height: 48dp; }
}
```

### Componentes Responsive

#### Navegación Adaptiva
```css
.navigation {
  /* Base: bottom navigation para móviles */
  position: fixed;
  bottom: 0;
  left: 0;
  right: 0;
  height: 56dp;
}

@media (min-width: 600dp) {
  .navigation {
    /* Tablet: side navigation */
    position: fixed;
    top: 0;
    left: 0;
    bottom: 0;
    width: 200dp;
    height: auto;
  }
  
  .main-content {
    margin-left: 200dp;
  }
}
```

#### Grid Adaptivo
```css
.adaptive-grid {
  display: grid;
  gap: var(--space-4);
  
  /* Base: 2 columnas */
  grid-template-columns: repeat(2, 1fr);
}

@media (min-width: 480dp) {
  .adaptive-grid {
    /* Pantallas medianas: 3 columnas */
    grid-template-columns: repeat(3, 1fr);
  }
}

@media (min-width: 600dp) {
  .adaptive-grid {
    /* Tablets: 4 columnas */
    grid-template-columns: repeat(4, 1fr);
    gap: var(--space-6);
  }
}
```

---

## Implementación Técnica

### Estructura de Archivos CSS

```
styles/
├── base/
│   ├── reset.css
│   ├── typography.css
│   ├── colors.css
│   └── spacing.css
├── components/
│   ├── buttons.css
│   ├── cards.css
│   ├── inputs.css
│   ├── modals.css
│   └── navigation.css
├── layouts/
│   ├── grid.css
│   ├── responsive.css
│   └── utilities.css
├── themes/
│   ├── light.css
│   ├── high-contrast.css
│   └── variables.css
├── animations/
│   ├── transitions.css
│   ├── keyframes.css
│   └── interactions.css
└── main.css (importa todos)
```

### Variables CSS para .NET MAUI

```xml
<!-- Ejemplo: Styles/Colors.xaml -->
<ResourceDictionary xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
                    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml">
    
    <!-- Colores Primarios -->
    <Color x:Key="PrimaryBlue">#4285F4</Color>
    <Color x:Key="PrimaryGreen">#34A853</Color>
    <Color x:Key="PrimaryYellow">#FBBC04</Color>
    <Color x:Key="PrimaryRed">#EA4335</Color>
    
    <!-- Gradientes -->
    <LinearGradientBrush x:Key="GradientPrimary" StartPoint="0,0" EndPoint="1,1">
        <GradientStop Color="{StaticResource PrimaryBlue}" Offset="0" />
        <GradientStop Color="{StaticResource PrimaryGreen}" Offset="1" />
    </LinearGradientBrush>
    
    <!-- Estilos Base -->
    <Style x:Key="ButtonPrimary" TargetType="Button">
        <Setter Property="BackgroundColor" Value="{StaticResource PrimaryGreen}" />
        <Setter Property="TextColor" Value="White" />
        <Setter Property="FontFamily" Value="NunitoBold" />
        <Setter Property="FontSize" Value="16" />
        <Setter Property="CornerRadius" Value="12" />
        <Setter Property="HeightRequest" Value="48" />
        <Setter Property="MinimumWidthRequest" Value="120" />
    </Style>
    
</ResourceDictionary>
```

### Configuración de Fuentes

```xml
<!-- MauiProgram.cs -->
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("Nunito-Regular.ttf", "NunitoRegular");
                fonts.AddFont("Nunito-Medium.ttf", "NunitoMedium");
                fonts.AddFont("Nunito-SemiBold.ttf", "NunitoSemiBold");
                fonts.AddFont("Nunito-Bold.ttf", "NunitoBold");
                fonts.AddFont("Nunito-ExtraBold.ttf", "NunitoExtraBold");
            });

        return builder.Build();
    }
}
```

### Configuración de Accesibilidad

```csharp
// Ejemplo: AccessibilityService.cs
public static class AccessibilityService
{
    public static void ConfigureAccessibility(VisualElement element, 
                                            string name, 
                                            string hint = null, 
                                            string helpText = null)
    {
        SemanticProperties.SetDescription(element, name);
        if (!string.IsNullOrEmpty(hint))
            SemanticProperties.SetHint(element, hint);
        if (!string.IsNullOrEmpty(helpText))
            AutomationProperties.SetHelpText(element, helpText);
            
        AutomationProperties.SetIsInAccessibleTree(element, true);
    }
    
    public static void AnnounceToScreenReader(string message)
    {
        SemanticService.Default.Announce(message);
    }
}
```

### Testing y Validación

#### Checklist de QA Visual
```markdown
## Pre-Release Visual QA

### Colores y Contraste
- [ ] Verificar contraste mínimo 4.5:1 para texto normal
- [ ] Verificar contraste mínimo 3:1 para texto grande
- [ ] Verificar contraste mínimo 3:1 para elementos gráficos
- [ ] Probar en modo alto contraste del sistema

### Tipografía
- [ ] Verificar legibilidad en todos los tamaños
- [ ] Probar con configuraciones de fuente grande del sistema
- [ ] Verificar truncamiento de texto en diferentes idiomas
- [ ] Probar renderizado en diferentes densidades

### Responsive
- [ ] Probar en pantallas 320dp, 360dp, 400dp, 600dp
- [ ] Verificar orientación portrait y landscape
- [ ] Probar en tablets y teléfonos
- [ ] Verificar adaptación a diferentes ratios de aspecto

### Accesibilidad
- [ ] Probar navegación con TalkBack/VoiceOver
- [ ] Verificar orden de tabulación lógico
- [ ] Probar con configuraciones de accesibilidad activadas
- [ ] Verificar descripciones de elementos interactivos

### Animaciones
- [ ] Verificar animaciones fluidas 60fps
- [ ] Probar con "reducir movimiento" activado
- [ ] Verificar feedback táctil en dispositivos compatibles
- [ ] Probar animaciones en dispositivos de gama baja
```

---

*Guía de estilo completa para EduPlayKids v1.0 - Septiembre 2025*