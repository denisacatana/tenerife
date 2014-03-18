using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace joculet.Classes
{
    public class VisualChanges
    {
        public List<Change> Changes { get; private set; }
        public int Length
        {
            get
            {
                return Changes.Count;
            }
            private set { }
        }

        public VisualChanges()
        {
            Changes = new List<Change>();
        }

        public Change this[int index]
        {
            get
            {
                return Changes[index];
            }
            private set
            {

            }
        }
        
        public void Add(Change change)
        {
            Changes.Add(change);
        }

        public Change GetNext()
        {
            if (Changes.Count > 0)
            {
                Change returnChange = Changes[0];
                Changes.RemoveAt(0);
                return returnChange;
            }
            return null;
        }
    }
}
