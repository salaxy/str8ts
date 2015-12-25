using System;
using System.Globalization;
//using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Straights.Logik
{
    /// <summary>
    /// Diese Klasse hält statische Methoden bereit zur Drcukfunktion
    /// </summary>
    /// <remarks>
    /// Autor:
    /// Andy Klay klay@fh-brandenburg.de
    /// </remarks>
    class PrintHelper
    {
        /// <summary>
        /// Drucken eines FrameworkElement mit einer Überschrift
        /// </summary>
        /// <param name="ele">Element</param>
        /// <param name="name">Bezeichnung</param>
        public static void Print(FrameworkElement ele, string name)
        {
            //Randabstand 
            const double margin = 30;
            //Titelposition zum Rand
            const double titlePadding = 10;

            //Druckdialog erstellen und aufrufen
            PrintDialog printDlg = new PrintDialog();

            if (printDlg.ShowDialog() != true)
            {
                return;
            }

            //Text Zeile fuer Bezeichnung des Straights erstellen
            FormattedText formattedText = new FormattedText(name, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Verdana"), 25, Brushes.Black);

            //formattedText.MaxTextWidth = printDlg.PrintableAreaWidth;

            //skalierungfaktoren berechnen
            double scale = Math.Min(printDlg.PrintableAreaWidth / (ele.ActualWidth + (margin * 2)), (printDlg.PrintableAreaHeight - (formattedText.Height + titlePadding)) / (ele.ActualHeight + (margin * 2)));

            //Renderobject erstellen
            DrawingVisual visual = new DrawingVisual();

            //ZeichenContext holen und zeichnen
            ///DrawingContext beansprucht jetzt best. Ressourcen >>> using Direktive
            using (DrawingContext context = visual.RenderOpen())
            {
                VisualBrush brush = new VisualBrush(ele);

                //Fensterinhalt zeichnen
                context.DrawRectangle(brush, null, new Rect(new Point(margin, margin + formattedText.Height + titlePadding), new Size(ele.ActualWidth, ele.ActualHeight)));

                //Text zeichnen
                context.DrawText(formattedText, new Point(margin, margin));
            }

            //Skalieren
            visual.Transform = new ScaleTransform(scale, scale);

            printDlg.PrintVisual(visual, name);
        }
    }
}
