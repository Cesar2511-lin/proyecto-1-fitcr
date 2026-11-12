namespace FitCR.Models;

/// <summary>
/// Miembro estándar del gimnasio con seguimiento físico, rutinas asignadas, insignias y asistencia.
/// </summary>
public class UsuarioBasico : Persona
{
    public double PesoKg { get; set; }
    public double AlturaM { get; set; }
    public double IMC { get; set; }
    public string? EntrenadorAsignadoId { get; set; }

    public List<Rutina> Rutinas { get; set; } = new();
    public List<Asistencia> HistorialAsistencias { get; set; } = new();
    public List<Insignia> Insignias { get; set; } = new();
    public List<Notificacion> Notificaciones { get; set; } = new();

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

    /// <summary>
    /// Calcula los días transcurridos desde el último entrenamiento registrado.
    /// </summary>
    public int DiasDesdeUltimoEntrenamiento()
    {
        if (HistorialAsistencias.Count == 0) return 999;
        var ultimaFecha = HistorialAsistencias
            .Select(a => a.FechaParsed)
            .Max();
        return (DateTime.Today - ultimaFecha.Date).Days;
    }
}
