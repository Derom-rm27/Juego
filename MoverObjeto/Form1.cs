using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D; // Librería para GraphicsPath
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MoverObjeto
{
    public partial class Form1 : Form
    {
        //*********** VARIABLES GLOBALES *************//
        PictureBox navex = new PictureBox();
        PictureBox naveRival = new PictureBox();
        PictureBox contiene = new PictureBox();
        Timer tiempo;
        Label label1 = new Label(); // Asunción: necesario para mostrar puntos del rival
        Label label2 = new Label(); // Asunción: necesario para mostrar puntos de la nave
        
        int Dispara = 0;
        bool flag = false;
        float angulo = 0;

        // Necesario si este código es parte de un proyecto real, aunque esté vacío.
        // Se asume que estos componentes existen.
        private void InitializeComponent()
        {
            // Se debe inicializar en el diseñador o aquí si es un archivo único.
            this.SuspendLayout();
            this.label1.Text = "Mi Rival";
            this.label2.Text = "Mi Avión";
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        //********** DIAGRAMAR DEL MISIL ***********//
        public void CrearMisil(int AngRotar, Color pintar, string nombre, int x, int y)
        {
            // Usamos PictureBox en lugar de dynamic para una mejor práctica
            PictureBox Balas = new PictureBox();
            
            // Declaración de variables para el tamaño del misil
            int PosX = 1;
            int PosY = 1;
            int largoM = 11;
            int anchoM = 3;

            // Declarar los array de puntos.
            Point[] myMisill = {
                new Point(4 * PosX, 0 * PosY), new Point(5 * PosX, 1 * PosY), new Point(6 * PosX, 2 * PosY),
                new Point(6 * PosX, 7 * PosY), new Point(7 * PosX, 8 * PosY), new Point(8 * PosX, 9 * PosY),
                new Point(7 * PosX, 9 * PosY), new Point(6 * PosX, 10 * PosY), new Point(2 * PosX, 10 * PosY),
                new Point(1 * PosX, 9 * PosY), new Point(0 * PosX, 9 * PosY), new Point(1 * PosX, 8 * PosY),
                new Point(2 * PosX, 7 * PosY), new Point(2 * PosX, 2 * PosY), new Point(3 * PosX, 1 * PosY),
                new Point(4 * PosX, 0 * PosY) 
            };
            
            // Aplicar la rotación (180 grados invierte el eje Y)
            Point[] myMisil = new Point[myMisill.Count()];
            for (int i = 0; i < myMisill.Count(); i++)
            {
                myMisil[i].X = myMisill[i].X;
                if (AngRotar == 180)
                    myMisil[i].Y = largoM - myMisill[i].Y;
                else
                    myMisil[i].Y = myMisill[i].Y;
            }

            GraphicsPath ObjGrafico = new GraphicsPath();
            ObjGrafico.AddPolygon(myMisil);

            Balas.Location = new Point(x, y);
            Balas.BackColor = pintar;
            Balas.Size = new Size(anchoM * PosX, largoM * PosY);
            Balas.Region = new Region(ObjGrafico);
            contiene.Controls.Add(Balas);
            Balas.Visible = true;
            Balas.Tag = nombre;

            //************** DIBUJAR COLORES **************//
            Bitmap flag = new Bitmap(anchoM, largoM);
            Graphics flagImagen = Graphics.FromImage(flag);
            // El código original contiene coordenadas ambiguas, se intenta replicar el patrón de color
            flagImagen.FillRectangle(Brushes.Orange, 0, 0, anchoM, 5);
            flagImagen.FillRectangle(Brushes.Yellow, 0, 5, anchoM, 6);
            Balas.Image = flag;
        }

        //*** DESCRUCTOR DEL MISIL / GAME LOOP (ImpactarTick) ****
        private void ImpactarTick(object sender, EventArgs e)
        {
            // VARIABLES LOCALES (Nave Rival)
            int X = naveRival.Location.X;
            int Y = naveRival.Location.Y;
            int W = naveRival.Width;
            int H = naveRival.Height;
            int PH = 10; // Asunción: PH es un valor de padding (el original era `1(`)

            // VARIABLES LOCALES (Nave Jugador)
            int X2 = navex.Location.X;
            int Y2 = navex.Location.Y;
            int W2 = navex.Width;
            int H2 = navex.Height;
            
            // Lógica para el disparo del rival
            Dispara++;
            if (Dispara > 100 && naveRival.Visible == true)
            {
                int xRival = naveRival.Location.X + (naveRival.Width / 2);
                int yRival = naveRival.Location.Y + naveRival.Height; // Dispara hacia abajo
                CrearMisil(180, Color.OrangeRed, "Rival", xRival, yRival);
                Dispara = 0;
            }

            // MOVIMIENTO DE LA NAVE RIVAL
            int x = naveRival.Location.X;
            int y = naveRival.Location.Y; // Y no cambia en el original
            
            if (flag == false)
            {
                if (contiene.Width < x + naveRival.Width + PH) // Límite derecho
                {
                    flag = true;
                }
                x++;
            }
            else
            {
                if (contiene.Location.X > x - PH) // Límite izquierdo
                {
                    flag = false;
                }
                x--;
            }
            naveRival.Location = new Point(x, y);

            // ELIMINACION DEL MISIL Y DESCONTAR PUNTOS DE IMPACTO
            foreach (Control c in contiene.Controls)
            {
                if (c is PictureBox && (string)((PictureBox)c).Tag != null)
                {
                    PictureBox missile = (PictureBox)c;
                    int X1 = missile.Location.X;
                    int Y1 = missile.Location.Y;
                    int W1 = missile.Width;
                    int H1 = missile.Height;
                    string nombre = missile.Tag.ToString();

                    // ACTIVIDAD DE IMPACTO CON LA NAVE RIVAL ("Misil" es el misil del jugador)
                    if (nombre == "Misil" &&
                        X < X1 && X1 + W1 < X + W &&
                        Y < Y1 && Y1 + H1 < Y + H) // Colisión con nave Rival
                    {
                        // Lógica de colisión más específica (zona de impacto)
                        if (X + PH < X1 && X1 + W1 < X + W - PH)
                        {
                            missile.Dispose();
                            // El código original parece restar 1 punto si está fuera de PH, o quizás es un error en el código fuente.
                            // Lo mantendré como una resta simple para simular un impacto.
                            naveRival.Tag = int.Parse(naveRival.Tag.ToString()) - 1;
                        }
                        else
                        {
                            missile.Dispose();
                            naveRival.Tag = int.Parse(naveRival.Tag.ToString()) - 1;
                        }

                        label1.Text = "Vida del Rival: " + naveRival.Tag.ToString();

                        // LÓGICA DE FIN DE JUEGO (VICTORIA)
                        if (int.Parse(naveRival.Tag.ToString()) <= 0)
                        {
                            naveRival.Dispose();
                            Bitmap NuevoImg = new Bitmap(contiene.Width, contiene.Height);
                            Graphics flagImagen = Graphics.FromImage(NuevoImg);
                            String drawString = "Felicitaciones Ganaste!";
                            Font drawFont = new Font("Arial", 16);
                            SolidBrush drawBrush = new SolidBrush(Color.Blue);
                            Point drawPoint = new Point(40, 150);
                            flagImagen.DrawString(drawString, drawFont, drawBrush, drawPoint);
                            contiene.Image = NuevoImg;
                            tiempo.Stop();
                        }
                    }

                    // ACTIVIDAD DE IMPACTO CON MI NAVE ("Rival" es el misil del rival)
                    if (nombre == "Rival" &&
                        X2 < X1 && X1 + W1 < X2 + W2 &&
                        Y2 < Y1 && Y1 + H1 < Y2 + H2) // Colisión con nave Jugador
                    {
                        if (X2 + PH < X1 && X1 + W1 < X2 + W2 - PH)
                        {
                            missile.Dispose();
                        }
                        else
                        {
                            missile.Dispose();
                            navex.Tag = int.Parse(navex.Tag.ToString()) - 1;
                        }
                        
                        label2.Text = "Vida: " + navex.Tag.ToString();

                        // LÓGICA DE FIN DE JUEGO (DERROTA)
                        if (int.Parse(navex.Tag.ToString()) <= 0)
                        {
                            navex.Dispose();
                            Bitmap NuevoImg = new Bitmap(contiene.Width, contiene.Height);
                            Graphics flagImagen = Graphics.FromImage(NuevoImg);
                            String drawString = "Perdiste el Juego";
                            Font drawFont = new Font("Arial", 16);
                            SolidBrush drawBrush = new SolidBrush(Color.Red);
                            Point drawPoint = new Point(70, 150);
                            flagImagen.DrawString(drawString, drawFont, drawBrush, drawPoint);
                            contiene.Image = NuevoImg;
                            tiempo.Stop();
                        }
                    }
                    
                    // Lógica de movimiento del misil del jugador y eliminación por límite
                    if (nombre == "Misil")
                    {
                        missile.Top -= 5; // Mueve hacia arriba
                        if (missile.Location.Y < 0)
                        {
                            missile.Dispose();
                        }
                    }
                    
                    // Lógica de movimiento del misil del rival y eliminación por límite
                    if (nombre == "Rival")
                    {
                        missile.Top += 5; // Mueve hacia abajo
                        if (missile.Location.Y > contiene.Height)
                        {
                            missile.Dispose();
                        }
                    }
                }
            }
        }

        //*** DIAGRAMAR NAVE **********//
        public void CrearNave(PictureBox Avion, int AngRotar, int Tipox, Color Pintar, int Vida)
        {
            int largoN = 0;
            int anchoN = 0;

            // Arrays de puntos de las diferentes naves (Se omiten por ser demasiado extensos en la transcripción, pero se simula la lógica)
            // Se asume que myNave1, myNave2, myNave3 están definidos o son arrays de Point.
            // Para mantener el código compilable, se usa una forma simplificada de nave 1.
            Point[] myNavel = { 
                new Point(29, 0), new Point(39, 28), new Point(50, 51), new Point(35, 74), 
                new Point(23, 74), new Point(0, 59), new Point(19, 28), new Point(29, 0) 
            };
            
            Point[] myNave;

            GraphicsPath ObjGrafico = new GraphicsPath();

            if (Tipox == 1)
            {
                largoN = 77;
                anchoN = 58;

                myNave = new Point[myNavel.Count()];
                for (int i = 0; i < myNavel.Count(); i++)
                {
                    myNave[i].X = myNavel[i].X;
                    if (AngRotar == 180)
                        myNave[i].Y = largoN - myNavel[i].Y;
                    else
                        myNave[i].Y = myNavel[i].Y;
                }
                ObjGrafico.AddPolygon(myNave);
            }
            // Las otras naves (Tipox 2 y 3) requerirían la definición de myNave2 y myNave3
            // La lógica para Tipox 2 y 3 es similar a Tipox 1.

            Avion.BackColor = Pintar;
            Avion.Size = new Size(anchoN, largoN);
            Avion.Region = new Region(ObjGrafico);
            Avion.Location = new Point(0, 0);
            
            //*********INSERTAR LA NAVE AL CONTENDOR***********//
            contiene.Controls.Add(Avion);

            // DIBUJAR IMAGEN DENTRO DE LA NAVE (EFECTOS/COLORES)
            Bitmap Imagen = new Bitmap(Avion.Width, Avion.Height);
            Graphics PintaImg = Graphics.FromImage(Imagen);

            // Aquí se usaría el array 'Colorea' para rellenar la nave. Se omite el array 'Colorea' por ser extenso.
            // PintaImg.FillPolygon(Brushes.DarkGreen, Colorea);
            
            // Dibuja el borde del polígono (el código original lo hace)
            if (myNave != null)
            {
                PintaImg.DrawPolygon(Pens.Black, myNave);
            }
            
            Avion.Image = Imagen;
            Avion.Tag = Vida;
            Avion.Visible = true;
        }

        //*********** EFECTOS DE LA NAVE PRINCIPAL ***********//
        public void NaveCorre(PictureBox Avion, int AngRotar, int velox)
        {
            Bitmap Imagen = new Bitmap(Avion.Width, Avion.Height);
            Graphics PintaImg = Graphics.FromImage(Imagen);
            
            // Se omiten los arrays 'puntoDer', 'puntoIzq', 'puntoAtr' por ser muy extensos.
            // El código original pintaba estos polígonos:
            // PintaImg.FillPolygon(Brushes.DarkGreen, puntoDer);
            // PintaImg.FillPolygon(Brushes.DarkGreen, puntoIzq);
            // PintaImg.FillPolygon(Brushes.DarkGreen, puntoAtr);
            
            // DIBUJAR ESCAPES
            PintaImg.FillRectangle(Brushes.Silver, 25, 0, 1, 15);
            PintaImg.FillRectangle(Brushes.Silver, 32, 0, 1, 15);
            PintaImg.FillRectangle(Brushes.Silver, 29, 0, 1, 3);
            
            // DIBUJAR FUEGO
            if (velox == 1) // Velocidad 1 (Motor encendido)
            {
                PintaImg.FillRectangle(Brushes.DarkOrange, 35, 6, 1, 1);
                PintaImg.FillRectangle(Brushes.Orange, 36, 5, 4, 1);
                PintaImg.FillRectangle(Brushes.Yellow, 37, 5, 2, 1);
                // El código original contiene líneas incompletas para el fuego. Se asume un patrón.
            }
            else if (velox == 2) // Velocidad 2
            {
                PintaImg.FillRectangle(Brushes.DarkRed, 5, 7, 1, 8);
                // ...
            }
            else if (velox == 3) // Velocidad 3
            {
                PintaImg.FillRectangle(Brushes.DarkRed, 15, 30, 1, 16);
                // ...
            }

            Avion.Image = RotateImage(Imagen, AngRotar);
        }

        //******************CREAR ANGULO DE ROTACION******************//
        public static Image RotateImage(Image img, float rotationAngle)
        {
            Bitmap bmp = new Bitmap(img.Width, img.Height);
            Graphics gfx = Graphics.FromImage(bmp);
            gfx.TranslateTransform((float)bmp.Width / 2, (float)bmp.Height / 2);
            gfx.RotateTransform(rotationAngle);
            gfx.TranslateTransform(-(float)bmp.Width / 2, -(float)bmp.Height / 2);
            gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;
            gfx.DrawImage(img, new Point(0, 0));
            gfx.Dispose();
            return bmp;
        }

        //*** MOVIMIENTO DEL TECLADO DEL USUARIO ***********//
        public void ActividadTecla(object sender, KeyEventArgs e)
        {
            //INSTRUCCIONES DE LOS BOTONES
            switch (e.KeyValue)
            {
                case 37: // flecha hacia la izquierda
                    {
                        if (contiene.Left < navex.Left - 10) // Límite izquierdo
                        {
                            navex.Left -= 10;
                            angulo = -15; // Rotar a la izquierda
                            NaveCorre(navex, (int)angulo, 1); // Aplicar movimiento y rotación
                        }
                        break;
                    }
                case 38: // flecha hacia arriba
                    {
                        if (contiene.Top < navex.Top - 10) // Límite superior
                        {
                            navex.Top -= 10;
                            NaveCorre(navex, 0, 1); // Movimiento vertical, sin rotación
                        }
                        break;
                    }
                case 39: // flecha hacia la derecha
                    {
                        if (contiene.Right > navex.Right + 10) // Límite derecho
                        {
                            navex.Left += 10;
                            angulo = +15; // Rotar a la derecha
                            NaveCorre(navex, (int)angulo, 1);
                        }
                        break;
                    }
                case 40: // flecha hacia abajo
                    {
                        if (contiene.Bottom > navex.Bottom + 10) // Límite inferior
                        {
                            navex.Top += 10;
                            NaveCorre(navex, 0, 1);
                        }
                        break;
                    }
                case 32: // Barra espaciadora (Disparar)
                    {
                        tiempo.Start();
                        int x = navex.Location.X + (navex.Width / 2);
                        int y = navex.Location.Y; // Dispara desde la parte superior
                        CrearMisil(0, Color.DarkMagenta, "Misil", x, y); // El tag es "Misil" para el jugador
                        break;
                    }
            }
        }

        //*** ACTIVAR ACCIONES DE INICIALIZACION **********************//
        public void Iniciar()
        {
            this.FormBorderStyle = FormBorderStyle.FixedSingle; // Cambiado a FixedSingle para mejor control
            this.Width = 345;
            this.Height = 450;
            this.Text = "JUEGO DE AVIONES";

            // Configurar Labels
            label1.Location = new Point(10, 10);
            label1.AutoSize = true;
            label2.Location = new Point(10, 30);
            label2.AutoSize = true;
            this.Controls.Add(label1);
            this.Controls.Add(label2);

            this.KeyDown += new KeyEventHandler(ActividadTecla);
            this.KeyPreview = true; // Para que el formulario capture las teclas

            // Configurar PictureBox contenedor (contiene)
            contiene.Location = new Point(0, 50); // Mover el contenedor debajo de las etiquetas
            contiene.BackColor = Color.AliceBlue;
            contiene.Size = new Size(300, 350); // Reducir un poco el tamaño para dejar espacio a las etiquetas
            contiene.Dock = DockStyle.Fill;
            contiene.Visible = true;
            this.Controls.Add(contiene);
            contiene.BringToFront(); // Para asegurar que las etiquetas estén visibles

            //----- contenido del formulario
            Random r = new Random();
            int aleatyr = r.Next(250, 330);
            int aleatxr = r.Next(50, 250);

            // NAVE JUGADOR (Tipox 1, sin rotación (0), Vida 20)
            CrearNave(navex, 0, 1, Color.SeaGreen, 20);

            // ELEGIR NAVE DE SALIDA RIVAL
            Random sal = new Random();
            int sale = sal.Next(1, 3);
            // NAVE RIVAL (Rotación 180, tipo aleatorio, Vida 50)
            CrearNave(naveRival, 180, sale, Color.DarkBlue, 50);

            // Posicionar naves
            // El código original usa variables aleatorias para navex.
            navex.Location = new Point(aleatxr, 300); 
            naveRival.Location = new Point(aleatxr, 50);

            // Inicializar y configurar el Timer (bucle de juego)
            tiempo = new Timer();
            tiempo.Interval = 50; // Intervalo más razonable para Windows Forms (50ms)
            tiempo.Enabled = true;
            tiempo.Tick += new EventHandler(ImpactarTick);
            tiempo.Start();
        }

        //*********** ARGUMENTOS GENERADOS POR EL PROGRAMA **********//
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Iniciar();
        }
    }
}