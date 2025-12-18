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
        System.Windows.Forms.Timer tiempo = new System.Windows.Forms.Timer();
        Label label1 = new Label(); // Vida del rival
        Label label2 = new Label(); // Vida del jugador
        Panel panelInformacion = new Panel();
        Panel barraVidaJugadorContenedor = new Panel();
        Panel barraVidaJugador = new Panel();
        Panel barraVidaRivalContenedor = new Panel();
        Panel barraVidaRival = new Panel();

        int Dispara = 0;
        bool flag = false;
        float angulo = 0;
        bool naveColisionando = false;
        int vidaMaxJugador;
        int vidaMaxRival;
        int escalaJugador;
        int escalaRival;
        AvionConfig configJugadorActual = AvionFactory.Blindado;
        AvionConfig configRivalActual = new("Rival", 1, 50, Color.DarkBlue, 2);

        private readonly AvionConfig seleccionJugador;
        private readonly int escenarioSeleccionado;
        private bool juegoListo;

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

        private void ActualizarBarra(Panel barra, int valorActual, int valorMaximo)
        {
            if (barra.Parent == null || valorMaximo <= 0)
            {
                return;
            }

            int anchoMaximo = barra.Parent.Width - 4;
            int ancho = (int)(anchoMaximo * Math.Max(valorActual, 0) / (float)valorMaximo);
            barra.Width = Math.Max(0, ancho);
        }

        private void ActualizarVidaJugador(int delta)
        {
            int vidaActual = Math.Max(0, int.Parse(navex.Tag.ToString()) + delta);
            navex.Tag = vidaActual;
            label2.Text = "Vida: " + vidaActual;
            ActualizarBarra(barraVidaJugador, vidaActual, vidaMaxJugador);

            if (vidaActual <= 0)
            {
                navex.Dispose();
                barraVidaJugadorContenedor.Visible = false;
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
            int vidaActual = Math.Max(0, int.Parse(naveRival.Tag.ToString()) + delta);
            naveRival.Tag = vidaActual;
            label1.Text = "Vida del Rival: " + vidaActual;
            ActualizarBarra(barraVidaRival, vidaActual, vidaMaxRival);

            if (vidaActual <= 0)
            {
                naveRival.Dispose();
                barraVidaRivalContenedor.Visible = false;
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
            ActualizarPosicionBarraRival();

            if (Colision.EntreNaves(navex, naveRival))
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
            foreach (var c in contiene.Controls.OfType<PictureBox>().ToList())
            {
                if (c.Tag is string nombre)
                {
                    var missile = c;
                    int X1 = missile.Location.X;
                    int Y1 = missile.Location.Y;
                    int W1 = missile.Width;
                    int H1 = missile.Height;

                    // ACTIVIDAD DE IMPACTO CON LA NAVE RIVAL ("Misil" es el misil del jugador)
                    if (nombre == "Misil" &&
                        Colision.ImpactaObjeto(new Rectangle(X, Y, W, H), new Rectangle(X1, Y1, W1, H1), PH))
                    {
                        missile.Dispose();
                        ActualizarVidaRival(-1);
                    }

                    // ACTIVIDAD DE IMPACTO CON MI NAVE ("Rival" es el misil del rival)
                    if (nombre == "Rival" &&
                        Colision.ImpactaObjeto(new Rectangle(X2, Y2, W2, H2), new Rectangle(X1, Y1, W1, H1), PH))
                    {
                        missile.Dispose();
                        ActualizarVidaJugador(-1);
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

        private void ActualizarPosicionBarraRival()
        {
            if (naveRival.IsDisposed || !barraVidaRivalContenedor.Visible || !naveRival.Visible)
            {
                return;
            }

            int x = naveRival.Left;
            int y = Math.Max(5, naveRival.Top - barraVidaRivalContenedor.Height - 2);
            barraVidaRivalContenedor.Location = new Point(x, y);
        }

        private void AjustarHud()
        {
            int anchoDisponible = panelInformacion.ClientSize.Width - 20;
            barraVidaJugadorContenedor.Width = Math.Max(120, anchoDisponible);
            if (navex.Tag is int vida)
            {
                ActualizarBarra(barraVidaJugador, vida, vidaMaxJugador);
            }
        }

        private void MantenerNavesEnPantalla()
        {
            if (contiene.ClientRectangle.Width == 0 || contiene.ClientRectangle.Height == 0)
            {
                return;
            }

            void Ajustar(PictureBox nave)
            {
                if (nave.IsDisposed)
                {
                    return;
                }

                int x = Math.Min(Math.Max(nave.Left, 0), Math.Max(0, contiene.Width - nave.Width));
                int y = Math.Min(Math.Max(nave.Top, 0), Math.Max(0, contiene.Height - nave.Height));
                nave.Location = new Point(x, y);
            }

            Ajustar(navex);
            Ajustar(naveRival);
            ActualizarPosicionBarraRival();
        }

        private void ReescalarNave(PictureBox avion, AvionConfig config, int angulo, ref int escalaActual)
        {
            if (avion.IsDisposed)
            {
                return;
            }

            if (!avion.Visible)
            {
                escalaActual = config.Escala;
                return;
            }

            int nuevaEscala = config.Escala;
            if (nuevaEscala == escalaActual)
            {
                return;
            }

            Point centroActual = new(avion.Left + avion.Width / 2, avion.Top + avion.Height / 2);
            int vidaActual = avion.Tag is int vida ? vida : int.Parse(avion.Tag?.ToString() ?? config.Vida.ToString());

            AvionRender render = AvionFactory.Crear(config.Tipo, angulo, config.Color, nuevaEscala);
            avion.Size = render.Tamano;
            avion.Region = render.Region;
            avion.Image = render.Imagen;
            avion.Location = new Point(
                Math.Max(0, centroActual.X - avion.Width / 2),
                Math.Max(0, centroActual.Y - avion.Height / 2));
            avion.Tag = vidaActual;
            avion.BringToFront();

            escalaActual = nuevaEscala;
            MantenerNavesEnPantalla();
        }

        //*** DIAGRAMAR NAVE **********//
        public void CrearNave(PictureBox Avion, int AngRotar, AvionConfig config)
        {
            AvionRender render = AvionFactory.Crear(config.Tipo, AngRotar, config.Color, config.Escala);

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
            if (!juegoListo || navex.Tag == null)
            {
                return;
            }

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

            MantenerNavesEnPantalla();
            ActualizarPosicionBarraRival();
        }

        private void RedimensionarTodo()
        {
            if (!juegoListo)
            {
                return;
            }

            RegenerarEscenario();
            AjustarHud();
            int nuevaEscalaJugador = CalcularEscalaJugador();
            int nuevaEscalaRival = CalcularEscalaRival();
            var configJugadorRedimensionado = configJugadorActual with { Escala = nuevaEscalaJugador };
            ReescalarNave(navex, configJugadorRedimensionado, 0, ref escalaJugador);
            configJugadorActual = configJugadorRedimensionado;

            var configRivalRedimensionado = configRivalActual with { Escala = nuevaEscalaRival };
            ReescalarNave(naveRival, configRivalRedimensionado, 180, ref escalaRival);
            configRivalActual = configRivalRedimensionado;

            MantenerNavesEnPantalla();
        }

        private void RegenerarEscenario()
        {
            if (contiene.ClientSize.Width == 0 || contiene.ClientSize.Height == 0)
            {
                return;
            }

            contiene.BackgroundImage = Escenarios.CrearEscenario(escenarioSeleccionado, contiene.ClientSize);
        }

        private int CalcularEscalaJugador()
        {
            return Math.Max(3, Math.Min(6, contiene.ClientSize.Width / 160));
        }

        private int CalcularEscalaRival()
        {
            return Math.Max(2, Math.Min(5, contiene.ClientSize.Width / 200));
        }

        //*** ACTIVAR ACCIONES DE INICIALIZACION **********************//
        public void Iniciar()
        {
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MinimumSize = new Size(360, 500);
            this.Width = 1600;
            this.Height = 1024;
            this.Text = "JUEGO DE AVIONES";
            this.DoubleBuffered = true;

            panelInformacion.Dock = DockStyle.Top;
            panelInformacion.Height = 80;
            panelInformacion.Padding = new Padding(10);
            panelInformacion.BackColor = Color.FromArgb(30, 30, 30);

            // Configurar Labels
            label1.Location = new Point(10, 10);
            label1.ForeColor = Color.White;
            label1.AutoSize = true;
            label2.Location = new Point(10, 30);
            label2.ForeColor = Color.White;
            label2.AutoSize = true;

            barraVidaJugadorContenedor.Size = new Size(200, 16);
            barraVidaJugadorContenedor.Location = new Point(10, 50);
            barraVidaJugadorContenedor.BorderStyle = BorderStyle.FixedSingle;
            barraVidaJugadorContenedor.BackColor = Color.Black;

            barraVidaJugador.Height = 14;
            barraVidaJugador.Width = 200;
            barraVidaJugador.BackColor = Color.LimeGreen;
            barraVidaJugadorContenedor.Controls.Add(barraVidaJugador);

            panelInformacion.Controls.Add(barraVidaJugadorContenedor);
            panelInformacion.Controls.Add(label2);
            panelInformacion.Controls.Add(label1);

            this.KeyDown += new KeyEventHandler(ActividadTecla);
            this.KeyPreview = true; // Para que el formulario capture las teclas
            this.Resize += (s, e) => RedimensionarTodo();

            // Configurar PictureBox contenedor (contiene)
            contiene.BackColor = Color.AliceBlue;
            contiene.Size = new Size(this.ClientSize.Width, this.ClientSize.Height - panelInformacion.Height);
            contiene.Dock = DockStyle.Fill;
            contiene.Visible = true;

            contiene.BackgroundImageLayout = ImageLayout.Stretch;

            this.Controls.Add(contiene);
            this.Controls.Add(panelInformacion);

            //----- contenido del formulario
            escalaJugador = CalcularEscalaJugador();
            configJugadorActual = seleccionJugador with { Escala = escalaJugador };
            CrearNave(navex, 0, configJugadorActual);
            navex.Tag = configJugadorActual.Vida; // Vida inicial garantizada
            vidaMaxJugador = configJugadorActual.Vida;

            // ELEGIR NAVE DE SALIDA RIVAL
            Random sal = new Random();
            int sale = sal.Next(1, 4);
            escalaRival = CalcularEscalaRival();
            configRivalActual = new AvionConfig("Rival", sale, 50, Color.DarkBlue, escalaRival);
            CrearNave(naveRival, 180, configRivalActual);
            vidaMaxRival = configRivalActual.Vida;

            // Posicionar naves
            int centroX = Math.Max(0, (contiene.Width / 2) - (navex.Width / 2));
            navex.Location = new Point(centroX, Math.Max(0, contiene.Height - navex.Height - 20)); 
            naveRival.Location = new Point(Math.Max(0, (contiene.Width / 2) - (naveRival.Width / 2)), 20);
            navex.BringToFront();
            naveRival.BringToFront();

            barraVidaRivalContenedor.Size = new Size(naveRival.Width, 8);
            barraVidaRivalContenedor.BackColor = Color.Black;
            barraVidaRivalContenedor.Visible = true;
            barraVidaRivalContenedor.Controls.Clear();

            barraVidaRival.Height = 6;
            barraVidaRival.Width = naveRival.Width;
            barraVidaRival.BackColor = Color.Red;
            barraVidaRivalContenedor.Controls.Add(barraVidaRival);
            contiene.Controls.Add(barraVidaRivalContenedor);
            barraVidaRivalContenedor.BringToFront();
            ActualizarPosicionBarraRival();

            label1.Text = "Vida del Rival: " + configRivalActual.Vida;
            label2.Text = "Vida: " + navex.Tag;
            ActualizarBarra(barraVidaJugador, (int)navex.Tag, vidaMaxJugador);
            ActualizarBarra(barraVidaRival, (int)naveRival.Tag, vidaMaxRival);
            RegenerarEscenario();
            AjustarHud();

            // Inicializar y configurar el Timer (bucle de juego)
            tiempo.Interval = 50; // Intervalo más razonable para Windows Forms (50ms)
            tiempo.Enabled = true;
            tiempo.Tick += new EventHandler(ImpactarTick);
            tiempo.Start();

            juegoListo = true;
        }
    }
}
