using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using joculet.Classes;
using System.Windows.Threading;

namespace joculet
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Level level1;

        private ImageSlot[,] imageSlots;
        private ImageCandy[,] imageCandies;

        private DispatcherTimer animationTimer;
        private double animationTime;
        private double fullAnimationTime;
        private List<ScreenAnimation> animations;
        private VisualChanges changes;

        private UIElement animatedObject;

        public double OffsetX = 0;
        public double OffsetY = 0;

        public MainWindow()
        {
            InitializeComponent();
            animationTimer = new DispatcherTimer();
            animationTimer.Interval = new TimeSpan(0, 0, 0, 0, 10);
            animationTimer.Tick += new EventHandler(animationTimer_Tick);

            int height = 8, width = 8;
            bool[,] slots = new bool[height, width];

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    slots[i, j] = true;
                }
            }

            //slots[4, 4] = false;
            //slots[1, 6] = false;
            //slots[2, 5] = false;
            //slots[3, 6] = false;
            //slots[0, 7] = false;

            level1 = new Level(slots, width, height);
            OffsetX = (Height-GetPixelHeight(level1))/2;
            OffsetY = (Width - GetPixelWidth(level1)) / 2;
            LoadLevel(level1);
        }

        public double GetPixelHeight(Level level)
        {
            return level.Layout.Height * ImageSlot.SlotHeight;
        }

        public double GetPixelWidth(Level level)
        {
            return level.Layout.Width * ImageSlot.SlotWidth;
        }

        public void LoadLevel(Level level)
        {
            changes = level.LoadLevel();
            Terrain levelTerrain = level.Layout;

            imageSlots = new ImageSlot[levelTerrain.Height, levelTerrain.Width];
            imageCandies = new ImageCandy[levelTerrain.Height, levelTerrain.Width];

            for(int i = 0;i<levelTerrain.Height;i++)
            {
                for(int j = 0;j<levelTerrain.Width;j++)
                {
                    Slot screenSlot = levelTerrain.GetSlot(i,j);
                    if(screenSlot != null)
                    {
                        imageSlots[i, j] = new ImageSlot();
                        canvasDisplay.Children.Add(imageSlots[i, j]);

                        Canvas.SetTop(imageSlots[i, j],i * ImageSlot.SlotHeight);
                        Canvas.SetLeft(imageSlots[i, j], j * ImageSlot.SlotWidth);
                    }
                }
            }
            SetAnimationsToLevels(levelTerrain);
            ExecuteChanges();            
        }

        List<List<Change>> levelChanges;
        public int changeLength;

        public void SetAnimationsToLevels(Terrain terrain)
        {
            List<Nod>[,] changeMatrix = new List<Nod>[terrain.Height,terrain.Width];
            List<Nod> nods = new List<Nod>();

            for (int i = 0; i < terrain.Height; i++)
                for (int j = 0; j < terrain.Width; j++)
                    changeMatrix[i, j] = new List<Nod>();
            List<Nod> startingAnimations = new List<Nod>();
            for (int i = 0; i < changes.Length; i++)
            {
                Nod x = new Nod(changes[i]);
                nods.Add(x);

                foreach (Coordonate coor in changes[i].Dependent)
                {
                    if (changeMatrix[coor.X, coor.Y].Count > 0)
                    {
                        Nod last = changeMatrix[coor.X, coor.Y][changeMatrix[coor.X, coor.Y].Count - 1];
                        last.AddChild(nods[i]);
                    }
                }
                foreach (Coordonate coor in changes[i].Affected)
                {
                    changeMatrix[coor.X, coor.Y].Add(x);
                }

                if (nods[i].HasNoDependencies())
                {
                    startingAnimations.Add(nods[i]);
                }
            }
            levelChanges = new List<List<Change>>();
            changeLength = 0;
            while (startingAnimations.Count > 0)
            {
                List<Nod> newStarting = new List<Nod>();
                levelChanges.Add(new List<Change>());

                foreach (Nod change in startingAnimations)
                {
                    levelChanges[changeLength].Add(change.Animation);
                    List<Nod> freedChildren = change.FreeChildren();
                    foreach (Nod nod in freedChildren)
                    {
                        newStarting.Add(nod);
                    }
                }
                startingAnimations = newStarting;
                changeLength += 1;
            }
            execChanges = 0;

            /*
            levelChanges = new List<List<Change>>();
            for(int i=0;i<changes.Length;i++)
            {
                levelChanges.Add(new List<Change>());
                levelChanges[i].Add(changes[i]);
            }
            changeLength = changes.Length;
            */
            //string Pr = "";
            //foreach(Change ch in changes.Changes)
            //{
            //    Pr += ch.Type+" ";
            //}
            //MessageBox.Show(Pr);

            //Pr = "";
            //for (int i = 0; i < changeLength; i++)
            //{
            //    foreach(Change ch in levelChanges[i])
            //    {
            //        Pr += ch.Type + "  ";
            //    }
            //    Pr += "\n";
            //}
            //MessageBox.Show(Pr);
        }

        private int execChanges;
        public void ExecuteChanges()
        {
            animations = new List<ScreenAnimation>();
            if (execChanges < changeLength)
            {
                foreach (Change change in levelChanges[execChanges])
                {
                    AnimateChange(change);
                }
            }
            execChanges += 1;
            //Change animationChange = changes.GetNext();

            //if (animationChange != null)
            //{
            //    AnimateChange(animationChange);
            //}
        }

        public void MoveToCoordonate(UIElement obj, Coordonate coordonate)
        {
            Canvas.SetTop(obj, coordonate.X * ImageSlot.SlotHeight);
            Canvas.SetLeft(obj, coordonate.Y * ImageSlot.SlotWidth);
        }

        public void AnimateChange(Change change)
        {
            switch (change.Type)
            {
                case ChangeType.FallDown:
                    Coordonate startCoordonate = ((FallDownChange)change).Start;
                    Coordonate finishCoordonate = ((FallDownChange)change).Finish;
                    animatedObject = imageCandies[startCoordonate.X, startCoordonate.Y];
                    imageCandies[finishCoordonate.X, finishCoordonate.Y] = (ImageCandy)animatedObject;

                    animations.Add(new MoveAnimation(imageCandies[finishCoordonate.X, finishCoordonate.Y], startCoordonate, finishCoordonate,ImageSlot.SlotHeight,ImageSlot.SlotWidth,change.FallTime));
                    break;
                case ChangeType.CreateCandy:
                    CreateCandyChange createChange = (CreateCandyChange)change;
                    animatedObject = new ImageCandy(createChange.TipBomboana);
                    canvasDisplay.Children.Add(animatedObject);
                    MoveToCoordonate(animatedObject, createChange.CoordonateCandy);
                    imageCandies[createChange.CoordonateCandy.X, createChange.CoordonateCandy.Y] = (ImageCandy)animatedObject;
                    //animation = null;
                    break;
                case ChangeType.DestroyCandy:
                    DestroyCandyChange destroyChange = ((DestroyCandyChange)change);
                    ImageCandy destroyImage = imageCandies[destroyChange.CoordonateCandy.X, destroyChange.CoordonateCandy.Y];
                    //Canvas.SetLeft(destroyImage, Canvas.GetLeft(destroyImage) + 10);
                    //destroyImage.Width -= 20;
                    canvasDisplay.Children.Remove(destroyImage);
                    //animation = null
                    break;
            }

            fullAnimationTime = change.FallTime;
            animationTime = change.FallTime;
            animationTimer.Start();
        }

        void animationTimer_Tick(object sender, EventArgs e)
        {
            double deltaTime = ((double)animationTimer.Interval.Milliseconds) / 1000;
            animationTime -= deltaTime;

            if (animations.Count > 0)
            {
                foreach (ScreenAnimation anim in animations)
                {
                    anim.Do(fullAnimationTime - animationTime);
                }
            }

            //if (animation != null)
            //{
            //    animation.Do(fullAnimationTime-animationTime);
            //}

            if (animationTime <= 0)
            {
                animationTimer.Stop();
                ExecuteChanges();
            }
        }
    }
}

public class Nod
{
    public Change Animation { get;private set; }
    private int dependencyLevel = 0;
    private List<Nod> children;

    public Nod(Change change)
    {
        Animation = change;
        children = new List<Nod>();
        dependencyLevel = 0;
    }

    public List<Nod> FreeChildren()
    {
        List<Nod> freeChildren = new List<Nod>();
        foreach(Nod nod in children)
        {
            nod.RemoveDependency();
            if(nod.HasNoDependencies())
            {
                freeChildren.Add(nod);
            }
        }
        return freeChildren;
    }

    public void RemoveDependency()
    {
        dependencyLevel -= 1;
    }

    public bool HasNoDependencies()
    {
        return dependencyLevel == 0;
    }

    public void AddDependency()
    {
        dependencyLevel += 1;
    }

    public void AddChild(Nod nod)
    {
        children.Add(nod);
        nod.AddDependency();
    }
}

public class ImageSlot : Image
{
    static public int SlotWidth = 64;
    static public int SlotHeight = 64;
    public ImageSlot()
        : base()
    {
        Width = SlotWidth;
        Height = SlotHeight;

        BitmapImage logo = new BitmapImage();
        logo.BeginInit();
        logo.UriSource = new Uri("C:\\Users\\Andreea Florea\\Desktop\\tenerife\\joculet T\\joculet\\Resources\\newSlot.png");
        logo.EndInit();
        Source = logo;
    }
}

public class ImageCandy : Image
{
    public CandyType Type { get; set; }

    public ImageCandy(CandyType candyType)
        : base()
    {
        Width = 64;
        Height = 64;

        BitmapImage logo = new BitmapImage();
        logo.BeginInit();
        Type = CandyType.Fulger;
        switch (candyType)
        {
            case CandyType.Inimioara:
                logo.UriSource = new Uri("C:\\Users\\Andreea Florea\\Desktop\\tenerife\\joculet T\\joculet\\Resources\\heart.png");
                Type = CandyType.Inimioara;
                break;
            case CandyType.Steluta:
                logo.UriSource = new Uri("C:\\Users\\Andreea Florea\\Desktop\\tenerife\\joculet T\\joculet\\Resources\\starnew.png");
                break;
            case CandyType.Norisor:
                logo.UriSource = new Uri("C:\\Users\\Andreea Florea\\Desktop\\tenerife\\joculet T\\joculet\\Resources\\cloud.png");
                break;
            case CandyType.SemiLuna:
                logo.UriSource = new Uri("C:\\Users\\Andreea Florea\\Desktop\\tenerife\\joculet T\\joculet\\Resources\\moonnew.png");
                break;
            case CandyType.Fulger:
                logo.UriSource = new Uri("C:\\Users\\Andreea Florea\\Desktop\\tenerife\\joculet T\\joculet\\Resources\\thunder.png");
                break;
        }
        logo.EndInit();
        Source = logo;
    }

}