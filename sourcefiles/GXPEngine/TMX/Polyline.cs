using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using GXPEngine;

/// <summary>
/// Represents a polyline tag inside object
/// </summary>
[XmlRoot("polyline")] // polyline is a root node
public class Polyline
{
    /* Fields */
    [XmlAttribute("points")] // points is a property of polyline
    public string Points { get; set; }

    // Constructor
    public Polyline() { }

    /// <summary>
    /// Get <paramref name="Points"/> split on empty spaces.
    /// </summary>
    /// <returns><see cref="string[]"/> of points which are split on empty space.</returns>
    public string[] GetPoints() {
        return Points.Split(' ');
    }

    /// <summary>
    /// Create a vector list based on the every point in <see cref="GetPoints()"/>.
    /// </summary>
    /// <returns>A list of type <see cref="Vec2"/> with all the coordinates of a polyline.</returns>
    public List<Vec2> GetPointsAsVectorList() {
        List<Vec2> vectorList = new List<Vec2>();
        Regex regex = new Regex("@[0-9]");
        foreach (string point in GetPoints()) {
            if (point == "0,0") {
                continue;
            }
            List<string> oddPoint = point.Trim().Split(',').ToList().Where((c, i) => i % 2 != 0).ToList();
            List<string> evenPoint = point.Trim().Split(',').ToList().Where((c, i) => i % 2 == 0).ToList();

            foreach (string singleEvenPoint in evenPoint) {
                int x = Convert.ToInt32(singleEvenPoint);
                foreach (string singleOddPoint in oddPoint) {
                    int y = Convert.ToInt32(singleOddPoint);
                    Vec2 myVec = new Vec2(x, y);
                    vectorList.Add(myVec);
                }
            }
        }

        return vectorList;
    }
}
