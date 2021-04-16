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
    public class Manufacturer
    {
        //public string name { get; set; }
        public string wikiLink { get; set; }
        public string website { get; set; }

        //public Manufacturer(string wikiLink, string webStite)
        //{
        //    this.wikiLink = wikiLink;
        //    this.website = website;
        //}
    }

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
        public Dictionary<string, Manufacturer> manufacturers = new Dictionary<string, Manufacturer>();

        public void GetManufacturers()
        {
            using (WebClient wc = new WebClient())
            {
                Console.WriteLine("Getting manufacturers list...");
                RootObject responseJson = GetWikiPage("https://en.wikipedia.org/w/api.php?format=json&action=query&prop=revisions&titles=List_of_computer_hardware_manufacturers&rvslots=*&rvprop=content&formatversion=2");
                List<string> manufacturerList = new List<string>();
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
                        if (str.Contains(']')) str = str.Substring(0, str.IndexOf(']'));                                                        // egy sorban két hivatkozás!
                        if (str.Split('|').Count() > 1) str = str.Split('|')[1].Replace("[[", "").Replace("]]", "").Replace("*", "").Trim();    // Van pipe-al rövid név megadva? Ha igen, akkor az kell.
                        else str = str.Replace("[[", "").Replace("]]", "").Replace("*", "").Trim();                                             // Kivesszük a dupla szögletes szárójeleket

                        if (!(manufacturers.Keys.Contains(str, StringComparer.OrdinalIgnoreCase))) manufacturers.Add(str, new Manufacturer());
                    }
                }
                bool foundHomepage;
                foreach (KeyValuePair<string, Manufacturer> kvp in manufacturers)
                {
                    Console.Write(kvp.Key + ": ");
                    responseJson = GetWikiPage("https://en.wikipedia.org/w/api.php?format=json&action=query&prop=revisions&titles=%name%&rvslots=*&rvprop=content&formatversion=2".Replace("%name%", kvp.Key));
                    if (responseJson.query.pages[0].revisions == null) { Console.WriteLine(">>> Wiki page not found!<<<"); continue; }
                    lines = responseJson.query.pages[0].revisions[0].slots.main.content.Split("\n");
                    foundHomepage = false; string s2 = "";
                    foreach (string s in lines)
                    {
                        if ((s.StartsWith("|")) && ((s.Contains("homepage")) || (s.Contains("website"))) && (s.Contains("http")))
                        {
                            foundHomepage = true;
                            if (s.IndexOf("homepage") > 0)
                            {
                                s2 = s.Remove(0, s.IndexOf("homepage=") + 9);
                                //s2 = s2.Remove(0, s.IndexOf("http") - 2);
                                if (s2.Contains(" ")) s2 = s2.Remove(s2.IndexOf(" "), s2.Length - s2.IndexOf(" "));
                            }
                            if (s.IndexOf("website") > 0)
                            {
                                s2 = s.Remove(0, s.IndexOf("website=") + 8);
                                //s2 = s2.Remove(0, s.IndexOf("http") - 2);
                                if (s2.Contains(" ")) s2 = s2.Remove(s2.IndexOf(" "), s2.Length - s2.IndexOf(" "));
                            }
                            Console.WriteLine(s2);
                        }
                    }
                    if (!foundHomepage) Console.WriteLine(">>> No homepage/website tag! <<<");
                }
                //    Console.WriteLine(kvp.Key);
                Console.WriteLine("Done.");
            }

        }

        public RootObject GetWikiPage(string link)
        {
            var client = new WebClient();
            var response = client.DownloadString(link);

            RootObject responseJson = JsonConvert.DeserializeObject<RootObject>(response);
            return responseJson;
        }
    }
}
