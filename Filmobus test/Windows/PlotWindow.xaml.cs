using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using Microsoft.Win32;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace Filmobus_test
{
    public enum DataFor { Desk, Rtu }
    /// <summary>
    /// Interaction logic for PlotWindow.xaml
    /// </summary>
    public partial class PlotWindow : Window
    {
        private PlotModel _dataPlot;
        private Timer _timer;
        private List<LineSeries> _deskSeries;
        private List<LineSeries> _rtuSeries;

        public PlotWindow()
        {
            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            _deskSeries = new List<LineSeries>();
            _rtuSeries = new List<LineSeries>();

            _dataPlot = new PlotModel
            {
                LegendPlacement = LegendPlacement.Outside,
                LegendOrientation = LegendOrientation.Vertical
            };
            DataPlotView.Model = _dataPlot;
            for (int i = 1; i <= 16; i++)
            {
                var series = new LineSeries
                {
                    Title = $"Desk {i}",
                    IsVisible = false
                };
                series.Points.Add(new DataPoint(0, 0));
                _dataPlot.Series.Add(series);
                _deskSeries.Add(series);
            }

            for (int i = 1; i <= 8; i++)
            {
                var series = new LineSeries
                {
                    Title = $"Rtu {i}",
                    IsVisible = false
                };
                series.Points.Add(new DataPoint(0, 0));
                _dataPlot.Series.Add(series);
                _rtuSeries.Add(series);
            }

            var horizontalAxis = new LinearAxis();
            horizontalAxis.MajorGridlineStyle = LineStyle.Solid;
            horizontalAxis.MinorGridlineStyle = LineStyle.Solid;
            horizontalAxis.MajorGridlineColor = OxyColors.Black;
            horizontalAxis.Position = AxisPosition.Bottom;

            var verticalAxis = new LinearAxis();
            verticalAxis.MajorGridlineStyle = LineStyle.Solid;
            verticalAxis.MinorGridlineStyle = LineStyle.Solid;
            verticalAxis.MajorGridlineColor = OxyColors.Black;
            verticalAxis.Position = AxisPosition.Left;

            _dataPlot.Axes.Add(horizontalAxis);
            _dataPlot.Axes.Add(verticalAxis);

            var timerCallback = new TimerCallback(RefreshGraphics);
            _timer = new Timer(timerCallback, null, 0, 1000);
        }

        private void RefreshGraphics(object obj)
        {
            Dispatcher.Invoke(() => { _dataPlot.InvalidatePlot(true); });
        }

        public void AddPoint(DataFor type, int number, int value)
        {
            List<DataPoint> points;
            switch (type)
            {
                case DataFor.Desk:
                    {
                        points = _deskSeries[number].Points;
                        break;
                    }
                case DataFor.Rtu:
                    {
                        points = _rtuSeries[number].Points;
                        break;
                    }
                default:
                    {
                        points = null;
                        break;
                    }
            }

            double lastX = 0;
            if(points.Count!=0)
                lastX = points[points.Count - 1].X;
            points.Add(new DataPoint(lastX + 1, value));
        }

        public void SetVisibility(bool visibility, DataFor type, int number)
        {
            switch (type)
            {
                case DataFor.Desk:
                    {
                        _deskSeries[number].IsVisible = visibility;
                        _deskSeries[number].Points.Clear();
                        break;
                    }
                case DataFor.Rtu:
                    {
                        _rtuSeries[number].IsVisible = visibility;
                        _rtuSeries[number].Points.Clear();
                        break;
                    }
            }
            _dataPlot.InvalidatePlot(true);
        }

        private void SavePlot_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog();
            dialog.FileName = "plot";
            dialog.Title = "Save plot data";
            dialog.Filter = "Plot data|*.pd";
            var isChoosen = dialog.ShowDialog();
            if (isChoosen != null && isChoosen.Value)
            {
                using (var stream = dialog.OpenFile())
                {
                    var formatter = new BinaryFormatter();
                    var data = new SerializableData();
                    foreach (var series in _deskSeries)
                    {
                        data.AddSeries(series, DataFor.Desk);
                    }
                    foreach (var series in _rtuSeries)
                    {
                        data.AddSeries(series, DataFor.Rtu);
                    }
                    formatter.Serialize(stream, data);
                }
            }
        }
        private void LoadPlot_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.FileName = "plot";
            dialog.Title = "Save plot data";
            dialog.Filter = "Plot data|*.pd";
            var isChoosen = dialog.ShowDialog();
            if (isChoosen != null && isChoosen.Value)
            {
                using (var stream = dialog.OpenFile())
                {
                    var formatter = new BinaryFormatter();
                    var data = (SerializableData)formatter.Deserialize(stream);
                    for (int i = 0; i < data.DeskSeries.Count; i++)
                    {
                        _deskSeries[i].Points.Clear();
                        for (int j = 0; j < data.DeskSeries[i].XCoordinates.Count; j++)
                        {
                            _deskSeries[i].Points.Add(new DataPoint(
                                data.DeskSeries[i].XCoordinates[j],
                                data.DeskSeries[i].YCoordinates[j]));
                        }
                    }
                    for (int i = 0; i < data.RtuSeries.Count; i++)
                    {
                        _rtuSeries[i].Points.Clear();
                        for (int j = 0; j < data.RtuSeries[i].XCoordinates.Count; j++)
                        {
                            _rtuSeries[i].Points.Add(new DataPoint(
                                data.RtuSeries[i].XCoordinates[j],
                                data.RtuSeries[i].YCoordinates[j]));
                        }
                    }
                }
            }
        }
        private void ClearPlot_Click(object sender, RoutedEventArgs e)
        {
            foreach (var series in _deskSeries)
            {
                series.Points.Clear();
            }
            foreach (var series in _rtuSeries)
            {
                series.Points.Clear();
            }
        }
    }
}
