using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace MoverObjeto
{
    public static class Escenarios
    {
        public static int TotalEscenarios => 4;

        public static Bitmap CrearEscenario(int numeroEscenario, Size tamano)
        {
            // Determinar el índice del escenario (1 a 4)
            int indice = (numeroEscenario % TotalEscenarios) + 1;
            
            // Intentar cargar la imagen para TODOS los escenarios
            string nombreArchivo = $"escenario{indice}.jpg";
            string ruta = Path.Combine(Application.StartupPath, nombreArchivo);

            // Búsqueda en directorio del proyecto si no está en bin/Debug
            if (!File.Exists(ruta))
            {
                try 
                {
                    string rutaProyecto = Path.GetFullPath(Path.Combine(Application.StartupPath, @"..\..\.."));
                    string rutaEnProyecto = Path.Combine(rutaProyecto, nombreArchivo);
                    if (File.Exists(rutaEnProyecto))
                    {
                        ruta = rutaEnProyecto;
                    }
                }
                catch { /* Ignorar errores de ruta */ }
            }

            if (File.Exists(ruta))
            {
                try
                {
                    using (var img = Image.FromFile(ruta))
                    {
                        // Crear un bitmap de alta calidad
                        Bitmap fondoAltaCalidad = new Bitmap(tamano.Width, tamano.Height);
                        using (Graphics g = Graphics.FromImage(fondoAltaCalidad))
                        {
                            // Configuración para máxima calidad de escalado
                            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            g.SmoothingMode = SmoothingMode.HighQuality;
                            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                            g.CompositingQuality = CompositingQuality.HighQuality;

                            g.DrawImage(img, new Rectangle(0, 0, tamano.Width, tamano.Height));
                        }
                        return fondoAltaCalidad;
                    }
                }
                catch 
                { 
                    // Si falla la carga, continuará y generará el fondo por código
                }
            }

            return GenerarFondoPorCodigo(numeroEscenario, tamano);
        }

        private static Bitmap GenerarFondoPorCodigo(int numeroEscenario, Size tamano)
        {
            Bitmap fondo = new Bitmap(tamano.Width, tamano.Height);
            using (Graphics g = Graphics.FromImage(fondo))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;

                switch (numeroEscenario % TotalEscenarios)
                {
                    case 0:
                        DibujarEspacioProfundoConNebulosa(g, tamano);
                        break;
                    case 1:
                        DibujarPlanetaConAnillos(g, tamano);
                        break;
                    case 2:
                        DibujarCampoDeAsteroidesDinamico(g, tamano);
                        break;
                    default:
                        DibujarEstacionEspacialFuturista(g, tamano);
                        break;
                }
            }
            return fondo;
        }

        private static void DibujarEspacioProfundoConNebulosa(Graphics g, Size tamano)
        {
            g.FillRectangle(new SolidBrush(Color.FromArgb(5, 0, 15)), 0, 0, tamano.Width, tamano.Height);
            
            using (var path = new GraphicsPath())
            {
                path.AddEllipse(new Rectangle(-tamano.Width / 4, tamano.Height / 4, tamano.Width, tamano.Height));
                using (var brush = new PathGradientBrush(path))
                {
                    brush.CenterColor = Color.FromArgb(80, 150, 100, 255);
                    brush.SurroundColors = new[] { Color.FromArgb(0, 5, 0, 15) };
                    g.FillPath(brush, path);
                }
            }
            DibujarEstrellas(g, tamano, 250, true);
        }

        private static void DibujarPlanetaConAnillos(Graphics g, Size tamano)
        {
            g.FillRectangle(new SolidBrush(Color.FromArgb(10, 5, 20)), 0, 0, tamano.Width, tamano.Height);
            DibujarEstrellas(g, tamano, 100, false);

            int planetaTamano = tamano.Width / 3;
            int planetaX = tamano.Width - planetaTamano - 50;
            int planetaY = 50;

            // Anillos
            g.DrawEllipse(new Pen(Color.FromArgb(100, 210, 210, 200), 15), planetaX - 20, planetaY + planetaTamano / 2 - 10, planetaTamano + 40, 40);
            
            // Planeta
            using (var brush = new LinearGradientBrush(new Point(planetaX, planetaY), new Point(planetaX + planetaTamano, planetaY + planetaTamano), Color.SteelBlue, Color.DarkSlateBlue))
            {
                g.FillEllipse(brush, planetaX, planetaY, planetaTamano, planetaTamano);
            }
        }

        private static void DibujarCampoDeAsteroidesDinamico(Graphics g, Size tamano)
        {
            g.FillRectangle(new SolidBrush(Color.FromArgb(15, 15, 15)), 0, 0, tamano.Width, tamano.Height);
            Random rand = new Random();
            for (int i = 0; i < 40; i++)
            {
                int size = rand.Next(10, 120);
                int x = rand.Next(-40, tamano.Width);
                int y = rand.Next(-40, tamano.Height);
                int grayTone = rand.Next(40, 120);
                g.FillEllipse(new SolidBrush(Color.FromArgb(rand.Next(100, 200), grayTone, grayTone, grayTone)), x, y, size, size);
            }
            DibujarEstrellas(g, tamano, 70, true);
        }

        private static void DibujarEstacionEspacialFuturista(Graphics g, Size tamano)
        {
            g.FillRectangle(new SolidBrush(Color.FromArgb(5, 5, 15)), 0, 0, tamano.Width, tamano.Height);
            
            Point centro = new Point(tamano.Width / 3, tamano.Height / 2);
            int radio = 120;
            
            // Estructura principal
            g.FillEllipse(Brushes.DimGray, centro.X - radio, centro.Y - radio, radio * 2, radio * 2);
            g.DrawEllipse(Pens.LightGray, centro.X - radio, centro.Y - radio, radio * 2, radio * 2);

            // Luces
            for (int i = 0; i < 360; i += 30)
            {
                double angulo = i * Math.PI / 180;
                int x = centro.X + (int)(radio * Math.Cos(angulo));
                int y = centro.Y + (int)(radio * Math.Sin(angulo));
                g.FillEllipse(Brushes.Aqua, x - 5, y - 5, 10, 10);
            }
            DibujarEstrellas(g, tamano, 120, false);
        }

        private static void DibujarEstrellas(Graphics g, Size tamano, int cantidad, bool conBrillo)
        {
            Random random = new Random();
            for (int i = 0; i < cantidad; i++)
            {
                int x = random.Next(tamano.Width);
                int y = random.Next(tamano.Height);
                int size = random.Next(1, 4);
                g.FillEllipse(Brushes.White, x, y, size, size);

                if (conBrillo && random.Next(0, 10) == 0)
                {
                    var pen = new Pen(Color.FromArgb(150, 255, 255, 255), 1);
                    g.DrawLine(pen, x + size / 2, y - size, x + size / 2, y + size * 2);
                    g.DrawLine(pen, x - size, y + size / 2, x + size * 2, y + size / 2);
                    pen.Dispose();
                }
            }
        }
    }
}
