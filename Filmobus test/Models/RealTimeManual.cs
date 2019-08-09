using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Filmobus_test.Models
{
    public class RealTimeManual:IMode
    {
        public int Value { get; set; }
        public bool IsDynamic => true;
        public int GetValue(double time)
        {
            return Value;
        }
    }
}
