using System.Windows.Forms;

namespace MoverObjeto;

partial class SeleccionarJugador
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null!;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }

        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        this.lblTitulo = new System.Windows.Forms.Label();
        this.rbBlindado = new System.Windows.Forms.RadioButton();
        this.rbEquilibrado = new System.Windows.Forms.RadioButton();
        this.rbLigero = new System.Windows.Forms.RadioButton();
        this.btnIniciar = new System.Windows.Forms.Button();
        this.cmbEscenario = new System.Windows.Forms.ComboBox();
        this.lblEscenario = new System.Windows.Forms.Label();
        this.SuspendLayout();
        // 
        // lblTitulo
        // 
        this.lblTitulo.AutoSize = true;
        this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
        this.lblTitulo.Location = new System.Drawing.Point(22, 18);
        this.lblTitulo.Name = "lblTitulo";
        this.lblTitulo.Size = new System.Drawing.Size(202, 21);
        this.lblTitulo.TabIndex = 0;
        this.lblTitulo.Text = "Selecciona tu avión inicial";
        // 
        // rbBlindado
        // 
        this.rbBlindado.AutoSize = true;
        this.rbBlindado.Checked = true;
        this.rbBlindado.Location = new System.Drawing.Point(26, 58);
        this.rbBlindado.Name = "rbBlindado";
        this.rbBlindado.Size = new System.Drawing.Size(181, 19);
        this.rbBlindado.TabIndex = 1;
        this.rbBlindado.TabStop = true;
        this.rbBlindado.Text = "Blindado (vida 100 - robusto)";
        this.rbBlindado.UseVisualStyleBackColor = true;
        // 
        // rbEquilibrado
        // 
        this.rbEquilibrado.AutoSize = true;
        this.rbEquilibrado.Location = new System.Drawing.Point(26, 83);
        this.rbEquilibrado.Name = "rbEquilibrado";
        this.rbEquilibrado.Size = new System.Drawing.Size(189, 19);
        this.rbEquilibrado.TabIndex = 2;
        this.rbEquilibrado.Text = "Equilibrado (vida 70 - balance)";
        this.rbEquilibrado.UseVisualStyleBackColor = true;
        // 
        // rbLigero
        // 
        this.rbLigero.AutoSize = true;
        this.rbLigero.Location = new System.Drawing.Point(26, 108);
        this.rbLigero.Name = "rbLigero";
        this.rbLigero.Size = new System.Drawing.Size(165, 19);
        this.rbLigero.TabIndex = 3;
        this.rbLigero.Text = "Ligero (vida 50 - ágil)";
        this.rbLigero.UseVisualStyleBackColor = true;
        // 
        // btnIniciar
        // 
        this.btnIniciar.Location = new System.Drawing.Point(26, 187);
        this.btnIniciar.Name = "btnIniciar";
        this.btnIniciar.Size = new System.Drawing.Size(217, 29);
        this.btnIniciar.TabIndex = 5;
        this.btnIniciar.Text = "Iniciar partida";
        this.btnIniciar.UseVisualStyleBackColor = true;
        this.btnIniciar.Click += new System.EventHandler(this.btnIniciar_Click);
        // 
        // cmbEscenario
        // 
        this.cmbEscenario.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.cmbEscenario.FormattingEnabled = true;
        this.cmbEscenario.Items.AddRange(new object[] {
            "Escenario 1",
            "Escenario 2",
            "Escenario 3",
            "Escenario 4"});
        this.cmbEscenario.Location = new System.Drawing.Point(26, 153);
        this.cmbEscenario.Name = "cmbEscenario";
        this.cmbEscenario.Size = new System.Drawing.Size(217, 23);
        this.cmbEscenario.TabIndex = 4;
        this.cmbEscenario.SelectedIndex = 0;
        // 
        // lblEscenario
        // 
        this.lblEscenario.AutoSize = true;
        this.lblEscenario.Location = new System.Drawing.Point(26, 135);
        this.lblEscenario.Name = "lblEscenario";
        this.lblEscenario.Size = new System.Drawing.Size(125, 15);
        this.lblEscenario.TabIndex = 6;
        this.lblEscenario.Text = "Escenario de comienzo";
        // 
        // SeleccionarJugador
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(276, 233);
        this.Controls.Add(this.lblEscenario);
        this.Controls.Add(this.cmbEscenario);
        this.Controls.Add(this.btnIniciar);
        this.Controls.Add(this.rbLigero);
        this.Controls.Add(this.rbEquilibrado);
        this.Controls.Add(this.rbBlindado);
        this.Controls.Add(this.lblTitulo);
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.Name = "SeleccionarJugador";
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        this.Text = "Seleccionar jugador";
        this.ResumeLayout(false);
        this.PerformLayout();

    }

    #endregion

    private Label lblTitulo;
    private RadioButton rbBlindado;
    private RadioButton rbEquilibrado;
    private RadioButton rbLigero;
    private Button btnIniciar;
    private ComboBox cmbEscenario;
    private Label lblEscenario;
}
