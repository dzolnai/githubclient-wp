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
using System.Diagnostics;
using System.Collections.ObjectModel;
using GithubClient.Entity;
using System.Net.Http;
using System.Net.Http.Headers;

namespace GithubClient
{
    public partial class RepositoryPage : PhoneApplicationPage
    {
        public ObservableCollection<Repository> Data { get; set; }
        public ObservableCollection<DownloadedFile> OfflineData { get; set; }

        public RepositoryPage()
        {
            InitializeComponent();         
            
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            // user can't go back to the login screen
            NavigationService.RemoveBackEntry();
            // in online mode, this should be set
            if (NavigationHelper.hasData() && NavigationHelper.getDataType() == GithubClient.Utils.NavigationHelper.DataType.REPOSITORY_LIST)
            {
                Data = new ObservableCollection<Repository>((List<Repository>)NavigationHelper.getData());
            }
            // if app was tombstoned, try to load the data from there.
            if (Data == null && App.WasDormant == false)
            {
                string param = this.LoadState<string>("AuthHeader");
                if (param != null)
                {
                    GitHubHttp.GetHttpClient().DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", param);
                }
                Data = this.LoadState<ObservableCollection<Repository>>("Data");
            }

            // check if we have something to work with
            if (Data == null)
            {
                NoOnlineItemsText.Visibility = Visibility.Visible;
            }
            // read the offline repos from the storage.
            if (OfflineData == null)
            {
                OfflineData = new ObservableCollection<DownloadedFile>(StorageUtils.GetAllRepos());
            }
            else
            {
                OfflineData.Clear();
                foreach (var item in StorageUtils.GetAllRepos())
                {
                    OfflineData.Add(item);
                }
            }
            // if we have some offline repos, hide the no items text
            if (OfflineData.Count > 0)
            {
                NoOfflineItemsText.Visibility = Visibility.Collapsed;
            }
            else
            {
                NoOfflineItemsText.Visibility = Visibility.Visible;
            }
        }
        
        /**
         * Save the state of the page
         */
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (GitHubHttp.GetHttpClient().DefaultRequestHeaders.Authorization != null)
            {
                this.SaveState("AuthHeader", GitHubHttp.GetHttpClient().DefaultRequestHeaders.Authorization.Parameter);
            }
            this.SaveState("Data", Data);
        }

        private void Repository_Selected(object sender, SelectionChangedEventArgs e)
        {
            int Index = Repositories.SelectedIndex;
            if (Index < 0)
            {
                return;
            }
            Repository selectedRepo = Data.ElementAt(Index);
            NavigationHelper.setNavigationData(selectedRepo, NavigationHelper.DataType.REPOSITORY);
            Repositories.SelectedIndex = -1; 
            NavigationService.Navigate(new Uri("/BrowsePage.xaml", UriKind.Relative));
        }

        private void Offline_Repository_Selected(object sender, SelectionChangedEventArgs e)
        {
            int Index = OfflineRepositories.SelectedIndex;
            if (Index < 0)
            {
                return;
            }
            DownloadedFile selectedRepo = OfflineData.ElementAt(Index);
            NavigationHelper.setNavigationData(selectedRepo, NavigationHelper.DataType.DOWNLOADED_FILE);
            OfflineRepositories.SelectedIndex = -1;
            NavigationService.Navigate(new Uri("/OfflineBrowsePage.xaml", UriKind.Relative));
            
        }

        private void OnLogoutClick(object sender, EventArgs e)
        {
            GitHubHttp.GetHttpClient().DefaultRequestHeaders.Authorization = null;
            NavigationService.Navigate(new Uri("/LoginPage.xaml", UriKind.Relative));
        }
    }
}