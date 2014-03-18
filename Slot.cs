using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace joculet.Classes
{
    public class Slot
    {
        private Slot slotUp;
        private Slot slotLeft;
        private Slot slotRight;
        private Slot slotDown;
        private SlotTypes type { get; set; }
        public Coordonate CurrentCoordonate { get; set; }
        private Candy currentCandy;
        public Candy CurrentCandy
        {
            get
            {
                return currentCandy;
            }
            set
            {
                currentCandy = value;
                if (currentCandy != null)
                    currentCandy.ContainerSlot = this;
            }
        }

        public void SetNearSlots(Slot up, Slot down, Slot left, Slot right)
        {
            slotUp = up;
            slotDown = down;
            slotLeft = left;
            slotRight = right;
        }

        /// <summary>
        /// Creaza un obiect nou de tipul Slot
        /// </summary>
        /// <param name="i">Punctul de coordonate X</param>
        /// <param name="j">Punctul de coordonate Y</param>
        public Slot(int i, int j)
        {
            CurrentCoordonate = new Coordonate(i, j);
        }

        public void CandyFall(VisualChanges currentChanges)
        {
            if (CurrentCandy == null)
            {
                if (slotUp == null)
                {
                    CurrentCandy = new Candy(currentChanges, this);
                }
                else
                {
                    if (slotUp.CurrentCandy == null)
                    {
                        CandyFallDown(currentChanges, this);
                    }
                    else
                    {
                        CurrentCandy = slotUp.CurrentCandy;
                        currentChanges.Add(new FallDownChange(slotUp.CurrentCoordonate, CurrentCoordonate));
                        slotUp.CurrentCandy = null;
                        slotUp.CandyFall(currentChanges);
                    }
                }
            }
        }

        public void CandyFallDown(VisualChanges currentChanges, Slot down)
        {
            CandyFall(currentChanges);
            down.CandyFall(currentChanges);
        }

        public bool CanExecute(Move move, Slot[,] slots, int Height, int Width)
        {
            return false;
        }

        //public void DestroyCandy(VisualChanges currentChanges, Terrain terrain)
        //{
        //    if (CurrentCandy != null)
        //    {
        //        CurrentCandy.Destroy(currentChanges, terrain);
        //    }
        //}

        public void SwapCandy(Slot otherSlot)
        {

        }
    }

    public enum SlotTypes
    {
        NormalSlot
    }
}
