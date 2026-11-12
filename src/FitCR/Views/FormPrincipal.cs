using System.Drawing;
using System.Windows.Forms;
using FitCR.Controllers;
using FitCR.Data;
using FitCR.Models;

namespace FitCR.Views;

public partial class FormPrincipal : Form
{
    private readonly UsuarioController _usuarioCtrl = new();
    private readonly ClaseController _claseCtrl = new();
    private readonly GamificacionController _gamificacionCtrl = new();
    private readonly DashboardController _dashboardCtrl = new();

    private readonly Persona? _usuarioAutenticado;
    private UsuarioBasico? _usuarioActual;
    private UsuarioBasico? _usuarioEnEdicion;

    // Controles Principales
    private Label lblSelPrompt = null!;
    private ComboBox cmbSelectorUsuarios = null!;
    private Button btnCerrarSesion = null!;
    private TabControl tabPrincipal = null!;

    // Controles Dashboard
    private Label lblKpiAdherencia = null!;
    private Label lblKpiVolumen = null!;
    private Label lblKpiCalorias = null!;
    private Label lblKpiImc = null!;
    private Label lblKpiInactividad = null!;
    private Label lblResumenUsuario = null!;
    private TextBox txtDetalleDashboard = null!;

    // Controles Usuarios
    private DataGridView dgvUsuarios = null!;
    private TextBox txtFiltroUsuario = null!;
    private TextBox txtNombre = null!;
    private TextBox txtApellidos = null!;
    private TextBox txtCedula = null!;
    private TextBox txtCorreo = null!;
    private NumericUpDown numEdad = null!;
    private NumericUpDown numPeso = null!;
    private NumericUpDown numAltura = null!;
    private ComboBox cmbEstadoUsuario = null!;
    private Label lblImcCalculado = null!;

    // Controles Rutinas
    private ListBox lstRutinas = null!;
    private DataGridView dgvEjercicios = null!;
    private Label lblResumenRutina = null!;

    // Controles Clases
    private DataGridView dgvClases = null!;
    private ListBox lstInscritosClase = null!;
    private Label lblCuposInfo = null!;

    // Controles Insignias
    private FlowLayoutPanel pnlInsignias = null!;

    // Controles Notificaciones
    private ListBox lstNotificaciones = null!;

    // Controles Historial
    private DataGridView dgvHistorial = null!;

    public FormPrincipal() : this(null)
    {
    }

    public FormPrincipal(Persona? usuarioSesion)
    {
        _usuarioAutenticado = usuarioSesion;
        InicializarComponentesPersonalizados();
        CargarUsuariosIniciales();
        ConfigurarRolYPermisos();
    }

    private void InicializarComponentesPersonalizados()
    {
        Text = "Fit.CR - Sistema de Acondicionamiento Fisico y Gestion Fitness";
        Width = 1180;
        Height = 750;
        MinimumSize = new Size(1100, 700);
        StartPosition = FormStartPosition.CenterScreen;
        Font = new Font("Segoe UI", 9.5f, FontStyle.Regular);
        BackColor = Color.FromArgb(245, 247, 250);

        // Header Superior
        var pnlHeader = new Panel
        {
            Dock = DockStyle.Top,
            Height = 75,
            BackColor = Color.FromArgb(24, 43, 73),
            Padding = new Padding(20, 10, 20, 10)
        };

        var lblTitulo = new Label
        {
            Text = "Fit.CR | Plataforma Integral de Gimnasio",
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 13.5f, FontStyle.Bold),
            AutoSize = true,
            Location = new Point(20, 14)
        };

        var rolTexto = _usuarioAutenticado != null ? $"[{_usuarioAutenticado.Rol}] {_usuarioAutenticado.NombreCompleto}" : "Acceso General";
        var lblSubtitulo = new Label
        {
            Text = $"Sesion activa: {rolTexto} | Control y Administracion de Gimnasio",
            ForeColor = Color.FromArgb(180, 200, 230),
            Font = new Font("Segoe UI", 8.5f),
            AutoSize = true,
            Location = new Point(21, 44)
        };

        lblSelPrompt = new Label
        {
            Text = "Miembro Activo:",
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 9f, FontStyle.Bold),
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            Location = new Point(590, 26),
            AutoSize = true
        };

        cmbSelectorUsuarios = new ComboBox
        {
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            Location = new Point(715, 23),
            Width = 270,
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        cmbSelectorUsuarios.SelectedIndexChanged += (s, e) => CambiarUsuarioSeleccionado();

        btnCerrarSesion = new Button
        {
            Text = "Cerrar Sesion",
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            Location = new Point(1005, 21),
            Width = 135,
            Height = 32,
            BackColor = Color.FromArgb(185, 28, 28),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 8.5f, FontStyle.Bold)
        };
        btnCerrarSesion.Click += (s, e) => Close();

        pnlHeader.Controls.AddRange(new Control[] { lblTitulo, lblSubtitulo, lblSelPrompt, cmbSelectorUsuarios, btnCerrarSesion });
        Controls.Add(pnlHeader);

        // TabControl Central
        tabPrincipal = new TabControl
        {
            Dock = DockStyle.Fill,
            Padding = new Point(14, 8)
        };

        tabPrincipal.TabPages.Add(CrearTabDashboard());
        tabPrincipal.TabPages.Add(CrearTabUsuarios());
        tabPrincipal.TabPages.Add(CrearTabRutinas());
        tabPrincipal.TabPages.Add(CrearTabClases());
        tabPrincipal.TabPages.Add(CrearTabInsignias());
        tabPrincipal.TabPages.Add(CrearTabNotificaciones());
        tabPrincipal.TabPages.Add(CrearTabHistorial());

        Controls.Add(tabPrincipal);
        tabPrincipal.BringToFront();
    }

    private TabPage CrearTabDashboard()
    {
        var tab = new TabPage("Panel Principal") { BackColor = Color.White };

        lblResumenUsuario = new Label
        {
            Location = new Point(20, 16),
            AutoSize = true,
            Font = new Font("Segoe UI", 11.5f, FontStyle.Bold),
            ForeColor = Color.FromArgb(30, 41, 59),
            Text = "Cargando metricas..."
        };
        tab.Controls.Add(lblResumenUsuario);

        // Tarjetas de KPIs con margenes ajustados y anchos expandidos
        lblKpiAdherencia = CrearTarjetaKpi(tab, "Adherencia Semanal", "0%", Color.FromArgb(37, 99, 235), 20, 52);
        lblKpiVolumen = CrearTarjetaKpi(tab, "Volumen Total Levantado", "0 kg", Color.FromArgb(16, 185, 129), 245, 52);
        lblKpiCalorias = CrearTarjetaKpi(tab, "Calorias Quemadas", "0 kcal", Color.FromArgb(245, 158, 11), 470, 52);
        lblKpiImc = CrearTarjetaKpi(tab, "Indice de Masa Corporal", "0.0", Color.FromArgb(139, 92, 246), 695, 52);
        lblKpiInactividad = CrearTarjetaKpi(tab, "Dias Inactivo", "0 dias", Color.FromArgb(239, 68, 68), 920, 52);

        var grpInfo = new GroupBox
        {
            Text = "Diagnostico Fisico y Recomendacion Tecnica",
            Location = new Point(20, 195),
            Size = new Size(1115, 415),
            Font = new Font("Segoe UI", 9.5f, FontStyle.Bold)
        };

        txtDetalleDashboard = new TextBox
        {
            Location = new Point(15, 25),
            Size = new Size(1085, 375),
            Multiline = true,
            ReadOnly = true,
            ScrollBars = ScrollBars.Vertical,
            Font = new Font("Consolas", 9.5f),
            BackColor = Color.FromArgb(248, 250, 252)
        };
        grpInfo.Controls.Add(txtDetalleDashboard);
        tab.Controls.Add(grpInfo);

        return tab;
    }

    private Label CrearTarjetaKpi(Control parent, string titulo, string valorInicial, Color accent, int x, int y)
    {
        var pnl = new Panel
        {
            Location = new Point(x, y),
            Size = new Size(215, 125),
            BackColor = Color.FromArgb(248, 250, 252),
            BorderStyle = BorderStyle.FixedSingle
        };

        var pnlTop = new Panel
        {
            Dock = DockStyle.Top,
            Height = 5,
            BackColor = accent
        };
        pnl.Controls.Add(pnlTop);

        var lblT = new Label
        {
            Text = titulo,
            Location = new Point(10, 12),
            Size = new Size(195, 20),
            Font = new Font("Segoe UI", 8.5f, FontStyle.Regular),
            ForeColor = Color.FromArgb(100, 116, 139)
        };
        pnl.Controls.Add(lblT);

        var lblVal = new Label
        {
            Text = valorInicial,
            Location = new Point(10, 38),
            Size = new Size(195, 75),
            Font = new Font("Segoe UI", 12f, FontStyle.Bold),
            ForeColor = Color.FromArgb(15, 23, 42)
        };
        pnl.Controls.Add(lblVal);

        parent.Controls.Add(pnl);
        return lblVal;
    }

    private TabPage CrearTabUsuarios()
    {
        var tab = new TabPage("Gestion de Usuarios") { BackColor = Color.White };

        var lblBuscador = new Label { Text = "Buscar miembro:", Location = new Point(20, 18), AutoSize = true };
        txtFiltroUsuario = new TextBox { Location = new Point(135, 15), Width = 380 };
        txtFiltroUsuario.TextChanged += (s, e) => RecargarGridUsuarios(txtFiltroUsuario.Text);

        dgvUsuarios = new DataGridView
        {
            Location = new Point(20, 50),
            Size = new Size(680, 555),
            ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            BackgroundColor = Color.White
        };
        dgvUsuarios.SelectionChanged += (s, e) => CargarUsuarioDesdeGrid();

        // Formulario Lateral de Edicion/Alta
        var grpForm = new GroupBox
        {
            Text = "Datos del Miembro",
            Location = new Point(720, 45),
            Size = new Size(415, 560)
        };

        int y = 30;
        txtNombre = AgregarCampo(grpForm, "Nombre:", ref y);
        txtApellidos = AgregarCampo(grpForm, "Apellidos:", ref y);
        txtCedula = AgregarCampo(grpForm, "Cedula:", ref y);
        txtCorreo = AgregarCampo(grpForm, "Correo:", ref y);

        var lblE = new Label { Text = "Edad:", Location = new Point(15, y + 3), AutoSize = true };
        numEdad = new NumericUpDown { Location = new Point(130, y), Width = 110, Minimum = 12, Maximum = 100, Value = 25 };
        grpForm.Controls.AddRange(new Control[] { lblE, numEdad });
        y += 38;

        var lblP = new Label { Text = "Peso (kg):", Location = new Point(15, y + 3), AutoSize = true };
        numPeso = new NumericUpDown { Location = new Point(130, y), Width = 110, Minimum = 30, Maximum = 300, DecimalPlaces = 1, Value = 70 };
        numPeso.ValueChanged += (s, e) => ActualizarPreviewImc();
        grpForm.Controls.AddRange(new Control[] { lblP, numPeso });
        y += 38;

        var lblA = new Label { Text = "Altura (m):", Location = new Point(15, y + 3), AutoSize = true };
        numAltura = new NumericUpDown { Location = new Point(130, y), Width = 110, Minimum = 1, Maximum = 2.5m, DecimalPlaces = 2, Increment = 0.01m, Value = 1.70m };
        numAltura.ValueChanged += (s, e) => ActualizarPreviewImc();
        grpForm.Controls.AddRange(new Control[] { lblA, numAltura });
        y += 38;

        var lblEstado = new Label { Text = "Estado:", Location = new Point(15, y + 3), AutoSize = true };
        cmbEstadoUsuario = new ComboBox { Location = new Point(130, y), Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
        cmbEstadoUsuario.Items.AddRange(new object[] { "Activo", "Inactivo" });
        cmbEstadoUsuario.SelectedIndex = 0;
        grpForm.Controls.AddRange(new Control[] { lblEstado, cmbEstadoUsuario });
        y += 42;

        lblImcCalculado = new Label
        {
            Text = "IMC Estimado: 24.2 (Normopeso)",
            Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
            ForeColor = Color.FromArgb(37, 99, 235),
            Location = new Point(15, y),
            AutoSize = true
        };
        grpForm.Controls.Add(lblImcCalculado);
        y += 46;

        var btnGuardar = new Button
        {
            Text = "Guardar / Actualizar",
            Location = new Point(15, y),
            Width = 185,
            Height = 38,
            BackColor = Color.FromArgb(37, 99, 235),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };
        btnGuardar.Click += (s, e) => GuardarUsuarioFormulario();

        var btnNuevo = new Button
        {
            Text = "Limpiar / Nuevo",
            Location = new Point(210, y),
            Width = 185,
            Height = 38,
            BackColor = Color.FromArgb(100, 116, 139),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };
        btnNuevo.Click += (s, e) => LimpiarFormularioUsuario();

        grpForm.Controls.AddRange(new Control[] { btnGuardar, btnNuevo });

        tab.Controls.AddRange(new Control[] { lblBuscador, txtFiltroUsuario, dgvUsuarios, grpForm });
        return tab;
    }

    private TextBox AgregarCampo(GroupBox parent, string etiqueta, ref int y)
    {
        var lbl = new Label { Text = etiqueta, Location = new Point(15, y + 3), AutoSize = true };
        var txt = new TextBox { Location = new Point(130, y), Width = 265 };
        parent.Controls.AddRange(new Control[] { lbl, txt });
        y += 38;
        return txt;
    }

    private void ActualizarPreviewImc()
    {
        var p = (double)numPeso.Value;
        var a = (double)numAltura.Value;
        if (a > 0)
        {
            var imc = Math.Round(p / (a * a), 1);
            var clasificacion = imc switch
            {
                < 18.5 => "Bajo peso",
                < 25.0 => "Normopeso",
                < 30.0 => "Sobrepeso",
                _ => "Obesidad"
            };
            lblImcCalculado.Text = $"IMC Estimado: {imc:F1} ({clasificacion})";
        }
    }

    private TabPage CrearTabRutinas()
    {
        var tab = new TabPage("Rutinas y Ejercicios") { BackColor = Color.White };

        var lblRuts = new Label { Text = "Rutinas Asignadas al Miembro Activo:", Location = new Point(20, 15), AutoSize = true, Font = new Font("Segoe UI", 10f, FontStyle.Bold) };
        lstRutinas = new ListBox
        {
            Location = new Point(20, 45),
            Size = new Size(420, 560),
            HorizontalScrollbar = true,
            Font = new Font("Segoe UI", 9.5f)
        };
        lstRutinas.SelectedIndexChanged += (s, e) => MostrarEjerciciosRutina();

        dgvEjercicios = new DataGridView
        {
            Location = new Point(460, 45),
            Size = new Size(675, 510),
            ReadOnly = true,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            BackgroundColor = Color.White
        };

        lblResumenRutina = new Label
        {
            Location = new Point(460, 570),
            AutoSize = true,
            Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
            ForeColor = Color.FromArgb(15, 23, 42),
            Text = "Selecciona una rutina para auditar ejercicios, carga total y volumen de trabajo."
        };

        tab.Controls.AddRange(new Control[] { lblRuts, lstRutinas, dgvEjercicios, lblResumenRutina });
        return tab;
    }

    private TabPage CrearTabClases()
    {
        var tab = new TabPage("Clases Grupales y Aforo") { BackColor = Color.White };

        dgvClases = new DataGridView
        {
            Location = new Point(20, 20),
            Size = new Size(700, 580),
            ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            BackgroundColor = Color.White
        };
        dgvClases.SelectionChanged += (s, e) => ActualizarDetalleClase();

        var grpReserva = new GroupBox
        {
            Text = "Control de Cupos y Miembros Inscritos",
            Location = new Point(740, 15),
            Size = new Size(395, 585)
        };

        lblCuposInfo = new Label
        {
            Text = "Selecciona una clase del listado.",
            Location = new Point(15, 25),
            AutoSize = true,
            Font = new Font("Segoe UI", 9.5f, FontStyle.Bold)
        };
        grpReserva.Controls.Add(lblCuposInfo);

        lstInscritosClase = new ListBox { Location = new Point(15, 55), Size = new Size(365, 450) };
        grpReserva.Controls.Add(lstInscritosClase);

        var btnInscribir = new Button
        {
            Text = "Inscribir Miembro Activo",
            Location = new Point(15, 525),
            Width = 175,
            Height = 40,
            BackColor = Color.FromArgb(16, 185, 129),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };
        btnInscribir.Click += (s, e) => InscribirEnClase();

        var btnCancelar = new Button
        {
            Text = "Cancelar Reserva",
            Location = new Point(205, 525),
            Width = 175,
            Height = 40,
            BackColor = Color.FromArgb(239, 68, 68),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };
        btnCancelar.Click += (s, e) => CancelarReservaClase();

        grpReserva.Controls.AddRange(new Control[] { btnInscribir, btnCancelar });

        tab.Controls.AddRange(new Control[] { dgvClases, grpReserva });
        return tab;
    }

    private TabPage CrearTabInsignias()
    {
        var tab = new TabPage("Insignias y Logros") { BackColor = Color.White };

        var lblInfo = new Label
        {
            Text = "Insignias de Logros e Inactividad:",
            Location = new Point(20, 15),
            AutoSize = true,
            Font = new Font("Segoe UI", 10f, FontStyle.Bold)
        };
        tab.Controls.Add(lblInfo);

        pnlInsignias = new FlowLayoutPanel
        {
            Location = new Point(20, 48),
            Size = new Size(1115, 555),
            AutoScroll = true,
            BackColor = Color.FromArgb(248, 250, 252),
            BorderStyle = BorderStyle.FixedSingle,
            Padding = new Padding(15)
        };
        tab.Controls.Add(pnlInsignias);

        return tab;
    }

    private TabPage CrearTabNotificaciones()
    {
        var tab = new TabPage("Notificaciones") { BackColor = Color.White };

        var lblPrompt = new Label
        {
            Text = "Notificaciones del Miembro:",
            Location = new Point(20, 15),
            AutoSize = true,
            Font = new Font("Segoe UI", 10f, FontStyle.Bold)
        };

        lstNotificaciones = new ListBox
        {
            Location = new Point(20, 48),
            Size = new Size(880, 555),
            HorizontalScrollbar = true,
            Font = new Font("Segoe UI", 10f)
        };

        var btnMarcarLeido = new Button
        {
            Text = "Marcar como Leida",
            Location = new Point(920, 48),
            Width = 215,
            Height = 45,
            BackColor = Color.FromArgb(37, 99, 235),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };
        btnMarcarLeido.Click += (s, e) => MarcarNotificacionLeida();

        tab.Controls.AddRange(new Control[] { lblPrompt, lstNotificaciones, btnMarcarLeido });
        return tab;
    }

    private TabPage CrearTabHistorial()
    {
        var tab = new TabPage("Historial de Asistencia") { BackColor = Color.White };

        dgvHistorial = new DataGridView
        {
            Location = new Point(20, 20),
            Size = new Size(1115, 580),
            ReadOnly = true,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            BackgroundColor = Color.White
        };

        tab.Controls.Add(dgvHistorial);
        return tab;
    }

    // --- CARGA DE DATOS Y EVENTOS ---

    private void CargarUsuariosIniciales()
    {
        var usuarios = _usuarioCtrl.ListarUsuarios();
        var idSeleccionado = _usuarioActual?.Id;
        cmbSelectorUsuarios.Items.Clear();
        foreach (var u in usuarios)
        {
            cmbSelectorUsuarios.Items.Add(u);
        }

        if (cmbSelectorUsuarios.Items.Count > 0)
        {
            var idx = usuarios.FindIndex(u => u.Id == idSeleccionado);
            cmbSelectorUsuarios.SelectedIndex = idx >= 0 ? idx : 0;
        }

        RecargarGridUsuarios();
        RecargarGridClases();
    }

    private void ConfigurarRolYPermisos()
    {
        if (_usuarioAutenticado == null) return;

        if (_usuarioAutenticado is UsuarioBasico miembro)
        {
            var idx = cmbSelectorUsuarios.Items.Cast<UsuarioBasico>().ToList().FindIndex(u => u.Id == miembro.Id);
            if (idx >= 0)
            {
                cmbSelectorUsuarios.SelectedIndex = idx;
            }
            lblSelPrompt.Visible = false;
            cmbSelectorUsuarios.Visible = false;
        }
        else if (_usuarioAutenticado is Entrenador)
        {
            lblSelPrompt.Text = "Monitorear Miembro:";
            lblSelPrompt.Visible = true;
            cmbSelectorUsuarios.Visible = true;
        }
        else if (_usuarioAutenticado is Administrador)
        {
            lblSelPrompt.Text = "Miembro Activo:";
            lblSelPrompt.Visible = true;
            cmbSelectorUsuarios.Visible = true;
        }
    }

    private void CambiarUsuarioSeleccionado()
    {
        if (cmbSelectorUsuarios.SelectedItem is UsuarioBasico u)
        {
            _usuarioActual = u;

            // Evaluar gamificacion
            _gamificacionCtrl.EvaluarInsignias(_usuarioActual);

            ActualizarDashboard();
            ActualizarRutinas();
            ActualizarInsignias();
            ActualizarNotificaciones();
            ActualizarHistorial();
        }
    }

    private void ActualizarDashboard()
    {
        if (_usuarioActual == null) return;

        var m = _dashboardCtrl.CalcularMetricas(_usuarioActual);
        lblResumenUsuario.Text = $"Resumen Fisico: {_usuarioActual.NombreCompleto} | Cedula: {_usuarioActual.Cedula} | Estado: {_usuarioActual.Estado}";
        lblKpiAdherencia.Text = $"{m.AdherenciaSemanalPct}%";
        lblKpiVolumen.Text = $"{m.VolumenTotalLevantadoKg:N0} kg";
        lblKpiCalorias.Text = $"{m.CaloriasTotalesQuemadas:N0} kcal";
        lblKpiImc.Text = $"{m.ImcActual:F1}\n{m.ClasificacionImc}";
        lblKpiInactividad.Text = $"{m.DiasInactividad} dias";

        txtDetalleDashboard.Text =
            "REPORTE DE ESTADO FISICO - FIT.CR\r\n" +
            "========================================================================================\r\n" +
            $"Miembro: {_usuarioActual.NombreCompleto} | Identificacion: {_usuarioActual.Cedula}\r\n" +
            $"Condicion de Membresia: {_usuarioActual.Estado} | Entrenador Asignado: {_usuarioActual.EntrenadorAsignadoId ?? "Sin asignar"}\r\n\r\n" +
            "1. DATOS FISICOS:\r\n" +
            $"   - Peso Actual: {_usuarioActual.PesoKg:F1} kg | Estatura: {_usuarioActual.AlturaM:F2} m\r\n" +
            $"   - Indice de Masa Corporal (IMC): {m.ImcActual:F1} kg/m2 -> Clasificacion: {m.ClasificacionImc}\r\n\r\n" +
            "2. ASISTENCIA Y RENDIMIENTO:\r\n" +
            $"   - Porcentaje de Asistencia: {m.AdherenciaSemanalPct}%\r\n" +
            $"   - Carga Total de Entrenamiento Acumulada: {m.VolumenTotalLevantadoKg:N0} kg\r\n" +
            $"   - Gasto Energetico Estimado: {m.CaloriasTotalesQuemadas:N0} kcal\r\n" +
            $"   - Dias sin Asistir: {m.DiasInactividad} dias\r\n\r\n" +
            "3. RECOMENDACION:\r\n" +
            (m.DiasInactividad > 7
                ? $"   [INACTIVIDAD DETECTADA]: Se registran {m.DiasInactividad} dias sin registrar entrenamiento. Se asigno insignia por inactividad. Se recomienda retomar la asistencia."
                : $"   [ASISTENCIA REGULAR]: El miembro mantiene un ritmo constante de asistencia ({m.AdherenciaSemanalPct}%). Se recomienda continuar con su rutina asignada.");
    }

    private void RecargarGridUsuarios(string filtro = "")
    {
        var lista = _usuarioCtrl.ListarUsuarios(filtro).Select(u => new
        {
            u.Id,
            u.Cedula,
            Nombre = u.NombreCompleto,
            u.Edad,
            u.PesoKg,
            u.AlturaM,
            u.IMC,
            u.Estado
        }).ToList();

        dgvUsuarios.DataSource = null;
        dgvUsuarios.DataSource = lista;
    }

    private void CargarUsuarioDesdeGrid()
    {
        if (dgvUsuarios.CurrentRow?.DataBoundItem != null)
        {
            var id = dgvUsuarios.CurrentRow.Cells["Id"].Value?.ToString();
            var usuario = _usuarioCtrl.ListarUsuarios().FirstOrDefault(u => u.Id == id);
            if (usuario != null)
            {
                _usuarioEnEdicion = usuario;
                txtNombre.Text = usuario.Nombre;
                txtApellidos.Text = usuario.Apellidos;
                txtCedula.Text = usuario.Cedula;
                txtCorreo.Text = usuario.Correo;
                numEdad.Value = Math.Clamp(usuario.Edad, numEdad.Minimum, numEdad.Maximum);
                numPeso.Value = Math.Clamp((decimal)usuario.PesoKg, numPeso.Minimum, numPeso.Maximum);
                numAltura.Value = Math.Clamp((decimal)usuario.AlturaM, numAltura.Minimum, numAltura.Maximum);
                cmbEstadoUsuario.SelectedItem = string.Equals(usuario.Estado, "Inactivo", StringComparison.OrdinalIgnoreCase) ? "Inactivo" : "Activo";
                ActualizarPreviewImc();
            }
        }
    }

    private void GuardarUsuarioFormulario()
    {
        var usuario = _usuarioEnEdicion ?? new UsuarioBasico();
        usuario.Nombre = txtNombre.Text.Trim();
        usuario.Apellidos = txtApellidos.Text.Trim();
        usuario.Cedula = txtCedula.Text.Trim();
        usuario.Correo = txtCorreo.Text.Trim();
        usuario.Edad = (int)numEdad.Value;
        usuario.PesoKg = (double)numPeso.Value;
        usuario.AlturaM = (double)numAltura.Value;
        usuario.Estado = cmbEstadoUsuario.SelectedItem?.ToString() ?? "Activo";

        var (esValido, mensaje) = _usuarioCtrl.ValidarYGuardarUsuario(usuario);
        MessageBox.Show(mensaje, "Fit.CR", MessageBoxButtons.OK, esValido ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

        if (esValido)
        {
            _usuarioEnEdicion = null;
            LimpiarFormularioUsuario();
            CargarUsuariosIniciales();
            ActualizarDashboard();
        }
    }

    private void LimpiarFormularioUsuario()
    {
        _usuarioEnEdicion = null;
        txtNombre.Clear();
        txtApellidos.Clear();
        txtCedula.Clear();
        txtCorreo.Clear();
        numEdad.Value = 25;
        numPeso.Value = 70;
        numAltura.Value = 1.70m;
        cmbEstadoUsuario.SelectedItem = "Activo";
        ActualizarPreviewImc();
    }

    private void ActualizarRutinas()
    {
        lstRutinas.Items.Clear();
        if (_usuarioActual == null) return;

        foreach (var r in _usuarioActual.Rutinas)
        {
            lstRutinas.Items.Add(r);
        }

        if (lstRutinas.Items.Count > 0)
        {
            lstRutinas.SelectedIndex = 0;
        }
        else
        {
            dgvEjercicios.DataSource = null;
            lblResumenRutina.Text = "Este usuario aun no tiene rutinas asignadas.";
        }
    }

    private void MostrarEjerciciosRutina()
    {
        if (lstRutinas.SelectedItem is Rutina r)
        {
            var ejercicios = r.Ejercicios.Select(e => new
            {
                e.Nombre,
                e.Series,
                e.Repeticiones,
                CargaKg = e.PesoKg,
                e.DescansoSeg,
                e.CaloriasEstimadas,
                VolumenKg = e.VolumenLevantadoKg
            }).ToList();

            dgvEjercicios.DataSource = null;
            dgvEjercicios.DataSource = ejercicios;
            lblResumenRutina.Text = $"Rutina: {r.Nombre} | Volumen Total de la Sesion: {r.VolumenTotalKg:N0} kg | Calorias Estimadas: {r.CaloriasTotales} kcal";
        }
    }

    private void RecargarGridClases()
    {
        var clases = _claseCtrl.ListarClases().Select(c => new
        {
            c.Id,
            Clase = c.Nombre,
            c.Dia,
            c.Hora,
            Cupo = $"{c.Inscritos.Count}/{c.CupoMaximo}",
            Disponibles = c.CuposDisponibles
        }).ToList();

        dgvClases.DataSource = null;
        dgvClases.DataSource = clases;
    }

    private void ActualizarDetalleClase()
    {
        if (dgvClases.CurrentRow?.DataBoundItem != null)
        {
            var id = dgvClases.CurrentRow.Cells["Id"].Value?.ToString();
            var clase = _claseCtrl.ListarClases().FirstOrDefault(c => c.Id == id);
            if (clase != null)
            {
                lblCuposInfo.Text = $"{clase.Nombre} ({clase.Dia} {clase.Hora}) - Disponibles: {clase.CuposDisponibles}/{clase.CupoMaximo}";
                lstInscritosClase.Items.Clear();
                foreach (var uid in clase.Inscritos)
                {
                    var u = _usuarioCtrl.ListarUsuarios().FirstOrDefault(x => x.Id == uid);
                    lstInscritosClase.Items.Add(u != null ? $"{u.NombreCompleto} ({u.Cedula})" : uid);
                }
            }
        }
    }

    private void InscribirEnClase()
    {
        if (_usuarioActual == null) return;
        var id = dgvClases.CurrentRow?.Cells["Id"].Value?.ToString();
        if (id == null) return;

        var (exito, mensaje) = _claseCtrl.InscribirUsuario(id, _usuarioActual.Id);
        MessageBox.Show(mensaje, "Inscripcion", MessageBoxButtons.OK, exito ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

        RecargarGridClases();
        ActualizarDetalleClase();
        ActualizarNotificaciones();
    }

    private void CancelarReservaClase()
    {
        if (_usuarioActual == null) return;
        var id = dgvClases.CurrentRow?.Cells["Id"].Value?.ToString();
        if (id == null) return;

        var (exito, mensaje) = _claseCtrl.CancelarInscripcion(id, _usuarioActual.Id);
        MessageBox.Show(mensaje, "Cancelacion", MessageBoxButtons.OK, exito ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

        RecargarGridClases();
        ActualizarDetalleClase();
    }

    private void ActualizarInsignias()
    {
        pnlInsignias.Controls.Clear();
        if (_usuarioActual == null) return;

        foreach (var ins in _usuarioActual.Insignias)
        {
            var card = new Panel
            {
                Size = new Size(335, 140),
                BackColor = ins.EsVerguenza ? Color.FromArgb(254, 242, 242) : Color.FromArgb(236, 253, 245),
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(12)
            };

            var topBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 5,
                BackColor = ins.EsVerguenza ? Color.FromArgb(239, 68, 68) : Color.FromArgb(16, 185, 129)
            };
            card.Controls.Add(topBar);

            var lblT = new Label
            {
                Text = (ins.EsVerguenza ? "[ALERTA] " : "[LOGRO] ") + ins.Nombre,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = ins.EsVerguenza ? Color.FromArgb(185, 28, 28) : Color.FromArgb(4, 120, 87),
                Location = new Point(12, 14),
                AutoSize = true
            };
            card.Controls.Add(lblT);

            var lblDesc = new Label
            {
                Text = ins.Descripcion,
                Location = new Point(12, 42),
                Size = new Size(310, 55),
                ForeColor = Color.FromArgb(51, 65, 85)
            };
            card.Controls.Add(lblDesc);

            var lblFecha = new Label
            {
                Text = $"Obtenida: {ins.FechaObtencion}",
                Font = new Font("Segoe UI", 8f, FontStyle.Italic),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(12, 110),
                AutoSize = true
            };
            card.Controls.Add(lblFecha);

            pnlInsignias.Controls.Add(card);
        }
    }

    private void ActualizarNotificaciones()
    {
        lstNotificaciones.Items.Clear();
        if (_usuarioActual == null) return;

        foreach (var n in _usuarioActual.Notificaciones)
        {
            lstNotificaciones.Items.Add(n);
        }
    }

    private void MarcarNotificacionLeida()
    {
        if (lstNotificaciones.SelectedItem is Notificacion n)
        {
            n.Leida = true;
            FitDataService.Instancia.GuardarCambios();
            ActualizarNotificaciones();
        }
    }

    private void ActualizarHistorial()
    {
        if (_usuarioActual == null) return;

        var historial = _usuarioActual.HistorialAsistencias.Select(a => new
        {
            a.Fecha,
            Minutos = a.MinutosEntrenamiento,
            a.CaloriasQuemadas,
            Estado = "Completado"
        }).ToList();

        dgvHistorial.DataSource = null;
        dgvHistorial.DataSource = historial;
    }
}
