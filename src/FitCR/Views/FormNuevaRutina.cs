using System.Data;
using FitCR.Controllers;
using FitCR.Models;

namespace FitCR.Views;

public class FormNuevaRutina : Form
{
    private readonly UsuarioBasico _usuario;
    private readonly RutinaController _rutinaCtrl;
    private readonly List<Ejercicio> _ejercicios = new();

    private TextBox txtNombreRutina = null!;
    private ComboBox cmbDia = null!;
    private TextBox txtNombreEjercicio = null!;
    private NumericUpDown numSeries = null!;
    private NumericUpDown numReps = null!;
    private NumericUpDown numCarga = null!;
    private NumericUpDown numDescanso = null!;
    private DataGridView dgvEjercicios = null!;
    private Label lblResumen = null!;

    public Rutina? RutinaCreada { get; private set; }

    public FormNuevaRutina(UsuarioBasico usuario, RutinaController rutinaController)
    {
        _usuario = usuario;
        _rutinaCtrl = rutinaController;
        InicializarComponentes();
    }

    private void InicializarComponentes()
    {
        Text = $"Asignar Rutina - {_usuario.NombreCompleto}";
        Width = 620;
        Height = 640;
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
            Text = "Cargar Nueva Rutina de Entrenamiento",
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 12f, FontStyle.Bold),
            AutoSize = true
        };

        var lblSub = new Label
        {
            Text = $"Asignando a miembro: {_usuario.NombreCompleto} (ID: {_usuario.Id})",
            ForeColor = Color.FromArgb(180, 200, 230),
            Font = new Font("Segoe UI", 8.5f),
            Location = new Point(20, 36),
            AutoSize = true
        };

        pnlHeader.Controls.AddRange(new Control[] { lblT, lblSub });
        Controls.Add(pnlHeader);

        int y = 80;

        var lblNom = new Label { Text = "Nombre de la Rutina:", Location = new Point(25, y + 3), AutoSize = true, Font = new Font("Segoe UI", 9f, FontStyle.Bold) };
        txtNombreRutina = new TextBox { Location = new Point(175, y), Width = 400, Text = "Rutina Personalizada" };
        Controls.AddRange(new Control[] { lblNom, txtNombreRutina });
        y += 36;

        var lblDia = new Label { Text = "Dia de la Semana:", Location = new Point(25, y + 3), AutoSize = true, Font = new Font("Segoe UI", 9f, FontStyle.Bold) };
        cmbDia = new ComboBox { Location = new Point(175, y), Width = 180, DropDownStyle = ComboBoxStyle.DropDownList };
        cmbDia.Items.AddRange(new object[] { "Lunes", "Martes", "Miercoles", "Jueves", "Viernes", "Sabado", "Domingo" });
        cmbDia.SelectedIndex = 0;
        Controls.AddRange(new Control[] { lblDia, cmbDia });
        y += 42;

        // GroupBox para agregar ejercicios
        var grpEj = new GroupBox
        {
            Text = "Agregar Ejercicios Estructurados",
            Location = new Point(25, y),
            Size = new Size(550, 130),
            Font = new Font("Segoe UI", 9f, FontStyle.Bold)
        };

        var lblE1 = new Label { Text = "Ejercicio:", Location = new Point(15, 26), AutoSize = true, Font = new Font("Segoe UI", 8.5f) };
        txtNombreEjercicio = new TextBox { Location = new Point(80, 23), Width = 230, Font = new Font("Segoe UI", 9f) };

        var lblE2 = new Label { Text = "Series:", Location = new Point(325, 26), AutoSize = true, Font = new Font("Segoe UI", 8.5f) };
        numSeries = new NumericUpDown { Location = new Point(375, 23), Width = 55, Minimum = 1, Maximum = 10, Value = 4, Font = new Font("Segoe UI", 9f) };

        var lblE3 = new Label { Text = "Reps:", Location = new Point(445, 26), AutoSize = true, Font = new Font("Segoe UI", 8.5f) };
        numReps = new NumericUpDown { Location = new Point(485, 23), Width = 50, Minimum = 1, Maximum = 100, Value = 12, Font = new Font("Segoe UI", 9f) };

        var lblE4 = new Label { Text = "Carga (kg):", Location = new Point(15, 62), AutoSize = true, Font = new Font("Segoe UI", 8.5f) };
        numCarga = new NumericUpDown { Location = new Point(80, 59), Width = 75, Minimum = 0, Maximum = 400, DecimalPlaces = 1, Value = 20, Font = new Font("Segoe UI", 9f) };

        var lblE5 = new Label { Text = "Descanso (seg):", Location = new Point(175, 62), AutoSize = true, Font = new Font("Segoe UI", 8.5f) };
        numDescanso = new NumericUpDown { Location = new Point(275, 59), Width = 65, Minimum = 10, Maximum = 300, Increment = 5, Value = 60, Font = new Font("Segoe UI", 9f) };

        var btnAgregar = new Button
        {
            Text = "Agregar Ejercicio",
            Location = new Point(365, 56),
            Width = 170,
            Height = 30,
            BackColor = Color.FromArgb(16, 185, 129),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 8.5f, FontStyle.Bold)
        };
        btnAgregar.Click += (s, e) => AgregarEjercicioALista();

        grpEj.Controls.AddRange(new Control[] { lblE1, txtNombreEjercicio, lblE2, numSeries, lblE3, numReps, lblE4, numCarga, lblE5, numDescanso, btnAgregar });
        Controls.Add(grpEj);
        y += 142;

        // DataGridView de ejercicios agregados
        dgvEjercicios = new DataGridView
        {
            Location = new Point(25, y),
            Size = new Size(550, 180),
            ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            BackgroundColor = Color.White
        };
        Controls.Add(dgvEjercicios);
        y += 190;

        lblResumen = new Label
        {
            Text = "Ejercicios cargados: 0 | Volumen total estimado: 0 kg",
            Location = new Point(25, y),
            AutoSize = true,
            Font = new Font("Segoe UI", 9f, FontStyle.Bold),
            ForeColor = Color.FromArgb(30, 41, 59)
        };
        Controls.Add(lblResumen);
        y += 32;

        var btnGuardar = new Button
        {
            Text = "Guardar y Asignar Rutina",
            Location = new Point(210, y),
            Width = 220,
            Height = 38,
            BackColor = Color.FromArgb(37, 99, 235),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9f, FontStyle.Bold)
        };
        btnGuardar.Click += (s, e) => GuardarRutina();

        var btnCancelar = new Button
        {
            Text = "Cancelar",
            Location = new Point(445, y),
            Width = 130,
            Height = 38,
            BackColor = Color.FromArgb(100, 116, 139),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };
        btnCancelar.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

        Controls.AddRange(new Control[] { btnGuardar, btnCancelar });

        ActualizarGridEjercicios();
    }

    private void AgregarEjercicioALista()
    {
        var nombre = txtNombreEjercicio.Text.Trim();
        if (string.IsNullOrWhiteSpace(nombre))
        {
            MessageBox.Show("Por favor indique el nombre del ejercicio.", "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var ej = new Ejercicio
        {
            Nombre = nombre,
            Series = (int)numSeries.Value,
            Repeticiones = (int)numReps.Value,
            PesoKg = (double)numCarga.Value,
            DescansoSeg = (int)numDescanso.Value
        };

        _ejercicios.Add(ej);
        txtNombreEjercicio.Clear();
        txtNombreEjercicio.Focus();
        ActualizarGridEjercicios();
    }

    private void ActualizarGridEjercicios()
    {
        dgvEjercicios.DataSource = null;
        dgvEjercicios.DataSource = _ejercicios.Select(e => new
        {
            e.Nombre,
            e.Series,
            Reps = e.Repeticiones,
            Carga = $"{e.PesoKg} kg",
            Descanso = $"{e.DescansoSeg}s",
            Volumen = $"{e.VolumenLevantadoKg} kg"
        }).ToList();

        var totalVolumen = _ejercicios.Sum(e => e.VolumenLevantadoKg);
        lblResumen.Text = $"Ejercicios cargados: {_ejercicios.Count} | Volumen total estimado: {totalVolumen:N0} kg";
    }

    private void GuardarRutina()
    {
        var nombre = txtNombreRutina.Text.Trim();
        if (string.IsNullOrWhiteSpace(nombre))
        {
            MessageBox.Show("El nombre de la rutina es requerido.", "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (_ejercicios.Count == 0)
        {
            MessageBox.Show("Debe agregar al menos un ejercicio a la rutina antes de guardar.", "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var rutina = new Rutina
        {
            Id = $"RUT-{Guid.NewGuid().ToString()[..6].ToUpper()}",
            Nombre = nombre,
            DiaSemana = cmbDia.SelectedItem?.ToString() ?? "Lunes",
            Ejercicios = new List<Ejercicio>(_ejercicios)
        };

        var (exito, mensaje) = _rutinaCtrl.AgregarOActualizarRutina(_usuario, rutina);
        if (exito)
        {
            RutinaCreada = rutina;
            MessageBox.Show($"Rutina '{rutina.Nombre}' asignada exitosamente a {_usuario.NombreCompleto}.", "Fit.CR", MessageBoxButtons.OK, MessageBoxIcon.Information);
            DialogResult = DialogResult.OK;
            Close();
        }
        else
        {
            MessageBox.Show(mensaje, "Error al guardar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
