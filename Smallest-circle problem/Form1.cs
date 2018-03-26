using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Smallest_circle_problem
{
    public partial class Form1 : Form
    {
        private const int POINT_RADIUS = 8;
        private List<Point> points = new List<Point>();
        private List<Point> convexHull = new List<Point>();
        private Point movePoint = new Point(-1, -1);
        private int radius;
        private Point center = new Point();


        public Form1()
        {
            InitializeComponent();
            pictureBox1.Height = this.Height;
            pictureBox1.Width = this.Width;
            pictureBox1.Location = this.Location;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            pictureBox1.Width = this.Width;
            pictureBox1.Height = this.Height;
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    LeftMouseClik(e.Location);
                    break;
                case MouseButtons.Right:
                    RightMouseClik(e.Location);
                    break;
            }
            Draw();
        }

        private void LeftMouseClik(Point p)
        {
            points.Add(p);
        }

        private void RightMouseClik(Point p)
        {
            DeletePoint(p);
            //Draw();
        }

        private void DeletePoint(Point p)
        {
            foreach(Point i in points)
            {
                if ((Math.Abs(i.X - p.X) < 3)  && (Math.Abs(i.Y - p.Y) < 3))
                {
                    points.Remove(i);
                    break;
                }
            }

        }
        

        private void Draw()
        {
            Bitmap bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            Graphics myGraphics = Graphics.FromImage(bmp);
            myGraphics.Clear(Color.White);

            if (points.Count > 1)
            {
              GetCircle();
               
                Pen myCirclePen = new Pen(Color.Red, POINT_RADIUS);
                Point a = new Point(center.X - radius, center.Y - radius);
                myGraphics.DrawEllipse(myCirclePen, a.X, a.Y, radius*2, radius*2);

                convexHull = SetConvexHull();
                Pen myHullPen = new Pen(Color.Black, POINT_RADIUS);
                for (int i = 0; i < convexHull.Count; i++)
                    myGraphics.DrawLine(myHullPen, convexHull[i], convexHull[(i + 1) % convexHull.Count]);
               
            }

            Brush myBrush = new SolidBrush(Color.Navy);
            foreach (Point i in points)
            {
                myGraphics.FillEllipse(myBrush, i.X - POINT_RADIUS, i.Y - POINT_RADIUS, POINT_RADIUS*2, POINT_RADIUS*2);
            }
            pictureBox1.Image = bmp;
        }

        private void GetCircle()
        {
            if (points.Count == 2)
            {
                center = GetCenter(points[0], points[1]);
                radius = (int)Math.Sqrt(Math.Pow((center.X - points[0].X), 2) + Math.Pow((center.Y - points[0].Y), 2));
            }
            else
            {
                Point firstP = new Point();
                firstP = points[0];
                Point secondP = new Point();
                secondP = farthestPoint(firstP);
                if (Dist(firstP, secondP) == 0)
                {
                    radius = 0;
                    return;
                }
                int opP = 2;
                Point thirdP = new Point();
                Point cen = new Point();
                cen = GetCenter(firstP, secondP);
                int rad = Dist(firstP, cen);
                int newRad = 0;
                while (true)
                {
                    Point newP = new Point();
                    newP = farthestPoint(cen);
                    if (Dist(cen, newP) <= Dist(cen, firstP))
                    {
                        center = cen;
                        radius = (int)(Math.Sqrt((double)rad) + 0.5);
                        break;
                    }
                    
                    if (Dist(firstP, newP) < Dist(secondP, newP))
                        Swap(ref firstP, ref secondP);
                    if ((opP == 3) && (Dist(firstP, newP) < Dist(thirdP, newP)))
                        Swap(ref firstP, ref thirdP);
                    Point centBeetFAndN = new Point();
                    centBeetFAndN = GetCenter(firstP, newP);
                    bool flag = true;
                    if (opP == 2)
                    {
                        flag = false;
                        if (Dist(secondP, centBeetFAndN) <= Dist(firstP, centBeetFAndN))
                        {
                            secondP = newP;
                            cen = GetCenter(firstP, secondP);
                            newRad = Dist(cen, firstP);
                            opP = 2;
                        }
                        else
                        {
                            thirdP = newP;
                            opP = 3;
                            if (secondP != thirdP)
                                cen = GetCentetByThreePoint(firstP, secondP, thirdP);
                            else cen = GetCenter(firstP, secondP);
                            if (cen.X < 10)
                                MessageBox.Show("222222");
                            newRad = Dist(cen, firstP);
                        }
                    }
                    if (flag)
                    {
                        if (Dist(secondP, centBeetFAndN) < Dist(thirdP, centBeetFAndN))
                            Swap(ref secondP, ref thirdP);
                        if (Dist(secondP, centBeetFAndN) <= Dist(firstP, centBeetFAndN))
                        {
                            opP = 2;
                            cen = centBeetFAndN;
                            newRad = Dist(cen, firstP);

                        }
                        else
                        {
                            thirdP = newP;
                            opP = 3;
                            if (secondP != thirdP)
                                cen = GetCentetByThreePoint(firstP, secondP, thirdP);
                            else cen = GetCenter(firstP, secondP);
                            newRad = Dist(cen, firstP);
                        }
                    }
                    if (newRad == rad)
                    {
                        center = cen;
                         radius = (int)(Math.Sqrt((double)newRad) + 0.5);
                        break;
                    }
                    rad = newRad;
                }
            }
        }

        private Point GetCentetByThreePoint(Point a, Point b, Point c)
        {
            Point m1, m2 = new Point();
            m1 = GetCenter(a, c);
            m2 = GetCenter(a, b);
            //MessageBox.Show(m2.X.ToString() + " " + m2.Y.ToString());
            double a1, d1, c1, a2, c2, d2;
            a1 = c.X - a.X;
            d1 = c.Y - a.Y;
            c1 = -a1 * m1.X - d1 * m1.Y;
            //MessageBox.Show(a1.ToString() + " " + d1.ToString() + " " + c1.ToString());

            a2 = b.X - a.X;
            d2 = b.Y - a.Y;
            c2 = -a2 * m2.X - d2 * m2.Y;
            //MessageBox.Show(a2.ToString() + " " + d2.ToString() + " " + c2.ToString());

            
            Point cen = new Point();
            cen.Y = (int)(((double)(-a1 * c2 + a2 * c1)) / (a1 * d2 - a2 * d1) + 0.5);
            cen.X = (int)(((double)(-c1 * d2 + d1 * c2)) / (a1 * d2 - a2 * d1) + 0.5);
            //MessageBox.Show(cen.X.ToString() + "  " + cen.Y.ToString());

            return cen;
        }

        private void Swap(ref Point firstP, ref Point secondP)
        {
            Point p = new Point(firstP.X, firstP.Y);
            firstP.X = secondP.X;
            firstP.Y = secondP.Y;
            secondP.X = p.X;
            secondP.Y = p.Y;
        }

        private Point  GetCenter(Point a, Point b)
        {
            Point p = new Point();
            p.X = (a.X + b.X)/ 2;
            p.Y = (a.Y + b.Y)/ 2;
            return p;
        }

        private Point farthestPoint(Point p)
        {
            Point ans = new Point();
            ans = points[0];
            foreach (Point i in points)
            {
                if (Dist(i, p) > Dist(p, ans))
                {
                    ans.X = i.X;
                    ans.Y = i.Y;
                }
            }
            return ans;
        }

        private List<Point> SetConvexHull()
        {
            if (points.Count < 3)
                return points;
            List<Point> myHull = new List<Point>();
            List<Point> myPoints = new List<Point>(points);
        
            myHull.Add(firstPoint(myPoints));
        
            Point next = new Point();
            while((next != myHull[0]) && (myPoints.Count > 0))
            {
                next = myPoints[0];
                foreach(Point i in myPoints)
                {
                    int last = myHull.Count - 1;
                    int vProduct = VectorProduct(myHull[last], next, i);
                    if (vProduct > 0)
                    {
                        next = i;
                    }
                    else if ((vProduct == 0) && (Dist(myHull[last], i) > Dist(myHull[last], next)))
                    {
                        next = i;
                    }
                }
                myHull.Add(next);
            }
            return myHull;
        }

        private int Dist(Point a, Point b)
        {
            return (a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y);
        }

        private int VectorProduct(Point a, Point b, Point c)
        {
            return (b.X - a.X) * (c.Y - a.Y) - (c.X - a.X) * (b.Y - a.Y);
        }

        private Point firstPoint(List<Point> myPoints)
        {

            Point p = new Point(this.Width, this.Height);
            foreach (Point i in myPoints)
            {
                if ((i.X < p.X) || (i.X == p.X && i.Y < p.Y))
                {
                    p.X = i.X;
                    p.Y = i.Y;
                }
            }
            return p;
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                return;       
            Point p = e.Location;
            foreach (Point i in points)
            {
                if ((Math.Abs(i.X - p.X) < 3) && (Math.Abs(i.Y - p.Y) < 3))
                {
                    movePoint = i;
                    points.Remove(i);
                    break;
                }
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (movePoint.X != -1)
            {
                points.Add(e.Location);
                Draw();
                points.RemoveAt(points.Count - 1);
            }

        }
        
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (movePoint.X != -1)
            {
                Draw();
                movePoint.X = -1;
                movePoint.Y = -1;
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            points.Clear();
            Draw();
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            //sdsdsdsds
            Random r = new Random();
            int count = r.Next(100, 1000);
            MessageBox.Show($"Proram has created {count} points", "Random filling");
            points.Clear();
            for (int i = 0; i < count; i++)
            {
                Point p = new Point(r.Next(40, pictureBox1.Width - 40), r.Next(50, pictureBox1.Height - 60));
                points.Add(p);
            }
            Draw();
        }

        private void aboutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Some text", "About");
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Some text", "Help");
        }
    }
}
