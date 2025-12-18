using System;
using System.Windows.Forms;

namespace MoverObjeto;

public partial class SeleccionarJugador : Form
{
    public SeleccionarJugador()
    {
        InitializeComponent();
    }

    private void btnIniciar_Click(object sender, EventArgs e)
    {
        AvionConfig configSeleccionado = ObtenerSeleccion();
        int escenario = cmbEscenario.SelectedIndex >= 0 ? cmbEscenario.SelectedIndex : 0;

        Form1 juego = new(configSeleccionado, escenario);
        juego.FormClosed += (_, _) => Close();
        Hide();
        juego.Show();
    }

    private AvionConfig ObtenerSeleccion()
    {
        if (rbEquilibrado.Checked)
        {
            return AvionFactory.Equilibrado;
        }

        if (rbLigero.Checked)
        {
            return AvionFactory.Ligero;
        }

        return AvionFactory.Blindado;
    }
}
