using Filmobus_test.HelpfulClasses;
using Filmobus_test.Models;
using Filmobus_test.Properties;
using System;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Filmobus_test.ViewModels
{
    public class SenderViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public SenderViewModel(SerialPort port)
        {
            _port = port;
            Fields = new InformationField[16];
            for (int i = 0; i < 16; i++)
            {
                Fields[i] = new InformationField();
            }

            Flags = new int[16];
            SettingsGroup = new int[6];
        }

        public int Timeout { get; set; }
        public bool Ask { get; set; }
        public bool SendSettings { get; set; }
        public bool AskSettings { get; set; }
        public string TypeModule { get; set; }
        public int[] Flags { get; set; }
        public InformationField[] Fields { get; set; }
        public int SettingsGroupNum { get; set; }
        public int[] SettingsGroup { get; set; }

        public string LastPacket
        {
            get => _lastPacket;
            set { _lastPacket = value; OnPropertyChanged(); }
        }

        private Timer _timer;
        private SerialPort _port;
        private byte[] _data;
        private ulong _counter;

        private RelayCommand _startSendingCommand;
        private string _lastPacket;
        public RelayCommand StartSendingCommand => _startSendingCommand ?? (_startSendingCommand = new RelayCommand(StartSending));

        private void StartSending(object obj)
        {
            _timer?.Dispose();
            _counter = 0;
            PrepareData();
            var callback = new TimerCallback(Send);
            _timer = new Timer(callback, null, 0, Timeout);
        }

        private void Send(object state)
        {
            if (_port == null || !_port.IsOpen)
            {
                _timer.Dispose();
                return;
            }

            var arr = GenerateData();
            LastPacket = string.Join(" ", arr.ToArray());
            _port.Write(arr, 0, arr.Length);
            _counter++;
        }

        private byte[] GenerateData()
        {
            PrepareData();
            return _data;
        }

        private void PrepareData()
        {
            double time = (_counter * (ulong)Timeout) / 1000f;
            int length = 3;//Identification, Flags
            for (int i = 0; i < Flags.Length; i++)
            {
                if (Flags[i] == 1)
                {
                    length += 2;
                }
            }

            if (SendSettings)
            {
                length += 7;
            }

            if (Ask)
            {
                length += 1;
            }

            length += 6;//Start Flag, Length, CRC
            _data = new byte[length];
            _data[0] = _data[1] = _data[2] = 255;
            _data[3] = (byte)(length - 4);
            // _data[3] = (byte)(length | 128);//RTU
            byte id = (byte)Convert.ToInt32(TypeModule, 2);
            if (Ask)
            {
                id = (byte)(id | 128);
            }
            if (SendSettings)
            {
                id = (byte)(id | 64);
            }
            if (AskSettings)
            {
                id = (byte)(id | 32);
            }

            _data[4] = id;
            int counter = 0;
            for (int i = 0; i < 8; i++)
            {
                if (Flags[i] == 1)
                {
                    counter++;
                    _data[5] = (byte)(_data[5] | (byte)Math.Pow(2, i));
                }
            }
            for (int i = 0; i < 8; i++)
            {
                if (Flags[8 + i] == 1)
                {
                    counter++;
                    _data[6] = (byte)(_data[6] | (byte)Math.Pow(2, i));
                }
            }

            for (int i = 0; i < counter; i++)
            {
                var value = Fields[i].SelectedViewModel.GetValue(time);
                _data[8 + i * 2] = (byte)(value >> 8);
                _data[7 + i * 2] = (byte)value;
            }

            if (SendSettings)
            {
                _data[7 + counter * 2] = (byte)SettingsGroupNum;
                for (int i = 0; i < SettingsGroup.Length; i++)
                {
                    _data[8 + counter * 2 + i] = (byte)SettingsGroup[i];
                }
            }

            var crc = CRC.CRC16(_data, _data.Length - 2,3);
            _data[_data.Length - 2] = (byte)(crc >> 8);
            _data[_data.Length - 1] = (byte)crc;
        }


        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
