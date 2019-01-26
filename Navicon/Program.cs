using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using HtmlAgilityPack;

namespace Navicon
{
    public class Source
    {
        static string LoadPage(string url)
        {
            var result = "";
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                try
                {
                    var response = (HttpWebResponse)request.GetResponse();
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var receiveStream = response.GetResponseStream();
                        if (receiveStream != null)
                        {
                            StreamReader readStream;
                            if (response.CharacterSet == null)
                                readStream = new StreamReader(receiveStream);
                            else
                                readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                            result = readStream.ReadToEnd();
                            readStream.Close();
                        }
                        response.Close();
                    }
                }
                catch (WebException e)
                {
                    Console.WriteLine(e);
                }
            }
            catch(UriFormatException e)
            {
                Console.WriteLine(e);
            }
            return result;
        }
        public List<string> GetData(string url)
        {
            var pageContent = LoadPage(url);
            var document = new HtmlDocument();
            document.LoadHtml(pageContent);
            HtmlNodeCollection links = document.DocumentNode.SelectNodes(".//a");
            int counter = 0;
            List<string> href_list = new List<string>();
            try
            {
                foreach (HtmlNode link in links)
                {
                    if (counter < 20)
                    {
                        var href = link.GetAttributeValue("href", "");
                        href_list.Add(href);
                        Console.WriteLine(href);
                        counter++;
                    }
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e);
            }
            return href_list;
        }
        public void SaveData(List<string> href_list)
        {
            string fileName = "output.txt";
            using (StreamWriter sw = new StreamWriter(fileName))
            {
                foreach (var item in href_list)
                {
                    sw.WriteLine(item);
                }
                sw.Close();
            }
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Source s = new Source();
            Console.WriteLine("Введите адрес сайта");
            string url = Console.ReadLine();
            List<string> data = s.GetData(url);
            s.SaveData(data);
        }
    }
}
