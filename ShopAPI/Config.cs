using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ShopAPI
{
    static class Config
    {
        static string configFile = Program.path + @"/shopapi.cfg";
        static Dictionary<string, string> configDict = new Dictionary<string, string>();

        // Give values to config variables
        public static void ReadConfig()
        {
            try
            {
                if (!File.Exists(configFile)) CreateConfigFile();
                ReadConfigDict(configFile);
                //Log.maxLogSize = long.Parse(configDict["MaxLogSize"]);
                //Listener.launcherPort = Int32.Parse(configDict["LauncherPort"]);
                SQL.dbServer = configDict["SQL-Server"];
                SQL.dbPort = configDict["SQL-Port"];
                SQL.dbUser = configDict["SQL-User"];
                SQL.dbPassword = configDict["SQL-Password"];
                SQL.dbDatabase = configDict["SQL-Database"];

            }
            catch (Exception e)
            {
                Console.WriteLine("Nem olvasható a config fájl!" + "(" + e.Message + ")");
                Environment.Exit(0);
            }
        }

        // Create blank config file
        static void CreateConfigFile()
        {
            StreamWriter sw = new StreamWriter(configFile);
            sw.WriteLine("MaxLogSize=629145600");
            sw.WriteLine("LauncherPort=");
            sw.WriteLine("SQL-Server=");
            sw.WriteLine("SQL-Port=");
            sw.WriteLine("SQL-User=");
            sw.WriteLine("SQL-Password=");
            sw.WriteLine("SQL-Database=");
            sw.Flush();
            sw.Close();
        }

        // Low level function to read the config file
        //static string ReadConfig2(string filename, string key)
        //{
        //    string value = "";
        //    string line;
        //    try
        //    {
        //        StreamReader sr = new StreamReader(filename);
        //        while (!sr.EndOfStream)
        //        {
        //            line = sr.ReadLine();
        //            if (line.Split('=')[0] == key) value = line.Split('=')[1];
        //        }
        //        sr.Close();
        //        sr = null;
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine("Nem olvasható a fájl: " + filename + "(" + e.Message + ")");
        //        Environment.Exit(0);
        //    }
        //    return value;
        //}


        // Read Config file into dictionary
        static void ReadConfigDict(string filename)
        {
            string line;
            string key;
            string value;
            try
            {
                StreamReader sr = new StreamReader(filename);
                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    key = line.Split("=")[0];
                    value = line.Split("=")[1];
                    if (configDict.ContainsKey(key)) configDict[key] = value;
                    else configDict.Add(key, value);
                }
                sr.Close();
                sr = null;
            }
            catch (Exception e)
            {
                Console.WriteLine("Nem olvasható a fájl: " + filename + "(" + e.Message + ")");
                Environment.Exit(0);
            }
        }
    }
}
