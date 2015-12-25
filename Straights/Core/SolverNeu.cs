using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Straights.Core
{
    /// <summary>
    /// Diese Klasse versucht eine Lösung eines Puzzles zu finden.
    /// (Funktionierender, erprobter Solver findet Spiele mit mind. einer Loesung)
    /// </summary>
    /// <remarks>
    /// Autoren:
    /// Andy Klay klay@fh-brandenburg.de,
    /// Frank Mertens mertens@fh-brandenburg.de
    /// </remarks>
    class SolverNeu
    {

        private Puzzle spiel;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="spiel"></param>
        public SolverNeu(Puzzle spiel)
        {
            this.spiel = spiel;
        }

        /// <summary>
        /// Diese Methode bereitet die Startkoordinaten vor und ruft den Solver auf.
        /// </summary>
        /// <returns>Rätsel ist lösbar oder nicht.</returns>
        public bool Solve()
        {
            return this.loeseSpielRekursiv(0, 0);
        }

        /// <summary>
        /// Löst das Puzzle.
        /// </summary>
        /// <param name="coords">Startkoordinaten, aber der die rekursive Suche beginnt. Normalerweise (0, 0).</param>
        /// <returns>Rätsel ist lösbar oder nicht.</returns>
        private bool loeseSpielRekursiv(uint aktX, uint aktY)
        {
            Zelle aktCell =spiel.GetCell(new Coordinates(aktX, aktY));
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
                            //Debug.WriteLine("Maximale Rekursionstiefe erreicht, Lösung gefunden.");
                            //System.Console.WriteLine("Zelle: x=" + aktX + ", y=" + aktY + ", V=" + aktCell.GetValue());
                            return true;
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
                    //Debug.WriteLine("Maximale Rekursionstiefe erreicht, Lösung gefunden.");
                    //System.Console.WriteLine("Zelle: x=" + aktX + ", y=" + aktY + ", V=" + aktCell.GetValue());
                    return true;
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

    }
}
