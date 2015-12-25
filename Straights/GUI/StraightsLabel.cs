using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using Straights.Core;

namespace Straights.GUI
{
    /// <summary>
    /// Diese Klasse stellt ein GUI-Element dar,
    /// welches weiß welche Zelle es selbst repräsentiert,
    /// Label und Zelle sind so synchron miteinander
    /// (MVC: Model-View Verbindung)
    /// </summary>
    /// <remarks>
    /// Autor:
    /// Andy Klay klay@fh-brandenburg.de
    /// </remarks>
    class StraightsLabel: Label
    {
        private Zelle zelle;
        private bool isCorrect = true;


        /// <summary>
        /// Konstruktor, um ein StraightsLabel zu erzeugen
        /// </summary>
        /// <param name="zelle">StraightsFeld im Model</param>
        public StraightsLabel(Straights.Core.Zelle zelle)
        {
            this.zelle = zelle;
            this.SetContent(zelle.Value);
        }

        /// <summary>
        /// setzen der Daten in Zelle und Label mit einem KeyEvent
        /// </summary>
        /// <param name="e">System.Windows.Input.KeyEventArgs</param>
        public void SetData(System.Windows.Input.KeyEventArgs e){

            // unschön. gibt's hier was besseres?
            switch (e.Key)
            {
                case Key.D1:
                case Key.NumPad1:
                    this.Content = 1;
                    this.zelle.Value = 1;
                    break;
                case Key.D2:
                case Key.NumPad2:
                    this.Content = 2;
                    this.zelle.Value = 2;
                    break;
                case Key.D3:
                case Key.NumPad3:
                    this.Content = 3;
                    this.zelle.Value = 3;
                    break;
                case Key.D4:
                case Key.NumPad4:
                    this.Content = 4;
                    this.zelle.Value = 4;
                    break;
                case Key.D5:
                case Key.NumPad5:
                    this.Content = 5;
                    this.zelle.Value = 5;
                    break;
                case Key.D6:
                case Key.NumPad6:
                    this.Content = 6;
                    this.zelle.Value = 6;
                    break;
                case Key.D7:
                case Key.NumPad7:
                    this.Content = 7;
                    this.zelle.Value = 7;
                    break;
                case Key.D8:
                case Key.NumPad8:
                    this.Content = 8;
                    this.zelle.Value = 8;
                    break;
                case Key.D9:
                case Key.NumPad9:
                    this.Content = 9;
                    this.zelle.Value = 9;
                    break;
                case Key.Delete:
                case Key.Back:
                    this.Content = "";
                    this.zelle.Value = 0;
                    break;
            }

        }


        /// <summary>
        /// Setzt den Inhalt des Labels
        /// </summary>
        /// <param name="value"></param>
        private void SetContent(uint value)
        {
            if (value == 0)
            {
                this.Content = "";
            }
            else
            {
                this.Content = value;
            }
        }

        /// <summary>
        /// Fragt ob ein StraightsLabel als Correkt oder nciht korrekt markiert ist
        /// (incorrect=rote zahlen in der GUI)
        /// </summary>
        public bool IsCoorect
        {
            get{return this.isCorrect;}
            set{this.isCorrect=value;}
        }


        /// <summary>
        /// Holt Zelle die das StraightsLabel representiert
        /// </summary>
        /// <returns></returns>
        public Zelle GetZelle()
        {
            return this.zelle;
        }

    }
}
