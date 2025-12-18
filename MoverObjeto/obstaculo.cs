using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace MoverObjeto
{
    public class Obstaculo
    {
        public PictureBox Roca { get; private set; }
        private Timer timerMovimiento;
        private int velocidadCaidaX;
        private int velocidadCaidaY;

        public Obstaculo(Control contenedor)
        {
            Random rand = new Random();
            
            int tamano = rand.Next(40, 80);
            Roca = new PictureBox
            {
                Size = new Size(tamano, tamano),
                BackColor = Color.Transparent,
                Tag = this 
            };

            Roca.Location = new Point(rand.Next(0, contenedor.ClientSize.Width - Roca.Width), -Roca.Height);
            
            velocidadCaidaX = rand.Next(-3, 4); 
            velocidadCaidaY = rand.Next(3, 7); 

            // Crear una forma irregular para el PictureBox
            GraphicsPath path = new GraphicsPath();
            path.AddEllipse(0, 0, tamano, tamano);
            Roca.Region = new Region(path);

            Roca.Image = CrearImagenAsteroide(tamano);
            
            contenedor.Controls.Add(Roca);
            Roca.BringToFront();

            timerMovimiento = new Timer { Interval = 20 };
            timerMovimiento.Tick += (sender, e) => Mover(contenedor);
            timerMovimiento.Start();
        }

        private Bitmap CrearImagenAsteroide(int tamano)
        {
            Bitmap bmp = new Bitmap(tamano, tamano);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;

                // La roca
                Rectangle rocaRect = new Rectangle(0, 0, tamano, tamano);
                using (var brush = new LinearGradientBrush(rocaRect, Color.SaddleBrown, Color.SandyBrown, LinearGradientMode.ForwardDiagonal))
                {
                    g.FillEllipse(brush, rocaRect);
                }

                // Cráteres
                Random rand = new Random();
                for (int i = 0; i < 5; i++)
                {
                    int craterTamano = rand.Next(tamano / 10, tamano / 5);
                    int x = rocaRect.X + rand.Next(0, rocaRect.Width - craterTamano);
                    int y = rocaRect.Y + rand.Next(0, rocaRect.Height - craterTamano);
                    g.FillEllipse(new SolidBrush(Color.FromArgb(150, 80, 40, 0)), x, y, craterTamano, craterTamano);
                }
                
                // Borde de calor
                using (Pen penCalor = new Pen(Color.FromArgb(180, 255, 100, 0), 3))
                {
                    g.DrawEllipse(penCalor, rocaRect);
                }
            }
            return bmp;
        }

        private void Mover(Control contenedor)
        {
            Roca.Left += velocidadCaidaX;
            Roca.Top += velocidadCaidaY;
            
            if (Roca.Top > contenedor.ClientSize.Height || Roca.Left < -Roca.Width || Roca.Left > contenedor.ClientSize.Width)
            {
                timerMovimiento.Stop();
                Roca.Dispose();
            }
        }

        public static void GenerarObstaculo(Control contenedor)
        {
            new Obstaculo(contenedor);
        }
    }
}
