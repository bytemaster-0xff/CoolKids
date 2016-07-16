using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CoolKids.HomeAutomation
{

    public class ATTDigitalLifeHTTPClient : HttpClient
    {
        private int TimeoutDelaySeconds = 30;


        public ATTDigitalLifeHTTPClient(string authToken, string requestToken, string appKey, bool returnJson)
        {
            this.Timeout = new TimeSpan(0, 0, TimeoutDelaySeconds);

            if (String.IsNullOrWhiteSpace(authToken) == false)
                this.DefaultRequestHeaders.TryAddWithoutValidation("Authtoken", authToken);

            if (String.IsNullOrWhiteSpace(requestToken) == false)
                this.DefaultRequestHeaders.TryAddWithoutValidation("Requesttoken", requestToken);

            if (String.IsNullOrWhiteSpace(appKey) == false)
                this.DefaultRequestHeaders.TryAddWithoutValidation("Appkey", appKey);


            if (returnJson == true)
            {
                this.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }
            else
            {
                this.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));
            }
        }
   

    }
}
