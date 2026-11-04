using FitCR.Models;

namespace FitCR.Data;

/// <summary>
/// Contrato del repositorio para abstraer el almacenamiento y persistencia de datos (Sprint 1).
/// </summary>
public interface IFitRepository
{
    List<UsuarioBasico> ObtenerUsuarios();
    List<Entrenador> ObtenerEntrenadores();

    void GuardarUsuario(UsuarioBasico usuario);
    void EliminarUsuario(string usuarioId);

    void GuardarCambios();
    void CargarDatos();
}
