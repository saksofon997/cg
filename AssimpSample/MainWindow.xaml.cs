using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using SharpGL.SceneGraph;
using SharpGL;
using Microsoft.Win32;


namespace AssimpSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Atributi

        /// <summary>
        ///	 Instanca OpenGL "sveta" - klase koja je zaduzena za iscrtavanje koriscenjem OpenGL-a.
        /// </summary>
        World m_world = null;

        #endregion Atributi

        #region Konstruktori

        public MainWindow()
        {
            // Inicijalizacija komponenti
            InitializeComponent();

            // Kreiranje OpenGL sveta
            try
            {
                m_world = new World(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Models"), "chopper.obj", "light.obj", (int)openGLControl.Width, (int)openGLControl.Height, openGLControl.OpenGL);
            }
            catch (Exception e)
            {
                MessageBox.Show("Neuspesno kreirana instanca OpenGL sveta. Poruka greške: " + e.Message, "Poruka", MessageBoxButton.OK);
                this.Close();
            }
        }

        #endregion Konstruktori

        /// <summary>
        /// Handles the OpenGLDraw event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLDraw(object sender, OpenGLEventArgs args)
        {
            m_world.Draw(args.OpenGL);
        }

        /// <summary>
        /// Handles the OpenGLInitialized event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLInitialized(object sender, OpenGLEventArgs args)
        {
            m_world.Initialize(args.OpenGL);
        }

        /// <summary>
        /// Handles the Resized event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_Resized(object sender, OpenGLEventArgs args)
        {
            m_world.Resize(args.OpenGL, (int)openGLControl.Width, (int)openGLControl.Height);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cb1.ItemsSource = Enum.GetValues(typeof(VerticalScaling));
            cb2.ItemsSource = Enum.GetValues(typeof(UniformScaling));
            cb3.ItemsSource = Enum.GetValues(typeof(AmbientLightColor));
            cb1.SelectedIndex = 1;
            cb2.SelectedIndex = 1;
            cb3.SelectedIndex = 1;
        }

        private void changeVerticalScale(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            m_world.LamppostVerticalScaling = (VerticalScaling)cb1.SelectedIndex;
            openGLControl.Focus();
        }

        private void changeUniformScale(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            m_world.MotorBikeAndLamppostUniformScaling = (UniformScaling)cb2.SelectedIndex;
            openGLControl.Focus();
        }

        private void changeAmbientalColor(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            m_world.AmbientLightColor = (AmbientLightColor)cb3.SelectedIndex;
            openGLControl.Focus();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F4: this.Close(); break;
                case Key.E: if (m_world.StartAnimation) break; if (m_world.RotationX >= -5.0f) m_world.RotationX -= 5.0f; break;
                case Key.D: if (m_world.StartAnimation) break; if (m_world.RotationX <= 90.0f) m_world.RotationX += 5.0f; break;
                case Key.S: if (m_world.StartAnimation) break; if (m_world.RotationY >= -90.0f) m_world.RotationY -= 5.0f; break;
                case Key.F: if (m_world.StartAnimation) break; if (m_world.RotationY <= 90.0f) m_world.RotationY += 5.0f; break;
                case Key.Add: if (m_world.StartAnimation) break; if (m_world.SceneDistance >= 500) m_world.SceneDistance -= 50.0f; break;
                case Key.Subtract: if (m_world.StartAnimation) break; if (m_world.SceneDistance <= 1300) m_world.SceneDistance += 50.0f; break;

                case Key.V: if (m_world.StartAnimation) m_world.StartAnimation = false; else{ m_world.StartAnimation = true; } break;

                case Key.I: if (m_world.StartAnimation) break; m_world.AmbientLightColorR += 0.1f; break;
                case Key.O: if (m_world.StartAnimation) break; m_world.AmbientLightColorG += 0.1f; break;
                case Key.P: if (m_world.StartAnimation) break; m_world.AmbientLightColorB += 0.1f; break;

                case Key.F2:
                    OpenFileDialog opfModel = new OpenFileDialog();
                    bool result = (bool) opfModel.ShowDialog();
                    if (result)
                    {

                        try
                        {
                            World newWorld = new World(Directory.GetParent(opfModel.FileName).ToString(), Path.GetFileName(opfModel.FileName), Path.GetFileName(opfModel.FileName), (int)openGLControl.ActualWidth, (int)openGLControl.ActualHeight, openGLControl.OpenGL);
                            m_world.Dispose();
                            m_world = newWorld;
                            m_world.Initialize(openGLControl.OpenGL);
                        }
                        catch (Exception exp)
                        {
                            MessageBox.Show("Neuspesno kreirana instanca OpenGL sveta:\n" + exp.Message, "GRESKA", MessageBoxButton.OK );
                        }
                    }
                    break;
            }
        }
    }
}
