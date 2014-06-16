using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using GithubClient.Utils;
using GithubClient.Entity;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace GithubClient
{
    public partial class BrowsePage : PhoneApplicationPage
    {
        public Repository Repository { get; set; }
        public ObservableCollection<File> CurrentItems { get; set; }

        private List<string> Parents { get; set; }

        public BrowsePage()
        {
            InitializeComponent();
        }

        /**
         * Set up the page, get the data from the state if needed
         */
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (NavigationHelper.hasData() && NavigationHelper.getDataType() == NavigationHelper.DataType.REPOSITORY)
            {
                Repository = (Repository)NavigationHelper.getData();
                CurrentItems = new ObservableCollection<File>();
                string BaseUrl = Repository.Url + "/contents/";
                Parents = new List<string>();
                Parents.Add(BaseUrl);
                LoadFilesFrom(BaseUrl);
            }
            else if (!App.WasDormant)
            {
                // get the data from the state, because the app was tombstoned.
                string param = this.LoadState<string>("AuthHeader");
                if (param != null)
                {
                    GitHubHttp.GetHttpClient().DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", param);
                }
                Parents = this.LoadState<List<String>>("OnlineParents");
                CurrentItems = this.LoadState<ObservableCollection<File>>("OnlineCurrentItems");
                Repository = this.LoadState<Repository>("OnlineRepository");
            }
        }

        /**
         * Save the state of the page, in case we need to retrieve it when being tombstoned.
         */
        protected override void OnNavigatedFrom(NavigationEventArgs e){
            if (GitHubHttp.GetHttpClient().DefaultRequestHeaders.Authorization != null)
            {
                this.SaveState("AuthHeader", GitHubHttp.GetHttpClient().DefaultRequestHeaders.Authorization.Parameter);
            }
            this.SaveState("OnlineParents", Parents);
            this.SaveState("OnlineCurrentItems", CurrentItems);
            this.SaveState("OnlineRepository", Repository);
        }

        /**
         * Add an "up one directory" item to the list, if going up is possible.
         */
        private void AddUpActionIfNeeded()
        {
            if (Parents.Count > 1)
            {
                var goUp = new File();
                bool isLightTheme = (Visibility)Application.Current.Resources["PhoneLightThemeVisibility"] == Visibility.Visible;
                goUp.Url = Parents.ElementAt(Parents.Count - 1);
                goUp.Name = "Up one directory...";
                goUp.Icon = isLightTheme ? "/Assets/Icons/up-light.png" : "/Assets/Icons/up-dark.png";
                goUp.Type = "UP";
                CurrentItems.Add(goUp);
            }
        }

        /**
         * Download the items, and put them into the new items array.
         * The collection updates the listbox, thus new items are displayed.
         */
        private async void LoadFilesFrom(string url)
        {
            try
            {
                HttpResponseMessage response = await GitHubHttp.GetHttpClient().GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    List<File> files = JsonConvert.DeserializeObject<List<File>>(content);
                    CurrentItems.Clear();
                    AddUpActionIfNeeded();
                    foreach (File file in files)
                    {
                        CurrentItems.Add(file);
                    }
                    Debugger.Log(0, "Data", "Loaded " + CurrentItems.Count + " files.\n");
                }
            }
            catch (WebException ex)
            {
                Debugger.Log(0, "Web", ex.Message);
                MessageBox.Show("Something's not right");
            }
        }

        /**
         * When the user selects an item:
         * - go one directory up, if it was that item
         * - load the items of the directory, if it was a directory
         * - open the file, if it was a file
         */
        private void OnItemSelected(object sender, SelectionChangedEventArgs e)
        {
            int index = ((ListBox)sender).SelectedIndex;
            if (index < 0)
            {
                // when item becomes unselected, this gets called with -1
                return;
            }
            File selectedFile = CurrentItems.ElementAt(index);
            // if navigating up between the directories, we use the parents stack to determine where to go
            if (selectedFile.Type.Equals("UP"))
            {
                int indexToGet = Parents.Count - 2;
                string urlToDownload = Parents.ElementAt(indexToGet);
                Parents.RemoveAt(Parents.Count - 1);
                LoadFilesFrom(urlToDownload);
            }
            else if (selectedFile.Type.Equals("dir"))
            {
                // if going down, we add the item to the parents stack, and load the items
                Parents.Add(selectedFile.Url);
                LoadFilesFrom(selectedFile.Url);
            }
            else
            {
                // type is a file, we download and open it
                DownloadFile(selectedFile);
            }
            Files.SelectedIndex = -1;
        }

        private async void DownloadFile(File file)
        {
            try
            {
                HttpResponseMessage response = await GitHubHttp.GetHttpClient().GetAsync(file.Url);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    FileContents fileContents = JsonConvert.DeserializeObject<FileContents>(content);
                    fileContents.Url = file.Url;
                    NavigationHelper.setNavigationData(fileContents, NavigationHelper.DataType.FILE);
                    NavigationService.Navigate(new Uri("/FilePage.xaml", UriKind.Relative));
                }
                else
                {
                    Debugger.Log(0, "Web", "" + response.StatusCode);
                    MessageBox.Show("There's something wrong with the connection!");
                }
            }
            catch (WebException ex)
            {
                Debugger.Log(0, "Web", ex.Message);
                MessageBox.Show("Can't download the file!");
            }
        }

        private void SaveRepo(object sender, EventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Would you like to make this repository offline available?",
                                                      Repository.Name, MessageBoxButton.OKCancel);

            if (result == MessageBoxResult.OK)
            {
                GitHubHttp.DownloadFilesRecursivelyFrom(Repository.Url + "/contents/", Repository.Name, true);
                MessageBox.Show("Downloading in the background. A message will appear when done.");
            }
        }
    }

}