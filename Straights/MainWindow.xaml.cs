using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Straights.Core;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System;
using Straights.GUI;
using System.Timers;
using System.Windows.Threading;




namespace Straights
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    /// <remarks>
    /// Autoren:
    /// Andy Klay klay@fh-brandenburg.de
    /// Frank Mertens mertens@fh-brandenburg.de
    /// </remarks>
    public partial class MainWindow : Window
    {
        private Logik.Control mainControl;
        private int playtimeInSec = 0;

        /// <summary>
        /// Konstruktor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            //tooltipps hinzufuegen
            this.openImage.ToolTip = "Neues Spiel öffnen";
            this.optionImage.ToolTip = "Optionen anzeigen";
            this.printImage.ToolTip = "Spielfeld Drucken";
            this.infoButton.ToolTip = "Über die Entwickler";


            //Logik holen
            mainControl = Logik.Control.GetInstance();

            this.InitializePuzzle();
            this.InitializeTime();
        }

        /// <summary>
        /// Event-Handler für das Zeitsignal
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            //eine Sekunde weiter zählen
            playtimeInSec++;

            //labelcontent aktualisieren
            this.zeitLabel.Content = Zeit.GetTimeAsString(playtimeInSec);
        }


        /// <summary>
        /// Initialisiert ein Testpuzzle und bereitet die GUI darauf vor, ein Puzzel anzuzeigen.
        /// </summary>
        private void InitializePuzzle()
        {
            //variable fuer spielstatus >> erlaubter zustand?
            bool gameState = true;

            // spielzeit setzen
            playtimeInSec = mainControl.GetPuzzle().GetPlaytime();
            //zeitLabel setzen
            this.zeitLabel.Content = Zeit.GetTimeAsString(playtimeInSec);

            //bezeichnerlabel setzen
            this.bezeichnungLabel.Content = mainControl.GetPuzzle().GetBezeichnung();

            //falls noch ein spiel geladen sit, clearen der alten zellen
            Spielfeld.Children.Clear();

            //zellen holen
            Puzzle puzzle = mainControl.GetPuzzle();

            //einmal pruefen
            gameState = mainControl.GetPuzzle().IsAllowedState();

            foreach (Straights.Core.Zelle feld in puzzle.Cells)
            {
                GenerateCell(feld, gameState);
            }

        }

        /// <summary>
        /// Zeitlogik initialisieren und starten
        /// </summary>
        private void InitializeTime()
        {
            DispatcherTimer dispatcherTimer = new DispatcherTimer();

            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);

            //alle sekunde updaten
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);

            dispatcherTimer.Start();
        }


        /// <summary>
        /// Generiert eine Spielzelle im Puzzle.
        /// </summary>
        /// <param name="zelle">Straightsfeld</param>
        /// <param name="gameState">Loesungsstatus des Spiels</param>
        private void GenerateCell(Straights.Core.Zelle zelle, bool gameState)
        {
            Border element = new Border();
            StraightsLabel sLabel = new StraightsLabel(zelle);

            if (zelle.CHANGEABLE)
            {
                element.Style = (Style)FindResource("BorderChangeableStyle");
                sLabel.Style = (Style)FindResource("LabelChangeableStyle");

                if (!gameState && zelle.Value != 0)
                {
                    sLabel.IsCoorect = false;
                    sLabel.Style = (Style)FindResource("LabelChangeableNotValidStyle");
                }

            }
            else
            {
                if (zelle.SOLID)
                {
                    element.Style = (Style)FindResource("BorderSolidStyle");
                    sLabel.Style = (Style)FindResource("LabelSolidStyle");
                }
                else
                {
                    element.Style = (Style)FindResource("BorderNotChangeableStyle");
                    sLabel.Style = (Style)FindResource("LabelNotChangeableStyle");
                }
            }

            element.Child = sLabel;
            Spielfeld.Children.Add(element);
        }


        /// <summary>
        /// Eingabe eines Wertes in ein StraightsLabel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Label_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (sender is StraightsLabel)
            {
                // sender in das Label casten, dass das Event ausgelöst hat.
                StraightsLabel sLabel = sender as StraightsLabel;

                sLabel.SetData(e);

                if (mainControl.GetPuzzle().IsAllowedState())
                {
                    sLabel.Style = (Style)FindResource("LabelChangeableStyle");

                    foreach (Border border in Spielfeld.Children)
                    {
                        if (!((StraightsLabel)border.Child).IsCoorect)
                        {
                            sLabel.IsCoorect = true;
                            ((StraightsLabel)border.Child).Style = (Style)FindResource("LabelChangeableStyle");
                        }
                    }

                    if (mainControl.GetPuzzle().IsAusgefuellt())
                    {
                        MessageBox.Show("Sie haben gewonnen!", "Straights", MessageBoxButton.OK, MessageBoxImage.Information);
                    }

                }
                else
                {
                    sLabel.Style = (Style)FindResource("LabelChangeableNotValidStyle");
                    sLabel.IsCoorect = false;
                }



            }
        }

        /// <summary>
        /// EventHandler für das ausklappen des Menues bei MouseEnter 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Label_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (sender is Label)
            {
                Label control = sender as Label; // sender in das Label casten, dass das Event ausgelöst hat.
                control.Focus(); // Dem Label den Focus geben, damit Key Events für das selektierte Label ausgelöst werden.
                control.Parent.SetValue(Panel.ZIndexProperty, 1);
            }
        }

        /// <summary>
        /// EventHandler für das einklappen des Menues bei MouseLeave 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Label_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Label)
            {
                Label control = sender as Label; // sender in das Label casten, dass das Event ausgelöst hat.
                control.Parent.SetValue(Panel.ZIndexProperty, 0);
            }
        }

        /// <summary>
        /// Neues Spiel laden 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_New(object sender, RoutedEventArgs e)
        {
            //LadeSpiel Dialog
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Straights Files|*.sts";
            openFileDialog.Title = "Wähle ein Straights-Game-File!";

            // Show the Dialog.
            openFileDialog.ShowDialog();

            //pruefen ob gültiger pfad
            if (openFileDialog.CheckPathExists)
            {
                //pruefen falls auf abbrechen geclickt wurde
                if (openFileDialog.FileName.Length > 0)
                {
                    mainControl.LoadGame(openFileDialog.FileName);
                }
            }

            this.InitializePuzzle();

        }

        /// <summary>
        ///Optionen aufrufen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Optionen(object sender, RoutedEventArgs e)
        {
            Optionen o = new Optionen(this, this.mainControl);
            //o.Show();//nicht modal
            o.ShowDialog();
            //CFG Datei neu schreiben
            this.mainControl.SaveConfig();
        }

        /// <summary>
        /// Event-Handler für das Beenden des Programms über den Menü-Button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Drucken(object sender, RoutedEventArgs e)
        {
            //this.StraightsBeenden();
            Drucken();
        }

        /// <summary>
        /// Event-Handler für das Beenden des Programms über ALT+F4 und das rote X.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //deaktivierung der standard beendigung      
            //zum umleiten in eigene Methode ist diese deaktivierung nötig
            e.Cancel = true;
            //eigene Beenden methode aufrufen
            StraightsBeenden();
        }

        /// <summary>
        /// Beenden des Programms
        /// </summary>
        private void StraightsBeenden()
        {
            // Configure the message box to be displayed
            string messageBoxText = "Willst Du das laufende Spiel sichern?";
            string caption = "Beenden?";
            MessageBoxButton button = MessageBoxButton.YesNoCancel;
            MessageBoxImage icon = MessageBoxImage.Warning;

            // Display message box
            MessageBoxResult result = MessageBox.Show(messageBoxText, caption, button, icon);

            // Process message box results
            switch (result)
            {
                case MessageBoxResult.Yes:
                    // User pressed Yes button
                    //MessageBox.Show("Programm wird beendet und aktuelles Spiel wird gespeichert!", "Straights", MessageBoxButton.OK, MessageBoxImage.Information);

                    //Spiel auf standardpfad sichern
                    mainControl.GetPuzzle().SetPlaytime(playtimeInSec);

                    //Programm beenden
                    mainControl.EndGame(true);

                    break;
                case MessageBoxResult.No:
                    // User pressed No button 
                    //MessageBox.Show("Programm wird beendet und aktuelles Spiel wird nicht gespeichert!", "Straights", MessageBoxButton.OK, MessageBoxImage.Information);

                    //Programm beenden
                    mainControl.EndGame(false);

                    break;
                case MessageBoxResult.Cancel:
                    // User pressed Cancel button
                    //MessageBox.Show("Programm wird nicht beendet!", "Straights", MessageBoxButton.OK, MessageBoxImage.Information);

                    //Nichts passiert, Spiel wird nicht beendet

                    break;
            }
        }


        /// <summary>
        /// Druckfunktion aufrufen
        /// </summary>
        private void Drucken()
        {

            //alte farbe merken
            System.Windows.Media.Brush merke = this.Background;
            //auf weiss setzen zum drucken
            this.Background = System.Windows.Media.Brushes.White;
            //menueschlatflaeche verstecken
            this.MenueSchaltflaeche.Visibility = Visibility.Collapsed;
            //info button verstecken
            this.infoButton.Visibility = Visibility.Collapsed;

            //leere straights felder weiss setzen
            foreach (Border control in Spielfeld.Children)
            {
                if (control.Style == (Style)FindResource("BorderNotChangeableStyle"))
                    control.Style = (Style)FindResource("BorderNotChangeablePrintStyle");

                if (control.Style == (Style)FindResource("BorderChangeableStyle"))
                    control.Style = (Style)FindResource("BorderChangeablePrintStyle");
            }

            //Drcukfunktion aufrufen
            Logik.PrintHelper.Print(this, mainControl.GetPuzzle().GetBezeichnung());


            // Weiße Spielfelder wieder auf hellgrau ändern.
            foreach (Border control in Spielfeld.Children)
            {
                if (control.Style == (Style)FindResource("BorderNotChangeablePrintStyle"))
                    control.Style = (Style)FindResource("BorderNotChangeableStyle");

                if (control.Style == (Style)FindResource("BorderChangeablePrintStyle"))
                    control.Style = (Style)FindResource("BorderChangeableStyle");
            }

            //farbe zurücksetzen
            this.Background = merke;
            //menue wieder sichtbar machen
            this.MenueSchaltflaeche.Visibility = Visibility.Visible;
            //infobutton wieder sichtbar machen
            this.infoButton.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Aufruf eines speicherdialogs
        /// </summary>
        private void SpeicherSpielDialog()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Straights Files|*.sts";
            saveFileDialog.Title = "Select a Straights-Game-File";

            // Show the Dialog.
            saveFileDialog.ShowDialog();

            //pruefen ob gültiger pfad
            if (saveFileDialog.CheckPathExists)
            {
                //pruefen falls auf abbrechen geclickt wurde
                if (saveFileDialog.FileName.Length > 0)
                    mainControl.SaveGame(saveFileDialog.FileName);
            }

        }

        /// <summary>
        /// Infodialog aufrufen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void infoButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("AlphaStraights \nCreated by\nAndy Klay   klay@fh-brandenburg.de\nFrank Mertens   mertens@fh-brandenburg.de\n"
                +"\nVersion 0.99 vom 28.06.2011\n(with 4th Generation of Generator)", "Entwickler", MessageBoxButton.OK, MessageBoxImage.Information);
        }

    }
}