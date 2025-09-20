# Índice de Documentación - EduPlayKids

Este documento proporciona una vista completa y navegable de toda la documentación del proyecto EduPlayKids.

## 📋 Documentación Principal

### 📁 Archivos Raíz
- **[README.md](../README.md)** - Introducción principal del proyecto
- **[INSTALL.md](../INSTALL.md)** - Guía completa de instalación
- **[CONTRIBUTING.md](../CONTRIBUTING.md)** - Guía de contribución al proyecto
- **[CLAUDE.md](../CLAUDE.md)** - Instrucciones para Claude Code

## 🏗️ Documentación de Diseño (Completada ✅)

### Fase 1: Requisitos
- **[1.1 Documentación de Requisitos/](../1.1%20Documentación%20de%20Requisitos/)**
  - [PRD - EduPlayKids.md](../1.1%20Documentación%20de%20Requisitos/PRD%20-%20EduPlayKids.md) - Product Requirements Document
  - [Especificaciones Funcionales - EduPlayKids.md](../1.1%20Documentación%20de%20Requisitos/Especificaciones%20Funcionales%20-%20EduPlayKids.md) - 22 funcionalidades detalladas
  - [Matriz de Trazabilidad de Requisitos - EduPlayKids.md](../1.1%20Documentación%20de%20Requisitos/Matriz%20de%20Trazabilidad%20de%20Requisitos%20-%20EduPlayKids.md) - Trazabilidad completa
  - [Análisis de Stakeholders y Usuarios Objetivo - EduPlayKids.md](../1.1%20Documentación%20de%20Requisitos/Análisis%20de%20Stakeholders%20y%20Usuarios%20Objetivo%20-%20EduPlayKids.md) - Análisis de mercado

### Fase 2: Arquitectura del Sistema
- **[1.2 Arquitectura del Sistema/](../1.2%20Arquitectura%20del%20Sistema/)**
  - [Documento de Arquitectura de Software (SAD) - EduPlayKids.md](../1.2%20Arquitectura%20del%20Sistema/Documento%20de%20Arquitectura%20de%20Software%20(SAD)%20-%20EduPlayKids.md) - Clean Architecture + MVVM
  - [Diagramas de Arquitectura (C4 Model) - EduPlayKids.md](../1.2%20Arquitectura%20del%20Sistema/Diagramas%20de%20Arquitectura%20(C4%20Model)%20-%20EduPlayKids.md) - Diagramas completos C4
  - [Especificaciones Técnicas de Componentes - EduPlayKids.md](../1.2%20Arquitectura%20del%20Sistema/Especificaciones%20Técnicas%20de%20Componentes%20-%20EduPlayKids.md) - Componentes técnicos
  - [Decisiones Arquitectónicas (ADR) - EduPlayKids.md](../1.2%20Arquitectura%20del%20Sistema/Decisiones%20Arquitectónicas%20(ADR)%20-%20EduPlayKids.md) - Registros de decisiones

### Fase 2.5: Base de Datos
- **[1.3 Diseño de Base de Datos/](../1.3%20Diseño%20de%20Base%20de%20Datos/)**
  - [Modelo Entidad-Relación (ERD) - EduPlayKids.md](../1.3%20Diseño%20de%20Base%20de%20Datos/Modelo%20Entidad-Relación%20(ERD)%20-%20EduPlayKids.md) - Modelo de datos completo
  - [Esquema de Base de Datos SQLite - EduPlayKids.md](../1.3%20Diseño%20de%20Base%20de%20Datos/Esquema%20de%20Base%20de%20Datos%20SQLite%20-%20EduPlayKids.md) - Esquema SQLite optimizado
  - [Diccionario de Datos - EduPlayKids.md](../1.3%20Diseño%20de%20Base%20de%20Datos/Diccionario%20de%20Datos%20-%20EduPlayKids.md) - Documentación completa de tablas
  - [Scripts de Migración - EduPlayKids.md](../1.3%20Diseño%20de%20Base%20de%20Datos/Scripts%20de%20Migración%20-%20EduPlayKids.md) - Entity Framework Core

### Fase 3: Diseño UX/UI
- **[2.1 Diseño UXUI/](../2.1%20Diseño%20UXUI/)**
  - [user-journey-maps.md](../2.1%20Diseño%20UXUI/user-journey-maps.md) - Journey maps por edad
  - [wireframes-pantallas.md](../2.1%20Diseño%20UXUI/wireframes-pantallas.md) - Wireframes child-friendly
  - [mockups-alta-fidelidad.md](../2.1%20Diseño%20UXUI/mockups-alta-fidelidad.md) - Mockups visuales
  - [guia-estilo.md](../2.1%20Diseño%20UXUI/guia-estilo.md) - Guía de estilo completa
  - [sistema-diseno.md](../2.1%20Diseño%20UXUI/sistema-diseno.md) - Sistema de diseño técnico

### Fase 3.5: Contenido Educativo
- **[2.2 Especificaciones de Contenido/](../2.2%20Especificaciones%20de%20Contenido/)**
  - [Taxonomía de Contenido Educativo.md](../2.2%20Especificaciones%20de%20Contenido/Taxonomía%20de%20Contenido%20Educativo.md) - Estructura de conocimiento
  - [Matriz de Progresión Curricular.md](../2.2%20Especificaciones%20de%20Contenido/Matriz%20de%20Progresión%20Curricular.md) - Secuencia de aprendizaje
  - [Especificaciones de Actividades.md](../2.2%20Especificaciones%20de%20Contenido/Especificaciones%20de%20Actividades.md) - 450+ actividades especificadas
  - [Assets Multimedia (especificaciones).md](../2.2%20Especificaciones%20de%20Contenido/Assets%20Multimedia%20(especificaciones).md) - 2,500+ assets multimedia

## 🔧 Documentación Técnica

### 📁 Estructura del Proyecto (REORGANIZADO ✅)

**Importante**: El proyecto EduPlayKids utiliza una arquitectura **documentation-first** con separación clara entre documentación y código fuente:

```
📁 EduPlayKids/ (raíz)
├── 📋 CLAUDE.md, README.md
├── 📁 docs/ (Hub de documentación técnica)
├── 📁 1.1-2.2 Documentación de Diseño/ (Fases completadas)
├── 📁 app/ ⭐ CÓDIGO FUENTE .NET MAUI
│   ├── 🔧 EduPlayKids.sln (Solución principal)
│   ├── 📁 src/ (Clean Architecture + MVVM)
│   └── 📁 tests/ (Pruebas por capas)
└── 📁 tools/ (Herramientas de desarrollo)
```

**Para Desarrolladores**: Todos los comandos de desarrollo deben ejecutarse desde el directorio `app/`:
```bash
cd app          # Navegar al código fuente
dotnet build    # Compilar la solución
dotnet test     # Ejecutar pruebas
```

### 🚀 Fase 4: Implementación Técnica (NUEVA ✅)
- **[technical/development/](technical/development/)**
  - **[phase-4-implementation-guide.md](technical/development/phase-4-implementation-guide.md)** - Guía completa de implementación Phase 4 ✅
  - **[project-structure.md](technical/development/project-structure.md)** - Estructura detallada del proyecto .NET MAUI ✅
  - **[coding-standards.md](technical/development/coding-standards.md)** - Estándares de codificación para niños ✅

### 🛠️ Instalación y Configuración
- **[technical/setup-and-installation/](technical/setup-and-installation/)**
  - **[developer-setup.md](technical/setup-and-installation/developer-setup.md)** - Configuración completa del entorno de desarrollo ✅
  - **[database-setup.md](technical/setup-and-installation/database-setup.md)** - Configuración SQLite + Entity Framework Core ✅

### 🏗️ Arquitectura y Diseño
- **[technical/architecture/](technical/architecture/)**
  - **[overview.md](technical/architecture/overview.md)** - Visión general de Clean Architecture + MVVM ✅

### 📊 Base de Datos y Persistencia
- **[technical/database/](technical/database/)**
  - Setup y configuración de SQLite
  - Migraciones y Entity Framework Core
  - Optimización de rendimiento

### 🔌 Documentación de API
- **[technical/api-documentation/](technical/api-documentation/)**
  - Interfaces de servicios
  - Contratos de datos
  - Ejemplos de uso

### 🚀 Despliegue
- **[technical/deployment/](technical/deployment/)**
  - CI/CD con GitHub Actions
  - Build y release processes
  - Configuración de entornos

## 🔒 Cumplimiento y Legal

### Compliance
- **[compliance/](compliance/)**
  - **[COPPA-COMPLIANCE.md](compliance/COPPA-COMPLIANCE.md)** - Cumplimiento COPPA (EE.UU.) ✅
  - **[GDPR-K-COMPLIANCE.md](compliance/GDPR-K-COMPLIANCE.md)** - Privacidad infantil (Europa)
  - **[PRIVACY-POLICY.md](compliance/PRIVACY-POLICY.md)** - Política de privacidad
  - **[TERMS-OF-SERVICE.md](compliance/TERMS-OF-SERVICE.md)** - Términos de uso

### Seguridad
- **[security/](security/)**
  - **[SECURITY.md](security/SECURITY.md)** - Políticas de seguridad
  - **[CHILD-SAFETY.md](security/CHILD-SAFETY.md)** - Protección infantil
  - **[DATA-PROTECTION.md](security/DATA-PROTECTION.md)** - Protección de datos

## 🧪 Testing y QA

### Estrategias de Testing
- **[testing/](testing/)**
  - **[TEST-STRATEGY.md](testing/TEST-STRATEGY.md)** - Estrategia general ✅
  - **[CHILD-USABILITY-TESTING.md](testing/CHILD-USABILITY-TESTING.md)** - Testing específico para niños ✅
  - **[EDUCATIONAL-CONTENT-QA.md](testing/EDUCATIONAL-CONTENT-QA.md)** - QA de contenido curricular ✅
  - **[ACCESSIBILITY-TESTING.md](testing/ACCESSIBILITY-TESTING.md)** - WCAG 2.1 AA compliance ✅
  - **[AUTOMATED-TESTING.md](testing/AUTOMATED-TESTING.md)** - CI/CD y automatización ✅
  - **[PERFORMANCE-TESTING.md](testing/PERFORMANCE-TESTING.md)** - Testing de rendimiento móvil ✅

## 👥 Guías de Usuario

### Para Padres
- **[user-guides/parents/](user-guides/parents/)**
  - Manual de uso para padres
  - Configuración de control parental
  - Seguimiento de progreso

### Para Educadores
- **[user-guides/teachers/](user-guides/teachers/)**
  - Guía pedagógica
  - Integración curricular
  - Evaluación de progreso

## 🎯 Agentes Especializados

### Configuración Claude Code
- **[.claude/agents/](../.claude/agents/)**
  - **[maui-infrastructure-architect.md](../.claude/agents/maui-infrastructure-architect.md)** - Arquitectura .NET MAUI
  - **[educational-content-specialist.md](../.claude/agents/educational-content-specialist.md)** - Contenido educativo
  - **[ui-ux-children-specialist.md](../.claude/agents/ui-ux-children-specialist.md)** - UX infantil
  - **[gamification-system-architect.md](../.claude/agents/gamification-system-architect.md)** - Gamificación
  - **[audio-multimedia-specialist.md](../.claude/agents/audio-multimedia-specialist.md)** - Multimedia
  - **[eduplay-qa-specialist.md](../.claude/agents/eduplay-qa-specialist.md)** - QA especializado
  - **[android-platform-specialist.md](../.claude/agents/android-platform-specialist.md)** - Android específico
  - **[project-coordinator.md](../.claude/agents/project-coordinator.md)** - Coordinación
  - **[maui-devops-specialist.md](../.claude/agents/maui-devops-specialist.md)** - DevOps
  - **[documentation-specialist.md](../.claude/agents/documentation-specialist.md)** - Documentación

## 📊 Estado de Completitud

### ✅ Completado (100%)
- **Fase 1**: Documentación de requisitos
- **Fase 2**: Arquitectura del sistema
- **Fase 2.5**: Diseño de base de datos
- **Fase 3**: Diseño UX/UI
- **Fase 3.5**: Especificaciones de contenido
- **Fase 4 Docs**: Documentación técnica de implementación ✅
  - Guía de implementación Phase 4
  - Configuración de entorno de desarrollo
  - Estructura del proyecto .NET MAUI
  - Setup de base de datos SQLite + EF Core
- Testing framework
- Compliance COPPA

### 🚧 En Progreso
- **Fase 4**: Implementación .NET MAUI (código)
- Documentación de API
- Guías de usuario finales

### ⏳ Pendiente
- iOS platform documentation
- Windows platform documentation
- Localization complete guides
- Advanced analytics documentation

## 🔍 Navegación Rápida por Tema

### Por Rol
- **Desarrolladores**:
  - **Nuevos**: [developer-setup.md](technical/setup-and-installation/developer-setup.md) → [project-structure.md](technical/development/project-structure.md)
  - **Implementación**: [phase-4-implementation-guide.md](technical/development/phase-4-implementation-guide.md) → [database-setup.md](technical/setup-and-installation/database-setup.md)
  - **Arquitectura**: [1.2 Arquitectura/](../1.2%20Arquitectura%20del%20Sistema/) → [overview.md](technical/architecture/overview.md)
- **Diseñadores**: [2.1 Diseño UXUI/](../2.1%20Diseño%20UXUI/) → [guia-estilo.md](../2.1%20Diseño%20UXUI/guia-estilo.md)
- **Educadores**: [2.2 Especificaciones de Contenido/](../2.2%20Especificaciones%20de%20Contenido/) → [user-guides/teachers/](user-guides/teachers/)
- **QA/Testing**: [testing/](testing/) → [TEST-STRATEGY.md](testing/TEST-STRATEGY.md)
- **Legal/Compliance**: [compliance/](compliance/) → [COPPA-COMPLIANCE.md](compliance/COPPA-COMPLIANCE.md)
- **Padres/Usuarios**: [user-guides/parents/](user-guides/parents/)

### Por Fase de Desarrollo
- **Planificación**: [1.1 Documentación de Requisitos/](../1.1%20Documentación%20de%20Requisitos/)
- **Diseño**: [1.2 Arquitectura/](../1.2%20Arquitectura%20del%20Sistema/) + [2.1 Diseño UXUI/](../2.1%20Diseño%20UXUI/)
- **Implementación Phase 4**:
  - [phase-4-implementation-guide.md](technical/development/phase-4-implementation-guide.md) (Guía completa 5 semanas)
  - [developer-setup.md](technical/setup-and-installation/developer-setup.md) (Setup entorno)
  - [project-structure.md](technical/development/project-structure.md) (Estructura Clean Architecture)
  - [database-setup.md](technical/setup-and-installation/database-setup.md) (SQLite + EF Core)
- **Testing**: [testing/](testing/)
- **Despliegue**: [technical/deployment/](technical/deployment/)

## 📞 Soporte de Documentación

Si encuentras enlaces rotos, información desactualizada o necesitas documentación adicional:

1. **Crear Issue**: Reportar en GitHub Issues
2. **Email**: documentation@eduplaykids.com
3. **Contribuir**: Seguir [CONTRIBUTING.md](../CONTRIBUTING.md)

---

**Última actualización**: Septiembre 2025
**Mantenido por**: Equipo de Documentación EduPlayKids
**Versión**: 1.0