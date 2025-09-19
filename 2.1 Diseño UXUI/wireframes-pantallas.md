# Wireframes de Pantallas Principales - EduPlayKids

## Información del Documento
- **Proyecto**: EduPlayKids - Aplicación Educativa Móvil
- **Versión**: 1.0
- **Fecha**: Septiembre 2025
- **Autor**: Equipo de Diseño UX/UI
- **Framework**: .NET MAUI (Android Primario)

---

## Especificaciones de Diseño

### Estándares de Interfaz Infantil
- **Botones mínimos**: 60dp (44dp Android + margen para niños)
- **Área táctil**: Mínimo 8mm físicos
- **Espaciado**: Mínimo 16dp entre elementos interactivos
- **Contraste**: Mínimo 7:1 para texto y 3:1 para elementos gráficos
- **Tipografía**: Sans-serif, mínimo 18sp para texto principal

### Paleta de Colores Base
```
Primarios:
- Azul Amigable: #4285F4
- Verde Éxito: #34A853  
- Amarillo Alegría: #FBBC04
- Rojo Suave: #EA4335

Secundarios:
- Morado Creativo: #9C27B0
- Naranja Energía: #FF9800
- Rosa Diversión: #E91E63
- Cian Tranquilo: #00BCD4

Neutros:
- Blanco: #FFFFFF
- Gris Claro: #F5F5F5
- Gris Medio: #9E9E9E
- Gris Oscuro: #424242
```

---

## 1. Pantalla de Bienvenida y Selección de Edad

### Wireframe Layout
```
┌─────────────────────────────────────┐
│                                     │
│         🎓 EduPlayKids              │
│      ¡Aprende Jugando!              │
│                                     │
│  ┌─────────────────────────────┐    │
│  │       🦁 Mascota            │    │
│  │    Animación Bienvenida     │    │
│  └─────────────────────────────┘    │
│                                     │
│    Selecciona tu edad:              │
│                                     │
│  ┌──────┐ ┌──────┐ ┌──────┐        │
│  │ 3-4  │ │ 5-6  │ │ 7-8  │        │
│  │ 👶   │ │ 🧒   │ │ 👦   │        │
│  │Pre-K │ │Kinder│ │Grado │        │
│  └──────┘ └──────┘ └──────┘        │
│                                     │
│     ┌─────────────────┐             │
│     │   🌟 EMPEZAR   │             │
│     └─────────────────┘             │
│                                     │
│  [👨‍👩‍👧‍👦] Padres    [🌐] Idioma      │
└─────────────────────────────────────┘
```

### Especificaciones Técnicas
- **Botones de edad**: 80x100dp cada uno
- **Botón principal**: 200x60dp
- **Iconos**: 48x48dp
- **Mascota**: Área 240x180dp con animación Lottie
- **Estados**: Seleccionado (borde brillante), No seleccionado (borde suave)

### Interacciones
1. **Selección de edad**: Feedback táctil + sonido
2. **Animación mascota**: Loop cada 10 segundos
3. **Botón empezar**: Deshabilitado hasta seleccionar edad
4. **Transición**: Slide hacia la derecha (300ms)

---

## 2. Menú Principal de Materias

### Wireframe Layout
```
┌─────────────────────────────────────┐
│ [👤] Hola, María  [⭐] 47  [👨‍👩‍👧‍👦] │
│                                     │
│     🎯 Elige tu aventura             │
│                                     │
│  ┌──────────┐  ┌──────────┐        │
│  │    🔢    │  │    📚    │        │
│  │Matemáticas│  │Lenguaje  │        │
│  │   ⭐⭐⭐   │  │   ⭐⭐☆   │        │
│  │ 12/15 ✓  │  │  8/12 ✓  │        │
│  └──────────┘  └──────────┘        │
│                                     │
│  ┌──────────┐  ┌──────────┐        │
│  │    🎨    │  │    🧩    │        │
│  │Colores y │  │ Lógica   │        │
│  │ Formas   │  │    ⭐⭐☆   │        │
│  │   ⭐⭐⭐   │  │  5/10 ✓  │        │
│  └──────────┘  └──────────┘        │
│                                     │
│  ┌──────────┐  ┌──────────┐        │
│  │    🌱    │  │    🎵    │        │
│  │ Ciencias │  │  Música  │        │
│  │    ⭐☆☆   │  │    🔒    │        │
│  │  2/8 ✓   │  │ Bloqueado│        │
│  └──────────┘  └──────────┘        │
│                                     │
│    [📊] Progreso  [🏆] Logros       │
└─────────────────────────────────────┘
```

### Especificaciones Técnicas
- **Cards de materia**: 140x120dp
- **Grid**: 2 columnas, 16dp padding
- **Iconos materias**: 64x64dp
- **Estrellas progreso**: 24x24dp cada una
- **Header**: 56dp altura
- **Footer buttons**: 48dp altura

### Estados de Card
1. **Disponible**: Colores vibrantes, sombra suave
2. **Completada**: Borde dorado, checkmark
3. **Bloqueada**: Gris, icono candado, sin interacción
4. **En progreso**: Borde azul pulsante

---

## 3. Pantalla de Actividad Educativa

### Wireframe Layout - Matemáticas (Ejemplo)
```
┌─────────────────────────────────────┐
│ [←] Matemáticas    [❤❤❤] [⏱ 2:30] │
│                                     │
│      Cuenta los patitos 🦆          │
│                                     │
│    ┌─────────────────────────┐      │
│    │  🦆 🦆 🦆                │      │
│    │                         │      │
│    │     🦆 🦆               │      │
│    │                         │      │
│    └─────────────────────────┘      │
│                                     │
│       ¿Cuántos patitos hay?         │
│                                     │
│    ┌────┐  ┌────┐  ┌────┐          │
│    │ 3  │  │ 5  │  │ 7  │          │
│    └────┘  └────┘  └────┘          │
│                                     │
│                                     │
│    ┌─────────────────┐              │
│    │   ✓ RESPONDER   │              │
│    └─────────────────┘              │
│                                     │
│  [💡] Pista    [🔊] Repetir         │
└─────────────────────────────────────┘
```

### Wireframe Layout - Lectura (Ejemplo)
```
┌─────────────────────────────────────┐
│ [←] Lenguaje      [❤❤❤] [⏱ 1:45] │
│                                     │
│       Traza la letra A              │
│                                     │
│    ┌─────────────────────────┐      │
│    │         A               │      │
│    │       /   \             │      │
│    │      /     \            │      │
│    │     /_______\           │      │
│    │    /         \          │      │
│    │                         │      │
│    │   👆 Traza aquí         │      │
│    └─────────────────────────┘      │
│                                     │
│    🔊 "Ay" como en "Avión"          │
│                                     │
│    ┌─────────────────┐              │
│    │   📝 PRACTICAR  │              │
│    └─────────────────┘              │
│                                     │
│    ┌─────────────────┐              │
│    │   ➡ SIGUIENTE   │              │
│    └─────────────────┘              │
│                                     │
│  [🔊] Sonido    [↻] Repetir         │
└─────────────────────────────────────┘
```

### Especificaciones Técnicas
- **Área de contenido**: 300x200dp mínimo
- **Botones de respuesta**: 80x60dp
- **Timer**: 32sp, color distintivo
- **Vidas**: 32x32dp cada corazón
- **Botón principal**: 180x50dp
- **Área de trazado**: Multi-touch habilitado

### Elementos de Gamificación
1. **Feedback inmediato**: Animación + sonido
2. **Progreso visual**: Barra de progreso superior
3. **Sistema de vidas**: 3 intentos por actividad
4. **Tiempo opcional**: Basado en nivel de dificultad

---

## 4. Panel de Padres (Con PIN)

### Wireframe Layout - Acceso PIN
```
┌─────────────────────────────────────┐
│                                     │
│        👨‍👩‍👧‍👦 Panel de Padres        │
│                                     │
│      Ingresa tu PIN de 4 dígitos    │
│                                     │
│        ┌─────────────────┐          │
│        │  ● ● ● ○        │          │
│        └─────────────────┘          │
│                                     │
│    ┌────┐ ┌────┐ ┌────┐            │
│    │ 1  │ │ 2  │ │ 3  │            │
│    └────┘ └────┘ └────┘            │
│    ┌────┐ ┌────┐ ┌────┐            │
│    │ 4  │ │ 5  │ │ 6  │            │
│    └────┘ └────┘ └────┘            │
│    ┌────┐ ┌────┐ ┌────┐            │
│    │ 7  │ │ 8  │ │ 9  │            │
│    └────┘ └────┘ └────┘            │
│        ┌────┐ ┌────┐               │
│        │ 0  │ │ ⌫  │               │
│        └────┘ └────┘               │
│                                     │
│  [←] Volver                         │
└─────────────────────────────────────┘
```

### Wireframe Layout - Dashboard Padres
```
┌─────────────────────────────────────┐
│ [←] Panel Padres        [⚙] Config │
│                                     │
│     📊 Progreso de María            │
│                                     │
│  Tiempo hoy: 45 min   🔥 Racha: 7   │
│  Actividades: 12/15   ⭐ Total: 47  │
│                                     │
│    ┌─────────────────────────┐      │
│    │  📈 Gráfico Semanal     │      │
│    │                         │      │
│    │  ▓░░▓▓░▓               │      │
│    │  L M X J V S D         │      │
│    └─────────────────────────┘      │
│                                     │
│  📚 Materias Favoritas:             │
│  1️⃣ Matemáticas (18 actividades)    │
│  2️⃣ Colores (15 actividades)        │
│  3️⃣ Lectura (12 actividades)        │
│                                     │
│  🏆 Últimos Logros:                 │
│  • Contador Experto 🔢             │
│  • Artista de Colores 🎨           │
│  • Lector Principiante 📖          │
│                                     │
│  ┌─────────────┐ ┌─────────────┐   │
│  │📊 Reportes  │ │💎 Premium   │   │
│  └─────────────┘ └─────────────┘   │
└─────────────────────────────────────┘
```

### Especificaciones Técnicas
- **Teclado numérico**: 60x60dp por botón
- **Indicador PIN**: 40x40dp por círculo
- **Gráficos**: Chart.js integration
- **Cards estadísticas**: Full width, 60dp altura
- **Protección**: Auto-logout después 5 min

---

## 5. Pantalla de Progreso y Estadísticas

### Wireframe Layout
```
┌─────────────────────────────────────┐
│ [←] Mi Progreso     [📅] Semanal    │
│                                     │
│       🌟 ¡Excelente trabajo!        │
│                                     │
│    ┌─────────────────────────┐      │
│    │    🦁 Mascota Feliz     │      │
│    │     Nivel 3 - León      │      │
│    │   ████████████░░ 75%    │      │
│    └─────────────────────────┘      │
│                                     │
│  📊 Estadísticas Generales:         │
│                                     │
│  ⭐ Estrellas totales: 47/90        │
│  🎯 Actividades: 23/45              │
│  🔥 Racha actual: 7 días            │
│  ⏱ Tiempo total: 12h 30min         │
│                                     │
│  📚 Progreso por Materia:           │
│                                     │
│  🔢 Matemáticas     ████████░░ 80%  │
│  📖 Lenguaje        ██████░░░░ 60%  │
│  🎨 Arte            █████████░ 90%  │
│  🧩 Lógica          ████░░░░░░ 40%  │
│  🌱 Ciencias        ██░░░░░░░░ 20%  │
│                                     │
│  ┌─────────────┐ ┌─────────────┐   │
│  │🏆 Mis Logros│ │📈 Reportes  │   │
│  └─────────────┘ └─────────────┘   │
└─────────────────────────────────────┘
```

### Especificaciones Técnicas
- **Mascota área**: 200x150dp con animación
- **Barras de progreso**: 280dp ancho, 24dp alto
- **Cards de estadística**: 48dp altura cada una
- **Botones secundarios**: 120x40dp
- **Animaciones**: Progress bars animadas al cargar

---

## 6. Pantalla Premium/Upgrade

### Wireframe Layout
```
┌─────────────────────────────────────┐
│ [×] Cerrar                          │
│                                     │
│        🌟 ¡Desbloquea Todo!         │
│                                     │
│    ┌─────────────────────────┐      │
│    │     👑 PREMIUM          │      │
│    │                         │      │
│    │  • Acceso ilimitado     │      │
│    │  • 150+ actividades     │      │
│    │  • Reportes avanzados   │      │
│    │  • Sin publicidad       │      │
│    │  • Soporte prioritario  │      │
│    └─────────────────────────┘      │
│                                     │
│   🎁 3 días GRATIS, luego:          │
│                                     │
│    ┌─────────────────────────┐      │
│    │      💎 $4.99/mes       │      │
│    │    Cancela cuando       │      │
│    │        quieras          │      │
│    └─────────────────────────┘      │
│                                     │
│    ┌─────────────────┐              │
│    │  🚀 COMENZAR    │              │
│    │   TRIAL GRATIS  │              │
│    └─────────────────┘              │
│                                     │
│    ┌─────────────────┐              │
│    │  💳 COMPRAR     │              │
│    │    PREMIUM      │              │
│    └─────────────────┘              │
│                                     │
│  📋 Términos    🔒 Privacidad       │
└─────────────────────────────────────┘
```

### Especificaciones Técnicas
- **Card principal**: Full width - 32dp margin
- **Precio destacado**: 36sp, color premium
- **Botones CTA**: 200x56dp, colores distintivos
- **Lista beneficios**: 20sp, iconos 24x24dp
- **Legal**: 12sp, enlaces subrayados

---

## 7. Pantalla de Configuración

### Wireframe Layout
```
┌─────────────────────────────────────┐
│ [←] Configuración                   │
│                                     │
│  👤 Perfil del Niño                 │
│    📝 Nombre: María                 │
│    🎂 Edad: 5 años                  │
│    ┌─────────────┐                  │
│    │   EDITAR    │                  │
│    └─────────────┘                  │
│                                     │
│  🔊 Audio y Sonidos                 │
│    🎵 Música        ●━━━━━━━ 70%    │
│    🔊 Efectos       ●━━━━━━━ 80%    │
│    🗣 Voz           ●━━━━━━━ 90%    │
│                                     │
│  🌐 Idioma                          │
│    Español 🇪🇸      [Cambiar]       │
│                                     │
│  🎯 Dificultad                      │
│    ○ Fácil  ●Medio  ○Difícil        │
│                                     │
│  👨‍👩‍👧‍👦 Control Parental            │
│    📱 PIN Padres    [Cambiar]       │
│    ⏰ Límite tiempo [30 min]        │
│    📊 Reportes      [Semanal]       │
│                                     │
│  ℹ Acerca de                        │
│    📖 Versión 1.0.0                 │
│    📋 Términos de uso               │
│    🔒 Política privacidad           │
│                                     │
│  ⚠ Zona de Peligro                 │
│    🗑 Resetear progreso             │
│    ❌ Eliminar cuenta               │
└─────────────────────────────────────┘
```

### Especificaciones Técnicas
- **Secciones**: Separadores con 24dp padding
- **Sliders**: Material Design sliders
- **Switches**: 48x24dp toggles
- **Botones de peligro**: Color rojo, confirmación doble
- **Cards**: Elevation 2dp, rounded 8dp

---

## Consideraciones de Accesibilidad

### Estándares WCAG 2.1 AA
- **Contraste**: Mínimo 4.5:1 para texto normal
- **Tamaño táctil**: Mínimo 44x44dp
- **Navegación**: Tab order lógico
- **Lectores**: Semantic HTML y ARIA labels
- **Colores**: No dependencia exclusiva del color

### Adaptaciones para Niños
- **Feedback táctil**: Vibración suave en interacciones
- **Feedback visual**: Animaciones suaves y divertidas
- **Feedback auditivo**: Sonidos alegres y motivadores
- **Timeouts**: Pausas automáticas cada 20 minutos
- **Error prevention**: Confirmaciones para acciones críticas

---

## Especificaciones Responsivas

### Tamaños de Pantalla Android
```
Small (320dp):    Layout compacto, botones apilados
Medium (360dp):   Layout estándar, 2 columnas
Large (400dp+):   Layout expandido, elementos más grandes
Tablets (600dp+): Layout adaptativo, aprovechar espacio extra
```

### Orientaciones
- **Portrait**: Layout principal optimizado
- **Landscape**: Reorganización automática de elementos
- **Auto-rotate**: Configuración parental

---

## Estados de Carga y Error

### Loading States
```
┌─────────────────────────────────────┐
│                                     │
│         🎓 EduPlayKids              │
│                                     │
│    ┌─────────────────────────┐      │
│    │     🦁 Cargando...      │      │
│    │       ●●●●●●●●          │      │
│    │                         │      │
│    └─────────────────────────┘      │
│                                     │
│    Preparando tu aventura...        │
│                                     │
└─────────────────────────────────────┘
```

### Error States
```
┌─────────────────────────────────────┐
│                                     │
│         😅 ¡Ups! Algo pasó          │
│                                     │
│    ┌─────────────────────────┐      │
│    │     🦁 Mascota Triste   │      │
│    │                         │      │
│    │   No pudimos cargar     │      │
│    │   esta actividad        │      │
│    └─────────────────────────┘      │
│                                     │
│    ┌─────────────────┐              │
│    │  🔄 INTENTAR    │              │
│    │   DE NUEVO      │              │
│    └─────────────────┘              │
│                                     │
│    [←] Volver al menú               │
└─────────────────────────────────────┘
```

---

## Notas de Implementación

### Prioridades de Desarrollo
1. **Fase 1**: Pantallas core (1-3)
2. **Fase 2**: Panel padres y progreso (4-5)
3. **Fase 3**: Premium y configuración (6-7)

### Tecnologías Requeridas
- **Animaciones**: Lottie files para mascota
- **Charts**: Syncfusion Charts para estadísticas
- **Audio**: MediaElement para sonidos
- **Gestures**: PanGestureRecognizer para trazado
- **Localización**: ResourceManager para idiomas

### Testing Checklist
- [ ] Pruebas en dispositivos 5" y 6"
- [ ] Verificación de contrastes
- [ ] Test de accesibilidad con TalkBack
- [ ] Pruebas con niños reales (UAT)
- [ ] Performance en dispositivos de gama baja

---

*Documento creado para EduPlayKids v1.0 - Septiembre 2025*