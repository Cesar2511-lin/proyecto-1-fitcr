using FitCR.Models;

namespace FitCR.Controllers;

/// <summary>
/// Provee calculos y metricas estadisticas para el panel principal y la consola ejecutiva.
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

    public class MetricasGlobalesGimnasio
    {
        public int TotalMiembros { get; set; }
        public int MiembrosActivos { get; set; }
        public int MiembrosInactivos { get; set; }
        public int MiembrosBloqueados { get; set; }
        public int TotalEntrenadores { get; set; }
        public int EntrenadoresPersonales { get; set; }
        public int EntrenadoresGimnasio { get; set; }
        public int ClasesProgramadas { get; set; }
        public int CuposTotales { get; set; }
        public int CuposOcupados { get; set; }
        public double OcupacionAforoPct { get; set; }
        public int MiembrosEnRiesgoDesercion { get; set; }
        public int TotalInsigniasOtorgadas { get; set; }
        public int InsigniasInactividad { get; set; }
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

    public MetricasGlobalesGimnasio CalcularMetricasGlobales(List<UsuarioBasico> usuarios, List<Entrenador> entrenadores, List<ClaseGrupal> clases)
    {
        var m = new MetricasGlobalesGimnasio
        {
            TotalMiembros = usuarios.Count,
            MiembrosActivos = usuarios.Count(u => string.Equals(u.Estado, "Activo", StringComparison.OrdinalIgnoreCase)),
            MiembrosInactivos = usuarios.Count(u => string.Equals(u.Estado, "Inactivo", StringComparison.OrdinalIgnoreCase)),
            MiembrosBloqueados = usuarios.Count(u => string.Equals(u.Estado, "Bloqueado", StringComparison.OrdinalIgnoreCase)),
            TotalEntrenadores = entrenadores.Count,
            EntrenadoresPersonales = entrenadores.Count(e => e is EntrenadorPersonal),
            EntrenadoresGimnasio = entrenadores.Count(e => e is EntrenadorGimnasio),
            ClasesProgramadas = clases.Count,
            CuposTotales = clases.Sum(c => c.CupoMaximo),
            CuposOcupados = clases.Sum(c => c.Inscritos.Count),
            MiembrosEnRiesgoDesercion = usuarios.Count(u => u.DiasDesdeUltimoEntrenamiento() > 7),
            TotalInsigniasOtorgadas = usuarios.Sum(u => u.Insignias.Count),
            InsigniasInactividad = usuarios.Sum(u => u.Insignias.Count(i => i.EsVerguenza))
        };

        if (m.CuposTotales > 0)
        {
            m.OcupacionAforoPct = Math.Round(((double)m.CuposOcupados / m.CuposTotales) * 100.0, 1);
        }

        return m;
    }
}
