namespace FitCR.Models;

/// <summary>
/// Modela una clase grupal con control estricto de cupo y lista de inscritos.
/// </summary>
public class ClaseGrupal
{
    public string Id { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string EntrenadorId { get; set; } = string.Empty;
    public string Dia { get; set; } = "Lunes";
    public string Hora { get; set; } = "07:00";
    public int CupoMaximo { get; set; } = 20;
    public List<string> Inscritos { get; set; } = new();

    public int CuposDisponibles => Math.Max(0, CupoMaximo - Inscritos.Count);
    public bool HayCupo => CuposDisponibles > 0;

    public bool InscribirUsuario(string usuarioId)
    {
        if (!HayCupo || Inscritos.Contains(usuarioId))
        {
            return false;
        }

        Inscritos.Add(usuarioId);
        return true;
    }

    public bool CancelarInscripcion(string usuarioId)
    {
        return Inscritos.Remove(usuarioId);
    }

    public override string ToString() => $"{Nombre} ({Dia} {Hora}) - Cupos: {CuposDisponibles}/{CupoMaximo}";
}
