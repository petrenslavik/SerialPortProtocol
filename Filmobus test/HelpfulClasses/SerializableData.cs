using Filmobus_test.Windows;
using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;

namespace Filmobus_test.HelpfulClasses
{
    [Serializable]
    public class SerializableData
    {
        public LegendPlacement LegendPlacement { get; set; }
        public LegendOrientation LegendOrientation { get; set; }
        public List<SerializableSeries> DeskSeries { get; set; }
        public List<SerializableSeries> RtuSeries { get; set; }

        public SerializableData()
        {
            DeskSeries = new List<SerializableSeries>();
            RtuSeries = new List<SerializableSeries>();
        }

        public void AddSeries(LineSeries series, DataFor type)
        {
            var serializableSeries = new SerializableSeries();
            serializableSeries.IsVisible = series.IsVisible;
            serializableSeries.Name = series.Title;
            foreach (var point in series.Points)
            {
                serializableSeries.XCoordinates.Add(point.X);
                serializableSeries.YCoordinates.Add(point.Y);
            }

            if (type == DataFor.Desk)
            {
                DeskSeries.Add(serializableSeries);
            }
            else
            {
                RtuSeries.Add(serializableSeries);
            }
        }
    }

    [Serializable]
    public class SerializableSeries
    {
        public string Name { get; set; }
        public bool IsVisible { get; set; }
        public List<double> XCoordinates { get; private set; }
        public List<double> YCoordinates { get; private set; }

        public SerializableSeries()
        {
            XCoordinates = new List<double>();
            YCoordinates = new List<double>();
        }
    }
}
