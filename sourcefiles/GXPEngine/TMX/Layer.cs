using System;
using System.Xml.Serialization;

/// <summary>
/// Represents a layer tag inside map
/// </summary>
[XmlRoot("layer")] // layer is a root node
public class Layer {
    /* Constants */
    public const string COLLIDABLE_LAYER_NAME = "Non-passable";

    /* Fields */
    [XmlAttribute("name")] // name is a property of layer
    public string Name { get; set; }
    [XmlAttribute("width")] // width is a property of layer
    public int Width { get; set; }
    [XmlAttribute("height")] // height is a property of layer
    public int Height { get; set; }
    [XmlElement("data")] // data is a nested node in map > layer
    public Data Data { get; set; }
    [XmlElement("properties")] // properties is a nested node in map > layer
    public Properties Properties { get; set; }
    

    // Constructor
    public Layer() { }
}