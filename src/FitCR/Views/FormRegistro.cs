using System;
using System.Drawing;
using System.Windows.Forms;
using FitCR.Controllers;
using FitCR.Models;

namespace FitCR.Views;

/// <summary>
/// Ventana modal para el registro formal de nuevos miembros en el sistema.
/// Permite capturar datos personales, calcular el IMC proyectado y asignar entrenador.
/// </summary>
public class FormRegistro : Form
{
    private readonly UsuarioController _usuarioCtrl = new();
    public UsuarioBasico? UsuarioRegistrado { get; private set; }

    private TextBox txtNombre = null!;
    private TextBox txtApellidos = null!;
    private TextBox txtCedula = null!;
    private TextBox txtCorreo = null!;
    private NumericUpDown numEdad = null!;
    private NumericUpDown numPeso = null!;
    private NumericUpDown numAltura = null!;
    private ComboBox cmbEntrenador = null!;
    private Label lblImcCalculado = null!;

    public FormRegistro()
    {
        InicializarComponentes();
    }

    private void InicializarComponentes()
    {
        Text = "Fit.CR - Registro de Nuevo Miembro";
        Width = 500;
        Height = 670;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;
        Font = new Font("Segoe UI", 9.5f, FontStyle.Regular);
        BackColor = Color.FromArgb(248, 250, 252);

        // Header Superior
        var pnlHeader = new Panel
        {
            Dock = DockStyle.Top,
            Height = 75,
            BackColor = Color.FromArgb(24, 43, 73),
            Padding = new Padding(20, 12, 20, 10)
        };

        var lblTitulo = new Label
        {
            Text = "Registro de Nuevo Miembro",
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 13f, FontStyle.Bold),
            AutoSize = true,
            Location = new Point(20, 12)
        };

        var lblSubtitulo = new Label
        {
            Text = "Complete el formulario para crear su cuenta y calcular su estado fisico",
            ForeColor = Color.FromArgb(180, 200, 230),
            Font = new Font("Segoe UI", 8.5f),
            AutoSize = true,
            Location = new Point(21, 42)
        };

        pnlHeader.Controls.AddRange(new Control[] { lblTitulo, lblSubtitulo });
        Controls.Add(pnlHeader);

        // Contenedor del Formulario
        var pnlBody = new Panel
        {
            Location = new Point(25, 95),
            Size = new Size(435, 520),
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            Padding = new Padding(20)
        };

        int y = 15;
        txtNombre = AgregarCampo(pnlBody, "Nombre:", ref y);
        txtApellidos = AgregarCampo(pnlBody, "Apellidos:", ref y);
        txtCedula = AgregarCampo(pnlBody, "Cedula:", ref y);
        txtCorreo = AgregarCampo(pnlBody, "Correo electronico:", ref y);

        // Edad
        var lblE = new Label { Text = "Edad:", Location = new Point(15, y + 3), AutoSize = true };
        numEdad = new NumericUpDown
        {
            Location = new Point(160, y),
            Width = 120,
            Minimum = 12,
            Maximum = 100,
            Value = 25
        };
        pnlBody.Controls.AddRange(new Control[] { lblE, numEdad });
        y += 38;

        // Peso
        var lblP = new Label { Text = "Peso inicial (kg):", Location = new Point(15, y + 3), AutoSize = true };
        numPeso = new NumericUpDown
        {
            Location = new Point(160, y),
            Width = 120,
            Minimum = 30,
            Maximum = 300,
            DecimalPlaces = 1,
            Value = 70.0m
        };
        numPeso.ValueChanged += (s, e) => RecalcularImc();
        pnlBody.Controls.AddRange(new Control[] { lblP, numPeso });
        y += 38;

        // Altura
        var lblA = new Label { Text = "Estatura (m):", Location = new Point(15, y + 3), AutoSize = true };
        numAltura = new NumericUpDown
        {
            Location = new Point(160, y),
            Width = 120,
            Minimum = 1.00m,
            Maximum = 2.50m,
            DecimalPlaces = 2,
            Increment = 0.01m,
            Value = 1.70m
        };
        numAltura.ValueChanged += (s, e) => RecalcularImc();
        pnlBody.Controls.AddRange(new Control[] { lblA, numAltura });
        y += 38;

        // Entrenador
        var lblEnt = new Label { Text = "Entrenador:", Location = new Point(15, y + 3), AutoSize = true };
        cmbEntrenador = new ComboBox
        {
            Location = new Point(160, y),
            Width = 245,
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        CargarEntrenadores();
        pnlBody.Controls.AddRange(new Control[] { lblEnt, cmbEntrenador });
        y += 45;

        // Resultado IMC Proyectado
        lblImcCalculado = new Label
        {
            Text = "IMC Inicial: 24.2 kg/m2 (Normopeso)",
            Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
            ForeColor = Color.FromArgb(37, 99, 235),
            Location = new Point(15, y),
            AutoSize = true
        };
        pnlBody.Controls.Add(lblImcCalculado);
        y += 45;

        // Boton Guardar
        var btnRegistrar = new Button
        {
            Text = "Completar Registro",
            Location = new Point(15, y),
            Width = 220,
            Height = 40,
            BackColor = Color.FromArgb(37, 99, 235),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9.5f, FontStyle.Bold)
        };
        btnRegistrar.Click += (s, e) => RegistrarUsuario();

        // Boton Cancelar
        var btnCancelar = new Button
        {
            Text = "Cancelar",
            Location = new Point(245, y),
            Width = 160,
            Height = 40,
            BackColor = Color.FromArgb(100, 116, 139),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };
        btnCancelar.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

        pnlBody.Controls.AddRange(new Control[] { btnRegistrar, btnCancelar });
        Controls.Add(pnlBody);

        RecalcularImc();
    }

    private TextBox AgregarCampo(Panel parent, string etiqueta, ref int y)
    {
        var lbl = new Label { Text = etiqueta, Location = new Point(15, y + 3), AutoSize = true };
        var txt = new TextBox { Location = new Point(160, y), Width = 245 };
        parent.Controls.AddRange(new Control[] { lbl, txt });
        y += 38;
        return txt;
    }

    private void CargarEntrenadores()
    {
        var entrenadores = _usuarioCtrl.ListarEntrenadores();
        cmbEntrenador.Items.Add("Sin asignar (Planta de Gimnasio)");
        foreach (var ent in entrenadores)
        {
            cmbEntrenador.Items.Add(ent);
        }
        cmbEntrenador.SelectedIndex = 0;
    }

    private void RecalcularImc()
    {
        var peso = (double)numPeso.Value;
        var altura = (double)numAltura.Value;
        if (altura > 0)
        {
            var imc = Math.Round(peso / (altura * altura), 1);
            var estado = imc switch
            {
                < 18.5 => "Bajo peso",
                < 25.0 => "Normopeso",
                < 30.0 => "Sobrepeso",
                _ => "Obesidad"
            };
            lblImcCalculado.Text = $"IMC Inicial: {imc:F1} kg/m2 ({estado})";
        }
    }

    private void RegistrarUsuario()
    {
        var nuevo = new UsuarioBasico
        {
            Id = $"USR-{DateTime.Now.Ticks % 100000}",
            Nombre = txtNombre.Text.Trim(),
            Apellidos = txtApellidos.Text.Trim(),
            Cedula = txtCedula.Text.Trim(),
            Correo = txtCorreo.Text.Trim(),
            Edad = (int)numEdad.Value,
            PesoKg = (double)numPeso.Value,
            AlturaM = (double)numAltura.Value,
            Estado = "Activo"
        };

        if (cmbEntrenador.SelectedItem is Entrenador e)
        {
            nuevo.EntrenadorAsignadoId = e.Id;
        }

        // Insignia inicial de bienvenida
        nuevo.Insignias.Add(new Insignia
        {
            Id = "INS-01",
            Nombre = "Bienvenida a Fit.CR",
            Descripcion = "Registro formal completado en el gimnasio.",
            Tipo = "Logro",
            FechaObtencion = DateTime.Now.ToString("yyyy-MM-dd")
        });

        // Notificacion inicial
        nuevo.Notificaciones.Add(new Notificacion
        {
            Id = $"NOTIF-{nuevo.Id}-1",
            Titulo = "Bienvenido a Fit.CR",
            Mensaje = "Tu membresia ha sido activada exitosamente.",
            Fecha = DateTime.Now.ToString("yyyy-MM-dd HH:mm"),
            Leida = false
        });

        // Rutinas basicas sugeridas por defecto
        nuevo.Rutinas.Add(new Rutina
        {
            Id = $"RUT-{nuevo.Id}-1",
            Nombre = "Rutina de Adaptacion - Pierna y Core",
            DiaSemana = "Lunes",
            DuracionMinutos = 50,
            Ejercicios = new()
            {
                new() { Nombre = "Sentadilla con peso corporal", Series = 4, Repeticiones = 12, PesoKg = 0, DescansoSeg = 60, CaloriasEstimadas = 35 },
                new() { Nombre = "Prensa inclinada", Series = 3, Repeticiones = 10, PesoKg = 40, DescansoSeg = 90, CaloriasEstimadas = 40 }
            }
        });

        var (esValido, mensaje) = _usuarioCtrl.ValidarYGuardarUsuario(nuevo);
        if (!esValido)
        {
            MessageBox.Show(mensaje, "Validacion de Registro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        UsuarioRegistrado = nuevo;
        MessageBox.Show($"Bienvenido a Fit.CR, {nuevo.NombreCompleto}.\nSu cuenta ha sido registrada con exito y su membresia se encuentra activa.", "Registro Completado", MessageBoxButtons.OK, MessageBoxIcon.Information);
        DialogResult = DialogResult.OK;
        Close();
    }
}
