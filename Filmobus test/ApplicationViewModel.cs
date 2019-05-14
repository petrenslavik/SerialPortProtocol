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

        public Packet CurrentDeskPacket
        {
            get => _deskPacket;
            set
            {
                _deskPacket = value;
                OnPropertyChanged();
            }
        }

        public Packet CurrentRtuPacket
        {
            get => _rtuPacket;
            set
            {
                _rtuPacket = value;
                OnPropertyChanged();
            }
        }


        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
