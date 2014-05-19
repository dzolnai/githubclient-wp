using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using GithubClient.Resources;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Diagnostics;
using Newtonsoft.Json;

namespace GithubClient
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        private async void Login_Button_Click(object sender, RoutedEventArgs e)
        {
            string userEmail = email.Text;
            string userPassword = password.Text;

            if (userEmail.Length == 0 || userPassword.Length == 0)
            {
                MessageBox.Show("Username or password is empty!");
                return;
            }

            progress.Visibility = System.Windows.Visibility.Visible;

            // set the HTTP request header from the details
            HttpClient httpClient = Utils.GetHttpClient();
            httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(String.Format("{0}:{1}", userEmail, userPassword))));

            // download the repositories
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync("https://api.github.com/user/repos");
                Debugger.Log(0, "Http", response.StatusCode.ToString());
                Console.WriteLine(response.StatusCode);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    List<Repository> repositories = JsonConvert.DeserializeObject<List<Repository>>(content);
                    Debugger.Log(0, "JSON", "Serialized " + repositories.Count + " repositories.");
                    // TODO send to the main page.
                }
                else
                {
                    MessageBox.Show("Username or password is invalid!");
                    string content = await response.Content.ReadAsStringAsync();
                    Debugger.Log(0, "Http", content);
                }
            }
            catch (WebException ex)
            {
                MessageBox.Show("Unexpected output received!");
                Debugger.Log(2, "Http", ex.Message);
            }
            progress.Visibility = System.Windows.Visibility.Collapsed;
        }
    }
}