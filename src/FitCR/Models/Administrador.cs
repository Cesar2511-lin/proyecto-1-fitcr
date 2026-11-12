namespace FitCR.Models;

/// <summary>
/// Modela al personal administrativo del centro deportivo con permisos globales.
/// Deriva de Persona en cumplimiento de los principios de herencia y especializacion.
/// </summary>
public class Administrador : Persona
{
    public string Departamento { get; set; } = "Administracion General";

    public Administrador()
    {
        Rol = "Administrador";
        Estado = "Activo";
    }
}
