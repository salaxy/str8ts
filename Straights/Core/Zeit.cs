using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Straights.Core
{
    /// <summary>
    /// Diese Klasse stellt ein paar statische Hilfsmethoden zur Zeitausgabe bereit
    /// </summary>
    /// <remarks>
    /// Autor:
    /// Andy Klay klay@fh-brandenburg.de
    /// </remarks>
    class Zeit
    {
        private static int GetMinutes(int sec)
        {
            return sec / 60;
        }

        private static int GetHours(int sec)
        {
            return sec / 3600;
        }

        private static int GetSecounds(int sec)
        {
            return sec % 60;
        }


        /// <summary>
        /// Wandelt sekunden in einen Zeitstring um
        /// (mit Platzhalter im format 00:00:00)
        /// </summary>
        /// <param name="sec">Sekunden</param>
        /// <returns></returns>
        public static string GetTimeAsString(int sec){

            int h=GetHours(sec);
            int m=GetMinutes(sec);
            int s=GetSecounds(sec);
            string timeString ;

            timeString= string.Format("{0:00}:" + "{1:00}:" + "{2:00}", h, m, s);

            //aktuelleZeit
            //DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString() + ":" + DateTime.Now.Second.ToString();

            return timeString;
        }

    }
}
