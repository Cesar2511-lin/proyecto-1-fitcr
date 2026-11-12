namespace FitCR.Models;

/// <summary>
/// Representa una insignia asignada al usuario (logro o alerta por inactividad).
/// </summary>
public class Insignia
{
    public string Id { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string Tipo { get; set; } = "Logro"; // "Logro" o "Verguenza"
    public string FechaObtencion { get; set; } = DateTime.Now.ToString("yyyy-MM-dd");

    public bool EsVerguenza => string.Equals(Tipo, "Verguenza", StringComparison.OrdinalIgnoreCase);

    public override string ToString() => $"[{(EsVerguenza ? "ALERTA" : "LOGRO")}] {Nombre}";
}
