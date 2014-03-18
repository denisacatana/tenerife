using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace joculet.Classes
{
    public class Coordonate
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Coordonate(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static Coordonate operator +(Coordonate a,Coordonate  b)
        {
            return new Coordonate(a.X + b.X, a.Y + b.Y);
        }

        public static Coordonate operator -(Coordonate a, Coordonate b)
        {
            return new Coordonate(a.X - b.X, a.Y - b.Y);
        }

        public static Coordonate operator *(Coordonate a, int value)
        {
            return new Coordonate(a.X * value, a.Y * value);
        }

        public override String ToString()
        {
            return "(" + X + "," + Y + ")";
        }
    }
}
