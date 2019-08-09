using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Filmobus_test.Converters;
using Filmobus_test.ViewModels;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using VerticalAlignment = System.Windows.VerticalAlignment;

namespace Filmobus_test.Windows
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //DeskDataBoxes = new List<TextBox>();
            //RtuDataBoxes = new List<TextBox>();
            //DeskDataCheckBoxes = new List<CheckBox>();
            //RtuDataCheckBoxes = new List<CheckBox>();

            //var converter = new NumberFormatConverter();

            //for (int i = 0; i < 16; i++)
            //{
            //    var checkbox = new CheckBox()
            //    {
            //        VerticalAlignment = VerticalAlignment.Top,
            //        HorizontalAlignment = HorizontalAlignment.Left,
            //        Margin = new Thickness(i * 36 + 12.5, 6, 0, 0)
            //    };

            //    checkbox.Tag = $"Desk {i}";

            //    var bindingCheckBox = new Binding { Path = new PropertyPath($"DeskDataCheckBoxes[{i}]") };
            //    checkbox.SetBinding(CheckBox.IsCheckedProperty, bindingCheckBox);

            //    var textbox = new TextBox
            //    {
            //        VerticalAlignment = VerticalAlignment.Bottom,
            //        HorizontalAlignment = HorizontalAlignment.Left,
            //        Width = 40,
            //        Margin = new Thickness(i * 36, 0, 0, 0),
            //    };

            //    textbox.Tag = $"Desk {i}";

            //    var binding = new MultiBinding { Converter = converter };
            //    var bindingViewModel = new Binding { Path = new PropertyPath($"CurrentDeskPacket.DeskArray[{i}]") };
            //    var bindingIsHexadecimal = new Binding { Path = new PropertyPath($"IsChecked"), ElementName = "IsHexadecimalCheckBox" };
            //    binding.Bindings.Add(bindingViewModel);
            //    binding.Bindings.Add(bindingIsHexadecimal);
            //    textbox.SetBinding(TextBox.TextProperty, binding);

        

            //    DeskDataCheckBoxes.Add(checkbox);
            //    DeskDataBoxes.Add(textbox);
            //    DeskDataGrid.Children.Add(checkbox);
            //    DeskDataGrid.Children.Add(textbox);
            //}

            //for (int i = 0; i < 8; i++)
            //{
            //    var checkbox = new CheckBox()
            //    {
            //        VerticalAlignment = VerticalAlignment.Top,
            //        HorizontalAlignment = HorizontalAlignment.Left,
            //        Margin = new Thickness(i * 38 + 8.5, 6, 0, 0)
            //    };

            //    var textbox = new TextBox
            //    {
            //        VerticalAlignment = VerticalAlignment.Bottom,
            //        HorizontalAlignment = HorizontalAlignment.Left,
            //        Width = 40,
            //        Margin = new Thickness(i * 37, 0, 0, 0)
            //    };
            //    checkbox.Tag = $"Rtu {i}";

            //    var bindingCheckBox = new Binding { Path = new PropertyPath($"RtuDataCheckBoxes[{i}]") };
            //    checkbox.SetBinding(CheckBox.IsCheckedProperty, bindingCheckBox);


            //    textbox.Tag = $"Rtu {i}";

            //    var binding = new MultiBinding {Converter = converter};
            //    var bindingViewModel = new Binding { Path = new PropertyPath($"CurrentRtuPacket.RtuArray[{i}]") };
            //    var bindingIsHexadecimal = new Binding { Path = new PropertyPath($"IsChecked"),ElementName = "IsHexadecimalCheckBox" };
            //    binding.Bindings.Add(bindingViewModel);
            //    binding.Bindings.Add(bindingIsHexadecimal);
            //    textbox.SetBinding(TextBox.TextProperty, binding);

            //    checkbox.SetValue(Grid.ColumnSpanProperty, 4);
            //    checkbox.SetValue(Grid.ColumnProperty, 1);
            //    checkbox.SetValue(Grid.RowProperty, 3);

            //    textbox.SetValue(Grid.ColumnSpanProperty, 4);
            //    textbox.SetValue(Grid.ColumnProperty, 1);
            //    textbox.SetValue(Grid.RowProperty, 3);

            //    RtuDataCheckBoxes.Add(checkbox);
            //    RtuDataBoxes.Add(textbox);
            //    RtuDataGrid.Children.Add(checkbox);
            //    RtuDataGrid.Children.Add(textbox);
            //}

            var model = new ApplicationViewModel();
            DataContext = model;
            Closing += model.Closing;
        }
    }
}
