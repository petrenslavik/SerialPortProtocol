using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Filmobus_test.Annotations;

namespace Filmobus_test
{
    public class ApplicationViewModel:INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private Packet _deskPacket;
        private Packet _rtuPacket;
        private string _exception;

        public Packet LastPacket { get; private set; }

        public string SerialPortException
        {
            get => _exception;
            set
            {
                _exception = value;
                OnPropertyChanged();
            }
        }
        public Packet CurrentDeskPacket
        {
            get => _deskPacket;
            set
            {
                _deskPacket = value;
                LastPacket = value;
                OnPropertyChanged();
                OnPropertyChanged("LastPacket");
            }
        }

        public Packet CurrentRtuPacket
        {
            get => _rtuPacket;
            set
            {
                _rtuPacket = value;
                LastPacket = value;
                OnPropertyChanged();
                OnPropertyChanged("LastPacket");
            }
        }


        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
