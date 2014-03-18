using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace joculet.Classes
{
    public class Move
    {
        public Move(Coordonate first, Coordonate second)
        {
              
        }

        public bool CanExecute(Slot[,] slots,int height,int width)
        {
            return false;
        }

        public void Execute(Slot[,] slots, int height, int width)
        {

        }
    }

    public enum CanExecute
    {
        TRUE,
        FALSE,
        NOT

    }
}
