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
            Repository = (Repository)NavigationHelper.getData();
            Setup();
        }

        /**
         * Sets up the view
         */
        private void Setup()
        {
            CurrentItems = new ObservableCollection<File>();
            string BaseUrl = Repository.Url + "/contents/";
            Parents = new List<string>();
            Parents.Add(BaseUrl);
            LoadFilesFrom(BaseUrl);
        }

        /**
         * Add an "up one directory" item to the list, if going up is possible.
         */
        private void AddUpActionIfNeeded()
        {
            if (Parents.Count > 1)
            {
                var goUp = new File();
                goUp.Url = Parents.ElementAt(Parents.Count - 1);
                goUp.Name = "Up one directory...";
                goUp.Icon = "/Assets/Icons/up.png";
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
                MessageBox.Show("Downloading in the background. A message will appear when done.");
            }
        }
    }

}