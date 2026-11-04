namespace FitCR.Models;

/// <summary>
/// Entrenador personal enfocado en atención personalizada individual con tarifa por hora.
/// </summary>
public class EntrenadorPersonal : Entrenador
{
    public decimal TarifaHora { get; set; } = 15000;
    public List<string> ClientesAsignadosIds { get; set; } = new();

    public EntrenadorPersonal()
    {
        Rol = "EntrenadorPersonal";
    }
}
