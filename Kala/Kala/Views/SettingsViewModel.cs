using System.ComponentModel;
using System.Runtime.CompilerServices;
 
namespace Kala
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        public string Protocol
        {
            get { return Settings.Protocol; }
            set
            {
                if (Settings.Protocol == value)
                    return;

                Settings.Protocol = value;
                OnPropertyChanged();
            }
        }

        public string Server
        {
            get { return Settings.Server; }
            set
            {
                if (Settings.Server == value)
                    return;

                Settings.Server = value;
                OnPropertyChanged();
            }
        }

        public int Port
        {
            get { return Settings.Port; }
            set
            {
                if (Settings.Port == value)
                    return;

                Settings.Port = value;
                OnPropertyChanged();
            }
        }

        public string Username
        {
            get { return Settings.Username; }
            set
            {
                if (Settings.Username== value)
                    return;

                Settings.Username = value;
                OnPropertyChanged();
            }
        }

        public string Password
        {
            get { return Settings.Password; }
            set
            {
                if (Settings.Password == value)
                    return;

                Settings.Password = value;
                OnPropertyChanged();
            }
        }

        public string Sitemap
        {
            get { return Settings.Sitemap; }
            set
            {
                if (Settings.Sitemap == value)
                    return;

                Settings.Sitemap = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName]string name = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;
            changed(this, new PropertyChangedEventArgs(name));
        }
    }
}