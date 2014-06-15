using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GithubClient.Entity
{
    [JsonConverter(typeof(FileConverter))]
    public class File : INotifyPropertyChanged
    {
        private string name;
        private string url;
        private string type;
        private string icon;

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (name != value)
                {
                    name = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }

        public string Url
        {
            get
            {
                return url;
            }
            set
            {
                if (url != value)
                {
                    url = value;
                    NotifyPropertyChanged("Url");
                }
            }
        }

        public string Type
        {
            get
            {
                return type;
            }
            set
            {
                if (type != value)
                {
                    type = value;
                    NotifyPropertyChanged("Type");
                }
            }
        }

        public string Icon
        {
            get
            {
                return icon;
            }
            set
            {
                if (icon != value)
                {
                    icon = value;
                    NotifyPropertyChanged("Icon");
                }
            }
        }
    }
}
