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
    /// (Generator dritter Generation)
    /// </summary>
    /// <remarks>Autoren:
    /// Andy Klay klay@fh-brandenburg.de,
    /// Frank Mertens mertens@fh-brandenburg.de
    /// </remarks>
	public class GeneratorAlt
	{

		private Puzzle spiel;
        private SolverNurEineLoesung solver;
		private Serialisator datei;
		private int gefundenCounter = 0;
        private readonly string GENERATEDIRECTORY="generated";
        Random zufall = null;

		public GeneratorAlt()
		{
			this.spiel = new Puzzle();
			datei = new Serialisator();
            zufall = new Random();
			this.solver = null;
		}

        /// <summary>
        /// Generierung  neuer Spiele
        /// </summary>
		public void Generate()
		{       
            uint anzahlSchwarz = 0;
            uint xSet = 0;
            uint ySet = 0;
            uint temp = 0;
            uint value = 0;
            bool testFeld;
            bool loesbar;

            //testen ob Ordner existiert und falls nicht erstellen
            if (!Directory.Exists(System.Threading.Thread.GetDomain().BaseDirectory + "\\" + GENERATEDIRECTORY))
            {
                Directory.CreateDirectory(System.Threading.Thread.GetDomain().BaseDirectory + "\\" + GENERATEDIRECTORY);
            }

            while (true)
            {
                //Anzahl der schwarzen felder generieren
                anzahlSchwarz = (uint)zufall.Next(4, 7);

                //Spiel initialisieren
				spiel = new Puzzle();

                //alle Felder initialisieren
                for (uint x = 0; x < 9; x++)
                {
                    for (uint y = 0; y < 9; y++)
                    {
                        spiel.SetCell(new Coordinates(x, y), new Zelle(0, true, false));
                    }
                }


                // schwarze Blöcke verteilen
                for (uint i = 0; i < anzahlSchwarz; i++)
                {

                    //koordinaten generieren fuers erste Feld
                    do
                    {
                        xSet = (uint)zufall.Next(0, 5);
                        ySet = (uint)zufall.Next(0, 4);
                    } while (spiel.GetCell(new Coordinates(xSet, ySet)).SOLID);


                    spiel.SetCell(new Coordinates(xSet, ySet), new Zelle(value, false, true));

                    // punktsymmetrische Felder zum ersten
                    for (uint j = 1; j <= 3; j++)
                    {
                        temp = ySet;
                        ySet = xSet;
                        xSet = 8 - temp;
                        spiel.SetCell(new Coordinates(xSet, ySet), new Zelle(value, false, true));
                    }
                }

			
                //erstes aussortieren
                testFeld = spiel.IsAllowedState();

                //wenn Regelkonform
                if (testFeld)
                {

                    Debug.WriteLine(" nutzbar");


					//versuche das Spiel zu loesen
                    solver = new SolverNurEineLoesung(spiel);
                    loesbar = solver.Solve();

                    //wenn loesbar
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
        /// GenerierungsMuster 1, zufälliges setzen von schwarzen Feldern beliebiger anzahl
        /// </summary>
        /// <param name="anzahlSchwarz"></param>
        public void EntwurfsMethodeEins(uint anzahlSchwarz)
        {
            uint xSet = 0;
            uint ySet = 0;
            uint value = 0;


            //Entwurfs methode
            for (int i = 0; i < anzahlSchwarz; i++)
            {
                //koordinaten generieren
                xSet = (uint)zufall.Next(0, 8);
                ySet = (uint)zufall.Next(0, 8);

                value = 0;

                ////soll schwarzer block geflüllt sein
                //if (zufall.Next(1, 8) > 5)
                //{
                //    value = (uint)zufall.Next(0, 9);

                //}

                //feld setzen
                spiel.SetCell(new Coordinates(xSet, ySet), new Zelle(value, false, true));
            }

        }

        /// <summary>
        /// GenerierungsMuster 2, zufälliges setzen von schwarzen Feldern Spaltenweise in beliebiger anzahl
        /// </summary>
        /// <param name="anzahlSchwarz"></param>
        public void EntwurfsMethodeZwo()
        {
            //weitere Entwurfsmethode
            //durchlaufe alle Reihen
            for (uint x = 0; x < 9; x++)
            {
                //wie viele schwarze Felder in der Spalte sollen gesetzt werden?
                uint zufallZahl = (uint)zufall.Next(0, 3);

                //setze alle schwarzen felder an einer zufälligen Stelle der Spalte
                for (uint i = 0; i < zufallZahl; i++)
                {
                    //zufällige Reihen-Koordinate
                    uint yCoord = (uint)zufall.Next(0, 8);
                    spiel.SetCell(new Coordinates(x, yCoord), new Zelle(0, false, true));
                }

            }

        }



        /// <summary>
        /// Referenz zum Testen (Standardstraights)
        /// </summary>
        public void InitStandardStraights(){

            //Als Referenz zum testen ein echtes Str8ts- (das standarad straights)

            spiel.SetCell(new Coordinates(0, 0), new Core.Zelle(0, false, true));
            spiel.SetCell(new Coordinates(1, 0), new Core.Zelle(0, true, false));
            spiel.SetCell(new Coordinates(2, 0), new Core.Zelle(0, true, false));
            spiel.SetCell(new Coordinates(3, 0), new Core.Zelle(3, false, false));
            spiel.SetCell(new Coordinates(4, 0), new Core.Zelle(0, false, true));
            spiel.SetCell(new Coordinates(5, 0), new Core.Zelle(0, false, true));
            spiel.SetCell(new Coordinates(6, 0), new Core.Zelle(7, false, false));
            spiel.SetCell(new Coordinates(7, 0), new Core.Zelle(0, true, false));
            spiel.SetCell(new Coordinates(8, 0), new Core.Zelle(0, false, true));

            //spalte 2                    Änderbar, Schwarz
            spiel.SetCell(new Coordinates(0, 1), new Core.Zelle(0, true, false));
            spiel.SetCell(new Coordinates(1, 1), new Core.Zelle(0, true, false));
            spiel.SetCell(new Coordinates(2, 1), new Core.Zelle(0, true, false));
            spiel.SetCell(new Coordinates(3, 1), new Core.Zelle(0, true, false));
            spiel.SetCell(new Coordinates(4, 1), new Core.Zelle(2, false, false));
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
            spiel.SetCell(new Coordinates(1, 3), new Core.Zelle(2, false, false));
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
            spiel.SetCell(new Coordinates(7, 5), new Core.Zelle(2, false, false));
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
            spiel.SetCell(new Coordinates(2, 8), new Core.Zelle(7, false, false));
            spiel.SetCell(new Coordinates(3, 8), new Core.Zelle(0, false, true));
            spiel.SetCell(new Coordinates(4, 8), new Core.Zelle(0, false, true));
            spiel.SetCell(new Coordinates(5, 8), new Core.Zelle(5, false, false));
            spiel.SetCell(new Coordinates(6, 8), new Core.Zelle(0, true, false));
            spiel.SetCell(new Coordinates(7, 8), new Core.Zelle(0, true, false));
            spiel.SetCell(new Coordinates(8, 8), new Core.Zelle(0, false, true));
        }


        //public void Print()
        //{
        //    for (uint y = 0; y < 9; y++)
        //    {
        //        for (uint x = 0; x < 9; x++)
        //        {
        //            Debug.Write(spiel.bezeichnung + " [" + spiel.GetCell(new Coordinates(x, y)).Value + "|" + spiel.GetCell(new Coordinates(x, y)).CHANGEABLE + "|" + spiel.GetCell(new Coordinates(x, y)).SOLID + "] ");
        //        }
        //        Debug.Write("\n");
        //    }
        //    Debug.Write("\n");

	}
}
