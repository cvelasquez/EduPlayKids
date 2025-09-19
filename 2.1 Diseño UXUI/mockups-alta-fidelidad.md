# Mockups de Alta Fidelidad - EduPlayKids

## Información del Documento
- **Proyecto**: EduPlayKids - Aplicación Educativa Móvil
- **Versión**: 1.0
- **Fecha**: Septiembre 2025
- **Autor**: Equipo de Diseño UX/UI
- **Framework**: .NET MAUI (Android Primario)

---

## Especificaciones Visuales

### Sistema de Colores Definitivo
```css
/* Colores Primarios */
--primary-blue: #4285F4;
--primary-green: #34A853;
--primary-yellow: #FBBC04;
--primary-red: #EA4335;

/* Colores Secundarios */
--secondary-purple: #9C27B0;
--secondary-orange: #FF9800;
--secondary-pink: #E91E63;
--secondary-cyan: #00BCD4;

/* Neutros */
--white: #FFFFFF;
--light-gray: #F8F9FA;
--medium-gray: #E8EAED;
--dark-gray: #5F6368;
--text-primary: #202124;

/* Estados */
--success: #137333;
--warning: #F29900;
--error: #D93025;
--info: #1A73E8;

/* Gradientes */
--gradient-primary: linear-gradient(135deg, #4285F4 0%, #34A853 100%);
--gradient-premium: linear-gradient(135deg, #9C27B0 0%, #E91E63 100%);
--gradient-success: linear-gradient(135deg, #34A853 0%, #FBBC04 100%);
```

### Tipografía
```css
/* Familia de Fuentes */
--font-family: 'Nunito', 'Roboto', sans-serif;

/* Tamaños */
--text-xs: 12sp;
--text-sm: 14sp;
--text-base: 16sp;
--text-lg: 18sp;
--text-xl: 20sp;
--text-2xl: 24sp;
--text-3xl: 30sp;
--text-4xl: 36sp;

/* Pesos */
--font-light: 300;
--font-regular: 400;
--font-medium: 500;
--font-semibold: 600;
--font-bold: 700;
--font-extrabold: 800;
```

### Sombras y Elevaciones
```css
--shadow-sm: 0 1px 2px rgba(60, 64, 67, 0.3);
--shadow-md: 0 1px 3px rgba(60, 64, 67, 0.3), 0 4px 8px rgba(60, 64, 67, 0.15);
--shadow-lg: 0 1px 2px rgba(60, 64, 67, 0.3), 0 2px 6px rgba(60, 64, 67, 0.15);
--shadow-xl: 0 1px 3px rgba(60, 64, 67, 0.3), 0 4px 8px rgba(60, 64, 67, 0.15), 0 8px 16px rgba(60, 64, 67, 0.15);

--border-radius-sm: 4dp;
--border-radius-md: 8dp;
--border-radius-lg: 12dp;
--border-radius-xl: 16dp;
--border-radius-full: 50%;
```

---

## 1. Mockup - Pantalla de Bienvenida

### Diseño Visual Completo
```
┌─────────────────────────────────────┐
│ Status Bar: #4285F4                 │ 24dp
├─────────────────────────────────────┤
│                                     │
│  ┌─ Gradient Background ──────────┐ │ 
│  │ #4285F4 → #34A853             │ │
│  │                               │ │
│  │     🎓 EduPlayKids            │ │ 36sp Bold
│  │   ¡Aprende Jugando!           │ │ 18sp Medium
│  │                               │ │
│  │ ┌─────────────────────────┐   │ │
│  │ │   🦁 Leo el León        │   │ │ 240x180dp
│  │ │   Animación Lottie      │   │ │ 
│  │ │   "¡Hola, soy Leo!"     │   │ │ 16sp Regular
│  │ └─────────────────────────┘   │ │
│  │                               │ │
│  │  Selecciona tu edad:          │ │ 20sp Medium
│  │                               │ │
│  │ ┌────┐  ┌────┐  ┌────┐       │ │
│  │ │👶  │  │🧒  │  │👦  │       │ │ 48x48dp
│  │ │3-4 │  │5-6 │  │7-8 │       │ │ 16sp Bold
│  │ │Pre │  │Kin │  │Gra │       │ │ 12sp Regular
│  │ │-K  │  │der │  │do  │       │ │
│  │ └────┘  └────┘  └────┘       │ │ 100x120dp
│  │   ○       ●       ○          │ │ Radio buttons
│  │                               │ │
│  │   ┌─────────────────────┐     │ │
│  │   │   🌟 EMPEZAR       │     │ │ 200x56dp
│  │   │   Gradient: Success │     │ │ Gradient
│  │   └─────────────────────┘     │ │ Shadow: lg
│  │                               │ │
│  └───────────────────────────────┘ │
│                                     │
│ ┌────────┐              ┌────────┐ │
│ │👨‍👩‍👧‍👦    │              │ 🌐 ES │ │ 48x48dp
│ │Padres  │              │English│ │ 14sp Medium
│ └────────┘              └────────┘ │
└─────────────────────────────────────┘
```

### Especificaciones Detalladas
- **Fondo**: Gradient diagonal 135° (Blue→Green)
- **Logo**: Nunito Extra Bold, color blanco
- **Mascota**: Lottie animation loop 8s
- **Botones edad**: Border 2dp cuando seleccionado (#FBBC04)
- **Estado deshabilitado**: 50% opacity hasta seleccionar edad
- **Transiciones**: Smooth 300ms ease-in-out

---

## 2. Mockup - Menú Principal de Materias

### Diseño Visual Completo
```
┌─────────────────────────────────────┐
│ Status Bar: #FFFFFF text #4285F4    │ 24dp
├─────────────────────────────────────┤
│ ┌─ Header Card ───────────────────┐ │
│ │ Background: #FFFFFF             │ │ 80dp height
│ │ Shadow: md                      │ │
│ │                                 │ │
│ │ 👤 Hola, María  ⭐47  👨‍👩‍👧‍👦      │ │ 18sp Medium
│ │ Color: #202124   #FBBC04  #666  │ │
│ └─────────────────────────────────┘ │
│                                     │
│ Background: #F8F9FA                 │
│                                     │
│     🎯 Elige tu aventura            │ 24sp Bold #202124
│                                     │
│ ┌─ Subject Cards Grid ───────────┐  │
│ │                               │  │
│ │ ┌──────────┐  ┌──────────┐   │  │
│ │ │Gradient: │  │Gradient: │   │  │ 140x130dp
│ │ │#4285F4   │  │#34A853   │   │  │ Each card
│ │ │→#00BCD4  │  │→#FBBC04  │   │  │
│ │ │          │  │          │   │  │
│ │ │   🔢     │  │   📚     │   │  │ 48x48dp icons
│ │ │Matemática│  │Lenguaje  │   │  │ 16sp Bold
│ │ │   s      │  │          │   │  │ White text
│ │ │          │  │          │   │  │
│ │ │ ⭐⭐⭐    │  │ ⭐⭐☆    │   │  │ 16x16dp stars
│ │ │ 12/15 ✓  │  │ 8/12 ✓   │   │  │ 12sp Regular
│ │ │          │  │          │   │  │
│ │ └──────────┘  └──────────┘   │  │
│ │                               │  │
│ │ ┌──────────┐  ┌──────────┐   │  │
│ │ │Gradient: │  │Gradient: │   │  │
│ │ │#FF9800   │  │#9C27B0   │   │  │
│ │ │→#E91E63  │  │→#EA4335  │   │  │
│ │ │          │  │          │   │  │
│ │ │   🎨     │  │   🧩     │   │  │
│ │ │Colores y │  │ Lógica   │   │  │
│ │ │ Formas   │  │          │   │  │
│ │ │          │  │          │   │  │
│ │ │ ⭐⭐⭐    │  │ ⭐⭐☆    │   │  │
│ │ │ 15/15 ✓  │  │ 5/10 ✓   │   │  │
│ │ │          │  │          │   │  │
│ │ └──────────┘  └──────────┘   │  │
│ │                               │  │
│ │ ┌──────────┐  ┌──────────┐   │  │
│ │ │Available │  │ Locked   │   │  │
│ │ │#34A853   │  │#E8EAED   │   │  │
│ │ │          │  │          │   │  │
│ │ │   🌱     │  │   🎵     │   │  │
│ │ │ Ciencias │  │ Música   │   │  │
│ │ │          │  │          │   │  │
│ │ │ ⭐☆☆     │  │   🔒     │   │  │
│ │ │ 2/8 ✓    │  │Bloqueado │   │  │
│ │ │          │  │          │   │  │
│ │ └──────────┘  └──────────┘   │  │
│ └───────────────────────────────┘  │
│                                     │
│ ┌─ Bottom Navigation ───────────┐   │
│ │ Background: #FFFFFF           │   │ 56dp height
│ │ Shadow: top md                │   │
│ │                               │   │
│ │ ┌────────┐     ┌────────┐    │   │
│ │ │  📊    │     │  🏆    │    │   │ 40x40dp
│ │ │Progreso│     │ Logros │    │   │ 12sp Medium
│ │ │#4285F4 │     │ #666   │    │   │ Active/Inactive
│ │ └────────┘     └────────┘    │   │
│ └───────────────────────────────┘   │
└─────────────────────────────────────┘
```

### Estados de Interacción
- **Card hover**: Scale 1.02, shadow-xl
- **Card press**: Scale 0.98, haptic feedback
- **Locked card**: No interaction, gray overlay
- **Completed card**: Golden border glow

---

## 3. Mockup - Actividad Matemáticas

### Diseño Visual Completo
```
┌─────────────────────────────────────┐
│ Status Bar: #4285F4                 │ 24dp
├─────────────────────────────────────┤
│ ┌─ Activity Header ───────────────┐ │
│ │ Background: Gradient #4285F4    │ │ 72dp height
│ │ → #00BCD4                       │ │
│ │                                 │ │
│ │ [←] Matemáticas  [❤❤❤] [⏱2:30] │ │
│ │ 16sp  18sp Bold   24x24   16sp  │ │ White text
│ │ Regular                         │ │
│ └─────────────────────────────────┘ │
│                                     │
│ Background: Linear gradient         │
│ #FFFFFF → #F8F9FA                   │
│                                     │
│      Cuenta los patitos 🦆          │ 22sp Bold #202124
│                                     │
│ ┌─ Content Area ─────────────────┐  │
│ │ Background: #FFFFFF           │  │ 300x220dp
│ │ Border: 2dp #E8EAED           │  │ Rounded 12dp
│ │ Shadow: lg                    │  │
│ │                               │  │
│ │    🦆    🦆    🦆            │  │ 48x48dp
│ │  (Animated duck sprites)     │  │ Bounce on tap
│ │                               │  │
│ │      🦆       🦆             │  │
│ │                               │  │
│ │  (Parallax water background) │  │ Subtle animation
│ │                               │  │
│ └───────────────────────────────┘  │
│                                     │
│       ¿Cuántos patitos hay?         │ 18sp Medium #5F6368
│                                     │
│ ┌─ Answer Options ──────────────┐   │
│ │                              │   │
│ │ ┌────┐  ┌────┐  ┌────┐      │   │
│ │ │ 3  │  │ 5  │  │ 7  │      │   │ 80x80dp
│ │ │    │  │    │  │    │      │   │ Each option
│ │ │    │  │    │  │    │      │   │
│ │ └────┘  └────┘  └────┘      │   │ 28sp Bold
│ │ Border  Active  Border      │   │ #E8EAED / #4285F4
│ │ #E8EAED #4285F4 #E8EAED     │   │
│ │                              │   │
│ └──────────────────────────────┘   │
│                                     │
│ ┌─ Primary Action ──────────────┐   │
│ │   ┌─────────────────────┐     │   │
│ │   │   ✓ RESPONDER      │     │   │ 220x56dp
│ │   │   Background:      │     │   │ Gradient
│ │   │   Success Gradient │     │   │ #34A853→#FBBC04
│ │   │   Text: #FFFFFF    │     │   │ 18sp Bold
│ │   │   Shadow: lg       │     │   │
│ │   └─────────────────────┘     │   │
│ └───────────────────────────────┘   │
│                                     │
│ ┌─ Secondary Actions ───────────┐   │
│ │ ┌──────────┐  ┌──────────┐   │   │
│ │ │ 💡 Pista │  │ 🔊 Audio │   │   │ 100x44dp
│ │ │ #F29900  │  │ #1A73E8  │   │   │ Each button
│ │ │ Rounded  │  │ Rounded  │   │   │ 14sp Medium
│ │ └──────────┘  └──────────┘   │   │ White text
│ └───────────────────────────────┘   │
└─────────────────────────────────────┘
```

### Animaciones y Micro-interacciones
- **Patitos**: Bob animation (2s loop)
- **Selección respuesta**: Scale + glow effect
- **Feedback correcto**: Confetti + celebración
- **Feedback incorrecto**: Shake + hint highlight
- **Timer**: Color change último 30s (#F29900)

---

## 4. Mockup - Actividad Lectura/Trazado

### Diseño Visual Completo
```
┌─────────────────────────────────────┐
│ Status Bar: #34A853                 │ 24dp
├─────────────────────────────────────┤
│ ┌─ Activity Header ───────────────┐ │
│ │ Background: Gradient #34A853    │ │ 72dp height
│ │ → #FBBC04                       │ │
│ │                                 │ │
│ │ [←] Lenguaje    [❤❤❤] [⏱1:45]  │ │
│ │ 16sp  18sp Bold  24x24  16sp    │ │ White text
│ │ Regular                         │ │
│ └─────────────────────────────────┘ │
│                                     │
│ Background: Linear gradient         │
│ #FFFFFF → #F8F9FA                   │
│                                     │
│       Traza la letra A              │ 22sp Bold #202124
│                                     │
│ ┌─ Tracing Area ────────────────┐   │
│ │ Background: #FFFFFF          │   │ 320x240dp
│ │ Border: 3dp #34A853         │   │ Rounded 16dp
│ │ Shadow: xl                   │   │
│ │                              │   │
│ │        A                     │   │ 120sp
│ │      /   \                  │   │ Ultra Bold
│ │     /     \                 │   │ #E8EAED (guide)
│ │    /_______\                │   │ 
│ │   /         \               │   │
│ │                              │   │
│ │  ┌─ Traced Path ──────────┐  │   │
│ │  │ Stroke: #34A853       │  │   │ 8dp width
│ │  │ Style: Rounded caps   │  │   │ Path tracing
│ │  │ Progress: 65%         │  │   │ Real-time
│ │  │ Animation: Sparkles   │  │   │ Follow finger
│ │  └───────────────────────┘  │   │
│ │                              │   │
│ │   👆 Traza aquí             │   │ 16sp Medium
│ │   Color: #5F6368            │   │ Animated pointer
│ │                              │   │
│ └──────────────────────────────┘   │
│                                     │
│ ┌─ Audio Pronunciation ─────────┐   │
│ │ 🔊 "Ay" como en "Avión"       │   │ 18sp Regular
│ │ Background: #F8F9FA           │   │ #202124
│ │ Rounded: 24dp                 │   │ Padding 16dp
│ │ Border: 1dp #E8EAED          │   │
│ └───────────────────────────────┘   │
│                                     │
│ ┌─ Action Buttons ──────────────┐   │
│ │                               │   │
│ │   ┌─────────────────────┐     │   │
│ │   │   📝 PRACTICAR     │     │   │ 200x48dp
│ │   │   Background:      │     │   │ #34A853
│ │   │   #34A853          │     │   │ White text
│ │   │   Disabled: 50%    │     │   │ 16sp Medium
│ │   └─────────────────────┘     │   │
│ │                               │   │
│ │   ┌─────────────────────┐     │   │
│ │   │   ➡ SIGUIENTE      │     │   │ 200x48dp
│ │   │   Background:      │     │   │ Gradient
│ │   │   Success Gradient │     │   │ #34A853→#FBBC04
│ │   │   Enabled after    │     │   │ 16sp Medium
│ │   │   completion       │     │   │
│ │   └─────────────────────┘     │   │
│ │                               │   │
│ └───────────────────────────────┘   │
│                                     │
│ ┌─ Helper Actions ──────────────┐   │
│ │ ┌──────────┐  ┌──────────┐   │   │
│ │ │ 🔊 Audio │  │ ↻ Reset  │   │   │ 90x40dp
│ │ │ #1A73E8  │  │ #F29900  │   │   │ Each button
│ │ │          │  │          │   │   │ 14sp Medium
│ │ └──────────┘  └──────────┘   │   │ White text
│ └───────────────────────────────┘   │
└─────────────────────────────────────┘
```

### Especificaciones de Trazado
- **Canvas**: Multi-touch habilitado
- **Stroke recognition**: Algoritmo de similaridad 85%
- **Feedback**: Real-time sparkles en path correcto
- **Completion**: Celebración con estrellas doradas
- **Error handling**: Guided correction con hints

---

## 5. Mockup - Panel de Padres (PIN)

### Diseño Visual Completo
```
┌─────────────────────────────────────┐
│ Status Bar: #9C27B0                 │ 24dp
├─────────────────────────────────────┤
│ ┌─ Parent Header ────────────────┐  │
│ │ Background: Gradient #9C27B0   │  │ 88dp height
│ │ → #E91E63                      │  │
│ │                                │  │
│ │      👨‍👩‍👧‍👦 Panel de Padres     │  │ 24sp Bold
│ │      Color: #FFFFFF            │  │ White text
│ │                                │  │
│ └────────────────────────────────┘  │
│                                     │
│ Background: #F8F9FA                 │
│                                     │
│    Ingresa tu PIN de 4 dígitos      │ 18sp Medium #202124
│                                     │
│ ┌─ PIN Input Display ───────────┐   │
│ │                              │   │
│ │      ┌─────────────────┐      │   │
│ │      │  ● ● ● ○        │      │   │ 280x60dp
│ │      │  Filled dots:   │      │   │ Container
│ │      │  #9C27B0        │      │   │ 16x16dp dots
│ │      │  Empty: #E8EAED │      │   │ 8dp spacing
│ │      │  Border: 2dp    │      │   │ Rounded 12dp
│ │      │  #E8EAED        │      │   │
│ │      └─────────────────┘      │   │
│ │                              │   │
│ └──────────────────────────────┘   │
│                                     │
│ ┌─ Numeric Keypad ──────────────┐   │
│ │                               │   │
│ │  ┌────┐ ┌────┐ ┌────┐        │   │
│ │  │ 1  │ │ 2  │ │ 3  │        │   │ 80x80dp
│ │  │    │ │    │ │    │        │   │ Each button
│ │  └────┘ └────┘ └────┘        │   │ 32sp Bold
│ │                               │   │ #202124
│ │  ┌────┐ ┌────┐ ┌────┐        │   │ Background: #FFFFFF
│ │  │ 4  │ │ 5  │ │ 6  │        │   │ Border: 1dp #E8EAED
│ │  │    │ │    │ │    │        │   │ Shadow: sm
│ │  └────┘ └────┘ └────┘        │   │ Rounded: 12dp
│ │                               │   │
│ │  ┌────┐ ┌────┐ ┌────┐        │   │
│ │  │ 7  │ │ 8  │ │ 9  │        │   │
│ │  │    │ │    │ │    │        │   │
│ │  └────┘ └────┘ └────┘        │   │
│ │                               │   │
│ │      ┌────┐ ┌────┐           │   │
│ │      │ 0  │ │ ⌫  │           │   │ Delete button
│ │      │    │ │    │           │   │ #EA4335 (red)
│ │      └────┘ └────┘           │   │
│ │                               │   │
│ └───────────────────────────────┘   │
│                                     │
│ ┌─ Back Navigation ─────────────┐   │
│ │  [←] Volver al juego          │   │ 16sp Medium
│ │  Color: #5F6368               │   │ #5F6368
│ │  Underline on press           │   │
│ └───────────────────────────────┘   │
└─────────────────────────────────────┘
```

### Estados y Validación
- **Input correcto**: Smooth transition al dashboard
- **Input incorrecto**: Shake animation, borrar automático
- **Intentos fallidos**: Mensaje de error después de 3 intentos
- **Biometrics**: Huella dactilar como alternativa (opcional)

---

## 6. Mockup - Dashboard de Padres

### Diseño Visual Completo
```
┌─────────────────────────────────────┐
│ Status Bar: #9C27B0                 │ 24dp
├─────────────────────────────────────┤
│ ┌─ Parent Header ────────────────┐  │
│ │ Background: Gradient #9C27B0   │  │ 72dp height
│ │ → #E91E63                      │  │
│ │                                │  │
│ │ [←] Panel Padres   [⚙] Config │  │ 18sp Medium
│ │ Color: #FFFFFF                 │  │ White text
│ │                                │  │
│ └────────────────────────────────┘  │
│                                     │
│ Background: #F8F9FA                 │
│                                     │
│     📊 Progreso de María            │ 20sp Bold #202124
│                                     │
│ ┌─ Stats Summary ───────────────┐   │
│ │ Background: #FFFFFF          │   │ Full width
│ │ Shadow: md, Rounded: 12dp    │   │ 16dp padding
│ │                              │   │
│ │ Tiempo hoy: 45 min  🔥 Racha:7│   │ 16sp Medium
│ │ Color: #202124     #FF9800   │   │ #202124, #FF9800
│ │                              │   │
│ │ Actividades: 12/15 ⭐Total:47│   │ 16sp Medium
│ │ Color: #34A853    #FBBC04    │   │ #34A853, #FBBC04
│ │                              │   │
│ └──────────────────────────────┘   │
│                                     │
│ ┌─ Weekly Chart ────────────────┐   │
│ │ Background: #FFFFFF          │   │ 300x180dp
│ │ Shadow: md, Rounded: 12dp    │   │
│ │                              │   │
│ │   📈 Gráfico Semanal         │   │ 18sp Bold #202124
│ │                              │   │
│ │   ┌─ Bar Chart ──────────┐   │   │
│ │   │                     │   │   │ Canvas element
│ │   │  ▓░░▓▓░▓           │   │   │ Animated bars
│ │   │  █░░██░█           │   │   │ #4285F4 filled
│ │   │  ████████          │   │   │ #E8EAED empty
│ │   │  L M X J V S D     │   │   │ 12sp labels
│ │   │                     │   │   │
│ │   └─────────────────────┘   │   │
│ │                              │   │
│ └──────────────────────────────┘   │
│                                     │
│ ┌─ Favorite Subjects ───────────┐   │
│ │ Background: #FFFFFF          │   │ Full width
│ │ Shadow: md, Rounded: 12dp    │   │
│ │                              │   │
│ │ 📚 Materias Favoritas:       │   │ 18sp Bold #202124
│ │                              │   │
│ │ 1️⃣ Matemáticas (18 acts)     │   │ 16sp Regular
│ │ 2️⃣ Colores (15 acts)         │   │ #5F6368
│ │ 3️⃣ Lectura (12 acts)         │   │
│ │                              │   │
│ └──────────────────────────────┘   │
│                                     │
│ ┌─ Recent Achievements ─────────┐   │
│ │ Background: #FFFFFF          │   │ Full width
│ │ Shadow: md, Rounded: 12dp    │   │
│ │                              │   │
│ │ 🏆 Últimos Logros:           │   │ 18sp Bold #202124
│ │                              │   │
│ │ • Contador Experto 🔢        │   │ 16sp Regular
│ │ • Artista de Colores 🎨      │   │ #5F6368
│ │ • Lector Principiante 📖     │   │ Bullet: #34A853
│ │                              │   │
│ └──────────────────────────────┘   │
│                                     │
│ ┌─ Action Buttons ──────────────┐   │
│ │                               │   │
│ │ ┌─────────┐ ┌─────────────┐   │   │
│ │ │📊       │ │💎 Premium   │   │   │ 140x48dp each
│ │ │Reportes │ │ Upgrade     │   │   │ Side by side
│ │ │#4285F4  │ │ Gradient    │   │   │ Colors
│ │ └─────────┘ └─────────────┘   │   │ 16sp Medium
│ │                               │   │
│ └───────────────────────────────┘   │
└─────────────────────────────────────┘
```

### Datos y Métricas
- **Tiempo de sesión**: Auto-tracking con SQLite
- **Gráfico actividad**: Chart.js con animaciones
- **Racha**: Cálculo de días consecutivos
- **Logros**: Sistema de badges desbloqueables

---

## 7. Mockup - Pantalla Premium

### Diseño Visual Completo
```
┌─────────────────────────────────────┐
│ Status Bar: #9C27B0                 │ 24dp
├─────────────────────────────────────┤
│ ┌─ Premium Header ───────────────┐  │
│ │ Background: Gradient #9C27B0   │  │ 88dp height
│ │ → #E91E63                      │  │
│ │                                │  │
│ │ [×] Cerrar                     │  │ 18sp Medium
│ │ Color: #FFFFFF                 │  │ White text
│ │                                │  │
│ └────────────────────────────────┘  │
│                                     │
│ Background: Radial gradient         │
│ Center: #FFFFFF, Edge: #F8F9FA      │
│                                     │
│        🌟 ¡Desbloquea Todo!         │ 28sp Bold #202124
│                                     │
│ ┌─ Premium Features Card ───────┐   │
│ │ Background: Gradient          │   │ 320x240dp
│ │ #9C27B0 → #E91E63            │   │ Rounded: 16dp
│ │ Shadow: xl                    │   │ Shadow: xl
│ │                               │   │
│ │     👑 PREMIUM                │   │ 24sp Bold
│ │     Color: #FFFFFF            │   │ White text
│ │                               │   │
│ │  • Acceso ilimitado           │   │ 16sp Regular
│ │  • 150+ actividades           │   │ White text
│ │  • Reportes avanzados         │   │ • bullets
│ │  • Sin publicidad             │   │ #FBBC04 (gold)
│ │  • Soporte prioritario        │   │
│ │                               │   │
│ │  ┌─ Sparkle Animation ─────┐  │   │
│ │  │ Floating particles    │  │   │ Lottie animation
│ │  │ Color: #FBBC04        │  │   │ Gold sparkles
│ │  │ Duration: 3s loop     │  │   │ Continuous
│ │  └───────────────────────┘  │   │
│ │                               │   │
│ └───────────────────────────────┘   │
│                                     │
│   🎁 3 días GRATIS, luego:          │ 18sp Medium #5F6368
│                                     │
│ ┌─ Pricing Card ───────────────┐    │
│ │ Background: #FFFFFF          │    │ 280x80dp
│ │ Border: 2dp #FBBC04         │    │ Gold border
│ │ Shadow: lg, Rounded: 12dp    │    │ Prominent
│ │                              │    │
│ │      💎 $4.99/mes            │    │ 32sp Bold
│ │      Color: #9C27B0          │    │ Premium purple
│ │                              │    │
│ │    Cancela cuando quieras    │    │ 14sp Regular
│ │    Color: #5F6368            │    │ Gray text
│ │                              │    │
│ └──────────────────────────────┘    │
│                                     │
│ ┌─ CTA Buttons ─────────────────┐   │
│ │                               │   │
│ │   ┌─────────────────────┐     │   │
│ │   │  🚀 COMENZAR       │     │   │ 240x56dp
│ │   │   TRIAL GRATIS     │     │   │ Gradient
│ │   │   Background:      │     │   │ #34A853→#FBBC04
│ │   │   Success Gradient │     │   │ 18sp Bold
│ │   │   Text: #FFFFFF    │     │   │ White text
│ │   │   Shadow: lg       │     │   │ Prominent
│ │   │   Pulse animation  │     │   │ Attention-grabbing
│ │   └─────────────────────┘     │   │
│ │                               │   │
│ │   ┌─────────────────────┐     │   │
│ │   │  💳 COMPRAR        │     │   │ 240x48dp
│ │   │    PREMIUM         │     │   │ Outline button
│ │   │   Border: 2dp      │     │   │ #9C27B0 border
│ │   │   #9C27B0          │     │   │ Purple text
│ │   │   Background:      │     │   │ Transparent
│ │   │   Transparent      │     │   │ 16sp Medium
│ │   └─────────────────────┘     │   │
│ │                               │   │
│ └───────────────────────────────┘   │
│                                     │
│ ┌─ Legal Links ─────────────────┐   │
│ │                               │   │
│ │ 📋 Términos    🔒 Privacidad  │   │ 12sp Regular
│ │ Color: #1A73E8 (links)        │   │ Blue links
│ │ Underline on press            │   │ Interactive
│ │                               │   │
│ └───────────────────────────────┘   │
└─────────────────────────────────────┘
```

### Microinteracciones Premium
- **Entrada**: Slide up from bottom con bounce
- **Sparkles**: Continuous floating animation
- **CTA principal**: Pulse effect cada 3s
- **Pricing card**: Gentle glow animation
- **Background**: Subtle particle system

---

## 8. Mockup - Progreso y Estadísticas

### Diseño Visual Completo
```
┌─────────────────────────────────────┐
│ Status Bar: #4285F4                 │ 24dp
├─────────────────────────────────────┤
│ ┌─ Progress Header ──────────────┐  │
│ │ Background: Gradient #4285F4   │  │ 72dp height
│ │ → #34A853                      │  │
│ │                                │  │
│ │ [←] Mi Progreso  [📅] Semanal  │  │ 18sp Medium
│ │ Color: #FFFFFF                 │  │ White text
│ │                                │  │
│ └────────────────────────────────┘  │
│                                     │
│ Background: #F8F9FA                 │
│                                     │
│       🌟 ¡Excelente trabajo!        │ 22sp Bold #202124
│                                     │
│ ┌─ Mascot Level Card ───────────┐   │
│ │ Background: Gradient          │   │ 300x120dp
│ │ #FBBC04 → #FF9800            │   │ Rounded: 16dp
│ │ Shadow: lg                    │   │
│ │                               │   │
│ │  ┌─ Mascot Animation ───────┐ │   │
│ │  │    🦁 Leo Feliz        │ │   │ 80x80dp
│ │  │    Happy bounce        │ │   │ Lottie animation
│ │  │    Color vibrant       │ │   │ Level-based
│ │  └────────────────────────┘ │   │
│ │                               │   │
│ │     Nivel 3 - León            │   │ 18sp Bold
│ │     Color: #FFFFFF            │   │ White text
│ │                               │   │
│ │   ████████████░░ 75%          │   │ Progress bar
│ │   Color: #FFFFFF              │   │ White fill
│ │   Background: rgba(255,255,255,0.3)│ Semi-transparent
│ │                               │   │
│ └───────────────────────────────┘   │
│                                     │
│ ┌─ General Stats Card ──────────┐   │
│ │ Background: #FFFFFF          │   │ Full width
│ │ Shadow: md, Rounded: 12dp    │   │ 16dp padding
│ │                              │   │
│ │ 📊 Estadísticas Generales:   │   │ 18sp Bold #202124
│ │                              │   │
│ │ ⭐ Estrellas totales: 47/90  │   │ 16sp Regular
│ │ Color: #FBBC04 / #5F6368     │   │ Gold / Gray
│ │                              │   │
│ │ 🎯 Actividades: 23/45        │   │ 16sp Regular
│ │ Color: #4285F4 / #5F6368     │   │ Blue / Gray
│ │                              │   │
│ │ 🔥 Racha actual: 7 días      │   │ 16sp Regular
│ │ Color: #FF9800 / #5F6368     │   │ Orange / Gray
│ │                              │   │
│ │ ⏱ Tiempo total: 12h 30min   │   │ 16sp Regular
│ │ Color: #9C27B0 / #5F6368     │   │ Purple / Gray
│ │                              │   │
│ └──────────────────────────────┘   │
│                                     │
│ ┌─ Subject Progress ────────────┐   │
│ │ Background: #FFFFFF          │   │ Full width
│ │ Shadow: md, Rounded: 12dp    │   │
│ │                              │   │
│ │ 📚 Progreso por Materia:     │   │ 18sp Bold #202124
│ │                              │   │
│ │ 🔢 Matemáticas              │   │ 16sp Medium
│ │ ████████░░ 80%              │   │ Progress bar
│ │ Color: #4285F4               │   │ Subject color
│ │                              │   │
│ │ 📖 Lenguaje                 │   │ 16sp Medium
│ │ ██████░░░░ 60%              │   │ Progress bar
│ │ Color: #34A853               │   │ Subject color
│ │                              │   │
│ │ 🎨 Arte                     │   │ 16sp Medium
│ │ █████████░ 90%              │   │ Progress bar
│ │ Color: #FF9800               │   │ Subject color
│ │                              │   │
│ │ 🧩 Lógica                   │   │ 16sp Medium
│ │ ████░░░░░░ 40%              │   │ Progress bar
│ │ Color: #9C27B0               │   │ Subject color
│ │                              │   │
│ │ 🌱 Ciencias                 │   │ 16sp Medium
│ │ ██░░░░░░░░ 20%              │   │ Progress bar
│ │ Color: #00BCD4               │   │ Subject color
│ │                              │   │
│ └──────────────────────────────┘   │
│                                     │
│ ┌─ Action Buttons ──────────────┐   │
│ │                               │   │
│ │ ┌─────────┐ ┌─────────────┐   │   │
│ │ │🏆       │ │📈 Reportes  │   │   │ 140x48dp each
│ │ │Mis      │ │ Detallados  │   │   │ Side by side
│ │ │Logros   │ │ #4285F4     │   │   │ Colors
│ │ │#FBBC04  │ │             │   │   │ 14sp Medium
│ │ └─────────┘ └─────────────┘   │   │
│ │                               │   │
│ └───────────────────────────────┘   │
└─────────────────────────────────────┘
```

### Animaciones de Progreso
- **Barras**: Count-up animation al cargar
- **Mascota**: Level-up celebration cuando apropiado
- **Números**: Rolling counter effect
- **Entrada**: Stagger animation de cards

---

## Especificaciones de Estados

### Estados Globales de Componentes

#### Botones
```css
/* Primary Button */
.button-primary {
  background: linear-gradient(135deg, #34A853 0%, #FBBC04 100%);
  color: #FFFFFF;
  font-weight: 600;
  border-radius: 12dp;
  elevation: 4dp;
}

.button-primary:hover {
  transform: scale(1.02);
  elevation: 6dp;
}

.button-primary:pressed {
  transform: scale(0.98);
  elevation: 2dp;
}

.button-primary:disabled {
  background: #E8EAED;
  color: #5F6368;
  elevation: 0dp;
}

/* Subject Card */
.subject-card {
  background: linear-gradient(135deg, var(--subject-color) 0%, var(--subject-secondary) 100%);
  border-radius: 16dp;
  elevation: 3dp;
  transition: all 300ms ease-in-out;
}

.subject-card:hover {
  transform: translateY(-2dp);
  elevation: 8dp;
}

.subject-card:pressed {
  transform: scale(0.98);
}

.subject-card.locked {
  background: #E8EAED;
  opacity: 0.6;
  transform: none;
}

.subject-card.completed {
  border: 2dp solid #FBBC04;
  box-shadow: 0 0 12dp rgba(251, 188, 4, 0.3);
}
```

### Responsive Breakpoints
```css
/* Small phones */
@media (max-width: 360dp) {
  .subject-card { width: 120dp; height: 100dp; }
  .button-primary { width: 180dp; height: 48dp; }
  .text-title { font-size: 20sp; }
}

/* Standard phones */
@media (min-width: 361dp) and (max-width: 410dp) {
  .subject-card { width: 140dp; height: 130dp; }
  .button-primary { width: 200dp; height: 56dp; }
  .text-title { font-size: 24sp; }
}

/* Large phones */
@media (min-width: 411dp) {
  .subject-card { width: 160dp; height: 140dp; }
  .button-primary { width: 220dp; height: 60dp; }
  .text-title { font-size: 28sp; }
}

/* Tablets */
@media (min-width: 600dp) {
  .layout-grid { columns: 3; }
  .subject-card { width: 180dp; height: 160dp; }
  .content-padding { padding: 24dp; }
}
```

---

## Especificaciones de Accesibilidad

### Etiquetas ARIA y Semántica
```xml
<!-- Ejemplo: Subject Card -->
<ContentView 
    AutomationId="SubjectCard_Mathematics"
    AutomationProperties.Name="Matemáticas"
    AutomationProperties.HelpText="Materia de matemáticas, progreso 12 de 15 actividades completadas, 3 estrellas"
    AutomationProperties.IsInAccessibleTree="True">
    
    <Frame StyleClass="subject-card"
           AutomationProperties.IsInAccessibleTree="False">
        <!-- Content -->
    </Frame>
</ContentView>

<!-- Ejemplo: Progress Bar -->
<ProgressBar Progress="{Binding MathProgress}"
             AutomationProperties.Name="Progreso Matemáticas"
             AutomationProperties.HelpText="{Binding MathProgressDescription}"
             AutomationId="ProgressBar_Mathematics" />
```

### Navegación por Teclado
- **Tab Order**: Lógico, de izquierda a derecha, arriba a abajo
- **Focus Indicators**: Border 3dp #1A73E8 con shadow
- **Skip Links**: "Saltar al contenido principal"
- **Shortcuts**: Teclas numéricas para selección rápida

### Soporte para Lectores de Pantalla
- **Descripciones**: Contexto completo para cada elemento
- **Estados**: Anuncio de cambios de estado
- **Navegación**: Landmarks y headings estructurados
- **Feedback**: Confirmaciones audibles para acciones

---

## Archivos de Assets Requeridos

### Iconografía (SVG/PNG)
```
icons/
├── subjects/
│   ├── mathematics.svg (48x48dp, 96x96dp)
│   ├── reading.svg
│   ├── colors.svg
│   ├── logic.svg
│   ├── science.svg
│   └── music.svg
├── ui/
│   ├── star-filled.svg (16x16dp, 24x24dp)
│   ├── star-empty.svg
│   ├── heart-full.svg
│   ├── heart-empty.svg
│   ├── lock.svg
│   └── checkmark.svg
└── navigation/
    ├── back-arrow.svg
    ├── settings.svg
    ├── close.svg
    └── menu.svg
```

### Animaciones (Lottie)
```
animations/
├── mascot/
│   ├── leo-happy.json
│   ├── leo-celebrating.json
│   ├── leo-thinking.json
│   └── leo-sleeping.json
├── effects/
│   ├── sparkles.json
│   ├── confetti.json
│   ├── stars-burst.json
│   └── loading-spinner.json
└── feedback/
    ├── correct-answer.json
    ├── incorrect-shake.json
    └── level-up.json
```

### Audio (MP3/WAV)
```
audio/
├── ui/
│   ├── button-tap.wav
│   ├── success-chime.wav
│   ├── error-buzz.wav
│   └── page-transition.wav
├── feedback/
│   ├── correct-celebration.wav
│   ├── incorrect-gentle.wav
│   ├── hint-notification.wav
│   └── achievement-fanfare.wav
└── pronunciations/
    ├── letters/
    │   ├── a.wav
    │   ├── b.wav
    │   └── ...
    └── numbers/
        ├── 1.wav
        ├── 2.wav
        └── ...
```

---

*Documento de mockups de alta fidelidad para EduPlayKids v1.0 - Septiembre 2025*