using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Straights.Serial;
using System.IO;

namespace Straights.Core
{
    /// <summary>
    /// Diese Klasse stellt die Generierungsmethode bereit
    /// (Generator vierter Generation)
    /// </summary>
    /// <remarks>
    /// Autor:
    /// Andy Klay klay@fh-brandenburg.de
    /// 
    /// Datum:
    /// 28.06.2011
    /// </remarks>
    public class GeneratorNeu
    {

        private Puzzle spiel;
        private SolverNeu vieleLoesungenSolver;
        //private SolverNurEineLoesung eineLoesungSolver;
        private Serialisator datei;
        private int gefundenCounter = 0;
        private readonly string GENERATEDIRECTORY = "generated";
        Random zufall = null;

        public GeneratorNeu()
        {
            this.spiel = new Puzzle();
            datei = new Serialisator();
            zufall = new Random();
            this.vieleLoesungenSolver = null;
            //this.eineLoesungSolver = null;
        }

        /// <summary>
        /// Generierung  neuer Spiele
        /// </summary>
        public void Generate()
        {
            uint anzahlSchwarz = 0;
            uint xSet = 0;
            uint ySet = 0;
            //Blau steht hier fuer die weißen nicht aenderbaren Felder
            uint anzahlBlau = 0;
            //uint value = 0;
            bool testFeld=false;
            bool loesbar=false;

            //testen ob Ordner existiert und falls nicht erstellen
            if (!Directory.Exists(System.Threading.Thread.GetDomain().BaseDirectory + "\\" + GENERATEDIRECTORY))
            {
                Directory.CreateDirectory(System.Threading.Thread.GetDomain().BaseDirectory + "\\" + GENERATEDIRECTORY);
            }

            while (true)
            {
                //Anzahl der schwarzen felder generieren
                //AENDERUNG(gemeint gegenueber Klasse Generator)
                //(habe suchraum erhöht>>> theor. max. ist 1 bis 80 schwarze Felder)
                //anzahlSchwarz = (uint)zufall.Next(4, 7);//vorher
                anzahlSchwarz = (uint)zufall.Next(3, 30);

                //Spiel initialisieren
                spiel = new Puzzle();

                //InitStandardStraights();

                //alle Felder initialisieren
                for (uint x = 0; x < 9; x++)
                {
                    for (uint y = 0; y < 9; y++)
                    {
                        spiel.SetCell(new Coordinates(x, y), new Zelle(0, true, false));
                    }
                }

                //AENDERUNG (gegenueber Klasse Generator)
                //(punktsymetrie ist zwar schoen aber verengt den Suchraum zusaetzlich)
                //dafuer jetzt zufaellige verteilung auf den feldern >>> groeßerer Raum an moegl. Loesungen wird durchprobiert

                //schwarze Blöcke verteilen
                for (int i = 0; i < anzahlSchwarz; i++)
                {
                    //koordinaten generieren
                    xSet = (uint)zufall.Next(0, 8);
                    ySet = (uint)zufall.Next(0, 8);

                    //feld setzen
                    spiel.SetCell(new Coordinates(xSet, ySet), new Zelle(0, false, true));
                }


                //erstes aussortieren
                testFeld = spiel.IsAllowedState();

                //wenn Regelkonform
                if (testFeld)
                {

                    Debug.WriteLine(" nutzbar");


                    //versuche das Spiel zu loesen
                    //AENDERUNG normaler Solver...soll überhaupt eine loesung finden
                    vieleLoesungenSolver = new SolverNeu(spiel);
                    loesbar = vieleLoesungenSolver.Solve();

                    //wenn loesbar
                    if (loesbar)
                    {
                        Debug.WriteLine(" LOESBAR!!! ");
                        //AENDERUNG
                        //es wurde ein konformes Spielfeld gefunden
                        //nun steht die loesung noch drin
                        //also setzen wir jetzt ein bis n felder davon fest
                        //und testen jetz darauf das es nur EINE loesung gibt mithilfe des anderen Solver

                        // 1 bis n festgesetzte felder>>> bestimmen im Endeffekt auch den schwierigkeitsgrad
                        anzahlBlau = (uint)zufall.Next(4, 10);

                        //name des spiels setzen
                        setzeName(anzahlBlau);

                        //felder festsetzen
                        while (anzahlBlau != 0)
                        {
                            //koordinaten generieren
                            xSet = (uint)zufall.Next(0, 8);
                            ySet = (uint)zufall.Next(0, 8);

                            if (spiel.GetCell(new Coordinates(xSet, ySet)).CHANGEABLE)
                            {
                                uint valueMemo = spiel.GetCell(new Coordinates(xSet, ySet)).Value;
                                //feld fest, nicht schwarz setzen (blaue zahl)
                                spiel.SetCell(new Coordinates(xSet, ySet), new Zelle(valueMemo, false, false));
                                anzahlBlau--;
                            }
                        }

                        //Loeschen der editierbaren felder
                        for (uint x = 0; x < 9; x++)
                        {
                            for (uint y = 0; y < 9; y++)
                            {
                                if (spiel.GetCell(new Coordinates(x, y)).CHANGEABLE) spiel.ChangeCell(new Coordinates(x, y), 0);
                            }
                        }


                        //jetzt testen ob loesung eindeutig (vorgesehn war die eindeutigkeits testung)
                        //aber diese führte zu keinem ergebnis>>> evtl. noch ein denkfehler im SolverNurEineLoesung
                        //daher normaler solver (solverZwo) zu zweiten ueberpruefung
                        loesbar = false;//zurücksetzen, sicherheitshalber falls seiteneffekt von vortest
                        vieleLoesungenSolver = new SolverNeu(spiel);
                        loesbar = vieleLoesungenSolver.Solve();

                        if (loesbar)
                        {
                            Debug.WriteLine(" STRAIGHTS GEFUNDEN!!! ");
                            //Debug.WriteLine("*******************");
                            //Print();
                            //Debug.WriteLine("*******************");


                            //Loeschen des Ergebnisses 
                            for (uint x = 0; x < 9; x++)
                            {
                                for (uint y = 0; y < 9; y++)
                                {
                                    if (spiel.GetCell(new Coordinates(x, y)).CHANGEABLE) spiel.ChangeCell(new Coordinates(x, y), 0);
                                }
                            }

                            ////Datei namen zusammsetzen mit Zeitstempel
                            string filename = string.Format(GENERATEDIRECTORY + "\\" + DateTime.Now.Day + "." + DateTime.Now.Month + "." + DateTime.Now.Year + "_" + DateTime.Now.Hour + "-" + DateTime.Now.Minute + "-" + DateTime.Now.Second + "_n{0:000000}.sts", gefundenCounter);

                            //Speichern
                            datei.SavePuzzleToFile(spiel, filename);

                            //gefundenes Straights zählen
                            gefundenCounter++;
                        }
                    }
                    else
                    {
                        Debug.WriteLine(" NICHT LOESBAR!!! ");
                    }

                }
                else
                {
                    Debug.WriteLine(" NICHT nutzbar ");
                }

            }
        }

        /// <summary>
        /// Setzt den Namen des spiels je nach zahl der Festen weißen zahlen
        /// </summary>
        /// <param name="schwierigkeit"></param>
        private void setzeName(uint zahlenFest)
        {
            switch (zahlenFest)
            {

                case 4: this.spiel.SetBezeichnung("sehrSchwer " + DateTime.Now.Ticks);
                    break;
                case 5: this.spiel.SetBezeichnung("schwer " + DateTime.Now.Ticks);
                    break;
                case 6: this.spiel.SetBezeichnung("schwer " + DateTime.Now.Ticks);
                    break;
                case 7: this.spiel.SetBezeichnung("mittel" + DateTime.Now.Ticks);
                    break;
                case 8: this.spiel.SetBezeichnung("leicht " + DateTime.Now.Ticks);
                    break;
                case 9: this.spiel.SetBezeichnung("leicht " + DateTime.Now.Ticks);
                    break;
                case 10: this.spiel.SetBezeichnung("sehrLeicht " + DateTime.Now.Ticks);
                    break;
            }
        }



        /// <summary>
        /// Referenz zum Testen (Standardstraights)
        /// </summary>
        public void InitStandardStraights()
        {
            //test straights fuer GEN 
            //Als Referenz zum testen ein echtes Str8ts- (das standarad straights)

            spiel.SetCell(new Coordinates(0, 0), new Core.Zelle(0, false, true));
            spiel.SetCell(new Coordinates(1, 0), new Core.Zelle(0, true, false));
            spiel.SetCell(new Coordinates(2, 0), new Core.Zelle(0, true, false));
            //spiel.SetCell(new Coordinates(3, 0), new Core.Zelle(3, false, false));
            spiel.SetCell(new Coordinates(3, 0), new Core.Zelle(0, true, false));
            spiel.SetCell(new Coordinates(4, 0), new Core.Zelle(0, false, true));
            spiel.SetCell(new Coordinates(5, 0), new Core.Zelle(0, false, true));
            //spiel.SetCell(new Coordinates(6, 0), new Core.Zelle(7, false, false));
            spiel.SetCell(new Coordinates(6, 0), new Core.Zelle(0, true, false));
            spiel.SetCell(new Coordinates(7, 0), new Core.Zelle(0, true, false));
            spiel.SetCell(new Coordinates(8, 0), new Core.Zelle(0, false, true));

            //spalte 2                    Änderbar, Schwarz
            spiel.SetCell(new Coordinates(0, 1), new Core.Zelle(0, true, false));
            spiel.SetCell(new Coordinates(1, 1), new Core.Zelle(0, true, false));
            spiel.SetCell(new Coordinates(2, 1), new Core.Zelle(0, true, false));
            spiel.SetCell(new Coordinates(3, 1), new Core.Zelle(0, true, false));
            //spiel.SetCell(new Coordinates(4, 1), new Core.Zelle(2, false, false));
            spiel.SetCell(new Coordinates(4, 1), new Core.Zelle(0, true, false));
            spiel.SetCell(new Coordinates(5, 1), new Core.Zelle(0, true, false));
            spiel.SetCell(new Coordinates(6, 1), new Core.Zelle(0, true, false));
            spiel.SetCell(new Coordinates(7, 1), new Core.Zelle(0, true, false));
            spiel.SetCell(new Coordinates(8, 1), new Core.Zelle(0, true, false));

            //spalte 3                    Änderbar, Schwarz
            spiel.SetCell(new Coordinates(0, 2), new Core.Zelle(0, true, false));
            spiel.SetCell(new Coordinates(1, 2), new Core.Zelle(0, true, false));
            spiel.SetCell(new Coordinates(2, 2), new Core.Zelle(0, true, false));
            spiel.SetCell(new Coordinates(3, 2), new Core.Zelle(1, false, true));
            spiel.SetCell(new Coordinates(4, 2), new Core.Zelle(0, true, false));
            spiel.SetCell(new Coordinates(5, 2), new Core.Zelle(0, true, false));
            spiel.SetCell(new Coordinates(6, 2), new Core.Zelle(0, false, true));
            spiel.SetCell(new Coordinates(7, 2), new Core.Zelle(0, true, false));
            spiel.SetCell(new Coordinates(8, 2), new Core.Zelle(0, true, false));


            //spalte 4                    Änderbar, Schwarz
            spiel.SetCell(new Coordinates(0, 3), new Core.Zelle(0, true, false));
            //spiel.SetCell(new Coordinates(1, 3), new Core.Zelle(2, false, false));
            spiel.SetCell(new Coordinates(1, 3), new Core.Zelle(0, true, false));
            spiel.SetCell(new Coordinates(2, 3), new Core.Zelle(0, false, true));
            spiel.SetCell(new Coordinates(3, 3), new Core.Zelle(0, true, false));
            spiel.SetCell(new Coordinates(4, 3), new Core.Zelle(0, true, false));
            spiel.SetCell(new Coordinates(5, 3), new Core.Zelle(0, false, true));
            spiel.SetCell(new Coordinates(6, 3), new Core.Zelle(0, true, false));
            spiel.SetCell(new Coordinates(7, 3), new Core.Zelle(0, true, false));
            spiel.SetCell(new Coordinates(8, 3), new Core.Zelle(4, false, true));


            //spalte 5                    Änderbar, Schwarz
            spiel.SetCell(new Coordinates(0, 4), new Core.Zelle(0, false, true));
            spiel.SetCell(new Coordinates(1, 4), new Core.Zelle(0, true, false));
            spiel.SetCell(new Coordinates(2, 4), new Core.Zelle(0, true, false));
            spiel.SetCell(new Coordinates(3, 4), new Core.Zelle(0, true, false));
            spiel.SetCell(new Coordinates(4, 4), new Core.Zelle(0, true, false));
            spiel.SetCell(new Coordinates(5, 4), new Core.Zelle(0, true, false));
            spiel.SetCell(new Coordinates(6, 4), new Core.Zelle(0, true, false));
            spiel.SetCell(new Coordinates(7, 4), new Core.Zelle(0, true, false));
            spiel.SetCell(new Coordinates(8, 4), new Core.Zelle(0, false, true));


            //spalte 6                    Änderbar, Schwarz
            spiel.SetCell(new Coordinates(0, 5), new Core.Zelle(5, false, true));
            spiel.SetCell(new Coordinates(1, 5), new Core.Zelle(0, true, false));
            spiel.SetCell(new Coordinates(2, 5), new Core.Zelle(0, true, false));
            spiel.SetCell(new Coordinates(3, 5), new Core.Zelle(9, false, true));
            spiel.SetCell(new Coordinates(4, 5), new Core.Zelle(0, true, false));
            spiel.SetCell(new Coordinates(5, 5), new Core.Zelle(0, true, false));
            spiel.SetCell(new Coordinates(6, 5), new Core.Zelle(8, false, true));
            //spiel.SetCell(new Coordinates(7, 5), new Core.Zelle(2, false, false));
            spiel.SetCell(new Coordinates(7, 5), new Core.Zelle(0, true, false));
            spiel.SetCell(new Coordinates(8, 5), new Core.Zelle(0, true, false));

            //spalte 7                    Änderbar, Schwarz
            spiel.SetCell(new Coordinates(0, 6), new Core.Zelle(0, true, false));
            spiel.SetCell(new Coordinates(1, 6), new Core.Zelle(0, true, false));
            spiel.SetCell(new Coordinates(2, 6), new Core.Zelle(0, false, true));
            spiel.SetCell(new Coordinates(3, 6), new Core.Zelle(0, true, false));
            spiel.SetCell(new Coordinates(4, 6), new Core.Zelle(0, true, false));
            spiel.SetCell(new Coordinates(5, 6), new Core.Zelle(7, false, true));
            spiel.SetCell(new Coordinates(6, 6), new Core.Zelle(0, true, false));
            spiel.SetCell(new Coordinates(7, 6), new Core.Zelle(0, true, false));
            spiel.SetCell(new Coordinates(8, 6), new Core.Zelle(0, true, false));

            //spalte 8                    Änderbar, Schwarz
            spiel.SetCell(new Coordinates(0, 7), new Core.Zelle(0, true, false));
            spiel.SetCell(new Coordinates(1, 7), new Core.Zelle(0, true, false));
            spiel.SetCell(new Coordinates(2, 7), new Core.Zelle(0, true, false));
            spiel.SetCell(new Coordinates(3, 7), new Core.Zelle(0, true, false));
            spiel.SetCell(new Coordinates(4, 7), new Core.Zelle(6, false, false));
            spiel.SetCell(new Coordinates(5, 7), new Core.Zelle(0, true, false));
            spiel.SetCell(new Coordinates(6, 7), new Core.Zelle(0, true, false));
            spiel.SetCell(new Coordinates(7, 7), new Core.Zelle(0, true, false));
            spiel.SetCell(new Coordinates(8, 7), new Core.Zelle(0, true, false));

            //spalte 9                    Änderbar, Schwarz
            spiel.SetCell(new Coordinates(0, 8), new Core.Zelle(6, false, true));
            spiel.SetCell(new Coordinates(1, 8), new Core.Zelle(0, true, false));
            //spiel.SetCell(new Coordinates(2, 8), new Core.Zelle(7, false, false));
            spiel.SetCell(new Coordinates(2, 8), new Core.Zelle(0, true, false));
            spiel.SetCell(new Coordinates(3, 8), new Core.Zelle(0, false, true));
            spiel.SetCell(new Coordinates(4, 8), new Core.Zelle(0, false, true));
            //spiel.SetCell(new Coordinates(5, 8), new Core.Zelle(5, false, false));
            spiel.SetCell(new Coordinates(5, 8), new Core.Zelle(0, true, false));
            spiel.SetCell(new Coordinates(6, 8), new Core.Zelle(0, true, false));
            spiel.SetCell(new Coordinates(7, 8), new Core.Zelle(0, true, false));
            spiel.SetCell(new Coordinates(8, 8), new Core.Zelle(0, false, true));

        }

    }
}
