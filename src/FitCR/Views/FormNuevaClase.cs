using FitCR.Controllers;
using FitCR.Models;

namespace FitCR.Views;

public class FormNuevaClase : Form
{
    private readonly ClaseController _claseCtrl;
    private readonly UsuarioController _usuarioCtrl;
    private readonly Persona? _usuarioEnSesion;

    private TextBox txtNombreClase = null!;
    private ComboBox cmbDia = null!;
    private ComboBox cmbHora = null!;
    private NumericUpDown numCupoMaximo = null!;
    private ComboBox cmbEntrenador = null!;

    public ClaseGrupal? ClaseCreada { get; private set; }

    public FormNuevaClase(ClaseController claseController, UsuarioController usuarioController, Persona? usuarioEnSesion)
    {
        _claseCtrl = claseController;
        _usuarioCtrl = usuarioController;
        _usuarioEnSesion = usuarioEnSesion;
        InicializarComponentes();
    }

    private void InicializarComponentes()
    {
        Text = "Programar Horario y Clase Grupal - Fit.CR";
        Width = 520;
        Height = 440;
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        BackColor = Color.White;
        Font = new Font("Segoe UI", 9f);

        var pnlHeader = new Panel
        {
            Dock = DockStyle.Top,
            Height = 65,
            BackColor = Color.FromArgb(24, 43, 73),
            Padding = new Padding(20, 12, 20, 10)
        };

        var lblT = new Label
        {
            Text = "Programar Clase y Horario de Inscripcion",
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 12f, FontStyle.Bold),
            AutoSize = true
        };

        var lblSub = new Label
        {
            Text = "Defina la modalidad, dia, hora y cupo maximo de aforo.",
            ForeColor = Color.FromArgb(180, 200, 230),
            Font = new Font("Segoe UI", 8.5f),
            Location = new Point(20, 36),
            AutoSize = true
        };

        pnlHeader.Controls.AddRange(new Control[] { lblT, lblSub });
        Controls.Add(pnlHeader);

        int y = 85;

        var lblNom = new Label { Text = "Nombre de la Clase:", Location = new Point(30, y + 3), AutoSize = true, Font = new Font("Segoe UI", 9f, FontStyle.Bold) };
        txtNombreClase = new TextBox { Location = new Point(190, y), Width = 280, Text = "Entrenamiento Funcional" };
        Controls.AddRange(new Control[] { lblNom, txtNombreClase });
        y += 40;

        var lblDia = new Label { Text = "Dia de la Semana:", Location = new Point(30, y + 3), AutoSize = true, Font = new Font("Segoe UI", 9f, FontStyle.Bold) };
        cmbDia = new ComboBox { Location = new Point(190, y), Width = 180, DropDownStyle = ComboBoxStyle.DropDownList };
        cmbDia.Items.AddRange(new object[] { "Lunes", "Martes", "Miercoles", "Jueves", "Viernes", "Sabado", "Domingo" });
        cmbDia.SelectedIndex = 0;
        Controls.AddRange(new Control[] { lblDia, cmbDia });
        y += 40;

        var lblHora = new Label { Text = "Horario de Inicio:", Location = new Point(30, y + 3), AutoSize = true, Font = new Font("Segoe UI", 9f, FontStyle.Bold) };
        cmbHora = new ComboBox { Location = new Point(190, y), Width = 180, DropDownStyle = ComboBoxStyle.DropDownList };
        cmbHora.Items.AddRange(new object[] { "06:00", "06:30", "07:00", "08:00", "09:00", "16:00", "17:00", "18:00", "18:30", "19:00", "19:30" });
        cmbHora.SelectedIndex = 2;
        Controls.AddRange(new Control[] { lblHora, cmbHora });
        y += 40;

        var lblAforo = new Label { Text = "Cupo Maximo (Aforo):", Location = new Point(30, y + 3), AutoSize = true, Font = new Font("Segoe UI", 9f, FontStyle.Bold) };
        numCupoMaximo = new NumericUpDown { Location = new Point(190, y), Width = 90, Minimum = 3, Maximum = 60, Value = 20 };
        Controls.AddRange(new Control[] { lblAforo, numCupoMaximo });
        y += 40;

        var lblEnt = new Label { Text = "Entrenador Asignado:", Location = new Point(30, y + 3), AutoSize = true, Font = new Font("Segoe UI", 9f, FontStyle.Bold) };
        cmbEntrenador = new ComboBox { Location = new Point(190, y), Width = 280, DropDownStyle = ComboBoxStyle.DropDownList };

        var entrenadores = _usuarioCtrl.ListarEntrenadores();
        foreach (var ent in entrenadores)
        {
            cmbEntrenador.Items.Add(new ItemEntrenadorCombo(ent.Id, $"{ent.NombreCompleto} ({ent.Rol})"));
        }

        if (cmbEntrenador.Items.Count > 0)
        {
            // Si el usuario en sesion es un entrenador, preseleccionarlo
            var idx = -1;
            if (_usuarioEnSesion is Entrenador entSesion)
            {
                for (int i = 0; i < cmbEntrenador.Items.Count; i++)
                {
                    if (cmbEntrenador.Items[i] is ItemEntrenadorCombo item && item.Id == entSesion.Id)
                    {
                        idx = i;
                        break;
                    }
                }
            }
            cmbEntrenador.SelectedIndex = idx >= 0 ? idx : 0;
        }

        Controls.AddRange(new Control[] { lblEnt, cmbEntrenador });
        y += 55;

        var btnGuardar = new Button
        {
            Text = "Programar Horario de Clase",
            Location = new Point(140, y),
            Width = 200,
            Height = 38,
            BackColor = Color.FromArgb(16, 185, 129),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9f, FontStyle.Bold)
        };
        btnGuardar.Click += (s, e) => GuardarClase();

        var btnCancelar = new Button
        {
            Text = "Cancelar",
            Location = new Point(350, y),
            Width = 120,
            Height = 38,
            BackColor = Color.FromArgb(100, 116, 139),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };
        btnCancelar.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

        Controls.AddRange(new Control[] { btnGuardar, btnCancelar });
    }

    private void GuardarClase()
    {
        var nombre = txtNombreClase.Text.Trim();
        if (string.IsNullOrWhiteSpace(nombre))
        {
            MessageBox.Show("El nombre de la modalidad de clase es obligatorio.", "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var entId = "ENT-1001";
        if (cmbEntrenador.SelectedItem is ItemEntrenadorCombo itemEnt)
        {
            entId = itemEnt.Id;
        }

        var clase = new ClaseGrupal
        {
            Id = $"CLS-{Guid.NewGuid().ToString()[..6].ToUpper()}",
            Nombre = nombre,
            Dia = cmbDia.SelectedItem?.ToString() ?? "Lunes",
            Hora = cmbHora.SelectedItem?.ToString() ?? "07:00",
            CupoMaximo = (int)numCupoMaximo.Value,
            EntrenadorId = entId,
            Inscritos = new List<string>()
        };

        var (exito, mensaje) = _claseCtrl.GuardarClase(clase);
        if (exito)
        {
            ClaseCreada = clase;
            MessageBox.Show(mensaje, "Clase Programada - Fit.CR", MessageBoxButtons.OK, MessageBoxIcon.Information);
            DialogResult = DialogResult.OK;
            Close();
        }
        else
        {
            MessageBox.Show(mensaje, "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private sealed class ItemEntrenadorCombo
    {
        public string Id { get; }
        public string Nombre { get; }

        public ItemEntrenadorCombo(string id, string nombre)
        {
            Id = id;
            Nombre = nombre;
        }

        public override string ToString() => Nombre;
    }
}
