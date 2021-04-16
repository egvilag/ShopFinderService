using System;

namespace ShopAPI
{
    class Program
    {
        static void Main(string[] args)
        {
            //SQL.MakeConnection();
            WikiAPI wa = new WikiAPI();
            wa.shortText();
        }
    }
}
