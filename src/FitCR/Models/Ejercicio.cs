namespace FitCR.Models;

/// <summary>
/// Representa un ejercicio individual dentro de una rutina con sus parámetros de carga y esfuerzo.
/// </summary>
public class Ejercicio
{
    public string Nombre { get; set; } = string.Empty;
    public int Series { get; set; } = 3;
    public int Repeticiones { get; set; } = 10;
    public double PesoKg { get; set; }
    public int DescansoSeg { get; set; } = 60;
    public int CaloriasEstimadas { get; set; }

    public double VolumenLevantadoKg => Series * Repeticiones * PesoKg;

    public override string ToString() => $"{Nombre} ({Series}x{Repeticiones} @ {PesoKg} kg)";
}
