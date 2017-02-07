using System;
using System.Xml.Serialization;


/// <summary>
/// Represents a tile tag inside tileset
/// </summary>
[XmlRoot("tile")] // tile is a root node
public class Tile
{
    /* Fields */
    [XmlAttribute("id")] // id is a property of tile
    public int Id { get;  set; }
    [XmlElement("properties")] // properties is a nested node in Map > tileset > tile
    public Properties Properties { get; set; }

    // Constructor
    public Tile() { }
}
