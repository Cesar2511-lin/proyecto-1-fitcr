namespace FitCR.Models;

/// <summary>
/// Miembro estándar del gimnasio con seguimiento físico antropométrico (Sprint 1 - PBI 1).
/// </summary>
public class UsuarioBasico : Persona
{
    public double PesoKg { get; set; }
    public double AlturaM { get; set; }
    public double IMC { get; set; }
    public string? EntrenadorAsignadoId { get; set; }

    public UsuarioBasico()
    {
        Rol = "UsuarioBasico";
    }

    /// <summary>
    /// Recalcula el Índice de Masa Corporal (IMC) a partir del peso y la altura.
    /// </summary>
    public void ActualizarIMC()
    {
        if (AlturaM > 0)
        {
            IMC = Math.Round(PesoKg / (AlturaM * AlturaM), 2);
        }
    }
}
