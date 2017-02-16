using System;
using System.Xml.Serialization;


/// <summary>
/// Represents an image tag inside tileset
/// </summary>
[XmlRoot("image")] // image is a root node
public class Image
{
    /* Fields */
    [XmlAttribute("source")] // source is a property of image
    public string Source { get; set; }
    [XmlAttribute("width")] // width is a property of image
    public int Width { get; set; }
    [XmlAttribute("height")] // height is a property of image
    public int Height { get; set; }

    // Constructor
    public Image() { }
}
