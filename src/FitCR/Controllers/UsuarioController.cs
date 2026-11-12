using FitCR.Data;
using FitCR.Models;

namespace FitCR.Controllers;

/// <summary>
/// Controlador para la gestion y validacion de usuarios.
/// </summary>
public class UsuarioController
{
    private readonly IFitRepository _repositorio;

    public UsuarioController(IFitRepository? repositorio = null)
    {
        _repositorio = repositorio ?? FitDataService.Instancia;
    }

    public List<UsuarioBasico> ListarUsuarios(string filtro = "")
    {
        var todos = _repositorio.ObtenerUsuarios();
        if (string.IsNullOrWhiteSpace(filtro))
        {
            return todos;
        }

        var f = filtro.Trim().ToLower();
        return todos.Where(u =>
            u.NombreCompleto.ToLower().Contains(f) ||
            u.Cedula.Contains(f) ||
            u.Correo.ToLower().Contains(f)).ToList();
    }

    public List<Entrenador> ListarEntrenadores()
    {
        return _repositorio.ObtenerEntrenadores();
    }

    public (bool EsValido, string Mensaje) ValidarYGuardarUsuario(UsuarioBasico usuario)
    {
        if (string.IsNullOrWhiteSpace(usuario.Nombre))
            return (false, "El nombre del usuario es obligatorio.");

        if (string.IsNullOrWhiteSpace(usuario.Apellidos))
            return (false, "Los apellidos son obligatorios.");

        if (string.IsNullOrWhiteSpace(usuario.Cedula))
            return (false, "La cédula de identidad es requerida.");

        if (usuario.Edad < 12 || usuario.Edad > 100)
            return (false, "La edad debe encontrarse en un rango válido (12 a 100 años).");

        if (usuario.PesoKg <= 20 || usuario.PesoKg > 350)
            return (false, "El peso ingresado debe ser un valor numérico coherente en kilogramos.");

        if (usuario.AlturaM <= 0.8 || usuario.AlturaM > 2.5)
            return (false, "La altura debe ser ingresada en metros (ejemplo: 1.75).");

        // Recalcular IMC con fórmula oficial
        usuario.ActualizarIMC();

        if (string.IsNullOrWhiteSpace(usuario.Id))
        {
            usuario.Id = $"USR-{DateTime.Now.Ticks % 100000}";
        }

        _repositorio.GuardarUsuario(usuario);
        return (true, "Usuario guardado exitosamente.");
    }

    public bool EliminarUsuario(string usuarioId)
    {
        if (string.IsNullOrWhiteSpace(usuarioId)) return false;
        _repositorio.EliminarUsuario(usuarioId);
        return true;
    }
}
