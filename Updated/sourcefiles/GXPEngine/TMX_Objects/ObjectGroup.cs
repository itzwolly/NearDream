using System;
using System.Xml.Serialization;

/// <summary>
/// Represents an objectgroup tag inside map
/// </summary>
[XmlRoot("objectgroup")] // objectgroup is a root node
public class ObjectGroup
{
    /* Fields */
    [XmlAttribute("name")] // name is a property of objectgroup
    public string Name { get; set; }
    [XmlElement("properties")] // properties is a nested node in objectgroup
    public Properties Properties { get; set; }
    [XmlElement("object")] // object is a nested node in map > objectgroup
    public TiledObject[] Object { get; set; }

    // Constructor
    public ObjectGroup() { }

    /// <summary>
    /// Iterates through all tiled objects and gets a <see cref="TiledObject"/> with the same name as given name.
    /// </summary>
    /// <param name="pName">Name of object</param>
    /// <returns><see cref="TiledObject"/> with the same name as given name.</returns>
    public TiledObject GetObjectByName(string pName) {
        for (int i = 0; i < Object.Length; i++) {
            if (Object[i].Name == pName) {
                return Object[i];
            }
        }
        return null;
    }
}
