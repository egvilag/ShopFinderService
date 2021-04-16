using System;
using System.Reflection;
using System.Threading;
using System.IO;

namespace ShopAPI
{
    class Program
    {
        // Program name with actual major.minor.build.revision number
        static string ProgramName = "ÉGvilág ShopAPI v" + Assembly.GetEntryAssembly().GetName().Version;
        // Full path to binary
        public static string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        static void Main(string[] args)
        {
            //SQL.MakeConnection();
            WikiAPI wa = new WikiAPI();
            wa.GetManufacturers();
        }
    }
}
