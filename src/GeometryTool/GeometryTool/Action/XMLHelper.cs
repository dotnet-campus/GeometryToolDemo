using System.IO;
using System.Text.RegularExpressions;

namespace GeometryTool;

public class XMLHelper
{
    /// <summary>
    ///     读取xml文件，返回图形所在Match
    /// </summary>
    /// <param name="vPath"></param>
    /// <param name="vRootCanvas"></param>
    public Match ReadXml(string vPath)
    {
        var _streamReader = new StreamReader(vPath);
        var _regex = new Regex(@"<Figures>\s*([^F]*)</Figures>");
        var _match = _regex.Match(_streamReader.ReadToEnd());
        _streamReader.Close();
        return _match;
    }

    /// <summary>
    ///     把图形写入XML文件中写入XML
    /// </summary>
    /// <param name="vPath"></param>
    /// <param name="vGeometryString"></param>
    public void WriteXml(string vPath, string vGeometryString)
    {
        var sw = new StreamWriter(vPath);
        sw.Write(vGeometryString);
        sw.Close();
    }
}