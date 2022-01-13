using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Lab2
{
    public partial class Form1 : Form
    {
        private static double mu = 3.98603 * Math.Pow(10, 14);
        private static int days = 86400;
        private static int h = 5;
        
        public struct Vector3
        {
            public double X, Y, Z;

            public Vector3(double x, double y, double z)
            {
                X = x;
                Y = y;
                Z = z;
            }
        }

        public struct VectorX
        {
            public Vector3 X;

            public Vector3 V;

            public VectorX(Vector3 x, Vector3 v)
            {
                X = x;
                V = v;
            }
        }

        public Vector3 GetA(Vector3 x)
        {
            Vector3 A;
            double r = Math.Sqrt(x.X * x.X + x.Y * x.Y + x.Z * x.Z);
            A.X = (-mu * x.X) / (r * r * r);
            A.Y = (-mu * x.Y) / (r * r * r);
            A.Z = (-mu * x.Z) / (r * r * r);
            return A;
        }


        public Form1()
        {
            InitializeComponent();
            List<VectorX> X = new List<VectorX>
                { new VectorX(new Vector3(42164000, 0, 0), new Vector3(0, 3066, 0)) };
            for (int t = h; t <= days*2; t += h)
            {
                X.Add(Eiler(X[X.Count - 1], h));
                //X.Add(RungeKutta(X[X.Count - 1], h));
            }

            int i = 0;
            for (double t = 0; t <= days*2; t += h)
            {
                chart1.Series[0].Points.AddXY(t, X[i].X.X);
                chart1.Series[1].Points.AddXY(t, X[i].X.Y);
                chart1.Series[2].Points.AddXY(t, X[i].X.Z);
                chart1.Series[3].Points.AddXY(t, X[i].V.X);
                chart1.Series[4].Points.AddXY(t, X[i].V.Y);
                chart1.Series[5].Points.AddXY(t, X[i].V.Z);
                i++;
            }
        }

        private VectorX Eiler(VectorX x, int h) => new VectorX(
            new Vector3(x.X.X + h * x.V.X, x.X.Y + h * x.V.Y, x.X.Z + h * x.V.Z),
            new Vector3(x.V.X + h * GetA(x.X).X, x.V.Y + h * GetA(x.X).Y, x.V.Z + h * GetA(x.X).Z)
        );
        
        VectorX RungeKutta(VectorX X, int h)
        {
            Vector3 a = GetA(X.X);

            VectorX k1 = new VectorX(X.V, a);

            VectorX k2 = new VectorX(new Vector3(X.V.X + k1.X.X * h / 2, X.V.Y + k1.X.Y * h / 2, X.V.Z + k1.X.Z * h / 2), 
                new Vector3(a.X + GetA(k1.X).X * h / 2, a.Y + GetA(k1.X).Y * h / 2, a.Z + GetA(k1.X).Z * h / 2));

            VectorX k3 = new VectorX(new Vector3(X.V.X + k2.X.X * h / 2, X.V.Y + k2.X.Y * h / 2, X.V.Z + k2.X.Z * h / 2),
                new Vector3(a.X + GetA(k2.X).X * h, a.Y + GetA(k2.X).Y * h, a.Z + GetA(k2.X).Z * h));

            VectorX k4 = new VectorX(new Vector3(X.V.X + k3.X.X * h / 2, X.V.Y + k3.X.Y * h / 2, X.V.Z + k3.X.Z * h / 2),
                new Vector3(a.X + GetA(k3.X).X * h, a.Y + GetA(k3.X).Y * h, a.Z + GetA(k3.X).Z * h));

            VectorX sum_k = new VectorX(new Vector3(k1.X.X + 2 * (k2.X.X + k3.X.X) + k4.X.X, 
                k1.X.Y + 2 * (k2.X.Y + k3.X.Y) + k4.X.Y, 
                k1.X.Z + 2 * (k2.X.Z + k3.X.Z) + k4.X.Z),
                
                new Vector3(k1.V.X + 2 * (k2.V.X + k3.V.X) + k4.V.X, 
                k1.V.Y + 2 * (k2.V.Y + k3.V.Y) + k4.V.Y, 
                k1.V.Z + 2 * (k2.V.Z + k3.V.Z) + k4.V.Z));
            
            return new VectorX(new Vector3(X.X.X + h / 6 * sum_k.X.X, 
                X.X.Y + h / 6 * sum_k.X.Y, 
                X.X.Z + h / 6 * sum_k.X.Z),
                
                new Vector3(X.V.X + h / 6 * sum_k.V.X, 
                X.V.Y + h / 6 * sum_k.V.Y, 
                X.V.Z + h / 6 * sum_k.V.Z));
        }
    }
}