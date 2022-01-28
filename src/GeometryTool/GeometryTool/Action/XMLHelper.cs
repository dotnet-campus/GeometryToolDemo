using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GeometryTool
{
    public class XMLHelper
    {
        /// <summary>
        /// 读取xml文件，返回图形所在Match
        /// </summary>
        /// <param name="vPath"></param>
        /// <param name="vRootCanvas"></param>
        public Match ReadXml(string vPath)
        {
            StreamReader _streamReader = new StreamReader(vPath);
            Regex _regex = new Regex(@"<Figures>\s*([^F]*)</Figures>");
            Match _match = _regex.Match(_streamReader.ReadToEnd());
            _streamReader.Close();
            return _match;
        }

        /// <summary>
        /// 把图形写入XML文件中写入XML
        /// </summary>
        /// <param name="vPath"></param>
        /// <param name="vGeometryString"></param>
        public void WriteXml(string vPath,string vGeometryString)
        {
            StreamWriter sw = new StreamWriter(vPath);
            sw.Write(vGeometryString);
            sw.Close();
        }
    }
}
