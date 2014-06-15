﻿using System;
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

namespace GithubClient
{
    public partial class RepositoryPage : PhoneApplicationPage
    {
        public ObservableCollection<Repository> Data { get; set; }
        public ObservableCollection<DownloadedFile> OfflineData { get; set; }

        public RepositoryPage()
        {
            InitializeComponent();         
            // in online mode, this should be set
            if (NavigationHelper.hasData() && NavigationHelper.getDataType() == GithubClient.Utils.NavigationHelper.DataType.REPOSITORY_LIST)
            {
                Data = new ObservableCollection<Repository>((List<Repository>)NavigationHelper.getData());
            } 
            else
            {
                NoOnlineItemsText.Visibility = Visibility.Visible;
            }
            // read the offline repos from the storage.
            OfflineData =  new ObservableCollection<DownloadedFile>(StorageUtils.GetAllRepos());
            // if we have some offline repos, hide the no items text
            if (OfflineData.Count > 0)
            {
                NoOfflineItemsText.Visibility = Visibility.Collapsed;
            }
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            // user can't go back to the login screen
            NavigationService.RemoveBackEntry();
        }

        private void Repository_Selected(object sender, SelectionChangedEventArgs e)
        {
            int Index = Repositories.SelectedIndex;
            Repository selectedRepo = Data.ElementAt(Index);
            NavigationHelper.setNavigationData(selectedRepo, NavigationHelper.DataType.REPOSITORY);
            NavigationService.Navigate(new Uri("/BrowsePage.xaml", UriKind.Relative));
            Debugger.Log(0, "Data", selectedRepo.Url + "\n");
        }
    }
}