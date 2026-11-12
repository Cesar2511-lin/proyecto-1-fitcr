namespace FitCR.Models;

/// <summary>
/// Mensaje cronológico de la línea de tiempo del usuario (alertas, clases, felicitaciones).
/// </summary>
public class Notificacion
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Titulo { get; set; } = string.Empty;
    public string Mensaje { get; set; } = string.Empty;
    public string Fecha { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
    public bool Leida { get; set; }

    public override string ToString() => $"{(Leida ? "[Leída]" : "[Nueva]")} {Titulo} ({Fecha})";
}
