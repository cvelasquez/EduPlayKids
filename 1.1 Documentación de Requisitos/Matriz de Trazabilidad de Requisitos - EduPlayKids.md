# MATRIZ DE TRAZABILIDAD DE REQUISITOS
## EduPlayKids - Aplicación Educativa Móvil

---

### 📋 Información del Documento

| Campo | Detalle |
|-------|---------|
| **Proyecto** | EduPlayKids |
| **Versión** | 1.0 |
| **Fecha** | Septiembre 2024 |
| **Tipo** | Matriz de Trazabilidad |
| **Alcance** | Requisitos Funcionales (Negocio y Técnicos) |

---

## 🎯 1. MÓDULO: CONFIGURACIÓN Y ONBOARDING

### 1.1 Requisitos de Negocio

| ID | Requisito de Negocio | Prioridad | Funcionalidad | Caso de Prueba |
|----|--------------------|-----------|---------------|----------------|
| **RN-001** | La app debe permitir registro simple con solo nombre y edad | Alta | F001: Primer Uso de la Aplicación | CP-001: Validar registro exitoso<br/>CP-002: Validar campos obligatorios |
| **RN-002** | Debe detectar automáticamente el idioma del sistema | Media | F011: Cambio de Idioma Sin Restricciones | CP-003: Validar detección automática idioma<br/>CP-004: Validar fallback a inglés |
| **RN-003** | Los padres deben poder cambiar la edad del niño sin restricciones | Media | F002: Gestión Flexible de Edad | CP-005: Validar cambio de edad<br/>CP-006: Validar preservación de progreso |

### 1.2 Requisitos Técnicos

| ID | Requisito Técnico | Prioridad | Funcionalidad | Caso de Prueba |
|----|------------------|-----------|---------------|----------------|
| **RT-001** | Almacenar configuración en SQLite local | Alta | F001: Primer Uso de la Aplicación | CP-007: Validar persistencia de datos<br/>CP-008: Validar integridad BD |
| **RT-002** | Implementar detección de idioma del sistema operativo | Media | F011: Cambio de Idioma Sin Restricciones | CP-009: Validar API de detección idioma<br/>CP-010: Validar compatibilidad SO |

---

## 🎮 2. MÓDULO: NAVEGACIÓN Y CONTENIDO EDUCATIVO

### 2.1 Requisitos de Negocio

| ID | Requisito de Negocio | Prioridad | Funcionalidad | Caso de Prueba |
|----|--------------------|-----------|---------------|----------------|
| **RN-004** | Los niños deben poder navegar libremente entre materias | Alta | F003: Menú Principal por Materias | CP-011: Validar acceso libre a materias<br/>CP-012: Validar navegación intuitiva |
| **RN-005** | Presentar contenido en orden curricular sugerido | Alta | F004: Progresión Curricular Sugerida | CP-013: Validar orden curricular<br/>CP-014: Validar indicadores visuales |
| **RN-006** | Contenido debe estar alineado con estándares curriculares USA | Alta | F005: Estructura de Actividades por Edad | CP-015: Validar alineación curricular<br/>CP-016: Validar progresión por edad |
| **RN-007** | Debe permitir repetición ilimitada de actividades | Media | F007: Sistema de Repetición y Mejora | CP-017: Validar repeticiones ilimitadas<br/>CP-018: Validar mejora de puntuación |

### 2.2 Requisitos Técnicos

| ID | Requisito Técnico | Prioridad | Funcionalidad | Caso de Prueba |
|----|------------------|-----------|---------------|----------------|
| **RT-003** | Implementar estructura modular por materias educativas | Alta | F005: Estructura de Actividades por Edad | CP-019: Validar arquitectura modular<br/>CP-020: Validar escalabilidad |
| **RT-004** | Almacenar progreso local sin conectividad | Alta | F007: Sistema de Repetición y Mejora | CP-021: Validar funcionamiento offline<br/>CP-022: Validar sincronización local |

---

## ⭐ 3. MÓDULO: EVALUACIÓN Y GAMIFICACIÓN

### 3.1 Requisitos de Negocio

| ID | Requisito de Negocio | Prioridad | Funcionalidad | Caso de Prueba |
|----|--------------------|-----------|---------------|----------------|
| **RN-008** | Sistema de evaluación basado en número de errores | Alta | F006: Sistema de Calificación por Estrellas | CP-023: Validar cálculo de estrellas<br/>CP-024: Validar criterios evaluación |
| **RN-009** | Implementar dificultad adaptativa con sistema de coronas | Media | F008: Sistema de Coronas por Excelencia | CP-025: Validar detección rendimiento<br/>CP-026: Validar desafíos adicionales |
| **RN-010** | Proporcionar feedback motivador inmediato | Alta | F017: Sistema de Retroalimentación Inmediata | CP-027: Validar feedback positivo<br/>CP-028: Validar respuesta a errores |
| **RN-011** | Incluir mascota motivadora que evoluciona | Media | F018: Personaje Mascota Motivadora | CP-029: Validar animaciones mascota<br/>CP-030: Validar evolución progreso |

### 3.2 Requisitos Técnicos

| ID | Requisito Técnico | Prioridad | Funcionalidad | Caso de Prueba |
|----|------------------|-----------|---------------|----------------|
| **RT-005** | Algoritmo de evaluación automática por errores | Alta | F006: Sistema de Calificación por Estrellas | CP-031: Validar algoritmo evaluación<br/>CP-032: Validar precisión cálculos |
| **RT-006** | Sistema de detección de rendimiento para coronas | Media | F008: Sistema de Coronas por Excelencia | CP-033: Validar algoritmo detección<br/>CP-034: Validar triggers automáticos |
| **RT-007** | Motor de animaciones para feedback visual | Alta | F017: Sistema de Retroalimentación Inmediata | CP-035: Validar rendimiento animaciones<br/>CP-036: Validar sincronización audio-visual |

---

## 💎 4. MÓDULO: MODELO FREEMIUM

### 4.1 Requisitos de Negocio

| ID | Requisito de Negocio | Prioridad | Funcionalidad | Caso de Prueba |
|----|--------------------|-----------|---------------|----------------|
| **RN-012** | Período gratuito de 3 días completos | Alta | F009: Gestión de Período Gratuito | CP-037: Validar 3 días gratuitos<br/>CP-038: Validar transición a limitado |
| **RN-013** | Límite de 10 lecciones diarias después del período gratuito | Alta | F009: Gestión de Período Gratuito | CP-039: Validar contador lecciones<br/>CP-040: Validar reset diario |
| **RN-014** | Repetir actividades no consume lecciones adicionales | Media | F009: Gestión de Período Gratuito | CP-041: Validar lógica repeticiones<br/>CP-042: Validar conservación contador |
| **RN-015** | Pantalla de upgrade clara y no agresiva | Media | F010: Pantalla de Upgrade Premium | CP-043: Validar UX upgrade<br/>CP-044: Validar información premium |
| **RN-016** | Precio premium de $4.99 USD | Alta | F010: Pantalla de Upgrade Premium | CP-045: Validar precio correcto<br/>CP-046: Validar proceso compra |

### 4.2 Requisitos Técnicos

| ID | Requisito Técnico | Prioridad | Funcionalidad | Caso de Prueba |
|----|------------------|-----------|---------------|----------------|
| **RT-008** | Sistema de tracking temporal desde primera instalación | Alta | F009: Gestión de Período Gratuito | CP-047: Validar tracking tiempo<br/>CP-048: Validar persistencia fecha |
| **RT-009** | Contador de lecciones con reset automático diario | Alta | F009: Gestión de Período Gratuito | CP-049: Validar lógica contador<br/>CP-050: Validar reset medianoche |
| **RT-010** | Integración con sistema de pagos de Google Play | Alta | F010: Pantalla de Upgrade Premium | CP-051: Validar integración pagos<br/>CP-052: Validar manejo errores pago |

---

## 🌐 5. MÓDULO: MULTIIDIOMA Y AUDIO

### 5.1 Requisitos de Negocio

| ID | Requisito de Negocio | Prioridad | Funcionalidad | Caso de Prueba |
|----|--------------------|-----------|---------------|----------------|
| **RN-017** | Soporte completo para español e inglés | Alta | F011: Cambio de Idioma Sin Restricciones | CP-053: Validar traducción completa<br/>CP-054: Validar cambio dinámico |
| **RN-018** | Cambio de idioma no debe afectar progreso guardado | Alta | F011: Cambio de Idioma Sin Restricciones | CP-055: Validar preservación progreso<br/>CP-056: Validar continuidad datos |
| **RN-019** | Audio pregrabado profesional en ambos idiomas | Alta | F012: Especificaciones de Audio Pregrabado | CP-057: Validar calidad audio<br/>CP-058: Validar sincronización idioma |

### 5.2 Requisitos Técnicos

| ID | Requisito Técnico | Prioridad | Funcionalidad | Caso de Prueba |
|----|------------------|-----------|---------------|----------------|
| **RT-011** | Sistema de localización con archivos .resx | Alta | F011: Cambio de Idioma Sin Restricciones | CP-059: Validar arquitectura localización<br/>CP-060: Validar carga recursos |
| **RT-012** | Biblioteca de audio organizada por idioma y categoría | Alta | F012: Especificaciones de Audio Pregrabado | CP-061: Validar organización archivos<br/>CP-062: Validar carga eficiente audio |
| **RT-013** | Sistema de fallback para recursos faltantes | Media | F011: Cambio de Idioma Sin Restricciones | CP-063: Validar fallback recursos<br/>CP-064: Validar manejo errores |

---

## 👨‍👩‍👧‍👦 6. MÓDULO: PANEL PARENTAL

### 6.1 Requisitos de Negocio

| ID | Requisito de Negocio | Prioridad | Funcionalidad | Caso de Prueba |
|----|--------------------|-----------|---------------|----------------|
| **RN-020** | Acceso seguro mediante PIN de 4 dígitos | Alta | F013: Sistema de PIN Parental | CP-065: Validar seguridad PIN<br/>CP-066: Validar recuperación PIN |
| **RN-021** | Estadísticas detalladas de progreso del niño | Alta | F014: Estadísticas de Progreso Detalladas | CP-067: Validar exactitud estadísticas<br/>CP-068: Validar visualización datos |
| **RN-022** | Gestión de múltiples perfiles de niños (Premium) | Media | F015: Gestión de Perfiles Múltiples | CP-069: Validar múltiples perfiles<br/>CP-070: Validar aislamiento datos |
| **RN-023** | Configuración de app y gestión premium | Alta | F016: Configuraciones y Premium | CP-071: Validar configuraciones<br/>CP-072: Validar gestión suscripción |

### 6.2 Requisitos Técnicos

| ID | Requisito Técnico | Prioridad | Funcionalidad | Caso de Prueba |
|----|------------------|-----------|---------------|----------------|
| **RT-014** | Encriptación de PIN y datos sensibles | Alta | F013: Sistema de PIN Parental | CP-073: Validar encriptación datos<br/>CP-074: Validar seguridad almacenamiento |
| **RT-015** | Generación de reportes estadísticos en tiempo real | Alta | F014: Estadísticas de Progreso Detalladas | CP-075: Validar cálculos tiempo real<br/>CP-076: Validar rendimiento consultas |
| **RT-016** | Base de datos multi-perfil con aislamiento | Media | F015: Gestión de Perfiles Múltiples | CP-077: Validar aislamiento perfiles<br/>CP-078: Validar integridad relacional |

---

## 🎵 7. MÓDULO: EXPERIENCIA DE USUARIO

### 7.1 Requisitos de Negocio

| ID | Requisito de Negocio | Prioridad | Funcionalidad | Caso de Prueba |
|----|--------------------|-----------|---------------|----------------|
| **RN-024** | Interfaz optimizada para niños de 3-8 años | Alta | F019: Elementos de Interface Infantil | CP-079: Validar usabilidad niños<br/>CP-080: Validar accesibilidad |
| **RN-025** | Diseño colorido y atractivo sin ser agresivo | Media | F020: Sistema de Colores y Temas | CP-081: Validar paleta colores<br/>CP-082: Validar contraste accesible |
| **RN-026** | Navegación intuitiva sin ayuda adulta | Alta | F019: Elementos de Interface Infantil | CP-083: Validar navegación autónoma<br/>CP-084: Validar simplicidad interface |

### 7.2 Requisitos Técnicos

| ID | Requisito Técnico | Prioridad | Funcionalidad | Caso de Prueba |
|----|------------------|-----------|---------------|----------------|
| **RT-017** | Botones mínimo 60dp con espaciado adecuado | Alta | F019: Elementos de Interface Infantil | CP-085: Validar dimensiones botones<br/>CP-086: Validar espaciado elementos |
| **RT-018** | Renderizado fluido de animaciones | Media | F017: Sistema de Retroalimentación Inmediata | CP-087: Validar performance animaciones<br/>CP-088: Validar fluidez transiciones |
| **RT-019** | Optimización para dispositivos de gama baja | Media | F019: Elementos de Interface Infantil | CP-089: Validar rendimiento gama baja<br/>CP-090: Validar uso memoria |

---

## 📊 8. RESUMEN ESTADÍSTICO DE TRAZABILIDAD

### 8.1 Distribución por Módulos

| Módulo | Req. Negocio | Req. Técnicos | Funcionalidades | Casos Prueba |
|--------|--------------|---------------|-----------------|--------------|
| **Configuración y Onboarding** | 3 | 2 | 3 | 10 |
| **Navegación y Contenido** | 4 | 2 | 4 | 12 |
| **Evaluación y Gamificación** | 4 | 3 | 4 | 14 |
| **Modelo Freemium** | 5 | 3 | 2 | 16 |
| **Multiidioma y Audio** | 3 | 3 | 2 | 12 |
| **Panel Parental** | 4 | 3 | 4 | 14 |
| **Experiencia de Usuario** | 3 | 3 | 3 | 12 |
| **TOTALES** | **26** | **19** | **22** | **90** |

### 8.2 Distribución por Prioridad

| Prioridad | Requisitos Negocio | Requisitos Técnicos | Total |
|-----------|-------------------|-------------------|-------|
| **Alta** | 18 (69%) | 13 (68%) | 31 (69%) |
| **Media** | 8 (31%) | 6 (32%) | 14 (31%) |
| **Baja** | 0 (0%) | 0 (0%) | 0 (0%) |

---

## 🔍 9. MATRIZ DE COBERTURA

### 9.1 Cobertura Requisitos → Funcionalidades

| Estado de Mapeo | Cantidad | Porcentaje |
|-----------------|----------|------------|
| ✅ **Completamente Mapeados** | 45 | 100% |
| ⚠️ **Parcialmente Mapeados** | 0 | 0% |
| ❌ **Sin Mapear** | 0 | 0% |

### 9.2 Cobertura Funcionalidades → Casos de Prueba

| Estado de Cobertura | Cantidad | Porcentaje |
|--------------------|----------|------------|
| ✅ **Completamente Cubiertos** | 22 | 100% |
| ⚠️ **Parcialmente Cubiertos** | 0 | 0% |
| ❌ **Sin Cobertura** | 0 | 0% |

---

## 📋 10. CASOS DE PRUEBA CRÍTICOS

### 10.1 Casos de Prueba de Alta Prioridad

| ID | Descripción | Módulo | Criticidad |
|----|-------------|--------|------------|
| **CP-001** | Validar registro exitoso nuevo usuario | Configuración | Crítica |
| **CP-023** | Validar cálculo correcto de estrellas | Evaluación | Crítica |
| **CP-037** | Validar funcionamiento 3 días gratuitos | Freemium | Crítica |
| **CP-053** | Validar traducción completa ambos idiomas | Multiidioma | Crítica |
| **CP-065** | Validar seguridad sistema PIN | Panel Parental | Crítica |
| **CP-079** | Validar usabilidad para niños 3-8 años | UX | Crítica |

### 10.2 Casos de Prueba de Integración

| ID | Descripción | Módulos Involucrados | Tipo |
|----|-------------|---------------------|------|
| **CP-INT-001** | Validar cambio idioma preserva progreso | Multiidioma + Evaluación | Integración |
| **CP-INT-002** | Validar contador lecciones con repeticiones | Freemium + Evaluación | Integración |
| **CP-INT-003** | Validar estadísticas reflejan progreso real | Panel Parental + Evaluación | Integración |
| **CP-INT-004** | Validar mascota evoluciona con estrellas | UX + Evaluación | Integración |

---

## 📝 11. CRITERIOS DE ACEPTACIÓN GENERALES

### 11.1 Definición de "Listo" (Definition of Done)

Para que un requisito se considere completamente implementado debe cumplir:

- ✅ **Requisito implementado** según especificaciones funcionales
- ✅ **Funcionalidad probada** con casos de prueba asociados
- ✅ **Código revisado** y documentado
- ✅ **Pruebas automatizadas** creadas (cuando aplique)
- ✅ **Probado en dispositivos target** (Android gama baja/media/alta)
- ✅ **Validado por stakeholder** de negocio

### 11.2 Criterios de Aceptación Específicos

| Categoría | Criterio |
|-----------|----------|
| **Funcionalidad** | 100% de casos de prueba pasan |
| **Usabilidad** | Niño de 5 años puede usar sin ayuda |
| **Performance** | Respuesta < 200ms en gama baja |
| **Compatibilidad** | Funciona en Android API 21+ |
| **Localización** | Textos y audio 100% traducidos |
| **Seguridad** | Cumple COPPA y GDPR-K |

---

*Esta matriz de trazabilidad asegura que todos los requisitos funcionales de EduPlayKids están correctamente mapeados a funcionalidades específicas y casos de prueba verificables, garantizando una cobertura completa del alcance del proyecto.*