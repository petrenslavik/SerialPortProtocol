using Microsoft.Win32;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Filmobus_test
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private Dictionary<string, string> _settings;

        public SettingsWindow(SettingsViewModel model)
        {
            InitializeComponent();
            DataContext = model;
        }

        private void SaveSettings_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                FileName = "settingsData",
                Title = "Save settings data",
                Filter = "Settings data|*.sd"
            };
            var isChoosen = dialog.ShowDialog();
            if (isChoosen != null && isChoosen.Value)
            {
                using (var stream = dialog.OpenFile())
                {
                    var formatter = new BinaryFormatter();
                    formatter.Serialize(stream, _settings);
                }
            }
        }

        private void LoadSettings_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Load settings data",
                Filter = "Settings data|*.sd"
            };
            var isChoosen = dialog.ShowDialog();
            if (isChoosen != null && isChoosen.Value)
            {
                using (var stream = dialog.OpenFile())
                {
                    var formatter = new BinaryFormatter();
                    _settings = (Dictionary<string, string>)formatter.Deserialize(stream);
                }
            }
        }
    }
}
