using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace ShopAPI
{
    static class SQL
    {
        static string connstr;
        static MySqlConnection myConn;
        static bool noDatabase = false;

        public static string dbServer;
        public static string dbPort;
        public static string dbUser;
        public static string dbPassword;
        public static string dbDatabase;

        // Make database connection with the actual credentials
        public static bool MakeConnection()
        {
            connstr = "Server=" + dbServer + ";Port=" + dbPort + ";Database=" + dbDatabase + ";Uid=" + dbUser + ";Pwd=" + dbPassword + ";CharSet=utf8;Connect Timeout=10";
            myConn = new MySqlConnection(connstr);
            if ((dbServer == "") || (dbDatabase == "") || (dbUser == "") || (dbPassword == ""))
            {
                //Log.WriteLog("Nincs megadva vagy hibás az adatbázis kapcsolat konfigurációja!", true);
                Environment.Exit(0);
            }
            Console.Write("Adatbázis kapcsolat tesztelése...");
            try
            {
                myConn.Open();
                myConn.Close();
                Console.WriteLine("OK");
                return true;
            }
            catch (Exception e)
            {
                //Log.WriteLog("Adatbázis hiba: " + e.Message, true);
                myConn.Close();
                Console.WriteLine("FAILED!");
                return false;
            }
        }

    }
}
