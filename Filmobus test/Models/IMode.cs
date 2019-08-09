using System;

namespace Filmobus_test.Models
{
    public interface IMode
    {
        bool IsDynamic { get; }
        int GetValue(double time);
    }
}
