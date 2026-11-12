using FitCR.Models;

namespace FitCR.Data;

/// <summary>
/// Interfaz para la persistencia y consulta de datos del sistema.
/// </summary>
public interface IFitRepository
{
    List<UsuarioBasico> ObtenerUsuarios();
    List<Entrenador> ObtenerEntrenadores();
    List<ClaseGrupal> ObtenerClasesGrupales();

    void GuardarUsuario(UsuarioBasico usuario);
    void EliminarUsuario(string usuarioId);
    void GuardarClase(ClaseGrupal clase);

    void GuardarCambios();
    void CargarDatos();
}
