using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace Filmobus_test
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private Dictionary<string, string> _settings;
        private List<TextBox> _settingsGroupTextBoxes;
        private List<TextBox> _settingsTextBoxes;

        public SettingsWindow()
        {
            InitializeComponent();
            _settingsGroupTextBoxes = new List<TextBox>();
            _settingsTextBoxes = new List<TextBox>();
        }

        public void SetData(Dictionary<string, string> settings)
        {
            _settings = settings;
            int count = 0;
            foreach (var pair in settings)
            {
                var groupTextBox = new TextBox
                {
                    Text = pair.Key,
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness(10, count * 23 + 10, 0, 0),
                    Width = 100,
                    Height = 23
                };

                var dataTextBox = new TextBox
                {
                    Text = pair.Value,
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness(100 + 10 + 10, count * 23 + 10, 0, 0),
                    Width = 280,
                    Height = 23
                };

                groupTextBox.SetValue(Grid.RowProperty, 1);
                dataTextBox.SetValue(Grid.RowProperty, 1);

                _settingsGroupTextBoxes.Add(groupTextBox);
                _settingsTextBoxes.Add(dataTextBox);
                DataGrid.Children.Add(groupTextBox);
                DataGrid.Children.Add(dataTextBox);
                count++;
            }
        }

        private void SaveSettings_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog();
            dialog.FileName = "settingsData";
            dialog.Title = "Save settings data";
            dialog.Filter = "Settings data|*.sd";
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
            var dialog = new OpenFileDialog();
            dialog.Title = "Load settings data";
            dialog.Filter = "Settings data|*.sd";
            var isChoosen = dialog.ShowDialog();
            if (isChoosen != null && isChoosen.Value)
            {
                using (var stream = dialog.OpenFile())
                {
                    var formatter = new BinaryFormatter();
                    _settings = (Dictionary<string, string>) formatter.Deserialize(stream);
                }
            }
        }
    }
}
