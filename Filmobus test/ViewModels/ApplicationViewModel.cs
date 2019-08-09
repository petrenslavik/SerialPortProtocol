using Filmobus_test.HelpfulClasses;
using Filmobus_test.Models;
using Filmobus_test.Properties;
using Filmobus_test.Windows;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Filmobus_test.ViewModels
{
    public class ApplicationViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public PortViewModel Port { get; set; }
        public SenderViewModel Sender { get; set; }
        public ReaderViewModel Reader { get; set; }

        private SerialPort _port;

        public ApplicationViewModel()
        {
            _port = new SerialPort();
            Port = new PortViewModel(_port);
            Sender = new SenderViewModel(_port);
            Reader = new ReaderViewModel(_port);
        }

        public void Closing(object sender, CancelEventArgs args)
        {
            Port.SaveSettings();
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
