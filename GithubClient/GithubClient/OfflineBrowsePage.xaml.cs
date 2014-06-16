using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using GithubClient.Entity;
using System.Collections.ObjectModel;
using GithubClient.Utils;
using System.Diagnostics;
using System.Net.Http.Headers;

namespace GithubClient
{
    public partial class OfflineBrowsePage : PhoneApplicationPage
    {
        public ObservableCollection<DownloadedFile> CurrentItems { get; set; }
        public string RepositoryName { get; set; }
        private List<String> Parents { get; set; }

        public OfflineBrowsePage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            Parents = new List<string>();
            CurrentItems = new ObservableCollection<DownloadedFile>();
            if (NavigationHelper.hasData() && NavigationHelper.getDataType() == NavigationHelper.DataType.DOWNLOADED_FILE)
            {
                DownloadedFile selectedRepo = (DownloadedFile)NavigationHelper.getData();
                RepositoryName = selectedRepo.Name;
                Parents.Add(selectedRepo.Url);
                GetSubItemsForItem(selectedRepo);
            }
            else if (!App.WasDormant)
            {
                // get the data from the state, because the app was tombstoned.
                string param = this.LoadState<string>("AuthHeader");
                GitHubHttp.GetHttpClient().DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", param);
                Parents = this.LoadState<List<String>>("OfflineParents");
                CurrentItems = this.LoadState<ObservableCollection<DownloadedFile>>("OfflineCurrentItems");
                RepositoryName = this.LoadState<string>("OfflineRepositoryName");
            }
        }

        /**
         * Save the state of the page, in case we need to retrieve it when being tombstoned.
         */
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.SaveState("AuthHeader", GitHubHttp.GetHttpClient().DefaultRequestHeaders.Authorization.Parameter);
            this.SaveState("OfflineParents", Parents);
            this.SaveState("OfflineCurrentItems", CurrentItems);
            this.SaveState("OfflineRepositoryName", RepositoryName);
        }


        /**
         * Add an "up one directory" item to the list, if going up is possible.
         */
        private void AddUpActionIfNeeded()
        {
            if (Parents.Count > 1)
            {
                var goUp = new DownloadedFile();
                bool isLightTheme = (Visibility)Application.Current.Resources["PhoneLightThemeVisibility"] == Visibility.Visible;
                goUp.Url = Parents.ElementAt(Parents.Count - 1);
                goUp.Name = "Up one directory...";
                goUp.Icon = isLightTheme ? "/Assets/Icons/up-light.png" : "/Assets/Icons/up-dark.png";
                goUp.Type = "UP";
                CurrentItems.Add(goUp);
            }
        }


        /**
         * Gets the subitems from the isolated storage for the item provided.
         */
        private void GetSubItemsForItem(DownloadedFile item)
        {
            bool isLightTheme = (Visibility)Application.Current.Resources["PhoneLightThemeVisibility"] == Visibility.Visible;
            List<DownloadedFile> subItems = new List<DownloadedFile>();
            foreach (string url in item.ContainsFiles)
            {
                DownloadedFile subItem = StorageUtils.GetFileByUrl(url);
                if (subItem.Type.Equals("dir"))
                {
                    subItem.Icon = isLightTheme ? "/Assets/Icons/directory-light.png" : "/Assets/Icons/directory-dark.png";
                }
                else
                {
                    subItem.Icon = isLightTheme ? "/Assets/Icons/file-light.png" : "/Assets/Icons/file-dark.png";
                }
                subItems.Add(subItem);
            }
            CurrentItems.Clear();
            AddUpActionIfNeeded();
            foreach (DownloadedFile file in subItems)
            {
                CurrentItems.Add(file);
            }
        }

        /**
         * Deletes the repo when being clicked
         */
        private void DeleteRepo(object sender, EventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are your sure you want to delete this repository from your local storage?",
                                                      RepositoryName, MessageBoxButton.OKCancel);

            if (result == MessageBoxResult.OK)
            {
                string urlToGet = Parents.ElementAt(0);
                DownloadedFile repoRoot = StorageUtils.GetFileByUrl(urlToGet);
                StorageUtils.DeleteFilesFromRoot(repoRoot);
                MessageBox.Show("Repository deleted.");
                NavigationService.GoBack();
            }
        }

        private void OnItemSelected(object sender, SelectionChangedEventArgs e)
        {
            int index = Files.SelectedIndex;
            if (index < 0)
            {
                return;
            }

            DownloadedFile selectedFile = CurrentItems.ElementAt(index);
            if (selectedFile.Type.Equals("UP"))
            {
                string urlToGet = Parents.ElementAt(Parents.Count - 2);
                Parents.RemoveAt(Parents.Count - 1);
                DownloadedFile fileToLoad = StorageUtils.GetFileByUrl(urlToGet);
                GetSubItemsForItem(fileToLoad);
            }
            else if (selectedFile.Type.Equals("dir"))
            {
                Parents.Add(selectedFile.Url);
                GetSubItemsForItem(selectedFile);
            }
            else
            {
                FileContents convertedContents = new FileContents();
                convertedContents.Content = selectedFile.Content;
                convertedContents.Name = selectedFile.Name;
                convertedContents.Url = selectedFile.Url;
                NavigationHelper.setNavigationData(convertedContents, NavigationHelper.DataType.FILE);
                NavigationService.Navigate(new Uri("/FilePage.xaml", UriKind.Relative));
            }
            Files.SelectedIndex = -1;
        }
    }
}