namespace FitCR.Models;

/// <summary>
/// Clase base para los roles de entrenador del gimnasio.
/// </summary>
public abstract class Entrenador : Persona
{
    public string Especialidad { get; set; } = string.Empty;
    public List<string> Certificaciones { get; set; } = new();
}
