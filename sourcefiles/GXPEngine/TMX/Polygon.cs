using System;
using System.Xml.Serialization;

/// <summary>
/// Represents a polygon tag inside object
/// </summary>
[XmlRoot("polygon")] // polygon is root node
public class Polygon
{
    /* Fields */
    [XmlAttribute("points")] // points is a property of polyline
    public string Points { get; set; }

    // Constructor
    public Polygon() { }
}
