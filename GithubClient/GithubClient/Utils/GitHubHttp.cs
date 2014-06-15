using GithubClient.Entity;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GithubClient.Utils
{
    public static class GitHubHttp
    {
        private static HttpClient httpClient;

        private static int activeCalls = 0;

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

        public static async void DownloadFilesRecursivelyFrom(string url, string name, bool isRepoRoot)
        {
            activeCalls++;
            string urlToFetch = url; // HttpUtility.UrlEncode(url);
            HttpResponseMessage response = await GetHttpClient().GetAsync(urlToFetch);
            string httpContent = await response.Content.ReadAsStringAsync();
            ParseDownloadedFile(httpContent, url, name, isRepoRoot);
        }

        private static void ParseDownloadedFile(string json, string url, string name, bool isRepoRoot)
        {
            var token = JToken.Parse(json);
            DownloadedFile file = new DownloadedFile();
            file.Url = url;
            file.Name = name;
            if (token is JArray)
            {
                // its a directory
                file.Type = "dir";
                IEnumerable<File> filesInDir = token.ToObject<List<File>>();
                List<string> names = new List<string>();
                file.ContainsFiles = new List<string>();
                foreach (var fileInDir in filesInDir)
                {
                    names.Add(fileInDir.Name);
                    file.ContainsFiles.Add(fileInDir.Url);
                }
                for (int i = 0; i < file.ContainsFiles.Count; ++i)
                {
                    string fileUrl = file.ContainsFiles.ElementAt(i);
                    string fileName = names.ElementAt(i);
                    DownloadFilesRecursivelyFrom(fileUrl, fileName, false);
                }
            }
            else if (token is JObject)
            {
                // its a file
                file.Type = "file";
                FileContents downloadedFile = token.ToObject<FileContents>();
                file.Content = downloadedFile.Content;
            }
            else
            {
                Debugger.Log(0, "Error", "Can't parse token\n");
            }

            if (isRepoRoot)
            {
                // save the repo to the repos directory
                StorageUtils.SaveRepo(file);
            }
            else
            {
                // save the file to another directory
                StorageUtils.SaveFile(file);
            }
            //Debugger.Log(0, "Data", "DOWNLOADED FILE: " + file.Name + "\n");
            activeCalls--;
            if (activeCalls == 0)
            {
                MessageBox.Show("Repository downloaded.");
            }
        }
    }
}


