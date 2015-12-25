using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using Straights.Core;

namespace Straights.Serial
{
    /// <summary>
    /// Diese Klasse hält Methoden bereit
    /// zur Persistenz von Konfiguration und Spielständen bereit hält
    /// </summary>
    /// <remarks>
    /// Autor:
    /// Andy Klay klay@fh-brandenburg.de
    /// </remarks>
    public class Serialisator
    {
        /// <summary>
        /// Speichern eines Spielstandes
        /// </summary>
        /// <param name="Object">spiel</param>
        /// <param name="FileName">Dateipfad</param>
        public void SavePuzzleToFile(Puzzle Object, string FileName)
        {
            FileStream fs = null;

            try
            {
                //FileStream für die Datei erzeugen 
                fs = new FileStream(FileName, FileMode.Create, FileAccess.Write);

                //Das Objekt serialisieren 
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, Object);
            }
            finally
            {
                //Am ende noch den FileStream schliesen. 
                if (fs != null)
                {
                    fs.Flush();
                    fs.Close();
                }
            }
        }

        /// <summary>
        /// Laden eines Spielstandes
        /// </summary>
        /// <param name="FileName">Dateipfad</param>
        /// <returns></returns>
        public Puzzle LoadPuzzleFromFile(string FileName)
        {
            // platzhalter fuer geladenes spiel
            Puzzle save = null;

            // Open the file containing the data that you want to deserialize.
            FileStream fs = new FileStream(FileName, FileMode.Open);
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();

                // Deserialisieren und referenzieren
                save = (Puzzle)formatter.Deserialize(fs);
            }
            catch (SerializationException e)
            {
                Console.WriteLine("Deserialize fehlgeschlagen: " + e.Message);
                throw;
            }
            finally
            {
                fs.Close();
            }

            return save;

        }

        /// <summary>
        /// Laden einer Spieleliste
        /// </summary>
        /// <param name="FileName">Dateipfad</param>
        /// <returns></returns>
        public List<Puzzle> LoadPuzzleListFromFile(string FileName)
        {
            // platzhalter fuer geladenes spiel
            List<Puzzle> save = null;

            // Open the file containing the data that you want to deserialize.
            FileStream fs = new FileStream(FileName, FileMode.Open);
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();

                // Deserialisieren und referenzieren
                save = (List<Puzzle>)formatter.Deserialize(fs);
            }
            catch (SerializationException e)
            {
                Console.WriteLine("Deserialize fehlgeschlagen: " + e.Message);
                throw;
            }
            finally
            {
                fs.Close();
            }


            return save;
        }


        /// <summary>
        /// Speichern einer Spieleliste
        /// </summary>
        /// <param name="Object">Liste</param>
        /// <param name="FileName">Dateipfad</param>
        public void SavePuzzleListToFile(List<Puzzle> Object, string FileName)
        {
            FileStream fs = null;

            try
            {
                //FileStream für die Datei erzeugen 
                fs = new FileStream(FileName, FileMode.Create, FileAccess.Write);

                //Das Objekt serialisieren 
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, Object);
            }
            finally
            {
                //Am ende noch den FileStream schliesen. 
                if (fs != null)
                {
                    fs.Flush();
                    fs.Close();
                }
            }
        }


        /// <summary>
        /// Speichern der Configuration
        /// </summary>
        /// <param name="Object">Dictionary</param>
        /// <param name="FileName">Dateipfad</param>
        public void SaveConfigToFile(Dictionary<string, string> Object, string FileName)
        {
            FileStream fs = null;

            try
            {
                //FileStream für die Datei erzeugen 
                fs = new FileStream(FileName, FileMode.Create, FileAccess.Write);

                //Das Objekt serialisieren 
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, Object);
            }
            finally
            {
                //Am ende noch den FileStream schliesen. 
                if (fs != null)
                {
                    fs.Flush();
                    fs.Close();
                }
            }
        }

        /// <summary>
        /// laden einer config
        /// </summary>
        /// <param name="FileName">Dateipfad</param>
        /// <returns></returns>
        public Dictionary<string, string> LoadConfigFromFile(string FileName)
        {
            // platzhalter fuer geladenes spiel
            Dictionary<string, string> save = null;

            // Open the file containing the data that you want to deserialize.
            FileStream fs = new FileStream(FileName, FileMode.Open);
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();

                // Deserialisieren und referenzieren
                save = (Dictionary<string, string>)formatter.Deserialize(fs);
            }
            catch (SerializationException e)
            {
                Console.WriteLine("Deserialize fehlgeschlagen: " + e.Message);
                throw;
            }
            finally
            {
                fs.Close();
            }

            return save;

        }

    }
}
