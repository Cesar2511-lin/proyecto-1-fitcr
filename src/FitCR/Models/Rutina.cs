namespace FitCR.Models;

/// <summary>
/// Modela una sesión o rutina estructurada de entrenamiento asignada a un día específico.
/// </summary>
public class Rutina
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Nombre { get; set; } = string.Empty;
    public string DiaSemana { get; set; } = "Lunes";
    public int DuracionMinutos { get; set; } = 60;
    public List<Ejercicio> Ejercicios { get; set; } = new();

    public double VolumenTotalKg => Ejercicios.Sum(e => e.VolumenLevantadoKg);
    public int CaloriasTotales => Ejercicios.Sum(e => e.CaloriasEstimadas);

    public override string ToString() => $"{Nombre} ({DiaSemana} - {Ejercicios.Count} ejercicios)";
}
