// -----------------------------------------------------------------------
// <file>World.cs</file>
// <copyright>Grupa za Grafiku, Interakciju i Multimediju 2013.</copyright>
// <author>Srđan Mihić</author>
// <author>Aleksandar Josić</author>
// <summary>Klasa koja enkapsulira OpenGL programski kod.</summary>
// -----------------------------------------------------------------------
using System;
using SharpGL.SceneGraph.Primitives;
using SharpGL.SceneGraph.Quadrics;
using SharpGL;
using System.Drawing;
using System.Drawing.Imaging;
using Lighting;
using System.Windows.Threading;

namespace AssimpSample
{
    /// <summary>
    ///  Nabrojani tip skaliranja bandere
    /// </summary>
    public enum VerticalScaling
    {
        Small,
        Normal,
        Double,
        Triple
    };

    /// <summary>
    ///  Nabrojani tip skaliranja bandere i motora
    /// </summary>
    public enum UniformScaling
    {
        Small,
        Normal,
        Double,
        Triple
    };

    /// <summary>
    ///  Nabrojani tip boja ambijentalnog osvetljenja
    /// </summary>
    public enum AmbientLightColor
    {
        White,
        Yellow,
        Red,
        Green,
        Blue
    };

    /// <summary>
    ///  Klasa enkapsulira OpenGL kod i omogucava njegovo iscrtavanje i azuriranje.
    /// </summary>
    public class World : IDisposable
    {
        #region Atributi

        /// <summary>
        ///	 Scena koja se prikazuje.
        /// </summary>
        private AssimpScene m_scene;

        /// <summary>
        ///	 Scena koja se prikazuje.
        /// </summary>
        private AssimpScene m_scene2;

        /// <summary>
        ///	 Ugao rotacije sveta oko X ose.
        /// </summary>
        private float m_xRotation = 0.0f;

        /// <summary>
        ///	 Ugao rotacije sveta oko Y ose.
        /// </summary>
        private float m_yRotation = 0.0f;

        /// <summary>
        ///	 Udaljenost scene od kamere.
        /// </summary>
        private float m_sceneDistance = 900f;

        /// <summary>
        ///	 Sirina OpenGL kontrole u pikselima.
        /// </summary>
        private int m_width;

        /// <summary>
        ///	 Visina OpenGL kontrole u pikselima.
        /// </summary>
        private int m_height;

        //LookAt parametri
        private float m_eyeX = 90.0f;
        private float m_eyeY = 80.0f;
        private float m_eyeZ = 0.0f;

        private float m_centerX = 90.0f;
        private float m_centerY = 80.0f;
        private float m_centerZ = -1.0f;

        private float m_upX = 0.0f;
        private float m_upY = 1.0f;
        private float m_upZ = 0.0f;

        //Izbor ambijentalnog
        float[] ambient_light = new float[] { 0.1f, 0.1f, 0.1f, 1.0f };

        /// <summary>
        ///	 Identifikatori tekstura za jednostavniji pristup teksturama
        /// </summary>
        private enum TextureObjects { Tree = 0, Asphalt, Building, Road, Brick };
        private readonly int m_textureCount = Enum.GetNames(typeof(TextureObjects)).Length;

        /// <summary>
        ///	 Identifikatori OpenGL tekstura
        /// </summary>
        private uint[] m_textures = null;

        /// <summary>
        ///	 Putanje do slika koje se koriste za teksture
        /// </summary>
        private string[] m_textureFiles = { "..//..//images//tree.jpg", "..//..//images//asphalt.jpg", "..//..//images//building.png", "..//..//images//road.jpeg", "..//..//images//brick.jpg" };

        //Animacije
        private Boolean m_startAnimation = false;

        private DispatcherTimer animationTimer;
        private float rotateBike = 0f;
        private float bike_z = 15f;
        private float bike_x = -3f;

        //Skaliranje
        private float light_scale_y = 1f;
        private float light_bike_uniform = 1f;

        /// <summary>
        ///	 Izabrani tip skaliranja.
        /// </summary>
        private VerticalScaling m_selectedVerticalScaling;

        /// <summary>
        ///	 Izabrani tip skaliranja.
        /// </summary>
        private UniformScaling m_selectedUniformScaling;

        /// <summary>
        ///	 Izabrani tip svetlosnog izvora.
        /// </summary>
        private AmbientLightColor Ambient_Light;

        #endregion Atributi

        #region Properties

        /// <summary>
        ///	 Boja R ambijentalnog osvetljenja
        /// </summary>
        public float AmbientLightColorR
        {
            get { return ambient_light[0]; }
            set
            {
                if (value >= 1f)
                    value = 0f;
                ambient_light[0] = value;
                gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_AMBIENT, ambient_light);

            }
        }

        /// <summary>
        ///	 Boja G ambijentalnog osvetljenja
        /// </summary>
        public float AmbientLightColorG
        {
            get { return ambient_light[1]; }
            set
            {
                if (value >= 1f)
                    value = 0f;
                ambient_light[1] = value;
                gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_AMBIENT, ambient_light);

            }
        }

        /// <summary>
        ///	 Boja B ambijentalnog osvetljenja
        /// </summary>
        public float AmbientLightColorB
        {
            get { return ambient_light[2]; }
            set
            {
                if (value >= 1f)
                    value = 0f;
                ambient_light[2] = value;
                gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_AMBIENT, ambient_light);

            }
        }

        /// <summary>
        ///	 Referenca na OpenGL instancu unutar aplikacije.
        /// </summary>
        private OpenGL gl;

        /// <summary>
        ///	 Ugao rotacije sveta oko X ose.
        /// </summary>
        public float RotationX
        {
            get { return m_xRotation; }
            set { m_xRotation = value; }
        }

        public float Bike_z
        {
            get { return bike_z; }
            set { bike_z = value; }
        }

        public float Bike_x
        {
            get { return bike_x; }
            set { bike_x = value; }
        }

        /// <summary>
        ///	 Ugao rotacije sveta oko Y ose.
        /// </summary>
        public float RotationY
        {
            get { return m_yRotation; }
            set { m_yRotation = value; }
        }

        /// <summary>
        ///	 Udaljenost scene od kamere.
        /// </summary>
        public float SceneDistance
        {
            get { return m_sceneDistance; }
            set { m_sceneDistance = value; }
        }

        /// <summary>
        ///	 Sirina OpenGL kontrole u pikselima.
        /// </summary>
        public int Width
        {
            get { return m_width; }
            set { m_width = value; }
        }

        /// <summary>
        ///	 Visina OpenGL kontrole u pikselima.
        /// </summary>
        public int Height
        {
            get { return m_height; }
            set { m_height = value; }
        }

        public Boolean StartAnimation
        {
            get { return m_startAnimation; }
            set { m_startAnimation = value; }
        }

        public VerticalScaling LamppostVerticalScaling
        {
            get { return m_selectedVerticalScaling; }
            set
            {
                m_selectedVerticalScaling = value;
                switch (m_selectedVerticalScaling)
                {
                    case VerticalScaling.Small:
                        light_scale_y = 0.5f;
                        break;

                    case VerticalScaling.Normal:
                        light_scale_y = 1f;
                        break;

                    case VerticalScaling.Double:
                        light_scale_y = 2f;
                        break;

                    case VerticalScaling.Triple:
                        light_scale_y = 3f;
                        break;
                };
            }
        }

        public UniformScaling MotorBikeAndLamppostUniformScaling
        {
            get { return m_selectedUniformScaling; }
            set
            {
                m_selectedUniformScaling = value;
                switch (m_selectedUniformScaling)
                {
                    case UniformScaling.Small:
                        light_bike_uniform = 0.5f;
                        break;

                    case UniformScaling.Normal:
                        light_bike_uniform = 1f;
                        break;

                    case UniformScaling.Double:
                        light_bike_uniform = 2f;
                        break;

                    case UniformScaling.Triple:
                        light_bike_uniform = 3f;
                        break;
                };
            }
        }

        public AmbientLightColor AmbientLightColor
        {
            get { return Ambient_Light; }
            set
            {
                Ambient_Light = value;
                float[] light0ambient = new float[] { 0.1f, 0.1f, 0.1f, 1.0f };

                switch (Ambient_Light)
                {
                    case AmbientLightColor.White:
                        light0ambient = new float[] { 0.1f, 0.1f, 0.1f, 1.0f };
                        break;

                    case AmbientLightColor.Red:
                        light0ambient = new float[] { 0.5f, 0f, 0f, 1.0f };
                        break;

                    case AmbientLightColor.Blue:
                        light0ambient = new float[] { 0f, 0f, 0.5f, 1.0f };
                        break;

                    case AmbientLightColor.Yellow:
                        light0ambient = new float[] { 0.5f, 0.5f, 0f, 1.0f };
                        break;

                    case AmbientLightColor.Green:
                        light0ambient = new float[] { 0f, 0.5f, 0f, 1.0f };
                        break;
                }
                gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_AMBIENT, light0ambient);

            }
        }

        #endregion Properties

        #region Konstruktori

        /// <summary>
        ///  Konstruktor klase World.
        /// </summary>
        public World(String scenePath, String sceneFileName, String sceneFileName2, int width, int height, OpenGL gl)
        {
            this.gl = gl;
            this.m_scene = new AssimpScene(scenePath, sceneFileName, gl);
            this.m_scene2 = new AssimpScene(scenePath, sceneFileName2, gl);
            this.m_width = width;
            this.m_height = height;
            m_textures = new uint[m_textureCount];
        }

        /// <summary>
        ///  Destruktor klase World.
        /// </summary>
        ~World()
        {
            this.Dispose(false);
        }

        #endregion Konstruktori

        #region Metode

        /// <summary>
        ///  Korisnicka inicijalizacija i podesavanje OpenGL parametara.
        /// </summary>
        public void Initialize(OpenGL gl)
        {
            gl.Enable(OpenGL.GL_DEPTH_TEST);
            gl.FrontFace(OpenGL.GL_CCW);
            gl.CullFace(OpenGL.GL_FRONT);
            gl.Enable(OpenGL.GL_CULL_FACE);

            gl.Enable(OpenGL.GL_NORMALIZE);
            gl.Enable(OpenGL.GL_AUTO_NORMAL);
            gl.Enable(OpenGL.GL_COLOR_MATERIAL);
            gl.ColorMaterial(OpenGL.GL_FRONT, OpenGL.GL_AMBIENT_AND_DIFFUSE);
            gl.ShadeModel(OpenGL.GL_SMOOTH);

            SetupTextures();

            SetupLighting();

            m_scene.LoadScene();
            m_scene.Initialize();
            m_scene2.LoadScene();
            m_scene2.Initialize();

            animationTimer = new DispatcherTimer();
            animationTimer.Interval = TimeSpan.FromMilliseconds(20);
            animationTimer.Tick += new EventHandler(UpdateAnimation);
        }

        private void UpdateAnimation(object sender, EventArgs e)
        {
            if(bike_x <= -30f)
            {
                bike_x = -3f;
                bike_z = 15f;
                rotateBike = 0f;
            }

            if (bike_z >= -13f)
            {
                bike_z -= 0.3f;
            }
            else if (bike_z <= -13f)
            {
                rotateBike = 90f;
                bike_x -= 0.6f;
            }
            else
            {
                bike_x -= 0.3f;
            }
        }

        private void SetupTextures()
        {
            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_ADD);

            // Ucitaj slike i kreiraj teksture
            gl.GenTextures(m_textureCount, m_textures);
            for (int i = 0; i < m_textureCount; ++i)
            {
                // Pridruzi teksturu odgovarajucem identifikatoru
                gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[i]);

                // Ucitaj sliku i podesi parametre teksture
                Bitmap image = new Bitmap(m_textureFiles[i]);
                // rotiramo sliku zbog koordinantog sistema opengl-a
                image.RotateFlip(RotateFlipType.RotateNoneFlipY);
                Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
                // RGBA format (dozvoljena providnost slike tj. alfa kanal)
                BitmapData imageData = image.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly,
                                                      System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                gl.Build2DMipmaps(OpenGL.GL_TEXTURE_2D, (int)OpenGL.GL_RGBA8, image.Width, image.Height, OpenGL.GL_BGRA, OpenGL.GL_UNSIGNED_BYTE, imageData.Scan0);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_NEAREST);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_NEAREST);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_S, OpenGL.GL_REPEAT);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_T, OpenGL.GL_REPEAT);

                image.UnlockBits(imageData);
                image.Dispose();
            }
        }

        private void SetupLighting()
        {
            // Ukljuci ambijentalno osvetljenje
            float[] global_ambient = new float[] { 0.3f, 0.3f, 0.3f, 1.0f };
            gl.LightModel(OpenGL.GL_LIGHT_MODEL_AMBIENT, global_ambient);

            // Pridruži komponente svetlosnom izvoru 0
            float[] light0pos = new float[] { 100f, 500f, 200f, 1.0f };
            float[] light0diffuse = new float[] { 0.7f, 0.7f, 0.7f, 1.0f };
            float[] light0specular = new float[] { 0.8f, 0.8f, 0.8f, 1.0f };

            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, light0pos);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_AMBIENT, ambient_light);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_DIFFUSE, light0diffuse);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPECULAR, light0specular);

            // Podesi parametre tackastog svetlosnog izvora
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPOT_CUTOFF, 180.0f);

            gl.Enable(OpenGL.GL_LIGHT0);

            // Pridruži komponente svetlosnom izvoru 1
            float[] light1ambient = new float[] { 1.0f, 1.0f, 0.0f, 1.0f };
            float[] light1diffuse = new float[] { 1.0f, 1.0f, 0.0f, 1.0f };
            float[] light1specular = new float[] { 1.0f, 1.0f, 0.0f, 1.0f };

            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_AMBIENT, light1ambient);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_DIFFUSE, light1diffuse);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPECULAR, light1specular);

            // Podesi parametre svetlosnog izvora na banderi
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPOT_CUTOFF, 40.0f);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPOT_EXPONENT, 5.0f);

            gl.Enable(OpenGL.GL_LIGHT1);

            gl.Enable(OpenGL.GL_LIGHTING);
        }

        /// <summary>
        ///  Iscrtavanje OpenGL kontrole.
        /// </summary>
        public void Draw(OpenGL gl) {

            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            gl.LookAt(m_eyeX, m_eyeY, m_eyeZ, m_centerX, m_centerY, m_centerZ, m_upX, m_upY, m_upZ);

            gl.ClearColor(0.0f, 0.0f, 0.9f, 0.1f);
            gl.Color(0.1f, 0.1f, 0.1f);

            gl.Translate(0.0f, 0.0f, -m_sceneDistance);

            gl.Rotate(m_xRotation, 1.0f, 0.0f, 0.0f);
            gl.Rotate(m_yRotation, 0.0f, 1.0f, 0.0f);

            gl.Enable(OpenGL.GL_TEXTURE_2D);
            DrawSurface(gl);
            gl.Disable(OpenGL.GL_TEXTURE_2D);

            gl.Enable(OpenGL.GL_TEXTURE_2D);
            DrawBuildings(gl);
            gl.Disable(OpenGL.GL_TEXTURE_2D);

            gl.Enable(OpenGL.GL_TEXTURE_2D);
            DrawLights(gl);
            gl.Disable(OpenGL.GL_TEXTURE_2D);

            DrawModels(gl);

            DrawText(gl);

            if (m_startAnimation)
            {
                animationTimer.Start();
            }
            else
            {
                animationTimer.Stop();

                bike_x = -3f;
                bike_z = 15f;
                rotateBike = 0f;
            }

            gl.Flush();
        }

        private void DrawLights(OpenGL gl)
        {
            gl.PushMatrix();

            //skaliranje uniform
            gl.Scale(light_bike_uniform, light_bike_uniform, light_bike_uniform);

            //skaliranje visine
            gl.Scale(1f, light_scale_y, 1f);

            gl.Scale(50f, 50f, 50f);

            gl.Translate(-5f, 3f, 0f);

            DrawSingleLight(gl);

            gl.Translate(10f, 0f, 0f);

            gl.Rotate(0f, 180f, 0f);

            DrawSingleLight(gl);

            gl.PopMatrix();
            
        }

        private void DrawSingleLight(OpenGL gl)
        {
            gl.PushMatrix();
            
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Tree]);

            gl.PushMatrix();
            gl.Scale(0.1f, 5f, 0.1f);
            Cube cube = new Cube();
            cube.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();

            gl.Translate(0.5f, 4.92f, 0f);

            gl.PushMatrix();
            gl.Scale(0.5f, 0.1f, 0.1f);
            Cube cube2 = new Cube();
            cube2.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();

            gl.Translate(0.4f, -0.5f, 0f);

            //light position
            float[] spot_direction = new float[] { 0f, -1f, 0f };
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPOT_DIRECTION, spot_direction);
            float[] light1pos = new float[] { 0f, 4f, 0f, 1.0f };
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_POSITION, light1pos);

            gl.Scale(0.3f, 0.5f, 0.3f);
            gl.Rotate(90f, 180f, 0f);
            Cylinder cil = new Cylinder();
            cil.CreateInContext(gl);
            cil.TextureCoords = true;
            cil.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);

            gl.PopMatrix();
        }

        private void DrawBuildings(OpenGL gl)
        {
            gl.PushMatrix();

            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Building]);

            gl.MatrixMode(OpenGL.GL_TEXTURE);
            gl.PushMatrix();
            gl.Rotate(0f, 0f, -90f);

            gl.MatrixMode(OpenGL.GL_MODELVIEW);

            gl.Scale(50f, 50f, 50f);

            gl.Translate(-10f, 10.1f, 0f);

            DrawSingleBuilding(gl);

            gl.Translate(20f, 0f, 0f);

            DrawSingleBuilding(gl);

            //front

            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Brick]);

            gl.MatrixMode(OpenGL.GL_TEXTURE);
            gl.PushMatrix();//PUSH TEXT
            gl.Scale(1f, 5f, 1f);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);

            gl.Translate(-10f, 0f, -21f);

            gl.Rotate(0f, 90f, 0f);
            gl.Scale(1f, 1f, 4f);
            DrawSingleBuilding(gl);

            gl.MatrixMode(OpenGL.GL_TEXTURE);
            gl.PopMatrix(); //POP TEXT

            gl.MatrixMode(OpenGL.GL_MODELVIEW);

            gl.PopMatrix();
        }

        private void DrawSingleBuilding(OpenGL gl)
        {
            gl.PushMatrix();

            gl.Scale(3f, 12f, 8f);

            Cube cube1 = new Cube();

            cube1.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);

            gl.PopMatrix();
        }

        private void DrawText(OpenGL gl)
        {
            //gl.PushMatrix();
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();
            gl.PushMatrix();

            gl.Ortho2D(0, m_width, 0, m_height);
            gl.PopMatrix();

            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();
            gl.PushMatrix();

            gl.Color(0f, 0f, 1f);
            gl.Scale(0.03f, 0.03f, 0.03f);
            gl.Translate(20f, -25f, 0f);
            gl.DrawText3D("Verdana", 1f, 1f, 0.1f, "Predmet: Racunarska grafika");
            gl.Translate(-12f, -0.3f, 0f);
            gl.DrawText3D("Verdana", 1f, 1f, 0.1f, "______________________");
            gl.Translate(-11.5f, -1.3f, 0f);
            gl.DrawText3D("Verdana", 1f, 1f, 0.1f, "Sk.god: 2019 / 20");
            gl.Translate(-7.6f, -0.35f, 0f);
            gl.DrawText3D("Verdana", 1f, 1f, 0.1f, "________________");
            gl.Translate(-8.3f, -1.3f, 0f);
            gl.DrawText3D("Verdana", 1f, 1f, 0.1f, "Ime: Aleksandar");
            gl.Translate(-7f, -0.35f, 0f);
            gl.DrawText3D("Verdana", 1f, 1f, 0.1f, "______________");
            gl.Translate(-7.1f, -1.3f, 0f);
            gl.DrawText3D("Verdana", 1f, 1f, 0.1f, "Prezime: Petakovic");
            gl.Translate(-8f, -0.35f, 0f);
            gl.DrawText3D("Verdana", 1f, 1f, 0.1f, "_________________");
            gl.Translate(-8.9f, -1.3f, 0f);
            gl.DrawText3D("Verdana", 1f, 1f, 0.1f, "Sifra zad: 12.2");
            gl.Translate(-6f, -0.35f, 0f);
            gl.DrawText3D("Verdana", 1f, 1f, 0.1f, "____________");

            gl.PopMatrix();

            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();
            gl.Perspective(60f, (double)m_width / m_height, 0.5f, 20000f);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();
            //gl.PopMatrix();
        }

        private void DrawModels(OpenGL gl)
        {

            gl.PushMatrix();

            gl.Scale(40f, 40f, 40f);

            gl.Translate(4.9f, -2.4f, -2f);

            gl.PushMatrix();

            gl.Rotate(-90f, 0f, 0f);

            gl.Scale(0.018f, 0.018f, 0.018f);

            m_scene2.Draw();

            gl.PopMatrix();

            gl.Translate(bike_x, -0.7f, bike_z);

            //skaliranje
            gl.Scale(light_bike_uniform, light_bike_uniform, light_bike_uniform);

            gl.Rotate(0f, rotateBike, 0f);

            gl.Scale(0.5f, 0.5f, 0.5f);

            m_scene.Draw();

            gl.PopMatrix();
        }

        private void DrawSurface(OpenGL gl)
        {
            gl.PushMatrix();

            gl.Translate(0.0f, -100f, 0f);
            gl.Scale(50f, 50f, 50f);

            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Asphalt]);
            gl.Begin(OpenGL.GL_QUADS);

            //gl.Normal(0f, 1f, 0f);
            gl.Normal(LightingUtilities.FindFaceNormal(-50f, 0f, -50f, -50f, 0f, 50f, 50f, 0f, 50f));

            gl.TexCoord(-2.0f, -2.0f);
            gl.Vertex(-50f, 0f, -50f);
            gl.TexCoord(-2.0f, 2.0f);
            gl.Vertex(-50f, 0f, 50f);
            gl.TexCoord(2.0f, 2.0f);
            gl.Vertex(50f, 0f, 50f);
            gl.TexCoord(2.0f, -2.0f);
            gl.Vertex(50f, 0f, -50f);

            gl.End();

            gl.Translate(0f, 0.01f, 0f);

            gl.PushMatrix();//PUSH

            gl.MatrixMode(OpenGL.GL_TEXTURE);
            gl.LoadIdentity();
            gl.PushMatrix();//PUSH TEXT
            gl.Scale(1f, 10f, 1f);

            gl.MatrixMode(OpenGL.GL_MODELVIEW);

            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Road]);
            gl.Begin(OpenGL.GL_QUADS);
            gl.Normal(LightingUtilities.FindFaceNormal(-4.5f, 0f, -50f, -4.5f, 0f, 50f, 4.5f, 0f, 50f));

            gl.TexCoord(0.0f, 1.0f);
            gl.Vertex(-4.5f, 0f, -50f);
            gl.TexCoord(0.0f, 0.0f);
            gl.Vertex(-4.5f, 0f, 50f);
            gl.TexCoord(1.0f, 0.0f);
            gl.Vertex(4.5f, 0f, 50f);
            gl.TexCoord(1.0f, 1.0f);
            gl.Vertex(4.5f, 0f, -50f);

            gl.End();

            gl.MatrixMode(OpenGL.GL_TEXTURE);
            gl.PopMatrix(); //POP TEXT

            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.PopMatrix(); //POP

            gl.PopMatrix();
        }

        /// <summary>
        /// Podesava viewport i projekciju za OpenGL kontrolu.
        /// </summary>
        public void Resize(OpenGL gl, int width, int height)
        {
            m_width = width;
            m_height = height;
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();
            gl.Perspective(60f, (double)width / height, 0.5f, 20000f);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();
        }

        /// <summary>
        ///  Implementacija IDisposable interfejsa.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                m_scene.Dispose();
            }
        }

        #endregion Metode

        #region IDisposable metode

        /// <summary>
        ///  Dispose metoda.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable metode
    }
}
