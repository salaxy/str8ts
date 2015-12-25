using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace Straights.Core
{
    /// <summary>
    /// Diese Klasse repräsentiert ein Str8ts-Spiel und stellt Methoden zu Manipulation und Regelprüfung dieses bereit.
    /// (MVC Model)
    /// </summary>
    /// <remarks>Autoren:
    /// Andy Klay klay@fh-brandenburg.de,
    /// Frank Mertens mertens@fh-brandenburg.de
    /// </remarks>
    [Serializable()] 
    public class Puzzle
    {
        private Zelle[,] puzzle; // Array, dass alle Zellen des Puzzles beinhaltet.
        private int playtime; //bisherige spielZeit in sekunden
        private string bezeichnung;

        /// <summary>
        /// Legt ein neues Puzzle-Objekt an.
        /// </summary>
        public Puzzle()
        {
            puzzle = new Zelle[9, 9];
            playtime = 0;
            bezeichnung = "StartGame";
        }

        /// <summary>
        /// Gibt das das komplette Puzzle als zweidimensionales Array aus Zelle-Objekten zurück.
        /// </summary>
        /// <returns></returns>
        public Zelle[,] Cells
        {
            get {return puzzle; }
        }

        /// <summary>
        /// Ändert die Zelle an den angegebenen Koordinaten auf die neue Zelle.
        /// </summary>
        /// <param name="coords"></param>
        /// <param name="zelle"></param>
        public void SetCell(Coordinates coords, Zelle zelle)
        {
            if (coords.AreValid()) // Koordinaten- und Objektüberprüfung
                puzzle[coords.x, coords.y] = zelle.Clone();
        }

        /// <summary>
        /// Gibt die Zelle an den angegebenen Koordinaten zurück.
        /// </summary>
        /// <param name="coords"></param>
        /// <returns></returns>
        public Zelle GetCell(Coordinates coords)
        {
            return puzzle[coords.x, coords.y];
        }

        /// <summary>
        /// Ändert den Wert in einem Feld (sofern erlaubt).
        /// </summary>
        /// <param name="coords"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool ChangeCell(Coordinates coords, uint value)
        {
            if (coords.AreValid()) // Koordinaten überprüfen
            {
                puzzle[coords.x, coords.y].Value = value;
                return IsAllowedValue(value) && puzzle[coords.x, coords.y].CHANGEABLE; // Rückgeben, ob der neue Wert im Rätsel gegen Spielregeln verstößt.
            }
            
            return false;
        }

        /// <summary>
        /// Startet eine komplette Spielregelnprüfng des gesamten Rätsels.
        /// </summary>
        /// <remarks>Unvollständige Rätsel können regelkonform sein!</remarks>
        /// <returns>Spiel ist regelkonform oder nicht.</returns>
        public bool IsAllowedState()
        { 
            // Alle Spalten und danach...
            for (uint x = 0; x < 9; x++)
            {
                //Debug.WriteLine(bezeichnung + " Prüfe Column " + x);
                if (!IsAllowedColumn(x))
                {
                    return false;
                }
            } 
            
            // alle Zeilen überprüfen.
            for (uint y = 0; y < 9; y++)
            {
                //Debug.WriteLine(bezeichnung + " Prüfe row " + y);
                if (!IsAllowedRow(y))
                {
                    return false;
                }
            }

            return true; // Nirgendswo wurde eine Regelverletzung festgestellt.
        }
       
        /// <summary>
        /// Überprüft eine Zeile auf Regelverstöße.
        /// </summary>
        /// <param name="y">Ordinate der Zeile</param>
        /// <returns></returns>
        public bool IsAllowedRow(uint y)
        {
            List<uint> usedValues = new List<uint>(9);  // Liste, vorkommender Zahlen.
            uint value;                                 // Wert in einer Zelle
            bool lastOneWasSolid = true;                // Vorherige Zelle war schwarz
            Coordinates coords = new Coordinates(0, y); // Koordinaten des aktuellen Felds

            while (coords.AreValid())
            {
                value = puzzle[coords.x, coords.y].Value;

                // Suche nach doppelt eingegeben Zahlen
                if (value != 0) // Wenn in der Zelle eine Zahl drin steht...
                {
                    if (!usedValues.Contains(value)) // ..überprüfe, ob sie in der Liste bereits verwendeter Zahlen steht...
                        usedValues.Add(value); // ...und falls nicht, nimm sie in der Liste auf.
                    
                    else // Eine doppelte Zahl wurde gefunden.
                        return false; // Regelverstoß erkannt.

                }

                if (lastOneWasSolid && !puzzle[coords.x, coords.y].SOLID) // Wenn das vorherige Feld schwarz war und das aktelle nicht,
                                                                          // dann ist das der Beginn einer Straße.
                {
                    if (!IsAllowedStreetRow(coords)) // Überprüfe, ob die gefundene Straße gültig ist.
                        return false;
                }

                // Falls ein schwarzes Feld gefunden wurde, merke es!
                lastOneWasSolid = puzzle[coords.x, coords.y].SOLID ? true : false;

                coords = coords.GetNextCoordinatesInRow(); // Koordinaten vom nächsten Feld in der Zeile bestimmen.
            }

            return true; // Keine Regelverstöße gefunden.
        }

        /// <summary>
        /// Überprüft eine Spalte auf Regelverstöße.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public bool IsAllowedColumn(uint x)
        {
            List<uint> usedValues = new List<uint>(9);  // Liste, vorkommender Zahlen.
            uint value;                                 // Wert in einer Zelle
            bool lastOneWasSolid = true;                // Vorherige Zelle war schwarz
            Coordinates coords = new Coordinates(x, 0); // Koordinaten des aktuellen Felds

            while (coords.AreValid())
            {
                value = puzzle[coords.x, coords.y].Value;

                // Suche nach doppelt eingegeben Zahlen
                if (value != 0) // Wenn in der Zelle eine Zahl drin steht...
                {
                    if (!usedValues.Contains(value)) // ..überprüfe, ob sie in der Liste bereits verwendeter Zahlen steht...
                        usedValues.Add(value); // ...und falls nicht, nimm sie in der Liste auf.

                    else // Eine doppelte Zahl wurde gefunden.
                    {
                        return false; // Regelverstoß erkannt.
                    }
                }

                if (lastOneWasSolid && !puzzle[coords.x, coords.y].SOLID) // Wenn daas vorherige Feld schwarz war und das aktelle nicht,
                                                                          // dann ist das der Beginn einer Straße.
                {
                    if (!IsAllowedStreetColumn(coords))  // Überprüfe, ob die gefundene Straße gültig ist.
                        return false;
                }

                // Falls ein schwarzes Feld gefunden wurde, merke es!
                lastOneWasSolid = puzzle[coords.x, coords.y].SOLID ? true : false;

                coords = coords.GetNextCoordinatesInColumn(); // Koordinaten vom nächsten Feld in der Zeile bestimmen.
            }

            return true; // Keine Regelverstöße gefunden.
        }

        /// <summary>
        /// Überprüft, ob die Straße in einer Zeile regelkonform ist.
        /// </summary>
        /// <param name="coords"></param>
        /// <returns></returns>
        public bool IsAllowedStreetRow(Coordinates _coords)
        {
            List<uint> usedValuesList = new List<uint>(); // Liste verwendeter Zahlen
            bool streetComplete = true;                // Die Startvermutung ist, dass alle Felder einer Straße ausgefüllt wurden.
            Coordinates coords = _coords.Clone();         // Koordinaten des aktuellen Felds

            while (coords.AreValid())
            {
                if (!puzzle[coords.x, coords.y].SOLID) // Wenn das Feld kein schwarzes ist...
                {
                    if (puzzle[coords.x, coords.y].Value != 0) // ...und nicht leer ist
                        usedValuesList.Add(puzzle[coords.x, coords.y].Value);

                    else
                        streetComplete = false;

                }
                else
                {
                    break;
                }

                coords = coords.GetNextCoordinatesInRow(); // Koordinaten vom nächsten Feld in der Zeile bestimmen.
            }

            if (streetComplete) // Wenn alle Daten der Straße bekannt sind
            {
                if (!IsContinousStreet(usedValuesList))
                    return false;
            }

            return true; // Straße ist regelkonform
        }

        /// <summary>
        /// Überprüft, ob die Straße in einer Reihe regelkonform ist.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool IsAllowedStreetColumn(Coordinates _coords)
        {
            List<uint> usedValuesList = new List<uint>(); // Liste verwendeter Zahlen
            bool streetComplete = true;                // Die Startvermutung ist, dass alle Felder einer Straße ausgefüllt wurden.
            Coordinates coords = _coords.Clone();         // Koordinaten des aktuellen Felds

            while (coords.AreValid())
            {
                if (!puzzle[coords.x, coords.y].SOLID) // Wenn das Feld kein schwarzes ist...
                {
                    if (puzzle[coords.x, coords.y].Value != 0) // ...und nicht leer ist
                        usedValuesList.Add(puzzle[coords.x, coords.y].Value); // Merken, um später Zahlenreihe zu überprüfen.

                    else
                        streetComplete = false;

                }
                else // Schwarzes Feld gefunden -> Ende der Straße erreicht.
                {
                    break;
                }

                coords = coords.GetNextCoordinatesInColumn(); // Koordinaten vom nächsten Feld in der Zeile bestimmen.
            }

            if (streetComplete) // Wenn alle Daten der Straße bekannt sind
            {
                if (!IsContinousStreet(usedValuesList))
                    return false;
            }

            return true; // Straße ist regelkonform
        }

        /// <summary>
        /// Diese Methode überprüft, ob die Zahlen in einer Liste eine ununterbrochene Reihe bilden.
        /// </summary>
        /// <param name="usedValuesList"></param>
        /// <returns></returns>
        private bool IsContinousStreet(List<uint> usedValuesList)
        {
            //Liste zum Array wandeln und sortieren
            uint[] usedValuesArray = usedValuesList.ToArray();
            Array.Sort(usedValuesArray);

            // Überprüfen, ob die Zahlen eine ununterbrochene Reihe bilden.
            // Dabei wird geschaut, ob der Vorgänger und der aktuelle Werte eine Differenz von +1 haben.
            uint differenz = 0;

            for (int i = 0; i < usedValuesArray.Length; i++)
            {
                if (i == 0)
                {
                    differenz = 0;
                }
                else
                {
                    differenz = usedValuesArray[i] - usedValuesArray[i - 1];
                }

                // Die Differenz ist nicht +1 -> die Reihe ist nicht ununterbrochen
                if (differenz > 1)
                {
                    return false;
                }
            }

            return true; // Zahlen bilden eine ununterbrochene Reihe
        }

        /// <summary>
        /// Überprüft, ob der Feldwert innerhalb des 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool IsAllowedValue(uint value)
        {
            return value <= 9 && value >= 0;
        }

        /// <summary>
        /// Fragt ab ob Spielfeld voellig ausgefüllt ist
        /// </summary>
        /// <returns></returns>
        public bool IsAusgefuellt()
        {

            foreach(Zelle z in this.Cells){

                if (z.CHANGEABLE && z.Value == 0)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Setzen der Spielzeit
        /// </summary>
        /// <param name="sec"></param>
        public void SetPlaytime(int sec)
        {
            this.playtime = sec;
        }

        /// <summary>
        /// holen der Spielzeit
        /// </summary>
        /// <returns></returns>
        public int GetPlaytime()
        {
            return this.playtime;
        }

        /// <summary>
        /// Holen der Bezeichnung
        /// </summary>
        /// <returns>string</returns>
        public string GetBezeichnung()
        {
            return this.bezeichnung;
        }

        /// <summary>
        /// setzen der Bezeichnung
        /// </summary>
        /// <param name="bezeichnung"></param>
        public void SetBezeichnung(string bezeichnung)
        {
            this.bezeichnung = bezeichnung;

        }
    }
}