using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace MoverObjeto;

public record AvionConfig(string Nombre, int Tipo, int Vida, Color Color);

public static class AvionFactory
{
    public static AvionConfig Blindado => new("Blindado", 1, 100, Color.SeaGreen);
    public static AvionConfig Equilibrado => new("Equilibrado", 2, 70, Color.SteelBlue);
    public static AvionConfig Ligero => new("Ligero", 3, 50, Color.Goldenrod);
    public static IReadOnlyList<AvionConfig> Disponibles => new[] { Blindado, Equilibrado, Ligero };

    public static AvionRender Crear(int tipox, int angRotar, Color color)
    {
        Point[] puntosBase = tipox switch
        {
            1 => new[]
            {
                new Point(29, 0), new Point(39, 28), new Point(50, 51), new Point(35, 74),
                new Point(23, 74), new Point(0, 59), new Point(19, 28), new Point(29, 0)
            },
            2 => new[]
            {
                new Point(22, 0), new Point(32, 10), new Point(42, 22), new Point(38, 40),
                new Point(28, 52), new Point(12, 52), new Point(4, 36), new Point(10, 18), new Point(22, 0)
            },
            3 => new[]
            {
                new Point(16, 0), new Point(30, 12), new Point(38, 26), new Point(32, 46),
                new Point(18, 54), new Point(4, 44), new Point(0, 26), new Point(8, 10), new Point(16, 0)
            },
            _ => throw new ArgumentOutOfRangeException(nameof(tipox), "Tipo de avión desconocido")
        };

        int ancho = puntosBase.Max(p => p.X);
        int largo = puntosBase.Max(p => p.Y);

        Point[] puntosRotados = new Point[puntosBase.Length];
        for (int i = 0; i < puntosBase.Length; i++)
        {
            puntosRotados[i].X = puntosBase[i].X;
            puntosRotados[i].Y = angRotar == 180 ? largo - puntosBase[i].Y : puntosBase[i].Y;
        }

        GraphicsPath path = new();
        path.AddPolygon(puntosRotados);
        Size tamano = new(ancho + 2, largo + 2);

        Bitmap imagen = new(tamano.Width, tamano.Height);
        using (Graphics g = Graphics.FromImage(imagen))
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.FillPolygon(new SolidBrush(color), puntosRotados);
            g.DrawPolygon(Pens.Black, puntosRotados);
        }

        return new AvionRender(puntosRotados, tamano, new Region(path), imagen);
    }
}

public record AvionRender(Point[] Puntos, Size Tamano, Region Region, Bitmap Imagen);
