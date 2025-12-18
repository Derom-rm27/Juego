using System.Drawing;
using System.Drawing.Drawing2D;

namespace MoverObjeto;

public static class Escenarios
{
    public static int TotalEscenarios => 4;

    public static Bitmap CrearEscenario(int numeroEscenario, Size tamano)
    {
        Bitmap fondo = new(tamano.Width, tamano.Height);
        using Graphics g = Graphics.FromImage(fondo);
        g.SmoothingMode = SmoothingMode.AntiAlias;

        switch (numeroEscenario % TotalEscenarios)
        {
            case 0:
                DibujarCielo(g, tamano, Color.LightSkyBlue, Color.SteelBlue);
                break;
            case 1:
                DibujarCielo(g, tamano, Color.DarkSlateGray, Color.Black);
                g.FillEllipse(new SolidBrush(Color.LightYellow), tamano.Width - 120, 30, 80, 80);
                break;
            case 2:
                DibujarCielo(g, tamano, Color.DarkOliveGreen, Color.ForestGreen);
                g.FillRectangle(new SolidBrush(Color.SandyBrown), 0, tamano.Height - 60, tamano.Width, 60);
                break;
            default:
                DibujarCielo(g, tamano, Color.MidnightBlue, Color.Indigo);
                DibujarEstrellas(g, tamano, 45);
                break;
        }

        return fondo;
    }

    private static void DibujarCielo(Graphics g, Size tamano, Color colorInicio, Color colorFin)
    {
        using LinearGradientBrush brush = new(new Point(0, 0), new Point(0, tamano.Height), colorInicio, colorFin);
        g.FillRectangle(brush, 0, 0, tamano.Width, tamano.Height);
    }

    private static void DibujarEstrellas(Graphics g, Size tamano, int cantidad)
    {
        Random random = new(7);
        for (int i = 0; i < cantidad; i++)
        {
            int x = random.Next(tamano.Width);
            int y = random.Next(tamano.Height / 2);
            g.FillEllipse(Brushes.WhiteSmoke, x, y, 3, 3);
        }
    }
}
