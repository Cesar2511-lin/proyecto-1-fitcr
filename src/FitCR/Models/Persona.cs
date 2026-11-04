namespace FitCR.Models;

/// <summary>
/// Clase base abstracta que modela los datos comunes de cualquier persona en el sistema.
/// Aplica el principio de Abstracción y Segregación de Interfaces.
/// </summary>
public abstract class Persona
{
    public string Id { get; set; } = string.Empty;
    public string Cedula { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public int Edad { get; set; }
    public string Rol { get; set; } = string.Empty;
    public string Estado { get; set; } = "Activo";

    public string NombreCompleto => $"{Nombre} {Apellidos}".Trim();

    public override string ToString() => $"{NombreCompleto} ({Rol})";
}
