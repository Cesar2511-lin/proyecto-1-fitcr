using FitCR.Data;
using FitCR.Models;

namespace FitCR.Controllers;

/// <summary>
/// Controlador para la asignacion y calculo de rutinas y ejercicios.
/// </summary>
public class RutinaController
{
    private readonly IFitRepository _repositorio;

    public RutinaController(IFitRepository? repositorio = null)
    {
        _repositorio = repositorio ?? FitDataService.Instancia;
    }

    public (bool EsValido, string Mensaje) AgregarOActualizarRutina(UsuarioBasico usuario, Rutina rutina)
    {
        if (string.IsNullOrWhiteSpace(rutina.Nombre))
            return (false, "El nombre descriptivo de la rutina es obligatorio.");

        if (rutina.Ejercicios.Count == 0)
            return (false, "La rutina debe contener al menos un ejercicio estructurado.");

        foreach (var ej in rutina.Ejercicios)
        {
            if (string.IsNullOrWhiteSpace(ej.Nombre))
                return (false, "Todos los ejercicios deben tener un nombre asignado.");
            if (ej.Series <= 0 || ej.Repeticiones <= 0)
                return (false, "Las series y repeticiones de cada ejercicio deben ser mayores a cero.");
        }

        var idx = usuario.Rutinas.FindIndex(r => r.Id == rutina.Id);
        if (idx >= 0)
        {
            usuario.Rutinas[idx] = rutina;
        }
        else
        {
            usuario.Rutinas.Add(rutina);
        }

        _repositorio.GuardarUsuario(usuario);
        return (true, "Rutina almacenada correctamente.");
    }

    public bool EliminarRutina(UsuarioBasico usuario, string rutinaId)
    {
        var eliminado = usuario.Rutinas.RemoveAll(r => r.Id == rutinaId) > 0;
        if (eliminado)
        {
            _repositorio.GuardarUsuario(usuario);
        }
        return eliminado;
    }
}
