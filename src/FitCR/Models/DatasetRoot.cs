namespace FitCR.Models;

/// <summary>
/// Raíz de la estructura del archivo JSON de persistencia del sistema Fit.CR.
/// </summary>
public class DatasetRoot
{
    public Dictionary<string, object> Metadatos { get; set; } = new();
    public List<UsuarioBasico> UsuariosBasicos { get; set; } = new();
    public List<System.Text.Json.JsonElement> EntrenadoresRaw { get; set; } = new();
    public List<ClaseGrupal> ClasesGrupales { get; set; } = new();
}
