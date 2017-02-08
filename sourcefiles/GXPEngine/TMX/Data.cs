using System;
using System.Linq;
using System.Xml.Serialization;

/// <summary>
/// Represents a data tag inside layer
/// </summary>
[XmlRoot("data")] // data is a root node
public class Data
{
    /* Constants*/
    //const uint FLIPPED_HORIZONTALLY_FLAG = 0x80000000;
    //const uint FLIPPED_VERTICALLY_FLAG = 0x40000000;
    //const uint FLIPPED_DIAGONALLY_FLAG = 0x20000000;

    /* Fields */
    [XmlAttribute("encoding")] // encoding is a property of data
    public string Encoding { get; set; }
    [XmlText] // represents the text inside the data tag
    public string InnerXML { get; set; }

    uint[,] _level;

    // Constructor
    public Data() { }

    /// <summary>
    /// Creates a 2D array using the data string and sets it in <see cref="_level"/>.
    /// </summary>
    /// <param name="pHeight">Aount of tiles on the Y axis</param>
    /// <param name="pWidth">Amount of tiles on the X axis</param>
    public void SetLevelArray(int pHeight, int pWidth) {
        _level = new uint[pHeight, pWidth];
        string[] dataRow = InnerXML.Split('\n').Where(x => !string.IsNullOrEmpty(x)).ToArray();

        for (int i = 0; i < dataRow.Length; i++) {
            // trimming the last comma, because otherwise length returns 17 instead of 16
            string[] dataCol = dataRow[i].TrimEnd(',').Split(',');

            for (int j = 0; j < dataCol.Length; j++) {
                _level[i, j] = uint.Parse(dataCol[j]);
            }
        }
    }

    /* Methods */
    /// <summary>
    /// Gets level array
    /// </summary>
    /// <returns>Array of type <see cref="uint"/></returns>
    public uint[,] GetLevelArray() {
        return _level;
    }

    /// <summary>
    /// Caculates tileId based on orientation of the tile.
    /// </summary>
    /// <param name="pTileId">Tile id</param>
    /// <returns>Tild id</returns>
    public uint GetGId(uint pTileId) {
        //uint flipped_horizontally = (pTileId & FLIPPED_HORIZONTALLY_FLAG);
        //uint flipped_vertically = (pTileId & FLIPPED_VERTICALLY_FLAG);
        //uint flipped_diagonally = (pTileId & FLIPPED_DIAGONALLY_FLAG);
        uint tileId = pTileId;
        if (pTileId / 1000000000 == 1) {
            tileId -= 1610612736;
        } else if (pTileId / 1000000000 == 2) {
            tileId -= 2684354560;
        } else if (pTileId / 1000000000 == 3) {
            tileId -= 3221225472;
        }
        return tileId;
    }

    /// <summary>
    /// Calculates rotation based on orientation of the tile.
    /// </summary>
    /// <param name="pTileId">Tile id</param>
    /// <returns>uint in degrees. ie: 90 = 90 degrees</returns>
    public uint GetRotation(uint pTileId) {
        if (pTileId / 1000000000 == 1) {
            return 90;
        } else if (pTileId / 1000000000 == 2) {
            return 270;
        } else if (pTileId / 1000000000 == 3) {
            return 180;
        } else {
            return 0;
        }
    }
}
