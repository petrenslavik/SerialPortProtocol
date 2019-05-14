
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using VerticalAlignment = System.Windows.VerticalAlignment;

namespace Filmobus_test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<TextBox> DeskDataBoxes;
        public List<TextBox> RtuDataBoxes;
        public List<CheckBox> DeskDataCheckBoxes;
        public List<CheckBox> RtuDataCheckBoxes;

        private ApplicationViewModel _model;
        private Dictionary<string, string> DeskSettings;
        private Dictionary<string, string> RtuSettings;
        private PlotWindow _plotWindow;
        private SerialPort _port;
        private List<byte> _cache;
        private bool _isCleared;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DeskSettings = new Dictionary<string, string>();
            RtuSettings = new Dictionary<string, string>();
            DeskDataBoxes = new List<TextBox>();
            RtuDataBoxes = new List<TextBox>();
            DeskDataCheckBoxes = new List<CheckBox>();
            RtuDataCheckBoxes = new List<CheckBox>();
            for (int i = 0; i < 16; i++)
            {
                var checkbox = new CheckBox()
                {
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness(i * 36 + 12.5, 6, 0, 0)
                };

                checkbox.Checked += CheckBoxChanged;
                checkbox.Unchecked += CheckBoxChanged;
                checkbox.Tag = $"Desk {i}";

                var textbox = new TextBox
                {
                    VerticalAlignment = VerticalAlignment.Bottom,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Width = 40,
                    Margin = new Thickness(i * 36, 0, 0, 0),
                };

                textbox.Tag = $"Desk {i}";

                var binding = new Binding();
                binding.Path = new PropertyPath($"CurrentDeskPacket.DeskArray[{i}]");
                textbox.SetBinding(TextBox.TextProperty, binding);

                checkbox.SetValue(Grid.ColumnSpanProperty, 4);
                checkbox.SetValue(Grid.ColumnProperty, 1);
                checkbox.SetValue(Grid.RowProperty, 3);

                textbox.SetValue(Grid.ColumnSpanProperty, 4);
                textbox.SetValue(Grid.ColumnProperty, 1);
                textbox.SetValue(Grid.RowProperty, 3);
                textbox.SetValue(Grid.ZIndexProperty, 16 - i);

                DeskDataCheckBoxes.Add(checkbox);
                DeskDataBoxes.Add(textbox);
                DeskDataGrid.Children.Add(checkbox);
                DeskDataGrid.Children.Add(textbox);
            }

            for (int i = 0; i < 8; i++)
            {
                var checkbox = new CheckBox()
                {
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness(i * 38 + 8.5, 6, 0, 0)
                };

                var textbox = new TextBox
                {
                    VerticalAlignment = VerticalAlignment.Bottom,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Width = 40,
                    Margin = new Thickness(i * 37, 0, 0, 0)
                };
                checkbox.Checked += CheckBoxChanged;
                checkbox.Unchecked += CheckBoxChanged;
                checkbox.Tag = $"Rtu {i}";

                textbox.Tag = $"Rtu {i}";

                var binding = new Binding();
                binding.Path = new PropertyPath($"CurrentRtuPacket.RtuArray[{i}]");
                textbox.SetBinding(TextBox.TextProperty, binding);


                checkbox.SetValue(Grid.ColumnSpanProperty, 4);
                checkbox.SetValue(Grid.ColumnProperty, 1);
                checkbox.SetValue(Grid.RowProperty, 3);

                textbox.SetValue(Grid.ColumnSpanProperty, 4);
                textbox.SetValue(Grid.ColumnProperty, 1);
                textbox.SetValue(Grid.RowProperty, 3);

                RtuDataCheckBoxes.Add(checkbox);
                RtuDataBoxes.Add(textbox);
                RtuDataGrid.Children.Add(checkbox);
                RtuDataGrid.Children.Add(textbox);
            }

            PortChooserBox.ItemsSource = SerialPort.GetPortNames().ToList();
            BaudRateChooserBox.ItemsSource = new List<int>()
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
            ParityChooserBox.ItemsSource =
                new List<Parity>() { Parity.None, Parity.Even, Parity.Mark, Parity.Odd, Parity.Space };
            StopBitsChooserBox.ItemsSource =
                new List<StopBits>() { StopBits.One, StopBits.OnePointFive, StopBits.Two };
            DataBitsChooserBox.ItemsSource = new List<int> { 5, 6, 7, 8 };
            _cache = new List<byte>();
            _port = new SerialPort();
            LoadSettings();
            _model = new ApplicationViewModel();
            this.DataContext = _model;
        }

        private void OpenPortButton_Click(object sender, RoutedEventArgs e)
        {
            if (_port.IsOpen)
            {
                return;
            }

            _port.Parity = (System.IO.Ports.Parity)ParityChooserBox.SelectedItem;
            _port.PortName = (string)PortChooserBox.SelectedItem;
            _port.BaudRate = (int)BaudRateChooserBox.SelectedItem;
            _port.DataBits = (int)DataBitsChooserBox.SelectedItem;
            _port.StopBits = System.IO.Ports.StopBits.One;
            _port.ReceivedBytesThreshold = 256;
            _port.DataReceived += ReadTempData;

            try
            {
                _port.Open();
            }
            catch(Exception ex)
            {
                (DataContext as ApplicationViewModel).SerialPortException = ex.Message;
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
                        _model.CurrentDeskPacket = packet;
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
                        _model.CurrentRtuPacket = packet;
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

        private void ClosePortButton_Click(object sender, RoutedEventArgs e)
        {
            ClosePort();
        }

        private void OpenPlotButton_Click(object sender, RoutedEventArgs e)
        {
            _plotWindow = new PlotWindow();
            _plotWindow.Show();
            for (int i = 0; i < 16; i++)
            {
                _plotWindow.SetVisibility(DeskDataCheckBoxes[i].IsChecked ?? false, DataFor.Desk, i);
            }

            for (int i = 0; i < 8; i++)
            {
                _plotWindow.SetVisibility(RtuDataCheckBoxes[i].IsChecked ?? false, DataFor.Rtu, i);
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            DirectionTextBox.Clear();
            AskTextBox.Clear();
            SettingsRequestTextBox.Clear();
            PacketTypeTextBox.Clear();
            FlagsArrayTextBox.Clear();
            SettingsNumTextBox.Clear();
            SettingsNum1TextBox.Clear();
            foreach (var textbox in DeskDataBoxes)
            {
                textbox.Clear();
            }
            foreach (var checkbox in DeskDataCheckBoxes)
            {
                checkbox.IsChecked = false;
            }
        }

        private void RtuClearButton_Click(object sender, RoutedEventArgs e)
        {
            RtuDirectionTextBox.Clear();
            RtuStatusTextBox.Clear();
            RtuFuncAddressTextBox.Clear();
            RtuFlagsArrayTextBox.Clear();
            RtuSettingsNumTextBox.Clear();
            RtuSettingsNum1TextBox.Clear();
            foreach (var textbox in RtuDataBoxes)
            {
                textbox.Clear();
            }
            foreach (var checkbox in RtuDataCheckBoxes)
            {
                checkbox.IsChecked = false;
            }
        }

        private void CheckBoxChanged(object sender, RoutedEventArgs e)
        {
            var checkbox = sender as CheckBox;
            var tag = checkbox.Tag;
            var words = tag.ToString().Split(' ');
            _plotWindow?.SetVisibility(checkbox.IsChecked ?? false, words[0] == "Desk" ? DataFor.Desk : DataFor.Rtu, int.Parse(words[1]));
        }

        private void LoadSettings()
        {
            PortChooserBox.SelectedItem = Properties.Settings.Default.PortName;
            BaudRateChooserBox.SelectedItem = Properties.Settings.Default.BaudRate;
            ParityChooserBox.SelectedItem = Properties.Settings.Default.Parity;
            StopBitsChooserBox.SelectedItem = Properties.Settings.Default.StopBits;
            DataBitsChooserBox.SelectedItem = Properties.Settings.Default.ByteSize;
        }

        private void SaveSettings()
        {
            Properties.Settings.Default.PortName = PortChooserBox.SelectedItem as string;
            Properties.Settings.Default.BaudRate = (int)BaudRateChooserBox.SelectedItem;
            Properties.Settings.Default.Parity = (System.IO.Ports.Parity)ParityChooserBox.SelectedItem;
            Properties.Settings.Default.StopBits = (System.IO.Ports.StopBits)StopBitsChooserBox.SelectedItem;
            Properties.Settings.Default.ByteSize = (int)DataBitsChooserBox.SelectedItem;
            Properties.Settings.Default.Save();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            ClosePort();
            SaveSettings();
        }

        private void ClosePort()
        {
            if (_port != null && _port.IsOpen)
            {
                _port.Close();
            }
        }

        private void ShowSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            (sender as Button).IsEnabled = false;
            var window = new SettingsWindow();
            window.SetData(DeskSettings);
            window.Closed += SettingsWindowOnClosed;
            window.Tag = "Desk";
            window.Show();
        }

        private void RtuShowSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            (sender as Button).IsEnabled = false;
            var window = new SettingsWindow();
            window.SetData(RtuSettings);
            window.Closed += SettingsWindowOnClosed;
            window.Tag = "Rtu";
            window.Show();
        }

        private void SettingsWindowOnClosed(object sender, EventArgs eventArgs)
        {
            if ((string)(sender as SettingsWindow).Tag == "Desk")
            {
                ShowSettingsButton.IsEnabled = true;
            }
            else
            {
                RtuShowSettingsButton.IsEnabled = true;
            }
        }
    }
}
