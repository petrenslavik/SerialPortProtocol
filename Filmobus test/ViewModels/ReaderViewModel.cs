using Filmobus_test.Annotations;
using Filmobus_test.HelpfulClasses;
using Filmobus_test.Models;
using Filmobus_test.Windows;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Filmobus_test.ViewModels
{
    public class ReaderViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ReaderViewModel(SerialPort port)
        {
            _port = port;
            _port.DataReceived += ReadTempData;
            _cache = new List<byte>();
            _deskSettingsValues = new List<ObservableCollection<int>>();
            _rtuSettingsValues = new List<ObservableCollection<int>>();
            DeskDataCheckBoxes = new List<UselessClass>(new UselessClass[16]);
            RtuDataCheckBoxes = new List<UselessClass>(new UselessClass[8]);
            for (int i = 0; i < 16; i++)
            {
                DeskDataCheckBoxes[i] = new UselessClass(i, true);
                DeskDataCheckBoxes[i].PropertyChanged += VisibilityChanged;
            }
            for (int i = 0; i < 8; i++)
            {
                RtuDataCheckBoxes[i] = new UselessClass(i, false);
                RtuDataCheckBoxes[i].PropertyChanged += VisibilityChanged;
            }
            Clear(null);
        }

        private void VisibilityChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_plotWindow == null)
                return;
            var obj = sender as UselessClass;
            if (obj == null)
            {
                return;
            }

            _plotWindow.SetVisibility(obj.IsVisible, obj.IsDesk ? DataFor.Desk : DataFor.Rtu, obj.Index);
        }

        private Packet _deskPacket;
        private Packet _rtuPacket;
        private SerialPort _port;

        private List<byte> _cache;
        private bool _isCleared;
        private PlotWindow _plotWindow;
        private List<ObservableCollection<int>> _deskSettingsValues;
        private List<ObservableCollection<int>> _rtuSettingsValues;

        public Packet LastPacket { get; private set; }

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

        public List<UselessClass> DeskDataCheckBoxes { get; set; }
        public List<UselessClass> RtuDataCheckBoxes { get; set; }

        private RelayCommand _openPlotCommand;
        public RelayCommand OpenPlotCommand => _openPlotCommand ?? (_openPlotCommand = new RelayCommand(OpenPlot));

        private RelayCommand _clearCommand;
        public RelayCommand ClearCommand => _clearCommand ?? (_clearCommand = new RelayCommand(Clear));

        private void Clear(object obj)
        {
            CurrentDeskPacket = Packet.Empty;
            CurrentRtuPacket = Packet.Empty;
        }

        private RelayCommand _openSettingsCommand;
        public RelayCommand OpenSettingsCommand => _openSettingsCommand ?? (_openSettingsCommand = new RelayCommand(OpenSettings));

        private void OpenSettings(object obj)
        {
            var window = new SettingsWindow(new SettingsViewModel(_deskSettingsValues, _rtuSettingsValues));
            window.Show();
        }

        private void ReadTempData(object sender, SerialDataReceivedEventArgs e)
        {
            var arr = new byte[_port.BytesToRead];
            var readed = _port.Read(arr, 0, arr.Length);
            CheckPackets(arr.ToList());
        }

        private void CheckPackets(List<byte> newData)
        {
            _cache.AddRange(newData);
            CheckPackets();
        }

        private void CheckPackets()
        {
            if (_isCleared)
            {
                while (true)
                {
                    while (true)
                    {
                        if (_cache.Count > 0 && _cache[0] == 0)
                        {
                            _cache.RemoveAt(0);
                        }
                        else
                        {
                            break;
                        }
                    }
                    var packet = Packet.TryToCreate(_cache);
                    if (packet == null)
                    {
                        break;
                    }

                    if (packet.Direction == 0)
                    {
                        CurrentDeskPacket = packet;
                        for (int index = 0; index < packet.DeskArray.Length; index++)
                        {
                            var val = packet.DeskArray[index];
                            _plotWindow?.AddPoint(DataFor.Desk, index, val);
                        }
                        if (packet.SendSettings != 0)
                        {
                            for (int i = 0; i < 6; i++)
                            {
                                _deskSettingsValues[packet.SettingsGroup][i] = packet.Settings[i];
                            }
                        }
                    }
                    else
                    {
                        CurrentRtuPacket = packet;
                        for (int index = 0; index < packet.RtuArray.Length; index++)
                        {
                            var val = packet.RtuArray[index];
                            _plotWindow?.AddPoint(DataFor.Rtu, index, val);
                        }
                        if (packet.SendSettings != 0)
                        {
                            for (int i = 0; i < 6; i++)
                            {
                                _rtuSettingsValues[packet.SettingsGroup][i] = packet.Settings[i];
                            }
                        }
                    }
                }
            }
            else
            {
                int counter = 0;
                while (true)
                {
                    if (counter == 3)
                    {
                        _isCleared = true;
                        CheckPackets();
                        break;
                    }

                    if (_cache.Count > 0)
                    {
                        if (_cache[counter] == 255)
                        {
                            counter++;
                        }
                        else
                        {
                            counter = 0;
                            _cache.RemoveRange(0, counter + 1);
                        }
                    }
                }
            }
        }

        private void OpenPlot(object sender)
        {
            _plotWindow = new PlotWindow();
            _plotWindow.Show();
            for (int i = 0; i < 16; i++)
            {
                _plotWindow.SetVisibility(DeskDataCheckBoxes[i].IsVisible, DataFor.Desk, i);
            }

            for (int i = 0; i < 8; i++)
            {
                _plotWindow.SetVisibility(RtuDataCheckBoxes[i].IsVisible, DataFor.Rtu, i);
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
