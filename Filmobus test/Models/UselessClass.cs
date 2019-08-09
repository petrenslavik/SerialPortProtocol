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
    public class UselessClass:INotifyPropertyChanged
    {
        private bool _isVisible;

        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                _isVisible = value;
                OnPropertyChanged();
            }
        }

        public int Index { get; private set; }
        public bool IsDesk { get; private set; }

        public UselessClass(int index, bool isDesk)
        {
            IsVisible = false;
            Index = index;
            IsDesk = isDesk;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
