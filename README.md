# Fit.CR - Plataforma Integral de Gimnasio

Sistema de gestion para acondicionamiento fisico, control de rutinas, aforo de clases, gamificacion con insignias y notificaciones desarrollado para el curso Tecnicas de Programacion de la Universidad Politecnica Internacional.

## Arquitectura del Sistema
El proyecto implementa el patron de arquitectura Modelo-Vista-Controlador (MVC) sobre la plataforma .NET 8.0 Windows:

* Models: Entidades de dominio con herencia y polimorfismo (Persona, UsuarioBasico, Entrenador, EntrenadorPersonal, EntrenadorGimnasio).
* Controllers: Logica de negocio, validaciones y calculos antropometricos (IMC).
* Data: Capa de persistencia desacoplada utilizando el patron Repository (IFitRepository, FitDataService) con almacenamiento en formato JSON.

## Estructura del Repositorio
* `/documentacion/`: Documento de planificacion agil (Entregable 1 en formato PDF).
* `/src/`: Solucion y codigo fuente del sistema en C#.
* `datos_iniciales.json`: Semilla de datos inicial con 40 registros de prueba.

## Requisitos de Ejecucion
* .NET SDK 8.0 o superior (Windows Forms).
* Sistema operativo Windows 10 o Windows 11.
