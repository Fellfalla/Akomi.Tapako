using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Tapako.Utilities.WiringTool.Extensions
{
    public static class PathFigureExtensions
    {
        /// <summary>
        /// Verbinde 2 Punkte mit einer Linie. 
        /// Als erster Punkt wird der Startpunkt des Konntektors übernommen
        /// </summary>
        /// <param name="connector"></param>
        /// <param name="endPoint"></param>
        /// <param name="bezierOffset"></param>
        public  static void ConnectConnectorWithPoint(this PathFigure connector, Point endPoint, double bezierOffset)
        {
            if (connector != null)
            {
                connector.Segments.Clear();

                // Bezier Abschnitt 1 -> waagrecht zum anfangspunkt
                var xValuePoint1 = connector.StartPoint.X + bezierOffset;

                // vom Konnektor halber weg zum endpunkt
                var yValuePoint1 = connector.StartPoint.Y;
                var point1 = new Point(xValuePoint1, yValuePoint1);

                //Bezier Abschnitt 2 -> waagrecht zum endpunkt nach links
                double xValuePoint2;
                if (Math.Abs(endPoint.X - connector.StartPoint.X) > Math.Abs(bezierOffset)) // it seems like the endPoint is on the other side -> inverted offset
                {
                    xValuePoint2 = endPoint.X - bezierOffset;
                }
                else // -> same offset direction
                {
                    xValuePoint2 = endPoint.X + bezierOffset;
                }
                // vom Endpunkt halber weg zum Anfangspunkt
                var yValuePoint2 = endPoint.Y;
                var point2 = new Point(xValuePoint2, yValuePoint2);

                connector.Segments.Add(new BezierSegment(point1, point2, endPoint, true));
            }
        }
    }
}
