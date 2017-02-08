using System;
using System.Xml.Serialization;



/// <summary>
/// Represents a tileset tag inside map
/// </summary>
[XmlRoot("tileset")] // tileset is a root node
public class TileSet
{
    /* Fields */
    [XmlAttribute("firstgid")] // firstgid is a property of tileset
    public int FirstGId { get; set; }
    [XmlAttribute("name")] // name is a property of tileset
    public string Name { get; set; }
    [XmlAttribute("tilewidth")] // tilewidth is a property of tileset
    public int TileWidth { get; set; }
    [XmlAttribute("tileheight")] // tileheight is a property of tileset
    public int TileHeight { get; set; }
    [XmlAttribute("tilecount")] // tilecount is a property of tileset
    public int TileCount { get; set; }
    [XmlAttribute("columns")] // columns is a property of tileset
    public int Columns { get; set; }
    [XmlElement("image")] // image is a nested node in Map > tileset
    public Image Image { get; set; } // pretty sure you can only have a one image in a tileset
    [XmlElement("properties")] // properties is a nested node in Map > tileset
    public Properties Properties { get; set; }
    [XmlElement("tile")] // tiles is a nested node in Map > tileset
    public Tile[] Tile { get; set; }

    // Constructor
    public TileSet() { }

    /* Getters */
    public double GetRows() { return Math.Floor(Convert.ToSingle(TileCount / Columns)); } // tiled doesn't save rows. Could be usefull
}