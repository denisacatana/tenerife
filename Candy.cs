using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace joculet.Classes
{
    public class Candy 
    {
        public CandyType Type { get; set; }
        public Slot ContainerSlot { get; set; }

        public static int numberOfTypesCandy = 5;
        public static Random r;

        public Candy(VisualChanges currentChanges, Slot currentSlot)
        {
            if (r == null)
                r = new Random();

            Type = (CandyType)r.Next(numberOfTypesCandy);
            ContainerSlot = currentSlot;

            CreateCandyChange change = new CreateCandyChange(Type, this.ContainerSlot.CurrentCoordonate);
            currentChanges.Add(change);
        }

        public bool CanExecuteMove(Move move, Slot[,] slots, int Height, int Width)
        {
            return false;
        }

        //public void Destroy(VisualChanges currentChanges, List<Coordonate> dependencies)
        //{
        //    DestroyCandyChange change = new DestroyCandyChange(this.ContainerSlot.CurrentCoordonate,dependencies);
        //    currentChanges.Add(change);
        //    ContainerSlot.CurrentCandy = null;
        //}

        public void Destory(VisualChanges currentChanges,Terrain terrain)
        {
            DestroyCandyChange change = new DestroyCandyChange(this.ContainerSlot.CurrentCoordonate);
            currentChanges.Add(change);
            ContainerSlot.CurrentCandy = null;
            terrain.AddDestroyCandy(this);
        }

        public void DestroyCandies(VisualChanges currentChanges, Slot[,] slots, int height, int width, int nrUp, int nrDown, int nrLeft, int nrRight,Terrain terrain)
        {
            int x = ContainerSlot.CurrentCoordonate.X;
            int y = ContainerSlot.CurrentCoordonate.Y;

            List<Coordonate> destroyedCandies = new List<Coordonate>();
            if (nrUp + nrDown >= 2 || nrLeft + nrRight >= 2)
            {
                destroyedCandies.Add(slots[x, y].CurrentCandy.ContainerSlot.CurrentCoordonate);

                if (nrUp + nrDown >= 2)
                {
                    for (int i = x - 1; i >= 0; i--)
                    {
                        if (nrUp > 0)
                        {
                            destroyedCandies.Add(slots[i, y].CurrentCandy.ContainerSlot.CurrentCoordonate);
                            nrUp--;
                        }
                    }
                    for (int i = x + 1; i < height; i++)
                    {
                        if (nrDown > 0)
                        {
                            destroyedCandies.Add(slots[i, y].CurrentCandy.ContainerSlot.CurrentCoordonate);
                            nrDown--;
                        }
                    }
                }
                if (nrLeft + nrRight >= 2)
                {
                    for (int j = y - 1; j >= 0; j--)
                    {
                        if (nrLeft > 0)
                        {
                            destroyedCandies.Add(slots[x, j].CurrentCandy.ContainerSlot.CurrentCoordonate);
                            nrLeft--;
                        }
                    }
                    for (int j = y + 1; j < height; j++)
                    {
                        if (nrRight > 0)
                        {
                            destroyedCandies.Add(slots[x, j].CurrentCandy.ContainerSlot.CurrentCoordonate);
                            nrRight--;
                        }
                    }
                }

                DestroyMultipleCandiesChange change = new DestroyMultipleCandiesChange(destroyedCandies);
                currentChanges.Add(change);

                foreach (Coordonate coord in destroyedCandies)
                {
                    slots[coord.X, coord.Y].CurrentCandy.Destory(currentChanges, terrain);
                }
            }
        }

        public void CheckDestroy(VisualChanges currentChanges, Slot[,] slots, int height, int width,Terrain terrain)
        {
            int x = ContainerSlot.CurrentCoordonate.X;
            int y = ContainerSlot.CurrentCoordonate.Y;
            
            int nrUp = 0, nrDown = 0, nrLeft = 0, nrRight = 0;

            nrUp = CheckUp(slots, x, y);
            nrDown = CheckDown(slots, x, y, height);
            nrLeft = CheckLeft(slots, x, y);
            nrRight = CheckRight(slots, x, y, width);

            //nrUp = CheckDirection(Direction.Up, slots, x, y, width, height);
            //nrDown = CheckDirection(Direction.Down, slots, x, y, width, height);
            //nrLeft = CheckDirection(Direction.Left, slots, x, y, width, height);
            //nrRight = CheckDirection(Direction.Right, slots, x, y, width, height);

            DestroyCandies(currentChanges, slots, height, width, nrUp, nrDown, nrLeft, nrRight, terrain);
        }

        private int CheckUp(Slot[,] slots, int x, int y)
        {
            int nrUp = 0;
            for (int i = x - 1; i >= 0; i--)
            {
                if (slots[i, y] == null)
                    return nrUp;
                if (slots[i, y].CurrentCandy == null)
                    break;
                if (slots[i, y].CurrentCandy.Type == Type)
                    nrUp++;
                else
                    return nrUp;
            }
            return nrUp;
        }

        private int CheckDown(Slot[,] slots, int x, int y, int height)
        {
            int nrDown = 0;
            for (int i = x + 1; i < height; i++)
            {
                if (slots[i, y] == null)
                    break;
                if (slots[i, y].CurrentCandy == null)
                    break;
                if (slots[i, y].CurrentCandy.Type == Type)
                    nrDown++;
                else
                    return nrDown;
            }
            return nrDown;
        }

        private int CheckLeft(Slot[,] slots, int x, int y)
        {
            int nrLeft = 0;
            for (int j = y - 1; j >= 0; j--)
            {
                if (slots[x, j] == null)
                    break;
                if (slots[x, j].CurrentCandy == null)
                    break;
                if (slots[x, j].CurrentCandy.Type == Type)
                    nrLeft++;
                else
                    return nrLeft;
            }
            return nrLeft;
        }

        private int CheckRight(Slot[,] slots, int x, int y, int width)
        {
            int nrRight = 0;
            for (int j = y + 1; j < width; j++)
            {
                if (slots[x, j] == null)
                    break;
                if (slots[x, j].CurrentCandy == null)
                    break;
                if (slots[x, j].CurrentCandy.Type == Type)
                    nrRight++;
                else
                    return nrRight;
            }
            return nrRight;
        }

        public void DestroyNear()
        {

        }

        public bool CanFall()
        {
            return true;
        }
    }

    public enum Direction
    {
        Left,
        Right,
        Up,
        Down
    }

    public enum CandyType
    {
        Inimioara,
        Steluta,
        SemiLuna,
        Fulger,
        Norisor
    }
}
