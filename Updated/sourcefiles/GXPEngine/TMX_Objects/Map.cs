using System;
using System.Xml.Serialization;



/// <summary>
/// Represents a Tiled map.
/// </summary>
[XmlRoot("map")] // Map is a root node
public class Map
{
    /* Fields */
    [XmlAttribute("version")] // version is a property of Map
    public string Version { get; set; }
    [XmlAttribute("orientation")] // orientation is a property of Map
    public string Orientation { get; set; }
    [XmlAttribute("renderorder")] // renderorder is a property of Map
    public string RenderOrder { get; set; }
    [XmlAttribute("width")] // width is a property of Map
    public int Width { get; set; }
    [XmlAttribute("height")] // height is a property of Map
    public int Height { get; set; }
    [XmlAttribute("tilewidth")] // tilewidth is a property of Map
    public int TileWidth { get; set; }
    [XmlAttribute("tileheight")] // tileheight is a property of Map
    public int TileHeight { get; set; }
    [XmlAttribute("nextobjectid")] // nextobjectid is a property of Map
    public int NextObjectId { get; set; }
    [XmlElement("layer")] // layers is a nested node in Map
    public TiledLayer[] Layer { get; set; }
    [XmlElement("tileset")] // tilesets is a nested node in Map
    public TileSet[] TileSet { get; set; }
    [XmlElement("objectgroup")] // objectgroup is a nested node in Map
    public ObjectGroup[] ObjectGroup { get; set; }
    [XmlElement("properties")]
    public Properties Properties { get; set; }

    // Constructor
    public Map() { }

    /* Methods */
    public int GetLevelWidth() {
        return Width * TileWidth;
    }

    public int GetLevelHeight() {
        return Height * TileHeight;
    }


    /// <summary>
    /// Calculates tile size.
    /// </summary>
    /// <returns><see cref="int"/> of tile size</returns>
    public int GetTileSize() { return Convert.ToInt32(TileWidth) * Convert.ToInt32(TileHeight); } // tiled doesn't calculate tile size. Could be usefull
    
    /// <summary>
    /// Iterates through all objectgroups and gets a <see cref="ObjectGroup"/> with the same name as given name.
    /// </summary>
    /// <param name="pName">Name of objectgroup</param>
    /// <returns><see cref="ObjectGroup"/> with the same name as given name.</returns>
    public ObjectGroup GetObjectGroupByName(string pName) {
        for (int i = 0; i < ObjectGroup.Length; i++) {
            if (ObjectGroup[i].Name == pName) {
                return ObjectGroup[i];
            }
        }
        return null;
    }

    /// <summary>
    /// A beautyfied string of all the properties in <see cref="GetMap()"/>.
    /// </summary>
    /// <returns>A more user-friendly string</returns>
    public override string ToString()
    {
        string readableString = "- Level properties " + Environment.NewLine +
                                "\tWidth = " + Width + " tiles" + Environment.NewLine +
                                "\tHeight = " + Height + " tiles" + Environment.NewLine +
                                "\tTilesize = " + GetTileSize() + " px" + Environment.NewLine +
                                "\t\tTileWidth = " + TileWidth + " px" + Environment.NewLine +
                                "\t\tTileHeight = " + TileHeight + " px" + Environment.NewLine +
                                Environment.NewLine +
                                "- Tile properties " + Environment.NewLine;

        if (TileSet != null)
        {
            for (int tileSetIndex = 0; tileSetIndex < TileSet.Length; tileSetIndex++)
            {
                readableString += "\t[Tileset #" + (tileSetIndex + 1) + "] {" + Environment.NewLine +
                                  "\t\tFirstGId = " + TileSet[tileSetIndex].FirstGId + Environment.NewLine +
                                  "\t\tName = " + TileSet[tileSetIndex].Name + Environment.NewLine +
                                  "\t\tTileWidth = " + TileSet[tileSetIndex].TileWidth + " px" + Environment.NewLine +
                                  "\t\tTileHeight = " + TileSet[tileSetIndex].TileHeight + " px" + Environment.NewLine +
                                  "\t\tTileCount = " + TileSet[tileSetIndex].TileCount + " tiles" + Environment.NewLine +
                                  "\t\tColumns = " + TileSet[tileSetIndex].Columns + " tiles" + Environment.NewLine +
                                  "\t\tRows = " + TileSet[tileSetIndex].GetRows() + " tiles" + Environment.NewLine;

                if (TileSet[tileSetIndex].Properties != null)
                {
                    for (int propertyIndex = 0; propertyIndex < TileSet[tileSetIndex].Properties.PropertyArray.Length; propertyIndex++)
                    {
                        readableString += "\t\t[Property #" + (propertyIndex + 1) + "] {" + Environment.NewLine +
                                          "\t\t\tName = " + TileSet[tileSetIndex].Properties.PropertyArray[propertyIndex].Name + Environment.NewLine +
                                          "\t\t\tType = " + TileSet[tileSetIndex].Properties.PropertyArray[propertyIndex].Type + Environment.NewLine +
                                          "\t\t\tValue = " + TileSet[tileSetIndex].Properties.PropertyArray[propertyIndex].Value + Environment.NewLine +
                                          "\t\t}" + Environment.NewLine;
                    }
                }

                readableString += "\t\tImage = " + TileSet[tileSetIndex].Image.Source + Environment.NewLine +
                                  "\t\t\tWidth = " + TileSet[tileSetIndex].Image.Width + " px" + Environment.NewLine +
                                  "\t\t\tHeight = " + TileSet[tileSetIndex].Image.Height + " px" + Environment.NewLine;

                if (TileSet[tileSetIndex].Tile != null)
                {
                    for (int tileIndex = 0; tileIndex < TileSet[tileSetIndex].Tile.Length; tileIndex++)
                    {
                        readableString += "\t\tTile = " + TileSet[tileSetIndex].Tile[tileIndex].Id + Environment.NewLine;

                        for (int propertyIndex = 0; propertyIndex < TileSet[tileSetIndex].Tile[tileIndex].Properties.PropertyArray.Length; propertyIndex++)
                        {
                            readableString += "\t\t\t[Property #" + (propertyIndex + 1) + "] { " + Environment.NewLine +
                                              "\t\t\t\tName = " + TileSet[tileSetIndex].Tile[tileIndex].Properties.PropertyArray[propertyIndex].Name + Environment.NewLine +
                                              "\t\t\t\tType = " + TileSet[tileSetIndex].Tile[tileIndex].Properties.PropertyArray[propertyIndex].Type + Environment.NewLine +
                                              "\t\t\t\tValue = " + TileSet[tileSetIndex].Tile[tileIndex].Properties.PropertyArray[propertyIndex].Value + Environment.NewLine +
                                              "\t\t\t}" + Environment.NewLine;
                        }
                    }
                }
                readableString += "\t}" + Environment.NewLine;
            }
        }

        if (Layer != null)
        {
            for (int layerIndex = 0; layerIndex < Layer.Length; layerIndex++)
            {
                readableString += "\t[Layer #" + (layerIndex + 1) + "] {" + Environment.NewLine +
                                  "\t\tName = " + Layer[layerIndex].Name + Environment.NewLine +
                                  "\t\tWidth = " + Layer[layerIndex].Width + " tiles" + Environment.NewLine +
                                  "\t\tHeight = " + Layer[layerIndex].Height + " tiles" + Environment.NewLine;

                if (Layer[layerIndex].Properties != null)
                {
                    for (int propertyIndex = 0; propertyIndex < Layer[layerIndex].Properties.PropertyArray.Length; propertyIndex++)
                    {
                        readableString += "\t\t[Property #" + (propertyIndex + 1) + "] {" + Environment.NewLine +
                                           "\t\t\tName = " + Layer[layerIndex].Properties.PropertyArray[propertyIndex].Name + Environment.NewLine +
                                           "\t\t\tType = " + Layer[layerIndex].Properties.PropertyArray[propertyIndex].Type + Environment.NewLine +
                                           "\t\t\tValue = " + Layer[layerIndex].Properties.PropertyArray[propertyIndex].Value + Environment.NewLine +
                                           "\t\t}" + Environment.NewLine;
                    }
                }


                readableString += "\t\t[Data = { " +
                                  Layer[layerIndex].Data.InnerXML + "\t\t}]" + Environment.NewLine +
                                  "\t}" + Environment.NewLine;
            }
        }

        if (ObjectGroup != null)
        {
            for (int objectGroupIndex = 0; objectGroupIndex < ObjectGroup.Length; objectGroupIndex++)
            {
                readableString += "\t[ObjectGroup #" + (objectGroupIndex + 1) + "] {" + Environment.NewLine +
                                  "\t\tName = " + ObjectGroup[objectGroupIndex].Name + Environment.NewLine;

                if (ObjectGroup[objectGroupIndex].Properties != null)
                {
                    for (int propertyIndex = 0; propertyIndex < ObjectGroup[objectGroupIndex].Properties.PropertyArray.Length; propertyIndex++)
                    {
                        readableString += "\t\t[Property #" + (propertyIndex + 1) + "] {" + Environment.NewLine +
                                          "\t\t\tName = " + ObjectGroup[objectGroupIndex].Properties.PropertyArray[propertyIndex].Name + Environment.NewLine +
                                          "\t\t\tType = " + ObjectGroup[objectGroupIndex].Properties.PropertyArray[propertyIndex].Type + Environment.NewLine +
                                          "\t\t\tValue = " + ObjectGroup[objectGroupIndex].Properties.PropertyArray[propertyIndex].Value + Environment.NewLine +
                                          "\t\t}" + Environment.NewLine;
                    }
                }

                for (int objectIndex = 0; objectIndex < ObjectGroup[objectGroupIndex].Object.Length; objectIndex++)
                {
                    readableString += "\t\t[Object #" + (objectIndex + 1) + "] {" + Environment.NewLine +
                                      "\t\t\tId = " + ObjectGroup[objectGroupIndex].Object[objectIndex].Id + Environment.NewLine;

                    if (ObjectGroup[objectGroupIndex].Object[objectIndex].Name != null)
                    {
                        readableString += "\t\t\tName = " + ObjectGroup[objectGroupIndex].Object[objectIndex].Name + Environment.NewLine;
                    }

                    if (ObjectGroup[objectGroupIndex].Object[objectIndex].Type != null)
                    {
                        readableString += "\t\t\tType = " + ObjectGroup[objectGroupIndex].Object[objectIndex].Type + Environment.NewLine;
                    }

                    if (ObjectGroup[objectGroupIndex].Object[objectIndex].GId != null)
                    {
                        readableString += "\t\t\tGId = " + ObjectGroup[objectGroupIndex].Object[objectIndex].GId + Environment.NewLine;
                    }

                    readableString += "\t\t\tX = " + ObjectGroup[objectGroupIndex].Object[objectIndex].X + " px" + Environment.NewLine +
                                      "\t\t\tY = " + ObjectGroup[objectGroupIndex].Object[objectIndex].Y + " px" + Environment.NewLine +
                                      "\t\t\tWidth = " + ObjectGroup[objectGroupIndex].Object[objectIndex].Width + " px" + Environment.NewLine +
                                      "\t\t\tHeight = " + ObjectGroup[objectGroupIndex].Object[objectIndex].Height + " px" + Environment.NewLine +
                                      "\t\t\tRotation = " + ObjectGroup[objectGroupIndex].Object[objectIndex].Rotation + Environment.NewLine;

                    if (ObjectGroup[objectGroupIndex].Object[objectIndex].Properties != null)
                    {
                        for (int propertyIndex = 0; propertyIndex < ObjectGroup[objectGroupIndex].Object[objectIndex].Properties.PropertyArray.Length; propertyIndex++)
                        {
                            readableString += "\t\t\tProperty[" + (propertyIndex + 1) + "] {" + Environment.NewLine +
                                              "\t\t\t\tName = " + ObjectGroup[objectGroupIndex].Object[objectIndex].Properties.PropertyArray[propertyIndex].Name + Environment.NewLine +
                                              "\t\t\t\tType = " + ObjectGroup[objectGroupIndex].Object[objectIndex].Properties.PropertyArray[propertyIndex].Type + Environment.NewLine +
                                              "\t\t\t\tValue = " + ObjectGroup[objectGroupIndex].Object[objectIndex].Properties.PropertyArray[propertyIndex].Value + Environment.NewLine +
                                              "\t\t\t}" + Environment.NewLine;
                        }
                    }

                    if (ObjectGroup[objectGroupIndex].Object[objectIndex].Polyline != null)
                    {
                        readableString += "\t\t\tPolyline = " + ObjectGroup[objectGroupIndex].Object[objectIndex].Polyline.Points + Environment.NewLine;
                    }
                    if (ObjectGroup[objectGroupIndex].Object[objectIndex].Polygon != null)
                    {
                        readableString += "\t\t\tPolygon = " + ObjectGroup[objectGroupIndex].Object[objectIndex].Polygon.Points + Environment.NewLine;
                    }
                    readableString += "\t\t}" + Environment.NewLine;
                }
                readableString += "\t}" + Environment.NewLine;
            }
        }
        return readableString;
    }
}
