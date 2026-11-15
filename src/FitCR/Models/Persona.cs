namespace FitCR.Models;

/// <summary>
/// Clase base para las personas registradas en el sistema.
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
    public string Contrasena { get; set; } = "FitCR2026!";

    public string NombreCompleto => $"{Nombre} {Apellidos}".Trim();

    public override string ToString() => $"{NombreCompleto} ({Rol})";
}
