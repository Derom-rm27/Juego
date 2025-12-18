using System;
using System.Drawing;
using System.Drawing.Drawing2D; // Librería para GraphicsPath
using System.Linq;
using System.Windows.Forms;

namespace MoverObjeto
{
    public partial class Form1 : Form
    {
        //*********** VARIABLES GLOBALES *************//
        PictureBox navex = new PictureBox();
        PictureBox naveRival = new PictureBox();
        PictureBox contiene = new PictureBox();
        Timer tiempo = new Timer();
        Label label1 = new Label(); // Vida del rival
        Label label2 = new Label(); // Vida del jugador

        int Dispara = 0;
        bool flag = false;
        float angulo = 0;
        bool naveColisionando = false;

        private readonly AvionConfig seleccionJugador;
        private readonly int escenarioSeleccionado;

        public Form1(AvionConfig? config = null, int escenario = 0)
        {
            seleccionJugador = config ?? AvionFactory.Blindado;
            escenarioSeleccionado = escenario;

            InitializeComponent();
            Load += Form1_Load;
        }

        private void Form1_Load(object? sender, EventArgs e)
        {
            Iniciar();
        }

        //********** DIAGRAMAR DEL MISIL ***********//
        public void CrearMisil(int AngRotar, Color pintar, string nombre, int x, int y)
        {
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

        private void ActualizarVidaJugador(int delta)
        {
            int vidaActual = int.Parse(navex.Tag.ToString()) + delta;
            navex.Tag = vidaActual;
            label2.Text = "Vida: " + Math.Max(vidaActual, 0);

            if (vidaActual <= 0)
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

        private void ActualizarVidaRival(int delta)
        {
            int vidaActual = int.Parse(naveRival.Tag.ToString()) + delta;
            naveRival.Tag = vidaActual;
            label1.Text = "Vida del Rival: " + Math.Max(vidaActual, 0);

            if (vidaActual <= 0)
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

            if (navex.Visible && naveRival.Visible && navex.Bounds.IntersectsWith(naveRival.Bounds))
            {
                if (!naveColisionando)
                {
                    ActualizarVidaJugador(-10);
                    naveColisionando = true;
                }
            }
            else
            {
                naveColisionando = false;
            }

            // ELIMINACION DEL MISIL Y DESCONTAR PUNTOS DE IMPACTO
            foreach (Control c in contiene.Controls.OfType<PictureBox>().ToList())
            {
                if ((string?)c.Tag != null)
                {
                    PictureBox missile = (PictureBox)c;
                    int X1 = missile.Location.X;
                    int Y1 = missile.Location.Y;
                    int W1 = missile.Width;
                    int H1 = missile.Height;
                    string nombre = missile.Tag!.ToString()!;

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
                            ActualizarVidaRival(-1);
                        }
                        else
                        {
                            missile.Dispose();
                            ActualizarVidaRival(-1);
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
                            ActualizarVidaJugador(-1);
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
        public void CrearNave(PictureBox Avion, int AngRotar, AvionConfig config)
        {
            AvionRender render = AvionFactory.Crear(config.Tipo, AngRotar, config.Color);

            Avion.BackColor = Color.Transparent;
            Avion.Size = render.Tamano;
            Avion.Region = render.Region;
            Avion.Location = new Point(0, 0);
            
            //*********INSERTAR LA NAVE AL CONTENDOR***********//
            contiene.Controls.Add(Avion);

            // DIBUJAR IMAGEN DENTRO DE LA NAVE (EFECTOS/COLORES)
            Avion.Image = render.Imagen;
            Avion.Tag = config.Vida;
            Avion.Visible = true;
        }

        //*********** EFECTOS DE LA NAVE PRINCIPAL ***********//
        public void NaveCorre(PictureBox Avion, int AngRotar, int velox)
        {
            Bitmap Imagen = new Bitmap(Avion.Width, Avion.Height);
            Graphics PintaImg = Graphics.FromImage(Imagen);
            
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
            }
            else if (velox == 2) // Velocidad 2
            {
                PintaImg.FillRectangle(Brushes.DarkRed, 5, 7, 1, 8);
            }
            else if (velox == 3) // Velocidad 3
            {
                PintaImg.FillRectangle(Brushes.DarkRed, 15, 30, 1, 16);
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

            contiene.BackgroundImage = Escenarios.CrearEscenario(escenarioSeleccionado, contiene.ClientSize);
            contiene.BackgroundImageLayout = ImageLayout.Stretch;

            //----- contenido del formulario
            Random r = new Random();
            int aleatxr = r.Next(50, 250);

            // NAVE JUGADOR
            CrearNave(navex, 0, seleccionJugador);

            // ELEGIR NAVE DE SALIDA RIVAL
            Random sal = new Random();
            int sale = sal.Next(1, 4);
            AvionConfig configRival = new("Rival", sale, 50, Color.DarkBlue);
            CrearNave(naveRival, 180, configRival);

            // Posicionar naves
            navex.Location = new Point(aleatxr, 300); 
            naveRival.Location = new Point(aleatxr, 50);

            label1.Text = "Vida del Rival: " + configRival.Vida;
            label2.Text = "Vida: " + seleccionJugador.Vida;

            // Inicializar y configurar el Timer (bucle de juego)
            tiempo.Interval = 50; // Intervalo más razonable para Windows Forms (50ms)
            tiempo.Enabled = true;
            tiempo.Tick += new EventHandler(ImpactarTick);
            tiempo.Start();
        }
    }
}
