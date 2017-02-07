using System;
using System.Linq;
using System.Xml.Serialization;


/// <summary>
/// Represents a properties tag
/// </summary>
[XmlRoot("properties")] // properties is a root node
public class Properties
{
    /* Fields */
    [XmlElement("property")] // property is a nested node in properties
    public Property[] PropertyArray { get; set; }

    // Constructor
    public Properties() { }

    /// <summary>
    /// Searches for <see cref="Property"/> with same name as given parameter.
    /// </summary>
    /// <param name="name">Property name</param>
    /// <returns><see cref="Property"/> where name is equal to given parameter <paramref name="name"/>.</returns>
    public Property GetPropertyByName(string name) {
        return PropertyArray.Where(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)).Single();
    }

    /// <summary>
    /// Searches for <see cref="Property[]"/> with same name as given parameter.
    /// </summary>
    /// <param name="name">Property name</param>
    /// <returns><see cref="Property[]"/> where name is equal to given parameter <paramref name="name"/>.</returns>
    [Obsolete("Method is deprecated. Use GetPropertyByName instead. Reason being: name is unique, so you can never have a list of properties with the same name.")]
    public Property[] GetPropertyArrayByName(string name) {
        return PropertyArray.Where(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)).ToArray();
    }
}
