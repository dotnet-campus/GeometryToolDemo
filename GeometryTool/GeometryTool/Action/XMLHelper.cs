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
        public MatchCollection ReadXml(string vPath)
        {
            StreamReader streamReader = new StreamReader(vPath);
            Regex regex = new Regex(@"<Geometry>\s*([^G]*)</Geometry>");
            MatchCollection matchList = regex.Matches(streamReader.ReadToEnd());
            streamReader.Close();
            return matchList;
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
