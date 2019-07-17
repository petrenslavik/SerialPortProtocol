using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Filmobus_test.Annotations;

namespace Filmobus_test.Models
{
    public class InformationField:INotifyPropertyChanged
    {
        private Mode _mode;

        public Mode Mode
        {
            get => _mode;
            set => _mode = value;
        }

        public IMode Instance { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public enum Mode
    {
        Level, Sin, Triangle, RealTimeManual
    }
}
