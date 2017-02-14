using System;
using System.IO;
using System.Xml.Serialization;

public class TMXParser
{
    // Constructor
    public TMXParser() {

    }

    /// <summary>
    /// De-serializes the given file's XML into objects of the specified type <see cref="Map"/>.
    /// </summary>
    /// <param name="pFileName">File path name.</param>
    public Map ParseFile(string pFileName)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(Map));

        TextReader reader = new StreamReader(pFileName);
        Map map = serializer.Deserialize(reader) as Map;
        reader.Close();

        return map;
    }
}
