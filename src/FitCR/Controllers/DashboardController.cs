using FitCR.Models;

namespace FitCR.Controllers;

/// <summary>
/// Provee calculos y metricas estadisticas para el panel principal.
/// </summary>
public class DashboardController
{
    public class MetricasDashboard
    {
        public double AdherenciaSemanalPct { get; set; }
        public double VolumenTotalLevantadoKg { get; set; }
        public int CaloriasTotalesQuemadas { get; set; }
        public double ImcActual { get; set; }
        public string ClasificacionImc { get; set; } = string.Empty;
        public int TotalRutinas { get; set; }
        public int DiasInactividad { get; set; }
        public int TotalInsignias { get; set; }
    }

    public MetricasDashboard CalcularMetricas(UsuarioBasico usuario)
    {
        var m = new MetricasDashboard
        {
            TotalRutinas = usuario.Rutinas.Count,
            DiasInactividad = usuario.DiasDesdeUltimoEntrenamiento(),
            TotalInsignias = usuario.Insignias.Count,
            ImcActual = usuario.IMC,
            CaloriasTotalesQuemadas = usuario.HistorialAsistencias.Sum(a => a.CaloriasQuemadas)
        };

        // Volumen de todas sus rutinas activas
        m.VolumenTotalLevantadoKg = usuario.Rutinas.Sum(r => r.VolumenTotalKg);

        // Adherencia: % de cumplimiento considerando base de 4 días ideales por semana
        var asistenciasUltimoMes = usuario.HistorialAsistencias
            .Count(a => a.FechaParsed >= DateTime.Today.AddDays(-30));
        var metaMes = 16.0;
        m.AdherenciaSemanalPct = Math.Min(100.0, Math.Round((asistenciasUltimoMes / metaMes) * 100.0, 1));

        // Clasificación OMS de IMC
        m.ClasificacionImc = usuario.IMC switch
        {
            < 18.5 => "Bajo peso",
            < 25.0 => "Normopeso (Saludable)",
            < 30.0 => "Sobrepeso",
            _ => "Obesidad"
        };

        return m;
    }
}
