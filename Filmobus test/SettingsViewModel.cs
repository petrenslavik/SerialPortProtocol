using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Filmobus_test.Annotations;

namespace Filmobus_test
{
    public class SettingsViewModel:INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public SettingsViewModel(List<ObservableCollection<int>> deskSettingsValues, List<ObservableCollection<int>> rtuSettingsValues)
        {
            DeskSettingsValues = deskSettingsValues;
            RtuSettingsValues = rtuSettingsValues;
        }

        public List<ObservableCollection<int>> DeskSettingsValues { get; set; }
        public List<ObservableCollection<int>> RtuSettingsValues { get; set; }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}