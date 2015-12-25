using Straights.Core;
using System.Diagnostics;
using System.IO;
using System.Data;
using System.Collections.Generic;
using Straights.Serial;
using System.Xml.Serialization;
using System.Threading;
using System.Windows.Threading;
using System;
using System.Windows;

namespace Straights.Logik
{    
    
    /// <summary>
    /// Diese Klasse steuert das Programm
    /// (MVC  Haupt-Control-Klasse als Singleton)
    /// </summary>
    /// <remarks>
    /// Autor:
    /// Andy Klay klay@fh-brandenburg.de
    /// </remarks>
    public class Control
    {
        private Puzzle puzzle;
        private bool generateGamesIsOn = false;
        private static Control instance=null;
        private readonly string LASTGAME="lastGame.sts";
        private readonly string CONFIGFILE = "straights.scfg";
        private System.Threading.Thread genThread = new System.Threading.Thread(new Straights.Core.GeneratorNeu().Generate);

        /// <summary>
        /// Erstellt eine Control Klasse mit einem Standard Spiel
        /// </summary>
        private Control()
        {
            //Letzten spielstand bzw. STANDARD Start Straights laden 
            InitPuzzle();

            //laden der Konfiguration
            LoadConfig();

            //wenn generieren eingeschaltet dann thread starten
            if (generateGamesIsOn)
            {                
                //hier generator thread starten
                genThread.Priority = ThreadPriority.BelowNormal;
                genThread.Start();
            }
        }

        /// <summary>
        /// Schaltet das Generien an
        /// </summary>
        public void SetGenerateON(){
            generateGamesIsOn = true;

            //falls thread nicht schon läuft, dann puzzleen starten
            if (!genThread.IsAlive)
            {
                genThread = new System.Threading.Thread(new Straights.Core.GeneratorNeu().Generate);
                genThread.Priority = ThreadPriority.BelowNormal;
                genThread.Start();
            }
        }

        /// <summary>
        /// Schaltet das Generien aus
        /// </summary>
        public void SetGenerateOFF()
        {
            generateGamesIsOn = false;

            // thread killen
            genThread.Abort();
        }

        /// <summary>
        /// Gibt Generierungstatus zurück
        /// </summary>
        /// <returns></returns>
        public bool GetGenerateState(){
            return this.generateGamesIsOn;
        }

        /// <summary>
        /// Singleton
        /// </summary>
        /// <returns></returns>
        public static Control GetInstance(){

            if (instance == null)
            {
                instance = new Control();
            }

            return instance;
        }

        /// <summary>
        /// Gibt Spielfeld zurück (MVC MODEL)
        /// </summary>
        /// <returns></returns>
        public Puzzle GetPuzzle()
        {
            return puzzle;
        }


        /// <summary>
        /// Beenden des Programms
        /// </summary>
        /// <param name="mitSpielSpeichern">option Spielstand speichern oder nicht</param>
        public void EndGame(bool mitSpielSpeichern)
        {
            // Überprüfen, ob das Spiel gespeichert werden soll!

            //thread abschießen falls am laufen
            if (!genThread.IsAlive)
            {
                genThread.Abort();
            }

            //spielstand speichern
            if (mitSpielSpeichern)
            {
                SaveLastGame();
            }

            //cfg saven
            SaveConfig();

            // Programm beenden.
            System.Environment.Exit(0);
        }

        /// <summary>
        /// Laden eines Spiels
        /// </summary>
        /// <param name="pfad"></param>
        public void LoadGame(string pfad){

            Serial.Serialisator s = new Serial.Serialisator();

            //Debug.Print("File:" + pfad + "\n");            

            puzzle = s.LoadPuzzleFromFile(pfad);
        }

        /// <summary>
        /// Speichern eines Spiels
        /// </summary>
        /// <param name="pfad"></param>
        public void SaveGame(string pfad)
        {
            
            Serial.Serialisator s = new Serial.Serialisator();


            s.SavePuzzleToFile(puzzle, pfad);
        }

        /// <summary>
        /// Speichern des letzen Spielstandes
        /// </summary>
        private void SaveLastGame()
        {

            Serial.Serialisator s = new Serial.Serialisator();
            s.SavePuzzleToFile(puzzle, LASTGAME);
        }

        /// <summary>
        /// Laden des letzten Spielstandes, wenn vorhanden
        /// sonst Standard Spiel initialisieren
        /// </summary>
        public void InitPuzzle()
        {
            //wenn Spielstand existiert
            if (File.Exists(LASTGAME))
            {
                //spiel laden
                this.LoadGame(LASTGAME);
            }
            else
            {
                //satndardspiel init
                this.InitStandardPuzzle();
            }
        }

        /// <summary>
        /// Laden der Configuration falls vorhanden
        /// </summary>
        private void LoadConfig()
        {
            Dictionary<string, string> dic=null;

            Serialisator serialisator = new Serialisator();

            //NUR LADEN WENN DATEI BEREITS EXISTIERT
            if (File.Exists(CONFIGFILE))
            {
                dic = serialisator.LoadConfigFromFile(CONFIGFILE);
            }

            //wenn erfolgreich geladen dann werte in der klasse setzen
            //sonst standardwerte in der klasse nutzen
            if (dic != null)
            {
                 try
                {
                    this.generateGamesIsOn = bool.Parse(dic["generateGamesIsOn"]);
                }
                catch(System.ArgumentNullException e)
                {
                    //fehler
                }
                catch(System.FormatException e)
                {

                }  

                //hier folgen weitere einstellungen
                //....this.attribute =dic["schluessel"]

            }

        }

        /// <summary>
        /// Speichern der Configuration
        /// </summary>
        public void SaveConfig()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();            
            Serialisator serialisator = new Serialisator();

            //cfg aus der klasse setzen
            dic.Add("generateGamesIsOn", this.generateGamesIsOn+"");

            //cfg speichern
            serialisator.SaveConfigToFile(dic, CONFIGFILE); 
        }

        /// <summary>
        /// Initialisieren eines Standardspiels
        /// </summary>
        private void InitStandardPuzzle()
        {

            puzzle = new Puzzle();
            puzzle.SetBezeichnung("Standardstart Straights");

            //spalte 1
            puzzle.SetCell(new Coordinates(0, 0), new Core.Zelle(0, false, true));
            puzzle.SetCell(new Coordinates(1, 0), new Core.Zelle(0, true, false));
            puzzle.SetCell(new Coordinates(2, 0), new Core.Zelle(0, true, false));
            puzzle.SetCell(new Coordinates(3, 0), new Core.Zelle(3, false, false));
            puzzle.SetCell(new Coordinates(4, 0), new Core.Zelle(0, false, true));
            puzzle.SetCell(new Coordinates(5, 0), new Core.Zelle(0, false, true));
            puzzle.SetCell(new Coordinates(6, 0), new Core.Zelle(7, false, false));
            puzzle.SetCell(new Coordinates(7, 0), new Core.Zelle(0, true, false));
            puzzle.SetCell(new Coordinates(8, 0), new Core.Zelle(0, false, true));

            //spalte 2                    Änderbar, Schwarz
            puzzle.SetCell(new Coordinates(0, 1), new Core.Zelle(0, true, false));
            puzzle.SetCell(new Coordinates(1, 1), new Core.Zelle(0, true, false));
            puzzle.SetCell(new Coordinates(2, 1), new Core.Zelle(0, true, false));
            puzzle.SetCell(new Coordinates(3, 1), new Core.Zelle(0, true, false));
            puzzle.SetCell(new Coordinates(4, 1), new Core.Zelle(2, false, false));
            puzzle.SetCell(new Coordinates(5, 1), new Core.Zelle(0, true, false));
            puzzle.SetCell(new Coordinates(6, 1), new Core.Zelle(0, true, false));
            puzzle.SetCell(new Coordinates(7, 1), new Core.Zelle(0, true, false));
            puzzle.SetCell(new Coordinates(8, 1), new Core.Zelle(0, true, false));

            //spalte 3                    Änderbar, Schwarz
            puzzle.SetCell(new Coordinates(0, 2), new Core.Zelle(0, true, false));
            puzzle.SetCell(new Coordinates(1, 2), new Core.Zelle(0, true, false));
            puzzle.SetCell(new Coordinates(2, 2), new Core.Zelle(0, true, false));
            puzzle.SetCell(new Coordinates(3, 2), new Core.Zelle(1, false, true));
            puzzle.SetCell(new Coordinates(4, 2), new Core.Zelle(0, true, false));
            puzzle.SetCell(new Coordinates(5, 2), new Core.Zelle(0, true, false));
            puzzle.SetCell(new Coordinates(6, 2), new Core.Zelle(0, false, true));
            puzzle.SetCell(new Coordinates(7, 2), new Core.Zelle(0, true, false));
            puzzle.SetCell(new Coordinates(8, 2), new Core.Zelle(0, true, false));


            //spalte 4                    Änderbar, Schwarz
            puzzle.SetCell(new Coordinates(0, 3), new Core.Zelle(0, true, false));
            puzzle.SetCell(new Coordinates(1, 3), new Core.Zelle(2, false, false));
            puzzle.SetCell(new Coordinates(2, 3), new Core.Zelle(0, false, true));
            puzzle.SetCell(new Coordinates(3, 3), new Core.Zelle(0, true, false));
            puzzle.SetCell(new Coordinates(4, 3), new Core.Zelle(0, true, false));
            puzzle.SetCell(new Coordinates(5, 3), new Core.Zelle(0, false, true));
            puzzle.SetCell(new Coordinates(6, 3), new Core.Zelle(0, true, false));
            puzzle.SetCell(new Coordinates(7, 3), new Core.Zelle(0, true, false));
            puzzle.SetCell(new Coordinates(8, 3), new Core.Zelle(4, false, true));


            //spalte 5                    Änderbar, Schwarz
            puzzle.SetCell(new Coordinates(0, 4), new Core.Zelle(0, false, true));
            puzzle.SetCell(new Coordinates(1, 4), new Core.Zelle(0, true, false));
            puzzle.SetCell(new Coordinates(2, 4), new Core.Zelle(0, true, false));
            puzzle.SetCell(new Coordinates(3, 4), new Core.Zelle(0, true, false));
            puzzle.SetCell(new Coordinates(4, 4), new Core.Zelle(0, true, false));
            puzzle.SetCell(new Coordinates(5, 4), new Core.Zelle(0, true, false));
            puzzle.SetCell(new Coordinates(6, 4), new Core.Zelle(0, true, false));
            puzzle.SetCell(new Coordinates(7, 4), new Core.Zelle(0, true, false));
            puzzle.SetCell(new Coordinates(8, 4), new Core.Zelle(0, false, true));


            //spalte 6                    Änderbar, Schwarz
            puzzle.SetCell(new Coordinates(0, 5), new Core.Zelle(5, false, true));
            puzzle.SetCell(new Coordinates(1, 5), new Core.Zelle(0, true, false));
            puzzle.SetCell(new Coordinates(2, 5), new Core.Zelle(0, true, false));
            puzzle.SetCell(new Coordinates(3, 5), new Core.Zelle(9, false, true));
            puzzle.SetCell(new Coordinates(4, 5), new Core.Zelle(0, true, false));
            puzzle.SetCell(new Coordinates(5, 5), new Core.Zelle(0, true, false));
            puzzle.SetCell(new Coordinates(6, 5), new Core.Zelle(8, false, true));
            puzzle.SetCell(new Coordinates(7, 5), new Core.Zelle(2, false, false));
            puzzle.SetCell(new Coordinates(8, 5), new Core.Zelle(0, true, false));

            //spalte 7                    Änderbar, Schwarz
            puzzle.SetCell(new Coordinates(0, 6), new Core.Zelle(0, true, false));
            puzzle.SetCell(new Coordinates(1, 6), new Core.Zelle(0, true, false));
            puzzle.SetCell(new Coordinates(2, 6), new Core.Zelle(0, false, true));
            puzzle.SetCell(new Coordinates(3, 6), new Core.Zelle(0, true, false));
            puzzle.SetCell(new Coordinates(4, 6), new Core.Zelle(0, true, false));
            puzzle.SetCell(new Coordinates(5, 6), new Core.Zelle(7, false, true));
            puzzle.SetCell(new Coordinates(6, 6), new Core.Zelle(0, true, false));
            puzzle.SetCell(new Coordinates(7, 6), new Core.Zelle(0, true, false));
            puzzle.SetCell(new Coordinates(8, 6), new Core.Zelle(0, true, false));

            //spalte 8                    Änderbar, Schwarz
            puzzle.SetCell(new Coordinates(0, 7), new Core.Zelle(0, true, false));
            puzzle.SetCell(new Coordinates(1, 7), new Core.Zelle(0, true, false));
            puzzle.SetCell(new Coordinates(2, 7), new Core.Zelle(0, true, false));
            puzzle.SetCell(new Coordinates(3, 7), new Core.Zelle(0, true, false));
            puzzle.SetCell(new Coordinates(4, 7), new Core.Zelle(6, false, false));
            puzzle.SetCell(new Coordinates(5, 7), new Core.Zelle(0, true, false));
            puzzle.SetCell(new Coordinates(6, 7), new Core.Zelle(0, true, false));
            puzzle.SetCell(new Coordinates(7, 7), new Core.Zelle(0, true, false));
            puzzle.SetCell(new Coordinates(8, 7), new Core.Zelle(0, true, false));

            //spalte 9                    Änderbar, Schwarz
            puzzle.SetCell(new Coordinates(0, 8), new Core.Zelle(6, false, true));
            puzzle.SetCell(new Coordinates(1, 8), new Core.Zelle(0, true, false));
            puzzle.SetCell(new Coordinates(2, 8), new Core.Zelle(7, false, false));
            puzzle.SetCell(new Coordinates(3, 8), new Core.Zelle(0, false, true));
            puzzle.SetCell(new Coordinates(4, 8), new Core.Zelle(0, false, true));
            puzzle.SetCell(new Coordinates(5, 8), new Core.Zelle(5, false, false));
            puzzle.SetCell(new Coordinates(6, 8), new Core.Zelle(0, true, false));
            puzzle.SetCell(new Coordinates(7, 8), new Core.Zelle(0, true, false));
            puzzle.SetCell(new Coordinates(8, 8), new Core.Zelle(0, false, true));
        }
    }

}
