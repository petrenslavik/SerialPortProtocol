using System;

namespace Filmobus_test.Models
{
    public class Triangle : IMode
    {
        public int Minimum { get; set; }
        public int Maximum { get; set; }
        public int Duration { get; set; }
        public bool IsDynamic => true;
        public int GetValue(double time)
        {
            var value = (1f - 4f * Math.Abs(Math.Round(time/Duration/2 - 0.25f) - (time/Duration/2 - 0.25f)) + 1)/2f;
            return (int)((Maximum - Minimum) * value + Minimum);
        }
    }
}
