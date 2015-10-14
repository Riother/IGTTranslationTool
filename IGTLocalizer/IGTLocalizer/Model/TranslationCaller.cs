using System;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
namespace IGTLocalizer.Model
{
    class TranslationCaller
    {
        private static string key = "trnsl.1.1.20151012T200733Z.8237c82a081681fe.61c963349cac1614d2ac8b43adde3cf5981d4750";// This key is needed to access Yandex API. 
        public string[] textSent { get; set; } // not longer then 10000 charaters

        public ParsedJson Translate(string from, string to)
        {
            string connected = "";
            int linecount = 0;
            string parsed = null;
            foreach (string context in textSent)
            {
                if (linecount > 0 && linecount < textSent.Length - 1)
                {
                    connected += "%3F&text=" + context.Replace(' ', '+');
                }
                else
                {
                    connected += "&text=" + context.Replace(' ', '+');
                }

                linecount++;

            }

            Stream response = GenerateRequest(connected, from, to, false);
            StreamReader reader = new StreamReader(response, Encoding.UTF8);
            Console.WriteLine("Response from Yandex.com");
            Console.WriteLine(parsed = reader.ReadToEnd());
            ParsedJson ret = JsonConvert.DeserializeObject<ParsedJson>(parsed);
            response.Close();
            reader.Close();
            return ret;
        }


        public DetectedLanguage Detect()
        {
            string connected = "";
            string parsed = "";
            int linecount = 0;
            foreach (string context in textSent)
            {
                if (linecount > 0 && linecount < textSent.Length - 1)
                {
                    connected += "%3F&text=" + context.Replace(' ', '+');
                }
                else
                {
                    connected += "&text=" + context.Replace(' ', '+');
                }

                linecount++;

            }

            Stream response = GenerateRequest(connected, null, null, true);
            StreamReader reader = new StreamReader(response, Encoding.UTF8);
            Console.WriteLine("Response from Yandex.com");
            Console.WriteLine(parsed = reader.ReadToEnd());
            DetectedLanguage ret = JsonConvert.DeserializeObject<DetectedLanguage>(parsed);
            response.Close();
            reader.Close();
            return ret;
        }


        Stream GenerateRequest(string text, string langFrom, string langTo, bool detect)
        {
            HttpWebRequest request = null;
            if (detect)
                request = (HttpWebRequest)HttpWebRequest.Create("https://translate.yandex.net/api/v1.5/tr.json/detect?key=" + key + "" + text);
            else
                request = (HttpWebRequest)HttpWebRequest.Create("https://translate.yandex.net/api/v1.5/tr.json/translate?key=" + key + "&lang=" +
                    langFrom + "-" + langTo + "" + text);
            request.Method = "GET";
            request.AllowAutoRedirect = true;

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            return response.GetResponseStream();
        }
    }
}
