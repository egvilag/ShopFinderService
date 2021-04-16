using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace ShopAPI
{
    public class pageval
    {
        public int pageid { get; set; }
        public int ns { get; set; }
        public string title { get; set; }
        public List<Revision> revisions { get; set; }
    }

    public class Revision
    {
        public Slot slots { get; set; }
    }

    public class Slot
    {
        public MainObj main { get; set; }
    }

    public class MainObj
    {
        public string contentmodel { get; set; }
        public string contentformat { get; set; }
        public string content { get; set; }
    }

    public class Query
    {
        public List<Normalised> normalized { get; set; }
        public List<pageval> pages { get; set; }
    }

    public class Normalised
    {
        public bool fromencoded { set; get; }
        public string from { set; get; }
        public string to { set; get; }
    }

    public class RootObject
    {
        public bool batchcomplete { get; set; }
        public Query query { get; set; }
    }

    class WikiAPI
    {
        public void shortText()
        {
            using (WebClient wc = new WebClient())
            {
                var client = new WebClient();
                var response = client.DownloadString("https://en.wikipedia.org/w/api.php?format=json&action=query&prop=revisions&titles=List_of_computer_hardware_manufacturers&rvslots=*&rvprop=content&formatversion=2");

                var responseJson = JsonConvert.DeserializeObject<RootObject>(response);
                Console.WriteLine("Result:");
                List<string> manufacturers = new List<string>();
                string[] lines = responseJson.query.pages[0].revisions[0].slots.main.content.Split("\n");
                string str;
                foreach (string s in lines)
                {
                    if (s.StartsWith("==See also")) break;                                                                                      // Lap alja a linkekkel már nem kell
                    if (s.StartsWith('*'))                                                                                                      // Felsorolást vizsgálunk csak
                    {
                        str = s;
                        if ((str.Contains("-->")) && !(str.Contains("<--"))) continue;
                        if (str.Contains('<')) str = str.Substring(0, str.IndexOf('<'));                                                        // Ha kezd bármi html tag-be, azt már figyelmen kívül hagyjuk
                        if (str.Contains('(')) str = str.Substring(0, str.IndexOf('('));                                                        // Zárójeles megjegyzések sem kellenek
                        if (str.Contains('"')) str = str.Substring(0, str.IndexOf('"'));                                                        // Idézőjelbe tett megjegyzéseket sem kérjük
                        if (str.Contains('{')) str = str.Substring(0, str.IndexOf('{'));                                                        // Van még csapda?
                        if (str.Contains(',')) str = str.Substring(0, str.IndexOf(','));                                                        // Ez most valami vicc!
                        //if (str.Contains('.')) str = str.Substring(0, str.IndexOf('.'));                                                        // Haza akarok menni....
                        if (str.Contains(']')) str = str.Substring(0, str.IndexOf(']'));                                                        // egy sorban két hivatkozás!
                        if (str.Split('|').Count() > 1) str = str.Split('|')[1].Replace("[[", "").Replace("]]", "").Replace("*", "").Trim();    // Van pipe-al rövid név megadva? Ha igen, akkor az kell.
                        else str = str.Replace("[[", "").Replace("]]", "").Replace("*", "").Trim();                                             // Kivesszük a dupla szögletes szárójeleket

                        if (!(manufacturers.Contains(str, StringComparer.OrdinalIgnoreCase))) manufacturers.Add(str);
                    }
                }
                foreach (string s in manufacturers)
                    Console.WriteLine(s);

                Console.WriteLine("Result end.");


                //foreach (string s in responseJson.query.pages[0].revisions[0].slots.main.content.Split().Where(x => x.StartsWith("[[") && x.EndsWith("]]")).Select(x => x.Replace("[[", string.Empty).Replace("]]", string.Empty)).ToList())
                //foreach (string s in responseJson.query.pages[0].revisions[0].slots.main.content.Split().Where(x => x.StartsWith("[[") && x.EndsWith("]]")).ToList())
                //foreach (string s in System.Text.RegularExpressions.Regex.Match(responseJson.query.pages[0].revisions[0].slots.main.content, ".*?\\.Parameters\\[\"(.*?)\"]").Groups)
                //    Console.WriteLine(s);


                //Console.ReadLine();
            }

        }

       
    }
}
