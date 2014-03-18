using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace joculet.Classes
{
    public class Level
    {
        public Terrain Layout { get; set; }

        public Level(bool[,] slotExist,int width,int height)
        {
            Layout = new Terrain(slotExist, width, height);
        }

        public VisualChanges LoadLevel()
        {
            return Layout.Initialize();
        }

        public void ExecuteMove(Move move)
        {

        }

        public bool TestWin()
        {
            return false;
        }
    }
}
