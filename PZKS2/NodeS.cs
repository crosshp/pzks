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
    public partial class NodeS : Control
    {
        public int number;
        public int mass;

        public Color Color = Color.Blue;
        public Color ColorSelected = Color.Red;
        public Font Font1 = new Font("Times New Roman", 10);

        public NodeS(int number_in, int mass_in)
        { 
            number = number_in;
            mass = mass_in;
        }

        protected override void OnPaint(PaintEventArgs e){
            base.OnPaint(e);
            Graphics g = e.Graphics;

            if (!Focused)
            {
                g.DrawEllipse(new Pen(Color, 1), new Rectangle(1, 1, this.Width - 2, this.Height - 2));
            }
            else
            {
                g.DrawEllipse(new Pen(Color, 2), new Rectangle(1, 1, this.Width - 2, this.Height - 2));
            }
            string txt = number.ToString() + " (" + mass.ToString() + ")";
            g.DrawString(txt, Font1, Brushes.Black, new Point(5, this.Height / 2 - Font.Height / 2));
        }
    }
}
