﻿using System;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
namespace IGTLocalizer.Model
{
    class TranslationCaller
    {
        private static string key = "trnsl.1.1.20151012T200733Z.8237c82a081681fe.61c963349cac1614d2ac8b43adde3cf5981d4750";// This key is needed to access Yandex API. 

        //TODO check character length is not over 10000
        //when translating a line you need to have the &text= before the text 
        //
        public string TranslateLine(string content, string from, string to, string additional = "")
        {
            string text = additional + "&text=" + content.Replace(' ', '+');
            return GetTranslationFromRequest(text, from, to, false);
        }

        public string[] TranslateMultiLines(string[] lines, string from, string to) {
            string[] translated = lines;
            int numLines = lines.Length;
            for(int i = 0; i < numLines; i ++){
                if(i > 0 && i < numLines - 1){
                    translated[i] = TranslateLine(lines[i].Replace(' ', '+'), from, to, "%3F");
                }else{
                    translated[i] = TranslateLine(lines[i].Replace(' ', '+'), from, to);
                }
            }
            return translated;
        }

        public DetectedLanguage Detect(string line)
        {
            return JsonConvert.DeserializeObject<DetectedLanguage>(GetTranslationFromRequest(line, null, null, true));
        }

        //generates a translation request and returns the response
        private HttpWebResponse GetTranslationResponse(string text, string langFrom, string langTo, bool detect)
        {
            HttpWebRequest request = (detect) ? (HttpWebRequest)HttpWebRequest.Create("https://translate.yandex.net/api/v1.5/tr.json/detect?key=" + key + "" + text) :
                (HttpWebRequest)HttpWebRequest.Create("https://translate.yandex.net/api/v1.5/tr.json/translate?key=" + key + "&lang=" + langFrom + "-" + langTo + "" + text);
            request.Method = "GET";
            request.AllowAutoRedirect = true;

            return (HttpWebResponse)request.GetResponse();
        }

        private string GetTranslationFromRequest(string text, string langFrom, string langTo, bool detect)
        {
            //make request and get response
            HttpWebResponse translationResponse = GetTranslationResponse(text, langFrom, langTo, detect);
            StreamReader responseReader = new StreamReader(translationResponse.GetResponseStream(), Encoding.UTF8);

            //parse response into string
            string parsed = responseReader.ReadToEnd();
            Console.WriteLine("Response from Yandex.com: \n\n" + parsed);

            //clean up
            translationResponse.Close();
            responseReader.Close();

            return parsed;
        }

        //string connected = "";
        //int linecount = 0;
        //foreach (string context in lines)
        //{
        //    if (linecount > 0 && linecount < textSent.Length - 1)
        //    {
        //        connected += "%3F&text=" + context.Replace(' ', '+');
        //    }
        //    else
        //    {
        //        connected += "&text=" + context.Replace(' ', '+');
        //    }

        //    linecount++;
        //}
        
    }
}
