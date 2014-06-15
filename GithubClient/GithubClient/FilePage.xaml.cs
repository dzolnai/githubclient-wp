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

namespace GithubClient
{
    public partial class FilePage : PhoneApplicationPage
    {
        public FileContents File { get; set; }

        public FilePage()
        {
            File = (FileContents)NavigationHelper.getData();
            InitializeComponent();
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