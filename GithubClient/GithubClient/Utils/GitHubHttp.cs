using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GithubClient.Utils
{
    public static class GitHubHttp
    {
        private static HttpClient httpClient;

        public static HttpClient GetHttpClient()
        {
            if (httpClient == null)
            {
                httpClient = new HttpClient();

                // Set the general settings of the client here

            }
            return httpClient;
        }

        public static HttpClient SetHttpClient(HttpClient newClient)
        {
            httpClient = newClient;
            return httpClient;
        }
    }
}
