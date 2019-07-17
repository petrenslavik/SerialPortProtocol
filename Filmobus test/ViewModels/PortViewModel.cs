using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Filmobus_test.Annotations;

namespace Filmobus_test.ViewModels
{
    public class PortViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _exception;
        private SerialPort _port;

        public Parity Parity { get; set; }
        public string PortName { get; set; }
        public int BaudRate { get; set; }
        public int DataBits { get; set; }
        public StopBits StopBits { get; set; }

        public string SerialPortException
        {
            get => _exception;
            set
            {
                _exception = value;
                OnPropertyChanged();
            }
        }

        public List<string> AvailablePorts { get; set; }
        public List<int> AvailableBaudRates { get; set; }
        public List<Parity> AvailableParity { get; set; }
        public List<StopBits> AvailableStopBits { get; set; }
        public List<int> AvailableDataBits { get; set; }

        public PortViewModel(SerialPort port)
        {
            _port = port;
            AvailablePorts = SerialPort.GetPortNames().ToList();
            AvailableBaudRates = new List<int>()
            {
                110,
                300,
                600,
                2100,
                2400,
                4800,
                9600,
                14400,
                19200,
                38400,
                56000,
                57600,
                115200,
                230400,
                460800,
                921600
            };
            AvailableParity = new List<Parity>() { Parity.None, Parity.Even, Parity.Mark, Parity.Odd, Parity.Space };
            AvailableStopBits = new List<StopBits>() { StopBits.One, StopBits.OnePointFive, StopBits.Two };
            AvailableDataBits = new List<int> { 5, 6, 7, 8 };
            LoadSettings();
        }

        private void ClosePort(object obj)
        {
            if (_port != null && _port.IsOpen)
            {
                _port.Close();
                SerialPortException = string.Empty;
            }
        }

        private void OpenPort(object obj)
        {
            if (_port.IsOpen)
            {
                SerialPortException = $"Port {_port.PortName} already opened";
                return;
            }

            _port.Parity = Parity;
            _port.PortName = PortName;
            _port.BaudRate = BaudRate;
            _port.DataBits = DataBits;
            _port.StopBits = StopBits;
            _port.ReceivedBytesThreshold = 64;
            SerialPortException = string.Empty;

            try
            {
                _port.Open();
            }
            catch (Exception ex)
            {
                SerialPortException = ex.Message;
            }
        }

        private RelayCommand _closePortCommand;
        public RelayCommand ClosePortCommand => _closePortCommand ?? (_closePortCommand = new RelayCommand(ClosePort));

        private RelayCommand _openPortCommand;
        public RelayCommand OpenPortCommand => _openPortCommand ?? (_openPortCommand = new RelayCommand(OpenPort));

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void LoadSettings()
        {
            PortName = Properties.Settings.Default.PortName;
            BaudRate = Properties.Settings.Default.BaudRate;
            Parity = Properties.Settings.Default.Parity;
            StopBits = Properties.Settings.Default.StopBits;
            DataBits = Properties.Settings.Default.ByteSize;
        }

        public void SaveSettings()
        {
            Properties.Settings.Default.PortName = PortName;
            Properties.Settings.Default.BaudRate = BaudRate;
            Properties.Settings.Default.Parity = Parity;
            Properties.Settings.Default.StopBits = StopBits;
            Properties.Settings.Default.ByteSize = DataBits;
            Properties.Settings.Default.Save();
        }
    }
}
