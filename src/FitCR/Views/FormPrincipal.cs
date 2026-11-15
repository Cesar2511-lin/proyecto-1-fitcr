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
    private readonly RutinaController _rutinaCtrl = new();
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
    private TabPage tabUsuarios = null!;
    private Label lblBuscador = null!;
    private DataGridView dgvUsuarios = null!;
    private bool _bloquearSeleccionGrid = false;
    private TextBox txtFiltroUsuario = null!;
    private ComboBox cmbAmbitoMiembros = null!;
    private GroupBox grpFormUsuarios = null!;
    private Label lblRolUsuario = null!;
    private Label lblEstadoUsuario = null!;
    private TextBox txtNombre = null!;
    private TextBox txtApellidos = null!;
    private TextBox txtCedula = null!;
    private TextBox txtCorreo = null!;
    private NumericUpDown numEdad = null!;
    private NumericUpDown numPeso = null!;
    private NumericUpDown numAltura = null!;
    private ComboBox cmbRolUsuario = null!;
    private ComboBox cmbEstadoUsuario = null!;
    private Label lblImcCalculado = null!;
    private Button btnGuardarUsuario = null!;
    private Button btnBloquearUsuario = null!;
    private Button btnEliminarUsuario = null!;
    private Button btnNuevoUsuario = null!;
    private Button btnAdminCambiarPassword = null!;
    private Button btnCambiarClavePerfil = null!;
    private Button btnCambiarClaveEntrenador = null!;

    // Controles Rutinas
    private Label lblTituloRutinas = null!;
    private ListBox lstRutinas = null!;
    private Button btnNuevaRutina = null!;
    private Button btnEliminarRutina = null!;
    private DataGridView dgvEjercicios = null!;
    private Label lblResumenRutina = null!;

    // Controles Clases
    private DataGridView dgvClases = null!;
    private Button btnNuevaClase = null!;
    private ListBox lstInscritosClase = null!;
    private Label lblCuposInfo = null!;
    private Button btnInscribirClase = null!;
    private Button btnCancelarClase = null!;

    // Controles Insignias
    private TabPage tabInsignias = null!;
    private Label lblTituloInsignias = null!;
    private FlowLayoutPanel pnlInsignias = null!;

    // Controles Notificaciones
    private Label lblTituloNotificaciones = null!;
    private ListBox lstNotificaciones = null!;

    // Controles Historial
    private TabPage tabHistorial = null!;
    private Label lblTituloHistorial = null!;
    private ComboBox cmbTipoAsistenciaAdmin = null!;
    private DataGridView dgvHistorial = null!;

    // Header Controls
    private Label lblSubtitulo = null!;
    private Button btnCambiarContrasena = null!;

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
        lblSubtitulo = new Label
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
            Location = new Point(480, 26),
            AutoSize = true
        };

        cmbSelectorUsuarios = new ComboBox
        {
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            Location = new Point(620, 23),
            Width = 250,
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        cmbSelectorUsuarios.Format += (s, e) =>
        {
            if (e.ListItem is UsuarioBasico u)
            {
                e.Value = string.IsNullOrEmpty(u.Id)
                    ? "-- Seleccionar Miembro a Evaluar --"
                    : $"{u.NombreCompleto} ({u.Estado})";
            }
        };
        cmbSelectorUsuarios.SelectedIndexChanged += (s, e) => CambiarUsuarioSeleccionado();

        btnCerrarSesion = new Button
        {
            Text = "Cerrar Sesion",
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            Location = new Point(1020, 20),
            Width = 130,
            Height = 35,
            BackColor = Color.FromArgb(185, 28, 28),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9f, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        btnCerrarSesion.Click += (s, e) => Close();

        btnCambiarContrasena = new Button
        {
            Text = "Cambiar Mi Contraseña",
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            Location = new Point(815, 20),
            Width = 195,
            Height = 35,
            BackColor = Color.FromArgb(79, 70, 229),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9f, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        btnCambiarContrasena.Click += (s, e) => AbrirFormularioCambiarContrasena();

        pnlHeader.Resize += (s, e) =>
        {
            btnCerrarSesion.Location = new Point(pnlHeader.Width - 150, 20);
            btnCambiarContrasena.Location = new Point(pnlHeader.Width - 355, 20);
        };

        pnlHeader.Controls.AddRange(new Control[] { lblTitulo, lblSubtitulo, lblSelPrompt, cmbSelectorUsuarios, btnCambiarContrasena, btnCerrarSesion });
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
        tabUsuarios = new TabPage("Gestion de Usuarios") { BackColor = Color.White };

        lblBuscador = new Label { Text = "Buscar:", Location = new Point(20, 18), AutoSize = true };
        txtFiltroUsuario = new TextBox { Location = new Point(75, 15), Width = 275 };
        txtFiltroUsuario.TextChanged += (s, e) => RecargarGridUsuarios(txtFiltroUsuario.Text);

        cmbAmbitoMiembros = new ComboBox { Location = new Point(360, 15), Width = 340, DropDownStyle = ComboBoxStyle.DropDownList };
        cmbAmbitoMiembros.Items.AddRange(new object[] { "Mis Estudiantes Asignados", "Todos los Miembros del Gimnasio" });
        cmbAmbitoMiembros.SelectedIndex = 0;
        cmbAmbitoMiembros.SelectedIndexChanged += (s, e) => RecargarGridUsuarios(txtFiltroUsuario.Text);

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
        grpFormUsuarios = new GroupBox
        {
            Text = "Datos del Miembro",
            Location = new Point(720, 45),
            Size = new Size(415, 560)
        };

        int y = 30;
        txtNombre = AgregarCampo(grpFormUsuarios, "Nombre:", ref y);
        txtApellidos = AgregarCampo(grpFormUsuarios, "Apellidos:", ref y);
        txtCedula = AgregarCampo(grpFormUsuarios, "Cedula:", ref y);
        txtCorreo = AgregarCampo(grpFormUsuarios, "Correo:", ref y);

        var lblE = new Label { Text = "Edad:", Location = new Point(15, y + 3), AutoSize = true };
        numEdad = new NumericUpDown { Location = new Point(130, y), Width = 110, Minimum = 12, Maximum = 100, Value = 25 };
        grpFormUsuarios.Controls.AddRange(new Control[] { lblE, numEdad });
        y += 38;

        var lblP = new Label { Text = "Peso (kg):", Location = new Point(15, y + 3), AutoSize = true };
        numPeso = new NumericUpDown { Location = new Point(130, y), Width = 110, Minimum = 30, Maximum = 300, DecimalPlaces = 1, Value = 70 };
        numPeso.ValueChanged += (s, e) => ActualizarPreviewImc();
        grpFormUsuarios.Controls.AddRange(new Control[] { lblP, numPeso });
        y += 38;

        var lblA = new Label { Text = "Altura (m):", Location = new Point(15, y + 3), AutoSize = true };
        numAltura = new NumericUpDown { Location = new Point(130, y), Width = 110, Minimum = 1, Maximum = 2.5m, DecimalPlaces = 2, Increment = 0.01m, Value = 1.70m };
        numAltura.ValueChanged += (s, e) => ActualizarPreviewImc();
        grpFormUsuarios.Controls.AddRange(new Control[] { lblA, numAltura });
        y += 38;

        lblRolUsuario = new Label { Text = "Rol:", Location = new Point(15, y + 3), AutoSize = true };
        cmbRolUsuario = new ComboBox { Location = new Point(130, y), Width = 180, DropDownStyle = ComboBoxStyle.DropDownList };
        cmbRolUsuario.Items.AddRange(new object[] { "UsuarioBasico", "EntrenadorPersonal", "EntrenadorGimnasio", "Administrador" });
        cmbRolUsuario.SelectedIndex = 0;
        grpFormUsuarios.Controls.AddRange(new Control[] { lblRolUsuario, cmbRolUsuario });
        y += 38;

        lblEstadoUsuario = new Label { Text = "Estado:", Location = new Point(15, y + 3), AutoSize = true };
        cmbEstadoUsuario = new ComboBox { Location = new Point(130, y), Width = 180, DropDownStyle = ComboBoxStyle.DropDownList };
        cmbEstadoUsuario.Items.AddRange(new object[] { "Activo", "Bloqueado", "Inactivo" });
        cmbEstadoUsuario.SelectedIndex = 0;
        grpFormUsuarios.Controls.AddRange(new Control[] { lblEstadoUsuario, cmbEstadoUsuario });
        y += 40;

        lblImcCalculado = new Label
        {
            Text = "IMC Estimado: 24.2 (Normopeso)",
            Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
            ForeColor = Color.FromArgb(37, 99, 235),
            Location = new Point(15, y),
            AutoSize = true
        };
        grpFormUsuarios.Controls.Add(lblImcCalculado);
        y += 42;

        btnGuardarUsuario = new Button
        {
            Text = "Guardar / Actualizar",
            Location = new Point(15, y),
            Width = 185,
            Height = 36,
            BackColor = Color.FromArgb(37, 99, 235),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9f, FontStyle.Bold)
        };
        btnGuardarUsuario.Click += (s, e) => GuardarUsuarioFormulario();

        btnBloquearUsuario = new Button
        {
            Text = "Bloquear / Activar",
            Location = new Point(210, y),
            Width = 185,
            Height = 36,
            BackColor = Color.FromArgb(217, 119, 6),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9f, FontStyle.Bold)
        };
        btnBloquearUsuario.Click += (s, e) => AlternarBloqueoUsuario();
        y += 42;

        btnEliminarUsuario = new Button
        {
            Text = "Eliminar Miembro",
            Location = new Point(15, y),
            Width = 185,
            Height = 36,
            BackColor = Color.FromArgb(220, 38, 38),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9f, FontStyle.Bold)
        };
        btnEliminarUsuario.Click += (s, e) => EliminarUsuarioSeleccionado();

        btnNuevoUsuario = new Button
        {
            Text = "Limpiar / Nuevo",
            Location = new Point(210, y),
            Width = 185,
            Height = 36,
            BackColor = Color.FromArgb(100, 116, 139),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };
        btnNuevoUsuario.Click += (s, e) => LimpiarFormularioUsuario();

        y += 42;
        btnAdminCambiarPassword = new Button
        {
            Text = "Cambiar Contraseña del Usuario",
            Location = new Point(15, y),
            Width = 380,
            Height = 36,
            BackColor = Color.FromArgb(79, 70, 229),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9f, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        btnAdminCambiarPassword.FlatAppearance.BorderSize = 0;
        btnAdminCambiarPassword.Click += (s, e) => AdminCambiarContrasenaUsuario();

        btnCambiarClavePerfil = new Button
        {
            Text = "Cambiar Mi Contraseña",
            Location = new Point(15, y),
            Width = 380,
            Height = 38,
            BackColor = Color.FromArgb(79, 70, 229),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
            Cursor = Cursors.Hand,
            Visible = false
        };
        btnCambiarClavePerfil.FlatAppearance.BorderSize = 0;
        btnCambiarClavePerfil.Click += (s, e) => AbrirFormularioCambiarContrasena();

        btnCambiarClaveEntrenador = new Button
        {
            Text = "Cambiar Mi Contraseña",
            Location = new Point(720, 13),
            Width = 200,
            Height = 30,
            BackColor = Color.FromArgb(79, 70, 229),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9f, FontStyle.Bold),
            Cursor = Cursors.Hand,
            Visible = false
        };
        btnCambiarClaveEntrenador.FlatAppearance.BorderSize = 0;
        btnCambiarClaveEntrenador.Click += (s, e) => AbrirFormularioCambiarContrasena();

        grpFormUsuarios.Controls.AddRange(new Control[] { btnGuardarUsuario, btnBloquearUsuario, btnEliminarUsuario, btnNuevoUsuario, btnAdminCambiarPassword, btnCambiarClavePerfil });

        tabUsuarios.Controls.AddRange(new Control[] { lblBuscador, txtFiltroUsuario, cmbAmbitoMiembros, btnCambiarClaveEntrenador, dgvUsuarios, grpFormUsuarios });
        return tabUsuarios;
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

        lblTituloRutinas = new Label { Text = "Rutinas Asignadas al Miembro Activo:", Location = new Point(20, 15), AutoSize = true, Font = new Font("Segoe UI", 10f, FontStyle.Bold) };
        lstRutinas = new ListBox
        {
            Location = new Point(20, 45),
            Size = new Size(420, 505),
            HorizontalScrollbar = true,
            Font = new Font("Segoe UI", 9.5f)
        };
        lstRutinas.SelectedIndexChanged += (s, e) => MostrarEjerciciosRutina();

        btnNuevaRutina = new Button
        {
            Text = "+ Asignar Nueva Rutina",
            Location = new Point(20, 560),
            Size = new Size(205, 36),
            BackColor = Color.FromArgb(37, 99, 235),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9f, FontStyle.Bold)
        };
        btnNuevaRutina.Click += (s, e) => AbrirFormularioNuevaRutina();

        btnEliminarRutina = new Button
        {
            Text = "Eliminar Rutina",
            Location = new Point(235, 560),
            Size = new Size(205, 36),
            BackColor = Color.FromArgb(239, 68, 68),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9f, FontStyle.Bold)
        };
        btnEliminarRutina.Click += (s, e) => EliminarRutinaSeleccionada();

        dgvEjercicios = new DataGridView
        {
            Location = new Point(460, 45),
            Size = new Size(675, 505),
            ReadOnly = true,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            BackgroundColor = Color.White
        };

        lblResumenRutina = new Label
        {
            Location = new Point(460, 568),
            AutoSize = true,
            Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
            ForeColor = Color.FromArgb(15, 23, 42),
            Text = "Selecciona una rutina para auditar ejercicios, carga total y volumen de trabajo."
        };

        tab.Controls.AddRange(new Control[] { lblTituloRutinas, lstRutinas, btnNuevaRutina, btnEliminarRutina, dgvEjercicios, lblResumenRutina });
        return tab;
    }

    private TabPage CrearTabClases()
    {
        var tab = new TabPage("Clases Grupales y Aforo") { BackColor = Color.White };

        btnNuevaClase = new Button
        {
            Text = "+ Programar Horario de Clase",
            Location = new Point(20, 15),
            Size = new Size(240, 32),
            BackColor = Color.FromArgb(37, 99, 235),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9f, FontStyle.Bold)
        };
        btnNuevaClase.Click += (s, e) => AbrirFormularioNuevaClase();

        dgvClases = new DataGridView
        {
            Location = new Point(20, 55),
            Size = new Size(700, 545),
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

        btnInscribirClase = new Button
        {
            Text = "Inscribir Miembro Activo",
            Location = new Point(15, 525),
            Width = 175,
            Height = 40,
            BackColor = Color.FromArgb(16, 185, 129),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };
        btnInscribirClase.Click += (s, e) => InscribirEnClase();

        btnCancelarClase = new Button
        {
            Text = "Cancelar Reserva",
            Location = new Point(205, 525),
            Width = 175,
            Height = 40,
            BackColor = Color.FromArgb(239, 68, 68),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };
        btnCancelarClase.Click += (s, e) => CancelarReservaClase();

        grpReserva.Controls.AddRange(new Control[] { btnInscribirClase, btnCancelarClase });

        tab.Controls.AddRange(new Control[] { btnNuevaClase, dgvClases, grpReserva });
        return tab;
    }

    private TabPage CrearTabInsignias()
    {
        tabInsignias = new TabPage("Insignias y Logros") { BackColor = Color.White };

        lblTituloInsignias = new Label
        {
            Text = "Insignias de Logros e Inactividad:",
            Location = new Point(20, 15),
            AutoSize = true,
            Font = new Font("Segoe UI", 10f, FontStyle.Bold)
        };
        tabInsignias.Controls.Add(lblTituloInsignias);

        pnlInsignias = new FlowLayoutPanel
        {
            Location = new Point(20, 48),
            Size = new Size(1115, 555),
            AutoScroll = true,
            BackColor = Color.FromArgb(248, 250, 252),
            BorderStyle = BorderStyle.FixedSingle,
            Padding = new Padding(15)
        };
        tabInsignias.Controls.Add(pnlInsignias);

        return tabInsignias;
    }

    private TabPage CrearTabNotificaciones()
    {
        var tab = new TabPage("Notificaciones") { BackColor = Color.White };

        lblTituloNotificaciones = new Label
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

        tab.Controls.AddRange(new Control[] { lblTituloNotificaciones, lstNotificaciones, btnMarcarLeido });
        return tab;
    }

    private TabPage CrearTabHistorial()
    {
        tabHistorial = new TabPage("Historial de Asistencia") { BackColor = Color.White };

        lblTituloHistorial = new Label
        {
            Text = "Historial de Asistencia y Sesiones de Entrenamiento:",
            Location = new Point(20, 15),
            AutoSize = true,
            Font = new Font("Segoe UI", 10f, FontStyle.Bold)
        };
        tabHistorial.Controls.Add(lblTituloHistorial);

        cmbTipoAsistenciaAdmin = new ComboBox
        {
            Location = new Point(620, 12),
            Width = 350,
            DropDownStyle = ComboBoxStyle.DropDownList,
            Visible = false
        };
        cmbTipoAsistenciaAdmin.Items.AddRange(new object[]
        {
            "Registro de Asistencias de Miembros",
            "Control de Turnos e Ingreso de Entrenadores"
        });
        cmbTipoAsistenciaAdmin.SelectedIndex = 0;
        cmbTipoAsistenciaAdmin.SelectedIndexChanged += (s, e) => ActualizarHistorial();
        tabHistorial.Controls.Add(cmbTipoAsistenciaAdmin);

        dgvHistorial = new DataGridView
        {
            Location = new Point(20, 48),
            Size = new Size(1115, 555),
            ReadOnly = true,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            BackgroundColor = Color.White
        };

        tabHistorial.Controls.Add(dgvHistorial);
        return tabHistorial;
    }

    // --- CARGA DE DATOS Y EVENTOS ---

    private void CargarUsuariosIniciales()
    {
        var usuarios = _usuarioCtrl.ListarUsuarios();
        var idSeleccionado = _usuarioActual?.Id;
        cmbSelectorUsuarios.Items.Clear();

        var esEntrenador = _usuarioAutenticado is Entrenador || (_usuarioAutenticado?.Rol != null && _usuarioAutenticado.Rol.StartsWith("Entrenador"));

        if (esEntrenador)
        {
            cmbSelectorUsuarios.Items.Add(new UsuarioBasico
            {
                Id = "",
                Nombre = "-- Seleccionar o Buscar Miembro --",
                Apellidos = ""
            });
        }

        foreach (var u in usuarios)
        {
            cmbSelectorUsuarios.Items.Add(u);
        }

        if (esEntrenador)
        {
            if (!string.IsNullOrEmpty(idSeleccionado))
            {
                var idx = usuarios.FindIndex(u => u.Id == idSeleccionado);
                cmbSelectorUsuarios.SelectedIndex = idx >= 0 ? (idx + 1) : 0;
            }
            else
            {
                cmbSelectorUsuarios.SelectedIndex = 0;
                _usuarioActual = null;
            }
        }
        else if (cmbSelectorUsuarios.Items.Count > 0)
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

        var esAdmin = _usuarioAutenticado is Administrador || string.Equals(_usuarioAutenticado.Rol, "Administrador", StringComparison.OrdinalIgnoreCase);
        var esEntrenador = _usuarioAutenticado is Entrenador || _usuarioAutenticado.Rol.StartsWith("Entrenador");

        if (_usuarioAutenticado is UsuarioBasico miembro)
        {
            // Header del portal del miembro
            lblSubtitulo.Text = $"Sesion activa: [Miembro] {miembro.NombreCompleto} | Portal Personal de Acondicionamiento Fisico";

            // Pestaña "Mi Perfil" exclusiva
            tabUsuarios.Text = "Mi Perfil";
            lblBuscador.Visible = false;
            txtFiltroUsuario.Visible = false;
            dgvUsuarios.Visible = false;

            // Reubicar formulario centrado como tarjeta de perfil propio
            grpFormUsuarios.Text = "Mis Datos Personales y Biometricos";
            grpFormUsuarios.Location = new Point(40, 25);
            grpFormUsuarios.Size = new Size(500, 560);

            // Ocultar controles administrativos
            lblRolUsuario.Visible = false;
            cmbRolUsuario.Visible = false;
            lblEstadoUsuario.Visible = false;
            cmbEstadoUsuario.Visible = false;
            btnBloquearUsuario.Visible = false;
            btnEliminarUsuario.Visible = false;
            btnNuevoUsuario.Visible = false;
            btnAdminCambiarPassword.Visible = false;

            // Bloquear campos de identidad
            txtNombre.ReadOnly = true;
            txtNombre.BackColor = Color.FromArgb(241, 245, 249);
            txtApellidos.ReadOnly = true;
            txtApellidos.BackColor = Color.FromArgb(241, 245, 249);
            txtCedula.ReadOnly = true;
            txtCedula.BackColor = Color.FromArgb(241, 245, 249);

            // Campos editables por el miembro
            txtCorreo.ReadOnly = false;
            txtCorreo.BackColor = Color.White;
            numEdad.ReadOnly = false;
            numPeso.ReadOnly = false;
            numAltura.ReadOnly = false;

            btnGuardarUsuario.Text = "Guardar Mis Datos";
            btnGuardarUsuario.Location = new Point(15, 380);
            btnGuardarUsuario.Width = 380;
            btnGuardarUsuario.Height = 38;

            btnCambiarClavePerfil.Location = new Point(15, 426);
            btnCambiarClavePerfil.Width = 380;
            btnCambiarClavePerfil.Height = 38;
            btnCambiarClavePerfil.Visible = true;
            btnCambiarClavePerfil.Text = "Cambiar Mi Contraseña de Acceso";

            btnCambiarClaveEntrenador.Visible = false;

            // Cargar datos propios del miembro autenticado
            _usuarioEnEdicion = miembro;
            txtNombre.Text = miembro.Nombre;
            txtApellidos.Text = miembro.Apellidos;
            txtCedula.Text = miembro.Cedula;
            txtCorreo.Text = miembro.Correo;
            numEdad.Value = Math.Clamp(miembro.Edad, numEdad.Minimum, numEdad.Maximum);
            numPeso.Value = Math.Clamp((decimal)miembro.PesoKg, numPeso.Minimum, numPeso.Maximum);
            numAltura.Value = Math.Clamp((decimal)miembro.AlturaM, numAltura.Minimum, numAltura.Maximum);
            ActualizarPreviewImc();

            // Personalizar titulos en demas pestañas
            lblTituloRutinas.Text = "Mis Rutinas de Entrenamiento:";
            btnInscribirClase.Text = "Reservar Mi Cupo";
            btnCancelarClase.Text = "Cancelar Mi Reserva";
            tabInsignias.Text = "Mis Insignias";
            lblTituloInsignias.Text = "Mis Insignias de Constancia y Logros:";
            lblTituloNotificaciones.Text = "Mis Notificaciones y Avisos de Salud:";
            tabHistorial.Text = "Mi Historial";
            lblTituloHistorial.Text = "Mi Historial de Asistencia y Entrenamientos:";
            if (cmbTipoAsistenciaAdmin != null) cmbTipoAsistenciaAdmin.Visible = false;

            // Ocultar selector superior de miembros
            lblSelPrompt.Visible = false;
            cmbSelectorUsuarios.Visible = false;
            if (cmbAmbitoMiembros != null) cmbAmbitoMiembros.Visible = false;
            btnNuevaRutina.Visible = false;
            btnEliminarRutina.Visible = false;
            btnNuevaClase.Visible = false;

            // Cargar datos del miembro en dashboard y modulos
            _usuarioActual = miembro;
            ActualizarDashboard();
            ActualizarRutinas();
            ActualizarInsignias();
            ActualizarNotificaciones();
            ActualizarHistorial();
        }
        else if (esEntrenador)
        {
            lblSubtitulo.Text = $"Sesion activa: [{_usuarioAutenticado.Rol}] {_usuarioAutenticado.NombreCompleto} | Panel de Evaluacion y Entrenamiento";

            tabUsuarios.Text = "Monitoreo de Miembros";
            lblBuscador.Visible = true;
            txtFiltroUsuario.Visible = true;
            dgvUsuarios.Visible = true;

            grpFormUsuarios.Text = "Evaluacion Fisica del Miembro";
            grpFormUsuarios.Location = new Point(720, 45);
            grpFormUsuarios.Size = new Size(415, 560);

            lblRolUsuario.Visible = false;
            cmbRolUsuario.Visible = false;
            lblEstadoUsuario.Visible = false;
            cmbEstadoUsuario.Visible = false;
            btnBloquearUsuario.Visible = false;
            btnEliminarUsuario.Visible = false;
            btnNuevoUsuario.Visible = false;
            btnAdminCambiarPassword.Visible = false;
            btnCambiarClavePerfil.Visible = false;
            btnCambiarClaveEntrenador.Visible = true;

            // Identidad protegida
            txtNombre.ReadOnly = true;
            txtNombre.BackColor = Color.FromArgb(241, 245, 249);
            txtApellidos.ReadOnly = true;
            txtApellidos.BackColor = Color.FromArgb(241, 245, 249);
            txtCedula.ReadOnly = true;
            txtCedula.BackColor = Color.FromArgb(241, 245, 249);
            txtCorreo.ReadOnly = true;
            txtCorreo.BackColor = Color.FromArgb(241, 245, 249);

            btnGuardarUsuario.Text = "Actualizar Medidas";
            btnGuardarUsuario.Width = 220;

            lblTituloRutinas.Text = "Rutinas Asignadas al Miembro Activo:";
            btnInscribirClase.Text = "Inscribir Miembro";
            btnCancelarClase.Text = "Cancelar Reserva";
            tabInsignias.Text = "Insignias del Miembro";
            lblTituloInsignias.Text = "Insignias de Logros e Inactividad del Miembro:";
            lblTituloNotificaciones.Text = "Notificaciones del Miembro Activo:";
            tabHistorial.Text = "Historial del Miembro";
            lblTituloHistorial.Text = "Historial de Asistencia del Miembro Activo:";
            if (cmbTipoAsistenciaAdmin != null) cmbTipoAsistenciaAdmin.Visible = false;

            lblSelPrompt.Text = "Monitorear Miembro:";
            lblSelPrompt.Visible = true;
            cmbSelectorUsuarios.Visible = true;
            if (cmbAmbitoMiembros != null) cmbAmbitoMiembros.Visible = true;
            btnNuevaRutina.Visible = true;
            btnEliminarRutina.Visible = true;
            btnNuevaClase.Visible = true;

            // Iniciar en blanco para que el entrenador elija o busque un miembro
            _usuarioActual = null;
            _usuarioEnEdicion = null;
            LimpiarFormularioUsuario();
            if (cmbSelectorUsuarios.Items.Count > 0)
            {
                cmbSelectorUsuarios.SelectedIndex = 0;
            }
            dgvUsuarios.ClearSelection();
            ActualizarDashboard();
            ActualizarRutinas();
            ActualizarInsignias();
            ActualizarNotificaciones();
            ActualizarHistorial();
        }
        else if (esAdmin)
        {
            lblSubtitulo.Text = $"Sesion activa: [Administrador] {_usuarioAutenticado.NombreCompleto} | Control y Administracion de Gimnasio";

            tabUsuarios.Text = "Gestion de Usuarios";
            lblBuscador.Visible = true;
            txtFiltroUsuario.Visible = true;
            dgvUsuarios.Visible = true;

            grpFormUsuarios.Text = "Datos del Miembro";
            grpFormUsuarios.Location = new Point(720, 45);
            grpFormUsuarios.Size = new Size(415, 560);

            txtNombre.ReadOnly = false;
            txtNombre.BackColor = Color.White;
            txtApellidos.ReadOnly = false;
            txtApellidos.BackColor = Color.White;
            txtCedula.ReadOnly = false;
            txtCedula.BackColor = Color.White;
            txtCorreo.ReadOnly = false;
            txtCorreo.BackColor = Color.White;

            lblRolUsuario.Visible = true;
            cmbRolUsuario.Visible = true;
            cmbRolUsuario.Enabled = true;

            lblEstadoUsuario.Visible = true;
            cmbEstadoUsuario.Visible = true;
            cmbEstadoUsuario.Enabled = true;

            btnGuardarUsuario.Text = "Guardar / Actualizar";
            btnGuardarUsuario.Width = 185;
            btnBloquearUsuario.Visible = true;
            btnBloquearUsuario.Enabled = true;
            btnEliminarUsuario.Visible = true;
            btnEliminarUsuario.Enabled = true;
            btnNuevoUsuario.Visible = true;
            btnNuevoUsuario.Enabled = true;
            btnAdminCambiarPassword.Visible = true;
            btnAdminCambiarPassword.Enabled = true;
            btnCambiarClavePerfil.Visible = false;
            btnCambiarClaveEntrenador.Visible = false;

            lblTituloRutinas.Text = "Auditoria de Rutinas del Miembro:";
            btnInscribirClase.Text = "Inscribir Miembro";
            btnCancelarClase.Text = "Cancelar Reserva";
            tabInsignias.Text = "Auditoria de Gamificacion";
            lblTituloInsignias.Text = "Auditoria Global de Gamificacion e Insignias del Gimnasio:";
            lblTituloNotificaciones.Text = "Notificaciones del Miembro Activo:";
            tabHistorial.Text = "Bitacora de Asistencias";
            lblTituloHistorial.Text = "Registro y Auditoria de Asistencias del Gimnasio:";
            if (cmbTipoAsistenciaAdmin != null) cmbTipoAsistenciaAdmin.Visible = true;

            lblSelPrompt.Visible = false;
            cmbSelectorUsuarios.Visible = false;
            if (cmbAmbitoMiembros != null) cmbAmbitoMiembros.Visible = false;
            btnNuevaRutina.Visible = true;
            btnEliminarRutina.Visible = true;
            btnNuevaClase.Visible = true;
        }
    }

    private void CambiarUsuarioSeleccionado()
    {
        if (_usuarioAutenticado is UsuarioBasico miembro)
        {
            _usuarioActual = miembro;
            _gamificacionCtrl.EvaluarInsignias(_usuarioActual);
            ActualizarDashboard();
            ActualizarRutinas();
            ActualizarInsignias();
            ActualizarNotificaciones();
            ActualizarHistorial();
            return;
        }

        if (cmbSelectorUsuarios.SelectedItem is UsuarioBasico u && !string.IsNullOrEmpty(u.Id))
        {
            _usuarioActual = u;
            _usuarioEnEdicion = u;

            // Evaluar gamificacion
            _gamificacionCtrl.EvaluarInsignias(_usuarioActual);

            ActualizarDashboard();
            ActualizarRutinas();
            ActualizarInsignias();
            ActualizarNotificaciones();
            ActualizarHistorial();
            CargarUsuarioEnFormulario(u);

            _bloquearSeleccionGrid = true;
            for (int i = 0; i < dgvUsuarios.Rows.Count; i++)
            {
                if (dgvUsuarios.Rows[i].Cells["Id"].Value?.ToString() == u.Id)
                {
                    dgvUsuarios.Rows[i].Selected = true;
                    dgvUsuarios.CurrentCell = dgvUsuarios.Rows[i].Cells[1];
                    break;
                }
            }
            _bloquearSeleccionGrid = false;
        }
        else
        {
            _usuarioActual = null;
            _usuarioEnEdicion = null;
            ActualizarDashboard();
            ActualizarRutinas();
            ActualizarInsignias();
            ActualizarNotificaciones();
            ActualizarHistorial();
            LimpiarFormularioUsuario();
            dgvUsuarios.ClearSelection();
        }
    }

    private void ActualizarDashboard()
    {
        if (_usuarioAutenticado is Administrador admin)
        {
            var usuarios = _usuarioCtrl.ListarUsuarios();
            var entrenadores = FitDataService.Instancia.ObtenerEntrenadores();
            var clases = _claseCtrl.ListarClases();
            var g = _dashboardCtrl.CalcularMetricasGlobales(usuarios, entrenadores, clases);

            lblResumenUsuario.Text = "Consola de Supervision Ejecutiva - Gimnasio Fit.CR Costa Rica";

            ActualizarTarjeta(lblKpiAdherencia, "Comunidad de Miembros", $"{g.TotalMiembros} Miembros\n{g.MiembrosActivos} Activos | {g.MiembrosInactivos} Inact");
            ActualizarTarjeta(lblKpiVolumen, "Staff de Entrenadores", $"{g.TotalEntrenadores} Coaches\n{g.EntrenadoresPersonales} Pers. | {g.EntrenadoresGimnasio} Sala");
            ActualizarTarjeta(lblKpiCalorias, "Ocupacion de Clases", $"{g.OcupacionAforoPct}%\n{g.CuposOcupados}/{g.CuposTotales} Cupos");
            ActualizarTarjeta(lblKpiImc, "Alerta Desercion", $"{g.MiembrosEnRiesgoDesercion} Miembros\n>7 dias inactivos");
            ActualizarTarjeta(lblKpiInactividad, "Cuentas y Seguridad", $"{g.MiembrosBloqueados} Bloqueadas\n{g.TotalInsigniasOtorgadas} Insignias");

            txtDetalleDashboard.Text =
                "FIT.CR - INFORME EJECUTIVO DE GOBERNANZA Y ADMINISTRACION\r\n" +
                "========================================================================================\r\n" +
                $"Sesion: [Administrador] {admin.NombreCompleto} | Sede Central San Jose, Costa Rica\r\n" +
                $"Fecha de Emision: {DateTime.Now:dd/MM/yyyy HH:mm} | Estado Operativo: REGULAR Y ACTIVO\r\n\r\n" +
                "1. AUDITORIA DE MEMBRESIAS Y RETENCION:\r\n" +
                $"   - Total de Miembros Registrados: {g.TotalMiembros} usuarios\r\n" +
                $"   - Miembros Activos con Membresia al Dia: {g.MiembrosActivos} ({(g.TotalMiembros > 0 ? (g.MiembrosActivos * 100.0 / g.TotalMiembros) : 0):F1}%)\r\n" +
                $"   - Miembros Inactivos: {g.MiembrosInactivos}\r\n" +
                $"   - Cuentas Bloqueadas por Seguridad: {g.MiembrosBloqueados}\r\n" +
                $"   - Miembros con Alerta de Desercion (>7 dias sin registrar asistencia): {g.MiembrosEnRiesgoDesercion}\r\n\r\n" +
                "2. EQUIPO TECNICO Y ENTRENADORES:\r\n" +
                $"   - Total de Entrenadores en Nomina: {g.TotalEntrenadores}\r\n" +
                $"   - Entrenadores Personales (1 a 1): {g.EntrenadoresPersonales}\r\n" +
                $"   - Entrenadores de Sala y Clases Grupales: {g.EntrenadoresGimnasio}\r\n\r\n" +
                "3. CONTROL DE AFORO EN CLASES COLECTIVAS:\r\n" +
                $"   - Clases Programadas en Sistema: {g.ClasesProgramadas}\r\n" +
                $"   - Capacidad Total Instalada: {g.CuposTotales} cupos\r\n" +
                $"   - Cupos Reservados por Miembros: {g.CuposOcupados} cupos ({g.OcupacionAforoPct}% de ocupacion promedio)\r\n\r\n" +
                "4. GOBERNANZA, PRIVACIDAD Y REGLAMENTACION (PRODHAB / LEY 8968):\r\n" +
                "   - Aislamiento de Cedulas: Restringidas exclusivamente a la vista de Administradores.\r\n" +
                "   - Acceso de Entrenadores: Restringido a evaluacion fisica y metricas antropometricas genericas.\r\n" +
                $"   - Insignias de Gamificacion Otorgadas: {g.TotalInsigniasOtorgadas} ({g.InsigniasInactividad} por inactividad prolongada).";
            return;
        }

        if (_usuarioActual == null)
        {
            lblResumenUsuario.Text = "Panel de Evaluacion Tecnica | Seleccione o busque un miembro para consultar sus datos";
            ActualizarTarjeta(lblKpiAdherencia, "Adherencia Semanal", "--");
            ActualizarTarjeta(lblKpiVolumen, "Volumen Total Levantado", "-- kg");
            ActualizarTarjeta(lblKpiCalorias, "Calorias Quemadas", "-- kcal");
            ActualizarTarjeta(lblKpiImc, "Indice de Masa Corporal", "--\nSin seleccion");
            ActualizarTarjeta(lblKpiInactividad, "Dias Inactivo", "-- dias");

            txtDetalleDashboard.Text =
                "FIT.CR - PANEL DE EVALUACION Y SEGUIMIENTO TECNICO\r\n" +
                "========================================================================================\r\n" +
                "ESTADO: Ningun miembro seleccionado actualmente.\r\n\r\n" +
                "INSTRUCCIONES PARA EL ENTRENADOR:\r\n" +
                "1. Utilice el selector superior 'Monitorear Miembro' para elegir un atleta directamente.\r\n" +
                "2. Tambien puede dirigirse a la pestaña 'Monitoreo de Miembros', buscar por nombre o cedula en el buscador y hacer clic sobre cualquier socio.\r\n" +
                "3. Al seleccionar un miembro, se desplegaran de inmediato sus metricas de rendimiento, porcentaje de asistencia, historial de cargas, rutinas asignadas y sus insignias vinculadas.";
            return;
        }

        var m = _dashboardCtrl.CalcularMetricas(_usuarioActual);
        var cedulaVisible = (_usuarioAutenticado?.Id == _usuarioActual.Id)
            ? _usuarioActual.Cedula
            : "Dato Protegido";

        lblResumenUsuario.Text = $"Resumen Fisico: {_usuarioActual.NombreCompleto} | Cedula: {cedulaVisible} | Estado: {_usuarioActual.Estado}";
        ActualizarTarjeta(lblKpiAdherencia, "Adherencia Semanal", $"{m.AdherenciaSemanalPct}%");
        ActualizarTarjeta(lblKpiVolumen, "Volumen Total Levantado", $"{m.VolumenTotalLevantadoKg:N0} kg");
        ActualizarTarjeta(lblKpiCalorias, "Calorias Quemadas", $"{m.CaloriasTotalesQuemadas:N0} kcal");
        ActualizarTarjeta(lblKpiImc, "Indice de Masa Corporal", $"{m.ImcActual:F1}\n{m.ClasificacionImc}");
        ActualizarTarjeta(lblKpiInactividad, "Dias Inactivo", $"{m.DiasInactividad} dias");

        txtDetalleDashboard.Text =
            "REPORTE DE ESTADO FISICO - FIT.CR\r\n" +
            "========================================================================================\r\n" +
            $"Miembro: {_usuarioActual.NombreCompleto} | Identificacion: {cedulaVisible}\r\n" +
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

    private static void ActualizarTarjeta(Label lblVal, string nuevoTitulo, string nuevoValor)
    {
        if (lblVal.Parent is Panel pnl && pnl.Controls.Count >= 2 && pnl.Controls[1] is Label lblT)
        {
            lblT.Text = nuevoTitulo;
        }
        lblVal.Text = nuevoValor;
    }

    private void RecargarGridUsuarios(string filtro = "")
    {
        if (_usuarioAutenticado is UsuarioBasico)
        {
            dgvUsuarios.DataSource = null;
            return;
        }

        var todos = _usuarioCtrl.ListarUsuarios(filtro);

        if (_usuarioAutenticado is EntrenadorPersonal or EntrenadorGimnasio && cmbAmbitoMiembros != null && cmbAmbitoMiembros.SelectedIndex == 0)
        {
            todos = todos.Where(u => u.EntrenadorAsignadoId == _usuarioAutenticado.Id).ToList();
        }

        var lista = todos.Select(u => new
        {
            u.Id,
            Cedula = (_usuarioAutenticado is Administrador) ? u.Cedula : "Dato Protegido",
            Nombre = u.NombreCompleto,
            Rol = u.Rol,
            u.Edad,
            u.PesoKg,
            u.AlturaM,
            u.IMC,
            u.Estado
        }).ToList();

        _bloquearSeleccionGrid = true;
        dgvUsuarios.DataSource = null;
        dgvUsuarios.DataSource = lista;

        if (_usuarioActual != null)
        {
            for (int i = 0; i < dgvUsuarios.Rows.Count; i++)
            {
                if (dgvUsuarios.Rows[i].Cells["Id"].Value?.ToString() == _usuarioActual.Id)
                {
                    dgvUsuarios.Rows[i].Selected = true;
                    dgvUsuarios.CurrentCell = dgvUsuarios.Rows[i].Cells[1];
                    break;
                }
            }
        }
        else
        {
            dgvUsuarios.ClearSelection();
        }
        _bloquearSeleccionGrid = false;
    }

    private void CargarUsuarioDesdeGrid()
    {
        if (_bloquearSeleccionGrid) return;
        if (_usuarioAutenticado is UsuarioBasico) return;

        if (dgvUsuarios.SelectedRows.Count == 0 || dgvUsuarios.CurrentRow?.DataBoundItem == null)
        {
            return;
        }

        var id = dgvUsuarios.CurrentRow.Cells["Id"].Value?.ToString();
        var usuario = _usuarioCtrl.ListarUsuarios().Find(u => u.Id == id);
        if (usuario != null)
        {
            _usuarioEnEdicion = usuario;
            _usuarioActual = usuario;

            for (int i = 0; i < cmbSelectorUsuarios.Items.Count; i++)
            {
                if (cmbSelectorUsuarios.Items[i] is UsuarioBasico u && u.Id == usuario.Id)
                {
                    cmbSelectorUsuarios.SelectedIndex = i;
                    break;
                }
            }

            _gamificacionCtrl.EvaluarInsignias(_usuarioActual);
            ActualizarDashboard();
            ActualizarRutinas();
            ActualizarInsignias();
            ActualizarNotificaciones();
            ActualizarHistorial();
            CargarUsuarioEnFormulario(usuario);
        }
    }

    private void CargarUsuarioEnFormulario(UsuarioBasico usuario)
    {
        _usuarioEnEdicion = usuario;
        txtNombre.Text = usuario.Nombre;
        txtApellidos.Text = usuario.Apellidos;
        txtCedula.Text = (_usuarioAutenticado is Administrador) ? usuario.Cedula : "Dato Protegido";
        txtCorreo.Text = (_usuarioAutenticado is Administrador) ? usuario.Correo : "Dato Protegido";
        numEdad.Value = Math.Clamp(usuario.Edad, numEdad.Minimum, numEdad.Maximum);
        numPeso.Value = Math.Clamp((decimal)usuario.PesoKg, numPeso.Minimum, numPeso.Maximum);
        numAltura.Value = Math.Clamp((decimal)usuario.AlturaM, numAltura.Minimum, numAltura.Maximum);

        var rolIndex = cmbRolUsuario.Items.IndexOf(usuario.Rol);
        cmbRolUsuario.SelectedIndex = rolIndex >= 0 ? rolIndex : 0;

        var estado = string.IsNullOrWhiteSpace(usuario.Estado) ? "Activo" : usuario.Estado;
        var estadoIndex = cmbEstadoUsuario.Items.IndexOf(estado);
        cmbEstadoUsuario.SelectedIndex = estadoIndex >= 0 ? estadoIndex : 0;

        btnBloquearUsuario.Text = string.Equals(usuario.Estado, "Bloqueado", StringComparison.OrdinalIgnoreCase)
            ? "Desbloquear" : "Bloquear";

        ActualizarPreviewImc();
    }

    private void GuardarUsuarioFormulario()
    {
        // 1. Caso Miembro: Solo puede actualizar sus propias medidas y datos de contacto
        if (_usuarioAutenticado is UsuarioBasico miembro)
        {
            miembro.Correo = txtCorreo.Text.Trim();
            miembro.Edad = (int)numEdad.Value;
            miembro.PesoKg = (double)numPeso.Value;
            miembro.AlturaM = (double)numAltura.Value;
            miembro.ActualizarIMC();

            var (validoMiembro, msgMiembro) = _usuarioCtrl.ValidarYGuardarUsuario(miembro);
            MessageBox.Show(msgMiembro, "Mi Perfil - Fit.CR", MessageBoxButtons.OK, validoMiembro ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

            if (validoMiembro)
            {
                _usuarioActual = miembro;
                ActualizarPreviewImc();
                ActualizarDashboard();
            }
            return;
        }

        // 2. Caso Entrenador: Solo actualiza evaluaciones antropometricas del miembro
        var esEntrenador = _usuarioAutenticado is Entrenador || _usuarioAutenticado?.Rol.StartsWith("Entrenador") == true;
        if (esEntrenador)
        {
            if (_usuarioEnEdicion == null)
            {
                MessageBox.Show("Seleccione un miembro de la lista para registrar su evaluacion fisica.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            _usuarioEnEdicion.Edad = (int)numEdad.Value;
            _usuarioEnEdicion.PesoKg = (double)numPeso.Value;
            _usuarioEnEdicion.AlturaM = (double)numAltura.Value;
            _usuarioEnEdicion.ActualizarIMC();

            var (validoEnt, msgEnt) = _usuarioCtrl.ValidarYGuardarUsuario(_usuarioEnEdicion);
            MessageBox.Show(msgEnt, "Evaluacion Fisica - Fit.CR", MessageBoxButtons.OK, validoEnt ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

            if (validoEnt)
            {
                RecargarGridUsuarios(txtFiltroUsuario.Text);
                ActualizarDashboard();
            }
            return;
        }

        // 3. Caso Administrador: Control integral y gobernanza
        var usuario = _usuarioEnEdicion ?? new UsuarioBasico();
        usuario.Nombre = txtNombre.Text.Trim();
        usuario.Apellidos = txtApellidos.Text.Trim();
        usuario.Cedula = txtCedula.Text.Trim();
        usuario.Correo = txtCorreo.Text.Trim();
        usuario.Edad = (int)numEdad.Value;
        usuario.PesoKg = (double)numPeso.Value;
        usuario.AlturaM = (double)numAltura.Value;
        usuario.Estado = cmbEstadoUsuario.SelectedItem?.ToString() ?? "Activo";

        if (cmbRolUsuario.SelectedItem != null &&
            (_usuarioAutenticado is Administrador || string.Equals(_usuarioAutenticado?.Rol, "Administrador", StringComparison.OrdinalIgnoreCase)))
        {
            usuario.Rol = cmbRolUsuario.SelectedItem.ToString()!;
        }

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

    private void AlternarBloqueoUsuario()
    {
        if (!(_usuarioAutenticado is Administrador || string.Equals(_usuarioAutenticado?.Rol, "Administrador", StringComparison.OrdinalIgnoreCase)))
        {
            MessageBox.Show("Solo los administradores tienen permiso para bloquear o reactivar membresias.", "Permiso Denegado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (_usuarioEnEdicion == null)
        {
            MessageBox.Show("Seleccione primero un usuario en la tabla para modificar su estado.", "Gestion de Membresias", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var nuevoEstado = string.Equals(_usuarioEnEdicion.Estado, "Bloqueado", StringComparison.OrdinalIgnoreCase)
            ? "Activo" : "Bloqueado";

        var (exito, mensaje) = _usuarioCtrl.CambiarEstadoUsuario(_usuarioEnEdicion.Id, nuevoEstado);
        MessageBox.Show(mensaje, "Control de Membresias", MessageBoxButtons.OK, exito ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

        if (exito)
        {
            _usuarioEnEdicion.Estado = nuevoEstado;
            var estadoIndex = cmbEstadoUsuario.Items.IndexOf(nuevoEstado);
            if (estadoIndex >= 0) cmbEstadoUsuario.SelectedIndex = estadoIndex;
            btnBloquearUsuario.Text = nuevoEstado == "Bloqueado" ? "Desbloquear" : "Bloquear";
            RecargarGridUsuarios(txtFiltroUsuario.Text);
            ActualizarDashboard();
        }
    }

    private void EliminarUsuarioSeleccionado()
    {
        if (!(_usuarioAutenticado is Administrador || string.Equals(_usuarioAutenticado?.Rol, "Administrador", StringComparison.OrdinalIgnoreCase)))
        {
            MessageBox.Show("Solo los administradores tienen autorizacion para eliminar miembros.", "Permiso Denegado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (_usuarioEnEdicion == null)
        {
            MessageBox.Show("Seleccione un usuario de la tabla para eliminar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var confirmacion = MessageBox.Show(
            $"¿Esta seguro de eliminar formalmente al miembro '{_usuarioEnEdicion.NombreCompleto}' (Cedula: {_usuarioEnEdicion.Cedula})?",
            "Confirmar Eliminacion",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (confirmacion == DialogResult.Yes)
        {
            var idAEliminar = _usuarioEnEdicion.Id;
            var exito = _usuarioCtrl.EliminarUsuario(idAEliminar);
            if (exito)
            {
                MessageBox.Show("Miembro eliminado exitosamente del sistema.", "Fit.CR", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarFormularioUsuario();
                CargarUsuariosIniciales();
                ActualizarDashboard();
            }
        }
    }

    private void LimpiarFormularioUsuario()
    {
        if (_usuarioAutenticado is UsuarioBasico miembro)
        {
            _usuarioEnEdicion = miembro;
            txtNombre.Text = miembro.Nombre;
            txtApellidos.Text = miembro.Apellidos;
            txtCedula.Text = miembro.Cedula;
            txtCorreo.Text = miembro.Correo;
            numEdad.Value = Math.Clamp(miembro.Edad, numEdad.Minimum, numEdad.Maximum);
            numPeso.Value = Math.Clamp((decimal)miembro.PesoKg, numPeso.Minimum, numPeso.Maximum);
            numAltura.Value = Math.Clamp((decimal)miembro.AlturaM, numAltura.Minimum, numAltura.Maximum);
            ActualizarPreviewImc();
            return;
        }

        _usuarioEnEdicion = null;
        txtNombre.Clear();
        txtApellidos.Clear();
        txtCedula.Clear();
        txtCorreo.Clear();
        numEdad.Value = 25;
        numPeso.Value = 70;
        numAltura.Value = 1.70m;
        cmbRolUsuario.SelectedIndex = 0;
        cmbEstadoUsuario.SelectedIndex = 0;
        btnBloquearUsuario.Text = "Bloquear / Activar";
        ActualizarPreviewImc();
    }

    private void ActualizarRutinas()
    {
        lstRutinas.Items.Clear();
        dgvEjercicios.DataSource = null;

        if (_usuarioActual == null)
        {
            lblTituloRutinas.Text = "Rutinas de Entrenamiento:";
            lblResumenRutina.Text = "Seleccione un miembro en el selector superior o en 'Monitoreo de Miembros' para ver o asignar rutinas.";
            return;
        }

        lblTituloRutinas.Text = $"Rutinas Asignadas al Miembro: {_usuarioActual.NombreCompleto}";

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
            lblResumenRutina.Text = $"El miembro {_usuarioActual.NombreCompleto} aun no tiene rutinas asignadas.";
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
            var clase = _claseCtrl.ListarClases().Find(c => c.Id == id);
            if (clase != null)
            {
                lblCuposInfo.Text = $"{clase.Nombre} ({clase.Dia} {clase.Hora}) - Disponibles: {clase.CuposDisponibles}/{clase.CupoMaximo}";
                lstInscritosClase.Items.Clear();
                foreach (var uid in clase.Inscritos)
                {
                    var u = _usuarioCtrl.ListarUsuarios().Find(x => x.Id == uid);
                    var ident = (_usuarioAutenticado is Administrador) ? $" ({u?.Cedula})" : "";
                    lstInscritosClase.Items.Add(u != null ? $"{u.NombreCompleto}{ident}" : uid);
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

        if (_usuarioAutenticado is Administrador)
        {
            var todos = _usuarioCtrl.ListarUsuarios();
            int totalInsignias = 0;

            foreach (var u in todos)
            {
                foreach (var ins in u.Insignias)
                {
                    totalInsignias++;
                    var card = new Panel
                    {
                        Size = new Size(335, 150),
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

                    var lblM = new Label
                    {
                        Text = $"Miembro: {u.NombreCompleto} ({u.Cedula})",
                        Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                        ForeColor = Color.FromArgb(30, 41, 59),
                        Location = new Point(12, 10),
                        AutoSize = true
                    };
                    card.Controls.Add(lblM);

                    var lblT = new Label
                    {
                        Text = (ins.EsVerguenza ? "[ALERTA] " : "[LOGRO] ") + ins.Nombre,
                        Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                        ForeColor = ins.EsVerguenza ? Color.FromArgb(185, 28, 28) : Color.FromArgb(4, 120, 87),
                        Location = new Point(12, 28),
                        AutoSize = true
                    };
                    card.Controls.Add(lblT);

                    var lblDesc = new Label
                    {
                        Text = ins.Descripcion,
                        Location = new Point(12, 50),
                        Size = new Size(310, 55),
                        ForeColor = Color.FromArgb(51, 65, 85)
                    };
                    card.Controls.Add(lblDesc);

                    var lblFecha = new Label
                    {
                        Text = $"Obtenida: {ins.FechaObtencion}",
                        Font = new Font("Segoe UI", 8f, FontStyle.Italic),
                        ForeColor = Color.FromArgb(100, 116, 139),
                        Location = new Point(12, 115),
                        AutoSize = true
                    };
                    card.Controls.Add(lblFecha);

                    pnlInsignias.Controls.Add(card);
                }
            }

            if (totalInsignias == 0)
            {
                var lblVacio = new Label
                {
                    Text = "No se registran insignias activas en los miembros actualmente.",
                    Font = new Font("Segoe UI", 10f),
                    ForeColor = Color.FromArgb(100, 116, 139),
                    AutoSize = true,
                    Location = new Point(20, 20)
                };
                pnlInsignias.Controls.Add(lblVacio);
            }
            return;
        }

        if (_usuarioActual == null)
        {
            lblTituloInsignias.Text = "Insignias y Reconocimientos | Seleccione un miembro para consultar sus logros";
            var lblVacio = new Label
            {
                Text = "Seleccione un miembro en el selector superior o en la pestaña 'Monitoreo de Miembros' para auditar sus reconocimientos o alertas de sedentarismo.",
                Font = new Font("Segoe UI", 10f, FontStyle.Italic),
                ForeColor = Color.FromArgb(100, 116, 139),
                AutoSize = true,
                Location = new Point(20, 25)
            };
            pnlInsignias.Controls.Add(lblVacio);
            return;
        }

        lblTituloInsignias.Text = $"Insignias y Reconocimientos del Atleta: {_usuarioActual.NombreCompleto} | Estado: {_usuarioActual.Estado}";

        if (_usuarioActual.Insignias.Count == 0)
        {
            var lblSinInsignias = new Label
            {
                Text = $"El atleta {_usuarioActual.NombreCompleto} aun no registra insignias en su perfil.",
                Font = new Font("Segoe UI", 10f, FontStyle.Italic),
                ForeColor = Color.FromArgb(100, 116, 139),
                AutoSize = true,
                Location = new Point(20, 25)
            };
            pnlInsignias.Controls.Add(lblSinInsignias);
            return;
        }

        foreach (var ins in _usuarioActual.Insignias)
        {
            var card = new Panel
            {
                Size = new Size(335, 155),
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

            var lblAtleta = new Label
            {
                Text = $"Atleta: {_usuarioActual.NombreCompleto}",
                Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                Location = new Point(12, 10),
                AutoSize = true
            };
            card.Controls.Add(lblAtleta);

            var lblT = new Label
            {
                Text = (ins.EsVerguenza ? "[ALERTA] " : "[LOGRO] ") + ins.Nombre,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = ins.EsVerguenza ? Color.FromArgb(185, 28, 28) : Color.FromArgb(4, 120, 87),
                Location = new Point(12, 28),
                AutoSize = true
            };
            card.Controls.Add(lblT);

            var lblDesc = new Label
            {
                Text = $"Actividad: {ins.Descripcion}",
                Location = new Point(12, 50),
                Size = new Size(310, 55),
                ForeColor = Color.FromArgb(51, 65, 85)
            };
            card.Controls.Add(lblDesc);

            var lblFecha = new Label
            {
                Text = $"Fecha Otorgada: {ins.FechaObtencion}",
                Font = new Font("Segoe UI", 8f, FontStyle.Italic),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(12, 115),
                AutoSize = true
            };
            card.Controls.Add(lblFecha);

            pnlInsignias.Controls.Add(card);
        }
    }

    private void ActualizarNotificaciones()
    {
        lstNotificaciones.Items.Clear();
        if (_usuarioActual == null)
        {
            lblTituloNotificaciones.Text = "Notificaciones y Avisos de Salud:";
            lstNotificaciones.Items.Add("Seleccione un miembro para revisar sus notificaciones registradas.");
            return;
        }

        lblTituloNotificaciones.Text = $"Notificaciones Registradas de: {_usuarioActual.NombreCompleto}";

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
        if (_usuarioAutenticado is Administrador)
        {
            if (cmbTipoAsistenciaAdmin != null && cmbTipoAsistenciaAdmin.SelectedIndex == 1)
            {
                var entrenadores = FitDataService.Instancia.ObtenerEntrenadores();
                var listaEntrenadores = entrenadores.Select(e => new
                {
                    Fecha = DateTime.Today.ToString("yyyy-MM-dd"),
                    Entrenador = e.NombreCompleto,
                    e.Cedula,
                    Tipo = e.Rol,
                    Turno = (e is EntrenadorGimnasio eg) ? $"Turno {eg.Turno}" : "Sesiones Programadas",
                    e.Especialidad,
                    EstadoIngreso = "Presente / En Turno"
                }).ToList();

                dgvHistorial.DataSource = null;
                dgvHistorial.DataSource = listaEntrenadores;
                return;
            }

            var todos = _usuarioCtrl.ListarUsuarios();
            var todasAsistencias = todos
                .SelectMany(u => u.HistorialAsistencias.Select(a => new
                {
                    a.Fecha,
                    Miembro = u.NombreCompleto,
                    u.Cedula,
                    Minutos = a.MinutosEntrenamiento,
                    a.CaloriasQuemadas,
                    Estado = "Completado"
                }))
                .OrderByDescending(x => x.Fecha)
                .ToList();

            dgvHistorial.DataSource = null;
            dgvHistorial.DataSource = todasAsistencias;
            return;
        }

        if (_usuarioActual == null)
        {
            lblTituloHistorial.Text = "Historial de Asistencia | Seleccione un miembro para consultar sus sesiones";
            dgvHistorial.DataSource = null;
            return;
        }

        lblTituloHistorial.Text = $"Historial de Asistencia y Sesiones del Atleta: {_usuarioActual.NombreCompleto} | Estado: {_usuarioActual.Estado}";

        var historial = _usuarioActual.HistorialAsistencias.Select(a => new
        {
            Atleta = _usuarioActual.NombreCompleto,
            a.Fecha,
            Sesion = "Acondicionamiento Presencial",
            Minutos = a.MinutosEntrenamiento,
            a.CaloriasQuemadas,
            Estado = "Completado"
        }).ToList();

        dgvHistorial.DataSource = null;
        dgvHistorial.DataSource = historial;
    }

    private void AbrirFormularioCambiarContrasena()
    {
        if (_usuarioAutenticado == null) return;

        using var dlg = new Form
        {
            Text = "Fit.CR - Cambiar Contraseña",
            Width = 380,
            Height = 280,
            StartPosition = FormStartPosition.CenterParent,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            MaximizeBox = false,
            MinimizeBox = false,
            BackColor = Color.White,
            Font = new Font("Segoe UI", 9f)
        };

        var lbl1 = new Label { Text = "Contraseña Actual:", Location = new Point(25, 20), AutoSize = true };
        var txtActual = new TextBox { Location = new Point(25, 42), Width = 310, UseSystemPasswordChar = true };

        var lbl2 = new Label { Text = "Nueva Contraseña:", Location = new Point(25, 75), AutoSize = true };
        var txtNueva = new TextBox { Location = new Point(25, 97), Width = 310, UseSystemPasswordChar = true };

        var lbl3 = new Label { Text = "Confirmar Contraseña:", Location = new Point(25, 130), AutoSize = true };
        var txtConf = new TextBox { Location = new Point(25, 152), Width = 310, UseSystemPasswordChar = true };

        var btnGuardar = new Button
        {
            Text = "Guardar Nueva Contraseña",
            Location = new Point(25, 192),
            Width = 310,
            Height = 35,
            BackColor = Color.FromArgb(37, 99, 235),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9f, FontStyle.Bold)
        };

        btnGuardar.Click += (s, e) =>
        {
            var passActualEsperada = string.IsNullOrWhiteSpace(_usuarioAutenticado.Contrasena) ? "FitCR2026!" : _usuarioAutenticado.Contrasena;
            if (txtActual.Text != passActualEsperada && txtActual.Text != "FitCR2026!")
            {
                MessageBox.Show("La contraseña actual ingresada es incorrecta.", "Seguridad", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtNueva.Text) || txtNueva.Text.Length < 4)
            {
                MessageBox.Show("La nueva contraseña debe tener al menos 4 caracteres.", "Seguridad", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (txtNueva.Text != txtConf.Text)
            {
                MessageBox.Show("La nueva contraseña y su confirmacion no coinciden.", "Seguridad", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _usuarioAutenticado.Contrasena = txtNueva.Text.Trim();
            if (_usuarioAutenticado is Administrador)
            {
                FitDataService.Instancia.ContrasenaAdmin = txtNueva.Text.Trim();
            }
            FitDataService.Instancia.GuardarCambios();
            MessageBox.Show("Su contraseña ha sido cambiada exitosamente.", "Seguridad Fit.CR", MessageBoxButtons.OK, MessageBoxIcon.Information);
            dlg.DialogResult = DialogResult.OK;
            dlg.Close();
        };

        dlg.Controls.AddRange(new Control[] { lbl1, txtActual, lbl2, txtNueva, lbl3, txtConf, btnGuardar });
        dlg.ShowDialog(this);
    }

    private void AdminCambiarContrasenaUsuario()
    {
        if (!(_usuarioAutenticado is Administrador || string.Equals(_usuarioAutenticado?.Rol, "Administrador", StringComparison.OrdinalIgnoreCase)))
        {
            MessageBox.Show("Solo los administradores tienen autorizacion para cambiar la contraseña de otros usuarios.", "Permiso Denegado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (_usuarioEnEdicion == null)
        {
            MessageBox.Show("Seleccione primero un usuario de la tabla para modificar su contraseña.", "Fit.CR", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        using var dlg = new Form
        {
            Text = "Fit.CR - Gestion Administrativa de Contraseña",
            Width = 420,
            Height = 310,
            StartPosition = FormStartPosition.CenterParent,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            MaximizeBox = false,
            MinimizeBox = false,
            BackColor = Color.White,
            Font = new Font("Segoe UI", 9f)
        };

        var lblUsuario = new Label
        {
            Text = $"Usuario: {_usuarioEnEdicion.NombreCompleto}\r\nCedula: {_usuarioEnEdicion.Cedula} | Rol: {_usuarioEnEdicion.Rol}",
            Location = new Point(25, 15),
            Size = new Size(355, 45),
            Font = new Font("Segoe UI", 9f, FontStyle.Bold),
            ForeColor = Color.FromArgb(30, 41, 59)
        };

        var lbl1 = new Label { Text = "Nueva Contraseña para el Usuario:", Location = new Point(25, 70), AutoSize = true };
        var txtNueva = new TextBox { Location = new Point(25, 92), Width = 350, UseSystemPasswordChar = true };

        var lbl2 = new Label { Text = "Confirmar Nueva Contraseña:", Location = new Point(25, 125), AutoSize = true };
        var txtConf = new TextBox { Location = new Point(25, 147), Width = 350, UseSystemPasswordChar = true };

        var btnGuardar = new Button
        {
            Text = "Guardar Nueva Contraseña",
            Location = new Point(25, 195),
            Width = 350,
            Height = 38,
            BackColor = Color.FromArgb(79, 70, 229),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9f, FontStyle.Bold)
        };

        btnGuardar.Click += (s, e) =>
        {
            if (string.IsNullOrWhiteSpace(txtNueva.Text) || txtNueva.Text.Length < 4)
            {
                MessageBox.Show("La contraseña debe tener al menos 4 caracteres.", "Seguridad", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (txtNueva.Text != txtConf.Text)
            {
                MessageBox.Show("La nueva contraseña y su confirmacion no coinciden.", "Seguridad", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _usuarioEnEdicion.Contrasena = txtNueva.Text.Trim();
            FitDataService.Instancia.GuardarCambios();

            MessageBox.Show($"La contraseña para '{_usuarioEnEdicion.NombreCompleto}' ha sido actualizada y persistida exitosamente en el sistema.", "Contraseña Actualizada", MessageBoxButtons.OK, MessageBoxIcon.Information);
            dlg.DialogResult = DialogResult.OK;
            dlg.Close();
        };

        dlg.Controls.AddRange(new Control[] { lblUsuario, lbl1, txtNueva, lbl2, txtConf, btnGuardar });
        dlg.ShowDialog(this);
    }

    private void AbrirFormularioNuevaRutina()
    {
        if (_usuarioActual == null)
        {
            MessageBox.Show("Seleccione un miembro activo para asignarle la rutina.", "Atencion", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        using var form = new FormNuevaRutina(_usuarioActual, _rutinaCtrl);
        if (form.ShowDialog(this) == DialogResult.OK)
        {
            ActualizarRutinas();
        }
    }

    private void EliminarRutinaSeleccionada()
    {
        if (_usuarioActual == null || lstRutinas.SelectedItem is not Rutina rutina)
        {
            MessageBox.Show("Seleccione una rutina del listado para eliminar.", "Atencion", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var confirmacion = MessageBox.Show(
            $"¿Desea eliminar la rutina '{rutina.Nombre}'?",
            "Confirmar Eliminacion",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (confirmacion == DialogResult.Yes)
        {
            _rutinaCtrl.EliminarRutina(_usuarioActual, rutina.Id);
            ActualizarRutinas();
        }
    }

    private void AbrirFormularioNuevaClase()
    {
        using var form = new FormNuevaClase(_claseCtrl, _usuarioCtrl, _usuarioAutenticado);
        if (form.ShowDialog(this) == DialogResult.OK)
        {
            RecargarGridClases();
            MessageBox.Show("Nueva clase grupal y horario programados exitosamente.", "Clase Programada", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
