﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Filmobus_test.Models
{
    public class Level:IMode
    {
        public int Value { get; set; }
        public bool IsDynamic => false;
        public int GetValue(double time)
        {
            return Value;
        }
    }
}
