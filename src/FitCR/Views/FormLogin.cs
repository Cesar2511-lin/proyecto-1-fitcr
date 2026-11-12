using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using FitCR.Controllers;
using FitCR.Models;

namespace FitCR.Views;

/// <summary>
/// Formulario de autenticacion y control de acceso por roles.
/// Permite el ingreso de miembros, entrenadores y administradores,
/// asi como el acceso directo a la ventana de registro de nuevos usuarios.
/// </summary>
public class FormLogin : Form
{
    private readonly UsuarioController _usuarioCtrl = new();

    private TextBox txtCredencial = null!;
    private ComboBox cmbPerfilesDemo = null!;
    private Label lblMensajeError = null!;

    public FormLogin()
    {
        InicializarComponentes();
        CargarPerfilesDemostracion();
    }

    private void InicializarComponentes()
    {
        Text = "Fit.CR - Control de Acceso y Autenticacion";
        Width = 460;
        Height = 560;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;
        Font = new Font("Segoe UI", 9.5f, FontStyle.Regular);
        BackColor = Color.FromArgb(248, 250, 252);

        // Header Superior Institucional
        var pnlHeader = new Panel
        {
            Dock = DockStyle.Top,
            Height = 85,
            BackColor = Color.FromArgb(24, 43, 73),
            Padding = new Padding(25, 14, 25, 10)
        };

        var lblTitulo = new Label
        {
            Text = "Fit.CR | Plataforma de Gimnasio",
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 13.5f, FontStyle.Bold),
            AutoSize = true,
            Location = new Point(22, 14)
        };

        var lblSubtitulo = new Label
        {
            Text = "Sistema de Acondicionamiento Fisico y Gestion de Membresias",
            ForeColor = Color.FromArgb(180, 200, 230),
            Font = new Font("Segoe UI", 8.5f),
            AutoSize = true,
            Location = new Point(23, 44)
        };

        pnlHeader.Controls.AddRange(new Control[] { lblTitulo, lblSubtitulo });
        Controls.Add(pnlHeader);

        // Contenedor Central
        var pnlCard = new Panel
        {
            Location = new Point(28, 105),
            Size = new Size(390, 395),
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            Padding = new Padding(22)
        };

        var lblSeccion = new Label
        {
            Text = "Iniciar Sesion",
            Font = new Font("Segoe UI", 12f, FontStyle.Bold),
            ForeColor = Color.FromArgb(15, 23, 42),
            Location = new Point(18, 16),
            AutoSize = true
        };
        pnlCard.Controls.Add(lblSeccion);

        var lblInstruccion = new Label
        {
            Text = "Ingrese su cedula o correo registrado para acceder a su perfil:",
            Font = new Font("Segoe UI", 8.5f),
            ForeColor = Color.FromArgb(100, 116, 139),
            Location = new Point(18, 44),
            Size = new Size(350, 32)
        };
        pnlCard.Controls.Add(lblInstruccion);

        var lblCred = new Label
        {
            Text = "Identificacion o Correo:",
            Font = new Font("Segoe UI", 9f, FontStyle.Bold),
            Location = new Point(18, 85),
            AutoSize = true
        };
        txtCredencial = new TextBox
        {
            Location = new Point(18, 108),
            Width = 350,
            Font = new Font("Segoe UI", 10f)
        };
        txtCredencial.KeyDown += (s, e) =>
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                IniciarSesion();
            }
        };
        pnlCard.Controls.AddRange(new Control[] { lblCred, txtCredencial });

        // Boton Iniciar Sesion
        var btnLogin = new Button
        {
            Text = "Ingresar al Sistema",
            Location = new Point(18, 150),
            Width = 350,
            Height = 40,
            BackColor = Color.FromArgb(37, 99, 235),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9.5f, FontStyle.Bold)
        };
        btnLogin.Click += (s, e) => IniciarSesion();
        pnlCard.Controls.Add(btnLogin);

        // Separador Visual
        var pnlSep = new Panel
        {
            Location = new Point(18, 206),
            Size = new Size(350, 1),
            BackColor = Color.FromArgb(226, 232, 240)
        };
        pnlCard.Controls.Add(pnlSep);

        // Selector Rapido de Perfiles por Rol (Facilita la revision docente)
        var lblDemo = new Label
        {
            Text = "Acceso rapido por rol (evaluacion):",
            Font = new Font("Segoe UI", 8.5f, FontStyle.Regular),
            ForeColor = Color.FromArgb(100, 116, 139),
            Location = new Point(18, 218),
            AutoSize = true
        };
        cmbPerfilesDemo = new ComboBox
        {
            Location = new Point(18, 240),
            Width = 350,
            DropDownStyle = ComboBoxStyle.DropDownList,
            Font = new Font("Segoe UI", 9f)
        };
        cmbPerfilesDemo.SelectedIndexChanged += (s, e) => SeleccionarPerfilDemo();
        pnlCard.Controls.AddRange(new Control[] { lblDemo, cmbPerfilesDemo });

        // Mensaje de Error
        lblMensajeError = new Label
        {
            Text = "",
            ForeColor = Color.FromArgb(220, 38, 38),
            Font = new Font("Segoe UI", 8.5f),
            Location = new Point(18, 275),
            Size = new Size(350, 35)
        };
        pnlCard.Controls.Add(lblMensajeError);

        // Boton Registro Nuevo Miembro
        var btnRegistro = new Button
        {
            Text = "Registrarse como Nuevo Miembro",
            Location = new Point(18, 320),
            Width = 350,
            Height = 38,
            BackColor = Color.FromArgb(241, 245, 249),
            ForeColor = Color.FromArgb(30, 41, 59),
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9f, FontStyle.Bold)
        };
        btnRegistro.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
        btnRegistro.Click += (s, e) => AbrirFormularioRegistro();
        pnlCard.Controls.Add(btnRegistro);

        Controls.Add(pnlCard);
    }

    private void CargarPerfilesDemostracion()
    {
        cmbPerfilesDemo.Items.Clear();
        cmbPerfilesDemo.Items.Add("-- Seleccionar perfil para prueba --");

        // Perfil Administrador Global
        cmbPerfilesDemo.Items.Add(new ItemPerfilDemo("[Administrador] Administracion Fit.CR", "admin@fitcr.com", "Administrador"));

        // Perfiles de Entrenadores
        var entrenadores = _usuarioCtrl.ListarEntrenadores();
        foreach (var ent in entrenadores)
        {
            var etiquetaRol = ent is EntrenadorPersonal ? "Entrenador Personal" : "Entrenador de Gimnasio";
            cmbPerfilesDemo.Items.Add(new ItemPerfilDemo($"[{etiquetaRol}] {ent.NombreCompleto}", ent.Correo, ent.Rol));
        }

        // Perfiles de Miembros
        var usuarios = _usuarioCtrl.ListarUsuarios();
        foreach (var u in usuarios.Take(6))
        {
            cmbPerfilesDemo.Items.Add(new ItemPerfilDemo($"[Miembro] {u.NombreCompleto} ({u.Cedula})", u.Cedula, "UsuarioBasico"));
        }

        cmbPerfilesDemo.SelectedIndex = 0;
    }

    private void SeleccionarPerfilDemo()
    {
        if (cmbPerfilesDemo.SelectedItem is ItemPerfilDemo item)
        {
            txtCredencial.Text = item.Identificacion;
            lblMensajeError.Text = "";
        }
    }

    private void IniciarSesion()
    {
        lblMensajeError.Text = "";
        var entrada = txtCredencial.Text.Trim();

        if (string.IsNullOrWhiteSpace(entrada))
        {
            lblMensajeError.Text = "Por favor ingrese su cedula o correo electronico.";
            return;
        }

        // 1. Validar si es el Administrador General
        if (entrada.Equals("admin@fitcr.com", StringComparison.OrdinalIgnoreCase) ||
            entrada.Equals("admin", StringComparison.OrdinalIgnoreCase) ||
            entrada.Equals("1-0000-0000", StringComparison.OrdinalIgnoreCase))
        {
            var admin = new Administrador
            {
                Id = "ADM-01",
                Nombre = "Administracion",
                Apellidos = "General Fit.CR",
                Cedula = "1-0000-0000",
                Correo = "admin@fitcr.com",
                Rol = "Administrador"
            };
            AbrirSistemaPrincipal(admin);
            return;
        }

        // 2. Validar si es un Entrenador registrado
        var entrenador = _usuarioCtrl.ListarEntrenadores().Find(e =>
            string.Equals(e.Cedula, entrada, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(e.Correo, entrada, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(e.Nombre, entrada, StringComparison.OrdinalIgnoreCase));

        if (entrenador != null)
        {
            AbrirSistemaPrincipal(entrenador);
            return;
        }

        // 3. Validar si es un Miembro / Usuario Basico registrado
        var usuario = _usuarioCtrl.ListarUsuarios().Find(u =>
            string.Equals(u.Cedula, entrada, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(u.Correo, entrada, StringComparison.OrdinalIgnoreCase));

        if (usuario != null)
        {
            AbrirSistemaPrincipal(usuario);
            return;
        }

        lblMensajeError.Text = "No se encontro ningun usuario o entrenador con la identificacion ingresada.";
    }

    private void AbrirSistemaPrincipal(Persona usuarioAutenticado)
    {
        Hide();
        using var principal = new FormPrincipal(usuarioAutenticado);
        principal.ShowDialog();
        
        // Al regresar del sistema principal, limpiar credenciales y refrescar lista
        txtCredencial.Clear();
        CargarPerfilesDemostracion();
        Show();
    }

    private void AbrirFormularioRegistro()
    {
        using var reg = new FormRegistro();
        if (reg.ShowDialog() == DialogResult.OK && reg.UsuarioRegistrado != null)
        {
            txtCredencial.Text = reg.UsuarioRegistrado.Cedula;
            CargarPerfilesDemostracion();
            lblMensajeError.ForeColor = Color.FromArgb(16, 185, 129);
            lblMensajeError.Text = "Registro completado. Presione 'Ingresar' para continuar.";
        }
    }

    private sealed class ItemPerfilDemo
    {
        public string Etiqueta { get; }
        public string Identificacion { get; }
        public string Rol { get; }

        public ItemPerfilDemo(string etiqueta, string identificacion, string rol)
        {
            Etiqueta = etiqueta;
            Identificacion = identificacion;
            Rol = rol;
        }

        public override string ToString() => Etiqueta;
    }
}
