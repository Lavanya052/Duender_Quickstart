using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace MyApp.Namespace
{
    public class CallApiModel(IHttpClientFactory httpClientFactory) : PageModel
    {
        public string Json = string.Empty;

        public async Task OnGet()
        {
            //var tokenInfo = await HttpContext.GetUserAccessTokenAsync(); // get all access token
            //var client = new HttpClient();
            //client.SetBearerToken(tokenInfo.AccessToken!); // if not request token

            var client = httpClientFactory.CreateClient("apiClient");  // this will handle all get,create refresh 

            var content = await client.GetStringAsync("https://localhost:6001/identity");

            var parsed = JsonDocument.Parse(content);
            var formatted = JsonSerializer.Serialize(parsed, new JsonSerializerOptions { WriteIndented = true });

            Json = formatted;
        }

    }

}
