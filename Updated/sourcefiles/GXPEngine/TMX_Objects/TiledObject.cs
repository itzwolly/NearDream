using System;
using System.Xml.Serialization;


/// <summary>
/// Represents an object tag inside objectgroup
/// </summary>
[XmlRoot("object")] // object is a root node
public class TiledObject
{
    /* Fields */
    [XmlAttribute("id")] // id is a property of object
    public int Id { get; set; }
    [XmlAttribute("name")] // name is a property of object
    public string Name { get; set; }
    [XmlAttribute("type")]
    public string Type { get; set; }
    [XmlAttribute("gid")] // name is a property of object
    public string GId { get; set; }
    [XmlAttribute("x")] // x is a property of object
    public float X { get; set; }
    [XmlAttribute("y")] // y is a property of object
    public float Y { get; set; }
    [XmlAttribute("width")] // width is a property of object
    public int Width { get; set; }
    [XmlAttribute("height")] // height is a property of object
    public int Height { get; set; }
    [XmlAttribute("rotation")] // height is a property of object
    public int Rotation { get; set; }
    [XmlElement("properties")] // properties is a nested node in object
    public Properties Properties { get; set; }
    [XmlElement("polyline")] // polyline is a nested node in object
    public Polyline Polyline { get; set; }
    [XmlElement("polygon")] // polygon is a nested node in object
    public Polygon Polygon { get; set; }
    [XmlElement("ellipse")] // polygon is a nested node in object
    public Ellipse Ellipse { get; set; }

    // Constructor
    public TiledObject() { }
}
