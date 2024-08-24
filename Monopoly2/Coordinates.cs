using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monopoly2
{
    public class Coordinates
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Coordinates(double x, double y)
        {
            X = x;
            Y = y;
        }
    }
}
