using FitCR.Controllers;

namespace FitCR;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        var controller = new UsuarioController();
        var usuarios = controller.ListarUsuarios();
        var entrenadores = controller.ListarEntrenadores();

        Console.WriteLine($"Fit.CR - Sprint 1 Inicializado");
        Console.WriteLine($"Usuarios registrados: {usuarios.Count}");
        Console.WriteLine($"Entrenadores registrados: {entrenadores.Count}");
    }
}
