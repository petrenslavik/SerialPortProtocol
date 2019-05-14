using Filmobus_test.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Filmobus_test
{
    public class ApplicationViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ApplicationViewModel()
        {
            _port = new SerialPort();
            _port.DataReceived += ReadTempData;
            _cache = new List<byte>();
            DeskSettings = new Dictionary<string, string>();
            RtuSettings = new Dictionary<string, string>();
            LoadSettings();
        }

        private Packet _deskPacket;
        private Packet _rtuPacket;
        private string _exception;

        private SerialPort _port;
        private List<byte> _cache;
        private bool _isCleared;
        private Dictionary<string, string> DeskSettings;
        private Dictionary<string, string> RtuSettings;
        private PlotWindow _plotWindow;

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

        public Parity Parity { get; set; }
        public string PortName { get; set; }
        public int BaudRate { get; set; }
        public int DataBits { get; set; }
        public StopBits StopBits { get; set; }

        private RelayCommand _closePortCommand;
        public RelayCommand ClosePortCommand => _closePortCommand ?? (_closePortCommand = new RelayCommand(ClosePort));

        private RelayCommand _openPortCommand;
        public RelayCommand OpenPortCommand => _openPortCommand ?? (_openPortCommand = new RelayCommand(OpenPort));

        private void ClosePort(object obj)
        {
            if (_port != null && _port.IsOpen)
            {
                _port.Close();
            }
        }

        private void OpenPort(object obj)
        {
            if (_port.IsOpen)
            {
                SerialPortException = $"Port {_port.PortName} already opened";
            }

            _port.Parity = Parity;
            _port.PortName = PortName;
            _port.BaudRate = BaudRate;
            _port.DataBits = DataBits;
            _port.StopBits = StopBits;
            _port.ReceivedBytesThreshold = 256;

            try
            {
                _port.Open();
            }
            catch (Exception ex)
            {
                SerialPortException = ex.Message;
            }
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

        public void CheckPackets()
        {
            if (_isCleared)
            {
                while (true)
                {
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

                        if (DeskSettings.ContainsKey(packet.Settings.ToString()))
                        {
                            DeskSettings[packet.Settings.ToString()] = packet.Settings2;
                        }
                        else
                        {
                            DeskSettings.Add(packet.Settings.ToString(), packet.Settings2);
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

                        if (RtuSettings.ContainsKey(packet.Settings.ToString()))
                        {
                            RtuSettings[packet.Settings.ToString()] = packet.Settings2;
                        }
                        else
                        {
                            RtuSettings.Add(packet.Settings.ToString(), packet.Settings2);
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

        private void LoadSettings()
        {
            PortName = Properties.Settings.Default.PortName;
            BaudRate = Properties.Settings.Default.BaudRate;
            Parity = Properties.Settings.Default.Parity;
            StopBits = Properties.Settings.Default.StopBits;
            DataBits = Properties.Settings.Default.ByteSize;
        }

        private void OpenPlot(object sender)
        {
            //_plotWindow = new PlotWindow();
            //_plotWindow.Show();
            //for (int i = 0; i < 16; i++)
            //{
            //    _plotWindow.SetVisibility(DeskDataCheckBoxes[i].IsChecked ?? false, DataFor.Desk, i);
            //}

            //for (int i = 0; i < 8; i++)
            //{
            //    _plotWindow.SetVisibility(RtuDataCheckBoxes[i].IsChecked ?? false, DataFor.Rtu, i);
            //}
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
