using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Straights.Core
{
    /// <summary>
    /// Diese Klasse versucht eine Lösung eines Puzzles zu finden.
    /// (findet Spiele die GENAU eine Loesung haben)
    /// </summary>
    /// <remarks>
    /// Autoren:
    /// Andy Klay klay@fh-brandenburg.de,
    /// Frank Mertens mertens@fh-brandenburg.de
    /// </remarks>
    class SolverNurEineLoesung
    {

        private Puzzle spiel;
        private uint anzahlLoesungen=0;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="spiel"></param>
        public SolverNurEineLoesung(Puzzle spiel)
        {
            this.spiel = spiel;
        }

        /// <summary>
        /// Diese Methode bereitet die Startkoordinaten vor und ruft den Solver auf.
        /// </summary>
        /// <returns>Rätsel ist lösbar oder nicht.</returns>
        public bool Solve()
        {
            //return this.loeseSpielRekursiv(0, 0);//ursprüngl. anweisung

            loeseSpielRekursiv(0, 0);//loesung2


            if (anzahlLoesungen == 1)//loesung2
            {
                return true;//loesung2
            }
            else
            {
                return false;//loesung2
            }
        }

        /// <summary>
        /// Löst das Puzzle.
        /// </summary>
        /// <param name="coords">Startkoordinaten, aber der die rekursive Suche beginnt. Normalerweise (0, 0).</param>
        /// <returns>Rätsel ist lösbar oder nicht.</returns>
        private bool loeseSpielRekursiv(uint aktX, uint aktY)
        {
            Zelle aktCell = spiel.GetCell(new Coordinates(aktX, aktY));
            //this.Print(); 

            if (aktCell.CHANGEABLE)
            {
                //zelle durchprobieren
                for (uint i = 1; i <= 9; i++)
                {
                    //zelle setten
                    aktCell.Value = i;

                    //erlaubter zustand?
                    if (spiel.IsAllowedState())
                    {
                        uint x = aktX;
                        uint y = aktY;

                        //ende schon erreicht?
                        if (x == 8 && y == 8)
                        {
                            //return true;
                            //HIER SETZT DIE MODIFIKATION ZUR EINDEUTIGEN LOESUNG EIN
                            //Die Tiefensuche wird nun erneut losgeschickt und der loesungsraum weiter durchsucht
                            //return true;//ursprüngliche anweiseung
                            //loeseWeiter();//erste Idee zur eindeutigen loesung
                            //viel einfachere Loesung ... loesung2
                            this.anzahlLoesungen++;//loesung2
                            if (this.anzahlLoesungen > 1)//loesung2
                            {                                
                                //abbrechen weil zu viele loesungen
                                return true;//loesung2                                
                            }
                            else
                            {
                                //sonst suche weiter
                                return false;//loesung2
                            }

                        }

                        //ein feld weiter wandern
                        if (x < 8)
                        {
                            x++;
                        }
                        else
                        {
                            if (y < 8)
                            {
                                y++;
                                x = 0;
                            }
                        }

                        //tiefer in der rekursion
                        if (loeseSpielRekursiv(x, y))
                        {
                            //System.Console.WriteLine("Zelle: x=" + aktX + ", y=" + aktY + ", V=" + aktCell.GetValue());
                            return true;
                        }

                    } // if (spiel.IsAllowedState())
                }
                //loeschen des rekursionschrittes beim scheitern
                aktCell.Value = 0;

                return false;
            }
            else // if (aktCell.CHANGEABLE)
            {
                //naechste zelle
                uint x = aktX;
                uint y = aktY;

                //ende schon erreicht?
                if (x == 8 && y == 8)
                {
                    //HIER SETZT DIE MODIFIKATION ZUR EINDEUTIGEN LOESUNG EIN
                    //Die Tiefensuche wird nun erneut losgeschickt und der loesungsraum weiter durchsucht
                    //return true;//ursprüngliche anweiseung
                    //loeseWeiter();//erste Idee zur eindeutigen loesung
                    //viel einfachere Loesung ... loesung2
                    this.anzahlLoesungen++;//loesung2
                    if (this.anzahlLoesungen > 1)//loesung2
                    {
                        //abbrechen weil zu viele loesungen
                        return true;//loesung2                                
                    }
                    else
                    {
                        //sonst suche weiter
                        return false;//loesung2
                    }
                }
                else
                {
                    //ein feld weiter wandern
                    if (x < 8)
                    {
                        x++;
                    }
                    else
                    {
                        if (y < 8)
                        {
                            y++;
                            x = 0;
                        }
                    }

                    //System.Console.WriteLine("Zelle: x=" + aktX + ", y=" + aktY + ", V=" + aktCell.GetValue());
                    return loeseSpielRekursiv(x, y);
                }


            }

        }


        public void Print()
        {

            for (uint y = 0; y < 9; y++)
            {
                for (uint x = 0; x < 9; x++)
                {
                    Debug.Write(spiel.GetCell(new Coordinates(x, y)).Value);
                }
                Debug.Write("\n");
            }
            Debug.Write("\n");


        }


        //erste Idee zur eindeutigen loesung
        public void loeseWeiter(){
            this.anzahlLoesungen++;

            //suche letzte Zelle die CHANGABLE ist und 
            //wenn value!=9 dann value++;
            //sonst suche die vorletze Zelle die CHANGABLE ist und wenn value!=9 dann value++;
            //usw...
            //damit wird der Algorithmus wieder losgeschickt

            //aber es gibt noch eine viel einfachere Variante :)

        }

    }
}
