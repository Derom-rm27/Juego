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
        const int factorEscala = 2;
        Point[] puntosBase = tipox switch
        {
            1 => new[]
            {
                new Point(18, 0), new Point(32, 18), new Point(40, 32), new Point(48, 36),
                new Point(58, 52), new Point(46, 76), new Point(36, 94), new Point(24, 110),
                new Point(14, 94), new Point(4, 76), new Point(0, 52), new Point(10, 36),
                new Point(18, 32), new Point(24, 18), new Point(18, 0)
            },
            2 => new[]
            {
                new Point(16, 0), new Point(30, 10), new Point(46, 22), new Point(54, 36),
                new Point(60, 52), new Point(52, 70), new Point(34, 88), new Point(18, 70),
                new Point(10, 52), new Point(8, 36), new Point(12, 22), new Point(16, 10), new Point(16, 0)
            },
            3 => new[]
            {
                new Point(12, 0), new Point(24, 12), new Point(38, 20), new Point(48, 34),
                new Point(44, 52), new Point(34, 66), new Point(24, 80), new Point(12, 66),
                new Point(6, 48), new Point(6, 28), new Point(10, 14), new Point(12, 0)
            },
            _ => throw new ArgumentOutOfRangeException(nameof(tipox), "Tipo de avión desconocido")
        };

        Point[] puntosEscalados = puntosBase
            .Select(p => new Point(p.X * factorEscala, p.Y * factorEscala))
            .ToArray();

        int ancho = puntosEscalados.Max(p => p.X);
        int largo = puntosEscalados.Max(p => p.Y);

        Point[] puntosRotados = new Point[puntosEscalados.Length];
        for (int i = 0; i < puntosEscalados.Length; i++)
        {
            puntosRotados[i].X = puntosEscalados[i].X;
            puntosRotados[i].Y = angRotar == 180 ? largo - puntosEscalados[i].Y : puntosEscalados[i].Y;
        }

        GraphicsPath path = new();
        path.AddPolygon(puntosRotados);
        Size tamano = new(ancho + 2, largo + 2);

        Bitmap imagen = new(tamano.Width, tamano.Height);
        using (Graphics g = Graphics.FromImage(imagen))
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            Color brillo = Color.FromArgb(
                Math.Min(255, color.R + 40),
                Math.Min(255, color.G + 40),
                Math.Min(255, color.B + 40));

            using var pincel = new LinearGradientBrush(new Rectangle(0, 0, tamano.Width, tamano.Height), color, brillo, LinearGradientMode.ForwardDiagonal);
            using var lapiz = new Pen(Color.Black, 2);

            g.FillPolygon(pincel, puntosRotados);
            g.DrawPolygon(lapiz, puntosRotados);

            Rectangle cabina = new Rectangle((tamano.Width / 2) - 8, 6 * factorEscala, 16, 22);
            g.FillEllipse(new SolidBrush(Color.FromArgb(200, Color.LightSkyBlue)), cabina);
            g.DrawEllipse(Pens.DarkBlue, cabina);

            Rectangle decoracionCola = new Rectangle((int)(tamano.Width * 0.35), (int)(tamano.Height * 0.55), (int)(tamano.Width * 0.3), (int)(tamano.Height * 0.25));
            g.FillRectangle(new SolidBrush(Color.FromArgb(120, brillo)), decoracionCola);
            g.DrawRectangle(Pens.Black, decoracionCola);
        }

        return new AvionRender(puntosRotados, tamano, new Region(path), imagen);
    }
}

public record AvionRender(Point[] Puntos, Size Tamano, Region Region, Bitmap Imagen);
