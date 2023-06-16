using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Lab6
{
    public partial class Form1 : Form
    {
        public Graphics gr; 
        public Graphics gr1;
        public int pcig = 180;
        public int phig = 109;
        public List<Figure> figures = new List<Figure>();
        public List<Label> labels = new List<Label>();
        bool showCoords = false;

        public Form1()
        {
            InitializeComponent();
            comboBox1.Items.AddRange(new string[] { "f(x, y) = sin(x)", "f(x, y) = sin(x) * sin(y)", "f(x, y) = sin(√(x^2 + y^2))", "f(x, y) = 1 / (1 + x^2) + 1 / (1 + y^2)" });
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            gr = pictureBox1.CreateGraphics();
            gr.TranslateTransform((float)pictureBox1.Width / 2, (float)pictureBox1.Height / 2);
            gr1 = pictureBox2.CreateGraphics();
            radioButton1.Checked = true;
        }

        public void draw()
        {
            gr.Clear(Color.White);
            labels.Clear();
            pictureBox1.Controls.Clear();
           
            foreach (var figure in figures)
            {
                foreach (var edge in figure.Edges)
                {
                    
                    List<Point3D> ps = new List<Point3D>() { figure.Points[edge.First], figure.Points[edge.Second] };
                    double pci = pcig * Math.PI / 180;
                    double phi = phig * Math.PI / 180;
                    double[,] mat = new double[4, 4] {
                    { Math.Cos(pci), Math.Sin(phi)*Math.Sin(pci), 0, 0 },
                    { 0, Math.Cos(phi), 0, 0 },
                    { Math.Sin(pci), -Math.Sin(phi)*Math.Cos(pci), 0, 0 },
                    { 0, 0, 0, 1 } };
                    List<Point3D> r = func(ps, mat);

                    if (showCoords)
                    {
                        labels.Add(new Label());
                        labels[labels.Count - 1].Location = new Point((int)r[0].X + 3 + pictureBox1.Width / 2, (int)r[0].Y + 3 + pictureBox1.Height / 2);
                        labels[labels.Count - 1].AutoSize = true;
                        labels[labels.Count - 1].Text = $"X = {(int)ps[0].X}; Y = {(int)ps[0].Y}; Z = {(int)ps[0].Z}";
                        pictureBox1.Controls.Add(labels[labels.Count - 1]);


                        labels.Add(new Label());
                        labels[labels.Count - 1].Location = new Point((int)r[1].X + 3 + pictureBox1.Width / 2, (int)r[1].Y + 3 + pictureBox1.Height / 2);
                        labels[labels.Count - 1].AutoSize = true;
                        labels[labels.Count - 1].Text = $"X = {(int)ps[1].X}; Y = {(int)ps[1].Y}; Z = {(int)ps[1].Z}";
                        pictureBox1.Controls.Add(labels[labels.Count - 1]);

                    }

                    gr.DrawLine(new Pen(Brushes.Black), new Point((int)r[0].X, (int)r[0].Y), new Point((int)r[1].X, (int)r[1].Y));  
                }
            }
        }

        // Создать гексаэдр
        public void MakeGeksaedr(Point3D center, double R, out List<Point3D> points, out List<Edge> edges, out List<Face> faces)
        {
            points = new List<Point3D>();
            edges = new List<Edge>();
            faces = new List<Face>();

            var k = Math.Sqrt(8 * R * R) / 2.0;

            var A = new Point3D(center.X - k, center.Y, center.Z - R);
            var B = new Point3D(center.X, center.Y - k, center.Z - R);
            var C = new Point3D(center.X + k, center.Y, center.Z - R);
            var D = new Point3D(center.X, center.Y + k, center.Z - R);
            var E = new Point3D(center.X - k, center.Y, center.Z + R);
            var F = new Point3D(center.X, center.Y - k, center.Z + R);
            var G = new Point3D(center.X + k, center.Y, center.Z + R);
            var H = new Point3D(center.X, center.Y + k, center.Z + R);

            points.Add(A);
            points.Add(B);
            points.Add(C);
            points.Add(D);
            points.Add(E);
            points.Add(F);
            points.Add(G);
            points.Add(H);

            var AB = new Edge(0, 1);
            var BC = new Edge(1, 2);
            var CD = new Edge(2, 3);
            var DA = new Edge(3, 0);
            var AE = new Edge(0, 4);
            var EH = new Edge(4, 7);
            var HG = new Edge(7, 6);
            var GC = new Edge(6, 2);
            var GF = new Edge(6, 5);
            var FE = new Edge(5, 4);
            var DH = new Edge(3, 7);
            var BF = new Edge(1, 5);

            edges.Add(AB);
            edges.Add(BC);
            edges.Add(CD);
            edges.Add(DA);
            edges.Add(AE);
            edges.Add(EH);
            edges.Add(HG);
            edges.Add(GC);
            edges.Add(GF);
            edges.Add(FE);
            edges.Add(DH);
            edges.Add(BF);

            faces.Add(new Face(new List<int>() { 0, 1, 2, 3 }));
            faces.Add(new Face(new List<int>() { 0, 3, 7, 4 }));
            faces.Add(new Face(new List<int>() { 2, 3, 7, 6 }));
            faces.Add(new Face(new List<int>() { 1, 2, 6, 5 }));
            faces.Add(new Face(new List<int>() { 0, 1, 5, 4 }));
            faces.Add(new Face(new List<int>() { 4, 5, 6, 7 }));
        }

        public void MakeTetraedr(Point3D center, double R, out List<Point3D> points, out List<Edge> edges, out List<Face> faces)
        {
            points = new List<Point3D>();
            edges = new List<Edge>();
            faces = new List<Face>();

            var pred_points = new List<Point3D>();
            var pred_edges = new List<Edge>();
            var pred_faces = new List<Face>();
            MakeGeksaedr(center, R, out pred_points, out pred_edges, out pred_faces);

            var A = pred_points[7]; //new Point3D(center.X + center.X / 8, center.Y, center.Z);//pred_points[0];
            var B = pred_points[0];
            var C = pred_points[2];
            var D = pred_points[3];

            points.Add(A);
            points.Add(B);
            points.Add(C);
            points.Add(D);

            var AB = new Edge(0, 1);
            var AC = new Edge(0, 2);
            var AD = new Edge(0, 3);
            var BD = new Edge(1, 3);
            var DC = new Edge(3, 2);
            var BC = new Edge(1, 2);

            edges.Add(AB);
            edges.Add(AC);
            edges.Add(AD);
            edges.Add(BD);
            edges.Add(DC);
            edges.Add(BC);

            faces.Add(new Face(new List<int>() { 0, 1, 2 }));
            faces.Add(new Face(new List<int>() { 0, 1, 3 }));
            faces.Add(new Face(new List<int>() { 0, 2, 3 }));
            faces.Add(new Face(new List<int>() { 1, 2, 3 }));

        }
        public void MakeOktaedr(Point3D center, double R, out List<Point3D> points, out List<Edge> edges, out List<Face> faces)
        {
            double X, Y;
            var pred_points = new List<Point3D>();
            var pred_edges = new List<Edge>();
            var pred_faces = new List<Face>();
            MakeGeksaedr(center, R, out pred_points, out pred_edges, out pred_faces);


            points = new List<Point3D>();
            edges = new List<Edge>();
            faces = new List<Face>();

            //координаты точки A
            var A = new Point3D(center.X, center.Y, center.Z + R);

            //координаты точки B
            X = (pred_points[0].X + pred_points[3].X) / 2.0;
            Y = (pred_points[0].Y + pred_points[3].Y) / 2.0;

            var B = new Point3D(X, Y, center.Z);

            //координаты точки С
            X = (pred_points[3].X + pred_points[2].X) / 2.0;
            Y = (pred_points[3].Y + pred_points[2].Y) / 2.0;

            var C = new Point3D(X, Y, center.Z);

            //координаты точки D
            X = (pred_points[2].X + pred_points[1].X) / 2.0;
            Y = (pred_points[2].Y + pred_points[1].Y) / 2.0;

            var D = new Point3D(X, Y, center.Z);

            //координаты точки E
            X = (pred_points[1].X + pred_points[0].X) / 2.0;
            Y = (pred_points[1].Y + pred_points[0].Y) / 2.0;

            var E = new Point3D(X, Y, center.Z);

            //координаты точки F
            var F = new Point3D(center.X, center.Y, center.Z - R);

            points.Add(A);
            points.Add(B);
            points.Add(C);
            points.Add(D);
            points.Add(E);
            points.Add(F);

            var AB = new Edge(0, 1);
            var AC = new Edge(0, 2);
            var AD = new Edge(0, 3);
            var AE = new Edge(0, 4);
            var BC = new Edge(1, 2);
            var CD = new Edge(2, 3);
            var DE = new Edge(3, 4);
            var EB = new Edge(4, 1);
            var BF = new Edge(1, 5);
            var CF = new Edge(2, 5);
            var DF = new Edge(3, 5);
            var EF = new Edge(4, 5);

            edges.Add(AB);
            edges.Add(AC);
            edges.Add(AD);
            edges.Add(AE);
            edges.Add(BC);
            edges.Add(CD);
            edges.Add(DE);
            edges.Add(EB);
            edges.Add(BF);
            edges.Add(CF);
            edges.Add(DF);
            edges.Add(EF);

            faces.Add(new Face(new List<int>() { 0, 1, 2 }));
            faces.Add(new Face(new List<int>() { 0, 2, 3 }));
            faces.Add(new Face(new List<int>() { 0, 3, 4 }));
            faces.Add(new Face(new List<int>() { 0, 1, 4 }));

            faces.Add(new Face(new List<int>() { 1, 2, 5 }));
            faces.Add(new Face(new List<int>() { 2, 3, 5 }));
            faces.Add(new Face(new List<int>() { 3, 4, 5 }));
            faces.Add(new Face(new List<int>() { 1, 4, 5 }));
        }

        private List<Point3D> func(List<Point3D> inp, double[,] mat)
        {
            List<Point3D> result = new List<Point3D>();
            for (int p = 0; p < inp.Count(); p++)
            {
                double[] pt = new double[4] { inp[p].X, inp[p].Y, inp[p].Z, 1 };
                double[] res = new double[4] { 0, 0, 0, 0 };

                for (int i = 0; i < 4; i++)
                    for (int j = 0; j < 4; j++)
                        res[i] += pt[j] * mat[j, i];
                    
                Point3D po = new Point3D(res[0], res[1], res[2]);
                result.Add(po);
            }
            return result;
        }

        //Смещение по x y z в афинных координатах
        private List<Point3D> dXdY(List<Point3D> inp, double dx = 0, double dy = 0, double dz = 0)
        {
            double[,] mat = new double[4, 4] { { 1, 0, 0, 0 }, { 0, 1, 0, 0 }, { 0, 0, 1, 0 }, { dx, dy, dz, 1 } };
            return (func(inp, mat));
        }

        //Поворот вокруг точки, ось Х. Базовый угол = Пи/2
        private List<Point3D> Povorot_tchk_X(List<Point3D> inp, Point3D tchk, double angle = Math.PI / 2)
        {
            List<Point3D> res = inp;

            res = dXdY(res, -tchk.X, -tchk.Y, -tchk.Z);
            double[,] mat = new double[4, 4] { { 1, 0, 0, 0 }, { 0, Math.Cos(angle), Math.Sin(angle), 0 }, { 0, -Math.Sin(angle), Math.Cos(angle), 0 }, { 0, 0, 0, 1 } };
            res = func(res, mat);
            res = dXdY(res, tchk.X, tchk.Y, tchk.Z);

            return res;
        }

        //Поворот вокруг точки, ось Z. Базовый угол = Пи/2
        private List<Point3D> Povorot_tchk_Z(List<Point3D> inp, Point3D tchk, double angle = Math.PI / 2)
        {
            List<Point3D> res = inp;

            res = dXdY(res, -tchk.X, -tchk.Y, -tchk.Z);
            double[,] mat = new double[4, 4] { { Math.Cos(angle), Math.Sin(angle), 0, 0 }, { -Math.Sin(angle), Math.Cos(angle), 0, 0 }, { 0, 0, 1, 0 }, { 0, 0, 0, 1 } };
            res = func(res, mat);
            res = dXdY(res, tchk.X, tchk.Y, tchk.Z);

            return res;
        }

        private List<Point3D> Povorot_tchk_Y(List<Point3D> inp, Point3D tchk, double angle = Math.PI / 2)
        {
            List<Point3D> res = inp;

            res = dXdY(res, -tchk.X, -tchk.Y, -tchk.Z);
            double[,] mat = new double[4, 4] { { Math.Cos(angle), 0, -Math.Sin(angle), 0 }, { 0, 1, 0, 0 }, { Math.Sin(angle), 0, Math.Cos(angle), 0 }, { 0, 0, 0, 1 } };
            res = func(res, mat);
            res = dXdY(res, tchk.X, tchk.Y, tchk.Z);

            return res;
        }

        private List<Point3D> MSHTB_tchk(List<Point3D> inp, Point3D tchk, double a = 1, double b = 1, double c = 1)
        {
            List<Point3D> res = inp;

            res = dXdY(res, -tchk.X, -tchk.Y, -tchk.Z);
            double[,] mat = new double[4, 4] { { a, 0, 0, 0 }, { 0, b, 0, 0 }, { 0, 0, c, 0 }, { 0, 0, 0, 1 } };
            res = func(res, mat);
            res = dXdY(res, tchk.X, tchk.Y, tchk.Z);

            return res;
        }

        //Повернуть объект вокруг прямой, заданной двумя точками на угол...
        private List<Point3D> Fig_aroundLine(List<Point3D> inp, Point3D tchk1, Point3D tchk, double angle = Math.PI / 2)
        {
            List<Point3D> res = inp;
            res = dXdY(res, -tchk1.X, -tchk1.Y, -tchk1.Z);
            double l = tchk.X - tchk1.X; //вычисляем вектор
            double m = tchk.Y - tchk1.Y;
            double n = tchk.Z - tchk1.Z;

            double mv = Math.Sqrt(l * l + m * m + n * n); //вычисляем модуль вектора
            l /= mv;//получаем единичный вектор
            m /= mv;
            n /= mv;

            double[,] mat = new double[4, 4] {
                { l*l + Math.Cos(angle) * (1 - l*l), l*(1 - Math.Cos(angle))*m + n*Math.Sin(angle), l*(1 - Math.Cos(angle))*n - m*Math.Sin(angle), 0 },
                { l*(1 - Math.Cos(angle))*m - n*Math.Sin(angle), m*m + Math.Cos(angle) * (1 - m*m), m*(1-Math.Cos(angle))*n + l*Math.Sin(angle), 0 },
                { l*(1 - Math.Cos(angle))*n + m*Math.Sin(angle), m*(1-Math.Cos(angle))*n - l*Math.Sin(angle), n*n + Math.Cos(angle) * (1 - n*n), 0 },
                { 0, 0, 0, 1 }
            };
            res = func(res, mat);
            res = dXdY(res, tchk1.X, tchk1.Y, tchk1.Z);
            return res;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            List<Point3D> points = new List<Point3D>();
            List<Edge> edges = new List<Edge>();
            List<Face> faces = new List<Face>();
            MakeGeksaedr(new Point3D(0, 0, 0), int.Parse(textBox1.Text), out points, out edges, out faces);
            figures.Add(new Figure(points, edges, faces));
            var figure = figures[figures.Count - 1];
            draw();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            List<Point3D> points = new List<Point3D>();
            List<Edge> edges = new List<Edge>();
            List<Face> faces = new List<Face>();
            MakeTetraedr(new Point3D(0, 0, 0), int.Parse(textBox1.Text), out points, out edges, out faces);
            figures.Add(new Figure(points, edges, faces));

            var figure = figures[figures.Count - 1];
            figure.Points = dXdY(figure.Points, -270, 300, 350);
            for (int i = 0; i < figure.Points.Count; i++)
            {
                figures[figures.Count - 1].Points[i].X = figure.Points[i].X;
                figures[figures.Count - 1].Points[i].Y = figure.Points[i].Y;
                figures[figures.Count - 1].Points[i].Z = figure.Points[i].Z;
            }

            draw();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            List<Point3D> points = new List<Point3D>();
            List<Edge> edges = new List<Edge>();
            List<Face> faces = new List<Face>();
            MakeOktaedr(new Point3D(0, 0, 0), int.Parse(textBox1.Text), out points, out edges, out faces);
            figures.Add(new Figure(points, edges, faces));

            var figure = figures[figures.Count - 1];
            figure.Points = dXdY(figure.Points, -270, 300, 350);
            for (int i = 0; i < figure.Points.Count; i++)
            {
                figures[figures.Count - 1].Points[i].X = figure.Points[i].X;
                figures[figures.Count - 1].Points[i].Y = figure.Points[i].Y;
                figures[figures.Count - 1].Points[i].Z = figure.Points[i].Z;
            }

            draw();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            figures.Clear();
            gr.Clear(Color.White);
            pictureBox1.Controls.Clear();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var figure = figures[figures.Count - 1];
            figure.Points = dXdY(figure.Points, -int.Parse(textBox2.Text), int.Parse(textBox3.Text), int.Parse(textBox4.Text));
            for (int i = 0; i < figure.Points.Count; i++)
            {
                figures[figures.Count - 1].Points[i].X = figure.Points[i].X;
                figures[figures.Count - 1].Points[i].Y = figure.Points[i].Y;
                figures[figures.Count - 1].Points[i].Z = figure.Points[i].Z;
            }
            draw();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            var figure = figures[figures.Count - 1];
            double sumX = 0;
            double sumY = 0;
            double sumZ = 0;
            foreach (var p in figure.Points)
            {
                sumX += p.X;
                sumY += p.Y;
                sumZ += p.Z;
            }
            int cnt = figure.Points.Count;
            figure.Points = MSHTB_tchk(figure.Points, new Point3D(sumX / cnt, sumY / cnt, sumZ / cnt), double.Parse(textBox5.Text), double.Parse(textBox6.Text), double.Parse(textBox7.Text));
            for (int i = 0; i < figure.Points.Count; i++)
            {
                figures[figures.Count - 1].Points[i].X = figure.Points[i].X;
                figures[figures.Count - 1].Points[i].Y = figure.Points[i].Y;
                figures[figures.Count - 1].Points[i].Z = figure.Points[i].Z;
            }
            draw();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            var figure = figures[figures.Count - 1];
            double sumX = 0;
            double sumY = 0;
            double sumZ = 0;
            foreach (var p in figure.Points)
            {
                sumX += p.X;
                sumY += p.Y;
                sumZ += p.Z;
            }
            int cnt = figure.Points.Count;
            Point3D center = new Point3D(sumX / cnt, sumY / cnt, sumZ / cnt);

            figure.Points = Povorot_tchk_Y(figure.Points, new Point3D(sumX / cnt, sumY / cnt, sumZ / cnt), int.Parse(textBox8.Text) * Math.PI / 180);
            for (int i = 0; i < figure.Points.Count; i++)
            {
                figures[figures.Count - 1].Points[i].X = figure.Points[i].X;
                figures[figures.Count - 1].Points[i].Y = figure.Points[i].Y;
                figures[figures.Count - 1].Points[i].Z = figure.Points[i].Z;
            }
            draw();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            var figure = figures[figures.Count - 1];
            double sumX = 0;
            double sumY = 0;
            double sumZ = 0;
            foreach (var p in figure.Points)
            {
                sumX += p.X;
                sumY += p.Y;
                sumZ += p.Z;
            }
            int cnt = figure.Points.Count;
            Point3D center = new Point3D(sumX / cnt, sumY / cnt, sumZ / cnt);

            figure.Points = Povorot_tchk_X(figure.Points, new Point3D(sumX / cnt, sumY / cnt, sumZ / cnt), int.Parse(textBox8.Text) * Math.PI / 180);
            for (int i = 0; i < figure.Points.Count; i++)
            {
                figures[figures.Count - 1].Points[i].X = figure.Points[i].X;
                figures[figures.Count - 1].Points[i].Y = figure.Points[i].Y;
                figures[figures.Count - 1].Points[i].Z = figure.Points[i].Z;
            }
            draw();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            var figure = figures[figures.Count - 1];
            double sumX = 0;
            double sumY = 0;
            double sumZ = 0;
            foreach (var p in figure.Points)
            {
                sumX += p.X;
                sumY += p.Y;
                sumZ += p.Z;
            }
            int cnt = figure.Points.Count;
            Point3D center = new Point3D(sumX / cnt, sumY / cnt, sumZ / cnt);

            figure.Points = Povorot_tchk_Z(figure.Points, new Point3D(sumX / cnt, sumY / cnt, sumZ / cnt), int.Parse(textBox8.Text) * Math.PI / 180);
            for (int i = 0; i < figure.Points.Count; i++)
            {
                figures[figures.Count - 1].Points[i].X = figure.Points[i].X;
                figures[figures.Count - 1].Points[i].Y = figure.Points[i].Y;
                figures[figures.Count - 1].Points[i].Z = figure.Points[i].Z;
            }
            draw();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            var figure = figures[figures.Count - 1];

            double sumX = 0;
            double sumY = 0;
            double sumZ = 0;
            foreach (var p in figure.Points)
            {
                sumX += p.X;
                sumY += p.Y;
                sumZ += p.Z;
            }
            int cnt = figure.Points.Count;
            Point3D center = new Point3D(sumX / cnt, sumY / cnt, sumZ / cnt);

            figure.Points = dXdY(figure.Points, center.X, -center.Y, -center.Z);
            double[,] mat = new double[4, 4] { { 1, 0, 0, 0 }, { 0, 1, 0, 0 }, { 0, 0, -1, 0 }, { 0, 0, 0, 1 } };
            figure.Points = func(figure.Points, mat);
            figure.Points = dXdY(figure.Points, -center.X, center.Y, center.Z);

            draw();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            var figure = figures[figures.Count - 1];

            double sumX = 0;
            double sumY = 0;
            double sumZ = 0;
            foreach (var p in figure.Points)
            {
                sumX += p.X;
                sumY += p.Y;
                sumZ += p.Z;
            }
            int cnt = figure.Points.Count;
            Point3D center = new Point3D(sumX / cnt, sumY / cnt, sumZ / cnt);

            figure.Points = dXdY(figure.Points, center.X, -center.Y, -center.Z);
            double[,] mat = new double[4, 4] { { 1, 0, 0, 0 }, { 0, -1, 0, 0 }, { 0, 0, 1, 0 }, { 0, 0, 0, 1 } };
            figure.Points = func(figure.Points, mat);
            figure.Points = dXdY(figure.Points, -center.X, center.Y, center.Z);

            draw();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            var figure = figures[figures.Count - 1];

            double sumX = 0;
            double sumY = 0;
            double sumZ = 0;
            foreach (var p in figure.Points)
            {
                sumX += p.X;
                sumY += p.Y;
                sumZ += p.Z;
            }
            int cnt = figure.Points.Count;
            Point3D center = new Point3D(sumX / cnt, sumY / cnt, sumZ / cnt);

            figure.Points = dXdY(figure.Points, -center.X, -center.Y, -center.Z);
            double[,] mat = new double[4, 4] { { -1, 0, 0, 0 }, { 0, 1, 0, 0 }, { 0, 0, 1, 0 }, { 0, 0, 0, 1 } };
            figure.Points = func(figure.Points, mat);
            figure.Points = dXdY(figure.Points, center.X, center.Y, center.Z);

            draw();
        }

        private void button13_Click(object sender, EventArgs e)
        {
            var figure = figures[figures.Count - 1];
            figure.Points = Fig_aroundLine(figure.Points, new Point3D(double.Parse(textBox9.Text), double.Parse(textBox11.Text), double.Parse(textBox13.Text)), new Point3D(double.Parse(textBox10.Text), double.Parse(textBox12.Text), double.Parse(textBox14.Text)), int.Parse(textBox15.Text) * Math.PI / 180);
            for (int i = 0; i < figure.Points.Count; i++)
            {
                figures[figures.Count - 1].Points[i].X = figure.Points[i].X;
                figures[figures.Count - 1].Points[i].Y = figure.Points[i].Y;
                figures[figures.Count - 1].Points[i].Z = figure.Points[i].Z;
            }
            //figures[figures.Count - 1] = figure;
            draw();
        }

        private void button14_Click(object sender, EventArgs e)
        {
            showCoords = !showCoords;
            labels.Clear();
            pictureBox1.Controls.Clear();
            gr.Clear(Color.White);
            draw();
        }


        delegate double Func(double x, double y);
        //Диапазоны и число шагов с функцией координаты Z
        private Figure Func_3D(double x0, double x1, double y0, double y1, int steps, Func fun)
        {
            List<Point3D> res_p = new List<Point3D>();
            List<Edge> res_e = new List<Edge>();
            List<Face> res_f = new List<Face>();

            double stepX = (x1 - x0) / steps;
            double stepY = (y1 - y0) / steps;
            double curx = x0;
            double cury = y0;

            for (int i = 0; i < steps; i++)
            {
                cury = y0;
                for (int j = 0; j < steps; j++)
                {
                    res_p.Add(new Point3D(curx, cury, fun(curx, cury)));
                    if (j != 0)
                    {
                        res_e.Add(new Edge(i * steps + j - 1, i * steps + j));
                    }
                    cury += stepY;
                }
                curx += stepX;
            }

            for (int i = 0; i < steps - 1; i++)
            {
                for (int j = 0; j < steps; j++)
                {
                    res_e.Add(new Edge(i * steps + j, i * steps + j + steps));
                }
            }

            for (int cs = 0; cs < steps - 1; cs++)
                for (int i = 0; i < steps - 1; i++)
                    res_f.Add(new Face(new List<int>() { i * cs, i * cs + 1, i * (cs + 1), i * (cs + 1) + 1 }));

            Figure res = new Figure(res_p, res_e, res_f);
            return res;
        }
        private Figure Rotate(List<Point3D> points, string axis, int c)
        {
            List<Point3D> res_points = new List<Point3D>();
            for (int j = 0; j < points.Count; j++)
                res_points.Add(points[j]);

            var angle = 360.0 / (double)(c);

            //вычисляем координаты двух точек, лежащих на заданной оси
            Point3D a = new Point3D(0, 0, 0);
            Point3D b;
            if (axis == "x" || axis == "X")
                b = new Point3D(1, 0, 0);
            else if (axis == "y" || axis == "Y")
                b = new Point3D(0, 1, 0);
            else
                b = new Point3D(0, 0, 1);

            for (int i = 0; i < c; i++)
            {
                var new_points = Fig_aroundLine(points, a, b, angle * Math.PI / 180); //поворот относительно оси
                for (int j = 0; j < new_points.Count; j++)
                    res_points.Add(new_points[j]);
                angle += 360.0 / (double)(c);

            }

            List<Edge> res_edges = new List<Edge>();
            for (int i = 0; i < c; i++) // по кругу
            {
                for (int j = 0; j < points.Count - 1; j++) // вниз
                {
                    res_edges.Add(new Edge(i * points.Count + j, i * points.Count + j + 1));
                }
            }
            for (int i = 0; i < c; i++) // по кругу
            {
                for (int j = 0; j < points.Count; j++) // вниз
                {
                    res_edges.Add(new Edge(i * points.Count + j, (i + 1) * points.Count + j));
                }
            }
            //                res_edges.Add(new Edge(i, i + points.Count));
            //                    res_edges.Add(new Edge(j + points.Count * i, j + points.Count * i + 1));


            List<Face> res_faces = new List<Face>();
            for (int i = 0; i < c; i++)
            {
                for (int j = 0; j < points.Count; j++)
                {
                    Face face = new Face();
                    face.Points.Add(j * i);
                    face.Points.Add(j * (i + 1));
                    face.Points.Add(j * (i + 1) + 1);
                    face.Points.Add(j * i + 1);
                    res_faces.Add(face);
                }
            }

            return new Figure(res_points, res_edges, res_faces);
            /*for (int i = 0; i < steps - 1; i++)
            {
                for (int j = 0; j < steps; j++)
                {
                    res_e.Add(new Edge(i * steps + j, i * steps + j + steps));
                }
            }

            for (int cs = 0; cs < steps - 1; cs++)
                for (int i = 0; i < steps - 1; i++)
                    res_f.Add(new Face(new List<int>() { i * cs, i * cs + 1, i * (cs + 1), i * (cs + 1) + 1 }));*/
        }

        private void button16_Click(object sender, EventArgs e)
        {
            double x0 = double.Parse(textBox16.Text);
            double x1 = double.Parse(textBox17.Text);
            double y0 = double.Parse(textBox18.Text);
            double y1 = double.Parse(textBox19.Text);
            int step = int.Parse(textBox20.Text);

            if (comboBox1.Text == "f(x, y) = sin(x)")
                figures.Add(Func_3D(x0, x1, y0, y1, step, (double x, double y) => { return Math.Sin(x); }));
            if (comboBox1.Text == "f(x, y) = sin(x) * sin(y)")
                figures.Add(Func_3D(x0, x1, y0, y1, step, (double x, double y) => { return Math.Sin(x) * Math.Sin(y); }));
            if (comboBox1.Text == "f(x, y) = sin(√(x^2 + y^2))")
                figures.Add(Func_3D(x0, x1, y0, y1, step, (double x, double y) => { return Math.Sin(Math.Pow(x * x + y * y, 1 / 2.0)); }));
            if (comboBox1.Text == "f(x, y) = 1 / (1 + x^2) + 1 / (1 + y^2)")
                figures.Add(Func_3D(x0, x1, y0, y1, step, (double x, double y) => { return 1 / (1 + x * x) + 1 / (1 + y * y); }));

            draw();
        }

        private void button15_Click(object sender, EventArgs e)
        {
            String path = "";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                path = saveFileDialog1.FileName;
            StreamWriter sw = new StreamWriter(path);

            Figure figure = figures[figures.Count - 1];
            StringBuilder sb = new StringBuilder("");

            // Точки
            for (int i = 0; i < figure.Points.Count - 1; i++)
                sb.Append($"{figure.Points[i].X} {figure.Points[i].Y} {figure.Points[i].Z};");
            sb.Append($"{figure.Points[figure.Points.Count - 1].X} {figure.Points[figure.Points.Count - 1].Y} {figure.Points[figure.Points.Count - 1].Z}");
            sw.WriteLine(sb.ToString());

            // Рёбра
            sb = new StringBuilder("");
            for (int i = 0; i < figure.Edges.Count - 1; i++)
                sb.Append($"{figure.Edges[i].First} {figure.Edges[i].Second};");
            sb.Append($"{figure.Edges[figure.Edges.Count - 1].First} {figure.Edges[figure.Edges.Count - 1].Second}");
            sw.WriteLine(sb.ToString());

            // Грани
            sb = new StringBuilder("");
            for (int i = 0; i < figure.Faces.Count - 1; i++)
            {
                for (int j = 0; j < figure.Faces[i].Points.Count; j++)
                    sb.Append($"{figure.Faces[i].Points[j]} ");
                sb.Remove(sb.Length - 1, 1);
                sb.Append(';');
            }
            sb.Remove(sb.Length - 1, 1);
            sw.WriteLine(sb.ToString());

            sw.Close();
        }

        private void button17_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
                figures.Add(new Figure(openFileDialog1.FileName));
            draw();
        }
        List<Point> points_c = new List<Point>();
        int vcnt = 0;
        private void pictureBox2_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (vcnt == 0)
                {
                    points_c.Clear();
                    gr1.Clear(Color.White);
                }
                vcnt++;
                if (vcnt == 1)
                {
                    points_c.Add(e.Location);
                    gr1.FillRectangle(Brushes.Black, e.X, e.Y, 1, 1);
                }
                else
                {
                    points_c.Add(e.Location);
                    gr1.DrawLine(new Pen(Color.Black), points_c[points_c.Count - 1], points_c[points_c.Count - 2]);
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                vcnt = 0;
                points_c.Add(e.Location);
                gr1.DrawLine(new Pen(Color.Black), points_c[points_c.Count - 1], points_c[points_c.Count - 2]);
            }
        }

        private void button18_Click(object sender, EventArgs e)
        {
            double dx = 0;
            double dy = 0;
            int minx = int.MaxValue;
            int minind = 0;
            for (int i = 0; i < points_c.Count; i++)
            {
                if (points_c[i].X < minx)
                {
                    minx = points_c[i].X;
                    minind = i;
                }
            }

            dx = points_c[minind].X;
            dy = points_c[minind].Y;

            List<Point3D> points = new List<Point3D>();
            Figure figure = new Figure();
            int seg = 25;

            if (radioButton1.Checked)
            {
                // обнуляем y
                for (int i = 0; i < points_c.Count; i++)
                    points.Add(new Point3D(points_c[i].X - dx, 0.0, points_c[i].Y - dy));
                figure = Rotate(points, "X", seg);
            }
            if (radioButton2.Checked)
            {
                // обнуляем x
                for (int i = 0; i < points_c.Count; i++)
                    points.Add(new Point3D(0.0, points_c[i].X - dx, points_c[i].Y - dy));
                figure = Rotate(points, "Y", seg);

            }
            if (radioButton3.Checked)
            {
                // обнуляем y
                for (int i = 0; i < points_c.Count; i++)
                    points.Add(new Point3D(points_c[i].X - dx, 0.0, points_c[i].Y - dy));
                figure = Rotate(points, "Z", seg);
            }

            figures.Add(figure);
            draw();
        }
    }



    /// <summary>
    /// Трёхмерная точка.
    /// </summary>
    public class Point3D
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public Point3D(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }

    /// <summary>
    /// Ребро.
    /// </summary>
    public class Edge
    {
        public int First { get; set; }
        public int Second { get; set; }
        public Edge(int first, int second)
        {
            First = first;
            Second = second;
        }
    }

    /// <summary>
    /// Грань
    /// </summary>
    public class Face
    {
        public List<int> Points { get; set; }

        public Face(List<int> points)
        {
            Points = points;
        }
        public Face()
        {
            Points = new List<int>();
        }
    }

    public class Figure
    {
        public List<Point3D> Points { get; set; }
        public List<Edge> Edges { get; set; }
        public List<Face> Faces { get; set; }
        public Figure(List<Point3D> points, List<Edge> edges, List<Face> faces)
        {
            Points = points;
            Edges = edges;
            Faces = faces;
        }
        public Figure()
        {
            Points = new List<Point3D>();
            Edges = new List<Edge>();
            Faces = new List<Face>();
        }
        public Figure(String path)
        {
            StreamReader sr = new StreamReader(path);

            String line = sr.ReadLine(); // Точки
            var points_s = line.Split(';');
            Points = new List<Point3D>();
            for (int i = 0; i < points_s.Length; i++)
            {
                var coords = points_s[i].Split(' ');
                Points.Add(new Point3D(double.Parse(coords[0]), double.Parse(coords[1]), double.Parse(coords[2])));
            }

            line = sr.ReadLine(); // Ребра
            var edges_s = line.Split(';');
            Edges = new List<Edge>();
            for (int i = 0; i < edges_s.Length; i++)
            {
                var edges = edges_s[i].Split(' ');
                Edges.Add(new Edge(int.Parse(edges[0]), int.Parse(edges[1])));
            }

            line = sr.ReadLine(); // Грани
            var faces_s = line.Split(';');
            Faces = new List<Face>();
            for (int i = 0; i < faces_s.Length; i++)
            {
                var face = faces_s[i].Split(' ');
                Face curr_face = new Face();
                for (int j = 0; j < face.Length; j++)
                    curr_face.Points.Add(int.Parse(face[j]));
                Faces.Add(curr_face);
            }

            sr.Close();
        }
    }
}


/*
 * List<Point3D> points2 = dXdY(points, 0, 0, 100);
           for (int i = 0; i < points2.Count; i++)
           {
               points[i].X = points2[i].X;
               points[i].Y = points2[i].Y;
               points[i].Z = points2[i].Z;
}
*/

/*
foreach (var edge in edges)
{
    List<Point3D> ps = new List<Point3D>() { edge.First, edge.Second };
    double pci = pcig * Math.PI / 180;
    double phi = phig * Math.PI / 180;
    double[,] mat = new double[4, 4] {
                    { Math.Cos(pci), Math.Sin(phi)*Math.Sin(pci), 0, 0 },
                    { 0, Math.Cos(phi), 0, 0 },
                    { Math.Sin(pci), -Math.Sin(phi)*Math.Cos(pci), 0, 0 },
                    { 0, 0, 0, 1 } };
    List<Point3D> r = func(ps, mat);
    gr.DrawLine(new Pen(Brushes.Black), new Point((int)r[0].X, (int)r[0].Y), new Point((int)r[1].X, (int)r[1].Y));

    //int new_x1 =(int)(edge.First.X - edge.First.Y);
    //int new_y1 = (int)((edge.First.X + edge.First.Y) / 2 + edge.First.Z);
    //int new_x2 = (int)(edge.Second.X - edge.Second.Y);
    //int new_y2 = (int)((edge.Second.X + edge.Second.Y) / 2 + edge.Second.Z);
    //gr.DrawLine(new Pen(Brushes.Black), new Point(new_x1, new_y1), new Point(new_x2, new_y2));
    //int new_x = (x - y);
    //int new_y = (x + y) / 2 + z;
}
*/

/*
 

                List<Edge> new_edges = new List<Edge>();
                for (int j = 0; i < new_points.Count - 1; i++)
                    new_edges.Add(new Edge(i, i + 1));
                new_edges.Add(new Edge(new_points.Count - 1, 0));
                //res.Add(new Figure(new_points, new_edges));
                points = new_points;
 
 */