using System;
namespace Straights.Core
{
    /// <summary>
    /// Diese Klasse beinhaltet den Wert einer Zelle eines Rätsels.
    /// </summary>
    /// <remarks>Autoren:
    /// Andy Klay klay@fh-brandenburg.de,
    /// Frank Mertens mertens@fh-brandenburg.de
    /// </remarks>
    [Serializable()] 
    public class Zelle
    {
        public readonly bool SOLID;         // Schwarzer Block
        public readonly bool CHANGEABLE;    // Zelle darf geändert werden oder nicht
        private uint value = 0;             // Wert in der Zelle, 0 = keine Zahl eingetragen

        /// <summary>
        /// Der Konstruktor, um eine Zelle in einem Rätsel zu erzeugen.
        /// </summary>
        /// <param name="value">Wert in der Zelle. Erlaubter Bereich 0 bis 9. 0 = keine Zahl eingetragen. Andere Zahlen führen zu einer automatischen Belegung mit 0.</param>
        /// <param name="changeable">Setzt das Flag, ob der Wert dieser Zelle geändert werden darf.</param>
        /// <param name="solid">Setzt das Flag, ob es sich bei dieser Zelle um ein schwarzes Feld im Rätsel handelt. Impliziert CHANGEABLE = false.</param>
        public Zelle(uint value, bool changeable, bool solid)
        {
            this.value = value;
            this.SOLID = solid;
            this.CHANGEABLE = solid ? false : changeable; // Wenn schwarz, dann changeable = false
        }

        /// <summary>
        /// Legt den aktuellen Wert in der Zelle fest oder gibt ihn zurück.
        /// </summary>
        /// <remarks>Der Wert der Zelle muss sich immer im Bereich von 0 bis 9 befinden, andernfalls wird er auf 0 gesetzt.</remarks>
        public uint Value
        {
            get { return value; }
            set { this.value = (!SOLID && CHANGEABLE && value <= 9) ? value : this.value; }
        }

        /// <summary>
        /// Erzeugt eine neue Zelle mit denselben Eigenschaften der aktuellen Zelle.
        /// </summary>
        /// <returns>Geklonte Zelle</returns>
        public Zelle Clone()
        {
            return new Zelle(value, CHANGEABLE, SOLID);
        }
    }
}
