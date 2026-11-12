using FitCR.Data;
using FitCR.Models;

namespace FitCR.Controllers;

/// <summary>
/// Gestiona la asignacion de insignias de asistencia e inactividad.
/// </summary>
public class GamificacionController
{
    private readonly IFitRepository _repositorio;

    public GamificacionController(IFitRepository? repositorio = null)
    {
        _repositorio = repositorio ?? FitDataService.Instancia;
    }

    /// <summary>
    /// Evalua la situacion de un usuario y actualiza sus insignias y alertas en tiempo real.
    /// </summary>
    public List<Insignia> EvaluarInsignias(UsuarioBasico usuario)
    {
        var cambio = false;

        // 1. Insignia de Bienvenida
        if (!usuario.Insignias.Any(i => i.Id == "INS-01"))
        {
            usuario.Insignias.Add(new Insignia
            {
                Id = "INS-01",
                Nombre = "Bienvenida a Fit.CR",
                Descripcion = "Registro formal completado en el gimnasio.",
                Tipo = "Logro",
                FechaObtencion = DateTime.Now.ToString("yyyy-MM-dd")
            });
            cambio = true;
        }

        // 2. Insignia de Racha de Hierro (>= 5 asistencias)
        if (usuario.HistorialAsistencias.Count >= 5 && !usuario.Insignias.Any(i => i.Id == "INS-02"))
        {
            usuario.Insignias.Add(new Insignia
            {
                Id = "INS-02",
                Nombre = "Racha de Hierro",
                Descripcion = "Ha completado mas de 5 sesiones de entrenamiento registradas.",
                Tipo = "Logro",
                FechaObtencion = DateTime.Now.ToString("yyyy-MM-dd")
            });

            usuario.Notificaciones.Insert(0, new Notificacion
            {
                Titulo = "Insignia Desbloqueada: Racha de Hierro",
                Mensaje = "Se ha registrado el cumplimiento de mas de 5 entrenamientos."
            });
            cambio = true;
        }

        // 3. Insignia de Inactividad (Shame Badge): Mas de 7 dias inactivo
        var diasInactivo = usuario.DiasDesdeUltimoEntrenamiento();
        var tieneFantasma = usuario.Insignias.Any(i => i.Id == "INS-SHAME-01");

        if (diasInactivo > 7 && !tieneFantasma)
        {
            usuario.Insignias.Add(new Insignia
            {
                Id = "INS-SHAME-01",
                Nombre = "El Fantasma del Gym",
                Descripcion = $"Mas de {diasInactivo} dias consecutivos sin registrar asistencia al gimnasio.",
                Tipo = "Verguenza",
                FechaObtencion = DateTime.Now.ToString("yyyy-MM-dd")
            });

            usuario.Notificaciones.Insert(0, new Notificacion
            {
                Titulo = "Alerta de Inactividad Fisica",
                Mensaje = $"El miembro acumula {diasInactivo} dias consecutivos sin registrar visitas al gimnasio. Se asigna insignia de inactividad."
            });
            cambio = true;
        }
        else if (diasInactivo <= 7 && tieneFantasma)
        {
            usuario.Insignias.RemoveAll(i => i.Id == "INS-SHAME-01");
            usuario.Notificaciones.Insert(0, new Notificacion
            {
                Titulo = "Reanudacion de Entrenamientos",
                Mensaje = "El miembro ha vuelto a registrar asistencia. Se retira la insignia de inactividad."
            });
            cambio = true;
        }

        if (cambio)
        {
            _repositorio.GuardarUsuario(usuario);
        }

        return usuario.Insignias;
    }
}
