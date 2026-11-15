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

    public (bool Exito, string Mensaje) GuardarClase(ClaseGrupal clase)
    {
        if (string.IsNullOrWhiteSpace(clase.Nombre))
            return (false, "El nombre de la clase grupal es obligatorio.");

        if (clase.CupoMaximo <= 0)
            return (false, "El cupo maximo (aforo) debe ser mayor a cero.");

        if (string.IsNullOrWhiteSpace(clase.Dia))
            clase.Dia = "Lunes";

        if (string.IsNullOrWhiteSpace(clase.Hora))
            clase.Hora = "08:00";

        if (string.IsNullOrWhiteSpace(clase.Id))
        {
            clase.Id = $"CLS-{Guid.NewGuid().ToString()[..6].ToUpper()}";
        }

        _repositorio.GuardarClase(clase);
        return (true, $"Clase '{clase.Nombre}' programada correctamente para el {clase.Dia} a las {clase.Hora}.");
    }

    public (bool Exito, string Mensaje) InscribirUsuario(string claseId, string usuarioId)
    {
        var clase = _repositorio.ObtenerClasesGrupales().Find(c => c.Id == claseId);
        if (clase == null) return (false, "La clase especificada no existe.");

        if (clase.Inscritos.Contains(usuarioId))
            return (false, "El usuario ya se encuentra inscrito en esta sesión.");

        if (!clase.HayCupo)
            return (false, "El aforo máximo para esta clase está completo. No quedan cupos.");

        clase.InscribirUsuario(usuarioId);
        _repositorio.GuardarClase(clase);

        // Notificar al usuario
        var usuario = _repositorio.ObtenerUsuarios().Find(u => u.Id == usuarioId);
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
        var clase = _repositorio.ObtenerClasesGrupales().Find(c => c.Id == claseId);
        if (clase == null) return (false, "Clase no encontrada.");

        if (!clase.CancelarInscripcion(usuarioId))
            return (false, "El usuario no estaba registrado en la lista de asistencia.");

        _repositorio.GuardarClase(clase);
        return (true, "Cancelación de cupo realizada exitosamente.");
    }
}
