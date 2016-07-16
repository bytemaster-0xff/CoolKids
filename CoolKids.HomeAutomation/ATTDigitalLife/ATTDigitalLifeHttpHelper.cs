using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CoolKids.HomeAutomation
{
    public class ATTDigitalLifeHttpHelper
    {
        public static string authToken = "";
        public static string requestToken = "";
        public static string appKey = "";

        public static T Get<T>(string serviceBaseURI, string uriFragment) where T : class
        {
            var task = Task<T>.Run(async () => { return await GetAsync<T>(serviceBaseURI, uriFragment); });

            task.Wait();

            return task.Result;
        }

        public static async Task<T> GetAsync<T>(string serviceBaseURI, string uriFragment) where T : class
        {
            T result = default(T);

            using (var httpClient = new ATTDigitalLifeHTTPClient(authToken, requestToken, appKey, true))
            {
                string requestUrl = serviceBaseURI + uriFragment;

                var uri = new Uri(requestUrl);

                var response = await httpClient.GetAsync(uri);

                response.EnsureSuccessStatusCode();

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();

                    result = JSONSerializationHelper.Deserialize<T>(responseContent);
                }
                else
                {
                    throw new HttpRequestException();
                }
            }

            return result;
        }

        public static async Task<string> GetAsync(string serviceBaseURI, string uriFragment, bool returnJson) 
        {
            string result = "";

            using (var httpClient = new ATTDigitalLifeHTTPClient(authToken, requestToken, appKey, returnJson))
            {
                string requestUrl = serviceBaseURI + uriFragment;

                var uri = new Uri(requestUrl);

                var response = await httpClient.GetAsync(uri);

                response.EnsureSuccessStatusCode();

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    result = responseContent;
                }
                else
                {
                    throw new HttpRequestException();
                }
            }

            return result;
        }


        public static async Task<string> PostAsync(string serviceBaseURI, string uriFragment, HttpContent postBodyContent)
        {
            string result = "";

            using (var httpClient = new ATTDigitalLifeHTTPClient(authToken, requestToken, appKey, true))
            {
                string requestUrl = serviceBaseURI + uriFragment;

                var uri = new Uri(requestUrl);
                
                var response = await httpClient.PostAsync(uri, postBodyContent);

                response.EnsureSuccessStatusCode();

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    result = responseContent;
                }
                else
                {
                    throw new HttpRequestException();
                }
            }

            return result;
        }

    }
}
