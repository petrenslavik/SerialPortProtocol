using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Runtime.CompilerServices;
using Filmobus_test.Annotations;

namespace Filmobus_test.ViewModels
{
    public class ApplicationViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public PortViewModel Port { get; set; }
        public SenderViewModel Sender { get; set; }

        public ApplicationViewModel(List<ObservableCollection<int>> deskSettingsValues, List<ObservableCollection<int>> rtuSettingsValues)
        {
            _port = new SerialPort();
            Port = new PortViewModel(_port);
            Sender = new SenderViewModel();
            _deskSettingsValues = deskSettingsValues;
            _rtuSettingsValues = rtuSettingsValues;
            _port.DataReceived += ReadTempData;
            _cache = new List<byte>();
            DeskDataCheckBoxes = new ObservableCollection<bool>(new bool[16]);
            RtuDataCheckBoxes = new ObservableCollection<bool>(new bool[8]);
            DeskDataCheckBoxes.CollectionChanged += DeskDataCheckBoxes_CollectionChanged;
            RtuDataCheckBoxes.CollectionChanged += RtuDataCheckBoxes_CollectionChanged;
        }

        private void RtuDataCheckBoxes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_plotWindow == null)
            {
                return;
            }

            if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                _plotWindow.SetVisibility(DeskDataCheckBoxes[e.OldStartingIndex], DataFor.Rtu, e.OldStartingIndex);
            }
        }

        private void DeskDataCheckBoxes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_plotWindow == null)
            {
                return;
            }

            if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                _plotWindow.SetVisibility(DeskDataCheckBoxes[e.OldStartingIndex], DataFor.Desk, e.OldStartingIndex);
            }
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

        public ObservableCollection<bool> DeskDataCheckBoxes { get; set; }
        public ObservableCollection<bool> RtuDataCheckBoxes { get; set; }

        private RelayCommand _openPlotCommand;
        public RelayCommand OpenPlotCommand => _openPlotCommand ?? (_openPlotCommand = new RelayCommand(OpenPlot));
       
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
                        if (_cache.Count>0 && _cache[0] == 0)
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

                        for (int i = 0; i < 6; i++)
                        {
                            _deskSettingsValues[packet.SettingsGroup][i] = packet.Settings[i];
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

                        for (int i = 0; i < 6; i++)
                        {
                            _rtuSettingsValues[packet.SettingsGroup][i] = packet.Settings[i];
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
                _plotWindow.SetVisibility(DeskDataCheckBoxes[i], DataFor.Desk, i);
            }

            for (int i = 0; i < 8; i++)
            {
                _plotWindow.SetVisibility(RtuDataCheckBoxes[i], DataFor.Rtu, i);
            }
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
