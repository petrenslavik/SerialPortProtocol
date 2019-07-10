using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Filmobus_test.Converters;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using VerticalAlignment = System.Windows.VerticalAlignment;

namespace Filmobus_test
{
    public partial class MainWindow : Window
    {
        public List<TextBox> DeskDataBoxes;
        public List<TextBox> RtuDataBoxes;
        public List<CheckBox> DeskDataCheckBoxes;
        public List<CheckBox> RtuDataCheckBoxes;

        private List<ObservableCollection<int>> _deskSettings;
        private List<ObservableCollection<int>> _rtuSettings;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DeskDataBoxes = new List<TextBox>();
            RtuDataBoxes = new List<TextBox>();
            DeskDataCheckBoxes = new List<CheckBox>();
            RtuDataCheckBoxes = new List<CheckBox>();
            _deskSettings = new List<ObservableCollection<int>>();
            _rtuSettings = new List<ObservableCollection<int>>();

            var converter = new NumberFormatConverter();

            for (int i = 0; i < 16; i++)
            {
                var checkbox = new CheckBox()
                {
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness(i * 36 + 12.5, 6, 0, 0)
                };

                checkbox.Tag = $"Desk {i}";

                var bindingCheckBox = new Binding { Path = new PropertyPath($"DeskDataCheckBoxes[{i}]") };
                checkbox.SetBinding(CheckBox.IsCheckedProperty, bindingCheckBox);

                var textbox = new TextBox
                {
                    VerticalAlignment = VerticalAlignment.Bottom,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Width = 40,
                    Margin = new Thickness(i * 36, 0, 0, 0),
                };

                textbox.Tag = $"Desk {i}";

                var binding = new MultiBinding { Converter = converter };
                var bindingViewModel = new Binding { Path = new PropertyPath($"CurrentDeskPacket.DeskArray[{i}]") };
                var bindingIsHexadecimal = new Binding { Path = new PropertyPath($"IsChecked"), ElementName = "IsHexadecimalCheckBox" };
                binding.Bindings.Add(bindingViewModel);
                binding.Bindings.Add(bindingIsHexadecimal);
                textbox.SetBinding(TextBox.TextProperty, binding);

                checkbox.SetValue(Grid.ColumnSpanProperty, 4);
                checkbox.SetValue(Grid.ColumnProperty, 1);
                checkbox.SetValue(Grid.RowProperty, 3);

                textbox.SetValue(Grid.ColumnSpanProperty, 4);
                textbox.SetValue(Grid.ColumnProperty, 1);
                textbox.SetValue(Grid.RowProperty, 3);
                textbox.SetValue(Grid.ZIndexProperty, 16 - i);

                _deskSettings.Add(new ObservableCollection<int>(new int[6]));

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
                checkbox.Tag = $"Rtu {i}";

                var bindingCheckBox = new Binding { Path = new PropertyPath($"RtuDataCheckBoxes[{i}]") };
                checkbox.SetBinding(CheckBox.IsCheckedProperty, bindingCheckBox);


                textbox.Tag = $"Rtu {i}";

                var binding = new MultiBinding {Converter = converter};
                var bindingViewModel = new Binding { Path = new PropertyPath($"CurrentRtuPacket.RtuArray[{i}]") };
                var bindingIsHexadecimal = new Binding { Path = new PropertyPath($"IsChecked"),ElementName = "IsHexadecimalCheckBox" };
                binding.Bindings.Add(bindingViewModel);
                binding.Bindings.Add(bindingIsHexadecimal);
                textbox.SetBinding(TextBox.TextProperty, binding);

                _rtuSettings.Add(new ObservableCollection<int>(new int[6]));

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
            DataContext = new ApplicationViewModel(_deskSettings,_rtuSettings);
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
            SaveSettings();
        }

        private void ShowSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new SettingsWindow(new SettingsViewModel(_deskSettings, _rtuSettings));
            window.Closed += SettingsWindowOnClosed;
            window.Show();
        }

        private void RtuShowSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new SettingsWindow(new SettingsViewModel(_deskSettings, _rtuSettings));
            window.Closed += SettingsWindowOnClosed;
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
