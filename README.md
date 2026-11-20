# Fit.CR - Plataforma Integral de Gimnasio

Sistema de gestion para acondicionamiento fisico, control de rutinas, aforo de clases, gamificacion con insignias y notificaciones desarrollado para el curso Tecnicas de Programacion de la Universidad Politecnica Internacional.

## Arquitectura del Sistema
El proyecto implementa el patron de arquitectura Modelo-Vista-Controlador (MVC) sobre la plataforma .NET 8.0 Windows:

* Models: Entidades de dominio con herencia y polimorfismo (Persona, UsuarioBasico, Entrenador, EntrenadorPersonal, EntrenadorGimnasio, Administrador, Rutina, Ejercicio, ClaseGrupal).
* Controllers: Logica de negocio, validaciones y calculos antropometricos (UsuarioController, RutinaController, ClaseController, GamificacionController, DashboardController).
* Data: Capa de persistencia desacoplada utilizando el patron Repository (IFitRepository, FitDataService) con almacenamiento en formato JSON.

---

## Control de Acceso y Matriz de Permisos (RBAC)

Se implemento la matriz de control de acceso basada en roles (RBAC) para proteger la privacidad de los miembros del gimnasio:

| Rol de Usuario | Acceso a Gestion de Miembros | Modificacion de Roles / Estados | Prescripcion de Rutinas | Aforo de Clases | Privacidad de Identidad (Cedula) |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **Administrador** | Gestion total, busqueda, alta, baja y edicion completa. | Si (puede bloquear/activar y reasignar roles). | Audita y asigna rutinas a cualquier miembro. | Programa clases y administra aforos. | Acceso completo a cedula y datos sensibles. |
| **Entrenador Personal** | Monitoreo de miembros con selector de ambito ("Mis Estudiantes" / "Todos"). | No (sin privilegios administrativos). | Si (creacion con FormNuevaRutina y asignacion con series, reps, peso y descanso). | Consulta y programacion de horarios (FormNuevaClase). | **Dato Protegido (enmascarado)** para evitar sustraccion de PII. |
| **Entrenador de Gimnasio** | Monitoreo de miembros con selector de ambito ("Mis Estudiantes" / "Todos"). | No (sin privilegios administrativos). | Si (creacion y asignacion de rutinas personalizadas). | Programacion de clases grupales y aforo. | **Dato Protegido (enmascarado)** para evitar sustraccion de PII. |
| **Miembro (Usuario Basico)** | **Exclusivamente "Mi Perfil" (no ve a otros miembros).** | No (roles y estados ocultos). | Solo visualiza sus propias rutinas asignadas. | Reserva y cancelacion de cupos propios. | Solo visualiza su propia cedula. |

### Principios de Aislamiento, Privacidad y Herramientas Tecnicas
1. **Aislamiento para Miembros:**
   * La tabla general de usuarios y el buscador quedan completamente ocultos.
   * La pestaña se transforma de forma dinamica en **"Mi Perfil"**.
   * Su identificacion formal (Cedula, Nombre y Apellidos) se mantiene bloqueada en modo de solo lectura para evitar suplantaciones.
   * El miembro solo puede consultar y actualizar sus medidas personales (peso y altura, con recalculado automatico de IMC) y su informacion de contacto.

2. **Proteccion de Cedula ante Entrenadores:**
   * Para resguardar la identidad legal de los miembros y evitar fuga indebida de informacion personal, los entrenadores visualizan la cedula enmascarada como `"Dato Protegido"`.
   * El personal tecnico solo tiene acceso a las metricas biometricas requeridas para la prescripcion fisica (peso, altura, edad e IMC).

3. **Panel Ejecutivo y Auditoria para el Administrador:**
   * **Dashboard Global:** El administrador visualiza indicadores consolidados del gimnasio (total de miembros, distribucion activo/inactivo/bloqueado, entrenadores en plantilla, aforo promedio en salas y alertas de socios en riesgo de desercion por inactividad > 7 dias).
   * **Auditoria de Gamificacion:** Monitorea la distribucion de insignias del establecimiento e identifica usuarios en sedentarismo critico ("El Fantasma del Gym") para gestion proactiva de retencion.
   * **Auditoria de Asistencia:** Permite alternar entre el registro historico de sesiones de entrenamiento de los miembros y el control de turnos e ingreso diario del cuerpo de entrenadores.

4. **Gestion de Autenticacion, Resiliencia y Gobernanza de Contraseñas:**
   * El acceso al sistema requiere contrasena obligatoria (oculta mediante caracteres de seguridad).
   * La clave predeterminada de fabrica para los perfiles precargados es `FitCR2026!`.
   * **Autogestion de Clave:** Todo usuario autenticado puede actualizar su propia contrasena mediante el boton **"Cambiar Clave"** en la barra superior.
   * **Facultad Exclusiva del Administrador:** Solo el Administrador cuenta con autorizacion para modificar y asignar contrasenas a cualquier usuario del gimnasio (boton **"Cambiar Contraseña del Usuario"** en el panel de administracion).
   * **Resiliencia Total:** Los cambios de credenciales se persisten de inmediato en `datos_iniciales.json`, manteniendose intactos entre reinicios de la aplicacion.

5. **Flujo de Evaluacion para Entrenadores:**
   * Al iniciar sesion como Entrenador, el sistema abre en estado inicial neutro (en blanco), sin forzar un usuario preseleccionado.
   * El entrenador utiliza el selector superior o el buscador de la pestaña de monitoreo para seleccionar el atleta a evaluar.
   * Al seleccionar un miembro, todas las vistas se sincronizan: en **Insignias y Logros** cada tarjeta muestra expresamente el nombre del atleta asociado (`Atleta: [Nombre]`) y el motivo de su obtencion, y en **Historial de Asistencia** cada registro de sesion detalla al socio evaluado, eliminando cualquier ambiguedad de datos no vinculados.
   * Prescripcion de rutinas tecnicas mediante el modal `FormNuevaRutina` con calculo de tonelaje acumulado y programacion de clases colectivas con cupos maximos mediante `FormNuevaClase`.

---

## Estructura del Repositorio
* `/documentacion/`: Documento de planificacion agil (Entregable 1 en formato PDF).
* `/src/`: Solucion y codigo fuente del sistema en C#.
* `datos_iniciales.json`: Semilla de datos inicial con 40 registros de prueba.

## Requisitos de Ejecucion
* .NET SDK 8.0 o superior (Windows Forms).
* Sistema operativo Windows 10 o Windows 11.
