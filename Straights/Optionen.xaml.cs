using System;
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
using System.Windows.Shapes;
using System.Diagnostics;



namespace Straights
{
    /// <summary>
    /// Diese Klasse stellt ein Dialog zur verfügung der
    /// die Optionen der Hauptlogik anzeigt und setzen lässt
    /// </summary>
    /// <remarks>
    /// Autor:
    /// Andy Klay klay@fh-brandenburg.de
    /// </remarks>
    public partial class Optionen : Window
    {
        private bool generateIsOn=false;
        private Logik.Control mainControl;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="parent">Elternfenster</param>
        /// <param name="control">Hauptcontroller</param>
        public Optionen(Window parent, Logik.Control control)
        {
            this.Owner = parent;
            this.mainControl = control;

            InitializeComponent();

            this.AutoGenCheckbox.ToolTip = "Automatische Straights Generierung im Hintergrund AN/AUS";
            //Optionenwert aus Hauptcontroller holen
            this.generateIsOn = mainControl.GetGenerateState();

            //setzen des häkchen nach wert im Hauptcontroller
            if (this.generateIsOn)
            {
                this.AutoGenCheckbox.IsChecked = true;
            }
        }

        /// <summary>
        /// EventHandler wenn Checkbox checked wird
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">RoutedEventArgs</param>
        private void GenerateOption_Checked(object sender, RoutedEventArgs e)
        {
                mainControl.SetGenerateON();
                Debug.Print("Generate isON");
        }

        /// <summary>
        ///         /// EventHandler wenn Checkbox unchecked wird
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">RoutedEventArgs</param>
        private void GenerateOption_UnChecked(object sender, RoutedEventArgs e)
        {
            mainControl.SetGenerateOFF();
            Debug.Print("Generate isOFF");
        }

        /// <summary>
        /// Event-Handler für das Beenden des Programms bei Tätigiung des OK Buttons
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }


        /// <summary>
        /// Event-Handler für das Beenden des Programms über ALT+F4 und das rote X.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.DialogResult = true;
        }

    }
}
