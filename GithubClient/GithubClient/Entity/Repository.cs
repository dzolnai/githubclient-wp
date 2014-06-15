using Newtonsoft.Json;
using System.ComponentModel;

[JsonConverter(typeof(RepositoryConverter))]
public class Repository : INotifyPropertyChanged
{

    private string url;
    private string owner;
    private string name;

    public event PropertyChangedEventHandler PropertyChanged;

    private void NotifyPropertyChanged(string info)
    {
        if (PropertyChanged != null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(info));
        }
    }

    public string Url
    {
        get { return this.url; }
        set
        {
            if (value != this.url)
            {
                this.url = value;
                NotifyPropertyChanged("Url");
            }
        }
    }

    public string Name
    {
        get { return this.name; }
        set
        {
            if (value != this.name)
            {
                this.name = value;
                NotifyPropertyChanged("Name");
            }
        }
    }

    public string Owner
    {
        get { return this.owner; }
        set
        {
            if (value != this.owner)
            {
                this.owner = value;
                NotifyPropertyChanged("Owner");
            }
        }
    }

}