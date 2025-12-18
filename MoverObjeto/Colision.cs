using System.Drawing;
using System.Windows.Forms;

namespace MoverObjeto;

public static class Colision
{
    public static bool EntreNaves(PictureBox jugador, PictureBox enemigo)
    {
        return jugador.Visible && enemigo.Visible && jugador.Bounds.IntersectsWith(enemigo.Bounds);
    }

    public static bool ImpactaObjeto(Rectangle objetivo, Rectangle atacante, int padding)
    {
        return objetivo.Left < atacante.Left && atacante.Right < objetivo.Right &&
               objetivo.Top < atacante.Top && atacante.Bottom < objetivo.Bottom &&
               objetivo.Left + padding < atacante.Left && atacante.Right < objetivo.Right - padding;
    }
}
