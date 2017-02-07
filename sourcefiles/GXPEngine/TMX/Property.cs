using System;
using System.Xml.Serialization;


/// <summary>
/// Represents a property tag inside properties
/// </summary>
[XmlRoot("property")] // property is a root node
public class Property
{
    /* Fields */
    [XmlAttribute("name")] // name is a property of property
    public string Name { get; set; }
    [XmlAttribute("type")] // type is a property of property
    public string Type { get; set; }
    [XmlAttribute("value")] // value is a property of property
    public string Value { get; set; }

    // Constructor
    public Property() { }
}
