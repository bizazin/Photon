using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;
using Utils;

namespace Helpers
{
    public class CsvMap
    {
        /*
         *
         * Quick Usage Guide:
         *
         * CSVMap allows you to map a CSV file to a class in Unity.  This is perfect for monster lists, skill lists, or item lists
        Usage is simple.  Make a new CSVMap object like this:
        CSVMap myMap = new CSVMap();

        then you can make a class to define how your data is saved, here is an example of an item class:

        public class item {
            public int itemId;
            public string itemName;
            public string itemIcon;
            public string[] useEffects;
        }

        Now you can make a CSV file with the same header names as the field names of the class.
        They must be the same!
        Like this:

        itemId,itemName,itemIcon,useEffects
        1,Bens Magical Potion,POTION_ICON,POTION_HEAL|POTION_GROW|POTION_MAGIC_IMMUNE
        2,Bens Poison Potion, POTION_ICON_POISON, POTION_POISON

        To use CSVMap to transform your CSV data into an ArrayList of your items simply do this:

        CSVMap myMap = new CSVMap();
        myMap.defineColumns(typeof(ItemClass));
        ArrayList itemList = myMap.loadCsvFromFile("items"); //remember not to use an extension for embedded Unity resources.

        and thats it!  Now itemList will be filled with all of the data from your CSV file.
        Remember to put the CSV file in your Resources folder in your project root.
        If you don't have a resources folder you need to make one.

        A few tips:

        Vector3's are serialized in the CSV with colons.
        Example:  1:2:3 in a csv field is the same as new Vector3(1.0f,2.0f,3.0f);

        String lists are serialized with a pipe: |
        Example: apples|bananas|carrots makes a string[] with 3 elements.

        Currently the only array type that is supported is a string array.
        */

        private Hashtable _columnMap = new Hashtable();
        Type _classTemplate;

        public CsvMap()
        {
        }

        /// <summary>
        /// Shortcut to defineColumns from constructor
        /// </summary>
        /// <param name="classDefinition">Class definition.</param>
        public CsvMap(Type classDefinition)
        {
            DefineColumns(classDefinition);
        }

        /// <summary>
        /// Define Columns - call this first, with the typeof of the class you want to map to.  Example:
        /// CSVMap myMap = new CSVMap();
        /// myMap.defineColumns(typeof(MYCLASS));
        /// This will analyze your class and save the information for later.
        /// </summary>
        /// <param name="classDefinition">Class definition.</param>
        public void DefineColumns(Type classDefinition)
        {
            _classTemplate = classDefinition;
            _columnMap = new Hashtable();
            var members = classDefinition.GetFields();
            foreach (var memberInfo in members)
            {
                var m = memberInfo;
                _columnMap[m.Name] = m;
            }
        }

        /// <summary>
        /// Use this after calling defineColumns to load your CSV data (if it is not coming from a file).
        /// The data can easily come from a web service call over the internet, a website, a file, or elsewhere. 
        /// </summary>
        /// <returns>The csv from string.</returns>
        /// <param name="data">Data.</param>
        public ArrayList LoadCsvFromString(string data)
        {
            var lines = data.Split('\n');

            var ctr = 0;
            var rows = new ArrayList();
            var columns = new ArrayList();
            foreach (var line in lines)
            {
                var csvRead = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
                var c = csvRead.Split(line);

                for (var i = 0; i < c.Length; i++)
                {
                    c[i] = c[i].TrimStart(' ', '"');
                    c[i] = c[i].TrimEnd('"');
                }

                if (ctr == 0)
                {
                    foreach (var colName in c)
                        columns.Add(colName.Trim("\n\r ".ToCharArray())); // TODO remove trim
                }
                else
                {
                    var templated = Activator.CreateInstance(_classTemplate);
                    for (var i = 0; i < c.Length; i++)
                    {
                        if (i > _columnMap.Count - 1)
                            continue;

                        var templateInfo = (FieldInfo)_columnMap[columns[i]];
                        if (templateInfo == null)
                            Debug.LogError("CSV Field Not Found In ClassTemplate: " + columns[i] + "  length: " +
                                           columns[i].ToString().Length + "  in " + _classTemplate);

                        if (templateInfo != null)
                        {
                            var colType = templateInfo.FieldType;

                            if (c[i] != null && c[i].Length > 0)
                            {
                                if (colType == typeof(Vector3))
                                {
                                    var useVector = new Vector3(float.Parse(c[i].Split(':')[0]),
                                        float.Parse(c[i].Split(':')[1]), float.Parse(c[i].Split(':')[2]));
                                    templateInfo.SetValue(templated, useVector);
                                }
                                else if (colType == typeof(string[]))
                                {
                                    var useList = c[i].Split('|');
                                    templateInfo.SetValue(templated, useList);
                                }
                                else
                                {
                                    var withLineBreaks = CsvUtils.ReplaceLineBreaks(c[i]);
                                    templateInfo.SetValue(templated, Convert.ChangeType(withLineBreaks, colType));
                                }
                            }
                        }
                    }

                    rows.Add(templated);
                }

                ctr++;
            }

            return rows;
        }

        /// <summary>
        /// loadCsvFromFile will load data directly from your Resources folder of your Unity project.  Perfect for embedded game data.
        /// Remember not to put an extension on your filename.  Example:  "items.csv" just becomes "items".
        /// loadCsvFromFile("items");
        /// </summary>
        /// <returns>The csv from file.</returns>
        /// <param name="fileName">File name.</param>
        public ArrayList LoadCsvFromFile(string fileName)
        {
            var textAsset = (TextAsset)Resources.Load(fileName, typeof(TextAsset));

            if (textAsset == null)
                return null;
            return LoadCsvFromString(textAsset.text);
        }

        public ArrayList LoadCsvFromPersistentPathFile(string fileName)
        {
            var text = File.ReadAllText(Path.Combine(Application.persistentDataPath, fileName));

            if (string.IsNullOrEmpty(text))
                return null;
            return LoadCsvFromString(text);
        }
    }
}