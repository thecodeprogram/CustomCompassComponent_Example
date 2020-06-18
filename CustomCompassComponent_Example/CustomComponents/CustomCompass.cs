using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CustomCompassComponent_Example.CustomComponents
{
    public partial class CustomCompass : Control
    {

        private int Heading = 0;

        public CustomCompass()
        {
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            base.OnPaintBackground(pevent);
            //And lastly we are calling the Bitmap method on Painting Background override method to show it.
            pevent.Graphics.DrawImage(this.DrawCompass(this.Heading), new Point(0, 0));
        }

        //This function is required to refresh this component
        public void UpdateCompass(int _heading)
        {
            this.Heading = _heading;
            this.Refresh();
        }

        //This function return the needed distance from center
        //of the compass for the required markings of the compass
        public static double charSpread(double swidth)
        {
            return swidth / 6;
        }

        public Bitmap DrawCompass(double degree)
        {
            //First we need to know the component size.
            Size s = this.Size;
            
            //Then we create an image object and then a graphic object from this image.
            //We needgraphic object to make some drawings which in need for a compass
            Bitmap compassImage = new Bitmap(s.Width, s.Height);
            Graphics g = Graphics.FromImage(compassImage);

            //Set a black background for the compass
            SolidBrush blackBrush = new SolidBrush(Color.Black);
            Rectangle rect = new Rectangle(0, 0, compassImage.Width, compassImage.Height);
            g.FillRectangle(blackBrush, rect);


            //In here first we are going to declare some essential variables.
            //maxRadius variable will hold the max radius of compass. It will make the compass fit to the window.
            //sizeRatio variable will make the textx and lines appropraite according to window dimensions
            //outerradius and innerradius will specify the compass lines locations. Starting and ending points of the compass lines
            //degreeRadius will hold the radius of hidden circle of which hold the markings location
            double maxRadius = s.Width > s.Height ? s.Height / 2 : s.Width / 2;
            double sizeRatio = maxRadius / 360;
            double outerradius = maxRadius - sizeRatio * 60;
            double innerradius = maxRadius - sizeRatio * 90;
            double markingsRadius = maxRadius - sizeRatio * 135; 


            //Here we need to configuration of the marking texts and lines styles.
            //We will hold their sizes and colors here
            Font font1 = new Font("Arial", (float)(25 * sizeRatio));
            SolidBrush brushWhite = new SolidBrush(Color.FromArgb(255, 255, 255));
            Pen penThick = new Pen(Color.FromArgb(255, 255, 255), ((int)(sizeRatio) < 5 ? 5 : (int)(sizeRatio)));
            Pen penThin = new Pen(Color.FromArgb(255, 255, 255), ((int)(sizeRatio) < 2 ? 2 : (int)(sizeRatio)));

            //We are going to draw everything according to center point.
            //So I need to hold the center points of the compass.
            int xCenter = (int)(s.Width / 2);
            int yCenter = (int)((s.Height / 2));

            //We will also need the cosinus and sinus values of angles to locate the point 
            //even if the image has been rotated by a degree.

            //IMPORTANT : We are subtracting 90 from the angle because we need to rotate the image by 90 to counter clockwise.
            //because the axis started from 90 degrees and right side faced.
            double[] Cos = new double[360];
            double[] Sin = new double[360];
            for (int a = 0; a < 360; a++)
            {
                double curAngle = ((a - 90d) - degree) / 180F * Math.PI;
                Cos[a] = Math.Cos(curAngle);
                Sin[a] = Math.Sin(curAngle);
            }
            

            //In this for loop we are going to decide how many lines we are gonna draw.
            //As you can see below I increased the iterator by 5. So I will draw 360/5=72 lines.
            for (int d = 0; d < 360; d += 5)
            {
                //Here is the logic of the program:
                //--I will draw the thick lines at every 10 and its multiplies
                //--I will draw the thin lines at every 5 and its multiplies
                //--I will write the angles at every 30 degree
                //--And I will write the main directions N at 0, E at 90, S at 180 and W at 270 degrees.

                //In here we are going to specify the compass lines starting and ending points.
                //When we rotate the compassit will divide the circle into360 parts and it will get the starting and ending points
                //with a hidden triangle from the center. So I will use the cos and sin calculations
                //to locate the point on the x and y axis.
                Point pOuter = new Point((int)(outerradius * Cos[d]) + xCenter, (int)(outerradius * Sin[d]) + yCenter);
                Point pInner = new Point((int)(innerradius * Cos[d]) + xCenter, (int)(innerradius * Sin[d]) + yCenter);

                //Here I will locate the text as the same logic above. 
                Point pMarkings = new Point((int)(markingsRadius * Cos[d]) + xCenter, (int)(markingsRadius * Sin[d]) + yCenter);
                //Additionally I will center the text otherwise it will not located at the next of the compass line
                SizeF s1 = g.MeasureString(d.ToString(), font1);
                pMarkings.X = pMarkings.X - (int)(s1.Width / 2);
                pMarkings.Y = pMarkings.Y - (int)(s1.Height / 2);
                

                //I will draw the thick lines first.
                //Also I will perform the tasks which have to be performed at 30 and 90 degrees in here
                //I will draw the lines and text the markings in here
                if (d % 10 == 0) 
                {
                    //It will draw the line whatever happens at every multiplied by 10
                    g.DrawLine(penThick, pOuter, pInner);

                    //At Every 30 degress it will text the marking numbers.
                    if (d % 30 == 0)
                    {
                        //And every 90 degrees it will check and text main directions if it is
                        if (d % 90 == 0)
                        {
                            switch (d)
                            {
                                case 0:
                                case 360:
                                    g.DrawString("N", font1, brushWhite, pMarkings);
                                    break;

                                case 90:
                                    g.DrawString("E", font1, brushWhite, pMarkings);
                                    break;

                                case 180:
                                    g.DrawString(" S", font1, brushWhite, pMarkings);
                                    break;

                                case 270:
                                    g.DrawString("W", font1, brushWhite, pMarkings);
                                    break;
                            }
                        }
                        else //if current degree is not a main direction, it will write the degree
                            g.DrawString(d.ToString(), font1, brushWhite, pMarkings);
                    }
                }
                //It will draw the compass lines at every remain 5 degrees.
                else if (d % 5 == 0)
                {
                    g.DrawLine(penThin, pOuter, pInner);
                }
            }
            //Return the built compass image.
            return compassImage;
        }
    }
}
