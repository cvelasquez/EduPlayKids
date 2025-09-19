# ESPECIFICACIONES FUNCIONALES DETALLADAS
## EduPlayKids - Aplicación Educativa Móvil

---

### 📋 Información del Documento

| Campo | Detalle |
|-------|---------|
| **Proyecto** | EduPlayKids |
| **Versión** | 1.0 |
| **Fecha** | Septiembre 2024 |
| **Tipo** | Especificaciones Funcionales |
| **Estado** | En Desarrollo |

---

## 🎯 1. FUNCIONALIDADES PRINCIPALES

### 1.1 Configuración Inicial Simplificada

#### F001: Primer Uso de la Aplicación
**Descripción**: Flujo inicial extremadamente simple sin tutoriales complejos.

**Flujo de Usuario**:
1. **Pantalla de Bienvenida**: Logo y botón "Comenzar"
2. **Registro del Niño**:
   - Campo: Nombre del niño (máximo 50 caracteres)
   - Selector de edad: 3, 4, 5, 6, 7, 8 años
   - Botón "Crear Perfil"
3. **Detección Automática de Idioma**:
   - Sistema detecta idioma del dispositivo
   - Si no es español/inglés → predeterminado inglés
4. **Acceso Directo al Menú Principal**

**Criterios de Aceptación**:
- ✅ Flujo completo en máximo 3 pantallas
- ✅ Sin tutoriales ni explicaciones complejas
- ✅ Interfaz intuitiva para padres
- ✅ Botones grandes y coloridos

#### F002: Gestión Flexible de Edad
**Descripción**: Los padres pueden cambiar la edad del niño sin restricciones.

**Funcionalidad**:
- Acceso desde Panel Parental (con PIN)
- Cambio inmediato de contenido curricular
- Progreso anterior se mantiene intacto
- Nuevo contenido se desbloquea según nueva edad

**Criterios de Aceptación**:
- ✅ Cambio sin pérdida de progreso
- ✅ Contenido se adapta instantáneamente
- ✅ Sin confirmaciones complejas

---

## 🎮 2. SISTEMA DE NAVEGACIÓN Y CONTENIDO

### 2.1 Navegación Libre con Orden Curricular

#### F003: Menú Principal por Materias
**Descripción**: Los niños pueden elegir libremente entre materias, pero se presentan en orden curricular.

**Estructura del Menú**:
```
📚 MATEMÁTICAS
├── 🔢 Números (presentado primero)
├── ➕ Operaciones Básicas
├── 🔺 Formas y Geometría
└── 📊 Patrones

📖 LECTURA Y LENGUAJE
├── 🔤 Alfabeto (presentado primero)
├── 🔊 Fonética
├── 📝 Palabras
└── 📚 Comprensión

🧠 LÓGICA Y PENSAMIENTO
├── 🧩 Puzzles (presentado primero)
├── 🧠 Memoria
├── 🔍 Clasificación
└── 💡 Resolución Problemas

🔬 CIENCIAS
├── 🐾 Animales (presentado primero)
├── 🌱 Plantas
├── 🌤️ Clima
└── 🌍 Naturaleza

🎨 CONCEPTOS BÁSICOS
├── 🌈 Colores (presentado primero)
├── ⭕ Formas
├── 📏 Tamaños
└── 🔄 Patrones
```

**Criterios de Aceptación**:
- ✅ Acceso libre a todas las materias
- ✅ Subtemas ordenados curricularmente
- ✅ Navegación intuitiva con íconos grandes
- ✅ Sin bloqueos por completitud

#### F004: Progresión Curricular Sugerida
**Descripción**: Aunque la navegación es libre, el sistema sugiere el orden curricular optimal.

**Indicadores Visuales**:
- 🌟 **Próximo Recomendado**: Borde dorado brillante
- ✅ **Completado**: Marca verde + estrellas obtenidas
- 🔄 **En Progreso**: Borde azul + porcentaje
- 📋 **Disponible**: Diseño normal

---

### 2.2 Sistema de Actividades

#### F005: Estructura de Actividades por Edad

**Pre-K (3-4 años)**:
```
📊 MATEMÁTICAS
├── Números 1-5: Reconocimiento visual
├── Números 6-10: Conteo básico
├── Formas: Círculo, cuadrado, triángulo
└── Colores: Primarios

📚 LECTURA
├── Vocales: A, E, I, O, U
├── Consonantes: M, P, S, T, L
├── Sonidos: Reconocimiento auditivo
└── Palabras: Mamá, papá, sol

🧠 LÓGICA
├── Clasificación: Grande/pequeño
├── Memoria: 3-4 elementos
├── Puzzles: 4-6 piezas
└── Secuencias: 2-3 elementos
```

**Kindergarten (5 años)**:
```
📊 MATEMÁTICAS
├── Números 1-20: Reconocimiento y escritura
├── Suma: Hasta 10
├── Resta: Hasta 5
├── Formas: Rectángulo, óvalo, estrella
└── Patrones: AB, ABA

📚 LECTURA
├── Alfabeto completo
├── Sílabas: CV (consonante-vocal)
├── Palabras: 3-4 letras
└── Oraciones: 2-3 palabras

🧠 LÓGICA
├── Clasificación múltiple
├── Memoria: 5-6 elementos
├── Puzzles: 8-12 piezas
└── Secuencias: 4-5 elementos
```

**Grado 1-2 (6-8 años)**:
```
📊 MATEMÁTICAS
├── Números 1-100
├── Suma: Hasta 20
├── Resta: Hasta 20
├── Multiplicación: Tablas 2, 5, 10
├── Geometría: 2D y 3D básico
└── Tiempo: Horas, días, meses

📚 LECTURA
├── Sílabas complejas
├── Palabras: 5+ letras
├── Oraciones completas
├── Comprensión básica
└── Escritura: Palabras simples

🧠 LÓGICA
├── Resolución problemas
├── Memoria: 7+ elementos
├── Puzzles: 15+ piezas
└── Pensamiento crítico
```

---

## ⭐ 3. SISTEMA DE EVALUACIÓN Y PROGRESO

### 3.1 Criterio de Evaluación por Fallos

#### F006: Sistema de Calificación por Estrellas
**Descripción**: Evaluación basada en número de errores cometidos durante la actividad.

**Criterios de Evaluación**:

| Errores | Estrellas | Descripción |
|---------|-----------|-------------|
| 0 errores | ⭐⭐⭐ | Perfecto - Dorado |
| 1-2 errores | ⭐⭐ | Muy Bien - Plateado |
| 3+ errores | ⭐ | Bien - Bronce |

**Implementación**:
- Contador interno de errores por actividad
- Evaluación al finalizar cada actividad
- Animación de celebración según estrellas obtenidas
- Almacenamiento de mejor puntuación

**Criterios de Aceptación**:
- ✅ Evaluación automática e inmediata
- ✅ Feedback visual claro y motivador
- ✅ Sistema justo y comprensible para niños

#### F007: Sistema de Repetición y Mejora
**Descripción**: Los niños pueden repetir actividades indefinidamente para mejorar sus calificaciones.

**Funcionalidades**:
- **Repetir Actividad**: Botón siempre disponible
- **Mejor Puntuación**: Se guarda automáticamente
- **Histórico de Intentos**: Visible en panel parental
- **Motivación**: "¡Intenta conseguir 3 estrellas!"

**Criterios de Aceptación**:
- ✅ Sin límite de repeticiones
- ✅ Progreso se actualiza automáticamente
- ✅ Interfaz motiva la mejora continua

---

### 3.2 Dificultad Adaptativa y Logros

#### F008: Sistema de Coronas por Excelencia
**Descripción**: Cuando un niño demuestra rendimiento destacado, se le ofrece un desafío adicional para obtener una corona.

**Criterios para Activar Corona**:
- Obtener 3 estrellas en actividad
- Tiempo de respuesta promedio inferior al 75% del tiempo esperado
- Completar la actividad sin pistas adicionales

**Desafío de Corona**:
- 3-5 preguntas adicionales de la misma sección
- Dificultad ligeramente superior
- Límite de 1 error para obtener corona
- Tiempo limitado (opcional según edad)

**Recompensas**:
- 👑 **Corona Dorada**: Logro máximo
- 🎯 **Puntos Extra**: Para ranking interno
- 🎵 **Sonido Especial**: Celebración única
- 🏆 **Insignia**: Visible en perfil

**Criterios de Aceptación**:
- ✅ Sistema detecta automáticamente candidatos
- ✅ Desafío apropiado para la edad
- ✅ Motivación sin presión excesiva

---

## 🔒 4. MODELO FREEMIUM Y LIMITACIONES

### 4.1 Período Gratuito y Limitaciones

#### F009: Gestión de Período Gratuito
**Descripción**: Los primeros 3 días son completamente gratuitos, después se limita a 10 lecciones diarias.

**Implementación**:
```
Día 1-3: ACCESO ILIMITADO
├── Todas las materias disponibles
├── Sin limitación de actividades
├── Todas las funciones activas
└── Mensaje diario: "Día X de prueba gratuita"

Día 4+: MODO LIMITADO
├── Contador: "9 lecciones restantes hoy"
├── Reset automático cada 24 horas
├── Pantalla upgrade después de límite
└── Todas las funciones básicas activas
```

**Contador de Lecciones**:
- Una actividad completada = 1 lección
- Repetir actividad = NO cuenta como lección adicional
- Solo nuevas actividades o mejoras consumen contador
- Valor ajustable: inicial 10, modificable según uso

**Criterios de Aceptación**:
- ✅ Tracking preciso de días transcurridos
- ✅ Contador de lecciones visible y claro
- ✅ Reset automático diario a medianoche
- ✅ Sin pérdida de progreso al llegar al límite

#### F010: Pantalla de Upgrade Premium
**Descripción**: Cuando se agota el límite diario, se presenta opción de upgrade.

**Elementos de la Pantalla**:
- 🎓 **Título**: "¡Continúa Aprendiendo!"
- 📱 **Mensaje**: "Has completado tus 10 lecciones de hoy"
- ⏰ **Contador**: "Nuevas lecciones en: 14:32:15"
- 💎 **Botón Premium**: "Aprender sin Límites - $4.99"
- 🔙 **Botón Salir**: "Volver Mañana"

**Características Premium Mostradas**:
- ∞ Lecciones ilimitadas todos los días
- 📊 Estadísticas detalladas de progreso
- 🎯 Actividades de repaso personalizadas
- 👨‍👩‍👧‍👦 Múltiples perfiles de niños
- 📈 Informes de progreso para padres

---

## 🌐 5. SISTEMA MULTIIDIOMA

### 5.1 Gestión de Idiomas

#### F011: Cambio de Idioma Sin Restricciones
**Descripción**: El idioma puede cambiarse en cualquier momento sin afectar el progreso guardado.

**Funcionalidades**:
- **Detección Automática**: Idioma del sistema al instalar
- **Cambio Manual**: Disponible en Configuración
- **Persistencia**: Progreso independiente del idioma
- **Sincronización**: Mismas actividades en ambos idiomas

**Ubicaciones del Selector**:
- Pantalla de configuración (acceso normal)
- Panel parental (acceso con PIN)
- Pantalla principal (botón discreto)

**Criterios de Aceptación**:
- ✅ Cambio instantáneo de interfaz
- ✅ Progreso se mantiene intacto
- ✅ Audio cambia automáticamente
- ✅ Sin necesidad de reiniciar app

---

### 5.2 Sistema de Audio Pregrabado

#### F012: Especificaciones de Audio Pregrabado
**Descripción**: Todas las voces serán pregrabadas con calidad profesional en ambos idiomas.

**Estructura de Archivos de Audio**:
```
📁 Audio/
├── 📁 Spanish/
│   ├── 📁 Common/
│   │   ├── welcome_es.mp3          # "¡Bienvenido!"
│   │   ├── excellent_es.mp3        # "¡Excelente!"
│   │   ├── try_again_es.mp3        # "Inténtalo otra vez"
│   │   ├── perfect_es.mp3          # "¡Perfecto!"
│   │   └── great_job_es.mp3        # "¡Muy bien hecho!"
│   ├── 📁 Numbers/
│   │   ├── number_1_es.mp3         # "Uno"
│   │   ├── number_2_es.mp3         # "Dos"
│   │   └── count_objects_es.mp3    # "Cuenta los objetos"
│   ├── 📁 Letters/
│   │   ├── letter_a_es.mp3         # "A"
│   │   ├── sound_a_es.mp3          # Sonido de "A"
│   │   └── find_letter_es.mp3      # "Encuentra la letra"
│   └── 📁 Instructions/
│       ├── drag_drop_es.mp3        # "Arrastra y suelta"
│       ├── tap_correct_es.mp3      # "Toca la respuesta correcta"
│       └── trace_letter_es.mp3     # "Traza la letra"
├── 📁 English/
│   ├── 📁 Common/
│   │   ├── welcome_en.mp3          # "Welcome!"
│   │   ├── excellent_en.mp3        # "Excellent!"
│   │   ├── try_again_en.mp3        # "Try again"
│   │   ├── perfect_en.mp3          # "Perfect!"
│   │   └── great_job_en.mp3        # "Great job!"
│   ├── 📁 Numbers/
│   │   ├── number_1_en.mp3         # "One"
│   │   ├── number_2_en.mp3         # "Two"
│   │   └── count_objects_en.mp3    # "Count the objects"
│   ├── 📁 Letters/
│   │   ├── letter_a_en.mp3         # "A"
│   │   ├── sound_a_en.mp3          # Sound of "A"
│   │   └── find_letter_en.mp3      # "Find the letter"
│   └── 📁 Instructions/
│       ├── drag_drop_en.mp3        # "Drag and drop"
│       ├── tap_correct_en.mp3      # "Tap the correct answer"
│       └── trace_letter_en.mp3     # "Trace the letter"
└── 📁 SoundEffects/
    ├── success_sound.mp3           # Sonido éxito
    ├── error_sound.mp3             # Sonido error
    ├── star_earned.mp3             # Sonido obtener estrella
    ├── crown_earned.mp3            # Sonido obtener corona
    └── celebration.mp3             # Sonido celebración
```

---

## 👨‍👩‍👧‍👦 6. PANEL PARENTAL

### 6.1 Acceso y Seguridad

#### F013: Sistema de PIN Parental
**Descripción**: Panel parental protegido con PIN de 4 dígitos para acceso exclusivo de adultos.

**Configuración de PIN**:
- Primera configuración durante setup inicial
- PIN numérico de 4 dígitos
- Pregunta de seguridad para recuperación
- Cambio de PIN disponible en cualquier momento

**Acceso al Panel**:
- Botón discreto en pantalla principal: "👨‍👩‍👧‍👦"
- Pantalla de ingreso PIN con teclado numérico
- 3 intentos fallidos = bloqueo temporal (15 min)
- Opción "¿Olvidaste tu PIN?" con pregunta seguridad

**Criterios de Aceptación**:
- ✅ PIN seguro y fácil de recordar para padres
- ✅ Inaccesible para niños pequeños
- ✅ Recuperación de PIN sin perder datos

---

### 6.2 Funcionalidades del Panel Parental

#### F014: Estadísticas de Progreso Detalladas
**Descripción**: Dashboard completo del progreso educativo del niño.

**Dashboard Principal**:
```
👶 PERFIL DEL NIÑO
├── Nombre: [Editable]
├── Edad: [Modificable - selector 3-8]
├── Fecha registro: XX/XX/XXXX
└── Días activos: XX días

📊 RESUMEN DE PROGRESO
├── Actividades completadas: XXX/XXX
├── Estrellas totales: XXX ⭐
├── Coronas obtenidas: XX 👑
├── Tiempo total jugado: XX horas XX min
└── Promedio diario: XX minutos

📈 PROGRESO POR MATERIA
├── 📊 Matemáticas: 85% - 124⭐ - 5👑
├── 📚 Lectura: 72% - 98⭐ - 3👑
├── 🧠 Lógica: 91% - 156⭐ - 8👑
├── 🔬 Ciencias: 65% - 87⭐ - 2👑
└── 🎨 Conceptos: 88% - 134⭐ - 6👑

📅 ACTIVIDAD RECIENTE (7 días)
├── Lun: 15 actividades - 45 min
├── Mar: 12 actividades - 38 min
├── Mié: 18 actividades - 52 min
├── Jue: 10 actividades - 28 min
├── Vie: 22 actividades - 67 min
├── Sáb: 8 actividades - 22 min
└── Dom: 14 actividades - 41 min

🎯 ÁREAS DE OPORTUNIDAD
├── Resta básica: 2⭐ promedio
├── Comprensión lectora: 1⭐ promedio
└── Clasificación: Pendiente
```

#### F015: Gestión de Perfiles Múltiples (Premium)
**Descripción**: Capacidad de crear y gestionar múltiples perfiles de niños.

**Funcionalidades Premium**:
- Hasta 5 perfiles de niños diferentes
- Progreso independiente por perfil
- Estadísticas comparativas entre hermanos
- Cambio rápido de perfil activo

**Interface de Gestión**:
```
👨‍👩‍👧‍👦 MIS HIJOS
├── 👧 María (5 años) - Activo
│   ├── 📊 Ver estadísticas
│   ├── ✏️ Editar perfil
│   └── 🎯 Establecer metas
├── 👦 Carlos (7 años)
│   ├── 📊 Ver estadísticas
│   ├── ✏️ Editar perfil
│   └── 🎯 Establecer metas
└── ➕ Agregar nuevo hijo (3 restantes)
```

#### F016: Configuraciones y Premium
**Descripción**: Configuraciones de la aplicación y gestión de suscripción premium.

**Sección de Configuraciones**:
```
⚙️ CONFIGURACIÓN
├── 🌐 Idioma: Español/English
├── 🔊 Volumen: [Slider]
├── 🎵 Música de fondo: ON/OFF
├── 🔔 Notificaciones: ON/OFF
├── 🛡️ Cambiar PIN
└── 📱 Información de la app

💎 PREMIUM
├── Estado: Gratuito/Premium
├── Lecciones restantes hoy: X/10
├── 🔄 Próximo reset: XX:XX:XX
├── 💳 Adquirir Premium - $4.99
└── 📄 Términos y condiciones

📊 EXPORTAR DATOS
├── 📈 Generar reporte PDF
├── 📧 Enviar por email
└── 💾 Guardar en dispositivo
```

---

## 🎵 7. EXPERIENCIA DE USUARIO Y MULTIMEDIA

### 7.1 Feedback y Motivación

#### F017: Sistema de Retroalimentación Inmediata
**Descripción**: Respuesta inmediata y motivadora a cada acción del niño.

**Tipos de Feedback**:

**Respuesta Correcta**:
- ✅ Animación de checkmark verde
- 🎵 Sonido de éxito alegre
- 🗣️ Audio de felicitación ("¡Muy bien!", "¡Excelente!")
- ⭐ Contador de estrellas se actualiza visualmente

**Respuesta Incorrecta**:
- ❌ Animación suave de error (sin ser negativa)
- 🔊 Sonido neutral (no traumático)
- 🗣️ Audio motivador ("Inténtalo otra vez", "Casi lo tienes")
- 💡 Pista visual sutil si es posible

**Completar Actividad**:
- 🎉 Celebración animada según estrellas obtenidas
- 🎵 Música de celebración
- ⭐ Animación de estrellas cayendo
- 📊 Pantalla de resumen de logros

#### F018: Personaje Mascota Motivadora
**Descripción**: Mascota virtual que acompaña y motiva durante el aprendizaje.

**Características de la Mascota**:
- 🐱 **Diseño**: Amigable y colorido (neutro de género)
- 😊 **Expresiones**: Cambian según el progreso del niño
- 🎭 **Animaciones**: Celebra éxitos, anima en errores
- 🌟 **Evolución**: Crece/mejora con el progreso del niño

**Estados de la Mascota**:
```
😊 NEUTRO: Estado inicial, expectante
🎉 CELEBRANDO: Cuando el niño acierta
💪 ANIMANDO: Cuando el niño se equivoca
👑 ORGULLOSA: Cuando se obtiene corona
😴 DESCANSANDO: En modo de pausa
```

---

## 📱 8. ESPECIFICACIONES TÉCNICAS DE INTERFACE

### 8.1 Diseño Adaptado para Niños

#### F019: Elementos de Interface Infantil
**Descripción**: Diseño específicamente optimizado para usuarios de 3-8 años.

**Especificaciones de Botones**:
- **Tamaño Mínimo**: 60dp (mayor que estándar 44dp)
- **Espaciado**: Mínimo 16dp entre botones
- **Forma**: Redondeada con bordes suaves
- **Colores**: Vibrantes pero no estridentes
- **Iconos**: Grandes, simples y universalmente reconocibles

**Especificaciones de Texto**:
- **Fuente**: Sans-serif, clara y legible
- **Tamaño**: Mínimo 18sp para contenido
- **Contraste**: Mínimo 4.5:1 ratio
- **Colores**: Alto contraste sobre fondos

**Navegación**:
- **Botón Atrás**: Siempre visible y accesible
- **Menú**: Iconográfico, sin texto complejo
- **Transiciones**: Suaves y no demasiado rápidas
- **Orientación**: Portrait preferido, landscape opcional

#### F020: Sistema de Colores y Temas
**Descripción**: Paleta de colores amigable y accesible para niños.

**Paleta Principal**:
```
🟡 AMARILLO: #FFD700 - Matemáticas
🔵 AZUL: #4A90E2 - Lectura
🟣 MORADO: #9B59B6 - Lógica
🟢 VERDE: #27AE60 - Ciencias
🔴 ROJO: #E74C3C - Conceptos Básicos

🎨 COLORES SECUNDARIOS:
├── Éxito: #2ECC71 (Verde)
├── Error: #F39C12 (Naranja, no rojo traumático)
├── Neutro: #95A5A6 (Gris)
├── Fondo: #FFFFFF (Blanco)
└── Texto: #2C3E50 (Gris oscuro)
```

---

## 📋 9. MATRIZ DE TRAZABILIDAD DE REQUISITOS

### 9.1 Mapeo Requisitos vs Funcionalidades

| ID Requisito | Funcionalidad | Prioridad | Estado |
|--------------|---------------|-----------|---------|
| **REQ-001** | Configuración simple (nombre + edad) | Alta | ✅ F001 |
| **REQ-002** | Cambio de edad flexible | Media | ✅ F002 |
| **REQ-003** | Navegación libre con orden curricular | Alta | ✅ F003, F004 |
| **REQ-004** | Evaluación por número de errores | Alta | ✅ F006 |
| **REQ-005** | Repetición ilimitada de actividades | Media | ✅ F007 |
| **REQ-006** | Dificultad adaptativa con coronas | Media | ✅ F008 |
| **REQ-007** | Límite 10 lecciones después 3 días | Alta | ✅ F009, F010 |
| **REQ-008** | Sin contenido premium en MVP | Baja | ✅ Documentado |
| **REQ-009** | Cambio idioma sin afectar progreso | Media | ✅ F011 |
| **REQ-010** | Audio pregrabado bilingüe | Alta | ✅ F012 |
| **REQ-011** | Panel parental con PIN | Alta | ✅ F013, F014, F015, F016 |

---

## 🎯 10. CRITERIOS DE ACEPTACIÓN GENERALES

### 10.1 Usabilidad para Niños
- ✅ Un niño de 5 años puede navegar sin ayuda adulta
- ✅ Máximo 2 toques para acceder a cualquier actividad
- ✅ Feedback visual inmediato a toda acción
- ✅ Sin elementos que puedan confundir o frustrar

### 10.2 Funcionalidad Core
- ✅ Todas las actividades funcionan sin internet
- ✅ Progreso se guarda automáticamente
- ✅ Cambio de idioma instantáneo sin pérdida datos
- ✅ Sistema freemium funciona correctamente

### 10.3 Rendimiento
- ✅ Carga inicial < 3 segundos
- ✅ Respuesta a toque < 200ms
- ✅ Transición entre pantallas < 1 segundo
- ✅ Audio se reproduce sin retrasos

### 10.4 Calidad de Contenido
- ✅ Contenido alineado con estándares curriculares USA
- ✅ Progresión de dificultad apropiada por edad
- ✅ Audio claro y pronunciación correcta
- ✅ Actividades educativamente válidas

---

## 📝 11. NOTAS DE IMPLEMENTACIÓN

### 11.1 Consideraciones de Desarrollo
- **Base de Datos**: SQLite con esquema flexible para futuras expansiones
- **Localización**: Arquitectura preparada para idiomas adicionales
- **Analytics**: Solo datos locales, cumplimiento COPPA/GDPR-K
- **Caching**: Contenido multimedia precargado para experiencia fluida

### 11.2 Futuras Expansiones (Post-MVP)
- Cursos premium especializados (programación, música, magia)
- Modo multijugador para hermanos
- Integración con sistemas educativos escolares
- Reportes avanzados para educadores
- Soporte para dispositivos tablets específicos

---

*Este documento establece las especificaciones funcionales completas para EduPlayKids v1.0. Todas las funcionalidades están diseñadas para cumplir con la fecha de lanzamiento del 30 de octubre de 2024.*