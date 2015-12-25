using System;
namespace Straights.Core
{
    /// <summary>
    /// Diese Struktur repräsentiert die Zeile-/Reihe-Koordinaten einer Zelle.
    /// </summary>
    /// <remarks>Autoren:
    /// Andy Klay klay@fh-brandenburg.de,
    /// Frank Mertens mertens@fh-brandenburg.de
    /// </remarks>
    [Serializable()]
    public struct Coordinates
    {
        public uint x;  // aktuelle Koordinaten
        public uint y;

        /// <summary>
        /// Legt eine Koordinatenstruktur an.
        /// </summary>
        /// <param name="x">Spalte</param>
        /// <param name="y">Zeile</param>
        public Coordinates(uint x, uint y)
        {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// Gibt die nächsten gültigen Koordinaten im Rätsel zurück.
        /// Es erfolgt keine Prüfung, ob das Ende des Rätsels erreicht wurde.
        /// </summary>
        /// <returns>Darauffolgende Koordinaten</returns>
        public Coordinates GetNextCoordinates()
        {
            if (!AreLastCoordinatesInRow()) // Gibt es noch eine weitere Spalte?
                return new Coordinates(x+1, y);

            else // Nein, also in die nächste Zeile wechseln.
                return new Coordinates(0, y+1);
        }

        /// <summary>
        /// Gibt die nächsten Koordinaten rechts von den aktuellen zurück.
        /// </summary>
        /// <returns></returns>
        public Coordinates GetNextCoordinatesInRow()
        {
            return new Coordinates(x+1, y);
        }

        /// <summary>
        /// Gibt die nächsten Koordinaten unterhalb der aktuellen zurück.
        /// </summary>
        /// <returns></returns>
        public Coordinates GetNextCoordinatesInColumn()
        {
            return new Coordinates(x, y+1);
        }

        /// <summary>
        /// Überprüft, ob die aktuellen Koordinaten, die letzte Zeile und Spalte im Rätsel repräsentieren.
        /// </summary>
        /// <returns></returns>
        public bool AreLastCoordinates()
        {
            return AreLastCoordinatesInRow() && AreLastCoordinatesInColumn();
        }

        /// <summary>
        /// Überprüft, ob die aktuellen Koordinaten am Ende einer Zeile sind.
        /// </summary>
        /// <returns></returns>
        public bool AreLastCoordinatesInRow()
        {
            return x == 8;
        }

        /// <summary>
        /// Überprüft, ob die aktuellen Koordinaten am Ende einer Spalte sind.
        /// </summary>
        /// <returns></returns>
        public bool AreLastCoordinatesInColumn()
        {
            return y == 8;
        }

        /// <summary>
        /// Überprüft, ob die Koordinaten innerhalb des gültigen Bereichs liegen ([0,0] bis [8,8]).
        /// </summary>
        /// <returns></returns>
        public bool AreValid()
        {
            return x < 9 && y < 9;
        }

        /// <summary>
        /// Gibt die aktuellen Koordinaten als neues Objekt zurück.
        /// </summary>
        /// <returns></returns>
        public Coordinates Clone()
        {
            return new Coordinates(x, y);
        }
    }
}