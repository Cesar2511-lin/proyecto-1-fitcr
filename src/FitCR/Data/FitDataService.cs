using System.Text.Json;
using FitCR.Models;

namespace FitCR.Data;

/// <summary>
/// Servicio de persistencia y gestion de datos mediante archivos JSON.
/// </summary>
public class FitDataService : IFitRepository
{
    private static FitDataService? _instancia;
    public static FitDataService Instancia => _instancia ??= new FitDataService();

    private readonly string _rutaArchivo;
    public string ContrasenaAdmin { get; set; } = "FitCR2026!";

    private List<UsuarioBasico> _usuarios = new();
    private List<Entrenador> _entrenadores = new();
    private List<ClaseGrupal> _clases = new();

    public FitDataService(string? rutaPersonalizada = null)
    {
        _rutaArchivo = rutaPersonalizada ?? DeterminarRutaDatos();
        CargarDatos();
    }

    private static string DeterminarRutaDatos()
    {
        // Busca el archivo en la carpeta de ejecución o sube hacia la raíz del proyecto
        var opciones = new[]
        {
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "datos_iniciales.json"),
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "datos_iniciales.json"),
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "datos_iniciales.json"),
            "datos_iniciales.json"
        };

        foreach (var ruta in opciones)
        {
            var rutaNormalizada = Path.GetFullPath(ruta);
            if (File.Exists(rutaNormalizada))
            {
                return rutaNormalizada;
            }
        }

        return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "datos_iniciales.json");
    }

    public void CargarDatos()
    {
        if (!File.Exists(_rutaArchivo))
        {
            _usuarios = new List<UsuarioBasico>();
            _entrenadores = new List<Entrenador>();
            _clases = new List<ClaseGrupal>();
            return;
        }

        try
        {
            var json = File.ReadAllText(_rutaArchivo);
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (root.TryGetProperty("Metadatos", out var elemMeta) && elemMeta.TryGetProperty("ContrasenaAdmin", out var passElem))
            {
                ContrasenaAdmin = passElem.GetString() ?? "FitCR2026!";
            }

            // Deserializar Usuarios Básicos
            if (root.TryGetProperty("UsuariosBasicos", out var elemUsuarios))
            {
                _usuarios = JsonSerializer.Deserialize<List<UsuarioBasico>>(elemUsuarios.GetRawText(), new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new List<UsuarioBasico>();
            }

            // Deserializar Entrenadores con tipado polimórfico
            _entrenadores = new List<Entrenador>();
            if (root.TryGetProperty("Entrenadores", out var elemEntrenadores))
            {
                foreach (var item in elemEntrenadores.EnumerateArray())
                {
                    var rol = item.TryGetProperty("Rol", out var r) ? r.GetString() : "EntrenadorGimnasio";
                    if (string.Equals(rol, "EntrenadorPersonal", StringComparison.OrdinalIgnoreCase))
                    {
                        var ep = JsonSerializer.Deserialize<EntrenadorPersonal>(item.GetRawText(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        if (ep != null) _entrenadores.Add(ep);
                    }
                    else
                    {
                        var eg = JsonSerializer.Deserialize<EntrenadorGimnasio>(item.GetRawText(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        if (eg != null) _entrenadores.Add(eg);
                    }
                }
            }

            // Deserializar Clases Grupales
            if (root.TryGetProperty("ClasesGrupales", out var elemClases))
            {
                _clases = JsonSerializer.Deserialize<List<ClaseGrupal>>(elemClases.GetRawText(), new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new List<ClaseGrupal>();
            }
        }
        catch (Exception)
        {
            // Respaldo de seguridad en caso de corrupción
            _usuarios = new List<UsuarioBasico>();
            _entrenadores = new List<Entrenador>();
            _clases = new List<ClaseGrupal>();
        }
    }

    public void GuardarCambios()
    {
        try
        {
            var estructura = new
            {
                Metadatos = new
                {
                    Sistema = "Fit.CR",
                    Version = "1.0.0",
                    UltimaModificacion = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    TotalRegistros = _usuarios.Count + _entrenadores.Count,
                    ContrasenaAdmin = ContrasenaAdmin
                },
                UsuariosBasicos = _usuarios,
                Entrenadores = _entrenadores,
                ClasesGrupales = _clases
            };

            var json = JsonSerializer.Serialize(estructura, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(_rutaArchivo, json);

            // Resiliencia: sincronizar con la copia en la raiz del repositorio si existe
            var posiblesRutasRaiz = new[]
            {
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "datos_iniciales.json"),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "datos_iniciales.json")
            };
            foreach (var r in posiblesRutasRaiz)
            {
                var full = Path.GetFullPath(r);
                if (File.Exists(full) && !string.Equals(full, Path.GetFullPath(_rutaArchivo), StringComparison.OrdinalIgnoreCase))
                {
                    File.WriteAllText(full, json);
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error al guardar JSON: {ex.Message}");
        }
    }

    public List<UsuarioBasico> ObtenerUsuarios() => _usuarios;

    public List<Entrenador> ObtenerEntrenadores() => _entrenadores;

    public List<ClaseGrupal> ObtenerClasesGrupales() => _clases;

    public void GuardarUsuario(UsuarioBasico usuario)
    {
        var index = _usuarios.FindIndex(u => u.Id == usuario.Id);
        if (index >= 0)
        {
            _usuarios[index] = usuario;
        }
        else
        {
            _usuarios.Add(usuario);
        }
        GuardarCambios();
    }

    public void EliminarUsuario(string usuarioId)
    {
        _usuarios.RemoveAll(u => u.Id == usuarioId);
        GuardarCambios();
    }

    public void GuardarClase(ClaseGrupal clase)
    {
        var index = _clases.FindIndex(c => c.Id == clase.Id);
        if (index >= 0)
        {
            _clases[index] = clase;
        }
        else
        {
            _clases.Add(clase);
        }
        GuardarCambios();
    }
}
