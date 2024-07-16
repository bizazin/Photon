using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Helpers;

namespace Utils
{
    public static class CsvUtils
    {
        private const string LineBreak = "%n%";

        public static List<T> ParseCsv<T>(string data)
        {
            var myMap = new CsvMap();
            myMap.DefineColumns(typeof(T));
            var itemList = myMap.LoadCsvFromString(data);
            return itemList.Cast<T>().ToList();
        }

        public static List<T> ParseCsvFromResources<T>(string path)
        {
            var myMap = new CsvMap();
            myMap.DefineColumns(typeof(T));
            var itemList = myMap.LoadCsvFromFile(path);
            return itemList.Cast<T>().ToList();
        }

        public static void ExportCsv<T>(List<T> genericList, string finalPath)
        {
            var sb = new StringBuilder();
            var header = string.Empty;
            var info = typeof(T).GetFields();

            if (!File.Exists(finalPath))
            {
                var file = File.Create(finalPath);
                file.Close();
                foreach (var prop in info)
                    header += prop.Name + "; ";

                header = header.Substring(0, header.Length - 2);
                sb.AppendLine(header);
                TextWriter sw = new StreamWriter(finalPath, true);
                sw.Write(sb.ToString());
                sw.Close();
            }

            foreach (var obj in genericList)
            {
                sb = new StringBuilder();
                var line = string.Empty;
                foreach (var prop in info)
                    line += prop.GetValue(obj) + "; ";

                line = line.Substring(0, line.Length - 2);
                sb.AppendLine(line);
                TextWriter sw = new StreamWriter(finalPath, true);
                sw.Write(sb.ToString());
                sw.Close();
            }
        }

        public static string ReplaceLineBreaks(string data)
        {
            if (data == null)
                return "";

            return data.Replace(LineBreak, "\n");
        }
    }
}