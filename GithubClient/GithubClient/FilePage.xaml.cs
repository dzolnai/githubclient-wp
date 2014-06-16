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
using GithubClient.Utils;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Net.Http.Headers;

namespace GithubClient
{
    public partial class FilePage : PhoneApplicationPage
    {
        public FileContents File { get; set; }

        public FilePage()
        {
            InitializeComponent();
        }

        /**
 * Set up the page, get the data from the state if needed
 */
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (NavigationHelper.hasData() && NavigationHelper.getDataType() == NavigationHelper.DataType.FILE)
            {
                File = (FileContents)NavigationHelper.getData();
            }
            else if (!App.WasDormant)
            {
                // get the data from the state, because the app was tombstoned.
                string param = this.LoadState<string>("AuthHeader");
                if (param != null)
                {
                    GitHubHttp.GetHttpClient().DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", param);
                }
                File = this.LoadState<FileContents>("File");
            }
        }

        /**
         * Save the state of the page, in case we need to retrieve it when being tombstoned.
         */
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (GitHubHttp.GetHttpClient().DefaultRequestHeaders.Authorization != null)
            {
                this.SaveState("AuthHeader", GitHubHttp.GetHttpClient().DefaultRequestHeaders.Authorization.Parameter);
            }
            this.SaveState("File", File);
        }

        private void OnBrowserLoaded(object sender, RoutedEventArgs e)
        {
            string extension = Path.GetExtension(File.Url);
            bool isFileAnImage = extension.Equals("jpeg") || extension.Equals("jpg") || extension.Equals("png");
            string htmlContent;
            string fileContent = File.Content;
            if (isFileAnImage)
            {
                htmlContent = "<img alt=\"" + File.Name + "\" src=\"data:image/" + extension + ";base64," + fileContent + "\" />";
            }
            else
            {
                try
                {
                    byte[] data = Convert.FromBase64String(fileContent);
                    htmlContent = Encoding.UTF8.GetString(data, 0, data.Length);
                }
                catch (Exception ex)
                {
                    Debugger.Log(0, "Data", ex.Message);
                    MessageBox.Show("Can't parse the file contents!");
                    return;
                }
                // convert the spaces to non breaking ones
                htmlContent = htmlContent.Replace(" ", "&nbsp;");
                // convert the newlines to br tags
                htmlContent = htmlContent.Replace("\n", "<br/>");
                // change the font with the html
                htmlContent = "<html> \n"
                        + "<head> \n"
                        + "<style type=\"text/css\"> \n"
                        + "body {font-family: \"Trebuchet Ms\"; font-size: 12;}\n"
                        + "</style> \n"
                        + "</head> \n"
                        + "<body>" + htmlContent + "</body> \n"
                        + "</html>";
            }
            Viewer.NavigateToString(htmlContent);
        }
    }
}