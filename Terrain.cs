using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace joculet.Classes
{
    public class Terrain
    {
        private Slot[,] slots;
        private List<Coordonate> coordonate;
        private List<Coordonate> candyBuffer;

        public int Height { get; set; }
        public int Width { get; set; }

        public Terrain(bool[,] slotExist, int Width, int Height)
        {
            this.Width = Width;
            this.Height = Height;

            candyBuffer = new List<Coordonate>();
            slots = new Slot[Height, Width];

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    if (slotExist[i, j])
                    {
                        slots[i, j] = new Slot(i,j);
                    }
                }
            }
        }

        private Slot GetUpSlot(int i,int j)
        {
            if (i == 0)
                return null;
            else
            {
                if (slots[i - 1, j] == null)
                {
                    return GetUpSlot(i - 1, j);
                }
                else
                {
                    return slots[i - 1, j];
                }
            }
        }

        public VisualChanges Initialize()
        {
            VisualChanges currentChanges = new VisualChanges();           
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    Slot up = null, down = null, left = null, right = null;
                    if (slots[i, j] != null)
                    {
                        up = GetUpSlot(i, j);
                        if (i < Height - 1)
                        {
                            if (slots[i + 1, j] != null)
                            {
                                down = slots[i + 1, j];
                            }
                        }
                        if (j > 0)
                        {
                            if (slots[i, j - 1] != null)
                            {
                                left = slots[i, j - 1];
                            }
                        }
                        if (j < Width - 1)
                        {
                            if (slots[i, j + 1] != null)
                            {
                                right = slots[i, j + 1];
                            }
                        }
                        slots[i, j].SetNearSlots(up, down, left, right);
                    }
                   }
            }
            CandyFall(currentChanges);
            return currentChanges;
        }

        public void AddDestroyCandy(Candy candy)
        {
            candyBuffer.Add(candy.ContainerSlot.CurrentCoordonate);
        }

        public Slot GetSlot(int i, int j)
        {
            return slots[i, j];
        }

        public void CandyFall(VisualChanges currentChanges)
        {
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    if (slots[i, j] != null)
                    {
                        slots[i, j].CandyFall(currentChanges);
                    }
                }
            }
            currentChanges.Add(new WaitChange(slots, Height, Width));
            CheckMoves(currentChanges);
            DestroyCandies(currentChanges);
        }

        public void CandyFall(VisualChanges currentChanges, List<Coordonate> destroyCoordonate)
        {
            foreach (Coordonate coord in destroyCoordonate)
            {
                slots[coord.X, coord.Y].CandyFall(currentChanges);
            }
            currentChanges.Add(new WaitChange(slots, Height, Width));
            CheckMoves(currentChanges);
            DestroyCandies(currentChanges);
        }

        public void CheckMoves(VisualChanges currentChanges)
        {
            for (int i = 0; i < Height; i++)
                for (int j = 0; j < Width; j++)
                {
                    if (slots[i, j] != null)
                    {
                        if (slots[i, j].CurrentCandy != null)
                        {
                            slots[i, j].CurrentCandy.CheckDestroy(currentChanges, slots, Height, Width,this);
                        }
                    }
                }
        }

        public void DestroyCandies(VisualChanges currentChanges)
        {
            if (candyBuffer.Count > 0)
            {
                List<Coordonate> emptySlots = new List<Coordonate>();
                foreach (Coordonate candy in candyBuffer)
                {
                    emptySlots.Add(new Coordonate(candy.X, candy.Y));
                }
                candyBuffer = new List<Coordonate>();
                CandyFall(currentChanges, emptySlots);
            }
        }
        /*
        public void CandyFall(Coordonate[] coordonates)
        {

        }

        public void CheckMoves()
        {
            bool noMoves = true;
            CheckAlreadyGood();

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; i < Width; j++)
                {
                    Move moveDown = new Move(new Coordonate(i, j), new Coordonate(i - 1, j));
                    Move moveUp = new Move(new Coordonate(i, j), new Coordonate(i + 1, j));
                    Move moveRight = new Move(new Coordonate(i, j), new Coordonate(i, j + 1));
                    Move moveLeft = new Move(new Coordonate(i, j), new Coordonate(i, j - 1));

                    if (moveUp.CanExecute(slots, Height, Width) || moveDown.CanExecute(slots, Height, Width) || moveLeft.CanExecute(slots, Height, Width) || moveRight.CanExecute(slots, Height, Width))
                    {
                        noMoves = false; i = Width; j = Height;
                    }
                   
                }
            }
            if (noMoves)
            {
                Shuffle();
            }
        }

        public void CheckAlreadyGood()
        {
            //VERIFIC MUTARI
        }

        public void CheckMoves(Coordonate[] coordonates)
        {

        }

        public void Shuffle()
        {
            //Shuffle
            CheckMoves();
        }
        */
    }
}
