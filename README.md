# Fit.CR - Plataforma Integral de Gimnasio

Sistema de gestión para acondicionamiento físico, control de rutinas, aforo de clases, gamificación con insignias y notificaciones desarrollado para el curso **Técnicas de Programación** de la **Universidad Politécnica Internacional**.

---

## 🏛️ Arquitectura del Sistema
El proyecto implementa el patrón de arquitectura **Modelo-Vista-Controlador (MVC)** sobre la plataforma **.NET 8.0 Windows**:

* **Models:** Entidades de dominio con herencia y polimorfismo (`Persona`, `UsuarioBasico`, `Entrenador`, `EntrenadorPersonal`, `EntrenadorGimnasio`).
* **Controllers:** Lógica de negocio, validaciones y cálculos antropométricos (IMC).
* **Data:** Capa de persistencia desacoplada utilizando el patrón Repository (`IFitRepository`, `FitDataService`) con almacenamiento en formato JSON.

---

## 📁 Estructura del Repositorio
* `/documentacion/`: Documento oficial de planificación ágil (Entregable #1 en formato PDF).
* `/src/`: Solución y código fuente del sistema en C#.
* `datos_iniciales.json`: Semilla de datos inicial con 40 registros completos.

---

## 🚀 Estado del Desarrollo (Sprint 1)
* [x] **PBI 1:** Estructura base de la solución, modelos de usuarios y entrenadores, cálculo de IMC y persistencia JSON.
* [ ] **PBI 2:** Gestión integral de rutinas de ejercicio y control de cargas.
* [ ] **PBI 3:** Programación y aforo de clases grupales.
* [ ] **PBI 4:** Motor de gamificación (logros e insignias de inactividad).
* [ ] **PBI 5:** Línea de tiempo de notificaciones.
* [ ] **PBI 6:** Módulo de historial y progreso.
* [ ] **PBI 7:** Dashboard de estadísticas en tiempo real.
* [ ] **PBI 8:** Validación de calidad de código con Sonar Analyzer.
