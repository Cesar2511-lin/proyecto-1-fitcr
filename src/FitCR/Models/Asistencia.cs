namespace FitCR.Models;

/// <summary>
/// Registro histórico de visitas y entrenamientos ejecutados por el usuario en el gimnasio.
/// </summary>
public class Asistencia
{
    public string Fecha { get; set; } = DateTime.Now.ToString("yyyy-MM-dd");
    public int MinutosEntrenamiento { get; set; } = 60;
    public int CaloriasQuemadas { get; set; } = 400;

    public DateTime FechaParsed => DateTime.TryParse(Fecha, out var dt) ? dt : DateTime.MinValue;

    public override string ToString() => $"{Fecha}: {MinutosEntrenamiento} mins ({CaloriasQuemadas} kcal)";
}
