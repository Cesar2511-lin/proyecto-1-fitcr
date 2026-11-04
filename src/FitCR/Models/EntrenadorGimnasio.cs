namespace FitCR.Models;

/// <summary>
/// Entrenador de planta asignado a turnos de piso y clases grupales.
/// </summary>
public class EntrenadorGimnasio : Entrenador
{
    public string Turno { get; set; } = "Matutino";
    public List<string> ClasesImpartidasIds { get; set; } = new();

    public EntrenadorGimnasio()
    {
        Rol = "EntrenadorGimnasio";
    }
}
