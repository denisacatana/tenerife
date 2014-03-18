using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace joculet.Classes
{
    public abstract class ScreenAnimation
    {
        public abstract void Do(double deltaTime);
    }

    public class MoveAnimation : ScreenAnimation
    {
        public ImageCandy MoveCandy { get; set; }
        private double positionX;
        private double positionY;

        private Coordonate startCoordonate;
        private Coordonate finishCoordonate;

        private double time;

        public MoveAnimation(ImageCandy candy,Coordonate startCoordonate,Coordonate finishCoordonate,int height,int width,double time)
            : base()
        {
            MoveCandy = candy;

            this.startCoordonate = new Coordonate(startCoordonate.X * height, startCoordonate.Y * width);
            this.finishCoordonate = new Coordonate(finishCoordonate.X * height, finishCoordonate.Y * width);
            
            this.time = time;

            if (MoveCandy.Type == CandyType.Inimioara)
            {
                BitmapImage logo = new BitmapImage();
                logo.BeginInit();
                logo.UriSource = new Uri("C:\\Users\\Andreea Florea\\Desktop\\tenerife\\joculet T\\joculet\\Resources\\scaredHeart.png");
                logo.EndInit();

                MoveCandy.Source = logo;
            }
        }

        public override void Do(double currentTime)
        {
            if (currentTime < time)
            {
                positionX = startCoordonate.X +(finishCoordonate.X - startCoordonate.X) * currentTime / time;
                positionY = startCoordonate.Y +(finishCoordonate.Y - startCoordonate.Y) * currentTime / time;
            }
            else
            {
                positionX = finishCoordonate.X;
                positionY = finishCoordonate.Y;

                if (MoveCandy.Type == CandyType.Inimioara)
                {
                    BitmapImage logo = new BitmapImage();
                    logo.BeginInit();
                    logo.UriSource = new Uri("C:\\Users\\Andreea Florea\\Desktop\\tenerife\\joculet T\\joculet\\Resources\\heart.png");
                    logo.EndInit();

                    MoveCandy.Source = logo;
                }
            }
            Canvas.SetTop(MoveCandy, positionX);
            Canvas.SetLeft(MoveCandy, positionY);
        }
    }
}
