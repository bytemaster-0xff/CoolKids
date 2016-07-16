using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace CoolKids.Services.MicrosoftCognitiveServices
{
	public class ComputerVisionApi
	{
		const string ApiKey = "d03b6ec652c84ce89b762586e4f6a85b";

		public static async Task<string> Detect(byte[] byteData)
		{
			var client = new HttpClient();
			var queryString = new Dictionary<string, string> {
				{ "visualFeatures", string.Empty },
				//{ "details", string.Empty },
			};

			// Request headers
			client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", ApiKey);

			// Request parameters
			queryString["visualFeatures"] = "Description, Faces"; // "Categories,Tags,Description,Faces";
			var uri = "https://api.projectoxford.ai/vision/v1.0/analyze?" + queryString;

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