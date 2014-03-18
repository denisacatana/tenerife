using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace joculet.Classes
{
    public abstract class Change
    {
        public double FallTime { get; set; }
        public ChangeType Type { get; set; }

        public List<Coordonate> Dependent { get; set; }
        public List<Coordonate> Affected { get; set; }

        public Change()
        {
            FallTime = 0.2;
        }
    }

    public class FallDownChange : Change
    {
        public Coordonate Start { get; set; }
        public Coordonate Finish { get; set; }

        public FallDownChange(Coordonate start, Coordonate finish)
            : base()
        {
            Start = start;
            Finish = finish;
            Type = ChangeType.FallDown;

            Affected = new List<Coordonate>();
            Affected.Add(start);
            Affected.Add(finish);
            Dependent = new List<Coordonate>();
            Dependent.Add(start);
            Dependent.Add(finish);
        }
    }

    public class CreateCandyChange : Change
    {
        public CandyType TipBomboana { get; set; }
        public Coordonate CoordonateCandy { get; set; }

        public CreateCandyChange(CandyType tipBomboana, Coordonate coordonateCandy)
            : base()
        {
            TipBomboana = tipBomboana;
            CoordonateCandy = coordonateCandy;
            Type = ChangeType.CreateCandy;

            Dependent = new List<Coordonate>();
            Dependent.Add(coordonateCandy);
            Affected = new List<Coordonate>();
            Affected.Add(coordonateCandy);
        }
    }

    public class DestroyCandyChange : Change
    {
        public Coordonate CoordonateCandy { get; set; }

        public DestroyCandyChange(Coordonate coordonateCandy)
            : base()
        {
            CoordonateCandy = coordonateCandy;
            Type = ChangeType.DestroyCandy;

            Dependent = new List<Coordonate>();
            Dependent.Add(coordonateCandy);
            Affected = new List<Coordonate>();
            Affected.Add(coordonateCandy);
        }
    }

    public class WaitChange : Change
    {
        public WaitChange(Slot[,] slots,int height,int width)
            : base()
        {
            Type = ChangeType.Wait;

            Dependent = new List<Coordonate>();
            Affected = new List<Coordonate>();

            FallTime = 0;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (slots[i, j] != null)
                    {
                        Dependent.Add(new Coordonate(i, j));
                        Affected.Add(new Coordonate(i, j));
                    }
                }
            }
        }
    }

    public class DestroyMultipleCandiesChange : Change
    {
        public DestroyMultipleCandiesChange(List<Coordonate> dependencies)
            : base()
        {
            FallTime = 0;
            Type = ChangeType.DestroyMultipleCandies;

            Dependent = new List<Coordonate>();
            Affected = new List<Coordonate>();
            foreach (Coordonate dep in dependencies)
            {
                Dependent.Add(dep);
                Affected.Add(dep); 
            }
        }
    }

    public enum ChangeType
    {
        FallDown,
        CreateCandy,
        DestroyCandy,
        DestroyMultipleCandies,
        Wait
    }
}
