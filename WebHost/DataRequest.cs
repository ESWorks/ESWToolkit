using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ESWToolbox.WebHost
{
    public static class DataRequest
    {
        public static string PostRequest(HttpClient client, string url, Dictionary<string, string> values)
        {

            var content = new FormUrlEncodedContent(values);

            var response = client.PostAsync(url, content).Result;


            return response.Content.ReadAsStringAsync().Result;
        }
        public static string PutRequest(HttpClient client, string url, Dictionary<string, string> values)
        {

            var content = new FormUrlEncodedContent(values);

            var response = client.PutAsync(url, content).Result;

            return response.Content.ReadAsStringAsync().Result;
        }
        public static string GetRequest(HttpClient client, string url, Dictionary<string, string> values)
        {
            if(url[url.Length-1] == '\\')
            {
                url = url.TrimEnd('\\');
            }

            if (values != null)
            {
                url += "?";

                foreach (var item in values)
                {
                    url += item.Key + "=" + item.Value + "&";
                }

                url = url.TrimEnd('&');
            }

            return client.GetStringAsync(url).Result;
        }
    }
}
