using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using HtmlAgilityPack;

namespace ShopAPI
{
    public class Manufacturer
    {
        public string wikiLink { get; set; }
        public string website { get; set; }
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

    public class CpuProductLine
    {
        public string manufacturer { get; set; }
        public string line { get; set; }
    }

    class WikiAPI
    {
        public Dictionary<string, Manufacturer> manufacturers = new Dictionary<string, Manufacturer>();
        public List<CpuProductLine> cpu_prod_lines = new List<CpuProductLine>();

        public void GetManufacturers()
        {
            using (WebClient wc = new WebClient())
            {
                Console.WriteLine("Getting manufacturers list...");
                DateTime start = DateTime.Now;
                RootObject responseJson = GetWikiPage("https://en.wikipedia.org/w/api.php?format=json&action=query&prop=revisions&titles=List_of_computer_hardware_manufacturers&rvslots=*&rvprop=content&formatversion=2");
                List<string> manufacturerList = new List<string>();
                string[] lines = responseJson.query.pages[0].revisions[0].slots.main.content.Split("\n");
                string str, str2 = ""; string wikilink;
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
                        if (!str.Contains("Inc.")) str = str.Replace("Inc", "Inc.");
                        if (str.Split('|').Count() > 1)                                                                                         // Van pipe-al rövid név megadva? Ha igen, a második a cégnév, első a wiki link
                        {
                            str2 = str.Split('|')[1].Replace("[[", "").Replace("]]", "").Replace("*", "").Trim();
                            wikilink = str.Split('|')[0].Replace("[[", "").Replace("]]", "").Replace("*", "").Trim().Replace(" ", "_");
                        }
                        else
                        {
                            str2 = str.Replace("[[", "").Replace("]]", "").Replace("*", "").Trim();                                             // Kivesszük a dupla szögletes szárójeleket
                            wikilink = str2;
                        }

                        if (!(manufacturers.Keys.Contains(str2, StringComparer.OrdinalIgnoreCase))) 
                        { 
                            manufacturers.Add(str2, new Manufacturer()); manufacturers[str2].wikiLink = wikilink;
                            Console.WriteLine(str2);
                        }
                        //else manufacturers[str2].wikiLink = wikilink;
                    }
                }
                Console.WriteLine("Done.");
                string nodePath; 
                bool runX = true, runY; 
                int x = 2, y;
                while (runX)
                {
                    y = 1;
                    runY = true;
                    while (runY)
                    {
                        nodePath = @"/html/body/main/div[2]/div[1]/div[1]/div/div/div[%x%]/div[2]/div/div[%y%]/div/a";
                        if (!FetchDatasheets(@"https://www.intel.com/content/www/us/en/products/details/processors.html", nodePath.Replace("%x%", x.ToString()).Replace("%y%", y.ToString())))
                        {
                            runY = false;
                            if (y == 1) runX = false;
                        }
                        else y++;
                    }
                    x++;
                }
            //    bool foundHomepage;
            //    int good = 0, badformat = 0, repaired = 0, total; bool bad = false;
            //    total = manufacturers.Count();
            //    foreach (KeyValuePair<string, Manufacturer> kvp in manufacturers)
            //    {
            //        //Console.Write(kvp.Key + ": ");
            //        responseJson = GetWikiPage("https://en.wikipedia.org/w/api.php?format=json&action=query&prop=revisions&titles=%name%&rvslots=*&rvprop=content&formatversion=2".Replace("%name%", kvp.Value.wikiLink));
            //        if (responseJson.query != null)
            //        { if (responseJson.query.pages[0].revisions == null) { Console.WriteLine(kvp.Key + ": >>> Wiki page not found!<<<"); continue; } }
            //        else
            //        {
            //            Console.WriteLine(kvp.Key + ">>> Query is empty.");
            //            continue;
            //        }
            //        lines = responseJson.query.pages[0].revisions[0].slots.main.content.Split("\n");
            //        foundHomepage = false; string s2 = "", s1 = "";
            //        foreach (string s in lines)
            //        {
            //            if ((s.StartsWith("|")) && ((s.Contains("homepage")) || (s.Contains("website"))))
            //            {
            //                foundHomepage = true;
            //                s1 = s;
            //                if (!(s1.Contains("http")) && !(s1.Contains("www")))
            //                {
            //                    s1 = s1.Replace("{{url|", " http://").Replace("{{Url|", " http://").Replace("{{URL|", " http://");
            //                    Console.WriteLine(kvp.Key + ": >>> Bad homepage format: " + s + ". Trying to repair:" + s1 + " <<<");
            //                    badformat++; bad = true;
            //                    if (!s1.Contains("http"))
            //                        break;
            //                }
            //                //good++;
            //                var links = s1.Replace("|", "| ").Replace("[", "[ ").Split("\t\n ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Where(s => s.StartsWith("http://") || s.StartsWith("www.") || s.StartsWith("https://"));
            //                if (links.Count() > 0) 
            //                    s2 = links.First().Replace("]", "").Replace("}", "").Trim();
            //                if (s2.Length > 0)
            //                    if (manufacturers[kvp.Key].website != s2)
            //                    {
            //                        Console.WriteLine(kvp.Key + ": " + s2);
            //                        manufacturers[kvp.Key].website = s2;
            //                        good++;
            //                    }
            //                if (bad) { bad = false; repaired++; }
            //            }
            //            //if (foundHomepage) break;
            //        }
            //        if (!foundHomepage) Console.WriteLine(kvp.Key + ": >>> No homepage/website tag! <<<");
            //    }
            //    //    Console.WriteLine(kvp.Key);
            //    Console.WriteLine("Done. " + good + "/" + total + " links found (" + badformat + " with bad format, " + repaired + " repaired). Running time: " + Convert.ToInt32((DateTime.Now - start).TotalSeconds) + "s.");
            }

        }

        public RootObject GetWikiPage(string link)
        {
            var client = new WebClient();
            var response = client.DownloadString(link);
            RootObject responseJson = new RootObject();
            try { responseJson = JsonConvert.DeserializeObject<RootObject>(response); } catch { }
            return responseJson;
        }

        private bool FetchDatasheets(string link, string nodePath)
        {
            HtmlWeb web = new HtmlWeb();
            var htmlDoc = web.Load(link);
            var node = htmlDoc.DocumentNode.SelectSingleNode(nodePath);
            //Console.WriteLine(node.Name + " -> " + node.InnerText);
            if (node != null)
            {
                Console.WriteLine(node.Name + " -> " + node.Attributes["href"].Value);
                return true;
            }
            return false;
        }
    }
}
