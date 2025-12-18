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
        else if (tipox == 2)
        {
            // Diseño detallado Avion 2 (Gris Metálico)
            puntosBase = new[]
            {
                new Point(16, 0),   // Punta
                new Point(20, 6),
                new Point(30, 14),  // Ala derecha frente
                new Point(28, 24),  // Ala derecha tras
                new Point(20, 28),  // Fuselaje tras
                new Point(24, 32),  // Cola derecha
                new Point(8, 32),   // Cola izquierda
                new Point(12, 28),  // Fuselaje tras
                new Point(4, 24),   // Ala izquierda tras
                new Point(2, 14),   // Ala izquierda frente
                new Point(12, 6),
                new Point(16, 0)
            };
        }
        else if (tipox == 3)
        {
            // Diseño detallado Avion 3 (Bombardero Pesado / Espacial)
            // Base más ancha y robusta
            puntosBase = new[]
            {
                new Point(16, 0),   // Punta central
                new Point(22, 6),
                new Point(28, 12),  // Ala derecha inicio
                new Point(32, 20),  // Ala derecha borde
                new Point(28, 28),  // Ala derecha fin
                new Point(22, 32),  // Cola derecha
                new Point(10, 32),  // Cola izquierda
                new Point(4, 28),   // Ala izquierda fin
                new Point(0, 20),   // Ala izquierda borde
                new Point(4, 12),   // Ala izquierda inicio
                new Point(10, 6),
                new Point(16, 0)
            };
        }
        else
        {
            // Fallback
            puntosBase = new[] { new Point(0,0), new Point(10,10), new Point(0,10) };
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
                
                g.FillRectangle(brushMotor, 5*s, 11*s, 2*s, 6*s);
                g.DrawRectangle(penMotor, 5*s, 11*s, 2*s, 6*s);
                g.FillRectangle(brushMotor, 9*s, 10*s, 2*s, 6*s);
                g.DrawRectangle(penMotor, 9*s, 10*s, 2*s, 6*s);
                
                g.FillRectangle(brushMotor, 25*s, 11*s, 2*s, 6*s);
                g.DrawRectangle(penMotor, 25*s, 11*s, 2*s, 6*s);
                g.FillRectangle(brushMotor, 21*s, 10*s, 2*s, 6*s);
                g.DrawRectangle(penMotor, 21*s, 10*s, 2*s, 6*s);

                // Fuselaje
                Rectangle rectCola = new Rectangle(13*s, 20*s, 6*s, 6*s);
                using (LinearGradientBrush brCola = new LinearGradientBrush(rectCola, Color.Gray, Color.Red, 90f))
                {
                    g.FillRectangle(brCola, rectCola);
                }
                g.DrawRectangle(Pens.Black, rectCola);

                Rectangle rectFuselaje = new Rectangle(13*s, 6*s, 6*s, 14*s);
                using (LinearGradientBrush brFus = new LinearGradientBrush(rectFuselaje, Color.LightGray, Color.DimGray, 0f))
                {
                    g.FillRectangle(brFus, rectFuselaje);
                }
                g.DrawRectangle(Pens.Black, rectFuselaje);

                // Cola
                Point[] cola = { new Point(13*s, 26*s), new Point(19*s, 26*s), new Point(24*s, 32*s), new Point(8*s, 32*s) };
                g.FillPolygon(Brushes.Red, cola);
                g.DrawPolygon(Pens.Black, cola);
                g.DrawLine(new Pen(Color.Orange, 2), 16*s, 26*s, 16*s, 32*s);

                // Cabina
                g.FillRectangle(Brushes.Cyan, 14*s, 4*s, 4*s, 2*s);
                g.DrawRectangle(Pens.Black, 14*s, 4*s, 4*s, 2*s);

                // Punta
                Point[] morro = { new Point(13*s, 4*s), new Point(19*s, 4*s), new Point(16*s, 0*s) };
                g.FillPolygon(Brushes.Gold, morro);
                g.DrawPolygon(Pens.Black, morro);
            }
        }
        else if (tipox == 2)
        {
            // Dibujado detallado para Avion 2 (Gris Metálico)
            using (Graphics g = Graphics.FromImage(imagen))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                int s = factorEscala;

                Color grisClaro = Color.LightGray;
                Color grisOscuro = Color.Gray;
                Color azulAcero = Color.SteelBlue;
                Color amarilloPalido = Color.LightGoldenrodYellow;

                Point[] alaIzq = { new Point(12*s, 6*s), new Point(2*s, 14*s), new Point(4*s, 24*s), new Point(12*s, 22*s) };
                Point[] alaDer = { new Point(20*s, 6*s), new Point(30*s, 14*s), new Point(28*s, 24*s), new Point(20*s, 22*s) };

                using (LinearGradientBrush brAla = new LinearGradientBrush(new Point(0, 0), new Point(32*s, 0), grisClaro, grisOscuro))
                {
                    g.FillPolygon(brAla, alaIzq);
                    g.FillPolygon(brAla, alaDer);
                }
                g.DrawPolygon(Pens.Black, alaIzq);
                g.DrawPolygon(Pens.Black, alaDer);

                Rectangle rectFuselaje = new Rectangle(12*s, 6*s, 8*s, 22*s);
                using (LinearGradientBrush brFus = new LinearGradientBrush(rectFuselaje, Color.WhiteSmoke, grisOscuro, 0f))
                {
                    g.FillRectangle(brFus, rectFuselaje);
                }
                g.DrawRectangle(Pens.Black, rectFuselaje);
                
                for (int y = 8*s; y < 26*s; y += 4*s)
                {
                    g.DrawLine(Pens.DimGray, 12*s, y, 20*s, y);
                }

                Point[] colaIzq = { new Point(12*s, 26*s), new Point(8*s, 32*s), new Point(12*s, 32*s) };
                Point[] colaDer = { new Point(20*s, 26*s), new Point(24*s, 32*s), new Point(20*s, 32*s) };
                
                g.FillPolygon(Brushes.Gray, colaIzq);
                g.FillPolygon(Brushes.Gray, colaDer);
                g.DrawPolygon(Pens.Black, colaIzq);
                g.DrawPolygon(Pens.Black, colaDer);

                Point[] estabilizadorIzq = { new Point(13*s, 24*s), new Point(13*s, 30*s), new Point(15*s, 30*s) };
                Point[] estabilizadorDer = { new Point(19*s, 24*s), new Point(19*s, 30*s), new Point(17*s, 30*s) };
                
                g.FillPolygon(new SolidBrush(azulAcero), estabilizadorIzq);
                g.FillPolygon(new SolidBrush(azulAcero), estabilizadorDer);

                Rectangle rectCabina = new Rectangle(14*s, 8*s, 4*s, 6*s);
                g.FillRectangle(new SolidBrush(amarilloPalido), rectCabina);
                g.DrawRectangle(Pens.Black, rectCabina);
                g.DrawLine(Pens.Black, 14*s, 10*s, 18*s, 10*s);
                g.DrawLine(Pens.Black, 14*s, 12*s, 18*s, 12*s);

                Point[] morro = { new Point(12*s, 6*s), new Point(20*s, 6*s), new Point(16*s, 0*s) };
                g.FillPolygon(Brushes.Silver, morro);
                g.DrawPolygon(Pens.Black, morro);
            }
        }
        else if (tipox == 3)
        {
            // Dibujado detallado para Avion 3 (Estilo Espacial/Bombardero)
            using (Graphics g = Graphics.FromImage(imagen))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                int s = factorEscala;

                // Colores
                Color baseOscura = Color.FromArgb(40, 40, 50);
                Color baseClara = Color.FromArgb(80, 80, 100);
                Color energia = Color.OrangeRed;
                Color luces = Color.Cyan;

                // Alas principales (Grandes y angulares)
                Point[] alaIzq = { new Point(12*s, 8*s), new Point(0*s, 20*s), new Point(4*s, 28*s), new Point(12*s, 24*s) };
                Point[] alaDer = { new Point(20*s, 8*s), new Point(32*s, 20*s), new Point(28*s, 28*s), new Point(20*s, 24*s) };

                using (LinearGradientBrush brAla = new LinearGradientBrush(new Point(0, 0), new Point(32*s, 32*s), baseClara, baseOscura))
                {
                    g.FillPolygon(brAla, alaIzq);
                    g.FillPolygon(brAla, alaDer);
                }
                g.DrawPolygon(Pens.Black, alaIzq);
                g.DrawPolygon(Pens.Black, alaDer);

                // Detalles de energía en las alas
                g.FillRectangle(new SolidBrush(energia), 2*s, 22*s, 2*s, 4*s);
                g.FillRectangle(new SolidBrush(energia), 28*s, 22*s, 2*s, 4*s);

                // Fuselaje Central (Robusto)
                Point[] fuselaje = { 
                    new Point(14*s, 0*s), new Point(18*s, 0*s), // Punta
                    new Point(20*s, 6*s), new Point(20*s, 28*s), // Lado derecho
                    new Point(18*s, 32*s), new Point(14*s, 32*s), // Cola
                    new Point(12*s, 28*s), new Point(12*s, 6*s)  // Lado izquierdo
                };
                
                using (LinearGradientBrush brFus = new LinearGradientBrush(new Point(16*s, 0), new Point(16*s, 32*s), Color.LightSlateGray, baseOscura))
                {
                    g.FillPolygon(brFus, fuselaje);
                }
                g.DrawPolygon(Pens.Black, fuselaje);

                // Cabina (Grande y brillante)
                Point[] cabina = { new Point(14*s, 8*s), new Point(18*s, 8*s), new Point(18*s, 14*s), new Point(14*s, 14*s) };
                g.FillPolygon(Brushes.Gold, cabina);
                g.DrawPolygon(Pens.Black, cabina);

                // Motores traseros
                g.FillRectangle(Brushes.DarkGray, 13*s, 28*s, 2*s, 4*s);
                g.FillRectangle(Brushes.DarkGray, 17*s, 28*s, 2*s, 4*s);
                
                // Fuego de motores (Estático para el diseño)
                g.FillEllipse(Brushes.Orange, 13*s, 32*s, 2*s, 2*s);
                g.FillEllipse(Brushes.Orange, 17*s, 32*s, 2*s, 2*s);

                // Luces de navegación
                g.FillEllipse(new SolidBrush(luces), 0*s, 20*s, 2*s, 2*s);
                g.FillEllipse(new SolidBrush(luces), 30*s, 20*s, 2*s, 2*s);
            }
        }

        if (tipox == 1 || tipox == 2 || tipox == 3)
        {
            if (angRotar == 180)
            {
                imagen.RotateFlip(RotateFlipType.Rotate180FlipNone);
            }
        }

        return new AvionRender(puntosRotados, tamano, new Region(path), imagen);
    }
}

public record AvionRender(Point[] Puntos, Size Tamano, Region Region, Bitmap Imagen);