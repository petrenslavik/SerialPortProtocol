using Filmobus_test.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Filmobus_test.Models
{
    public class InformationField : INotifyPropertyChanged
    {
        private Mode _mode;

        public List<Mode> Modes { get; set; }

        public Mode SelectedMode
        {
            get => _mode;
            set
            {
                _mode = value;
                switch (_mode)
                {
                    case Mode.Level:
                        SelectedViewModel = new Level();
                        break;
                    case Mode.Sin:
                        SelectedViewModel = new Sin();
                        break;
                    case Mode.Triangle:
                        SelectedViewModel = new Triangle();
                        break;
                    case Mode.RealTimeManual:
                        SelectedViewModel = new RealTimeManual();
                        break;
                    default:
                        throw new InvalidOperationException($"Unexpected value _mode = {_mode}");
                }
                OnPropertyChanged();
                OnPropertyChanged("SelectedViewModel");
            }
        }

        public IMode SelectedViewModel { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public InformationField()
        {
            Modes = new List<Mode>() { Mode.Level, Mode.Sin, Mode.Triangle, Mode.RealTimeManual };
            SelectedMode = Mode.Level;
        }

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
