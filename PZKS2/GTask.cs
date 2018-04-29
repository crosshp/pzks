using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PZKS2
{
    partial class GTask : Panel
    {
        int[,] Lines;
        NodeT[] Nodes;
        int Nodes_Count = 0;

        public void LinesUpdate(NodeT[] Nodes_in, int[,] Lines_in, int count)
        {
            Lines = Lines_in;
            Nodes = Nodes_in;
            Nodes_Count = count;
        }
        public override void Refresh()
        {
            base.Refresh();
        }

        protected String getWeight(int mouseX, int mouseY) {
                double alfa;
                Point ps, pe, p1, ptxt;

                for (int i = 0; i < Nodes_Count; i++)
                    for (int j = 0; j < Nodes_Count; j++)
                    {
                        if (Lines[i, j] != 0)
                        {
                            Point ps1 = new Point(Nodes[i].Location.X + Nodes[i].Width / 2, Nodes[i].Location.Y + Nodes[i].Height / 2);
                            Point pe1 = new Point(Nodes[j].Location.X + Nodes[j].Width / 2, Nodes[j].Location.Y + Nodes[j].Height / 2);

                            try
                            {
                                alfa = (double)(Math.Atan(((double)(ps1.Y - pe1.Y) / (ps1.X - pe1.X))));
                            }
                            catch (Exception) { alfa = 99999; }

                            if (ps1.X > pe1.X)
                            {
                                ps = new Point((int)(ps1.X - Math.Cos(alfa) * 20), (int)(ps1.Y - Math.Sin(alfa) * 20));
                                pe = new Point((int)(pe1.X + Math.Cos(alfa) * 20), (int)(pe1.Y + Math.Sin(alfa) * 20));
                            }
                            else
                            {
                                ps = new Point((int)(ps1.X + Math.Cos(alfa) * 20), (int)(ps1.Y + Math.Sin(alfa) * 20));
                                pe = new Point((int)(pe1.X - Math.Cos(alfa) * 20), (int)(pe1.Y - Math.Sin(alfa) * 20));
                            }

                            Point p_mid = new Point((pe.X + ps.X) / 2, (ps.Y + pe.Y) / 2);
                            int length = (int)Math.Sqrt((pe.X - ps.X) * (pe.X - ps.X) + (pe.Y - ps.Y) * (pe.Y - ps.Y));

                            if (ps1.X >= pe1.X)
                            {
                                p1 = new Point((int)(p_mid.X - Math.Sin(alfa) * length / 4), (int)(p_mid.Y + Math.Cos(alfa) * length / 4));
                                ptxt = new Point((int)(p_mid.X - Math.Sin(alfa) * length / 30), (int)(p_mid.Y + Math.Cos(alfa) * length / 30));
                            }
                            else
                            {
                                p1 = new Point((int)(p_mid.X + Math.Sin(alfa) * length / 4), (int)(p_mid.Y - Math.Cos(alfa) * length / 4));
                                ptxt = new Point((int)(p_mid.X + Math.Sin(alfa) * length / 30), (int)(p_mid.Y - Math.Cos(alfa) * length / 30));
                            }

                            Point p_arr, p_arr1, p_arr2;
                            double alfa2 = 0;
                            try
                            {
                                alfa2 = (double)(Math.Atan((double)(ps.Y - pe.Y) / (ps.X - pe.X)));
                            }
                            catch (Exception) { alfa = 99999; }

                            if (p1.X > pe.X)
                            {
                                p_arr = new Point((int)(pe.X), (int)(pe.Y));
                                p_arr1 = new Point((int)(pe.X + Math.Cos(alfa2 + 0.4) * 20), (int)(pe.Y + Math.Sin(alfa2 + 0.4) * 20));
                                p_arr2 = new Point((int)(pe.X + Math.Cos(alfa2 - 0.4) * 20), (int)(pe.Y + Math.Sin(alfa2 - 0.4) * 20));
                            }
                            else
                            {
                                p_arr = new Point((int)(pe.X), (int)(pe.Y));
                                p_arr1 = new Point((int)(pe.X - Math.Cos(alfa2 + 0.4) * 20), (int)(pe.Y - Math.Sin(alfa2 + 0.4) * 20));
                                p_arr2 = new Point((int)(pe.X - Math.Cos(alfa2 - 0.4) * 20), (int)(pe.Y - Math.Sin(alfa2 - 0.4) * 20));
                            }
                        if (inRect(mouseX, mouseY, ptxt.X - 2, ptxt.Y - 2))
                        {
                            return Lines[i, j].ToString();
                        }
                            //g.FillRectangle(Brushes.Black, new Rectangle(ptxt.X - 2, ptxt.Y - 2, 20, 20));

                            //g.DrawString(Lines[i, j].ToString(), new Font("Times New Roman", 12), Brushes.Red, ptxt);
                        }
                    }
            return "0";
        }

        private bool inRect(int mouseX, int mouseY, int rectX, int rectY) {

            return false;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            double alfa;
            Point ps, pe, p1, ptxt;
            Graphics g = e.Graphics;
            for (int i = 0; i < Nodes_Count; i++)
                for (int j = 0; j < Nodes_Count; j++)
                {
                    if (Lines[i, j] != 0)
                    {
                        Point ps1 = new Point(Nodes[i].Location.X + Nodes[i].Width / 2, Nodes[i].Location.Y + Nodes[i].Height / 2);
                        Point pe1 = new Point(Nodes[j].Location.X + Nodes[j].Width / 2, Nodes[j].Location.Y + Nodes[j].Height / 2);

                        try
                        {
                            alfa = (double)(Math.Atan(((double)(ps1.Y - pe1.Y) / (ps1.X - pe1.X))));
                        }
                        catch (Exception) { alfa = 99999; }

                        if (ps1.X > pe1.X)
                        {
                            ps = new Point((int)(ps1.X - Math.Cos(alfa) * 20), (int)(ps1.Y - Math.Sin(alfa) * 20));
                            pe = new Point((int)(pe1.X + Math.Cos(alfa) * 20), (int)(pe1.Y + Math.Sin(alfa) * 20));
                        }
                        else
                        {
                            ps = new Point((int)(ps1.X + Math.Cos(alfa) * 20), (int)(ps1.Y + Math.Sin(alfa) * 20));
                            pe = new Point((int)(pe1.X - Math.Cos(alfa) * 20), (int)(pe1.Y - Math.Sin(alfa) * 20));
                        }

                        Point p_mid = new Point((pe.X + ps.X) / 2, (ps.Y + pe.Y) / 2);
                        int length = (int)Math.Sqrt((pe.X - ps.X) * (pe.X - ps.X) + (pe.Y - ps.Y) * (pe.Y - ps.Y));

                        if (ps1.X >= pe1.X)
                        {
                            p1 = new Point((int)(p_mid.X - Math.Sin(alfa) * length / 4), (int)(p_mid.Y + Math.Cos(alfa) * length / 4));
                            ptxt = new Point((int)(p_mid.X - Math.Sin(alfa) * length / 30), (int)(p_mid.Y + Math.Cos(alfa) * length / 30));
                        }
                        else
                        {
                            p1 = new Point((int)(p_mid.X + Math.Sin(alfa) * length / 4), (int)(p_mid.Y - Math.Cos(alfa) * length / 4));
                            ptxt = new Point((int)(p_mid.X + Math.Sin(alfa) * length / 30), (int)(p_mid.Y - Math.Cos(alfa) * length / 30));
                        }
                        g.DrawLine(new Pen(Color.Blue, 2), ps, pe);

                        Point p_arr, p_arr1, p_arr2;
                        double alfa2 = 0;
                        try
                        {
                            alfa2 = (double)(Math.Atan((double)(ps.Y - pe.Y)/(ps.X - pe.X)));
                        }
                        catch (Exception) { alfa = 99999; }

                        if (p1.X > pe.X)
                        {
                            p_arr = new Point((int)(pe.X), (int)(pe.Y));
                            p_arr1 = new Point((int)(pe.X + Math.Cos(alfa2 + 0.4) * 20), (int)(pe.Y + Math.Sin(alfa2 + 0.4) * 20));
                            p_arr2 = new Point((int)(pe.X + Math.Cos(alfa2 - 0.4) * 20), (int)(pe.Y + Math.Sin(alfa2 - 0.4) * 20));
                        }
                        else
                        {
                            p_arr = new Point((int)(pe.X), (int)(pe.Y));
                            p_arr1 = new Point((int)(pe.X - Math.Cos(alfa2 + 0.4) * 20), (int)(pe.Y - Math.Sin(alfa2 + 0.4) * 20));
                            p_arr2 = new Point((int)(pe.X - Math.Cos(alfa2 - 0.4) * 20), (int)(pe.Y - Math.Sin(alfa2 - 0.4) * 20));
                        }
                        g.DrawLine(new Pen(Color.Blue, 2), p_arr, p_arr1);
                        g.DrawLine(new Pen(Color.Blue, 2), p_arr, p_arr2);
                        Rectangle rect = new Rectangle(ptxt.X - 2, ptxt.Y - 2, 20, 20);
                        g.FillRectangle(Brushes.White, new Rectangle(ptxt.X - 2, ptxt.Y - 2, 20, 20));
                   
                        g.DrawString(Lines[i, j].ToString(), new Font("Times New Roman", 12), Brushes.Red, ptxt);
                    }
                }
        }
    }
}
