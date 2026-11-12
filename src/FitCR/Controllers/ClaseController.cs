using FitCR.Data;
using FitCR.Models;

namespace FitCR.Controllers;

/// <summary>
/// Controlador para la administracion de clases grupales y aforo.
/// </summary>
public class ClaseController
{
    private readonly IFitRepository _repositorio;

    public ClaseController(IFitRepository? repositorio = null)
    {
        _repositorio = repositorio ?? FitDataService.Instancia;
    }

    public List<ClaseGrupal> ListarClases()
    {
        return _repositorio.ObtenerClasesGrupales();
    }

    public (bool Exito, string Mensaje) InscribirUsuario(string claseId, string usuarioId)
    {
        var clase = _repositorio.ObtenerClasesGrupales().FirstOrDefault(c => c.Id == claseId);
        if (clase == null) return (false, "La clase especificada no existe.");

        if (clase.Inscritos.Contains(usuarioId))
            return (false, "El usuario ya se encuentra inscrito en esta sesión.");

        if (!clase.HayCupo)
            return (false, "El aforo máximo para esta clase está completo. No quedan cupos.");

        clase.InscribirUsuario(usuarioId);
        _repositorio.GuardarClase(clase);

        // Notificar al usuario
        var usuario = _repositorio.ObtenerUsuarios().FirstOrDefault(u => u.Id == usuarioId);
        if (usuario != null)
        {
            usuario.Notificaciones.Insert(0, new Notificacion
            {
                Titulo = $"Reserva Confirmada: {clase.Nombre}",
                Mensaje = $"Has quedado inscrito en la clase del {clase.Dia} a las {clase.Hora}."
            });
            _repositorio.GuardarUsuario(usuario);
        }

        return (true, "Inscripción confirmada satisfactoriamente.");
    }

    public (bool Exito, string Mensaje) CancelarInscripcion(string claseId, string usuarioId)
    {
        var clase = _repositorio.ObtenerClasesGrupales().FirstOrDefault(c => c.Id == claseId);
        if (clase == null) return (false, "Clase no encontrada.");

        if (!clase.CancelarInscripcion(usuarioId))
            return (false, "El usuario no estaba registrado en la lista de asistencia.");

        _repositorio.GuardarClase(clase);
        return (true, "Cancelación de cupo realizada exitosamente.");
    }
}
