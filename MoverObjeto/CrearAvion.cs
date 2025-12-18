using System;
using System.Collections.Generic;
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
        
        Point[] puntosBase;
        
        if (tipox == 1)
        {
            // Diseño detallado Avion 1 (32x32 base)
            puntosBase = new[]
            {
                new Point(16, 0),   // Punta
                new Point(20, 4),
                new Point(30, 12),  // Ala derecha punta frente
                new Point(30, 20),  // Ala derecha punta tras
                new Point(20, 24),  // Fuselaje tras
                new Point(24, 32),  // Cola derecha
                new Point(8, 32),   // Cola izquierda
                new Point(12, 24),  // Fuselaje tras
                new Point(2, 20),   // Ala izquierda punta tras
                new Point(2, 12),   // Ala izquierda punta frente
                new Point(12, 4),
                new Point(16, 0)
            };
        }
        else
        {
            puntosBase = tipox switch
            {
                2 => new[] // Avión ágil y rápido
                {
                    new Point(25, 2), new Point(27, 12), new Point(45, 18), new Point(48, 28),
                    new Point(42, 38), new Point(28, 42), new Point(22, 42), new Point(8, 38),
                    new Point(2, 28), new Point(5, 18), new Point(23, 12), new Point(25, 2)
                },
                3 => new[] // Bombardero pesado
                {
                    new Point(25, 2), new Point(28, 8), new Point(52, 12), new Point(55, 22),
                    new Point(50, 35), new Point(30, 45), new Point(20, 45), new Point(0, 35),
                    new Point(5, 22), new Point(8, 12), new Point(22, 8), new Point(25, 2)
                },
                _ => throw new ArgumentOutOfRangeException(nameof(tipox), "Tipo de avión desconocido")
            };
        }

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
        
        if (tipox == 1)
        {
            // Dibujado detallado para Avion 1
            using (Graphics g = Graphics.FromImage(imagen))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                int s = factorEscala;

                // Dibujar componentes (Coordenadas base 0 grados)
                
                // Alas (Naranja con borde Rojo y línea Amarilla)
                Point[] alaIzq = { new Point(13*s, 10*s), new Point(2*s, 12*s), new Point(2*s, 20*s), new Point(13*s, 22*s) };
                Point[] alaDer = { new Point(19*s, 10*s), new Point(30*s, 12*s), new Point(30*s, 20*s), new Point(19*s, 22*s) };
                
                g.FillPolygon(Brushes.Orange, alaIzq);
                g.DrawPolygon(Pens.Red, alaIzq);
                g.DrawLine(Pens.Yellow, alaIzq[0], alaIzq[1]); // Borde de ataque

                g.FillPolygon(Brushes.Orange, alaDer);
                g.DrawPolygon(Pens.Red, alaDer);
                g.DrawLine(Pens.Yellow, alaDer[0], alaDer[1]); // Borde de ataque

                // Motores/Misiles (Azul Cian)
                Brush brushMotor = Brushes.Cyan;
                Pen penMotor = Pens.Black;
                
                // Izquierda
                g.FillRectangle(brushMotor, 5*s, 11*s, 2*s, 6*s);
                g.DrawRectangle(penMotor, 5*s, 11*s, 2*s, 6*s);
                g.FillRectangle(brushMotor, 9*s, 10*s, 2*s, 6*s);
                g.DrawRectangle(penMotor, 9*s, 10*s, 2*s, 6*s);
                
                // Derecha
                g.FillRectangle(brushMotor, 25*s, 11*s, 2*s, 6*s);
                g.DrawRectangle(penMotor, 25*s, 11*s, 2*s, 6*s);
                g.FillRectangle(brushMotor, 21*s, 10*s, 2*s, 6*s);
                g.DrawRectangle(penMotor, 21*s, 10*s, 2*s, 6*s);

                // Fuselaje Inferior (Transición Gris a Rojo)
                Rectangle rectCola = new Rectangle(13*s, 20*s, 6*s, 6*s);
                using (LinearGradientBrush brCola = new LinearGradientBrush(rectCola, Color.Gray, Color.Red, 90f))
                {
                    g.FillRectangle(brCola, rectCola);
                }
                g.DrawRectangle(Pens.Black, rectCola);

                // Fuselaje Superior (Cilindro Gris)
                Rectangle rectFuselaje = new Rectangle(13*s, 6*s, 6*s, 14*s);
                using (LinearGradientBrush brFus = new LinearGradientBrush(rectFuselaje, Color.LightGray, Color.DimGray, 0f))
                {
                    g.FillRectangle(brFus, rectFuselaje);
                }
                g.DrawRectangle(Pens.Black, rectFuselaje);

                // Cola y Timón (Rojo y Naranja)
                Point[] cola = { new Point(13*s, 26*s), new Point(19*s, 26*s), new Point(24*s, 32*s), new Point(8*s, 32*s) };
                g.FillPolygon(Brushes.Red, cola);
                g.DrawPolygon(Pens.Black, cola);
                g.DrawLine(new Pen(Color.Orange, 2), 16*s, 26*s, 16*s, 32*s); // Timón

                // Cabina (Azul Cian)
                g.FillRectangle(Brushes.Cyan, 14*s, 4*s, 4*s, 2*s);
                g.DrawRectangle(Pens.Black, 14*s, 4*s, 4*s, 2*s);

                // Punta/Morro (Amarillo)
                Point[] morro = { new Point(13*s, 4*s), new Point(19*s, 4*s), new Point(16*s, 0*s) };
                g.FillPolygon(Brushes.Gold, morro);
                g.DrawPolygon(Pens.Black, morro);
            }

            if (angRotar == 180)
            {
                imagen.RotateFlip(RotateFlipType.Rotate180FlipNone);
            }
        }
        else
        {
            // Renderizado genérico para otros tipos
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
        }

        return new AvionRender(puntosRotados, tamano, new Region(path), imagen);
    }
}

public record AvionRender(Point[] Puntos, Size Tamano, Region Region, Bitmap Imagen);