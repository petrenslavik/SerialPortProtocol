using System;

namespace Filmobus_test.Models
{
    public class Sin : IMode
    {
        public bool IsDynamic => true;

        public int Frequency { get; set; }

        public int GetValue(double time)
        {
            return (int)(32767.5*Math.Sin(2f * Math.PI * Frequency * time) + 32767.5);
        }
    }
}
