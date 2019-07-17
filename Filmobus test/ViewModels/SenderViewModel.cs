using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Filmobus_test.Annotations;
using Filmobus_test.Models;

namespace Filmobus_test.ViewModels
{
    public class SenderViewModel:INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public SenderViewModel()
        {

        }
        
        public int Timeout { get; set; }
        public bool Ask { get; set; }
        public bool SendSettings { get; set; }
        public bool AskSettings { get; set; }
        public int TypeModule { get; set; }
        public int[] Flags { get; set; }
        public InformationField[] Fields { get; set; }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
