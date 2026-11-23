# Fit.CR - Plataforma Integral de Gimnasio

Sistema de informacion de escritorio para gestion de acondicionamiento fisico, control de rutinas, administracion de aforos en clases colectivas, gamificacion con incentivos y seguimiento antropometrico desarrollado para el curso **Tecnicas de Programacion** de la **Universidad Politecnica Internacional**.

---

## Ficha Tecnica del Proyecto

* **Institucion:** Universidad Politecnica Internacional (UPI)
* **Facultad y Sede:** Facultad de Ingenieria y Tecnologias de Informacion - Campus Heredia
* **Carrera:** Bachillerato en Ingenieria Informatica
* **Curso:** Tecnicas de Programacion
* **Estudiante:** Julio Cesar Garcia Serrano (DIMEX: 155830134429)
* **Docente:** Prof. Luis Felipe Mora Umaña
* **Repositorio Oficial:** https://github.com/Cesar2511-lin/proyecto-1-fitcr.git
* **Tablero Jira Oficial:** https://edu-team-ygfq61ur.atlassian.net/jira/software/projects/FIT/boards/2

---

## Arquitectura de Software y Buenas Practicas

El sistema se desarrollo bajo el lenguaje **C# sobre la plataforma .NET 8.0 (Windows Forms)**, aplicando patrones de diseño reconocidos y directrices estrictas de ingenieria de software:

1. **Patron Arquitectonico Modelo-Vista-Controlador (MVC):**
   * **Modelos (`FitCR.Models`):** Entidades de dominio fuertemente tipadas (`Persona`, `UsuarioBasico`, `Entrenador`, `EntrenadorPersonal`, `EntrenadorGimnasio`, `Administrador`, `Rutina`, `Ejercicio`, `ClaseGrupal`, `Insignia`, `Notificacion`, `Asistencia`).
   * **Controladores (`FitCR.Controllers`):** Orquestacion de logica de negocio, validaciones de integridad y reglas de calculo (`UsuarioController`, `RutinaController`, `ClaseController`, `GamificacionController`, `DashboardController`).
   * **Vistas (`FitCR.Views`):** Formularios graficos desacoplados (`FormLogin`, `FormRegistro`, `FormPrincipal`, `FormNuevaRutina`, `FormNuevaClase`).

2. **Programacion Orientada a Objetos (POO):**
   * Jerarquia con clase base abstracta `Persona` y especializacion de roles mediante herencia y polimorfismo.
   * Encapsulamiento riguroso de colecciones y metricas de salud.

3. **Patron de Persistencia Repository (`IFitRepository`):**
   * Desacoplamiento total entre la capa logica y el soporte fisico a traves de la implementacion `FitDataService`.
   * **Almacenamiento en JSON (`datos_iniciales.json`):** Cumple con la directriz docente de **no utilizar bases de datos relacionales SQL** en esta primera iteracion, garantizando portabilidad con 40 registros base iniciales.

4. **Analisis Estatico de Codigo:**
   * Integracion de `SonarAnalyzer.CSharp` (SonarSource) para cumplimiento de estandares de Clean Code, legibilidad y prevencion de vulnerabilidades.

---

## Control de Acceso y Matriz de Permisos (RBAC)

El sistema implementa una matriz formal de control de acceso basada en roles (RBAC) que protege la privacidad de los datos personales y de salud:

| Rol de Usuario | Acceso a Gestion de Miembros | Modificacion de Roles / Estados | Prescripcion de Rutinas | Aforo de Clases | Privacidad de Identidad (Cedula) |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **Administrador** | Gestion total, busqueda, alta, baja y edicion completa. | Si (puede bloquear/activar y reasignar roles). | Audita y asigna rutinas a cualquier miembro. | Programa clases y administra aforos. | Acceso completo a cedula y datos sensibles. |
| **Entrenador Personal** | Monitoreo de miembros con selector de ambito ("Mis Estudiantes" / "Todos"). | No (sin privilegios administrativos). | Si (creacion con FormNuevaRutina y asignacion con series, reps, peso y descanso). | Consulta y programacion de horarios (FormNuevaClase). | **Dato Protegido (enmascarado)** para evitar sustraccion de PII. |
| **Entrenador de Gimnasio** | Monitoreo de miembros con selector de ambito ("Mis Estudiantes" / "Todos"). | No (sin privilegios administrativos). | Si (creacion y asignacion de rutinas personalizadas). | Programacion de clases grupales y aforo. | **Dato Protegido (enmascarado)** para evitar sustraccion de PII. |
| **Miembro (Usuario Basico)** | **Exclusivamente "Mi Perfil" (no ve a otros miembros).** | No (roles y estados ocultos). | Solo visualiza sus propias rutinas asignadas. | Reserva y cancelacion de cupos propios. | Solo visualiza su propia cedula. |

### Principio de Aislamiento y Privacidad de Datos (PII)
1. **Aislamiento para Miembros:**
   * La tabla general de usuarios y la barra de busqueda quedan completamente ocultas.
   * La pestaña se transforma de forma dinamica en **"Mi Perfil"**.
   * Su identificacion formal (Cedula, Nombre y Apellidos) se mantiene bloqueada en modo de solo lectura para evitar suplantaciones.
   * El miembro solo puede consultar y actualizar sus medidas personales (peso y altura, con recalculado automatico de IMC) y su informacion de contacto.

2. **Proteccion de Cedula ante Entrenadores:**
   * Para resguardar la identidad legal de los miembros y evitar robo o fuga indebida de informacion personal, los entrenadores visualizan la cedula enmascarada como `"Dato Protegido"`.
   * El personal tecnico solo tiene acceso a las metricas biometricas requeridas para la prescripcion fisica (peso, altura, edad e IMC).

3. **Panel Ejecutivo y Auditoria para el Administrador:**
   * **Dashboard Global:** El administrador visualiza indicadores consolidados del gimnasio (total de miembros, distribucion activo/inactivo/bloqueado, entrenadores en plantilla, aforo promedio en salas y alertas de socios en riesgo de desercion por inactividad > 7 dias).
   * **Auditoria de Gamificacion:** Monitorea la distribucion de insignias del establecimiento e identifica usuarios en sedentarismo critico ("El Fantasma del Gym") para gestion proactiva de retencion.
   * **Auditoria de Asistencia:** Permite alternar entre el registro historico de sesiones de entrenamiento de los miembros y el control de turnos e ingreso diario del cuerpo de entrenadores.

4. **Gestion de Autenticacion, Resiliencia y Gobernanza de Contraseñas:**
   * El acceso al sistema requiere contrasena obligatoria (oculta mediante caracteres de seguridad).
   * La clave predeterminada de fabrica para los perfiles precargados es `FitCR2026!`.
   * **Autogestion de Clave:** Todo usuario autenticado puede actualizar su propia contrasena de forma segura mediante el boton **"Cambiar Clave"** ubicado en el encabezado superior del sistema.
   * **Facultad Exclusiva del Administrador:** Unicamente el rol de Administrador cuenta con autorizacion y herramientas para modificar la contrasena de acceso de cualquier usuario del gimnasio (boton **"Cambiar Contraseña del Usuario"** en el panel de gestion).
   * **Resiliencia Total:** Cualquier cambio de clave (propio o realizado por el administrador) se persiste inmediatamente en `datos_iniciales.json`, garantizando su conservacion ante reinicios o nuevas ejecuciones de la aplicacion.
   * Por principios de seguridad de la informacion, no se incorporan mecanismos no verificados de restablecimiento externo en pantalla de bienvenida.

5. **Flujo de Evaluacion para Entrenadores:**
   * Al iniciar sesion como Entrenador, el sistema abre en estado inicial neutro (en blanco), sin forzar un usuario preseleccionado.
   * El entrenador utiliza el selector superior o el buscador de la pestaña de monitoreo para seleccionar el atleta a evaluar.
   * Al seleccionar un miembro, todas las vistas se sincronizan: en **Insignias y Logros** cada tarjeta muestra expresamente el nombre del atleta asociado (`Atleta: [Nombre]`) y el motivo de su obtencion, y en **Historial de Asistencia** cada registro de sesion detalla al socio evaluado, eliminando cualquier ambiguedad de datos no vinculados.

---

## Guia de Instalacion y Ejecucion para el Docente

El proyecto incluye metodos automatizados para facilitar la revision y ejecucion en cualquier equipo con Windows:

### Requisitos del Sistema
* Sistema Operativo: Windows 10 o Windows 11.
* SDK: .NET 8.0 SDK (versión 8.0.400 o superior).

### Metodo 1: Ejecucion Directa mediante Script (Recomendado)
1. Descargue o clone el repositorio en su equipo.
2. Haga doble clic en el archivo `iniciar_fitcr.bat` ubicado en la raiz de la carpeta.
3. El script compila y ejecuta automaticamente la aplicacion.

### Metodo 2: Mediante Linea de Comandos (.NET CLI)
Abra PowerShell o CMD en la raiz del proyecto y ejecute:
```powershell
dotnet build "src/FitCR/FitCR.csproj"
dotnet run --project "src/FitCR/FitCR.csproj"
```

### Metodo 3: Mediante Visual Studio 2022
Abra la solucion o el archivo de proyecto `src/FitCR/FitCR.csproj` y presione `F5` para iniciar la depuracion.

---

## Perfiles de Demostracion para Evaluacion Rapida

La ventana de inicio de sesion (`FormLogin`) cuenta con un menu desplegable titulado **"-- Seleccionar perfil para prueba --"** que autocompleta automaticamente tanto el identificador como la contrasena (`FitCR2026!`) para evaluar cada rol sin necesidad de digitar datos manuales:

1. **Perfil Administrador:**
   * Credencial: `admin@fitcr.com` o `admin` (Clave: `FitCR2026!`, `admin` o `admin123`)
   * Interfaz: Dashboard ejecutivo con metricas globales, auditoria de gamificacion, auditoria de turnos de entrenadores y gestion total de miembros.

2. **Perfil Entrenador Personal:**
   * Credencial: `carlos.entrenador@fitcr.com` (Clave: `FitCR2026!`)
   * Interfaz: Monitoreo de clientes, prescripcion de rutinas tecnicas (`FormNuevaRutina`), programacion de clases (`FormNuevaClase`) y cedulas protegidas.

3. **Perfil Entrenador de Gimnasio:**
   * Credencial: `laura.sala@fitcr.com` (Clave: `FitCR2026!`)
   * Interfaz: Control de aforo en clases, programacion de horarios de sala y evaluacion fisica con proteccion de identidad.

4. **Perfil Miembro Regular:**
   * Credencial: `1-1111-0001` o `carlos.rodriguez@fitcr.com` (Clave: `FitCR2026!`)
   * Interfaz: Acceso exclusivo a "Mi Perfil", rutinas personales, vitrina de insignias e historial propio.

5. **Registro de Nuevos Miembros:**
   * El boton **"Registrarse como Nuevo Miembro"** en la pantalla de bienvenida permite crear una cuenta autonoma, calculando el IMC en vivo bajo la formula oficial de la OMS ($IMC = Peso / Altura^2$), asignando una rutina de adaptacion inicial y otorgando la insignia de bienvenida.

---

## Modulos Funcionales de la Aplicacion

* **Panel Principal / Dashboard:** Tarjetas KPI con adherencia semanal, volumen de carga acumulado en kg, gasto energetico estimado (kcal), clasificacion del IMC y diagnostico preventivo ante ausentismo.
* **Gestion de Usuarios / Mi Perfil:** Modulo con control RBAC reactivo segun el tipo de sesion activa.
* **Rutinas y Ejercicios:** Desglose de dias de la semana, grupos musculares, series, repeticiones, cargas y tiempos de descanso. Incluye formulario modal de prescripcion (`FormNuevaRutina`) donde entrenadores y administradores configuran nuevas rutinas calculando el tonelaje y asignandolas directamente al miembro activo.
* **Clases Grupales y Aforo:** Programacion de clases (Yoga, Spinning, Crossfit, etc.) con cupos maximos y deteccion automatica de sobrecupo. Incluye formulario modal (`FormNuevaClase`) para apertura de nuevos horarios y asignacion de coach responsable.
* **Insignias y Gamificacion:** Motor de recompensas por constancia y penalizaciones por inactividad prolongada (regla de los 7 dias: insignia *"El Fantasma del Gym"*).
* **Notificaciones e Historial:** Linea cronológica de avisos con marcado de lectura y trazabilidad de asistencias por fecha y minutos de entrenamiento.

---

## Estructura del Repositorio

* `/src/`: Codigo fuente de la aplicacion en C# (.NET 8.0 Windows Forms).
* `/documentacion/`: Documentos formales de entrega en formato PDF:
  * `Entregable_1_Planificacion_FitCR.pdf` (Planificacion agil, Epicas, PBIs y arquitectura).
  * `Documentacion_Tecnica_FitCR.pdf` (Documento tecnico final con justificaciones de diseño, capturas y manual).
* `datos_iniciales.json`: Semilla maestra con 40 registros completos.
* `README.md`: Documentacion tecnica de referencia para ejecucion y evaluacion docente.
