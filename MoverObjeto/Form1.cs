using System;
using System.Drawing;
using System.Drawing.Drawing2D;
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
        System.Windows.Forms.Timer timerObstaculos = new System.Windows.Forms.Timer();
        Label label1 = new Label();
        Label label2 = new Label();
        Panel panelInformacion = new Panel();
        Panel barraVidaJugadorContenedor = new Panel();
        Panel barraVidaJugador = new Panel();
        Panel barraVidaRivalContenedor = new Panel();
        Panel barraVidaRival = new Panel();

        // Variables de Estado del Juego
        int Dispara = 0;
        bool naveColisionando = false;
        int vidaMaxJugador;
        int vidaMaxRival;
        
        // Variables de Control de Movimiento (Jugador)
        bool goLeft, goRight, goUp, goDown, goShoot;
        int playerSpeed = 20;
        int cooldownDisparo = 0;

        // Variables de Control de Movimiento (Enemigo)
        int enemySpeedX = 6;
        int enemySpeedY = 4;
        bool enemyMovingRight = true;
        bool enemyMovingDown = true;

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
            
            int PosX = 1;
            int PosY = 1;
            int largoM = 20;
            int anchoM = 6;

            Point[] myMisill = {
                new Point(4 * PosX, 0 * PosY), new Point(5 * PosX, 1 * PosY), new Point(6 * PosX, 2 * PosY),
                new Point(6 * PosX, 7 * PosY), new Point(7 * PosX, 8 * PosY), new Point(8 * PosX, 9 * PosY),
                new Point(7 * PosX, 9 * PosY), new Point(6 * PosX, 10 * PosY), new Point(2 * PosX, 10 * PosY),
                new Point(1 * PosX, 9 * PosY), new Point(0 * PosX, 9 * PosY), new Point(1 * PosX, 8 * PosY),
                new Point(2 * PosX, 7 * PosY), new Point(2 * PosX, 2 * PosY), new Point(3 * PosX, 1 * PosY),
                new Point(4 * PosX, 0 * PosY) 
            };
            
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

            Bitmap flag = new Bitmap(anchoM, largoM);
            Graphics flagImagen = Graphics.FromImage(flag);
            flagImagen.FillRectangle(Brushes.Orange, 0, 0, anchoM, 5);
            flagImagen.FillRectangle(Brushes.Yellow, 0, 5, anchoM, 6);
            Balas.Image = flag;
        }

        private void ActualizarBarra(Panel barra, int valorActual, int valorMaximo)
        {
            if (barra.Parent == null || valorMaximo <= 0) return;
            int anchoMaximo = barra.Parent.Width - 4;
            int ancho = (int)(anchoMaximo * Math.Max(valorActual, 0) / (float)valorMaximo);
            barra.Width = Math.Max(0, ancho);
        }

        private void MostrarFinJuego(bool victoria)
        {
            tiempo.Stop();
            timerObstaculos.Stop();
            
            Panel panelFin = new Panel {
                Size = new Size(400, 200),
                BackColor = Color.FromArgb(240, 240, 240),
                BorderStyle = BorderStyle.FixedSingle,
                Anchor = AnchorStyles.None, // Para centrarlo
                Location = new Point((contiene.Width - 400) / 2, (contiene.Height - 200) / 2)
            };

            Label lblTitulo = new Label {
                Text = victoria ? "¡VICTORIA!" : "¡DERROTA!",
                Font = new Font("Arial", 24, FontStyle.Bold),
                ForeColor = victoria ? Color.Green : Color.Red,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 80
            };

            Label lblMensaje = new Label {
                Text = victoria ? "¡Has derribado al enemigo!" : "Tu avión ha sido destruido.",
                Font = new Font("Arial", 14),
                TextAlign = ContentAlignment.TopCenter,
                Dock = DockStyle.Fill
            };
            
            panelFin.Controls.Add(lblMensaje);
            panelFin.Controls.Add(lblTitulo);
            
            contiene.Controls.Add(panelFin);
            panelFin.BringToFront();
        }

        private void ActualizarVidaJugador(int delta)
        {
            if (!juegoListo || navex.Tag == null) return;
            int vidaActual = Math.Max(0, int.Parse(navex.Tag.ToString()) + delta);
            navex.Tag = vidaActual;
            label2.Text = "Vida: " + vidaActual;
            ActualizarBarra(barraVidaJugador, vidaActual, vidaMaxJugador);

            if (vidaActual <= 0)
            {
                navex.Dispose();
                barraVidaJugadorContenedor.Visible = false;
                MostrarFinJuego(false);
            }
        }

        private void ActualizarVidaRival(int delta)
        {
            if (!juegoListo || naveRival.Tag == null) return;
            int vidaActual = Math.Max(0, int.Parse(naveRival.Tag.ToString()) + delta);
            naveRival.Tag = vidaActual;
            label1.Text = "Vida del Rival: " + vidaActual;
            ActualizarBarra(barraVidaRival, vidaActual, vidaMaxRival);

            if (vidaActual <= 0)
            {
                naveRival.Dispose();
                barraVidaRivalContenedor.Visible = false;
                MostrarFinJuego(true);
            }
        }

        private void ImpactarTick(object sender, EventArgs e)
        {
            if (!juegoListo) return;

            if (goLeft && contiene.Left < navex.Left - playerSpeed) navex.Left -= playerSpeed;
            if (goRight && contiene.Right > navex.Right + playerSpeed) navex.Left += playerSpeed;
            if (goUp && contiene.Top < navex.Top - playerSpeed) navex.Top -= playerSpeed;
            if (goDown && contiene.Bottom > navex.Bottom + playerSpeed) navex.Top += playerSpeed;

            if (goShoot && cooldownDisparo < 1)
            {
                CrearMisil(0, Color.DarkMagenta, "Misil", navex.Location.X + (navex.Width / 2), navex.Location.Y);
                cooldownDisparo = 5;
            }
            if (cooldownDisparo > 0) cooldownDisparo--;

            int xRival = naveRival.Location.X;
            int yRival = naveRival.Location.Y;
            int limiteMedio = contiene.Height / 2;
            int padding = 10;

            if (enemyMovingRight)
            {
                xRival += enemySpeedX;
                if (xRival + naveRival.Width > contiene.Width - padding) enemyMovingRight = false;
            }
            else
            {
                xRival -= enemySpeedX;
                if (xRival < padding) enemyMovingRight = true;
            }

            if (enemyMovingDown)
            {
                yRival += enemySpeedY;
                if (yRival > limiteMedio) enemyMovingDown = false;
            }
            else
            {
                yRival -= enemySpeedY;
                if (yRival < padding) enemyMovingDown = true;
            }
            naveRival.Location = new Point(xRival, yRival);
            ActualizarPosicionBarraRival();

            Dispara++;
            if (Dispara > 30 && naveRival.Visible)
            {
                CrearMisil(180, Color.OrangeRed, "Rival", naveRival.Location.X + (naveRival.Width / 2), naveRival.Location.Y + naveRival.Height);
                Dispara = 0;
            }

            if (Colision.EntreNaves(navex, naveRival)) {
                if (!naveColisionando) {
                    ActualizarVidaJugador(-10);
                    naveColisionando = true;
                }
            } else {
                naveColisionando = false;
            }

            foreach (var c in contiene.Controls.OfType<PictureBox>().ToList())
            {
                if (c.Tag is string nombre)
                {
                    if (nombre == "Misil" && Colision.ImpactaObjeto(naveRival.Bounds, c.Bounds, 10)) {
                        c.Dispose();
                        ActualizarVidaRival(-1);
                    } else if (nombre == "Rival" && Colision.ImpactaObjeto(navex.Bounds, c.Bounds, 10)) {
                        c.Dispose();
                        ActualizarVidaJugador(-1);
                    }
                    
                    if (nombre == "Misil") {
                        c.Top -= 15;
                        if (c.Location.Y < 0) c.Dispose();
                    } else if (nombre == "Rival") {
                        c.Top += 15;
                        if (c.Location.Y > contiene.Height) c.Dispose();
                    }
                }
                else if (c.Tag is Obstaculo)
                {
                    if (navex.Bounds.IntersectsWith(c.Bounds))
                    {
                        ActualizarVidaJugador(-15);
                        c.Dispose();
                    }
                }
            }
        }

        private void ActualizarPosicionBarraRival()
        {
            if (!barraVidaRivalContenedor.Visible || !naveRival.Visible) return;
            barraVidaRivalContenedor.Location = new Point(naveRival.Left, Math.Max(5, naveRival.Top - barraVidaRivalContenedor.Height - 2));
        }

        public void CrearNave(PictureBox Avion, int AngRotar, AvionConfig config)
        {
            AvionRender render = AvionFactory.Crear(config.Tipo, AngRotar, config.Color);
            Avion.BackColor = Color.Transparent;
            Avion.Size = render.Tamano;
            Avion.Region = render.Region;
            Avion.Location = new Point(0, 0);
            contiene.Controls.Add(Avion);
            Avion.Image = render.Imagen;
            Avion.Tag = config.Vida;
            Avion.Visible = true;
        }

        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left) goLeft = true;
            if (e.KeyCode == Keys.Right) goRight = true;
            if (e.KeyCode == Keys.Up) goUp = true;
            if (e.KeyCode == Keys.Down) goDown = true;
            if (e.KeyCode == Keys.Space) goShoot = true;
        }

        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left) goLeft = false;
            if (e.KeyCode == Keys.Right) goRight = false;
            if (e.KeyCode == Keys.Up) goUp = false;
            if (e.KeyCode == Keys.Down) goDown = false;
            if (e.KeyCode == Keys.Space) goShoot = false;
        }

        public void Iniciar()
        {
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.Text = "JUEGO DE AVIONES";

            panelInformacion.Dock = DockStyle.Top;
            panelInformacion.Height = 80;
            panelInformacion.Padding = new Padding(10);
            panelInformacion.BackColor = Color.FromArgb(30, 30, 30);

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

            this.KeyDown += KeyIsDown;
            this.KeyUp += KeyIsUp;
            this.KeyPreview = true;

            contiene.BackColor = Color.AliceBlue;
            contiene.Dock = DockStyle.Fill;
            contiene.Visible = true;
            contiene.BackgroundImageLayout = ImageLayout.Stretch;
            
            this.Controls.Add(contiene);
            this.Controls.Add(panelInformacion);

            // Evento para re-dibujar el fondo cuando se redimensiona
            this.Resize += (sender, args) => {
                if (juegoListo) {
                    contiene.BackgroundImage = Escenarios.CrearEscenario(escenarioSeleccionado, contiene.ClientSize);
                }
            };

            Random r = new Random();
            int aleatxr = r.Next(50, this.ClientSize.Width - 100);

            CrearNave(navex, 0, seleccionJugador);
            navex.Tag = seleccionJugador.Vida;
            vidaMaxJugador = seleccionJugador.Vida;

            Random sal = new Random();
            int sale = sal.Next(1, 4);
            AvionConfig configRival = new("Rival", sale, 50, Color.DarkBlue);
            CrearNave(naveRival, 180, configRival);
            vidaMaxRival = configRival.Vida;

            navex.Location = new Point(aleatxr, this.ClientSize.Height - 150); 
            naveRival.Location = new Point(aleatxr, 50);

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

            label1.Text = "Vida del Rival: " + configRival.Vida;
            label2.Text = "Vida: " + navex.Tag;
            ActualizarBarra(barraVidaJugador, (int)navex.Tag, vidaMaxJugador);
            ActualizarBarra(barraVidaRival, (int)naveRival.Tag, vidaMaxRival);

            tiempo.Interval = 50;
            tiempo.Enabled = true;
            tiempo.Tick += new EventHandler(ImpactarTick);
            tiempo.Start();

            timerObstaculos.Interval = 2000;
            timerObstaculos.Tick += (sender, e) => Obstaculo.GenerarObstaculo(contiene);
            timerObstaculos.Start();

            juegoListo = true;
            // Forzar la primera carga del fondo con el tamaño correcto
            contiene.BackgroundImage = Escenarios.CrearEscenario(escenarioSeleccionado, contiene.ClientSize);
        }
    }
}
