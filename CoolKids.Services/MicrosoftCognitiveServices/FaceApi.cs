using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CoolKids.Services.MicrosoftCognitiveServices
{
	public class FaceApi
	{
		const string ApiKey = "12d56fdef1b249bc87b89512dd23230a";

		public static async Task<string> Detect(byte [] byteData)
		{
			var client = new HttpClient();
			var queryString = new Dictionary<string, string> {
				{ "returnFaceId", string.Empty },
				{ "returnFaceLandmarks", string.Empty },
				{ "returnFaceAttributes", string.Empty },
			};

			// Request headers
			client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", ApiKey);

			// Request parameters
			queryString["returnFaceId"] = "true";
			queryString["returnFaceLandmarks"] = "true";
			//queryString["returnFaceAttributes"] = "age,gender,headPose,smile,facialHair,glasses";
			var uri = "https://api.projectoxford.ai/face/v1.0/detect?" + queryString;

			HttpResponseMessage response;

			using (var content = new ByteArrayContent(byteData))
			{
				content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
				response = await client.PostAsync(uri, content);
				return await response.Content.ReadAsStringAsync();
			}
		}


	}
}