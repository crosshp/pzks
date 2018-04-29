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
    partial class GSystem : Panel
    {
        int[,] Lines;
        NodeS[] Nodes;
        int Nodes_Count = 0;

        public void LinesUpdate(NodeS[] Nodes_in, int[,] Lines_in, int count)
        {
            Lines = Lines_in;
            Nodes = Nodes_in;
            Nodes_Count = count;
        }
        public override void Refresh()
        {
            base.Refresh();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            double alfa;
            Point ps, pe, ptxt;
            Graphics g = e.Graphics;
            for (int i = 0; i < Nodes_Count; i++)
                for (int j = i; j < Nodes_Count; j++)
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
                        g.DrawLine(new Pen(Color.Blue, 2), ps, pe);

                        Point p_mid = new Point((pe.X + ps.X) / 2, (ps.Y + pe.Y) / 2);
                        int length = (int)Math.Sqrt((pe.X - ps.X) * (pe.X - ps.X) + (pe.Y - ps.Y) * (pe.Y - ps.Y));

                        if (ps1.X >= pe1.X)
                            ptxt = new Point((int)(p_mid.X - Math.Sin(alfa) * length / 30), (int)(p_mid.Y + Math.Cos(alfa) * length / 30));
                        else
                            ptxt = new Point((int)(p_mid.X + Math.Sin(alfa) * length / 30), (int)(p_mid.Y - Math.Cos(alfa) * length / 30));
                        
                        g.FillRectangle(Brushes.White, new Rectangle(ptxt.X - 2, ptxt.Y - 2, 20, 20));
                        g.DrawString(Lines[i, j].ToString(), new Font("Times New Roman", 12), Brushes.Red, ptxt);

                        
                    }
                }
        }
    }
}
