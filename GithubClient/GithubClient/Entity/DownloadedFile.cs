using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GithubClient.Entity
{
    [DataContract] 
    public class DownloadedFile : INotifyPropertyChanged
    {
        private string name;
        private string url;   
        private string content;
        private string type;
        private List<string> containsFiles;

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        [DataMember]
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (value != name)
                {
                    name = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }

        [DataMember]
        public string Url
        {
            get
            {
                return url;
            }
            set
            {
                if (value != url)
                {
                    url = value;
                    NotifyPropertyChanged("Url");
                }
            }
        }

        [DataMember]
        public string Content
        {
            get
            {
                return content;
            }
            set
            {
                if (value != content)
                {
                    content = value;
                    NotifyPropertyChanged("Content");
                }
            }
        }

        [DataMember]
        public string Type
        {
            get
            {
                return type;
            }
            set
            {
                if (value != type)
                {
                    type = value;
                    NotifyPropertyChanged("Type");
                }
            }
        }

        [DataMember]
        public List<string> ContainsFiles
        {
            get
            {
                return containsFiles;
            }
            set
            {
                if (value != containsFiles)
                {
                    containsFiles = value;
                    NotifyPropertyChanged("ContainsFiles");
                }
            }
        }

        public string Icon { get; set; }
    }
}
