using System;
using System.Collections.Generic;
using System.Linq;

namespace Filmobus_test
{
    public class Packet
    {
        public byte[] _data;
        public byte Length { get; private set; }
        public byte Direction { get; private set; }
        public byte Ask { get; private set; }
        public byte SendSettings { get; private set; }
        public byte AskSettings { get; private set; }
        public string PacketType { get; private set; }
        public string FlagsArray { get; private set; }
        public string SettingsRequest => SendSettings + " | " + AskSettings;
        public int[] DeskArray { get; private set; }

        public int SettingsGroup { get; private set; }
        public int[] Settings { get; private set; }

        public string RtuStatus { get; private set; }
        public string RtuFuncAddress { get; private set; }
        public int[] RtuArray { get; private set; }

        public string Data { get; private set; }

        private Packet(byte[] arr)
        {
            _data = arr;
            Analyze();
        }

        public static Packet TryToCreate(List<byte> data)
        {
            byte length;
            if (data.Count > 4)
            {
                length = (byte)(data[3] & 127);
            }
            else
            {
                return null;
            }

            if (data.Count > length + 4)
            {
                byte[] arr = new byte[length + 4];
                data.CopyTo(0, arr, 0, length + 4);
                data.RemoveRange(0, length + 4);
                return new Packet(arr);
            }

            return null;
        }

        private void Analyze()
        {
            Direction = (byte)(_data[3] / 128);
            if (Direction == 0)
            {
                Ask = (byte)(_data[4] / 128);

                SendSettings = (byte)((_data[4] >> 6) & 1);
                AskSettings = (byte)((_data[4] >> 5) & 1);
                for (int i = 3; i >= 0; i--)
                {
                    var value = _data[4] & (int)Math.Pow(2, i);
                    PacketType += value != 0 ? $"1 | " : $"0 | ";
                }

                FlagsArray = string.Empty;
                int amountOfCache = 0;

                for (int i = 0; i < 8; i++)
                {
                    var t = _data[5] & (int)Math.Pow(2, i);
                    if (t != 0)
                    {
                        amountOfCache++;
                        FlagsArray += $" 1 |";
                    }
                    else
                    {
                        FlagsArray += $" 0 |";
                    }
                }

                for (int i = 0; i < 8; i++)
                {
                    var t = _data[6] & (int)Math.Pow(2, i);
                    if (t != 0)
                    {
                        amountOfCache++;
                        FlagsArray += $" 1 |";
                    }
                    else
                    {
                        FlagsArray += $" 0 |";
                    }
                }

                DeskArray = new int[16];
                for (int i = 0; i < amountOfCache; i++)
                {
                    int value = _data[8 + i * 2];
                    value = value << 8;
                    value += _data[7 + i * 2];
                    DeskArray[i] = value;
                }

                if (SendSettings != 0)
                {
                    SettingsGroup = _data[7 + amountOfCache * 2];
                    Settings = new int[6];
                    for (int i = 0; i < 6; i++)
                    {
                        Settings[i] = _data[8 + amountOfCache * 2 + i];
                    }
                }
            }
            else
            {
                int settingsGroup = _data[4] & 128;
                RtuStatus = string.Empty;
                for (int i = 6; i >= 4; i--)
                {
                    var value = _data[4] & (int)Math.Pow(2, i);
                    RtuStatus += value != 0 ? $"1 | " : $"0 | ";
                }

                RtuFuncAddress = string.Empty;
                for (int i = 3; i >= 0; i--)
                {
                    var value = _data[4] & (int)Math.Pow(2, i);
                    RtuFuncAddress += value != 0 ? $"1 | " : $"0 | ";
                }

                FlagsArray = string.Empty;
                int amountOf_cache = 0;

                for (int i = 0; i < 8; i++)
                {
                    var t = _data[5] & (int)Math.Pow(2, i);
                    if (t != 0)
                    {
                        amountOf_cache++;
                        FlagsArray += $" 1 |";
                    }
                    else
                    {
                        FlagsArray += $" 0 |";
                    }
                }

                RtuArray = new int[8];
                for (int i = 0; i < amountOf_cache; i++)
                {
                    int value = _data[7 + i * 2];
                    value = value << 8;
                    value += _data[6 + i * 2];
                    RtuArray[i] = value;
                }

                if (settingsGroup != 0)
                {
                    SettingsGroup = _data[6 + amountOf_cache * 2];
                    Settings = new int[6];
                    for (int i = 0; i < 6; i++)
                    {
                        Settings[i] = _data[7 + amountOf_cache * 2 + i];
                    }
                }
            }
            Data = string.Join(" ", _data.ToArray());
        }
    }
}
